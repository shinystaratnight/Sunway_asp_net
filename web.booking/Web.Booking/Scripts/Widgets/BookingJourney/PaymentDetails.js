import DateSelectInput from 'components/form/dateselectinput';
import React from 'react';
import SelectInput from 'components/form/selectinput';
import TextInput from 'components/form/textinput';

export default class PaymentDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
        this.handleCardTypeChange = this.handleCardTypeChange.bind(this);
        this.handleCardNumberChange = this.handleCardNumberChange.bind(this);
        this.handleSecurityNumberChange = this.handleSecurityNumberChange.bind(this);
    }
    handleCardTypeChange(event) {
        const field = event.target.name;
        const cardTypeId = parseInt(event.target.value, 10);
        const percentage = 100;

        const selectedCardType = this.props.cardTypes.find(cardType => cardType.Id === cardTypeId);
        const amount = this.props.paymentDetails.Amount;
        let surcharge = 0;
        if (selectedCardType.SurchargePercentage) {
            surcharge = amount * (selectedCardType.SurchargePercentage / percentage);
        }
        const totalAmount = amount + surcharge;

        if (surcharge !== this.props.paymentDetails.Surcharge) {
            this.props.updateValue('basket.PaymentDetails.Surcharge', surcharge);
        }

        if (totalAmount !== this.props.paymentDetails.TotalAmount) {
            this.props.updateValue('basket.PaymentDetails.TotalAmount', totalAmount);
        }

        this.props.updateValue(`basket.${field}`, cardTypeId);
    }
    handleCardNumberChange(event) {
        const value = event.target.value;
        this.props.updateValue('basket.PaymentDetails.CardNumber', value.toString());
    }
    handleSecurityNumberChange(event) {
        const value = event.target.value;
        this.props.updateValue('basket.PaymentDetails.SecurityNumber', value.toString());
    }
    getCardTypeProps() {
        const paymentDetails = this.props.paymentDetails;

        const options = [];
        this.props.cardTypes.forEach(cardType => {
            const option = {
                Id: cardType.Id,
                Name: cardType.Name,
            };
            if (cardType.SurchargePercentage) {
                option.Name += ` (+${cardType.SurchargePercentage}% surcharge)`;
            }
            options.push(option);
        });

        const props = {
            name: 'PaymentDetails.CardTypeID',
            label: this.props.labels.cardType,
            placeholder: this.props.placeholders.cardType,
            required: true,
            onChange: this.handleCardTypeChange,
            value: paymentDetails.CardTypeID ? paymentDetails.CardTypeID : 0,
            options,
            error: this.props.warnings['PaymentDetails.CardTypeID'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            selectContainerClass: 'col-xs-12 col-md-4',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };
        return props;
    }
    getCardNumberProps() {
        const maxCreditCardLength = 19;
        const paymentDetails = this.props.paymentDetails;
        const props = {
            name: 'PaymentDetails.CardNumber',
            label: this.props.labels.cardNumber,
            required: true,
            type: 'text',
            onChange: this.handleCardNumberChange,
            maxLength: maxCreditCardLength,
            value: paymentDetails.CardNumber ? paymentDetails.CardNumber : '',
            error: this.props.warnings['PaymentDetails.CardNumber'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-4',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            numericOnly: true,
        };
        return props;
    }
    getCardHoldersNameProps() {
        const paymentDetails = this.props.paymentDetails;
        const props = {
            name: 'PaymentDetails.CardHoldersName',
            label: this.props.labels.cardHoldersName,
            required: true,
            type: 'text',
            onChange: this.props.onChange,
            value: paymentDetails.CardHoldersName ? paymentDetails.CardHoldersName : '',
            error: this.props.warnings['PaymentDetails.CardHoldersName'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-6',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };
        return props;
    }
    getSecurityNumberProps() {
        const paymentDetails = this.props.paymentDetails;
        const props = {
            name: 'PaymentDetails.SecurityNumber',
            label: this.props.labels.securityNumber,
            required: true,
            type: 'text',
            onChange: this.handleSecurityNumberChange,
            value: paymentDetails.SecurityNumber ? paymentDetails.SecurityNumber : '',
            error: this.props.warnings['PaymentDetails.SecurityNumber'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-2',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            numericOnly: true,
            maxLength: 3,
        };
        return props;
    }
    getStartDateProps() {
        const paymentDetails = this.props.paymentDetails;
        const today = new Date();
        const maxYearsBehind = 10;
        const minYear = today.getFullYear() - maxYearsBehind;
        const maxYear = today.getFullYear();

        let startDate = null;
        if (paymentDetails.StartMonth && paymentDetails.StartYear) {
            startDate = new Date(1, paymentDetails.StartMonth - 1, paymentDetails.StartYear);
        }

        const props = {
            name: 'PaymentDetails.StartDate',
            label: this.props.labels.startDate,
            required: false,
            updateValue: this.props.updateValue,
            value: startDate,
            error: this.props.warnings['PaymentDetails.StartDate'],
            hideDaySelect: true,
            minYear,
            maxYear,
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            selectContainerClass: 'col-xs-6 col-md-2',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            placeholders: {
                month: this.props.placeholders.month,
                year: this.props.placeholders.year,
            },
            fields: {
                month: 'basket.PaymentDetails.StartMonth',
                year: 'basket.PaymentDetails.StartYear',
            },
        };
        return props;
    }
    getExpiryDateProps() {
        const today = new Date();
        const maxYearsAhead = 20;
        const minYear = today.getFullYear();
        const maxYear = today.getFullYear() + maxYearsAhead;
        const paymentDetails = this.props.paymentDetails;

        let expiryDate = null;
        if (paymentDetails.ExpiryMonth && paymentDetails.ExpiryYear) {
            expiryDate = new Date(1, paymentDetails.ExpiryMonth - 1, paymentDetails.ExpiryYear);
        }

        const props = {
            name: 'PaymentDetails.ExpiryDate',
            label: this.props.labels.expiryDate,
            required: true,
            updateValue: this.props.updateValue,
            value: expiryDate,
            error: this.props.warnings['PaymentDetails.ExpiryDate'],
            hideDaySelect: true,
            minYear,
            maxYear,
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            selectContainerClass: 'col-xs-6 col-md-2',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            placeholders: {
                month: this.props.placeholders.month,
                year: this.props.placeholders.year,
            },
            fields: {
                month: 'basket.PaymentDetails.ExpiryMonth',
                year: 'basket.PaymentDetails.ExpiryYear',
            },
        };
        return props;
    }
    render() {
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                    <p>* Required fields</p>
                </header>
                <div className="panel-body">
                    <SelectInput {...this.getCardTypeProps()} />
                    <TextInput {...this.getCardNumberProps()} />
                    <TextInput {...this.getCardHoldersNameProps()} />
                    <DateSelectInput {...this.getStartDateProps()} />
                    <DateSelectInput {...this.getExpiryDateProps()} />
                    <TextInput {...this.getSecurityNumberProps()} />
                 </div>
            </div>
        );
    }
}

PaymentDetails.propTypes = {
    paymentDetails: React.PropTypes.object.isRequired,
    onChange: React.PropTypes.func.isRequired,
    updateValue: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    labels: React.PropTypes.shape({
        cardType: React.PropTypes.string.isRequired,
        cardNumber: React.PropTypes.string.isRequired,
        cardHoldersName: React.PropTypes.string.isRequired,
        startDate: React.PropTypes.string.isRequired,
        expiryDate: React.PropTypes.string.isRequired,
        securityNumber: React.PropTypes.string.isRequired,
    }),
    placeholders: React.PropTypes.shape({
        cardType: React.PropTypes.string,
        month: React.PropTypes.string,
        year: React.PropTypes.string,
    }),
    cardTypes: React.PropTypes.array.isRequired,
    warnings: React.PropTypes.object.isRequired,
};
