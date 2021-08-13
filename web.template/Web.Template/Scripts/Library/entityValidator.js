import ObjectFunctions from 'library/objectfunctions';
import StringFunctions from 'library/stringfunctions';
import WebRequest from 'library/webrequest';

class EntityValidator {
    constructor(site) {
        this.definitions = {};
        this.entityPromises = [];
        this.jsonSchema = {};
        this.site = StringFunctions.safeUrl(site);
    }

    /**
     * Function to get definintion schema for a given property
     * @param {string} key - The object property name
     * @param {object} schemaProperty - The json schema property object
     * @return {object} The schema
     */
    getArrayItemSchema(key, schemaProperty) {
        let definitionSchema = schemaProperty.items;
        if (this.definitions.hasOwnProperty(key)) {
            definitionSchema = this.definitions[key];
        }
        if (definitionSchema.hasOwnProperty('$ref')) {
            let definitionType = definitionSchema.$ref;
            definitionType = definitionType.replace('#/definitions/', '');
            definitionSchema = this.jsonSchema.definitions[definitionType];
        }
        return definitionSchema;
    }

    /**
     * Function to get a default value by type
     * @param {string} type - The type
     * @return {object} the default type value
     */
    getDefaultValueByType(type) {
        let defaultTypeValue = '';
        switch (type) {
            case 'object':
                defaultTypeValue = {};
                break;
            case 'array':
                defaultTypeValue = [];
                break;
            case 'integer':
                defaultTypeValue = 0;
                break;
            case 'boolean':
                defaultTypeValue = false;
                break;
            case 'number':
                defaultTypeValue = 0;
                break;
            default:
        }
        return defaultTypeValue;
    }

    /**
     * Load CMS model
     * @param {object} objectType - The object type to return
     * @param {string} id - Id of the object type
     * @return {object} The model
     */
    loadCMSModel(objectType, id) {
        const entityUrl = `/api/cms/${objectType}/${id}`;
        return WebRequest.send(entityUrl)
            .then(response => {
                let cmsModel = {};
                if (response.ok) {
                    cmsModel = JSON.parse(response.object.ContentJSON);
                }
                return cmsModel;
            });
    }

    /**
     * Load entity model
     * @param {string} site - The site
     * @param {string} entityName - The entity to return
     * @param {string} context - The context of the entity
     * @param {string} mode - The content mode
     * @return {object} The model
     */
    loadEntityModel(site, entityName, context, mode) {
        const entityUrl = `/api/sitebuilder/model/${site}/${entityName}/${context}/${mode}`;
        return WebRequest.send(entityUrl)
            .then(response => {
                let entityModel = {};
                if (response.ok) {
                    entityModel = response.object;
                }
                return entityModel;
            });
    }

    /**
     * Function to set a default value
     * @param {object} schemaObject - The json schema property object
     * @param {object} model - The entity model data
     * @param {object} objectPath - The object path
     * @return {object} valid entity model
     */
    setDefaultValue(schemaObject, model, objectPath) {
        let validModel = Object.assign({}, model);
        const defaultValue = schemaObject.default
            ? schemaObject.default : this.getDefaultValueByType(schemaObject.type);
        validModel = ObjectFunctions.setValueByStringPath(validModel, objectPath, defaultValue);
        return validModel;
    }

    /**
     * Function to get definintion schema for a given property
     * @param {object} schemaProperty - The json schema property object
     * @param {object} model - The entity model data
     * @param {string} objectPath - The object path
     * @return {undefined}
     */
    setDefinintionSchema(schemaProperty, model, objectPath) {
        const property = schemaProperty.format.split(',')[1];
        let definitionType
            = ObjectFunctions.getValueByStringPath(model, objectPath);
        definitionType = definitionType ? definitionType : schemaProperty.default;
        const definitionSchema = this.jsonSchema.definitions[definitionType];
        this.definitions[property] = definitionSchema;
    }

    /**
     * Function to ensure entity model matches with schema
     * @param {string} key - The object property name
     * @param {object} schemaObject - The json schema property object
     * @param {object} model - The entity model data
     * @param {object} objectPath - The object path
     * @param {int} index - The index of an array item
     * @return {object} valid entity model with lookup content
     */
    setupLookupContent(key, schemaObject, model, objectPath) {
        const format = schemaObject.format.split(',')[0];
        const lookupType = schemaObject.format.split(',')[1];
        const customFormats = ['autocustomlookup', 'autocontextlookup'];
        const customMultiFormats = ['automulticontextlookup'];

        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;
        const contextValue = ObjectFunctions.getValueByStringPath(model, currentObjectPath);

        const contentKey = key.replace('Id', '');
        const contentObjectKey = objectPath ? `${objectPath}.${contentKey}` : contentKey;

        let validModel = Object.assign({}, model);
        if (customFormats.indexOf(format) > -1 && contextValue) {
            const promise = this.loadEntityModel(this.site, lookupType, contextValue, 'live')
                .then(result => {
                    validModel = ObjectFunctions.setValueByStringPath(validModel,
                        contentObjectKey, result);
                    if (ObjectFunctions.isNullOrEmpty(result)) {
                        validModel = ObjectFunctions.setValueByStringPath(validModel,
                            currentObjectPath, '');
                    }
                });
            this.entityPromises.push(promise);
        }

        if (customMultiFormats.indexOf(format) > -1 && contextValue) {
            const promise = new Promise((resolve) => {
                const itemPromises = [];
                contextValue.forEach(context => {
                    const itemPromise = this.loadEntityModel(this.site, lookupType,
                        context, 'live');
                    itemPromises.push(itemPromise);
                });
                Promise.all(itemPromises)
                    .then(results => {
                        validModel = ObjectFunctions.setValueByStringPath(validModel,
                            contentObjectKey, results);
                        resolve(validModel);
                    });
            });
            this.entityPromises.push(promise);
        }

        if (format === 'autocmslookup') {
            const promise = this.loadCMSModel(lookupType, contextValue).then(result => {
                validModel = ObjectFunctions.setValueByStringPath(validModel,
                    contentObjectKey, result[contentKey]);
            });
            this.entityPromises.push(promise);
        }

        return validModel;
    }

    /**
     * Function to validate an array property
     * @param {string} key - The object property name
     * @param {object} schemaProperty - The json schema property object
     * @param {object} model - The entity model data
     * @param {string} objectPath - The object path
     * @return {object} valid model
     */
    validateArrayProperty(key, schemaProperty, model, objectPath) {
        let validModel = model;
        const defaultMinItems = 1;
        const minItems = schemaProperty.minItems
            ? schemaProperty.minItems : defaultMinItems;

        const definitionSchema = this.getArrayItemSchema(key, schemaProperty);
        const modelItems = ObjectFunctions.getValueByStringPath(validModel, objectPath);
        const itemLength = modelItems
            && modelItems.length > minItems ? modelItems.length : minItems;
        for (let i = 0; i < itemLength; i++) {
            const objectKey = `${objectPath}[${i}]`;
            if (definitionSchema.hasOwnProperty('properties')) {
                validModel
                    = this.validateDefinition(objectKey, definitionSchema, validModel);
            } else {
                const value = ObjectFunctions.getValueByStringPath(validModel, objectKey);
                if (!value) {
                    validModel = this.setDefaultValue(definitionSchema,
                        validModel, objectPath);
                }
            }
        }

        return validModel;
    }

    /**
     * Function to ensure a valid definition object
     * @param {string} key - The object property name
     * @param {object} definitionSchema - Json Schema of the definition
     * @param {object} model - The entity model
     * @param {string} objectPath - The object path
     * @return {object} valid entity
     */
    validateDefinition(key, definitionSchema, model, objectPath) {
        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;
        let validModel = Object.assign({}, model);

        const value = ObjectFunctions.getValueByStringPath(validModel, currentObjectPath);
        if (!value) {
            validModel = this.setDefaultValue(definitionSchema, validModel, currentObjectPath);
        }

        Object.keys(definitionSchema.properties).map((objectKey) => {
            validModel = this.validateSchemaProperty(objectKey,
                definitionSchema.properties[objectKey],
                validModel,
                currentObjectPath);
        });
        return validModel;
    }

     /**
     * Function to ensure a valid entity object
     * @param {object} entity - The entity object
     * @param {boolean} blockSchemaDefaults - If this is true we dont want to set default
     * values on the model (e.g. when updating to allow us to delete integers)
     * @return {object} valid entity
     */
    validateEntity(entity, blockSchemaDefaults) {
        return new Promise((resolve) => {
            const validEntity = Object.assign({}, entity);
            let validModel = Object.assign({}, entity.model);
            this.jsonSchema = entity.jsonSchema;

            Object.keys(entity.jsonSchema.properties)
                .map((key) => {
                    const schemaProperty = entity.jsonSchema.properties[key];
                    validModel = this.validateSchemaProperty(key, schemaProperty, validModel,
                        blockSchemaDefaults);
                });

            validEntity.model = validModel;

            Promise.all(this.entityPromises).then(() => resolve(validEntity));
        });
    }

    /**
     * Function to ensure entity model matches with schema
     * @param {string} key - The object property name
     * @param {object} schemaObject - The json schema property object
     * @param {object} model - The entity model data
     * @param {object} objectPath - The object path
     * @param {boolean} blockSchemaDefaults -  If this is true we dont want to set default
     * @return {object} valid entity model adding any missing objects
     */
    validateModel(key, schemaObject, model, objectPath, blockSchemaDefaults) {
        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;
        let validModel = model;
        const value = ObjectFunctions.getValueByStringPath(validModel, currentObjectPath);

        if ((!value
            && (schemaObject.type !== 'string' || value !== '')
            && (schemaObject.type !== 'boolean' || typeof value !== 'boolean'))
            && blockSchemaDefaults !== true) {
            validModel = this.setDefaultValue(schemaObject, validModel, currentObjectPath);
        }
        if (schemaObject.hasOwnProperty('format')
                       && schemaObject.format.indexOf('lookup') !== -1) {
            validModel = this.setupLookupContent(key, schemaObject, validModel, objectPath);
        }

        if (schemaObject.type === 'array'
            && schemaObject.items
            && schemaObject.items.format
            && schemaObject.items.format.indexOf('lookup') !== -1) {
            validModel = this.setupLookupContent(key, schemaObject.items, validModel, objectPath);
        }

        return validModel;
    }

    /**
     * Function to ensure a valid schema property
     * @param {string} key - The object property name
     * @param {object} schemaProperty - The json schema property object
     * @param {object} model - The entity model data
     * @param {string} objectPath - The object path
     * @param {boolean} blockSchemaDefaults -  If this is true we dont want to set default
     * @return {object} valid entity
     */
    validateSchemaProperty(key, schemaProperty, model, objectPath, blockSchemaDefaults) {
        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;

        let validModel = model;
        validModel = this.validateModel(key, schemaProperty, validModel, objectPath,
            blockSchemaDefaults);

        let format = schemaProperty.format;
        if (format && format.indexOf(',') !== -1) {
            format = format.split(',')[0];
        }

        if (schemaProperty && schemaProperty.type === 'string' && format === 'definition') {
            this.setDefinintionSchema(schemaProperty, validModel, currentObjectPath);
        }

        if (schemaProperty && schemaProperty.type === 'array') {
            validModel = this.validateArrayProperty(key, schemaProperty,
                validModel, currentObjectPath);
        }

        if (schemaProperty
                && schemaProperty.type === 'object'
                && this.definitions.hasOwnProperty(key)) {
            const definitionSchema = this.definitions[key];
            validModel
                = this.validateDefinition(currentObjectPath, definitionSchema, validModel);
        }

        if (schemaProperty.hasOwnProperty('properties')) {
            Object.keys(schemaProperty.properties).map((objectKey) => {
                const objectSchemaProperty = schemaProperty.properties[objectKey];
                validModel = this.validateSchemaProperty(objectKey,
                    objectSchemaProperty, validModel, currentObjectPath, blockSchemaDefaults);
            });
        }

        return validModel;
    }
}


export default EntityValidator;
