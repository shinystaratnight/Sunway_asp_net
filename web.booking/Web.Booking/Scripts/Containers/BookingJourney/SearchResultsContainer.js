import * as AirportActions from 'actions/lookups/airportActions';
import * as AirportResortGroupActions from '../../actions/lookups/airportResortGroupActions';
import * as AlertActions from 'actions/bookingjourney/alertActions';
import * as AlertConstants from 'constants/alerts';
import * as BasketActions from 'actions/bookingjourney/basketactions';
import * as CountryActions from 'actions/lookups/countryActions';
import * as FilterFacilityActions from 'actions/lookups/filterFacilityActions';
import * as FlightCarrierActions from 'actions/lookups/flightCarrierActions';
import * as FlightClassActions from 'actions/lookups/flightClassActions';
import * as MealBasisActions from 'actions/lookups/mealBasisActions';
import * as QuoteActions from 'actions/bookingjourney/quoteActions';
import * as SearchActions from 'actions/bookingjourney/searchActions';
import * as SearchConstants from 'constants/search';
import * as SearchResultActions from 'actions/bookingjourney/searchResultActions';
import * as SearchResultsConstants from 'constants/searchresults';

import BasketFlight from 'components/searchresults/basketflight';
import FlightResult from 'components/searchresults/flightresult';
import React from 'react';
import SearchResultSummary from 'components/searchresults/searchresultsummary';
import SearchResultsFilter from 'widgets/bookingjourney/searchresultsfilter';
import SearchResultsList from 'widgets/bookingjourney/searchresultslist';
import StringFunctions from 'library/stringfunctions';
import UrlFunctions from '../../library/urlfunctions';
import WidgetContainer from 'containers/common/widgetcontainer';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class SearchResultsContainer extends React.Component {
    constructor(props) {
        super(props);
        const initialSearch = JSON.parse(JSON.stringify(this.props.search));
        this.state = {
            isDisabled: false,
            displayFilters: false,
            checkComponentToken: '',
            checkChangeFlight: '',
            initialSearch,
            filterDefaultsSetup: false,
            resultsTop: 0,
            scrollTimeout: null,
            cancellationChargesToDisplay: [],
        };
        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleFilterRangeChange = this.handleFilterRangeChange.bind(this);
        this.updateFilterValue = this.updateFilterValue.bind(this);
        this.handleSearchResultUpdateExpandedSummary
            = this.handleSearchResultUpdateExpandedSummary.bind(this);
        this.handleSortChange = this.handleSortChange.bind(this);
        this.handlePageChange = this.handlePageChange.bind(this);
        this.handleEmailQuoteShow = this.handleEmailQuoteShow.bind(this);
        this.handleChangeFlight = this.handleChangeFlight.bind(this);
        this.handleChangeFlightSelect = this.handleChangeFlightSelect.bind(this);
        this.handleToggleFiltersDisplay = this.handleToggleFiltersDisplay.bind(this);
        this.handleAddComponent = this.handleAddComponent.bind(this);
        this.handleGetCancellationCharges = this.handleGetCancellationCharges.bind(this);
        this.handleHideCancellationCharges = this.handleHideCancellationCharges.bind(this);
        this.scrollToTop = this.scrollToTop.bind(this);
        this.setDefaultSort = this.setDefaultSort.bind(this);
        this.renderSummary = this.renderSummary.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadCountriesIfNeeded();
        this.props.actions.loadMealBasisIfNeeded();
        this.props.actions.loadAirportsIfNeeded();
        this.props.actions.loadFlightCarriersIfNeeded();
        this.props.actions.loadFlightClassesIfNeeded();
        this.props.actions.loadFilterFacilities();
        this.props.actions.loadAirportResortGroupsIfNeeded();
        this.setupBasket();
        this.checkFlightSeatLock();
        this.checkWarning();
    }
    recommendedSort(attribute, secondarySort) {
        return (a, b) => {
            if (a.MetaData.ProductAttributes.indexOf(attribute) === -1
                && b.MetaData.ProductAttributes.indexOf(attribute) > -1) {
                return 1;
            }
            if (a.MetaData.ProductAttributes.indexOf(attribute) > -1
                && b.MetaData.ProductAttributes.indexOf(attribute) === -1) {
                return -1;
            }
            const nameA = a.DisplayName.toLowerCase();
            const nameB = b.DisplayName.toLowerCase();
            switch (secondarySort) {
                case 'Price Ascending':
                    if (a.Price > b.Price) {
                        return 1;
                    }
                    if (a.Price < b.Price) {
                        return -1;
                    }
                    return 0;
                case 'Average Score':
                    if (a.MetaData.ReviewAverageScore < b.MetaData.ReviewAverageScore) {
                        return 1;
                    }
                    if (a.MetaData.ReviewAverageScore > b.MetaData.ReviewAverageScore) {
                        return -1;
                    }
                    return 0;
                case 'Name (A to Z)':
                    if (nameA > nameB) {
                        return 1;
                    }
                    if (nameA < nameB) {
                        return -1;
                    }
                    return 0;
                default:
                    return 0;
                }
        };
    }
    setupRecommendedSort() {
        const configuration = this.props.entity.model.Configuration;
        const attribute = configuration.HotelSortOptions.RecommendedSort.Attribute;
        const secondarySort = configuration.HotelSortOptions.RecommendedSort.SecondaryAttribute;

        const sortfunc = this.recommendedSort(attribute, secondarySort);

        let name = 'Recommended';

        if (configuration.HotelSortOptions.RecommendedSort.Title
            && configuration.HotelSortOptions.RecommendedSort.Title !== '') {
            name = configuration.HotelSortOptions.RecommendedSort.Title;
        }

        const recommendedSort = {
            name,
            sortFunction: sortfunc,
        };

        return recommendedSort;
    }
    setDefaultSort(configuration) {
        let defaultSortOption = configuration.HotelSortOptions.DefaultSort;

        if (defaultSortOption === 'Recommended') {
            if (!configuration.HotelSortOptions.EnableRecommendedSort) {
                defaultSortOption = 'Price Ascending';
            } else if (configuration.HotelSortOptions.RecommendedSort.Title
                && configuration.HotelSortOptions.RecommendedSort.Title !== '') {
                defaultSortOption = configuration.HotelSortOptions.RecommendedSort.Title;
            }
        }
        this.props.actions.searchResultsSort(configuration.SearchMode,
                    defaultSortOption);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.search.searchComplete
            && nextProps.search.searchComplete) {
            const initialSearch = JSON.parse(JSON.stringify(nextProps.search));
            this.state.initialSearch = initialSearch;
            this.setState({ initialSearch });
        }

        const configuration = this.props.entity.model.Configuration;
        if (this.props.searchResults) {
            if (!this.props.searchResults.Hotel.isLoaded
                && nextProps.searchResults.Hotel.isLoaded
                && configuration.SearchMode === 'Hotel') {
                this.setDefaultSort(configuration);
            }
        }

        if (this.shouldLoadSearchResults(nextProps)) {
            const customSorts = [];

            if (configuration.HotelSortOptions.EnableRecommendedSort) {
                customSorts.push(this.setupRecommendedSort());
            }

            this.loadSearchResults(nextProps, customSorts);
        }

        if (this.state.checkChangeFlight) {
            this.checkChangeFlight(nextProps);
        }

        if (this.state.checkComponentToken) {
            this.checkComponentToken(nextProps);
        }

        if (this.shouldSearchBookingAdjustments(nextProps)) {
            this.searchBookingAdjustments(nextProps);
        }

        const searchMode = this.props.entity.model.Configuration.SearchMode;
        const filters = nextProps.searchResults[searchMode].filters;
        if (!this.state.filterDefaultsSetup && filters.length > 0) {
            const options = this.props.entity.model[`${searchMode}FilterOptions`];
            this.props.actions.setupDefaultFilterOptions(searchMode, options);
            this.setState({ filterDefaultsSetup: true });
        }
    }
    componentDidUpdate(prevProps) {
        const searchMode = this.props.entity.model.Configuration.SearchMode;
        const searchResults = this.props.searchResults[searchMode];
        const previousSearchResults = prevProps.searchResults[searchMode];
        if (searchResults.isLoaded
            && searchResults.currentPage
                !== previousSearchResults.currentPage) {
            const propertyContainerRect
                = document.getElementById('search-result-list-container').getBoundingClientRect();
            this.state.resultsTop = propertyContainerRect.top + window.scrollY;
            this.scrollToTop();
        }
    }
    checkWarning() {
        const warning = UrlFunctions.getQueryStringValue('warning');
        if (warning === 'prebookfailed') {
            this.props.actions.addAlert('SearchResults_PreBookFailed',
            AlertConstants.ALERT_TYPES.DANGER,
            'Sorry we were unable to confirm this booking. Please choose an alternative.');
        }
        if (warning === 'quote-flight') {
            let noFlightWarning = 'The flight you have selected is no longer available, ';
            noFlightWarning += 'please check flight times or select another flight.';
            this.props.actions.addAlert('Quote_Flight',
                AlertConstants.ALERT_TYPES.WARNING,
                noFlightWarning);
        }
        if (warning === 'quote-hotel') {
            let noHotelWarning = 'The accommodation you have selected is no longer available, ';
            noHotelWarning += 'please select new accommodation.';
            this.props.actions.addAlert('Quote_Hotel',
                AlertConstants.ALERT_TYPES.WARNING,
                noHotelWarning);
        }
    }
    checkComponentToken(nextProps) {
        const componentIndex = nextProps.basket.basket.Components.findIndex(component =>
            component.ComponentToken === this.state.checkComponentToken);

        const redirect
            = (componentIndex > -1)
            && nextProps.basket.basket.Components[componentIndex].ComponentPreBooked;
        if (redirect) {
            const page = window.location.pathname.split('/')[2];
            const extrasUrl = window.location.pathname.replace(`/booking/${page}`,
                '/booking/extras');
            window.location = `${extrasUrl}?t=${this.props.basket.basket.BasketToken}`;
        }

        if (componentIndex > -1
            && !nextProps.basket.basket.Components[componentIndex].ComponentPreBooked
            && nextProps.basket.basket.Warnings.length > 0) {
            const component = nextProps.basket.basket.Components[componentIndex];
            const componentType = component.ComponentType.toLowerCase();
            this.props.actions.addAlert('SearchResults_PreBookFailed',
            AlertConstants.ALERT_TYPES.DANGER,
            `Sorry we were unable to confirm this ${componentType}. Please choose an alternative`);
        }
    }
    checkChangeFlight(nextProps) {
        const nextSelectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
            nextProps.searchResults);
        const searchDetails = nextProps.search.searchDetails;

        if (nextSelectedFlight.ComponentToken === this.state.checkChangeFlight) {
            const hotelResults = this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL];
            const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);

            this.props.actions.filterSearchResults(SearchConstants.SEARCH_MODES.HOTEL,
               hotelResults.results, hotelResults.filters, flightPlusHotelType);
            this.state.checkChangeFlight = '';

            const componentToken = parseInt(UrlFunctions.getQueryStringValue('sf'), 10);
            if (componentToken) {
                const url = window.location.href.replace(componentToken,
                    nextSelectedFlight.ComponentToken);
                history.pushState({}, '', url);
            }

            this.searchBookingAdjustments(nextProps);
        }
    }
    checkFlightSeatLock() {
        const configuration = this.props.entity.model.Configuration;
        const renderedSearchMode = configuration.SearchMode;
        const basketToken = UrlFunctions.getQueryStringValue('t');
        if (basketToken && renderedSearchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            this.props.actions.basketReleaseFlightSeatLock(basketToken);
        }
    }
    displayChangeFlight() {
        const bookingJourneyConfig = this.props.site.SiteConfiguration.BookingJourneyConfiguration;
        const pageUrl = this.props.page.PageURL;
        const display = bookingJourneyConfig.ChangeFlightPages.indexOf(pageUrl) > -1;
        return display;
    }
    displayResults() {
        let display = true;
        const configuration = this.props.entity.model.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];

        if (this.isChangeFlight() && !searchResults.isChangingFlight) {
            display = false;
        }
        return display;
    }
    getFlightResultProps(selectedResult) {
        const configuration = this.props.entity.model.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        const displayedResults = searchResults.results.filter(result => result.Display === true);
        const collapseMobile = configuration.CollapseOnMobile;
        const flightResultProps = {
            result: selectedResult,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            flightButtons: this.props.entity.model.FlightButtons,
            changeFlight: true,
            onChangeFlight: this.handleChangeFlight,
            onToggleSummary: this.handleSearchResultUpdateExpandedSummary,
            isChangingFlight: searchResults.isChangingFlight,
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            charterFlightText: this.props.entity.model.CharterFlightText,
            charterPackageText: this.props.entity.model.CharterPackageText,
            searchMode: this.props.search.searchDetails.SearchMode,
            rooms: this.props.search.searchDetails.Rooms,
            totalResults: displayedResults.length,
            collapseMobile,
        };
        return flightResultProps;
    }
    getBasketFlightProps(basketFlight) {
        const basketFlightProps = {
            flight: basketFlight,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
        };
        return basketFlightProps;
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
    getSearchToken(searchMode, resultTokens) {
        let searchToken = '';
        switch (searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL: {
                const hotelResultToken = UrlFunctions.getQueryStringValue('h');
                searchToken = hotelResultToken ? hotelResultToken : resultTokens.Hotel;
                break;
            }
            case SearchConstants.SEARCH_MODES.FLIGHT: {
                const flightResultToken = UrlFunctions.getQueryStringValue('f');
                searchToken = flightResultToken ? flightResultToken : resultTokens.Flight;
                break;
            }
            default:
        }
        return searchToken;
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
    getPaxSummary(roomCount, adultCount, childCount, infantCount) {
        let paxSummary = roomCount > 1 ? `${roomCount} rooms, ` : '';
        paxSummary += StringFunctions.getPluralisationValue(adultCount, 'Adult', 'Adults');
        paxSummary += childCount > 0
            ? `, ${StringFunctions.getPluralisationValue(childCount, 'Child', 'Children')}` : '';
        paxSummary += infantCount > 0
            ? `, ${StringFunctions.getPluralisationValue(infantCount, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    getSearchResultsFilterProps() {
        const contentModel = this.props.entity.model;
        const configuration = this.props.entity.model.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        const searchDetails = this.state.initialSearch.searchDetails;
        const renderedSearchMode = configuration.SearchMode;

        let priceAdjustment = this.getAdjustmentAmount();
        if (this.shouldAddFlightPrice()) {
            const basketFlight = this.props.basket.basket.Components
                .find(component => component.ComponentType === 'Flight');
            if (basketFlight) {
                priceAdjustment += Math.ceil(basketFlight.TotalPrice);
            } else {
                const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
                    this.props.searchResults);
                priceAdjustment += Math.ceil(selectedFlight.Price);
            }
        }
        return {
            searchResults: searchResults.results,
            searchDetails,
            renderedSearchMode,
            isPackageSearch: this.isPackageSearch(),
            countries: this.props.countries.items,
            mealBasis: this.props.mealBasis.items,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            filters: searchResults.filters.filter(f => f.Hidden !== true),
            onChange: this.handleFilterChange,
            onRangeChange: this.handleFilterRangeChange,
            updateValueFunction: this.updateFilterValue,
            handleToggleFiltersDisplay: this.handleToggleFiltersDisplay,
            displayFilters: this.state.displayFilters,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            priceAdjustment,
            searchSummaryHeaderFormat: contentModel.SearchSummaryTitle,
            displayRatingAsStars: contentModel.DisplayRatingAsStars,
            updatingPrice: this.props.search.isSearchingForAdjustments,
        };
    }
    getSearchResultsListProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        const resultsPerPage = parseInt(configuration.ResultsPerPage, 10);
        const resultStart = (searchResults.currentPage - 1) * resultsPerPage;

        const displayedResults = searchResults.results.filter(result => result.Display === true);
        const pagedResults = displayedResults.slice(resultStart, resultStart + resultsPerPage);

        const changeFlight = this.isChangeFlight();

        const siteConfig = this.props.site.SiteConfiguration;
        const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
        const renderMobileSummary = bookingJourneyConfig.RenderMobileSummary;
        const totalPages = Math.ceil(displayedResults.length / resultsPerPage);
        const props = {
            searchDetails: this.state.initialSearch.searchDetails,
            renderedSearchMode: configuration.SearchMode,
            resultTokens: this.props.search.searchResult.ResultTokens,
            searchResults: pagedResults,
            totalResults: displayedResults.length,

            sortOptions: searchResults.sortOptions,
            selectedSort: searchResults.selectedSort,
            onSortChange: this.handleSortChange,

            totalPages,
            currentPage: searchResults.currentPage,
            pageLinks: totalPages,
            onPageClick: this.handlePageChange,

            countries: this.props.countries.items,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            starRatingConfiguration: this.props.site.SiteConfiguration.StarRatingConfiguration,
            mealBasis: this.props.mealBasis.items,
            airports: this.props.airports.items,
            flightCarriers: this.props.flightCarriers.items,
            flightClasses: this.props.flightClasses.items,
            flightButtons: contentModel.FlightButtons,
            airportResortGroups: this.props.airportResortGroups.items,

            renderSubResults: false,
            changeFlight,
            onChangeFlightSelect: this.handleChangeFlightSelect,

            onToggleSummary: this.handleSearchResultUpdateExpandedSummary,
            onComponentAdd: this.handleAddComponent,
            cmsBaseUrl: this.props.site.SiteConfiguration.CmsConfiguration.BaseUrl,
            searchSummaryHeaderFormat: contentModel.SearchSummaryTitle,
            onGetCancellationCharges: this.handleGetCancellationCharges,
            handleHideCancellationCharges: this.handleHideCancellationCharges,
            cancellationCharges: this.props.basket.cancellationCharges,
            cancellationChargesToDisplay: this.state.cancellationChargesToDisplay,

            onQuoteShow: this.handleEmailQuoteShow,

            renderPaxSummary: configuration.RenderPaxSummary,
            charterPackageText: contentModel.CharterPackageText,
            charterFlightText: contentModel.CharterFlightText,

            customerComponents: this.props.customerComponents,

            updatingPrice: this.props.search.isSearchingForAdjustments,
            adjustmentAmount: this.getAdjustmentAmount(),
            basket: this.props.basket.basket,
            roomTypeCharacterLimit: parseInt(configuration.RoomTypeCharacterLimit, 10) || 0,
            renderMobileSummary,
            displayQuoteEmail: configuration.DisplayQuoteEmail,
            quoteEmailTooltip: configuration.QuoteEmailTooltip,
    };

        if (this.isPackageSearch()) {
            const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
                this.props.searchResults);
            props.selectedFlight = selectedFlight;
        }

        return props;
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
    handleSortChange(event) {
        const configuration = this.props.entity.model.Configuration;
        const sortOption = event.target.value;
        this.props.actions.searchResultsSort(configuration.SearchMode, sortOption);
    }
    handlePageChange(page) {
        const configuration = this.props.entity.model.Configuration;
        this.props.actions.searchResultsUpdatePage(configuration.SearchMode, page);
    }
    handleFilterChange(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateFilterValue(field, value);
    }
    handleFilterRangeChange(key, value, type) {
        const field = type === 'min' ? `${key}.minValue` : `${key}.maxValue`;
        this.updateFilterValue(field, value);
    }
    handleChangeFlight() {
        const configuration = this.props.entity.model.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        this.props.actions.searchResultsChangeFlight(configuration.SearchMode,
            !searchResults.isChangingFlight);
    }
    handleChangeFlightSelect(componentToken) {
        const configuration = this.props.entity.model.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        this.props.actions.searchResultsSetSelected(configuration.SearchMode,
            searchResults.results, componentToken);
        this.state.checkChangeFlight = componentToken;
    }
    handleSearchResultUpdateExpandedSummary(id) {
        const configuration = this.props.entity.model.Configuration;
        const renderedSearchMode = configuration.SearchMode;
        this.props.actions.searchResultsUpdateExpandedSummary(renderedSearchMode, id);
    }
    handleEmailQuoteShow(token, cheapestResults) {
        this.props.actions.showQuoteEmailPopup(token, cheapestResults, this.props.site);
    }
    handleAddComponent(componentToken, subComponentTokens) {
        const configuration = this.props.entity.model.Configuration;
        const searchToken = this.getSearchToken(configuration.SearchMode,
            this.props.search.searchResult.ResultTokens);
        const componentType = configuration.SearchMode;
        const componentModel = {
            BasketToken: this.props.basket.basket.BasketToken,
            SearchToken: searchToken,
            ComponentToken: componentToken,
            SubComponentTokens: subComponentTokens,
            componentType,
        };
        this.state.checkComponentToken = componentToken;
        this.props.actions.basketAddComponent(componentModel);
    }
    handleGetCancellationCharges(componentToken, subComponentTokens) {
        if (!this.props.basket.cancellationCharges[componentToken]) {
            const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
                this.props.searchResults);
            const configuration = this.props.entity.model.Configuration;
            const searchToken = this.getSearchToken(configuration.SearchMode,
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
    handleToggleFiltersDisplay() {
        this.setState({
            displayFilters: !this.state.displayFilters,
        });
    }
    hasExistingComponent(componentType) {
        const basket = this.props.basket;
        const basketComponentExists = basket.isLoaded
            && basket.basket.Components.some(component =>
                component.ComponentType === componentType);
        return basketComponentExists;
    }
    isInitialised() {
        return this.props.entity.isLoaded
            && this.props.search.isLoaded
            && this.props.search.searchComplete
            && this.props.countries.isLoaded
            && this.props.mealBasis.isLoaded
            && this.props.airports.isLoaded
            && this.props.flightCarriers.isLoaded
            && this.props.flightClasses.isLoaded
            && this.props.filterFacilities.isLoaded
            && this.props.basket.isLoaded
            && (this.hasExistingComponent(this.props.entity.model.Configuration.SearchMode)
                || this.isResultsLoaded());
    }
    isResultsLoaded() {
        let isResultsLoaded;
        const searchMode = this.props.entity.model.Configuration.SearchMode;
        const componentCheck = searchMode === SearchConstants.SEARCH_MODES.FLIGHT
            ? SearchConstants.SEARCH_MODES.HOTEL
            : SearchConstants.SEARCH_MODES.FLIGHT;
        if (this.isPackageSearch() && !this.hasExistingComponent(componentCheck)) {
            isResultsLoaded = this.props.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded
                && this.props.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].results.length > 0
                && this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL].isLoaded
                && this.props.searchResults[SearchConstants.SEARCH_MODES.HOTEL].results.length > 0;
        } else {
            isResultsLoaded = this.props.searchResults[searchMode].isLoaded
                && this.props.searchResults[searchMode].results.length > 0;
        }
        return isResultsLoaded;
    }
    isPackageSearch() {
        const searchDetails = this.state.initialSearch.searchDetails;
        const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);
        const flightPlusHotelTypes = SearchResultsConstants.FLIGHT_PLUS_HOTEL_TYPES;

        return searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
            && (flightPlusHotelType === flightPlusHotelTypes.PACKAGESEARCH
                || flightPlusHotelType === flightPlusHotelTypes.PACKAGEPRICE);
    }
    isChangeFlight() {
        const configuration = this.props.entity.model.Configuration;
        const isPackageSearch = this.isPackageSearch();
        return isPackageSearch && configuration.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT;
    }
    isValidSearchMode(searchMode, renderedSearchMode) {
        let valid = false;
        switch (renderedSearchMode) {
            case SearchConstants.SEARCH_MODES.FLIGHT:
                valid = searchMode === SearchConstants.SEARCH_MODES.FLIGHT
                    || searchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL;
                break;
            case SearchConstants.SEARCH_MODES.HOTEL:
                valid = searchMode === SearchConstants.SEARCH_MODES.HOTEL
                    || searchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL;
                break;
            default:
        }
        return valid;
    }
    loadSearchResults(nextProps, customSorts) {
        const configuration = nextProps.entity.model.Configuration;
        const searchDetails = nextProps.search.searchDetails;
        if (this.isValidSearchMode(searchDetails.SearchMode, configuration.SearchMode)) {
            const searchToken = this.getSearchToken(configuration.SearchMode,
                nextProps.search.searchResult.ResultTokens);
            const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);
            this.props.actions.loadSearchResults(searchToken,
                configuration.SearchMode, flightPlusHotelType, null, customSorts);
        } else {
            this.state.isDisabled = true;
        }
    }
    scrollToTop() {
        if (window.scrollY > this.state.resultsTop) {
            const scrollBy = 50;
            const diff = window.scrollY - this.state.resultsTop;
            const nextScrollBy = diff > scrollBy ? scrollBy : diff;
            window.scrollBy(0, -nextScrollBy);
            this.scrollTimeout = setTimeout(this.scrollToTop, 10);
        } else {
            clearTimeout(this.scrollTimeout);
        }
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
            selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT,
                nextProps.searchResults);
        }

        let property = null;
        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
                || searchDetails.SearchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            property = nextProps.searchResults[SearchConstants.SEARCH_MODES.HOTEL].results[0];
        }

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
    setupBasket() {
        const basketToken = UrlFunctions.getQueryStringValue('t');
        if (basketToken) {
            this.props.actions.loadBasket(basketToken);
        } else {
            this.props.actions.createBasket();
        }
    }
    shouldAddFlightPrice() {
        const configuration = this.props.entity.model.Configuration;
        const renderedSearchMode = configuration.SearchMode;
        return this.isPackageSearch()
            && renderedSearchMode === SearchConstants.SEARCH_MODES.HOTEL;
    }
    shouldLoadSearchResults(nextProps) {
        const renderedSearchMode = this.props.entity.model.Configuration.SearchMode;
        const searchDetails = this.state.initialSearch.searchDetails;

        let flightLoadCheck = true;
        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
            && renderedSearchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            const basketFlightExists = nextProps.basket.isLoaded
                && nextProps.basket.basket.Components
                .some(component => component.ComponentType === 'Flight');

            flightLoadCheck = nextProps.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded
                || basketFlightExists;
        }
        const hotelResultToken = UrlFunctions.getQueryStringValue('h');
        const flightResultToken = UrlFunctions.getQueryStringValue('f');

        const existingResults = ((renderedSearchMode === SearchConstants.SEARCH_MODES.HOTEL
                && hotelResultToken)
            || (renderedSearchMode === SearchConstants.SEARCH_MODES.FLIGHT && flightResultToken));

        const existingComponent = nextProps.basket.isLoaded
                && nextProps.basket.basket.Components
                .some(component => component.ComponentType === renderedSearchMode);

        return !this.state.isDisabled
            && !existingComponent
            && nextProps.filterFacilities.isLoaded
            && nextProps.search.isLoaded
            && nextProps.airports.isLoaded
            && (nextProps.search.searchComplete || existingResults)
            && !nextProps.searchResults[renderedSearchMode].isLoaded
            && !nextProps.searchResults[renderedSearchMode].isFetching
            && flightLoadCheck;
    }
    shouldRenderLoader() {
        let shouldRenderLoader = true;

        const configuration = this.props.entity.model.Configuration;
        const searchDetails = this.state.initialSearch.searchDetails;

        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL
                && configuration.SearchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            shouldRenderLoader = false;
        }

        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT
            && configuration.SearchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            shouldRenderLoader = false;
        }

        if (searchDetails.SearchMode === SearchConstants.SEARCH_MODES.HOTEL
            && configuration.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            shouldRenderLoader = false;
        }

        return shouldRenderLoader;
    }
    shouldSearchBookingAdjustments(nextProps) {
        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;

        const searchDetails = this.state.initialSearch.searchDetails;
        const hotelOnly = searchDetails.SearchMode === SearchConstants.SEARCH_MODES.HOTEL;
        const flightOnly = searchDetails.SearchMode === SearchConstants.SEARCH_MODES.FLIGHT;

        const flightLoadCheck
            = nextProps.searchResults[SearchConstants.SEARCH_MODES.FLIGHT].isLoaded;
        const hotelLoadCheck
            = nextProps.searchResults[SearchConstants.SEARCH_MODES.HOTEL].isLoaded;

        return searchConfig.SearchBookingAdjustments
            && !nextProps.search.adjustmentSearchComplete
            && !nextProps.search.isSearchingForAdjustments
            && (hotelOnly || flightLoadCheck)
            && (flightOnly || hotelLoadCheck);
    }
    updateFilterValue(field, value) {
        const searchMode = this.props.entity.model.Configuration.SearchMode;
        const searchDetails = this.state.initialSearch.searchDetails;
        const flightPlusHotelType = this.getFlightPlusHotelType(searchDetails);
        this.props.actions.searchResultsFilterUpdateValue(searchMode,
            flightPlusHotelType, field, value);
    }
    renderSearchSummary() {
        const searchDetails = this.props.search.searchDetails;
        const departureDate = moment(searchDetails.DepartureDate).format('DD MMMM Y');
        const returnDate = moment(searchDetails.DepartureDate)
            .add(searchDetails.Duration, 'days').format('DD MMMM Y');
        const duration = `${searchDetails.Duration} nights`;

        const roomCount = searchDetails.Rooms.length;
        let adultCount = 0;
        let childCount = 0;
        let infantCount = 0;

        for (let i = 0; i < searchDetails.Rooms.length; i++) {
            const room = searchDetails.Rooms[i];
            adultCount += room.Adults;
            childCount += room.Children;
            infantCount += room.Infants;
        }

        const pax = this.getPaxSummary(roomCount, adultCount, childCount, infantCount);
        const searchSummary = `For ${pax}, ${duration} (${departureDate} - ${returnDate})`;

        const summaryClass = `search-summary ${this.props.search.searchDetails.SearchMode}`;

        return (
            <p className={summaryClass}>{searchSummary}</p>
        );
    }
    renderSelectedFlight() {
        const configuration = this.props.entity.model.Configuration;
        const selectedResult = this.getSelectedResult(configuration.SearchMode,
            this.props.searchResults);

        const basketFlight = this.props.basket.basket.Components
            .find(component => component.ComponentType === 'Flight');

        let FlightComponent = FlightResult;
        if (this.props.customerComponents.hasOwnProperty('FlightResult')) {
            FlightComponent = this.props.customerComponents.FlightResult;
        }

        let BasketFlightComponent = BasketFlight;
        if (this.props.customerComponents.hasOwnProperty('BasketFlight')) {
            BasketFlightComponent = this.props.customerComponents.BasketFlight;
        }
        const renderSummary = configuration.RenderSearchSummary;

        const siteConfig = this.props.site.SiteConfiguration;
        const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
        const renderSearchTool = bookingJourneyConfig.SearchToolLocation === 'Flight';
        const widgetContainerProps = {
            entityName: 'SearchTool',
            context: `search-again-${this.props.site.Name.toLowerCase()}`,
            entitySpecific: false,
            editable: true,
            shared: true,
        };
        let flightClass = 'search-selected-flight col-xs-12';
        if (renderSearchTool) {
            flightClass += ' col-md-9';
        }
        return (
            <div className="flight-search-container">
                 {renderSearchTool
                    && <div className="flight-searchtool col-md-3 col-xs-12">
                            <WidgetContainer {...widgetContainerProps} />
                        </div>
                 }
                <div className={flightClass}>
                    {(selectedResult || basketFlight)
                        && <h3 className="h-tertiary">
                                {this.props.entity.model.FlightResultTitle}
                            </h3>}
                    {(selectedResult || basketFlight) && renderSummary
                            && this.renderSearchSummary()}
                    {selectedResult
                        && <FlightComponent {...this.getFlightResultProps(selectedResult)} />}
                    {basketFlight
                        && <BasketFlightComponent {...this.getBasketFlightProps(basketFlight)} />}
                </div>
            </div>
        );
    }
    renderListView() {
        return (
            <div className="col-xs-12 col-md-9">
               <SearchResultsList {...this.getSearchResultsListProps()} />
            </div>
        );
    }
    renderSummary(mobileOnly) {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const searchResults = this.props.searchResults[configuration.SearchMode];
        const displayedResults = searchResults.results.filter(result => result.Display === true);

        const searchResultSummaryProps = {
            searchDetails: this.props.search.searchDetails,
            searchMode: configuration.SearchMode,
            totalResults: displayedResults.length,
            countries: this.props.countries.items,
            changeFlight: this.isChangeFlight(),
            airportResortGroups: this.props.airportResortGroups.items,
        };
        if (contentModel.SearchSummaryTitle) {
            searchResultSummaryProps.headerFormat = contentModel.SearchSummaryTitle;
        }

        let containerClass = 'col-xs-12 search-result-summary-container';
        if (mobileOnly) {
            containerClass += ' hidden-md-up';
        }
        return (
            <div className={containerClass}>
                <SearchResultSummary {...searchResultSummaryProps} />
            </div>
        );
    }
    renderFilter() {
        const widgetContainerProps = {
            entityName: 'SearchTool',
            context: `search-again-${this.props.site.Name.toLowerCase()}`,
            entitySpecific: false,
            editable: true,
            shared: true,
        };
        const siteConfig = this.props.site.SiteConfiguration;
        const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
        const renderSearchTool = (bookingJourneyConfig.SearchToolLocation === 'Hotel'
                                    && this.isPackageSearch())
                                || !this.isPackageSearch();
        return (
            <div className="col-xs-12 col-md-3 search-filter">
                {renderSearchTool
                    && <WidgetContainer {...widgetContainerProps} />
                }
                <SearchResultsFilter {...this.getSearchResultsFilterProps()} />
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
        const isInitialised = this.isInitialised();
        const displayResults = isInitialised && this.displayResults();
        const configuration = this.props.entity.model.Configuration;
        const renderedSearchMode = configuration.SearchMode;

        let containerClass = 'search-results-container container';
        containerClass += ` ${renderedSearchMode.toLowerCase()}`;
        containerClass += this.isChangeFlight() ? ' change-flight' : '';

        const siteConfig = this.props.site.SiteConfiguration;
        const bookingJourneyConfig = siteConfig.BookingJourneyConfiguration;
        const renderMobileSummary = bookingJourneyConfig.RenderMobileSummary;

        return (
            <div className={containerClass} id="search-result-list-container">
                <div className="row">
                    {isInitialised
                        && this.isChangeFlight()
                        && this.displayChangeFlight()
                        && this.renderSelectedFlight()}

                    {isInitialised
                        && displayResults
                        && renderMobileSummary
                        && this.renderSummary(true)}

                    {isInitialised
                       && displayResults
                       && this.renderFilter()}

                     {isInitialised
                        && displayResults
                        && this.renderListView()}

                    {this.shouldRenderLoader()
                        && !isInitialised
                        && !this.hasExistingComponent()
                        && this.props.search.searchComplete
                        && this.renderLoader()}
                </div>
            </div>
        );
    }
}

SearchResultsContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.object.isRequired,
    countries: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    flightCarriers: React.PropTypes.object.isRequired,
    flightClasses: React.PropTypes.object.isRequired,
    filterFacilities: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    customerComponents: React.PropTypes.object,
    airportResortGroups: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const searchResults = state.searchResults ? state.searchResults : {};
    const countries = state.countries ? state.countries : {};
    const mealBasis = state.mealBasis ? state.mealBasis : {};
    const airports = state.airports ? state.airports : {};
    const flightCarriers = state.flightCarriers ? state.flightCarriers : {};
    const flightClasses = state.flightClasses ? state.flightClasses : {};
    const filterFacilities = state.filterFacilities ? state.filterFacilities : {};
    const search = state.search ? state.search : {};
    const session = state.session ? state.session : {};
    const site = state.site ? state.site : {};
    const basket = state.basket ? state.basket : {};
    const airportResortGroups = state.airportResortGroups ? state.airportResortGroups : {};

    return {
        searchResults,
        countries,
        mealBasis,
        airports,
        flightCarriers,
        flightClasses,
        filterFacilities,
        search,
        session,
        site,
        basket,
        airportResortGroups,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        AlertActions,
        AirportActions,
        AirportResortGroupActions,
        BasketActions,
        CountryActions,
        FlightCarrierActions,
        FlightClassActions,
        FilterFacilityActions,
        MealBasisActions,
        QuoteActions,
        SearchActions,
        SearchResultActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SearchResultsContainer);
