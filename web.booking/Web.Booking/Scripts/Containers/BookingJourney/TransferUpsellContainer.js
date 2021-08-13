import * as AirportActions from 'actions/lookups/airportActions';
import * as BasketActions from '../../actions/bookingjourney/basketactions';
import * as CountryActions from 'actions/lookups/countryActions';
import * as SearchActions from '../../actions/bookingjourney/searchActions';
import * as SearchResultActions from '../../actions/bookingjourney/searchResultActions';

import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import TransferUpsell from '../../widgets/bookingjourney/transferUpsell';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class TransferUpsellContainer extends React.Component {
    constructor(props) {
        super(props);
        this.renderModes = {
            PACKAGESEARCH: 'PackageSearch',
            SEARCHFORM: 'SearchForm',
        };
        this.state = {
            isInitialised: false,
            renderMode: '',
            isLoadingResultsFromBasket: false,
            selectedComponentToken: 0,
            searchFailed: false,
            freeTransferAdded: false,
            options: {
                isInitialised: false,
                airports: [],
            },
            selected: {
                departureAirport: 0,
                outbound: {
                    flightNumber: '',
                    flightTime: '',
                    applyFormatting: false,
                },
                inbound: {
                    flightNumber: '',
                    flightTime: '',
                    applyFormatting: false,
                },
            },
            validateInputs: false,
            addingComponent: false,
        };
        this.performPackageTransferSearch = this.performPackageTransferSearch.bind(this);
        this.performHotelTransferSearch = this.performHotelTransferSearch.bind(this);
        this.handleDepartureAirportUpdate = this.handleDepartureAirportUpdate.bind(this);
        this.handleFlightTimeUpdate = this.handleFlightTimeUpdate.bind(this);
        this.updateFlightTime = this.updateFlightTime.bind(this);
        this.handleFlightNumberUpdate = this.handleFlightNumberUpdate.bind(this);
        this.handleFlightTimeFocusChange = this.handleFlightTimeFocusChange.bind(this);
        this.handleModifyBasket = this.handleModifyBasket.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.state.isInitialised
            && nextProps.search.isLoaded
            && nextProps.basket.isLoaded) {
            this.init(nextProps);
        }

        if (this.state.renderMode === this.renderModes.SEARCHFORM
            && !this.state.options.isInitialised
            && nextProps.airports.isLoaded
            && nextProps.countries.isLoaded) {
            this.setupHotelSearchOptions();
        }

        if (this.isTransferSearchComplete(nextProps)
                && !nextProps.searchResults.isFetching
                && !nextProps.searchResults.isLoaded) {
            const transferResultToken = nextProps.search.searchResult.ResultTokens.Transfer;
            this.props.actions.loadExtraSearchResults(transferResultToken, 'Transfer');
        }

        if (this.props.searchResults.isFetching
            && nextProps.searchResults.isLoaded) {
            this.handleResultsLoaded(nextProps);
        }
    }
    init(props) {
        const renderMode = this.getRenderMode(props);
        this.setState({
            isInitialised: true,
            renderMode,
        });

        const basketTransfer = this.getBasketTransfer(props);
        if (basketTransfer) {
            this.loadResultsFromBasketTransfer(basketTransfer);
        } else if (renderMode === this.renderModes.SEARCHFORM) {
            this.props.actions.loadAirportsIfNeeded();
            this.props.actions.loadCountriesIfNeeded();
        } else if (renderMode === this.renderModes.PACKAGESEARCH) {
            this.performPackageTransferSearch(props);
        }
    }
    getRenderMode(props) {
        let renderMode = '';
        const searchMode = props.search.searchDetails.SearchMode;
        if (searchMode === 'Hotel') {
            renderMode = this.renderModes.SEARCHFORM;
        } else if (searchMode === 'FlightPlusHotel') {
            renderMode = this.renderModes.PACKAGESEARCH;
        }
        return renderMode;
    }
    handleResultsLoaded(props) {
        const displayedResults = props.searchResults.results.filter(result =>
            this.isCharterPackage(props.basket.basket) || result.Price > 0);
        const searchFailed = displayedResults.length === 0;

        if (this.state.isLoadingResultsFromBasket && searchFailed) {
            if (this.state.renderMode === this.renderModes.PACKAGESEARCH) {
                this.performPackageTransferSearch(props);
            } else if (this.state.renderMode === this.renderModes.SEARCHFORM) {
                this.props.actions.loadAirportsIfNeeded();
                this.props.actions.loadCountriesIfNeeded();
            }
        } else if (!this.state.isLoadingResultsFromBasket && searchFailed) {
            this.setState({
                searchFailed,
            });
        }

        if (!searchFailed
            && !this.state.freeTransferAdded
            && !this.getBasketTransfer(props)
            && this.isCharterPackage(props.basket.basket)) {
            this.addFreeTransfer(props);
        }
    }
    getMetaData() {
        const basketFlight
            = this.props.basket.basket.Components.find(c => c.ComponentType === 'Flight');
        const metaData = {};
        if (!ObjectFunctions.isNullOrEmpty(basketFlight)) {
            metaData.OutboundFlightCode = basketFlight.OutboundFlightDetails.FlightCode;
            metaData.ReturnFlightCode = basketFlight.ReturnFlightDetails.FlightCode;
        } else {
            metaData.OutboundFlightCode = this.state.selected.outbound.flightNumber;
            metaData.ReturnFlightCode = this.state.selected.inbound.flightNumber;
        }
        return metaData;
    }
    addFreeTransfer(props) {
        const basketToken = props.basket.basket.BasketToken;
        const searchToken = this.props.search.searchResult.ResultTokens.Transfer;
        const freeTransfer = props.searchResults.results.find(result => result.Price === 0);

        if (freeTransfer) {
            const componentModel = {
                basketToken,
                searchToken,
                componentToken: freeTransfer.ComponentToken,
                metaData: this.getMetaData(),
            };
            this.props.actions.basketAddComponent(componentModel);

            const selectedComponentToken = parseInt(freeTransfer.ComponentToken, 10);
            this.setState({ selectedComponentToken, freeTransferAdded: true });
        }
    }
    loadResultsFromBasketTransfer(basketTransfer) {
        const selected = Object.assign({}, this.state.selected);
        selected.departureAirport
            = basketTransfer.DepartureParentType === 'Airport'
            ? basketTransfer.DepartureParentID : 0;
        selected.outbound.flightTime = basketTransfer.OutboundJourneyDetails.Time;
        selected.outbound.flightNumber = basketTransfer.OutboundJourneyDetails.FlightCode
            ? basketTransfer.OutboundJourneyDetails.FlightCode : '';
        selected.inbound.flightTime = basketTransfer.ReturnJourneyDetails.Time;
        selected.inbound.flightNumber = basketTransfer.ReturnJourneyDetails.FlightCode
            ? basketTransfer.ReturnJourneyDetails.FlightCode : '';

        const selectedComponentToken = basketTransfer.ComponentToken;
        this.setState({
            selected,
            selectedComponentToken,
            isLoadingResultsFromBasket: true,
        });

        const transferResultToken = basketTransfer.SearchToken;
        this.props.actions.loadExtraSearchResults(transferResultToken, 'Transfer');
    }
    getBasketTransfer(props) {
        const basketComponents = props.basket.basket.Components;
        const basketTransfer = basketComponents.find(c => c.ComponentType === 'Transfer');
        return basketTransfer;
    }
    getBasketFlight(props) {
        const basketComponents = props.basket.basket.Components;
        const basketFlight = basketComponents.find(c => c.ComponentType === 'Flight');
        return basketFlight;
    }
    getBasketHotel(props) {
        const basketComponents = props.basket.basket.Components;
        const basketHotel = basketComponents.find(c => c.ComponentType === 'Hotel');
        return basketHotel;
    }
    getTransferUpsellProps() {
        const contentModel = this.props.entity.model;
        const selectedComponentToken = this.state.selectedComponentToken;

        const searchResults = this.props.searchResults;
        let displayedResults = [];
        if (searchResults.isLoaded) {
            displayedResults = searchResults.results.filter(result =>
                this.isCharterPackage(this.props.basket.basket) || result.Price > 0);
        }

        const props = {
            title: contentModel.Title,
            description: contentModel.Description,
            tableHeadings: contentModel.TableHeadings,
            outboundText: contentModel.OutboundHeader,
            inboundText: contentModel.ReturnHeader,
            airportSelectText: contentModel.AirportSelectText,
            airportPrependedText: contentModel.AirportPrependedText,
            searchButton: contentModel.SearchButton,
            failedSearchMessage: contentModel.FailedSearchMessage,
            flightCodeErrorMessage: contentModel.FlightCodeErrorMessage,
            flightTimeErrorMessage: contentModel.FlightTimeErrorMessage,
            outbound: this.state.selected.outbound,
            inbound: this.state.selected.inbound,
            performSearch: this.performHotelTransferSearch,
            handleDepartureAirportUpdate: this.handleDepartureAirportUpdate,
            handleFlightTimeFocusChange: this.handleFlightTimeFocusChange,
            handleFlightTimeUpdate: this.handleFlightTimeUpdate,
            handleFlightNumberUpdate: this.handleFlightNumberUpdate,
            handleModifyBasket: this.handleModifyBasket,
            updateFlightTime: this.updateFlightTime,
            searchMode: this.props.search.searchDetails.SearchMode,
            departureAirports: this.state.options.airports,
            selectedAirport: this.state.selected.departureAirport,
            validateInputs: this.state.validateInputs,
            searchFailed: this.state.searchFailed,
            results: displayedResults,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            selectedCurrency: this.props.session.UserSession.SelectCurrency,
            selectedComponentToken,
            addingComponent: this.state.addingComponent,
            waitMessage: contentModel.SearchWaitMessage,
            isSearching: this.props.search.extraSearch.Transfer
                && this.props.search.extraSearch.Transfer.isSearching,
        };
        return props;
    }
    handleFlightNumberUpdate(value, direction) {
        const selected = Object.assign({}, this.state.selected);
        const newValue = value;
        if (direction === 'outbound') {
            selected.outbound.flightNumber = newValue;
        } else {
            selected.inbound.flightNumber = newValue;
        }
        this.setState({ selected });
    }
    handleFlightTimeUpdate(value, direction) {
        const selected = Object.assign({}, this.state.selected);
        const newValue = value;
        if (direction === 'outbound') {
            selected.outbound.flightTime = newValue;
        } else {
            selected.inbound.flightTime = newValue;
        }
        this.setState({ selected });
    }
    handleFlightTimeFocusChange(direction, blurOrFocus) {
        const selected = Object.assign({}, this.state.selected);
        const applyFormatting = (blurOrFocus === 'blur');
        if (direction === 'outbound') {
            selected.outbound.applyFormatting
                = applyFormatting;
        } else {
            selected.inbound.applyFormatting
                = applyFormatting;
        }
        this.setState({ selected });
    }
    handleDepartureAirportUpdate(event) {
        const airportId = parseInt(event.target.value, 10);
        const selected = Object.assign({}, this.state.selected);
        selected.departureAirport = airportId;
        this.setState({ selected });
    }
    handleModifyBasket(componentToken) {
        const basket = this.props.basket.basket;
        const basketToken = basket.BasketToken;
        const basketTransfer = this.getBasketTransfer(this.props);
        const removeAndAdd = basketTransfer && basketTransfer.ComponentToken !== componentToken;
        const add = !basketTransfer;
        const searchToken = this.props.search.searchResult.ResultTokens.Transfer;
        const componentModel = {
            basketToken,
            searchToken,
            metaData: this.getMetaData(),
        };
        if (removeAndAdd) {
            componentModel.componentToken = basketTransfer.ComponentToken;
            this.props.actions.basketRemoveComponent(componentModel);
        }
        if (removeAndAdd || add) {
            componentModel.componentToken = componentToken;
            this.props.actions.basketAddComponent(componentModel);
            const selectedComponentToken = parseInt(componentToken, 10);
            this.setState({
                selectedComponentToken,
                addingComponent: false,
            });
        } else {
            this.setState({
                addingComponent: false,
            });
        }
    }
    isCharterPackage(basket) {
        let result = false;
        if (this.props.basket.isLoaded) {
            const flightIndex = basket.Components.findIndex(component =>
                component.ComponentType === 'Flight' && component.Source === 'Own');
            const propertyIndex = basket.Components.findIndex(component =>
                component.ComponentType === 'Hotel');
            result = (flightIndex > -1) && (propertyIndex > -1);
        }
        return result;
    }
    isTransferSearchComplete(props) {
        const transferSearchComplete = props.search.extraSearch.Transfer
                && !props.search.extraSearch.Transfer.isSearching
                && props.search.extraSearch.Transfer.searchComplete
                && props.search.searchResult.ResultTokens.Transfer;
        return transferSearchComplete;
    }
    performPackageTransferSearch(props) {
        const searchDetails = Object.assign({}, props.search.searchDetails);
        const basketHotel = this.getBasketHotel(props);
        const basketFlight = this.getBasketFlight(props);
        searchDetails.SearchMode = 'Transfer';
        searchDetails.DepartureType = 'Airport';
        searchDetails.DepartureId = basketFlight.ArrivalAirportId;
        searchDetails.ArrivalType = 'Property';
        searchDetails.ArrivalId = basketHotel.PropertyReferenceId;

        searchDetails.DepartureDate = basketFlight.OutboundFlightDetails.ArrivalDate;
        searchDetails.DepartureTime = basketFlight.OutboundFlightDetails.ArrivalTime;
        searchDetails.ReturnTime = basketFlight.ReturnFlightDetails.DepartureTime;

        const arrivalDate = moment(basketFlight.OutboundFlightDetails.ArrivalDate);
        const returnDate = moment(basketFlight.ReturnFlightDetails.DepartureDate);
        searchDetails.Duration = returnDate.diff(arrivalDate, 'days');

        props.actions.performExtraSearch('Transfer', searchDetails);
    }
    performHotelTransferSearch() {
        const searchDetails = Object.assign({}, this.props.search.searchDetails);
        const selected = Object.assign({}, this.state.selected);
        const allDetailsEntered = selected.outbound.flightTime
            && selected.outbound.flightNumber
            && selected.inbound.flightTime
            && selected.inbound.flightNumber;
        let validateInputs = true;
        if (allDetailsEntered) {
            const basketHotel = this.getBasketHotel(this.props);
            searchDetails.SearchMode = 'Transfer';
            searchDetails.DepartureType = 'Airport';
            searchDetails.DepartureId = selected.departureAirport;
            searchDetails.ArrivalType = 'Property';
            searchDetails.ArrivalId = basketHotel.PropertyReferenceId;
            searchDetails.DepartureTime = selected.outbound.flightTime;
            searchDetails.ReturnTime = selected.inbound.flightTime;
            validateInputs = false;
            this.props.actions.performExtraSearch('Transfer', searchDetails);
        }
        this.setState({ validateInputs });
    }
    shouldRenderHotelMode() {
        const shouldRender = this.state.ShouldRenderSearchForm
            && this.state.options.initialised
            && this.state.options.airports;
        return shouldRender;
    }
    shouldRenderPackageMode() {
        let shouldRender = false;
        let displayedResults = [];
        if (this.props.searchResults.isLoaded
                && this.props.basket.isLoaded) {
            displayedResults = this.props.searchResults.results.filter(result =>
                this.isCharterPackage(this.props.basket.basket) || result.Price > 0);
            shouldRender = displayedResults.length > 0;
        }
        return shouldRender;
    }
    setupHotelSearchOptions() {
        if (this.props.airports.items.length > 0) {
            const basketHotel = this.getBasketHotel(this.props);
            const resortId = basketHotel.GeographyLevel3Id;
            const allAirports = this.props.airports.items;
            const relevantAirports = [];
            allAirports.forEach(airport => {
                const airportResorts = airport.Resorts;
                const airportAssociatedWithResort
                    = airportResorts.filter(ar => ar.Id === resortId).length > 0;
                if (airportAssociatedWithResort) {
                    relevantAirports.push(airport);
                }
            });

            const options = Object.assign({}, this.state.options);
            options.airports = relevantAirports;
            options.isInitialised = true;

            const selected = Object.assign({}, this.state.selected);
            selected.departureAirport
                = relevantAirports.length > 0
                ? relevantAirports[0].Id
                : selected.departureAirport;

            this.setState({
                options,
                selected,
            });
        }
    }
    updateFlightTime(value, direction) {
        const selected = Object.assign({}, this.state.selected);
        const newValue = value;
        if (direction === 'outbound') {
            selected.outbound.flightTime = newValue;
        } else {
            selected.inbound.flightTime = newValue;
        }
        this.state.selected = selected;
    }
    isInitialised() {
        const isInitialised = this.props.search.isLoaded
            && this.props.basket.isLoaded
            && (this.state.renderMode !== this.renderModes.SEARCHFORM
                || this.state.options.isInitialised);
        return isInitialised;
    }
    renderPlaceholder() {
        const contentModel = this.props.entity.model;
        return (
            <div className="widget-transfer-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">{contentModel.Title}</h3>
                </div>
                <div className="panel-body">
                    <div className="wait-message-container">
                        <i className="loading-icon" aria-hidden="true"></i>
                        <div className="wait-message">
                            <p>Loading...</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        return (
            <div>
                {this.isInitialised()
                    && <TransferUpsell {...this.getTransferUpsellProps()} />}
                {!this.isInitialised()
                    && this.renderPlaceholder()}
            </div>
        );
    }
}

TransferUpsellContainer.propTypes = {
    airports: React.PropTypes.object,
    context: React.PropTypes.string,
    countries: React.PropTypes.object,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    site: React.PropTypes.object,
    search: React.PropTypes.object,
    searchResults: React.PropTypes.object,
    session: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const airports = state.airports ? state.airports : {};
    const basket = state.basket ? state.basket : {};
    const countries = state.countries ? state.countries : {};
    const site = state.site ? state.site : {};
    const search = state.search ? state.search : {};
    const searchResults = state.searchResults && state.searchResults.Transfer
            ? state.searchResults.Transfer : {};
    return {
        airports,
        basket,
        countries,
        site,
        search,
        searchResults,
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
        SearchActions,
        SearchResultActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(TransferUpsellContainer);
