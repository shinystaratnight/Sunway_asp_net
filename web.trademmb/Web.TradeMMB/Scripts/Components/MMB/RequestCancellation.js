import CheckboxInput from '../form/checkboxinput';
import React from 'react';
import TextInput from '../../components/form/textinput';
import ValidateFunctions from '../../library/validateFunctions';
import fetch from 'isomorphic-fetch';

export default class RequestCancellation extends React.Component {
    constructor() {
        super();
        this.state = {};
    }
    checkAllSelected() {
        return this.props.selectedComponentTokens
            .indexOf(this.props.cancellationInformation.all.Token) !== -1;
    }
    getCancellationTotal() {
        const selectedComponents = this.getSelectedComponents();
        const allSelected = this.checkAllSelected();
        let totalSelected = 0;
        if (allSelected) {
            totalSelected = this.props.cancellationInformation.all.CancellationCost;
        } else {
            selectedComponents.forEach(sc => {
                totalSelected += sc.CancellationCost;
            });
        }
        return `${this.props.currencySymbol}${totalSelected.toFixed(2)}`;
    }
    getSelectedComponents() {
        return this.props.cancellationInformation.components
            .filter(c => this.props.selectedComponentTokens.indexOf(c.Token) !== -1);
    }
    renderHeader() {
        const btnProps = {
            onClick: () => {
                this.props.handleToggleCancellationCharges(this.props.bookingReference);
                this.props.resetWarnings();
                this.props.resetResponseMessage();
                this.props.resetFormValues();
            },
            className: 'fa fa-times',
        };

        const message = this.props.labels.CancelRequestText !== ''
             ? this.props.labels.CancelRequestText : 'Please select at least one component or all';

        return (
            <div className="panel-header panel-section float-left">
                    <h3 className=" h-tertiary">Request cancellation</h3>
                    <span {...btnProps}></span>
                    <p>{message}</p>
            </div>
        );
    }
    renderContentRow(component, index) {
        const key = `${this.props.bookingReference}-${index}-content-row`;
        const value = this.props.selectedComponentTokens.indexOf(component.Token) !== -1;
        const disabled = this.props.selectedComponentTokens
            .indexOf(this.props.cancellationInformation.all.Token) !== -1
            && component.Type !== 'All';
        const checkboxProps = {
            name: `${this.props.bookingReference}-${index}`,
            label: '',
            disabled,
            onChange: () => this.props.handleComponentToggle(component.Type, component.Token),
            value,
            required: false,
            displayInline: false,
            containerClass: '',
        };
        const cost = `${this.props.currencySymbol}${component.CancellationCost}`;
        return (
            <div key={key} className="content-row float-left">
                <p className="component-name">{component.Type}</p>
                <p className="component-fee">{cost}</p>
                <CheckboxInput {...checkboxProps} />
            </div>
        );
    }
    renderContent() {
        const cancellationInformation = this.props.cancellationInformation.components;
        return (
            <div className="panel-body panel-section float-left">
                <p className="component-name font-weight-bold">Component</p>
                <p className="component-fee font-weight-bold">Cancellation fee</p>
                {cancellationInformation.map(this.renderContentRow, this)}
                {this.renderContentRow(this.props.cancellationInformation.all, 1)}
            </div>
        );
    }
    renderEmailFields() {
        const success = Object.keys(this.props.emailForm.warnings).length > 0
                            ? 'warning' : 'success';
        const messageClass = `message ${success}`;
        return (
            <div className="col-xs-12 col-md-12 email-details">
                {this.props.emailForm.message !== ''
                    && <p className={messageClass}>{this.props.emailForm.message}</p>}
                <TextInput type="textarea"
                    onChange={this.props.onChange}
                    name="Message"
                    placeholder="Message *"
                    error={this.props.emailForm.warnings.Message}/>
                <TextInput onChange={this.props.onChange}
                    name="ContactName"
                    placeholder="Contact Name *"
                    error={this.props.emailForm.warnings.ContactName}/>
                <TextInput onChange={this.props.onChange}
                    name="ContactTelephone"
                    placeholder="Contact Telephone *"
                    error={this.props.emailForm.warnings.ContactTelephone}/>
                <TextInput onChange={this.props.onChange}
                    name="ContactEmailAddress"
                    placeholder="Contact Email Address *"
                    error={this.props.emailForm.warnings.ContactEmailAddress}/>
            </div>
        );
    }
    renderFooter() {
        const selectedComponents = this.getSelectedComponents();
        const allSelected = this.checkAllSelected();
        const totalSelected = this.getCancellationTotal();
        const validSelection = allSelected || selectedComponents.length > 0;
        const validClass = !validSelection ? 'disabled' : '';
        const buttonProps = {
            className: `btn btn-default btn-cancel ${validClass}`,
            disabled: !validSelection,
            onClick: this.props.sendCancellationEmail
                           ? this.validate.bind(this)
                           : this.props.handleCancelComponents.bind(this.props.bookingReference),
        };
        let buttonText = 'Cancel';
        if (this.props.sendCancellationEmail
                && this.props.labels.SendEmailCancellationRequestButtonLabel !== '') {
            buttonText = this.props.labels.SendEmailCancellationRequestButtonLabel;
        } else if (this.props.labels.CancelBookingButtonLabel !== '') {
            buttonText = this.props.labels.CancelBookingButtonLabel;
        }
        return (
             <div className="panel-footer panel-section float-left">
                <p className="component-name">Total Cancellation Fee</p>
                <p className="component-fee">{totalSelected}</p>
                {this.props.sendCancellationEmail
                    && this.renderEmailFields()}
                <button {...buttonProps}>{buttonText}</button>
            </div>
        );
    }
    render() {
        const contentClass = 'col-xs-12 col-sm-8 col-sm-push-2 col-md-6 col-md-push-3';
        return (
            <div className="container request-cancellation">
                <div className="row">
                    <div className={contentClass}>
                        <div className="content panel panel-basic">
                            {this.renderHeader()}
                            {this.renderContent()}
                            {this.renderFooter()}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    formatComponents() {
        const components = this.getSelectedComponents();
        const formattedComponents = {};
        if (this.checkAllSelected()) {
            const price = this.props.cancellationInformation.all.CancellationCost;
            formattedComponents.All = `${this.props.currencySymbol}${price.toFixed(2)}`;
        } else {
            Object.keys(components)
            .map(key => {
                const name = components[key].Type;
                const cost = `${this.props.currencySymbol}${components[key].CancellationCost}`;
                formattedComponents[name] = cost;
            });
        }
        return formattedComponents;
    }
    populateCancellationEmail(emailModel) {
        const components = this.formatComponents();
        const email = Object.assign({}, emailModel);
        Object.keys(components)
            .map(key => {
                email.EmailBody[key] = components[key];
            });

        email.EmailBody['Total Cancellation Fee'] = this.getCancellationTotal();
        email.EmailBody.Message = this.props.emailForm.fields.Message;
        email.EmailBody.Contact = this.props.emailForm.fields.ContactName;
        email.EmailBody['Telephone Number']
            = this.props.emailForm.fields.ContactTelephone.toString();
        email.EmailBody['Contact Email Address']
            = this.props.emailForm.fields.ContactEmailAddress;
        return email;
    }
    sendCancellationEmail() {
        const bookingRef = this.props.bookingReference;
        let emailModel = {
            EmailSubject: `Cancellation Request ${bookingRef}`,
            FromEmail: this.props.emailForm.fields.ContactEmailAddress,
            From: this.props.emailForm.fields.ContactName,
            ToEmail: this.props.cancellationRequestEmailAddress,
            EmailBody: {
                '': `Please cancel the following components on booking ${bookingRef}:`,
            },
            EmailFooter: {
                '': this.props.labels.CancelRequestText,
            },
        };
        emailModel = this.populateCancellationEmail(emailModel);

        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(emailModel),
        };
        const cancellationRequestEmailURL = '/tradebookings/api/email/send';
        return fetch(cancellationRequestEmailURL, fetchOptions)
            .then(response => response.json())
            .then(response => {
                if (response) {
                    this.sendCancellationCustomerEmail();
                }
            });
    }
    sendCancellationCustomerEmail() {
        const bookingRef = this.props.bookingReference;
        let emailModel = {
            EmailSubject: `Cancellation Request ${bookingRef}`,
            FromEmail: this.props.cancellationRequestEmailAddress,
            From: this.props.cancellationRequestEmailAddress,
            ToEmail: this.props.emailForm.fields.ContactEmailAddress,
            EmailBody: {
                '': 'Thank you for your cancellation request. We will respond as soon as possible.',
                'Booking': this.props.bookingReference,
            },
            EmailFooter: {
                '': this.props.labels.CancelRequestText,
            },
        };
        emailModel = this.populateCancellationEmail(emailModel);

        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'POST',
            credentials: 'same-origin',
            body: JSON.stringify(emailModel),
        };
        const cancellationRequestEmailURL = '/tradebookings/api/email/send';
        return fetch(cancellationRequestEmailURL, fetchOptions);
    }
    validate() {
        this.props.resetWarnings();
        let formValid = true;
        if (this.props.emailForm.fields.Message === '') {
            this.props.onError('Message', 'Required field *');
            formValid = false;
        }
        if (this.props.emailForm.fields.ContactName === '') {
            this.props.onError('ContactName', 'Required field *');
            formValid = false;
        }
        if (!ValidateFunctions.isNumericPhoneNumber(this.props.emailForm.fields.ContactTelephone)
            || this.props.emailForm.fields.ContactTelephone === '') {
            this.props.onError('ContactTelephone', 'Must be a valid phone number *');
            formValid = false;
        }
        if (!ValidateFunctions.isEmail(this.props.emailForm.fields.ContactEmailAddress)) {
            this.props.onError('ContactEmailAddress', 'Must be a valid Email Address *');
            formValid = false;
        }
        if (!ValidateFunctions.isEmail(this.props.cancellationRequestEmailAddress)) {
            formValid = false;
        }
        if (formValid) {
            this.sendCancellationEmail();
            this.props.emailResponseMessage(this.props.labels.CancelRequestResponseMessage);
        } else {
            this.props.emailResponseMessage(this.props.labels.CancelRequestResponseErrorMessage);
        }
        window.scrollTo(0, 0);
    }
}

RequestCancellation.propTypes = {
    currencySymbol: React.PropTypes.string,
    bookingReference: React.PropTypes.string,
    cancellationInformation: React.PropTypes.object,
    selectedComponentTokens: React.PropTypes.array,
    handleToggleCancellationCharges: React.PropTypes.func.isRequired,
    handleComponentToggle: React.PropTypes.func.isRequired,
    handleCancelComponents: React.PropTypes.func.isRequired,
    sendCancellationEmail: React.PropTypes.bool,
    labels: React.PropTypes.object,
    onChange: React.PropTypes.func,
    onError: React.PropTypes.func,
    resetWarnings: React.PropTypes.func,
    emailForm: React.PropTypes.object,
    cancellationRequestEmailAddress: React.PropTypes.string,
    emailResponseMessage: React.PropTypes.func,
    resetResponseMessage: React.PropTypes.func,
    resetFormValues: React.PropTypes.func,
};
