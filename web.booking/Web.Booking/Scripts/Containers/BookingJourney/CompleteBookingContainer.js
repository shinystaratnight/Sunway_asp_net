import * as AlertActions from '../../actions/bookingjourney/alertActions';
import * as AlertConstants from '../../constants/alerts';
import * as BasketActions from '../../actions/bookingjourney/basketactions';
import * as EntityActions from 'actions/content/entityActions';
import * as QuoteActions from 'actions/bookingjourney/quoteActions';
import * as SearchActions from 'actions/bookingjourney/searchActions';
import * as SearchResultActions from 'actions/bookingjourney/searchResultActions';

import CompleteBooking from '../../widgets/bookingjourney/completeBooking';
import ExpiredWarning from 'components/bookingjourney/expiredwarning';
import ModalPopup from 'components/common/modalpopup';
import React from 'react';
import UrlFunctions from '../../library/urlfunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class CompleteBookingContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            resultExpired: false,
            messagesLoaded: false,
            displayQuoteCreatedPopup: false,
        };
        this.resultTimeout = {};
        this.resultExpiryTime = 1800000;
        this.handleBook = this.handleBook.bind(this);
        this.handleError = this.handleError.bind(this);
        this.resultExpired = this.resultExpired.bind(this);
        this.handleQuote = this.handleQuote.bind(this);
    }
    componentDidMount() {
        this.checkWarning();
    }
    componentWillReceiveProps(nextProps) {
        if (!nextProps.basket.isLoaded
            && !nextProps.basket.isLoading) {
            const basketToken = UrlFunctions.getQueryStringValue('t');
            this.props.actions.loadBasket(basketToken);
        }
        if (!this.props.basket.isLoaded
                && nextProps.basket.isLoaded) {
            this.resultTimeout = setTimeout(this.resultExpired, this.resultExpiryTime);
        }
        if (!this.state.messagesLoaded) {
            const context = `default-${this.props.site.Name.toLowerCase()}`;
            this.state.messagesLoaded = true;
            this.props.actions.loadEntity('Shared', 'Warnings', context, 'dev');
        }

        if (this.props.basket.isBooking
            && !nextProps.basket.isBooking
            && nextProps.basket.isBookingFailed) {
            if (nextProps.basket.isPaymentFailed) {
                this.addPaymentWarning();
            } else {
                this.addFailedBookingWarning();
            }
        }

        if (this.props.basket.isCreatingQuote
            && nextProps.basket.isQuoteCreated) {
            this.setState({ displayQuoteCreatedPopup: true });
        }
    }
    addFailedBookingWarning() {
        this.props.actions.addAlert('CompleteBooking_BookingFailed',
            AlertConstants.ALERT_TYPES.DANGER,
            this.props.entity.model.Warnings.FailedBooking);
    }
    addPaymentWarning() {
        this.props.actions.addAlert('CompleteBooking_PaymentFailed',
            AlertConstants.ALERT_TYPES.DANGER,
            this.props.entity.model.Warnings.FailedPayment);
    }
    checkWarning() {
        const warning = UrlFunctions.getQueryStringValue('warning');
        if (warning === 'paymentfailed') {
            this.addPaymentWarning();
        }
    }
    resultExpired() {
        this.setState({ resultExpired: true });
    }
    handleBook() {
        if (!this.props.basket.isBooking) {
            this.props.actions.basketBook(
                this.props.basket.basket.BasketToken,
                this.props.basket.basket,
                this.props.basket.hotelRequest);
        }
    }
    handleError(key, value) {
        this.props.actions.addWarning(key, value);
        this.props.actions.addAlert('CompleteBooking_MandatoryFields',
            AlertConstants.ALERT_TYPES.DANGER,
            this.props.entity.model.Warnings.MandatoryFields);
    }
    handleQuote() {
        this.props.actions.createQuote(this.props.basket.basket);
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    getDeeplinkSearchUrl() {
        const currentPath = window.location.pathname;
        const deeplinkUrl = currentPath.replace('payment', 'results');
        return (deeplinkUrl);
    }
    getCompleteBookingProps() {
        const displayPriceBreakdown = !this.props.session.UserSession.OverBranded
            && this.props.entity.model.DisplayPriceBreakdown;
        const contentModel = this.props.entity.model;
        const warnings = contentModel.Warnings;
        const trade = this.props.tradeSession.Trade;
        const siteConfig = this.props.site.SiteConfiguration;
        const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
        const completeBookingProps = {
            basket: this.props.basket,
            onBook: this.handleBook,
            onError: this.handleError,
            onQuote: this.handleQuote,
            resetWarnings: this.props.actions.resetWarnings,

            title: contentModel.Title,
            message: contentModel.Message,
            button: this.props.entity.model.Button,
            ReturnHomeButton: this.props.entity.model.ReturnHomeButton,

            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: siteConfig.PricingConfiguration,
            displayPriceBreakdown,
            nonTransacting: this.isTrade() && trade && trade.NonTransacting,
            isQuoteCreated: this.props.basket.isQuoteCreated,
            hideCancellationCharges: bookingJourneyConfig.HideCancellationCharges,
            validateChildInfantAges: bookingJourneyConfig.ValidateChildInfantAges,

            warnings: {
                guest: {
                    title: warnings.Guest.Title,
                    firstName: warnings.Guest.FirstName,
                    lastName: warnings.Guest.LastName,
                    dateOfBirth: warnings.Guest.DateOfBirth,
                    address: warnings.Guest.Address,
                    city: warnings.Guest.City,
                    postcode: warnings.Guest.Postcode,
                    country: warnings.Guest.Country,
                    phone: warnings.Guest.Phone,
                    email: warnings.Guest.Email,
                },
                paymentDetails: {
                    cardType: warnings.PaymentDetails.CardType,
                    cardHoldersName: warnings.PaymentDetails.CardHoldersName,
                    cardNumber: warnings.PaymentDetails.CardNumber,
                    expiryDate: warnings.PaymentDetails.ExpiryDate,
                    securityNumber: warnings.PaymentDetails.SecurityNumber,
                },
                cancellationTerms: warnings.CancellationTerms,
                errata: warnings.Errata,
                priceIncrease: warnings.PriceIncrease,
                termsAndConditions: warnings.TermsAndConditions,
                tradeReference: warnings.TradeReference,
                childAgeNotValid: warnings.ChildAgeNotValid,
                infantAgeNotValid: warnings.InfantAgeNotValid,
            },
        };
        return completeBookingProps;
    }
    renderExpiredPopup() {
        const expiredWarningProps = {
            onClickLink: this.getDeeplinkSearchUrl(),
            title: this.props.warningMessage.Title,
            body: this.props.warningMessage.Message,
            button: this.props.warningMessage.Button,
        };
        return (
            <ModalPopup>
                <ExpiredWarning {...expiredWarningProps} />
            </ModalPopup>
        );
    }
    renderQuoteCreatedPopup() {
        const reference = this.props.basket.basket.QuoteReference;
        let successMessage = 'Thank you for creating a quote.';
        successMessage += ` Your quote reference is ${reference}`;

        const buttonProps = {
            className: 'btn btn-primary',
            onClick: () => {
                this.setState({ displayQuoteCreatedPopup: false });
            },
        };
        return (
            <ModalPopup>
                <div className="panel panel-basic modal-container quote-created">
                    <div className="panel-body text-center">
                        <p>{successMessage}</p>
                    </div>
                    <footer className="panel-footer text-center">
                        <button {...buttonProps}>Close</button>
                    </footer>
                </div>
            </ModalPopup>
        );
    }
    renderLoader() {
        return (
            <div className="col-xs-12 my-3 text-center">
                <img className="mb-2" src="/booking/images/loader.gif" alt="loading..." />
                <p>Loading...</p>
            </div>
        );
    }
    render() {
        return (
            <div>
                {this.state.resultExpired
                    && this.renderExpiredPopup()}
                {this.props.basket.isLoaded
                    && <CompleteBooking {...this.getCompleteBookingProps()} />}
                {!this.props.basket.isLoaded
                    && this.renderLoader()}
                {this.state.displayQuoteCreatedPopup
                    && this.renderQuoteCreatedPopup()}
            </div>
        );
    }
}

CompleteBookingContainer.propTypes = {
    context: React.PropTypes.string,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    warningMessage: React.PropTypes.object,
    tradeSession: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const site = state.site ? state.site : {};
    let warningMessage = {};
    if (site) {
        const warningEntityKey = `Warnings-default-${site.Name.toLowerCase()}`;
        warningMessage
            = state.entities[warningEntityKey] && state.entities[warningEntityKey].isLoaded
            ? state.entities[warningEntityKey].model.BasketExpired.Payment : {};
    }
    const tradeSession = state.session.UserSession && state.session.UserSession.TradeSession
        ? state.session.UserSession.TradeSession : {};

    return {
        basket,
        site,
        warningMessage,
        tradeSession,
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
        AlertActions,
        SearchActions,
        SearchResultActions,
        EntityActions,
        QuoteActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(CompleteBookingContainer);
