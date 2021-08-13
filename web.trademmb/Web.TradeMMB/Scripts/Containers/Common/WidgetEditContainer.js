import * as EntityActions from '../../actions/content/entityActions';

import ArrayFunctions from '../../library/arrayfunctions';
import ModalPopup from '../../components/common/modalpopup';
import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import WidgetEdit from '../../widgets/common/widgetedit';

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
            valid: true,
            newContextCheck: {
                entity: '',
                key: '',
            },
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
        this.handleCancelEditList = this.handleCancelEditList.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (this.state.newContextCheck.key) {
            if (nextProps.entities[this.state.newContextCheck.key]) {
                const entityName = this.state.newContextCheck.entity;
                this.props.actions.loadEntityContexts(this.props.site.Name, entityName);
                this.state.newContextCheck.entity = '';
                this.state.newContextCheck.key = '';
            }
        }

        this.setState({
            entity: nextProps.editEntity,
            widgetEdit: nextProps.editEntity.isEditing,
            entityListEdit: nextProps.entityList.isEditing,
        });
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
        }
        return this.updateProperty(field, value);
    }
    updateProperty(field, value) {
        const entity = this.state.entity;
        const updatedModel = ObjectFunctions.setValueByStringPath(entity.model, field, value);
        entity.model = updatedModel;
        this.setState({ entity });
        this.props.actions.updateEntityValue(entity, field, value);
    }
    handleContextChange(event) {
        const value = event.target.value;
        const entity = this.state.entity;
        entity.context = value;
        return this.setState({ entity });
    }
    handleSave() {
        const entity = this.props.editEntity;
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
    handleEntityClick(entityName) {
        this.props.actions.loadEntityContexts(this.props.site.Name, entityName);
    }
    handleContextClick(entityName, context) {
        this.props.actions.editEntity(this.props.site.Name, entityName, context);
    }
    handleAddNewItem(entity) {
        this.props.actions.editEntity(entity.name, 'entity-new-item');
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
    widgetEditProps() {
        const entity = this.state.entity;
        const widgetEditProps = {
            jsonSchema: entity.jsonSchema,
            dataModel: entity.model,
            updatePropertyFunction: this.updateProperty,
            onChange: this.setModelState,
            onSave: this.validateForm,
            onCancel: this.handleCancel,
            errors: this.state.errors,
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
    formatName(name) {
        const label = name.replace(/([A-Z][a-z])/g, ' $1').replace(/^./, str => str.toUpperCase());
        return label;
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
            'TradeGroup',
            'TradeParentGroup',
            'TradeContactGroup'];
        const addNewEnabled = selectedEntity && lookupEntities.indexOf(selectedEntity.name) === -1;
        let containerClass = 'entity-list-nav entity-list-contexts';
        if (addNewEnabled) {
            containerClass = `${containerClass} entity-list-add-view`;
        }
        return (
            <nav className={containerClass}>
                <h2>Context</h2>
                {!selectedEntity
                    && <p>Select an entity</p>}
                {selectedEntity
                    && <ul className="nav nav-stacked">
                    {selectedEntity.contexts.map((context, index) =>
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
        let className = 'nav-list-item';
        if (this.props.editEntity.context === context) {
            className = `${className} selected`;
        }
        const key = `context-${index}`;
        return (
            <li key={key}
                className={className}
                onClick={() => this.handleContextClick(entity.name, context)}>{context}</li>
        );
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
                                    && <WidgetEdit {...this.widgetEditProps()} />}
                            </div>
                    </ModalPopup>}
            </div>
        );
    }
}

WidgetEditContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    editEntity: React.PropTypes.object.isRequired,
    entityList: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
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
