import * as BasketActions from 'actions/bookingjourney/basketactions';
import * as BrandActions from 'actions/lookups/brandActions';
import * as CardTypeActions from 'actions/lookups/cardTypeActions';

import PaymentDetails from 'widgets/bookingjourney/paymentdetails';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PaymentDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            paymentDetailsInitialised: false,
        };
        this.setBasket = this.setBasket.bind(this);
        this.updateBasketValue = this.updateBasketValue.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadCardTypesIfNeeded();
        this.props.actions.loadBrandsIfNeeded();
    }
    componentWillReceiveProps(nextProps) {
        if ((!this.state.paymentDetailsInitialised && nextProps.basket.isLoaded)
            || (nextProps.basket.basket.TotalPrice !== this.props.basket.basket.TotalPrice)) {
            if (this.requiresPaymentDetails()) {
                this.props.actions.updateBasketValue('requiresPaymentDetails', true);
            }

            const paymentDetails = Object.assign({}, nextProps.basket.basket.PaymentDetails);
            const siteConfig = this.props.site.SiteConfiguration;
            const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
            const payments = nextProps.basket.basket.Payments;
            const discountToday = payments.length === 1
                ? nextProps.basket.basket.PromoCodeDiscount : 0;
            const total = bookingJourneyConfig.DefaultDepositPayment
                && (!this.isTrade() || this.isCreditCardAgent())
                ? nextProps.basket.basket.AmountDueToday - discountToday
                : nextProps.basket.basket.TotalAmountDue;

            paymentDetails.Amount = total;
            paymentDetails.PaymentType = 'CreditCard';
            paymentDetails.TotalAmount = total;
            this.props.actions.updateBasketValue('basket.PaymentDetails', paymentDetails);

            if (payments.length > 0) {
                const lastPayment = payments[payments.length - 1];

                if (nextProps.basket.basket.PromoCodeDiscount !== 0) {
                    lastPayment.DisplayAmount = lastPayment.Amount
                        - nextProps.basket.basket.PromoCodeDiscount;
                } else {
                    lastPayment.DisplayAmount = null;
                }
            }

            this.props.actions.updateBasketValue('basket.Payments', payments);

            this.state.paymentDetailsInitialised = true;
        }
    }
    isInitialised() {
        const isInitialised
            = this.props.basket.isLoaded
                && this.props.cardTypes.isLoaded
                && this.props.brands.isLoaded;
        return isInitialised;
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    isCreditCardAgent() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        return tradeSession.CreditCardAgent;
    }
    setBasket(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateBasketValue(`basket.${field}`, value);
    }
    updateBasketValue(field, value) {
        this.props.actions.updateBasketValue(field, value);
    }
    getPaymentDetailsProps() {
        const contentModel = this.props.entity.model;

        const brand = this.props.brands.items.find(b => b.Id === this.props.site.BrandId);
        const sellingGeographyLevel1Id = brand.SellingGeographyLevel1Id;

        const cardTypes = this.props.cardTypes.items.filter(item =>
            item.SellingGeographyLevel1ID === sellingGeographyLevel1Id);

        const props = {
            paymentDetails: this.props.basket.basket.PaymentDetails,
            onChange: this.setBasket,
            updateValue: this.updateBasketValue,
            title: 'Payment Details',
            cardTypes,
            warnings: this.props.basket.warnings,
            labels: {
                cardType: contentModel.Labels.CardType,
                cardNumber: contentModel.Labels.CardNumber,
                cardHoldersName: contentModel.Labels.CardHoldersName,
                startDate: contentModel.Labels.StartDate,
                expiryDate: contentModel.Labels.ExpiryDate,
                securityNumber: contentModel.Labels.SecurityNumber,
            },
            placeholders: {
                cardType: contentModel.Placeholders.CardType,
                month: contentModel.Placeholders.Month,
                year: contentModel.Placeholders.Year,
            },
        };
        return props;
    }
    requiresPaymentDetails() {
        const siteConfig = this.props.site.SiteConfiguration;
        const paymentMode = siteConfig.BookingJourneyConfiguration.PaymentMode;
        if (paymentMode === 'Standard') {
            return (!this.isTrade() || this.isCreditCardAgent());
        }
        return false;
    }
    render() {
        const shouldRender = this.requiresPaymentDetails();
        return (
            <div className="payment-container">
                {this.isInitialised()
                    && shouldRender
                    && <PaymentDetails {...this.getPaymentDetailsProps()} />}
            </div>
        );
    }
}

PaymentDetailsContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    cardTypes: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    brands: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const cardTypes = state.cardTypes ? state.cardTypes : {};
    const site = state.site ? state.site : {};
    const brands = state.brands ? state.brands : {};

    return {
        basket,
        brands,
        cardTypes,
        site,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
    BasketActions,
    BrandActions,
    CardTypeActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PaymentDetailsContainer);
