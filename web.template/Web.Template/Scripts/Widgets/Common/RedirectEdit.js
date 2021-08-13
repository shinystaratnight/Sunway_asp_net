import '../../../styles/widgets/common/_redirectEdit.scss';

import React from 'react';
import TextInput from '../../components/form/textinput';

class RedirectEdit extends React.Component {
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
    render() {
        const selectedRedirect = this.props.redirects.selectedRedirect;
        const previousInputProps = {
            name: 'Url',
            label: 'Previous URL',
            type: 'text',
            value: selectedRedirect.Url,
            onChange: this.props.onChange,
            error: this.props.redirects.warnings.Url,
        };

        const currentInputProps = {
            name: 'RedirectUrl',
            label: 'Current URL',
            type: 'text',
            value: selectedRedirect.RedirectUrl,
            onChange: this.props.onChange,
            error: this.props.redirects.warnings.RedirectUrl,
        };

        let alert = '';
        if (this.props.redirects.warnings && this.props.redirects.warnings.alert) {
            alert = this.props.redirects.warnings.alert;
        }

        return (
            <section className="redirect-edit-content">
                <header className="header">
                    <h1>URL Redirect</h1>
                    <span className="close fa fa-times" onClick={this.props.onClose}></span>
                </header>

                {alert !== ''
                    && <p className="text-danger redirect-alert">
                        {alert}
                    </p>}

                {this.props.redirects.isEditing
                && <div className="redirect-edit">
                       <TextInput {...previousInputProps} />
                       <TextInput {...currentInputProps} />
                </div>}


                {this.props.redirects.isEditing
                    && <footer className="footer clear">
                     {<button type="button"
                                className="btn btn-primary"
                                 onClick={this.props.onSave}>
                                <span className="fa fa-floppy-o"></span>Save</button>}
                     {<button type="button"
                            className="btn btn-default"
                            onClick={this.props.onCancel}>
                            <span className="fa fa-times"></span>Cancel</button>}
                     {this.props.redirects.selectedRedirect.RedirectId > 0
                         && <button type="button"
                            className="btn btn-default btn-delete"
                            onClick={this.props.onDelete}>
                            <span className="fa fa-trash"></span>Delete</button>}
                </footer>}
            </section>
        );
    }
}

RedirectEdit.propTypes = {
    jsonSchema: React.PropTypes.object.isRequired,
    dataModel: React.PropTypes.object.isRequired,
    updatePropertyFunction: React.PropTypes.func.isRequired,
    onChange: React.PropTypes.func.isRequired,
    onSave: React.PropTypes.func.isRequired,
    onCancel: React.PropTypes.func.isRequired,
    onClose: React.PropTypes.func.isRequired,
    onDelete: React.PropTypes.func.isRequired,
    errors: React.PropTypes.object.isRequired,
    context: React.PropTypes.string,
    siteName: React.PropTypes.string,
    contextEdit: React.PropTypes.bool,
    onContextChange: React.PropTypes.func,
    redirects: React.PropTypes.object.isRequired,
};

export default RedirectEdit;
