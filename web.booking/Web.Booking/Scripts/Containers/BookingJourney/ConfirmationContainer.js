import * as AirportActions from 'actions/lookups/airportActions';
import * as BasketActions from 'actions/bookingjourney/basketactions';
import * as BookingActions from 'actions/bookingjourney/bookingactions';
import * as FlightCarrierActions from 'actions/lookups/flightCarrierActions';
import * as FlightClassActions from 'actions/lookups/flightClassActions';
import * as MealBasisActions from 'actions/lookups/mealBasisActions';

import Confirmation from 'widgets/bookingjourney/confirmation';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class ConfirmationContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    componentDidMount() {
        if (!this.props.basket.isLoaded
            && !this.props.basket.isLoading) {
            const basketToken = UrlFunctions.getQueryStringValue('t');
            this.props.actions.loadBasket(basketToken);
        }
        this.props.actions.loadMealBasisIfNeeded();
        this.props.actions.loadAirportsIfNeeded();
        this.props.actions.loadFlightCarriersIfNeeded();
        this.props.actions.loadFlightClassesIfNeeded();
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isLoaded
            && nextProps.basket.isLoaded) {
            const bookingReference = nextProps.basket.basket.BookingReference;
            this.props.actions.retrieveBooking(bookingReference);
        }
    }
    isInitialised() {
        return this.props.entity.isLoaded
            && this.props.booking.isLoaded
            && this.props.mealBasis.isLoaded
            && this.props.airports.isLoaded
            && this.props.flightCarriers.isLoaded
            && this.props.flightClasses.isLoaded;
    }
    getConfirmationProps() {
        const contentModel = this.props.entity.model;
        const confirmationProps = {
            bookingDetails: this.props.booking.details,
            title: contentModel.Title,
            message: contentModel.Message,
            bookingSummary: {
                title: contentModel.BookingSummary.Title,
                componentTypeTitles: {
                    flight: contentModel.BookingSummary.FlightTitle,
                    property: contentModel.BookingSummary.PropertyTitle,
                    transfer: contentModel.BookingSummary.TransferTitle,
                    extras: contentModel.BookingSummary.ExtrasTitle,
                },
            },
            disclaimer: contentModel.Disclaimer,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            currency: this.props.session.UserSession.SelectCurrency,
            lookups: {
                airports: this.props.airports.items,
                flightCarriers: this.props.flightCarriers.items,
                flightClasses: this.props.flightClasses.items,
                mealBasis: this.props.mealBasis.items,
            },
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
        };
        return confirmationProps;
    }
    renderLoader() {
        return (
            <div className="col-xs-12 my-3 text-center">
                <img className="mb-2" src="/booking/images/loader.gif" alt="loading..." />
                <p>Loading your booking...</p>
            </div>
        );
    }
    render() {
        let ConfirmationWidget = Confirmation;
        if (this.props.customerWidgets.hasOwnProperty('Confirmation')) {
            ConfirmationWidget = this.props.customerWidgets.Confirmation;
        }
        return (
            <div className="confirmation-container">
                {this.isInitialised()
                    && <ConfirmationWidget {...this.getConfirmationProps()} />}

                {!this.isInitialised()
                    && this.renderLoader()}
            </div>
        );
    }
}

ConfirmationContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    booking: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    flightCarriers: React.PropTypes.object.isRequired,
    flightClasses: React.PropTypes.object.isRequired,
    customerWidgets: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const booking = state.booking ? state.booking : {};
    const site = state.site ? state.site : {};
    const mealBasis = state.mealBasis ? state.mealBasis : {};
    const airports = state.airports ? state.airports : {};
    const flightCarriers = state.flightCarriers ? state.flightCarriers : {};
    const flightClasses = state.flightClasses ? state.flightClasses : {};

    return {
        basket,
        booking,
        site,
        mealBasis,
        airports,
        flightCarriers,
        flightClasses,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        AirportActions,
        BasketActions,
        BookingActions,
        FlightCarrierActions,
        FlightClassActions,
        MealBasisActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ConfirmationContainer);
