import Price from 'components/common/price';
import React from 'react';
import ValidateFunctions from '../../library/validateFunctions';

import moment from 'moment';

export default class CompleteBooking extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
        this.validate = this.validate.bind(this);
        this.validateQuote = this.validateQuote.bind(this);
    }
    getPriceProps(amount) {
        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        pricingConfiguration.PerPersonPricing = false;
        pricingConfiguration.PriceFormatDisplay = 'TwoDP';

        const priceProps = {
            amount,
            currency: this.props.currency,
            pricingConfiguration,
        };

        return priceProps;
    }
    getPaymentAmount(payment) {
        return payment.DisplayAmount === undefined
            || payment.DisplayAmount === null
            ? payment.Amount : payment.DisplayAmount;
    }
    validate() {
        this.props.resetWarnings();

        let valid = true;
        if (!this.props.hideCancellationCharges && !this.props.basket.acceptedCancellation) {
            valid = false;
            this.props.onError('Cancellation', this.props.warnings.cancellationTerms);
        }

        if (this.props.basket.basket.Errata.length > 0 && !this.props.basket.acceptedErrata) {
            valid = false;
            this.props.onError('Errata', this.props.warnings.errata);
        }

        if (!this.props.basket.acceptedTAndC) {
            valid = false;
            this.props.onError('TermsAndConditions', this.props.warnings.termsAndConditions);
        }

        if ((this.props.basket.requiresTradeReference
            && (this.props.basket.basket.TradeReference === null)
            || this.props.basket.basket.TradeReference === '')) {
            valid = false;
            this.props.onError('TradeReference', this.props.warnings.tradeReference);
        }

        if (this.props.basket.priceChangeAmount > 0
            && !this.props.basket.hasAcceptedPriceIncrease) {
            valid = false;
            this.props.onError('PriceIncrease', this.props.warnings.priceIncrease);
        }

        if (this.props.basket.requiresLeadGuest) {
            if (!this.validateLeadGuest()) {
                valid = false;
            }
        }

        if (this.props.basket.requiresPaymentDetails) {
            if (!this.validatePaymentDetails()) {
                valid = false;
            }
        }

        let roomIndex = 0;
        this.props.basket.basket.Rooms.forEach(
            room => {
                let guestIndex = 0;
                room.Guests.forEach(
                    guest => {
                        const guestKey = `Rooms[${roomIndex}].Guests[${guestIndex}]`;
                        if (!guest.FirstName) {
                            valid = false;
                            const errorKey = `${guestKey}.FirstName`;
                            this.props.onError(errorKey,
                                this.props.warnings.guest.firstName);
                        }

                        if (!guest.LastName) {
                            valid = false;
                            this.props.onError(`${guestKey}.LastName`,
                                this.props.warnings.guest.lastName);
                        }

                        if (!guest.Title) {
                            valid = false;
                            this.props.onError(`${guestKey}.Title`,
                                this.props.warnings.guest.title);
                        }

                        const requiresDoB = this.props.basket.basket.GuestsRequireDoB;
                        if (requiresDoB
                            && (!guest.DateOfBirth
                                || guest.DateOfBirth === '0001-01-01T00:00:00')) {
                            valid = false;
                            const errorKey = `${guestKey}.DateOfBirth`;
                            this.props.onError(errorKey,
                                this.props.warnings.guest.dateOfBirth);
                        }

                        if (requiresDoB && this.props.validateChildInfantAges) {
                            const humanStage = guest.Type;
                            if (humanStage !== 'Adult') {
                                const dateOfBirth = moment(guest.DateOfBirth);
                                const holidayDuration
                                    = this.props.basket.basket.SearchDetails.Duration;
                                const returnDate
                                    = moment(this.props.basket.basket.SearchDetails.DepartureDate)
                                    .add(holidayDuration, 'days');
                                const returnAge = returnDate.diff(dateOfBirth, 'years');
                                const errorKey = `${guestKey}.DateOfBirth`;
                                if (humanStage === 'Child') {
                                    const prePopulatedAge = guest.Age;
                                    if (prePopulatedAge !== returnAge) {
                                        valid = false;
                                        this.props.onError(errorKey,
                                            this.props.warnings.childAgeNotValid);
                                    }
                                }
                                if (humanStage === 'Infant') {
                                    if (returnAge >= 2) {
                                        valid = false;
                                        this.props.onError(errorKey,
                                            this.props.warnings.infantAgeNotValid);
                                    }
                                }
                            }
                        }
                        guestIndex += 1;
                    });
                roomIndex += 1;
            });

        if (valid) {
            this.props.onBook();
        } else {
            window.scrollTo(0, 0);
        }
    }
    validateLeadGuest() {
        let valid = this.validateLeadGuestTextField('FirstName',
            this.props.warnings.guest.firstName);
        valid = this.validateLeadGuestTextField('LastName',
            this.props.warnings.guest.lastName) && valid;
        valid = this.validateLeadGuestTextField('Title',
            this.props.warnings.guest.title) && valid;
        valid = this.validateLeadGuestTextField('AddressLine1',
            this.props.warnings.guest.address) && valid;
        valid = this.validateLeadGuestTextField('TownCity',
            this.props.warnings.guest.city) && valid;
        valid = this.validateLeadGuestTextField('Postcode',
            this.props.warnings.guest.postcode) && valid;

        if (!this.props.basket.basket.LeadGuest.BookingCountryID) {
            valid = false;
            this.props.onError('LeadGuest.Country', this.props.warnings.guest.country);
        }
        if (!ValidateFunctions.isEmail(this.props.basket.basket.LeadGuest.Email)) {
            valid = false;
            this.props.onError('LeadGuest.Email', this.props.warnings.guest.email);
        }
        if (!ValidateFunctions.isNumericPhoneNumber(this.props.basket.basket.LeadGuest.Phone)) {
            valid = false;
            this.props.onError('LeadGuest.Phone', this.props.warnings.guest.phone);
        }
        return valid;
    }

    validateLeadGuestTextField(field, warning) {
        let valid = true;
        if (!this.props.basket.basket.LeadGuest[field]) {
            valid = false;
            this.props.onError(`LeadGuest.${field}`, warning);
        }
        return valid;
    }
    validatePaymentDetails() {
        let valid = true;
        const paymentDetails = this.props.basket.basket.PaymentDetails;
        const today = new Date();
        const warnings = this.props.warnings.paymentDetails;

        if (!paymentDetails.CardTypeID) {
            valid = false;
            this.props.onError('PaymentDetails.CardTypeID', warnings.cardType);
        }

        if (!paymentDetails.CardHoldersName) {
            valid = false;
            this.props.onError('PaymentDetails.CardHoldersName', warnings.cardHoldersName);
        }

        if (!paymentDetails.CardNumber) {
            valid = false;
            this.props.onError('PaymentDetails.CardNumber', warnings.cardNumber);
        }

        if (!paymentDetails.ExpiryYear
            || !paymentDetails.ExpiryMonth
            || (paymentDetails.ExpiryYear === today.getFullYear()
                    && paymentDetails.ExpiryMonth < today.getMonth() + 1)) {
            valid = false;
            this.props.onError('PaymentDetails.ExpiryDate', warnings.expiryDate);
        }

        if (!paymentDetails.SecurityNumber) {
            valid = false;
            this.props.onError('PaymentDetails.SecurityNumber', warnings.securityNumber);
        }

        return valid;
    }
    validateQuote() {
        this.props.resetWarnings();

        let valid = true;

        if ((this.props.basket.requiresTradeReference
            && (this.props.basket.basket.TradeReference === null)
            || this.props.basket.basket.TradeReference === '')) {
            valid = false;
            this.props.onError('TradeReference', this.props.warnings.tradeReference);
        }

        if (valid) {
            this.props.onQuote();
        } else {
            window.scrollTo(0, 0);
        }
    }
    renderHeader() {
        return (
            <header className="panel-header">
                <h2 className="h-tertiary">{this.props.title}</h2>
            </header>
        );
    }

    renderPaymentBreakdown() {
        const paymentDetails = this.props.basket.basket.PaymentDetails;
        return (
            <div className="row">
                  <div className="complete-payment col-xs-12 col-md-9">
                    <dl className="data-list row">
                        <dt className="col-xs-6">Amount</dt>
                        <dd className="col-xs-6 text-right">
                            <Price {...this.getPriceProps(paymentDetails.Amount)} />
                        </dd>
                        <dt className="col-xs-6">Surcharge</dt>
                        <dd className="col-xs-6 text-right">
                            <Price {...this.getPriceProps(paymentDetails.Surcharge)} />
                        </dd>
                    </dl>
                </div>
            </div>
        );
    }
    renderTotalPayment() {
        const basket = this.props.basket.basket;
        const paymentDetails = basket.PaymentDetails;
        const isDeposit = paymentDetails.Amount < basket.TotalAmountDue;
        const totalLabel = isDeposit ? 'Deposit Amout' : 'Total Amount';
        let totalCommission = 0;
        let vatOnCommission = 0;
        const discount = basket.PromoCodeDiscount;
        let commissionOnDiscount = 0;
        const percentage = 100;
        basket.Components.forEach(component => {
            totalCommission += component.TotalCommission;
            vatOnCommission += component.VATOnCommission;
        });

        if (basket.PromoCodeDiscount > 0 && basket.IsTrade) {
            commissionOnDiscount = discount * basket.CommissionPercentage / percentage;
            totalCommission -= commissionOnDiscount;
        }

        let dataListClass = 'data-list row';
        if (isDeposit || totalCommission > 0) {
            dataListClass += ' my-0';
        }

        return (
            <div className="complete-payment col-xs-12 col-md-9">
                <dl className={dataListClass}>
                    <dt className="col-xs-6 font-weight-bold" key="total-dt">{totalLabel}</dt>
                    <dd className="col-xs-6 text-right font-weight-bold" key="total-dd">
                        <Price {...this.getPriceProps(paymentDetails.TotalAmount)} />
                    </dd>
                    {isDeposit && basket.Payments.map(this.renderPaymentPlan, this)}
                    {totalCommission > 0 && this.renderCommission(totalCommission, vatOnCommission)}
                </dl>
            </div>
        );
    }
    renderCommission(commission, vatOnCommission) {
        const commissionElements = [];
        commissionElements.push(
            <dt className="col-xs-6" key="commission-dt">Commission</dt>
        );
        commissionElements.push(
            <dd className="col-xs-6 text-right" key="commission-dd">
                <Price {...this.getPriceProps(commission)} />
            </dd>
        );
        commissionElements.push(
            <dt className="col-xs-6" key="vat-commission-dt">VAT On Commission</dt>
        );
        commissionElements.push(
            <dd className="col-xs-6 text-right" key="vat-commission-dd">
                <Price {...this.getPriceProps(vatOnCommission)} />
            </dd>
        );
        return commissionElements;
    }
    renderPaymentPlan(payment, index) {
        const paymentPlanElements = [];

        const paymentDate = moment(payment.DateDue);
        if (paymentDate.isAfter(moment())) {
            const label = `Amount due by ${paymentDate.format('ll')}`;
            paymentPlanElements.push(
                <dt className="col-xs-6" key={`payment-dt-${index}`}>{label}</dt>
            );
            paymentPlanElements.push(
                <dd className="col-xs-6 text-right" key={`payment-dd-${index}`}>
                    <Price {...this.getPriceProps(this.getPaymentAmount(payment)) } />
                </dd>
            );
        }
        return paymentPlanElements;
    }
    renderQuoteButton() {
        const buttonProps = {
            type: 'button',
            className: 'btn btn-default btn-lg btn-quote btn-block-xs',
            onClick: this.validateQuote,
        };
        if (!this.props.nonTransacting) {
            buttonProps.className += ' mr-3';
        }
        return (
            <button {...buttonProps}>Save Quote</button>
        );
    }
    renderBookButton() {
        return (
            <button type="button"
                className="btn btn-primary btn-lg btn-search btn-block-xs"
                onClick={this.validate}>
                {this.props.button && this.props.button.Text}
            </button>
        );
    }
    renderRHPButton() {
        return (
            <button type="button"
                className="btn btn-primary btn-lg btn-rhp btn-block-xs"
                 onClick ={() => (window.location.href = '/')}>
                    {this.props.ReturnHomeButton && this.props.ReturnHomeButton.Text}
            </button>
        );
    }
    render() {
        const paymentDetails = this.props.basket.basket.PaymentDetails;
        return (
            <div className="panel panel-basic">
                {this.props.title
                    && this.renderHeader()}
                <div className="panel-body">
                    {this.props.message
                        && <p className="mb-2">{this.props.message}</p>}
                    {paymentDetails.Surcharge > 0
                        && this.renderPaymentBreakdown()}
                    <div className="row">
                        {this.props.displayPriceBreakdown
                            && this.renderTotalPayment()}
                        <div className="col-xs-12 col-md-7 text-align-right-xs right">
                            {this.props.displayQuoteOption
                                && this.renderQuoteButton()}
                            {!this.props.nonTransacting && !this.props.isQuoteCreated
                                && this.renderBookButton()}
                            {this.props.isQuoteCreated
                                && this.renderRHPButton()}
                         </div>
                         {this.props.basket.warnings.PriceIncrease
                            && <div className="col-xs-12">
                                    <p className="text-danger">
                                        {this.props.basket.warnings.PriceIncrease}</p>
                                </div>}
                    </div>
                </div>
            </div>
        );
    }
}

CompleteBooking.propTypes = {
    basket: React.PropTypes.object,
    onBook: React.PropTypes.func.isRequired,
    onError: React.PropTypes.func.isRequired,
    onQuote: React.PropTypes.func.isRequired,
    resetWarnings: React.PropTypes.func.isRequired,

    title: React.PropTypes.string,
    message: React.PropTypes.string,
    button: React.PropTypes.object,
    ReturnHomeButton: React.PropTypes.object,

    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    displayPriceBreakdown: React.PropTypes.bool,
    hideCancellationCharges: React.PropTypes.bool,
    validateChildInfantAges: React.PropTypes.bool,

    nonTransacting: React.PropTypes.bool,
    isQuoteCreated: React.PropTypes.bool,
    displayQuoteOption: React.PropTypes.bool,

    warnings: React.PropTypes.shape({
        guest: React.PropTypes.shape({
            title: React.PropTypes.string,
            firstName: React.PropTypes.string,
            lastName: React.PropTypes.string,
            dateOfBirth: React.PropTypes.string,
            address: React.PropTypes.string,
            city: React.PropTypes.string,
            postcode: React.PropTypes.string,
            country: React.PropTypes.string,
            phone: React.PropTypes.string,
            email: React.PropTypes.string,
        }),
        paymentDetails: React.PropTypes.shape({
            cardType: React.PropTypes.string,
            cardHoldersName: React.PropTypes.string,
            cardNumber: React.PropTypes.string,
            expiryDate: React.PropTypes.string,
            securityNumber: React.PropTypes.string,
        }),
        cancellationTerms: React.PropTypes.string,
        errata: React.PropTypes.string,
        priceIncrease: React.PropTypes.string,
        termsAndConditions: React.PropTypes.string,
        tradeReference: React.PropTypes.string,
        childAgeNotValid: React.PropTypes.string,
        infantAgeNotValid: React.PropTypes.string,
    }),
};

CompleteBooking.defaultProps = {
};
