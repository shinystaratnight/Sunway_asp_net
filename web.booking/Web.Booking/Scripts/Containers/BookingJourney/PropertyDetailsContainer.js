import 'widgets/bookingjourney/_propertydetails.scss';

import * as AirportActions from 'actions/lookups/airportActions';
import * as BasketActions from 'actions/bookingjourney/basketactions';
import * as CountryActions from 'actions/lookups/countryActions';
import * as FlightCarrierActions from 'actions/lookups/flightCarrierActions';
import * as FlightClassActions from 'actions/lookups/flightClassActions';
import * as MealBasisActions from 'actions/lookups/mealBasisActions';
import * as PropertyActions from 'actions/content/propertyActions';
import * as SearchActions from 'actions/bookingjourney/searchActions';
import * as SearchConstants from 'constants/search';
import * as SearchResultActions from 'actions/bookingjourney/searchResultActions';
import * as SearchResultsConstants from 'constants/searchresults';

import BasketProperty from 'components/searchresults/basketproperty';
import FlightResult from 'components/searchresults/flightresult';
import PropertyResult from 'components/searchresults/propertyresult';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PropertyDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            checkComponentToken: [],
            isAddingComponent: false,
            propertyId: 0,
            cancellationChargesToDisplay: [],
        };
        this.handleAddComponent = this.handleAddComponent.bind(this);
        this.handleGetCancellationCharges = this.handleGetCancellationCharges.bind(this);
        this.handleHideCancellationCharges = this.handleHideCancellationCharges.bind(this);
    }
    componentDidMount() {
        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;
        this.props.actions.loadSearchDetailsIfNeeded(searchConfig);

        this.props.actions.loadCountriesIfNeeded();
        this.props.actions.loadMealBasisIfNeeded();
        this.props.actions.loadAirportsIfNeeded();
        this.props.actions.loadFlightCarriersIfNeeded();
        this.props.actions.loadFlightClassesIfNeeded();

        const basketToken = UrlFunctions.getQueryStringValue('t');
        if (basketToken) {
            this.props.actions.loadBasket(basketToken);
        } else {
            this.props.actions.createBasket();
        }

        this.state.propertyId = UrlFunctions.getQueryStringValue('propertyId');
        this.props.actions.loadProperty(this.state.propertyId);
    }
    componentWillReceiveProps(nextProps) {
        if (!nextProps.searchResults[SearchConstants.SEARCH_MODES.HOTEL].isLoaded
            && !nextProps.searchResults[SearchConstants.SEARCH_MODES.HOTEL].isFetching
            && nextProps.search.isLoaded
            && nextProps.airports.isLoaded
            && ((!this.isPackageSearch(nextProps) || this.hasExistingComponent('Flight'))
                || nextProps.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded)) {
            const hotelResultToken = UrlFunctions.getQueryStringValue('h');
            const searchDetails = nextProps.search.searchDetails;
            const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);
            this.props.actions.loadSearchResults(hotelResultToken,
                SearchConstants.SEARCH_MODES.HOTEL, flightPlusHotelType);
        }

        if (this.isPackageSearch(nextProps)
            && nextProps.search.isLoaded
            && nextProps.airports.isLoaded
            && (nextProps.basket.isLoaded
                && !nextProps.basket.basket.Components.some(component =>
                    component.ComponentType === 'Flight'))
            && !nextProps.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded
            && !nextProps.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isFetching) {
            const flightResultToken = UrlFunctions.getQueryStringValue('f');
            const flightComponentToken = parseInt(UrlFunctions.getQueryStringValue('sf'), 10);
            const searchDetails = nextProps.search.searchDetails;
            const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);
            this.props.actions.loadSearchResults(flightResultToken,
                SearchConstants.SEARCH_MODES.FLIGHT, flightPlusHotelType, flightComponentToken);
        }

        if (this.state.checkComponentToken.length > 0) {
            let basketComponents = 0;
            this.state.checkComponentToken.forEach(token => {
                const componentIndex = nextProps.basket.basket.Components.findIndex(component =>
                    component.ComponentToken === token);
                if (componentIndex > -1) {
                    basketComponents += 1;
                }
            });

            if (basketComponents === this.state.checkComponentToken.length) {
                this.state.isAddingComponent = false;
                const page = window.location.pathname.split('/')[2];
                if (nextProps.basket.basket.Warnings
                    && nextProps.basket.basket.Warnings.length > 0) {
                    const resultsUrl = window.location.pathname.replace(`/booking/${page}`,
                        '/booking/results');
                    window.location = `${resultsUrl}?warning=prebookfailed`;
                } else {
                    const extrasUrl = window.location.pathname.replace(`/booking/${page}`,
                        '/booking/extras');
                    window.location = `${extrasUrl}?t=${this.props.basket.basket.BasketToken}`;
                }
            }
        }

        if (this.shouldSearchBookingAdjustments(nextProps)) {
            this.searchBookingAdjustments(nextProps);
        }
    }
    displayChangeFlight() {
        const bookingJourneyConfig = this.props.site.SiteConfiguration.BookingJourneyConfiguration;
        const pageUrl = this.props.page.PageURL;
        const display = bookingJourneyConfig.ChangeFlightPages.indexOf(pageUrl) > -1;
        return display;
    }
    getFlightPlusHotelType(searchDetails) {
        const siteConfiguration = this.props.site.SiteConfiguration;
        const flightPlusHotelTypes = SearchResultsConstants.FLIGHT_PLUS_HOTEL_TYPES;

        let flightPlusHotelType = flightPlusHotelTypes.SEPARATE;
        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            flightPlusHotelType = flightPlusHotelTypes.FLIGHTONLY;
        } else if (siteConfiguration.SearchConfiguration.PackageSearch) {
            flightPlusHotelType = flightPlusHotelTypes.PACKAGESEARCH;
        } else if (siteConfiguration.PricingConfiguration.PackagePrice) {
            flightPlusHotelType = flightPlusHotelTypes.PACKAGEPRICE;
        }

        return flightPlusHotelType;
    }
    getPropertyResult() {
        const propertyId = parseInt(UrlFunctions.getQueryStringValue('propertyId'), 10);
        const results = this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL].results;
        const property = results.find(propertyResult =>
            propertyResult.MetaData.PropertyReferenceId === propertyId);
        return property;
    }
    getAdjustmentAmount() {
        let adjustmentAmount = 0;
        const adjustments = this.props.searchResults.adjustments;
        if (adjustments) {
            adjustments.forEach(adjustment => {
                adjustmentAmount += adjustment.AdjustmentAmount;
            });
        }
        return adjustmentAmount;
    }
    getPropertyResultProps() {
        const result = this.getPropertyResult();
        const contentModel = this.props.entity.model;
        const cancellationCharges = this.props.basket.cancellationCharges[result.ComponentToken];
        const charges = cancellationCharges ? cancellationCharges.cancellationCharges : {};
        const payments = cancellationCharges ? cancellationCharges.payments : {};
        const displayCancellationCharges
          = this.state.cancellationChargesToDisplay.indexOf(result.ComponentToken) !== -1;
        const props = {
            result,
            resultTokens: this.props.search.searchResult.ResultTokens,
            rooms: this.props.search.searchDetails.Rooms,
            countries: this.props.countries.items,
            mealBasis: this.props.mealBasis.items,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            starRatingConfiguration: this.props.site.SiteConfiguration.StarRatingConfiguration,
            renderSubResults: true,
            onComponentAdd: this.handleAddComponent,
            summaryBehaviour: 'Full',
            content: this.props.properties[this.state.propertyId].content,
            renderPaxSummary: contentModel.Configuration.RenderPaxSummary,
            charterPackageText: contentModel.CharterPackageText,
            updatingPrice: this.props.search.isSearchingForAdjustments,
            adjustmentAmount: this.getAdjustmentAmount(),
            searchDetails: this.props.search.searchDetails,
            onGetCancellationCharges: this.handleGetCancellationCharges,
            onHideCancellationCharges: this.handleHideCancellationCharges,
            cancellationCharges: charges,
            payments,
            displayCancellationCharges,
            mapConfiguration: this.props.site.SiteConfiguration.MapConfiguration,
        };
        const basketFlight = this.props.basket.basket.Components
            .find(component => component.ComponentType === 'Flight');

        if (basketFlight) {
            props.basketFlight = basketFlight;
            props.basketToken = this.props.basket.BasketToken;
        }
        if (this.isPackageSearch(this.props) && !basketFlight) {
            props.selectedFlight = this.getSelectedFlight();
        }
        return props;
    }
    getBasketPropertyProps() {
        const basketProperty = this.props.basket.basket.Components
            .find(component => component.ComponentType === 'Hotel');
        const props = {
            property: basketProperty,
            rooms: this.props.search.searchDetails.Rooms,
            countries: this.props.countries.items,
            mealBasis: this.props.mealBasis.items,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            starRatingConfiguration: this.props.site.SiteConfiguration.StarRatingConfiguration,
            onComponentAdd: this.handleAddComponent,
            summaryBehaviour: 'Full',
            updatingPrice: this.props.search.isSearchingForAdjustments,
            adjustmentAmount: this.getAdjustmentAmount(),
        };
        if (this.isPackageSearch(this.props)) {
            props.selectedFlight = this.getSelectedFlight();
        }
        return props;
    }
    getSelectedFlight() {
        const componentToken = parseInt(UrlFunctions.getQueryStringValue('sf'), 10);
        const results = this.props.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].results;
        const selectedResult = results.find(result => result.ComponentToken === componentToken);
        return selectedResult;
    }
    handleAddComponent(componentToken, subComponentTokens, arrivalDate, duration) {
        if (this.state.isAddingComponent) {
            return;
        }
        this.state.isAddingComponent = true;

        if (!this.hasExistingComponent('Hotel')) {
            const searchResult = this.props.search.searchResult;
            const componentModel = {
                AdjustmentSearchToken: searchResult.ResultTokens.BookingAdjustments,
                BasketToken: this.props.basket.basket.BasketToken,
                SearchToken: searchResult.ResultTokens.Hotel,
                ComponentToken: componentToken,
                SubComponentTokens: subComponentTokens,
                MetaData: {
                    ArrivalDate: arrivalDate,
                    Duration: duration,
                },
            };
            this.state.checkComponentToken.push(componentToken);
            this.props.actions.basketAddComponent(componentModel);
        }

        if (this.isPackageSearch(this.props) && !this.hasExistingComponent('Flight')) {
            const flightComponentToken = parseInt(UrlFunctions.getQueryStringValue('sf'), 10);
            const flightComponent = {
                componentType: 'Flight',
                BasketToken: this.props.basket.basket.BasketToken,
                SearchToken: this.props.search.searchResult.ResultTokens.Flight,
                ComponentToken: flightComponentToken,
            };
            this.state.checkComponentToken.push(flightComponentToken);
            this.props.actions.basketAddComponent(flightComponent);
        }
    }
    getSearchToken(searchMode, resultTokens) {
        let searchToken = '';
        switch (searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL: {
                const hotelResultToken = UrlFunctions.getQueryStringValueWithSpaces('h');
                searchToken = hotelResultToken ? hotelResultToken : resultTokens.Hotel;
                break;
            }
            case SearchConstants.SEARCH_MODES.FLIGHT: {
                const flightResultToken = UrlFunctions.getQueryStringValueWithSpaces('f');
                searchToken = flightResultToken ? flightResultToken : resultTokens.Flight;
                break;
            }
            default:
        }
        return searchToken;
    }
    handleGetCancellationCharges(componentToken, subComponentTokens) {
        if (!this.props.basket.cancellationCharges[componentToken]) {
            const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
                this.props.searchResults);
            const configuration = this.props.entity.model.Configuration;

            const searchToken = this.getSearchToken(SearchConstants.SEARCH_MODES.HOTEL,
                this.props.search.searchResult.ResultTokens);
            const flightSearchToken = this.getSearchToken(SearchConstants.SEARCH_MODES.FLIGHT,
                this.props.search.searchResult.ResultTokens);
            const componentType = configuration.SearchMode;
            const componentModel = [
                {
                    SearchToken: searchToken,
                    ComponentToken: componentToken,
                    SubComponentTokens: subComponentTokens,
                    componentType,
                },
            ];
            if (selectedFlight) {
                componentModel.push({
                    SearchToken: flightSearchToken,
                    ComponentToken: selectedFlight.ComponentToken,
                    componentType,
                });
            }
            this.props.actions.basketGetComponentCancellationCharges(componentModel);
        }
        if (this.state.cancellationChargesToDisplay.indexOf(componentToken) === -1) {
            const cancellationChargesToDisplay = this.state.cancellationChargesToDisplay;
            cancellationChargesToDisplay.push(componentToken);
            this.setState({
                cancellationChargesToDisplay,
            });
        }
    }
    handleHideCancellationCharges(componentToken) {
        const componentIndex = this.state.cancellationChargesToDisplay.indexOf(componentToken);
        if (componentIndex > -1) {
            const cancellationCharges = this.state.cancellationChargesToDisplay;
            cancellationCharges.splice(componentIndex, 1);
            this.setState({
                cancellationChargesToDisplay:
                    cancellationCharges,
            });
        }
    }
    getSelectedResult(searchMode, searchResults) {
        const modeResults = searchResults[searchMode];
        let selectedResult = null;
        for (let i = 0; i < modeResults.results.length; i++) {
            const result = modeResults.results[i];
            if (result.isSelected) {
                selectedResult = result;
                break;
            }
        }
        return selectedResult;
    }
    hasExistingComponent(componentType) {
        const basket = this.props.basket;
        const basketComponentExists = basket.isLoaded
            && basket.basket.Components.some(component =>
                component.ComponentType === componentType);
        return basketComponentExists;
    }
    isInitialised() {
        return this.props.search.isLoaded
            && this.props.countries.isLoaded
            && this.props.mealBasis.isLoaded
            && this.props.basket.isLoaded
            && this.state.propertyId > 0
            && this.props.properties[this.state.propertyId].isLoaded
            && (this.isResultsLoaded() || this.hasExistingComponent('Hotel'));
    }
    isPackageSearch(props) {
        const searchDetails = props.search.searchDetails;
        const siteConfiguration = props.site.SiteConfiguration;

        return searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
            && siteConfiguration.PricingConfiguration.PackagePrice;
    }
    isResultsLoaded() {
        let isResultsLoaded;
        if (this.isPackageSearch(this.props) && !this.hasExistingComponent('Flight')) {
            isResultsLoaded = this.props.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded
                && this.props.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].results.length > 0
                && this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL].isLoaded
                && this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL].results.length > 0;
        } else {
            const searchMode = SearchConstants.SEARCH_MODES.HOTEL;
            isResultsLoaded = this.props.searchResults[searchMode].isLoaded
                && this.props.searchResults[searchMode].results.length > 0;
        }
        return isResultsLoaded;
    }
    searchBookingAdjustments(nextProps) {
        const searchDetails = nextProps.search.searchDetails;

        let totalPassengers = 0;
        for (let i = 0; i < searchDetails.Rooms.length; i++) {
            const room = searchDetails.Rooms[i];
            totalPassengers += room.Adults;
            totalPassengers += room.Children;
            totalPassengers += room.Infants;
        }

        let selectedFlight = null;
        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
                || searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            selectedFlight = this.getSelectedFlight();
        }

        const property = this.getPropertyResult();

        const searchModel = {
            FirstDepartureDate: searchDetails.DepartureDate,
            FlightCarrierId: selectedFlight ? selectedFlight.MetaData.FlightCarrierId : 0,
            FlightSupplierId: selectedFlight ? selectedFlight.MetaData.FlightSupplierId : 0,
            FlightPrice: selectedFlight ? selectedFlight.Price : 0,
            PropertyPrice: property ? property.Price : 0,
            GeographyLevel3Id: property ? property.MetaData.GeographyLevel3Id : 0,
            TotalPassengers: totalPassengers,
            SearchMode: searchDetails.SearchMode,
        };

        this.props.actions.performAdjustmentSearch(searchModel);
    }
    shouldSearchBookingAdjustments(nextProps) {
        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;

        return searchConfig.SearchBookingAdjustments
            && !nextProps.search.adjustmentSearchComplete
            && !nextProps.search.isSearchingForAdjustments
            && this.isResultsLoaded();
    }
    renderSelectedFlight() {
        const selectedFlight = this.getSelectedFlight();
        const flightResultProps = {
            result: selectedFlight,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            flightButtons: this.props.entity.model.FlightButtons,
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            displayPrice: false,
            rooms: this.props.search.searchDetails.Rooms,
        };

        let FlightComponent = FlightResult;
        if (this.props.customerComponents.hasOwnProperty('FlightResult')) {
            FlightComponent = this.props.customerComponents.FlightResult;
        }

        return (
            <div className="row">
                <div className="search-selected-flight col-xs-12">
                    {selectedFlight
                        && <h3 className="h-tertiary">Your Selected Flight</h3>}
                    {selectedFlight
                        && <FlightComponent {...flightResultProps} />}
                </div>
            </div>
        );
    }
    renderLoader() {
        return (
            <div className="col-xs-12 my-3 text-center">
                <img className="mb-2" src="/booking/images/loader.gif" alt="loading..." />
                <p>Loading your results...</p>
            </div>
        );
    }
    render() {
        let PropertyComponent = PropertyResult;
        if (this.props.customerComponents.hasOwnProperty('PropertyResult')) {
            PropertyComponent = this.props.customerComponents.PropertyResult;
        }

        let BasketPropertyComponent = BasketProperty;
        if (this.props.customerComponents.hasOwnProperty('BasketProperty')) {
            BasketPropertyComponent = this.props.customerComponents.BasketProperty;
        }
        return (
            <div className="property-details-content container">
                {this.isInitialised()
                    && this.isPackageSearch(this.props)
                    && !this.displayChangeFlight()
                    && this.renderSelectedFlight()}

                {this.isInitialised() && !this.hasExistingComponent('Hotel')
                    && <PropertyComponent {...this.getPropertyResultProps()} />}

                {this.isInitialised() && this.hasExistingComponent('Hotel')
                    && <BasketPropertyComponent {...this.getBasketPropertyProps()} />}

                {!this.isInitialised()
                    && this.renderLoader()}
            </div>
        );
    }
}

PropertyDetailsContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    countries: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    properties: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    flightCarriers: React.PropTypes.object.isRequired,
    flightClasses: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    customerComponents: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    const searchResults = state.searchResults ? state.searchResults : {};
    const session = state.session ? state.session : {};
    const countries = state.countries ? state.countries : {};
    const mealBasis = state.mealBasis ? state.mealBasis : {};
    const basket = state.basket ? state.basket : {};
    const properties = state.properties ? state.properties : {};
    const airports = state.airports ? state.airports : {};
    const flightCarriers = state.flightCarriers ? state.flightCarriers : {};
    const flightClasses = state.flightClasses ? state.flightClasses : {};

    return {
        search,
        searchResults,
        session,
        countries,
        mealBasis,
        basket,
        properties,
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
        CountryActions,
        FlightCarrierActions,
        FlightClassActions,
        MealBasisActions,
        PropertyActions,
        SearchActions,
        SearchResultActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PropertyDetailsContainer);
