import * as ContextLookups from 'constants/contextLookups';
import * as EntityActions from 'actions/content/entityActions';

import ArrayFunctions from 'library/arrayfunctions';
import ModalPopup from 'components/common/modalpopup';
import ObjectFunctions from 'library/objectfunctions';
import Prompt from 'components/common/prompt';
import React from 'react';
import StringFunctions from 'library/stringfunctions';
import TextInput from 'components/form/textinput';
import WidgetEdit from 'widgets/common/widgetedit';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class WidgetEditContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            widgetEdit: false,
            entityListEdit: false,
            entity: {},
            errors: {},
            copyContext: '',
            copyError: '',
            valid: true,
            displayDeletePrompt: false,
            displayCopy: false,
            displayCopyPrompt: false,
            deleteItem: {
                key: '',
                entity: '',
                context: '',
                site: '',
                isConfirmed: false,
            },
            newContextCheck: {
                entity: '',
                key: '',
            },
            contextFilter: '',
        };
        this.setModelState = this.setModelState.bind(this);
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
        this.updateProperty = this.updateProperty.bind(this);
        this.validateField = this.validateField.bind(this);
        this.validateForm = this.validateForm.bind(this);
        this.handleEntityClick = this.handleEntityClick.bind(this);
        this.handleContextClick = this.handleContextClick.bind(this);
        this.handleAddNewItem = this.handleAddNewItem.bind(this);
        this.handleContextChange = this.handleContextChange.bind(this);
        this.handleCopy = this.handleCopy.bind(this);
        this.handleCopyCancel = this.handleCopyCancel.bind(this);
        this.handleCopyConfirm = this.handleCopyConfirm.bind(this);
        this.handleCopyDisplay = this.handleCopyDisplay.bind(this);
        this.handleCopySelect = this.handleCopySelect.bind(this);
        this.handleCancelEditList = this.handleCancelEditList.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleContextDeleteClick = this.handleContextDeleteClick.bind(this);
        this.handleDeleteConfirm = this.handleDeleteConfirm.bind(this);
        this.handleDeleteCancel = this.handleDeleteCancel.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (this.state.newContextCheck.key) {
            if (nextProps.entities[this.state.newContextCheck.key]) {
                this.props.actions.loadEntityContexts(nextProps.site.Name,
                    this.state.newContextCheck.entity, nextProps.editEntity);
                this.state.newContextCheck.entity = '';
                this.state.newContextCheck.key = '';
            }
        }

        if (this.state.deleteItem.key && this.state.deleteItem.isConfirmed) {
            if (!this.props.entities[this.state.deleteItem.key].isDeleted
                && nextProps.entities[this.state.deleteItem.key].isDeleted) {
                this.props.actions.loadEntityContexts(this.props.site.Name,
                    this.state.deleteItem.entity);
                this.state.deleteItem = {
                    key: '',
                    entity: '',
                    context: '',
                    site: '',
                    isConfirmed: false,
                };
            }
        }

        this.setState({
            entity: nextProps.editEntity,
            widgetEdit: nextProps.editEntity.isEditing,
            entityListEdit: nextProps.entityList.isEditing,
        });
    }
    formatName(name) {
        const label = name.replace(/([A-Z][a-z])/g, ' $1').replace(/^./, str => str.toUpperCase());
        return label;
    }
    getDeletePromptProps() {
        const deleteItem = this.state.deleteItem;
        let message = 'Are you sure you want to delete ';
        message += `${deleteItem.entity} ${deleteItem.context}? This will be irreversible.`;

        const props = {
            title: 'Confirm Delete',
            message,
            onConfirm: this.handleDeleteConfirm,
            onCancel: this.handleDeleteCancel,
        };

        return props;
    }

    getCopyPromptProps() {
        let message = 'Are you sure you want to copy ';
        message += `${this.state.copyContext}? This will be irreversible.`;

        const props = {
            title: 'Confirm Copy',
            message,
            onConfirm: this.handleCopyConfirm,
            onCancel: this.handleCopyCancel,
        };

        return props;
    }

    getWidgetEditProps() {
        const entity = this.state.entity;
        const widgetEditProps = {
            jsonSchema: entity.jsonSchema,
            copyError: this.state.copyError,
            dataModel: entity.model,
            displayCopy: this.state.displayCopy,
            entityName: entity.name,
            updatePropertyFunction: this.updateProperty,
            onChange: this.setModelState,
            onCopyDisplay: this.handleCopyDisplay,
            onCopyCancel: this.handleCopyCancel,
            onCopyChange: this.handleCopySelect,
            onCopy: this.handleCopy,
            onSave: this.validateForm,
            onCancel: this.handleCancel,
            onDelete: this.handleDelete,
            errors: this.state.errors,
            siteName: StringFunctions.safeUrl(this.props.site.Name),
        };
        if (entity
                && entity.type === 'Custom'
                && entity.status === 'no content') {
            const context = entity.context === 'entity-new-item'
                ? '' : entity.context;
            widgetEditProps.context = context;
            widgetEditProps.contextEdit = true;
            widgetEditProps.onContextChange = this.handleContextChange;
        }

        return widgetEditProps;
    }
    handleContextChange(event) {
        const entity = this.state.entity;
        const context = event.target.value;
        entity.context = context;

        const urlSafeContext = StringFunctions.safeUrl(context);
        const key = `${entity.name}-${urlSafeContext}`;
        const newContextCheck = {
            key,
            entity: entity.name,
        };
        this.state.newContextCheck = newContextCheck;
        this.setState({ entity, newContextCheck });
    }
    handleSave() {
        const entity = this.props.editEntity;
        entity.context = StringFunctions.safeUrl(entity.context);
        this.props.actions.saveEntity(entity);
        if (entity.context === 'entity-new-item') {
            const key = `${entity.name}-${entity.context}`;
            this.state.newContextCheck.key = key;
            this.state.newContextCheck.entity = entity.name;
        }
    }
    handleCancel() {
        const entity = this.props.editEntity;
        this.props.actions.cancelEditEntity(entity.site, entity.name, entity.context, 'draft');
    }
    handleCopy() {
        if (this.state.copyContext !== '') {
            this.setState({ displayCopyPrompt: true });
        } else {
            this.setState({ copyError: 'Please select a context to copy from first' });
        }
    }
    handleCopyDisplay() {
        this.setState({ displayCopy: true, copyError: '' });
    }
    handleCopyCancel() {
        this.setState({ displayCopy: false, displayCopyPrompt: false });
    }
    handleCopyConfirm() {
        this.props.actions.addContextBridge(this.state.entity, this.state.copyContext);
        this.setState({ displayCopy: false, displayCopyPrompt: false });
    }
    handleCopySelect(context) {
        this.setState({ copyContext: context, copyError: '' });
    }
    handleDeleteConfirm() {
        const deleteItem = this.state.deleteItem;
        this.props.actions.deleteEntity(deleteItem.site, deleteItem.entity, deleteItem.context);
        deleteItem.isConfirmed = true;
        this.setState({ displayDeletePrompt: false, deleteItem });
    }
    handleDeleteCancel() {
        const deleteItem = {
            key: '',
            entity: '',
            context: '',
            site: '',
            isConfirmed: false,
        };
        this.setState({ displayDeletePrompt: false, deleteItem });
    }
    handleDelete() {
        const entity = this.props.editEntity;
        this.setDeleteItem(entity.name, entity.context, entity.site);
    }
    handleEntityClick(entityName) {
        this.props.actions.loadEntityContexts(this.props.site.Name, entityName);
        this.setState({ contextFilter: '' });
    }
    handleContextClick(entityName, context) {
        const urlSafeContext = StringFunctions.safeUrl(context);
        this.setState({ displayCopy: false });
        this.props.actions.editEntity(this.props.site.Name, entityName, urlSafeContext);
    }
    handleContextDeleteClick(entityName, context, site) {
        this.setDeleteItem(entityName, context, site);
    }
    handleAddNewItem(entity) {
        this.props.actions.editEntity(this.props.site.Name, entity.name, 'entity-new-item');
    }
    handleCancelEditList() {
        this.props.actions.entityListCancel();
    }
    isRequiredProperty(key) {
        const required = this.props.editEntity.jsonSchema.required
                    && this.props.editEntity.jsonSchema.required.indexOf(key) !== -1;
        return required;
    }
    mapObject(schemaObject, returnFunction, objectPath) {
        return Object.keys(schemaObject).map((key) =>
            returnFunction(key, schemaObject[key], objectPath));
    }
    setModelState(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        const schemaPropertyType = event.target.dataset.type ? event.target.dataset.type : '';
        if (value
                && schemaPropertyType === 'integer'
                && typeof value === 'string'
                && !isNaN(value)) {
            value = parseInt(value, 10);
        } else if (value
                && schemaPropertyType === 'number'
                && typeof value === 'string'
                && !isNaN(value)) {
            value = parseFloat(value);
        }
        return this.updateProperty(field, value);
    }
    setDeleteItem(entityName, context, site) {
        const key = `${entityName}-${context}`;
        const deleteItem = {
            key,
            entity: entityName,
            context,
            site,
            isConfirmed: false,
        };
        this.state.deleteItem = deleteItem;
        this.setState({ displayDeletePrompt: true, deleteItem });
    }
    updateProperty(field, value) {
        const entity = this.state.entity;
        const updatedModel = ObjectFunctions.setValueByStringPath(entity.model, field, value);
        entity.model = updatedModel;
        this.setState({ entity });
        this.props.actions.updateEntityValue(entity, `model.${field}`, value);
    }
    validateForm() {
        this.state.errors = {};
        this.state.valid = true;
        this.mapObject(this.props.editEntity.jsonSchema.properties, this.validateField);
        this.setState({ errors: this.state.errors, valid: this.state.valid });

        if (this.state.valid) {
            this.handleSave();
        }
    }
    validateField(key, schemaObject) {
        const value = ObjectFunctions.getValueByStringPath(this.props.editEntity.model, key);

        if (this.isRequiredProperty(key) && !value) {
            this.state.errors[key] = `${key} is required`;
            this.state.valid = false;
        }
        if (schemaObject.minLength && value.length < schemaObject.minLength) {
            this.state.errors[key] = `${key} minimum length is ${schemaObject.minLength}`;
            this.state.valid = false;
        }
    }
    renderEntityList() {
        let entities = this.props.entityList.entities;
        entities = ArrayFunctions.sortByPropertyAscending(entities, 'name');
        return (
            <div className="entity-list">
                <nav className="entity-list-nav">
                    <h2>Entity</h2>
                    <ul className="nav nav-stacked">
                        {entities.map(this.renderEntityListItem, this)}
                    </ul>
                </nav>
                {this.renderContextsList()}
            </div>
        );
    }
    renderEntityListItem(entity) {
        let className = 'nav-list-item';
        if (entity.isEditing) {
            className = `${className} selected`;
        }
        return (
            <li key={entity.name}
                className={className}
                onClick={() => this.handleEntityClick(entity.name)}>
                {this.formatName(entity.name)}</li>
        );
    }
    renderContextsList() {
        const entities = this.props.entityList.entities;
        const selectedEntity = entities.find(entity => entity.isEditing);
        if (selectedEntity) {
            selectedEntity.contexts.sort();
        }
        const lookupEntities = ['Region',
            'Property',
            'Trade',
            'TradeGroup',
            'TradeParentGroup',
            'TradeContactGroup'];
        const addNewEnabled = selectedEntity && lookupEntities.indexOf(selectedEntity.name) === -1;
        let containerClass = 'entity-list-nav entity-list-contexts';
        if (addNewEnabled) {
            containerClass = `${containerClass} entity-list-add-view`;
        }
        const contextFilterProps = {
            name: 'contextFilter',
            label: '',
            placeholder: 'Filter',
            type: 'text',
            onChange: (event) => {
                this.setState({ contextFilter: event.target.value });
            },
            value: this.state.contextFilter,
            containerClasses: 'mb-0',
        };
        let contexts = [];
        if (selectedEntity) {
            contexts = selectedEntity.contexts.filter(context =>
                context.toLowerCase().indexOf(this.state.contextFilter.toLowerCase()) > -1);
        }
        return (
            <nav className={containerClass}>
                <h2>Context</h2>
                {!selectedEntity
                    && <p>Select an entity</p>}
                {selectedEntity
                    && <TextInput {...contextFilterProps} />}
                {selectedEntity
                    && <ul className="nav nav-stacked">
                    {contexts.map((context, index) =>
                        this.renderContextsListItem(context, selectedEntity, index), this)}
                </ul>}

                {addNewEnabled
                    && <button
                            className="btn btn-primary"
                            onClick={() => this.handleAddNewItem(selectedEntity)}>Add New</button>}
            </nav>
        );
    }
    renderContextsListItem(context, entity, index) {
        let listItem = '';
        if (context.trim() && context !== '-') {
            let className = 'nav-list-item';
            const urlSafeContext = StringFunctions.safeUrl(context);
            if (this.props.editEntity.context === urlSafeContext) {
                className = `${className} selected`;
            }
            const key = `context-${index}`;
            const displayContext = StringFunctions.undoSafeUrl(context);

            const deleteIcon = {
                className: 'item-delete fa fa-times',
                onClick: (event) => {
                    event.stopPropagation();
                    this.handleContextDeleteClick(entity.name, context, entity.site);
                },
            };

            listItem = <li key={key}
                            className={className}
                            onClick={() => this.handleContextClick(entity.name, context)}>
                                {displayContext}
                            {!ContextLookups.entities.hasOwnProperty(entity.name)
                                && <span {...deleteIcon}></span>}
                        </li>;
        }
        return listItem;
    }
    render() {
        let containerClass = 'widget-edit-container';
        if (this.props.entityList.isEditing) {
            containerClass = `${containerClass} entity-list-view`;
        }

        return (
            <div>
                {(this.state.widgetEdit || this.state.entityListEdit)
                    && <ModalPopup>
                            <div className={containerClass}>
                                {this.props.entityList.isEditing
                                    && this.renderEntityList()}

                                {this.state.entityListEdit
                                    && !this.state.widgetEdit
                                    && <span className="close fa fa-times"
                                        onClick={this.handleCancelEditList}></span>}

                                {this.state.widgetEdit
                                    && <WidgetEdit {...this.getWidgetEditProps()} />}

                                {this.state.displayDeletePrompt
                                    && <Prompt {...this.getDeletePromptProps()} />}

                                {this.state.displayCopyPrompt
                                        && <Prompt {...this.getCopyPromptProps()} />}

                                {(this.state.displayCopyPrompt || this.state.displayCopyPrompt)
                                    && <div className="prompt-backdrop"></div>}
                            </div>
                    </ModalPopup>}
            </div>
        );
    }
}

WidgetEditContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    editEntity: React.PropTypes.object.isRequired,
    entityList: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
* map state to props
* @param {object} state - The current state.
* @return {object} the props mapped to state.
*/
function mapStateToProps(state) {
    let editEntity = {
        isEditing: false,
    };

    Object.keys(state.entities).map((key) => {
        const stateEntity = state.entities[key];
        if (stateEntity && stateEntity.isEditing) {
            editEntity = stateEntity;
        }
    });

    const entityList = state.entityList ? state.entityList : {};
    const entities = state.entities ? state.entities : {};

    return {
        editEntity,
        entityList,
        entities,
    };
}

/**
* map dispatch to props
* @param {object} dispatch - redux dispatch
* @return {object} entity actions
*/
function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(EntityActions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(WidgetEditContainer);
