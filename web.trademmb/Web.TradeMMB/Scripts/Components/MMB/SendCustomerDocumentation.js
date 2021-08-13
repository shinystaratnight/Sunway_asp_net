import React from 'react';
import TextInput from '../form/textinput';

export default class SendCustomerDocumentation extends React.Component {
    constructor() {
        super();
        this.state = {
            overrideEmail: '',
        };
    }
    renderHeader() {
        const btnProps = {
            onClick: () => this.props.handleShowDocumentPopup(''),
            className: 'fa fa-times',
        };
        return (
            <div className="panel-header panel-section">
                    <h3 className=" h-tertiary">{this.props.title}</h3>
                    <span {...btnProps}></span>
            </div>
        );
    }
    renderContent() {
        const textInputProps = {
            name: 'override email',
            label: this.props.emailLabel,
            value: this.state.overrideEmail,
            onChange: (event) => { this.setState({ overrideEmail: event.target.value }); },
            numericOnly: false,
        };
        return (
            <div className="panel-body panel-section">
                <p className="font-weight-bold">{this.props.guidance}</p>
                <TextInput {...textInputProps} />
            </div>
        );
    }
    sendDocs(documentId) {
        this.props.handleSendDocumentation(documentId,
            this.props.type, this.props.reference, this.state.overrideEmail);
        this.props.handleShowDocumentPopup('');
    }
    renderFooter() {
        const firstDoc = this.props.documents.FirstDocument;
        const secondDoc = this.props.documents.SecondDocument;
        const firstDocButtonProps = {
            className: 'btn btn-default mr-3',
            onClick: () => this.sendDocs(firstDoc.Document),
        };
        const secondDocButtonProps = {
            className: 'btn btn-default',
            onClick: () => this.sendDocs(secondDoc.Document),
        };
        return (
            <div className="panel-footer panel-section">
                {firstDoc.Show
                    && <button {...firstDocButtonProps}>{firstDoc.Name}</button>}
                {secondDoc.Show
                    && <button {...secondDocButtonProps}>{secondDoc.Name}</button>}
            </div>
        );
    }
    render() {
        return (
            <div className="container request-cancellation">
                <div className="row">
                    <div className=" col-xs-12 col-sm-8 col-sm-push-2 col-md-6 col-md-push-3">
                        <div className="panel panel-basic modal-container">
                            {this.renderHeader()}
                            {this.renderContent()}
                            {this.renderFooter()}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

SendCustomerDocumentation.propTypes = {
    type: React.PropTypes.oneOf(['booking', 'quote']),
    reference: React.PropTypes.string,
    title: React.PropTypes.string,
    guidance: React.PropTypes.string,
    emailLabel: React.PropTypes.string,
    handleSendDocumentation: React.PropTypes.func,
    handleShowDocumentPopup: React.PropTypes.func,
    documents: React.PropTypes.object,
};
