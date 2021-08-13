import * as FormInputs from '../../components/form';
import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import Tabs from '../../components/common/tabs';

class JSONSchemaForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            definitions: {},
            imageSearch: {},
            images: {},
            sections: {},
            valid: true,
        };
        this.addArrayItem = this.addArrayItem.bind(this);
        this.removeArrayItem = this.removeArrayItem.bind(this);
        this.getSchemaDefinition = this.getSchemaDefinition.bind(this);
        this.mapObject = this.mapObject.bind(this);
        this.mapObjectByString = this.mapObjectByString.bind(this);
        this.parseSchemaProperty = this.parseSchemaProperty.bind(this);
        this.parseStringProperty = this.parseStringProperty.bind(this);
        this.renderFormInput = this.renderFormInput.bind(this);
        this.renderSection = this.renderSection.bind(this);
        this.isRequiredProperty = this.isRequiredProperty.bind(this);
        this.setSelectedTab = this.setSelectedTab.bind(this);
        this.toggleArrayItem = this.toggleArrayItem.bind(this);
    }
    mapObject(schemaObject, returnFunction, objectPath) {
        return Object.keys(schemaObject).map((key) =>
            returnFunction(key, schemaObject[key], objectPath));
    }
    isRequiredProperty(key) {
        let required = false;
        required = this.props.jsonSchema.required
                    && this.props.jsonSchema.required.indexOf(key) !== -1;
        return required;
    }
    mapObjectByString(mapObject, mapPath) {
        let path = mapPath ? mapPath : '';
        path = path.replace(/\[(\w+)\]/g, '.$1');
        path = path.replace(/^\./, '');

        const fields = path.split('.');
        let currentObject = mapObject;
        let match = false;

        for (let i = 0; i < fields.length; i++) {
            const field = fields[i];
            if (field in currentObject) {
                currentObject = currentObject[field];
                match = true;
            } else {
                match = false;
                break;
            }
        }
        return match ? currentObject : null;
    }
    getValue(objectPath) {
        return this.mapObjectByString(this.props.dataModel, objectPath);
    }
    getSchemaDefinition(name) {
        let schema = {};
        if (this.props.jsonSchema.definitions.hasOwnProperty(name)) {
            schema = this.props.jsonSchema.definitions[name];
        }
        return schema;
    }
    createLabel(key) {
        const label = key.replace(/([A-Z][a-z])/g, ' $1').replace(/^./, str => str.toUpperCase());
        return label;
    }
    validProperty(key, objectPath) {
        let valid = true;
        if (this.state.sections.hasOwnProperty(objectPath)) {
            const excludeProperties = [];
            const section = this.state.sections[objectPath];
            const tabs = section.tabs;
            tabs.forEach(tab => {
                if (tab !== 'Content' && tab !== section.selectedTab) {
                    excludeProperties.push(tab);
                }
            });
            if (excludeProperties.length > 0 && excludeProperties.indexOf(key) !== -1) {
                valid = false;
            }
        } else if (this.props.excludeProperties !== '' && key === this.props.excludeProperties) {
            valid = false;
        }
        return valid;
    }
    parseSchemaProperty(key, schemaObject, objectPath) {
        let input = '';

        if (!this.validProperty(key, objectPath)) {
            return '';
        }

        let type = schemaObject.type;
        if (Array.isArray(schemaObject.type)) {
            type = schemaObject.type[0];
        }
        switch (type) {
            case 'string':
                input = this.parseStringProperty(key, schemaObject, objectPath);
                break;
            case 'integer':
            case 'number': {
                input = this.parseNumericProperty(key, schemaObject, objectPath);
                break;
            }
            case 'boolean':
                input = this.renderFormInput('Checkbox', key, schemaObject, objectPath);
                break;
            case 'object':
                input = this.renderSection(key, schemaObject, objectPath);
                break;
            case 'array':
                if (!this.state.definitions.hasOwnProperty(key)
                        || (this.state.definitions.hasOwnProperty(key)
                            && this.getValue(key) !== '')) {
                    input = this.parseArrayProperty(key, schemaObject, objectPath);
                }
                break;
            default:
                input = '';
        }
        return input;
    }
    parseFormat(format) {
        let parsedFormat = format ? format : '';
        if (parsedFormat.indexOf(',') !== -1) {
            parsedFormat = parsedFormat.split(',')[0];
        }
        return parsedFormat;
    }
    parseObjectKey(objectPath, key) {
        return objectPath ? `${objectPath}.${key}` : key;
    }
    parseStringProperty(key, schemaObject, objectPath) {
        let input = '';
        const format = this.parseFormat(schemaObject.format);
        switch (format) {
            case 'select':
                input = this.renderFormInput('Select', key, schemaObject, objectPath);
                break;
            case 'radio':
                input = this.renderFormInput('Radio', key, schemaObject, objectPath);
                break;
            case 'checkbox':
                input = this.renderFormInput('Checkbox', key, schemaObject, objectPath);
                break;
            case 'image':
                input = this.renderFormInput('ImageSelect', key, schemaObject, objectPath);
                break;
            case 'definition': {
                const objectKey = this.parseObjectKey(objectPath, key);
                const property = schemaObject.format.split(',')[1];
                const propertyKey = this.parseObjectKey(objectPath, property);

                this.state.definitions[propertyKey] = objectKey;
                input = this.renderFormInput('Select', key, schemaObject, objectPath);
                break;
            }
            case 'autolookup':
            case 'autocmslookup':
            case 'autocontextlookup':
            case 'autocustomlookup':
            case 'automultilookup':
            case 'automulticustomlookup':
            case 'automulticontextlookup':
                input = this.renderFormInput('AutoComplete', key, schemaObject, objectPath);
                break;
            case 'url':
            case 'email':
            case 'date':
            case 'color':
            case 'colorpicker':
            default:
                input = this.renderFormInput('Text', key, schemaObject, objectPath);
                break;
        }
        return input;
    }
    parseNumericProperty(key, schemaObject, objectPath) {
        let input = '';
        const format = this.parseFormat(schemaObject.format);
        switch (format) {
            case 'autolookup':
            case 'autocmslookup':
            case 'autocontextlookup':
            case 'autocustomlookup':
            case 'automultilookup':
            case 'automulticustomlookup':
                input = this.renderFormInput('AutoComplete', key, schemaObject, objectPath);
                break;
            case 'selectlookup':
            case 'select':
                input = this.renderFormInput('Select', key, schemaObject, objectPath);
                break;
            default:
                input = this.renderFormInput('Text', key, schemaObject, objectPath);
                break;
        }
        return input;
    }
    parseArrayProperty(key, schemaObject, objectPath) {
        let input = '';
        const objectKey = objectPath ? `${objectPath}.${key}` : key;
        const arrayItemSchema = schemaObject.items;
        if (arrayItemSchema.hasOwnProperty('properties')
                || this.state.definitions.hasOwnProperty(objectKey)
                || arrayItemSchema.hasOwnProperty('$ref')) {
            input = this.renderArray(key, schemaObject, objectPath);
        } else if (arrayItemSchema.hasOwnProperty('format')) {
            const format = this.parseFormat(schemaObject.format);
            if (format.indexOf(('automulti') > -1)) {
                input = this.renderFormInput('AutoComplete', key, arrayItemSchema, objectPath);
            }
        }
        return input;
    }
    renderFormInput(type, key, schemaObject, objectPath) {
        const FormInput = FormInputs[`${type}Input`];
        const label = this.createLabel(key);
        const objectKey = this.parseObjectKey(objectPath, key);
        let value = this.getValue(objectKey);
        value = value ? value : '';

        const format = this.parseFormat(schemaObject.format);

        const inputProps = {
            key: objectKey,
            name: objectKey,
            label,
            error: this.props.errors[objectKey],
            required: this.isRequiredProperty(key),
            description: schemaObject.description,
            value,
            onChange: this.props.onChange,
            siteName: this.props.siteName,
            dataAttributes: [
                {
                    key: 'type',
                    value: schemaObject.type,
                },
            ],
        };

        switch (type) {
            case 'Text':
                inputProps.maxLength = schemaObject.maxLength;
                inputProps.placeholder = schemaObject.title;
                inputProps.type = schemaObject.format;
                break;
            case 'Select':
                if (schemaObject.hasOwnProperty('enum')) {
                    inputProps.options = schemaObject.enum;
                }
                if (schemaObject.hasOwnProperty('minimum')
                        && schemaObject.hasOwnProperty('maximum')) {
                    inputProps.optionsRangeMin = schemaObject.minimum;
                    inputProps.optionsRangeMax = schemaObject.maximum;
                }
                if (format === 'selectlookup') {
                    const formats = schemaObject.format.split(',');
                    inputProps.lookup = formats[1];
                    inputProps.fieldValue = formats.length > 2 ? formats[2] : 'Name';
                }
                break;
            case 'Checkbox':
            case 'Radio':
                inputProps.value = typeof inputProps.value !== 'boolean' ? false : inputProps.value;
                break;
            case 'AutoComplete': {
                let lookup = schemaObject.format.split(',')[1];
                if (format === 'autocustomlookup' || format === 'automulticustomlookup') {
                    lookup = `custom|${lookup}`;
                } else if (format === 'autocontextlookup' || format === 'automulticontextlookup') {
                    lookup = `context|${lookup}`;
                }

                inputProps.lookup = lookup;
                inputProps.label = schemaObject.title ? schemaObject.title : inputProps.label;
                inputProps.onChange = this.props.updatePropertyFunction;
                inputProps.multiple = format.indexOf('automulti') > -1;
                inputProps.siteName = this.props.siteName;
                inputProps.allowZeroValue = schemaObject.type === 'integer'
                    || schemaObject.type === 'number';
                break;
            }
            case 'ImageSelect':
                inputProps.onChange = this.props.updatePropertyFunction;
                break;
            default:
        }

        return (
            <FormInput {...inputProps} />
        );
    }
    renderSection(key, schemaObject, objectPath) {
        const objectKey = objectPath ? `${objectPath}.${key}` : key;

        let sectionSchema = schemaObject;
        if (this.state.definitions.hasOwnProperty(objectKey)) {
            const definitionKey = this.state.definitions[objectKey];
            const definition = this.getValue(definitionKey);
            sectionSchema = this.getSchemaDefinition(definition);
        }
        if (sectionSchema.hasOwnProperty('$ref')) {
            let definition = sectionSchema.$ref;
            definition = definition.replace('#/definitions/', '');
            sectionSchema = this.getSchemaDefinition(definition);
        }

        if (!this.state.sections.hasOwnProperty(objectKey)) {
            const tabs = [];
            for (const prop in sectionSchema.properties) {
                if (sectionSchema.properties[prop].hasOwnProperty('format')
                        && sectionSchema.properties[prop].format === 'tab') {
                    tabs.push(prop);
                }
            }
            this.state.sections[objectKey] = {
                selectedTab: 'Content',
                tabs,
                expanded: false,
            };
        }

        const sectionItem = this.state.sections[objectKey];

        let sectionProperties = sectionSchema.properties;
        let sectionKey = objectKey;

        if (sectionItem.selectedTab !== 'Content') {
            sectionProperties = sectionSchema.properties[sectionItem.selectedTab].properties;
            sectionKey = `${objectKey}.${sectionItem.selectedTab}`;
        }
        return (
            <div key={objectKey} className="section">
                {schemaObject.title && schemaObject.title !== ''
                        && <h2>{schemaObject.title}</h2>}
                {schemaObject.description && schemaObject.description !== ''
                    && <p className="description">{schemaObject.description}</p>}
                {sectionItem.tabs.length > 0
                        && this.renderTabs(sectionItem.tabs,
                            sectionItem.selectedTab, objectKey)}
                {sectionSchema.properties
                    && this.mapObject(sectionProperties,
                    this.parseSchemaProperty, sectionKey)}
            </div>
        );
    }
    renderArray(key, schemaObject, objectPath) {
        const objectKey = objectPath ? `${objectPath}.${key}` : key;
        const defaultMinItems = 1;
        const minItems = schemaObject.minItems ? schemaObject.minItems : defaultMinItems;
        const arrayItems = [];
        let arrayItemSchema = schemaObject.items;
        let definition = '';
        if (this.state.definitions.hasOwnProperty(objectKey)) {
            const definitionKey = this.state.definitions[objectKey];
            definition = this.getValue(definitionKey);
            arrayItemSchema = this.getSchemaDefinition(definition);
        }
        if (arrayItemSchema.hasOwnProperty('$ref')) {
            definition = arrayItemSchema.$ref;
            definition = definition.replace('#/definitions/', '');
            arrayItemSchema = this.getSchemaDefinition(definition);
        }
        const modelItems = this.getValue(objectKey);
        const itemLength = modelItems ? modelItems.length : minItems;
        for (let i = 0; i < itemLength; i++) {
            const arrayItem = {
                key: `${objectKey}[${i}]`,
                parentKey: objectKey,
                index: i,
                schema: arrayItemSchema,
                definition: definition ? definition : 'Item',
            };
            arrayItems.push(arrayItem);
        }
        return (
            <div className="form-section" key={objectKey}>
                {schemaObject.title && schemaObject.title !== ''
                    && <h2>{schemaObject.title}</h2>}
                {schemaObject.description && schemaObject.description !== ''
                    && <p className="description">{schemaObject.description}</p>}
                {arrayItems.map(this.renderArrayItem, this)}
                {arrayItems.length === 0
                    && <p className="no-items">No Items Added</p>}

                <button type="button" className="btn btn-default btn-xs btn-array-add"
                    onClick={() => this.addArrayItem(objectKey)}>
                    <span className="fa fa-plus"></span>Add Item</button>
            </div>
        );
    }
    renderArrayItem(arrayItem) {
        if (!this.state.sections.hasOwnProperty(arrayItem.key)) {
            const tabs = [];
            for (const prop in arrayItem.schema.properties) {
                if (arrayItem.schema.properties[prop].hasOwnProperty('format')
                        && arrayItem.schema.properties[prop].format === 'tab') {
                    tabs.push(prop);
                }
            }
            this.state.sections[arrayItem.key] = {
                selectedTab: 'Content',
                tabs,
                expanded: false,
            };
        }

        const sectionItem = this.state.sections[arrayItem.key];

        let arrayItemProperties = arrayItem.schema.properties;
        let arrayItemKey = arrayItem.key;

        if (sectionItem.selectedTab !== 'Content') {
            arrayItemProperties = arrayItem.schema.properties[sectionItem.selectedTab].properties;
            arrayItemKey = `${arrayItem.key}.${sectionItem.selectedTab}`;
        }

        const toggleIconClass = sectionItem.expanded ? 'fa-minus' : 'fa-plus';
        const sectionToggleClass = `section-item-toggle fa ${toggleIconClass}`;

        let contentClass = 'section-item-content';
        if (sectionItem.expanded) {
            contentClass = `${contentClass} section-item-expanded`;
        }
        const itemTitle = `${this.createLabel(arrayItem.definition)} ${arrayItem.index + 1}`;
        return (
            <div className="form-section-item" key={arrayItem.key}>
                <div className="section-item-header">
                    <span className="section-number">{arrayItem.index + 1}</span>
                    <h3>{itemTitle}</h3>
                    <span className={sectionToggleClass}
                        onClick={() => this.toggleArrayItem(arrayItem.key)}></span>
                </div>

                <div className={contentClass}>
                    {sectionItem.tabs.length > 0
                        && this.renderTabs(sectionItem.tabs,
                            sectionItem.selectedTab, arrayItem.key)}

                        {this.mapObject(arrayItemProperties,
                            this.parseSchemaProperty, arrayItemKey)}
                        <button
                        type="button"
                        className="btn btn-default btn-xs"
                        onClick={() => this.removeArrayItem(arrayItem.parentKey, arrayItem.index)}>
                    <span className="fa fa-minus"></span>Remove</button>
                </div>
            </div>
        );
    }
    renderTabs(tabs, selectedTab, key) {
        const tabProps = {
            selectedTab,
            tabs: [
                {
                    name: 'Content',
                    onClick: () => this.setSelectedTab(key, 'Content'),
                },
            ],
        };
        tabs.forEach(tab => {
            const tabPropsTab = {
                name: tab,
                onClick: () => this.setSelectedTab(key, tab),
            };
            tabProps.tabs.push(tabPropsTab);
        });
        return (
            <nav className="nav-pills">
                <Tabs {...tabProps} />
            </nav>
        );
    }
    addArrayItem(key) {
        const modelItems = this.getValue(key);
        modelItems.push({});
        this.props.updatePropertyFunction(key, modelItems);
    }
    removeArrayItem(key, index) {
        const modelItems = this.getValue(key);
        modelItems.splice(index, 1);
        this.props.updatePropertyFunction(key, modelItems);
    }
    setSelectedTab(key, tab) {
        const sections = this.state.sections;
        sections[key] = ObjectFunctions.setValueByStringPath(sections[key], 'selectedTab', tab);
        this.setState({ sections });
    }
    toggleArrayItem(key) {
        const sections = this.state.sections;
        const expanded = sections[key].expanded;
        sections[key] = ObjectFunctions.setValueByStringPath(sections[key], 'expanded', !expanded);
        this.setState({ sections });
    }
    render() {
        return (
            <form>
                {this.mapObject(this.props.jsonSchema.properties,
                    this.parseSchemaProperty, this.props.objectPath)}
            </form>
        );
    }
}

JSONSchemaForm.propTypes = {
    jsonSchema: React.PropTypes.object.isRequired,
    objectPath: React.PropTypes.string,
    dataModel: React.PropTypes.object.isRequired,
    updatePropertyFunction: React.PropTypes.func.isRequired,
    onChange: React.PropTypes.func.isRequired,
    excludeProperties: React.PropTypes.string,
    siteName: React.PropTypes.string,
    errors: React.PropTypes.object.isRequired,
};

export default JSONSchemaForm;
