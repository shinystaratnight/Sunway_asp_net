import '../../../styles/components/_widgetstatus.scss';
import '../../../styles/widgets/common/_widgetEdit.scss';

import JSONSchemaForm from '../../components/common/JSONSchemaForm';
import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import Tabs from '../../components/common/tabs';
import TextInput from '../../components/form/textinput';

class WidgetEdit extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedTab: 'Content',
        };
        this.setSelectedTab = this.setSelectedTab.bind(this);
    }
    setSelectedTab(tab) {
        this.setState({ selectedTab: tab });
    }
    contextProps() {
        const props = {
            key: 'context',
            name: 'context',
            label: 'Context',
            required: true,
            description: 'Name to represent this item which must be unique for the entity.',
            value: this.props.context,
            onChange: this.props.onContextChange,
        };
        return props;
    }
    render() {
        const tabProps = {
            selectedTab: this.state.selectedTab,
            tabs: [
                {
                    name: 'Content',
                    iconClass: 'fa fa-file-text-o',
                    onClick: () => this.setSelectedTab('Content'),

                },
                // {
                //    name: 'History',
                //    iconClass: 'fa fa-history',
                //    onClick: () => this.setSelectedTab('History'),
                // },
            ],
        };

        if (!ObjectFunctions.isNullOrEmpty(this.props.jsonSchema.properties.Configuration)) {
            tabProps.tabs.push(
                {
                    name: 'Configuration',
                    iconClass: 'fa fa-cog',
                    onClick: () => this.setSelectedTab('Configuration'),
                }
            );
        }

        const jsonSchemaFormProps = {
            jsonSchema: this.props.jsonSchema,
            dataModel: this.props.dataModel,
            updatePropertyFunction: this.props.updatePropertyFunction,
            onChange: this.props.onChange,
            errors: this.props.errors,
            siteName: this.props.siteName,
        };

        if (this.state.selectedTab === 'Content') {
            jsonSchemaFormProps.excludeProperties = 'Configuration';
        }

        if (this.state.selectedTab === 'Configuration') {
            jsonSchemaFormProps.jsonSchema = this.props.jsonSchema.properties.Configuration;
            jsonSchemaFormProps.objectPath = 'Configuration';
        }
        return (
            <section className="widget-edit-content">
                <header className="header">
                    <h1>{this.props.jsonSchema.title}</h1>
                    <p className="lead">{this.props.jsonSchema.description}</p>
                    <span className="close fa fa-times" onClick={this.props.onCancel}></span>
                    <nav className="nav">
                        <Tabs {...tabProps} />
                    </nav>
                </header>
                <div className="widget-edit-form-container">
                    {!this.props.jsonSchema.properties
                        && <p>Loading...</p>
                    }

                    {this.state.selectedTab === 'Content'
                        && this.props.contextEdit
                        && <TextInput {...this.contextProps()} />}

                    {this.state.selectedTab === 'Content'
                        && this.props.jsonSchema.properties
                        && <JSONSchemaForm {...jsonSchemaFormProps} />}

                    {this.state.selectedTab === 'Configuration'
                        && this.props.jsonSchema.properties.hasOwnProperty('Configuration')
                            && <JSONSchemaForm {...jsonSchemaFormProps} />}

                    {this.state.selectedTab === 'History'
                        && <p>Content History</p>}
                </div>
                <footer className="footer clear">
                     {this.props.jsonSchema.properties
                        && <button type="button"
                                className="btn btn-primary"
                                 onClick={this.props.onSave}>
                                <span className="fa fa-floppy-o"></span>Save</button>}
                     {this.props.jsonSchema.properties
                        && <button type="button"
                            className="btn btn-default"
                            onClick={this.props.onCancel}>
                            <span className="fa fa-times"></span>Cancel</button>}
                     {this.props.jsonSchema.properties
                        && !this.props.contextEdit
                        && <button type="button"
                            className="btn btn-default btn-delete"
                            onClick={this.props.onDelete}>
                            <span className="fa fa-trash"></span>Delete</button>}
                </footer>
            </section>
        );
    }
}

WidgetEdit.propTypes = {
    jsonSchema: React.PropTypes.object.isRequired,
    dataModel: React.PropTypes.object.isRequired,
    updatePropertyFunction: React.PropTypes.func.isRequired,
    onChange: React.PropTypes.func.isRequired,
    onSave: React.PropTypes.func.isRequired,
    onCancel: React.PropTypes.func.isRequired,
    onDelete: React.PropTypes.func.isRequired,
    errors: React.PropTypes.object.isRequired,
    context: React.PropTypes.string,
    siteName: React.PropTypes.string,
    contextEdit: React.PropTypes.bool,
    onContextChange: React.PropTypes.func,
};

export default WidgetEdit;
