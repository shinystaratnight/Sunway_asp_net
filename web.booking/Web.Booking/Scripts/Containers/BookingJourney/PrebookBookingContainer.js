import * as AlertActions from '../../actions/bookingjourney/alertActions';
import * as AlertConstants from '../../constants/alerts';
import * as BasketActions from '../../actions/bookingjourney/basketactions';
import * as EntityActions from 'actions/content/entityActions';

import ExpiredWarning from 'components/bookingjourney/expiredwarning';
import ModalPopup from 'components/common/modalpopup';
import PreBookBooking from '../../widgets/bookingjourney/prebookBooking';
import React from 'react';
import UrlFunctions from '../../library/urlfunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PreBookBookingContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            resultExpired: false,
            messagesLoaded: false,
        };
        this.resultTimeout = {};
        this.resultExpiryTime = 1800000;
        this.handlePreBook = this.handlePreBook.bind(this);
        this.handleError = this.handleError.bind(this);
        this.resultExpired = this.resultExpired.bind(this);
        this.handleCheckbox = this.handleCheckbox.bind(this);
    }

    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isPreBooked && nextProps.basket.isPreBooked) {
            const basketToken = UrlFunctions.getQueryStringValue('t');
            const page = window.location.pathname.split('/')[2];
            const siteConfig = this.props.site.SiteConfiguration;
            const paymentPageName = siteConfig.BookingJourneyConfiguration.PaymentUrl;

            let paymentUrl = window.location.pathname.replace(`/booking/${page}`,
                `/booking/${paymentPageName}`);

            paymentUrl += `?t=${basketToken}`;

            if (nextProps.basket.priceChangeAmount
                && Math.abs(nextProps.basket.priceChangeAmount) >= 1) {
                paymentUrl += `&pricechange=${nextProps.basket.priceChangeAmount.toFixed(2)}`;
            }

            window.location.href = paymentUrl;
        }

        if (!this.props.basket.isPreBookFailed && nextProps.basket.isPreBookFailed) {
            this.props.actions.addAlert('PreBook_Fail',
                AlertConstants.ALERT_TYPES.DANGER,
                this.props.entity.model.PrebookFailWarning);
        }

        if (!this.props.basket.isLoaded && nextProps.basket.isLoaded) {
            this.resultTimeout = setTimeout(this.resultExpired, this.resultExpiryTime);
        }
        if (!this.state.messagesLoaded) {
            const context = `default-${this.props.site.Name.toLowerCase()}`;
            this.state.messagesLoaded = true;
            this.props.actions.loadEntity('Shared', 'Warnings', context, 'dev');
        }
    }

    handlePreBook() {
        this.props.actions.basketPreBook(this.props.basket.basket.BasketToken);
    }

    handleCheckbox(ev) {
        this.setState({ buttonEnabled: ev.target.checked });
        this.setState({ checkboxChecked: ev.target.checked });
    }

    handleError(key, value) {
        this.props.actions.addWarning(key, value);
        this.props.actions.addAlert('Prebook_MandatoryFields',
            AlertConstants.ALERT_TYPES.DANGER,
            this.props.entity.model.MandatoryFieldsWarning);
    }

    resultExpired() {
        this.setState({ resultExpired: true });
    }

    getDeeplinkSearchUrl() {
        const currentPath = window.location.pathname;
        const deeplinkUrl = currentPath.replace('extras', 'results');
        return (deeplinkUrl);
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
    renderLoader() {
        return (
            <div className="col-xs-12 my-3 text-center">
                <img className="mb-2" src="/booking/images/loader.gif" alt="loading..." />
                <p>Loading...</p>
            </div>
        );
    }

    validateBaggage() {
        if (this.shouldValidateBaggage()) {
            if (this.checkFlightResults()) {
                if (this.props.basket.basket.Components.filter(
                    component => component.ComponentType === 'Flight').filter(
                        flightComponent => flightComponent.SubComponents.filter(
                            subComponent => subComponent.ExtraType === 'Baggage'
                                && subComponent.QuantitySelected !== 0).length > 0).length === 0) {
                    return false;
                }
            }
        }
        return true;
    }
    shouldValidateBaggage() {
        if (this.props.entity.model.Configuration.ValidateBaggageUpsell) {
            if (this.props.entity.model.NoBags !== '') {
                return true;
            }
        }
        return false;
    }
    checkFlightResults() {
        if ((this.props.basket.basket.Components.filter(
            component => component.ComponentType === 'Flight').filter(
                flightComponent => flightComponent.SubComponents.filter(
                    subComponent => subComponent.ExtraType === 'Baggage').length
                    > 0)).length > 0) {
            return true;
        }
        return false;
    }
    validateTransfers() {
        // if they want to validate transfers
        if (this.shouldValidateTransfers()) {
            if (this.checkTransferResults()) {
                // If no transfer is selected, set noTransfer to true
                if (this.props.basket.basket.Components.filter(component =>
                    component.ComponentType === 'Transfer').length
                    === 0) {
                    return false;
                }
            }
        }
        return true;
    }
    shouldValidateTransfers() {
        if (this.props.entity.model.Configuration.ValidateTransferUpsell) {
            if (this.props.entity.model.NoTransfers !== '') {
                return true;
            }
        }
        return false;
    }
    checkTransferResults() {
        if (this.props.searchResults.hasOwnProperty('Transfer')) {
            if (this.props.searchResults.Transfer.results.length > 0) {
                return true;
            }
        }
        return false;
    }

    getPreBookBookingProps() {
        const validBaggage = this.validateBaggage();
        const validTransfers = this.validateTransfers();

        if (validTransfers) {
            if (validBaggage) {
                this.state.message = this.props.entity.model.AllAvailable;
                this.state.requireCheckbox
                    = this.props.entity.model.AllAvailableConfirmationCheckbox;
                this.state.buttonEnabled = true;
            } else {
                this.state.message = this.props.entity.model.NoBags;
                this.state.requireCheckbox = this.props.entity.model.NoBagsConfirmationCheckbox;
            }
        } else if (validBaggage) {
            this.state.message = this.props.entity.model.NoTransfers;
            this.state.requireCheckbox = this.props.entity.model.NoTransfersConfirmationCheckbox;
        } else {
            this.state.message = this.props.entity.model.NoBagsOrTransfers;
            this.state.requireCheckbox
                = this.props.entity.model.NoBagsOrTransfersConfirmationCheckbox;
        }

        return {
            basket: this.props.basket,
            onPreBook: this.handlePreBook,
            onError: this.handleError,
            resetWarnings: this.props.actions.resetWarnings,
            message: this.state.message,
            button: this.props.entity.model.Button,
            requireCheckbox: this.state.requireCheckbox,
            handleCheckbox: this.handleCheckbox,
            buttonEnabled: this.state.buttonEnabled,
            buttonClass: this.state.buttonClass,
            checkboxChecked: this.state.checkboxChecked,
        };
    }
    render() {
        const prebookBookingProps = this.getPreBookBookingProps();
        return (
            <div>
                {this.state.resultExpired
                    && this.renderExpiredPopup()}
                <PreBookBooking {...prebookBookingProps} />
            </div>
        );
    }
}

PreBookBookingContainer.propTypes = {
    context: React.PropTypes.string,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    warningMessage: React.PropTypes.object,
    searchResults: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const searchResults = state.searchResults ? state.searchResults : {};
    const basket = state.basket ? state.basket : {};
    const site = state.site ? state.site : {};
    let warningMessage = {};
    if (site) {
        const warningEntityKey = `Warnings-default-${site.Name.toLowerCase()}`;
        warningMessage
            = state.entities[warningEntityKey] && state.entities[warningEntityKey].isLoaded
                ? state.entities[warningEntityKey].model.BasketExpired.Payment : {};
    }

    return {
        basket,
        warningMessage,
        searchResults,
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
        AlertActions,
        EntityActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PreBookBookingContainer);
