import ObjectFunctions from '../library/objectfunctions';
import fetch from 'isomorphic-fetch';

class EntityAPI {
    constructor() {
        this.definitions = {};
        this.entityPromises = [];
        this.jsonSchema = {};
        this.mode = '';
    }

    /**
     * Load CMS model
     * @param {object} objectType - The object type to return
     * @param {string} id - Id of the object type
     * @return {object} The model
     */
    loadCMSModel(objectType, id) {
        const entityUrl = `/tradebookings/api/cms/${objectType}/${id}`;
        return fetch(entityUrl).then(response => response.json()).then(result =>
            JSON.parse(result.ContentJSON));
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
        const entityUrl
            = `/tradebookings/api/sitebuilder/model/${site}/${entityName}/${context}/${mode}`;
        return fetch(entityUrl).then(response => response.json()).then(result => result);
    }

    /**
     * Function to get a default value by type
     * @param {string} type - The type
     * @return {object} the default type value
     */
    defaultValueByType(type) {
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
     * Function to set a default value
     * @param {object} schemaObject - The json schema property object
     * @param {object} model - The entity model data
     * @param {object} objectPath - The object path
     * @return {object} valid entity model
     */
    setDefaultValue(schemaObject, model, objectPath) {
        let validModel = Object.assign({}, model);
        const defaultValue = schemaObject.default
            ? schemaObject.default : this.defaultValueByType(schemaObject.type);
        validModel = ObjectFunctions.setValueByStringPath(validModel, objectPath, defaultValue);
        return validModel;
    }

    /**
     * Parse ISO date string format into date object
     * @param {string} s - an ISO 8001 format date and time string
     * with all components, e.g. 2015-11-24T19:40:00
     * @returns {Date} - Date instance from parsing the string.
     */
    parseISOLocal(s) {
        const b = s.split(/\D/);
        return new Date(b[0], b[1] - 1, b[2], b[3], b[4], b[5]);
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

        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;
        const entityId = ObjectFunctions.getValueByStringPath(model, currentObjectPath);

        const contentKey = key.replace('Id', '');
        const contentObjectKey = objectPath ? `${objectPath}.${contentKey}` : contentKey;

        let validModel = Object.assign({}, model);
        if (customFormats.indexOf(format) > -1 && entityId) {
            const promise = this.loadEntityModel(this.site, lookupType, entityId, this.mode)
                .then(result => {
                    validModel = ObjectFunctions.setValueByStringPath(validModel,
                        contentObjectKey, JSON.parse(result));
                });
            this.entityPromises.push(promise);
        }

        if (format === 'autocmslookup') {
            const promise = this.loadCMSModel(lookupType, entityId).then(result => {
                validModel = ObjectFunctions.setValueByStringPath(validModel,
                    contentObjectKey, result[contentKey]);
            });
            this.entityPromises.push(promise);
        }

        return validModel;
    }

    /**
     * Function to ensure entity model matches with schema
     * @param {string} key - The object property name
     * @param {object} schemaObject - The json schema property object
     * @param {object} model - The entity model data
     * @param {object} objectPath - The object path
     * @param {int} index - The index of an array item
     * @return {object} valid entity model adding any missing objects
     */
    validateModel(key, schemaObject, model, objectPath) {
        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;
        let validModel = model;

        const value = ObjectFunctions.getValueByStringPath(validModel, currentObjectPath);

        if (!value
            && (schemaObject.type !== 'string' || value !== '')
            && (schemaObject.type !== 'boolean' || typeof value !== 'boolean')) {
            validModel = this.setDefaultValue(schemaObject, validModel, currentObjectPath);
        }

        if (schemaObject.hasOwnProperty('format')
                       && schemaObject.format.indexOf('lookup') !== -1) {
            validModel = this.setupLookupContent(key, schemaObject, validModel, objectPath);
        }

        return validModel;
    }

    /**
     * Function to ensure a valid schema property
     * @param {string} key - The object property name
     * @param {object} schemaProperty - The json schema property object
     * @param {object} model - The entity model data
     * @param {string} objectPath - The object path
     * @return {object} valid entity
     */
    validateSchemaProperty(key, schemaProperty, model, objectPath) {
        const currentObjectPath = objectPath ? `${objectPath}.${key}` : key;

        let validModel = model;
        validModel = this.validateModel(key, schemaProperty, validModel, objectPath);

        let format = schemaProperty.format;
        if (format && format.indexOf(',') !== -1) {
            format = format.split(',')[0];
        }

        if (schemaProperty && schemaProperty.type === 'string' && format === 'definition') {
            const property = schemaProperty.format.split(',')[1];
            let definitionType
                = ObjectFunctions.getValueByStringPath(validModel, currentObjectPath);
            definitionType = definitionType ? definitionType : schemaProperty.default;
            const definitionSchema = this.jsonSchema.definitions[definitionType];
            this.definitions[property] = definitionSchema;
        }
        if (schemaProperty && schemaProperty.type === 'array') {
            const defaultMinItems = 1;
            const minItems = schemaProperty.minItems
                ? schemaProperty.minItems : defaultMinItems;

            let definitionSchema = schemaProperty.items;
            if (this.definitions.hasOwnProperty(key)) {
                definitionSchema = this.definitions[key];
            }
            if (definitionSchema.hasOwnProperty('$ref')) {
                let definitionType = definitionSchema.$ref;
                definitionType = definitionType.replace('#/definitions/', '');
                definitionSchema = this.jsonSchema.definitions[definitionType];
            }

            const modelItems = ObjectFunctions.getValueByStringPath(validModel, currentObjectPath);
            const itemLength = modelItems
                && modelItems.length > minItems ? modelItems.length : minItems;
            for (let i = 0; i < itemLength; i++) {
                const objectKey = `${currentObjectPath}[${i}]`;
                if (definitionSchema.hasOwnProperty('properties')) {
                    validModel
                        = this.validateDefinition(objectKey, definitionSchema, validModel);
                } else {
                    const value = ObjectFunctions.getValueByStringPath(validModel, objectKey);
                    if (!value) {
                        validModel = this.setDefaultValue(definitionSchema,
                            validModel, currentObjectPath);
                    }
                }
            }
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
                    objectSchemaProperty, validModel, currentObjectPath);
            });
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
     * @return {object} valid entity
     */
    validateEntity(entity) {
        return new Promise((resolve) => {
            const validEntity = Object.assign({}, entity);
            let validModel = Object.assign({}, entity.model);
            this.jsonSchema = entity.jsonSchema;

            Object.keys(entity.jsonSchema.properties)
                .map((key) => {
                    const schemaProperty = entity.jsonSchema.properties[key];
                    validModel = this.validateSchemaProperty(key, schemaProperty, validModel);
                });

            validEntity.model = validModel;
            resolve(validEntity);
        });
    }

    /**
     * Function for loading an entity.
     * @param {string} site - The site
     * @param {string} name - The name of the entity
     * @param {string} context - The entity context
     * @param {string} mode - The entity mode (draft or live).
     * @return {object} The entity
     */
    loadEntity(site, name, context, mode) {
        const entityURL
            = `/tradebookings/api/sitebuilder/entity/${site}/${name}/${context}/${mode}`;
        return fetch(entityURL)
            .then(response => response.json())
            .then(result => {
                const entity = {
                    site,
                    context: result.Context,
                    name: result.Name,
                    jsonSchema: JSON.parse(result.JsonSchema),
                    model: JSON.parse(result.Model),
                    lastModifiedDate: this.parseISOLocal(result.LastModifiedDate),
                    lastModifiedUser: result.LastModifiedUser,
                    status: result.Status,
                    type: result.Type,
                };
                this.mode = mode;
                this.site = site;
                return this.validateEntity(entity);
            })
            .then(entity => Promise.all(this.entityPromises)
            .then(() => entity))
            .then(entity => {
                this.entityPromises = [];
                return entity;
            });
    }

    /**
     * Function for updating an entity.
     * @param {object} entity - The entity to update
     * @param {string} field - The field to update
     * @param {string} value - The new value to set
     * @return {object} The updated entity
     */
    updateEntityValue(entity, field, value) {
        const updatedEntity = Object.assign({}, entity);
        const updatedModel = ObjectFunctions.setValueByStringPath(entity.model, field, value);
        updatedEntity.model = updatedModel;
        updatedEntity.lastModifiedDate = new Date();
        return this.validateEntity(updatedEntity)
            .then(validEntity => Promise.all(this.entityPromises)
            .then(() => validEntity))
            .then(validEntity => {
                this.entityPromises = [];
                return validEntity;
            });
    }

    /**
     * Function for saving an entity.
     * @param {object} entity - The entity to update
     * @return {object} The entity
     */
    saveEntity(entity) {
        const entityBaseUrl = '/tradebookings/api/sitebuilder/model/';
        let entityURL = `${entityBaseUrl}${entity.site}/${entity.name}/${entity.context}`;
        if (entity.type.toLowerCase() === 'custom') {
            entityURL = `${entityURL}?publish=true`;
        }
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'PUT',
            credentials: 'same-origin',
            body: JSON.stringify(entity.model),
        };
        return fetch(entityURL, fetchOptions)
            .then(response => response.json())
            .then(() => {
                const updatedEntity = Object.assign({}, entity);
                updatedEntity.lastModifiedDate = new Date();
                updatedEntity.status = 'draft';
                this.mode = 'draft';
                return this.validateEntity(updatedEntity);
            })
            .then(result => Promise.all(this.entityPromises)
            .then(() => result))
            .then(result => {
                this.entityPromises = [];
                return result;
            });
    }

    /**
     * Function for publishing an entity.
     * @param {string} site - The site
     * @param {string} name - The name of the entity
     * @param {string} context - The entity context
     * @param {object} entity - The entity to update
     * @return {object} The entity
     */
    publishEntity(site, name, context, entity) {
        const entityURL = `/tradebookings/api/sitebuilder/model/${site}/${name}/${context}/publish`;
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'PUT',
            credentials: 'same-origin',
        };
        return fetch(entityURL, fetchOptions)
            .then(response => response.json())
            .then(() => {
                const updatedEntity = Object.assign({}, entity);
                updatedEntity.lastModifiedDate = new Date();
                updatedEntity.status = 'live';
                this.mode = 'live';
                return this.validateEntity(updatedEntity);
            })
            .then(result => Promise.all(this.entityPromises)
            .then(() => result))
            .then(result => {
                this.entityPromises = [];
                return result;
            });
    }


    /**
     * Function for loading a list of entities.
     * @param {string} site - The site
     * @param {string} type - The entity type
     * @return {object} The entities
     */
    loadEntityList(site, type) {
        let entitiesUrl = `/tradebookings/api/sitebuilder/entities/${site}`;
        if (type) {
            entitiesUrl = `${entitiesUrl}?type=${type}`;
        }

        return fetch(entitiesUrl)
            .then(response => response.json())
            .then(result => {
                const entities = [];
                result.Entities.forEach(entity => {
                    const customEntity = {
                        name: entity.Name,
                        type: entity.Type,
                        contexts: [],
                    };
                    entities.push(customEntity);
                });
                return entities;
            });
    }

    /**
     * Function for loading a list of contexts from an entity
     * @param {string} site - The site
     * @param {string} entity - The entity
     * @return {object} The contexts
     */
    loadContextsList(site, entity) {
        const contextLookups = {
            Country: {
                apiUrl: '/tradebookings/api/geography/country',
            },
            Region: {
                apiUrl: '/tradebookings/api/geography/region',
            },
            Property: {
                apiUrl: '/tradebookings/api/property/propertyreference',
            },
            TradeGroup: {
                apiUrl: '/tradebookings/api/tradegroup',
            },
            TradeParentGroup: {
                apiUrl: '/tradebookings/api/tradeparentgroup',
            },
            TradeContactGroup: {
                apiUrl: '/tradebookings/api/tradecontactgroup',
            },
        };

        if (contextLookups.hasOwnProperty(entity)) {
            return fetch(contextLookups[entity].apiUrl)
                .then(response => response.json())
                .then(lookups => {
                    const contexts = [];
                    lookups.forEach(lookup => {
                        contexts.push(lookup.Name);
                    });
                    return contexts;
                });
        }

        const contextsUrl = `/tradebookings/api/sitebuilder/${site}/${entity}/contexts`;
        return fetch(contextsUrl)
            .then(response => response.json());
    }

    /**
    * Function for loading models of a given entity type
    * @param {string} site - The site
    * @param {string} entityType - The entity type
    * @return {object} The contexts
    */
    loadModelsFromEntityType(site, entityType) {
        const modelsURL = `/tradebookings/api/sitebuilder/models/${site}/${entityType}`;
        this.items = [];
        return fetch(modelsURL)
            .then(response => response.json())
            .then(models => {
                Object.keys(models)
                    .map(key => {
                        const item = JSON.parse(models[key]);
                        if (typeof item === 'object') {
                            item.context = key;
                            this.items.push(item);
                        }
                    });
                return this.items;
            }
        );
    }
}

export default EntityAPI;
