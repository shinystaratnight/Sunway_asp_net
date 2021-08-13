import * as AirportActions from 'actions/lookups/airportActions';
import * as AlertActions from 'actions/bookingjourney/alertActions';
import * as AlertConstants from 'constants/alerts';
import * as BasketActions from '../../actions/bookingjourney/basketactions';
import * as FlightCarrierActions from 'actions/lookups/flightCarrierActions';
import * as FlightClassActions from 'actions/lookups/flightClassActions';
import * as MealBasisActions from 'actions/lookups/mealBasisActions';

import Basket from '../../widgets/bookingjourney/basket';
import React from 'react';
import UrlFunctions from '../../library/urlfunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BasketContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    isInitialised() {
        return this.props.basket.isLoaded
            && this.props.mealBasis.isLoaded
            && this.props.airports.isLoaded
            && this.props.flightCarriers.isLoaded
            && this.props.flightClasses.isLoaded;
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
        this.checkWarning();
    }
    checkWarning() {
        const warning = UrlFunctions.getQueryStringValue('warning');
        if (warning === 'quote-extra') {
            let noHotelWarning = 'An extra you have selected is no longer available, ';
            noHotelWarning += 'please add any alternative extras to your package.';
            this.props.actions.addAlert('Quote_Extra',
                AlertConstants.ALERT_TYPES.WARNING,
                noHotelWarning);
        }
    }
    renderLoader() {
        const contentModel = this.props.entity.model;
        return (
            <div className="basket-content panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">{contentModel.Title}</h3>
                </header>
                <div className="panel-body">
                    <div className="my-3 text-center">
                        <img className="mb-2" src="/booking/images/loader.gif" alt="loading..." />
                        <p>Loading your basket...</p>
                    </div>
                 </div>
            </div>
        );
    }
    render() {
        const contentModel = this.props.entity.model;
        const basketProps = {
            basket: this.props.basket,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            title: contentModel.Title,
            componentTypeTitles: {
                Flight: contentModel.FlightTitle,
                Property: contentModel.HotelTitle,
                Transfer: contentModel.TransferTitle,
                Extras: contentModel.ExtrasTitle,
            },
            totalText: contentModel.TotalText,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            mealBasis: this.props.mealBasis.items,
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
            customerComponents: this.props.customerComponents,
            hideAdjustments: contentModel.Configuration.HideAdjustments,
        };
        return (
            <div>
                {this.isInitialised()
                    && <Basket {...basketProps} />}
                {!this.isInitialised()
                    && this.renderLoader()}
            </div>
        );
    }
}

BasketContainer.propTypes = {
    actions: React.PropTypes.object,
    basket: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    flightCarriers: React.PropTypes.object.isRequired,
    flightClasses: React.PropTypes.object.isRequired,
    customerComponents: React.PropTypes.object,
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
    const mealBasis = state.mealBasis ? state.mealBasis : {};
    const airports = state.airports ? state.airports : {};
    const flightCarriers = state.flightCarriers ? state.flightCarriers : {};
    const flightClasses = state.flightClasses ? state.flightClasses : {};

    return {
        basket,
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
        FlightCarrierActions,
        FlightClassActions,
        MealBasisActions,
        AlertActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BasketContainer);
