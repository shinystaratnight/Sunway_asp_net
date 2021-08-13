import * as AirportActions from '../../actions/lookups/airportActions';
import * as AirportGroupActions from '../../actions/lookups/airportGroupActions';
import * as AirportResortGroupActions from '../../actions/lookups/airportResortGroupActions';
import * as AlertActions from '../../actions/bookingjourney/alertActions';
import * as AlertConstants from 'constants/alerts';
import * as BrandActions from '../../actions/lookups/brandActions';
import * as CountryActions from '../../actions/lookups/countryActions';
import * as DFFlightCacheActions from '../../actions/lookups/dealFinderFlightCacheRouteActions';
import * as EntityActions from '../../actions/content/entityActions';
import * as FlightCacheRouteActions from '../../actions/lookups/flightCacheRouteActions';
import * as FlightClassActions from '../../actions/lookups/flightClassActions';
import * as RouteAvailabilityActions from '../../actions/lookups/routeAvailabilityActions';
import * as SearchActions from '../../actions/bookingjourney/searchActions';
import * as SearchConstants from 'constants/search';

import React from 'react';
import SearchTool from '../../widgets/bookingjourney/searchtool';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class SearchToolContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            useDealFinder: this.props.site.SiteConfiguration.SearchConfiguration.UseDealFinder,
        };
        this.setSearchDetails = this.setSearchDetails.bind(this);
        this.updateProperty = this.updateProperty.bind(this);
        this.changeDate = this.changeDate.bind(this);
        this.handleSearch = this.handleSearch.bind(this);
        this.getAirport = this.getAirport.bind(this);
        this.setUseDealFinder = this.setUseDealFinder.bind(this);
    }

    isInitialised() {
        const configuration = this.props.entity.model.Configuration;
        const needAirportResortGroups = configuration.DestinationSearchOptions.DisplayAirportGroups;
        const showAirportGroups = configuration.DepartureSearchOptions.ShowDepartureAirportGroups;
        return this.props.search.isLoaded
            && this.props.countries.isLoaded
            && this.props.airports.isLoaded
            && this.props.airportGroups.isLoaded
            || !showAirportGroups
            && this.props.flightClasses.isLoaded
            && (this.props.airportResortGroups.isLoaded || !needAirportResortGroups)
            && configuration.DisplayWidget;
    }

    componentDidMount() {
        const config = this.props.entity.model.Configuration;
        const departureSearchOptions = config.DepartureSearchOptions;
        const showPreferredAirportsFirst = departureSearchOptions.ShowPreferredAirportsFirst;
        const groupByAirportGroups = departureSearchOptions.GroupByAirportGroups;
        this.props.actions.loadBrands();
        this.props.actions.loadRoutes();
        this.props.actions.loadAirportsIfNeeded();
        this.props.actions.loadAirportGroupsIfNeeded(groupByAirportGroups,
            showPreferredAirportsFirst);
        this.props.actions.loadCountriesIfNeeded();
        this.props.actions.loadFlightClassesIfNeeded();

        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;
        this.props.actions.loadSearchDetailsIfNeeded(searchConfig);

        if (this.props.entity.model.Configuration.DestinationSearchOptions.DisplayAirportGroups) {
            this.props.actions.loadAirportResortGroupsIfNeeded();
        }
        this.warningCheck();

        if (this.props.search.searchDetails.DepartureId
                && this.props.search.searchDetails.ArrivalId) {
            this.updateCalendar(this.props);
        }
        this.props.actions.loadModelsFromEntityType(this.props.entity.site, 'Calendar');
    }

    componentWillReceiveProps(nextProps) {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        if (configuration.CalendarOptions
                && configuration.CalendarOptions.HighlightFlightCacheDates) {
            this.checkRouteChange(nextProps);
            if (this.state.useDealFinder) {
                this.validateFlightCacheResults(nextProps);
            }
        }
    }

    checkRouteChange(nextProps) {
        const currentSearchDetails = this.props.search.searchDetails;
        const updatedSearchDetails = nextProps.search.searchDetails;
        if (updatedSearchDetails.DepartureId
            && updatedSearchDetails.ArrivalId
            && ((currentSearchDetails.ArrivalId !== updatedSearchDetails.ArrivalId)
                || (currentSearchDetails.DepartureId !== updatedSearchDetails.DepartureId)
                || (currentSearchDetails.DepartureDate !== updatedSearchDetails.DepartureDate))) {
            if (!this.state.useDealFinder) {
            this.updateCalendar(nextProps);
            } else if (currentSearchDetails.ArrivalId !== updatedSearchDetails.ArrivalId
                || currentSearchDetails.DepartureId !== updatedSearchDetails.DepartureId) {
                const s = nextProps.search.searchDetails;
                const departureDate = s.DepartureDate ? s.DepartureDate : new Date().toDateString();
                this.props.actions.clearAndLoadDealFinderFlightCacheDates(s.DepartureId,
                    s.ArrivalType, s.ArrivalId, departureDate, !s.DepartureDate, !s.Duration);
            }
        }
    }

    warningCheck() {
        const warning = UrlFunctions.getQueryStringValue('warning');
        if (warning && warning === 'noresults') {
            this.props.actions.addAlert('Search_NoResults',
                AlertConstants.ALERT_TYPES.WARNING,
                'Sorry your search returned no results. Please try an alternative search below.');
        }
        if (warning && warning === 'quote-nopackage') {
            let noPackageWarning = 'The package you have selected is no longer available, ';
            noPackageWarning += 'please search for a new package.';
            this.props.actions.addAlert('Quote_NoPackage',
                AlertConstants.ALERT_TYPES.WARNING,
                noPackageWarning);
        }
        if (warning && warning === 'quote-noflight') {
            let noFlightWarning = 'The flight you have selected is no longer available, ';
            noFlightWarning += 'please try searching with alternative dates.';
            this.props.actions.addAlert('Quote_NoFlight',
                AlertConstants.ALERT_TYPES.WARNING,
                noFlightWarning);
        }
        if (warning && warning === 'quote-nohotel') {
            let noHotelWarning = 'The accommodation you have selected is no longer available, ';
            noHotelWarning += 'please try searching with alternative dates.';
            this.props.actions.addAlert('Quote_NoHotel',
                AlertConstants.ALERT_TYPES.WARNING,
                noHotelWarning);
        }
    }

    setSearchDetails(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value === undefined) {
            value = event.target.getAttribute('value');
        }
        if (field === 'ArrivalId' || field === 'DepartureId') {
            const typeAdapter
                = SearchConstants.ID_ADAPTERS.filter(ida => ida.min < value && ida.max > value)[0];
            const type = typeAdapter.type;
            if (field === 'ArrivalId' && this.props.search.searchDetails.ArrivalType !== type) {
                this.updateProperty('ArrivalType', type);
            }
            if (field === 'DepartureId' && this.props.search.searchDetails.DepartureType !== type) {
                this.updateProperty('DepartureType', type);
            }
            const idAdapter
                = SearchConstants.ID_ADAPTERS.filter(ida => ida.type
                    === type)[0];
            if (idAdapter) {
                value = idAdapter.inverseShift(value);
            }
        }
        if (value && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateProperty(field, value);
    }

    updateProperty(field, value) {
        if (field === 'SearchMode') {
            this.setState({ errors: {} });
            this.setPackage(value);
            if (value === 'Flight') {
                this.resetRooms();
            }
        }
        this.props.actions.searchUpdateValue(field, value);
    }

    handleSearch() {
        if (this.validateSearchDetails()) {
            let url;
            if (this.props.search.searchDetails.SearchURL !== '') {
                url = this.generateCustomSearchURL();
            } else {
                url = this.searchDetailsToUrl();
            if (window.location.host === 'localhost:64351') {
                url = `http://localhost:58633${url}`;
            }
            }
            window.location = url;
        }
    }

    resetRooms() {
        if (this.props.search.searchDetails.Rooms.length > 1) {
            const rooms = [
                {
                    Id: 1,
                    Adults: 2,
                    Children: 0,
                    Infants: 0,
                    ChildAges: [],
                },
            ];
            this.updateProperty('Rooms', rooms);
        }
    }

    setPackage(searchMode) {
        const searchDetails = this.props.search.searchDetails;
        if (searchMode !== 'FlightPlusHotel' && searchDetails.PackageSearch) {
            this.props.actions.searchUpdateValue('PackageSearch', false);
        } else {
            const siteConfig = this.props.site.SiteConfiguration;
            const packageSearch = siteConfig.SearchConfiguration.PackageSearch;
            if (packageSearch && !searchDetails.PackageSearch) {
                this.props.actions.searchUpdateValue('PackageSearch', true);
            }
        }
    }

    searchDetailsToUrl() {
        const searchDetails = this.props.search.searchDetails;
        let searchMode = searchDetails.SearchMode.toLowerCase();
        if (searchMode === 'flightplushotel' && searchDetails.PackageSearch) {
            searchMode = 'package';
        }

        let searchURL = `/booking/results/${searchMode}`;

        if (searchDetails.SearchMode === 'Flight'
                || searchDetails.SearchMode === 'FlightPlusHotel') {
            searchURL = `${searchURL}/${searchDetails.DepartureType}/${searchDetails.DepartureId}`;
        }

        let arrivalId = searchDetails.ArrivalId;
        const idAdapter
            = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === searchDetails.ArrivalType)[0];
        if (idAdapter) {
            arrivalId = idAdapter.inverseShift(arrivalId);
        }

        searchURL = `${searchURL}/${searchDetails.ArrivalType}/${arrivalId}`;

        const departureDate = moment(searchDetails.DepartureDate);
        searchURL = `${searchURL}/${departureDate.format('YYYY-MM-DD')}`;
        searchURL = `${searchURL}/${searchDetails.Duration}`;

        let adults = '';
        let children = '';
        let childAges = '';
        let infants = '';

        searchDetails.Rooms.forEach(room => {
            adults = adults !== '' ? `${adults}_${room.Adults}` : `${room.Adults}`;
            children = children !== '' ? `${children}_${room.Children}` : `${room.Children}`;
            infants = infants !== '' ? `${infants}_${room.Infants}` : `${room.Infants}`;

            let roomChildAges = '';
            room.ChildAges.forEach(childAge => {
                roomChildAges = roomChildAges !== ''
                    ? `${roomChildAges}-${childAge}` : `${childAge}`;
            });
            if (room.Id < searchDetails.Rooms.length) {
                roomChildAges += '_';
            }
            childAges += roomChildAges;
        });

        if (searchDetails.SearchMode !== 'Flight') {
            searchURL = `${searchURL}/${searchDetails.Rooms.length}`;
        }

        childAges = childAges ? childAges : '0';

        searchURL = `${searchURL}/${adults}/${children}/${infants}/${childAges}`;

        if (searchDetails.SearchMode === 'FlightPlusHotel'
            || searchDetails.SearchMode === 'Flight') {
            searchURL += searchURL.indexOf('?') > -1 ? '&' : '?';
            searchURL += `flightclassid=${searchDetails.FlightClassId}`;
            searchURL += `&direct=${searchDetails.Direct}`;
        }
        if (searchDetails.BrandOverride) {
            searchURL += `&bid=${searchDetails.BrandOverride}`;
        }

        return searchURL;
    }
    generateCustomSearchURL() {
        const searchDetails = this.props.search.searchDetails;
        const constants = 'view=summary&form=main&aff=&flex=yes';
        let iataCode;
        if (searchDetails.DepartureType === 'airport'
            && searchDetails.DepartureId > 0) {
            const departureAirport = this.getAirport(searchDetails.DepartureId);
            if (departureAirport) {
                iataCode = departureAirport.IATACode;
            }
        }
        let searchURL = searchDetails.SearchURL;
        let searchMode = 'j';
        if (searchDetails.SearchMode === 'Flight') {
            searchMode = 'z';
        }
        let guests = 0;
        let roomString = '';
        for (let i = 0; i < searchDetails.Rooms.length; i++) {
            const roomNo = i === 0 ? '' : `[${i + 1}]`;
            const adults = searchDetails.Rooms[i].Adults;
            const children = searchDetails.Rooms[i].Children;
            const infants = searchDetails.Rooms[i].Infants;
            guests += adults;
            roomString += `pax${roomNo}=${adults}`;
            guests += children;
            roomString += `&childs${roomNo}=${children}`;
            guests += infants;
            roomString += `&infants${roomNo}=${infants}`;
            for (let j = 0; j < searchDetails.Rooms[i].ChildAges.length; j++) {
                roomString += `&childage_${i + 1}_${j}=${searchDetails.Rooms[i].ChildAges[j]}`;
            }
        }
        const departureDate = new Date(searchDetails.DepartureDate);
        const monthNames = [
            'January', 'February', 'March',
            'April', 'May', 'June', 'July',
            'August', 'September', 'October',
            'November', 'December'];

        let fullDate = departureDate.getDate();
        fullDate += `+${monthNames[departureDate.getMonth()]}`;
        fullDate += `+${departureDate.getFullYear()}`;

        searchURL += `?${constants}`;
        searchURL += `&type=${searchMode}`;
        searchURL += `&originFlightHotel=${iataCode}&dest=`;
        searchURL += `&fulldate=${fullDate}`;
        searchURL += `&duration=${searchDetails.Duration}`;
        searchURL += `&guests=${guests}+Guests`;
        searchURL += `&${roomString}`;
        return searchURL;
    }

    updateCalendar(nextProps) {
        const s = nextProps.search.searchDetails;
        if (this.state.useDealFinder) {
            const item = nextProps.dealFinderFlightCache;
            if (!item || (!item.isFetching)) {
                const departureDate = s.DepartureDate ? s.DepartureDate : new Date().toDateString();
                this.props.actions.loadDealFinderFlightCacheRouteDates(s.DepartureId,
                    s.ArrivalType, s.ArrivalId, departureDate, false, !s.Duration);
            }
        } else {
            const key = `${s.ArrivalType}_${s.DepartureId}_${s.ArrivalId}`;
        const item = nextProps.flightCacheRouteDates.items[key];
        if (!item || (!item.isFetching && !item.isLoaded)) {
                this.props.actions.loadFlightCacheRouteDates(s.DepartureId,
                    s.ArrivalType, s.ArrivalId);
            }
        }
    }

    validateSearchDetails() {
        const contentModel = this.props.entity.model;
        let isValid = true;
        const searchDetails = this.props.search.searchDetails;
        const errors = {};

        if ((searchDetails.SearchMode === 'Flight'
                    || searchDetails.SearchMode === 'FlightPlusHotel')
                && !searchDetails.DepartureId) {
            isValid = false;
            errors.DepartureId = contentModel.Warnings.Departure;
        }

        if (searchDetails.SearchURL === ''
            && (!searchDetails.ArrivalId
            || (searchDetails.SearchMode === 'Hotel'
                && searchDetails.ArrivalType === 'airport'))) {
            isValid = false;
            errors.ArrivalId = contentModel.Warnings.Arrival;
        }

        if (!searchDetails.Duration) {
            isValid = false;
            errors.Duration = contentModel.Warnings.Duration;
        }

        this.setState({ errors });
        return isValid;
    }
    validateFlightCacheResults(nextProps) {
        const flightCache = nextProps.dealFinderFlightCache;
        const searchDetails = nextProps.search.searchDetails;

        if (flightCache.isInitialised
            && !flightCache.isFetching
            && searchDetails.DepartureId
            && searchDetails.ArrivalId
            && searchDetails.SearchMode !== 'Hotel') {
            const errors = {};
            const departureDates = flightCache.departureDates;
            if (!departureDates || departureDates.length === 0) {
                const noFlights = 'Sorry! There are no flights available for this route';
                errors.DepartureId = noFlights;
            }
            this.setState({ errors });
        }
    }
    changeDate(dateString) {
        if (this.state.useDealFinder) {
            this.updateProperty('DepartureDate', dateString);
            const dates = this.props.dealFinderFlightCache.departureDates;
            const date = new Date(dateString).toDateString();
            for (let i = 0; i < dates.length; i++) {
                const departure = new Date(dates[i].DepartureDate).toDateString();
                if (departure === date
                    && !dates[i].Durations.includes(this.props.search.searchDetails.Duration)) {
                    const seven = 7;
                    const fourteen = 14;
                    let val = 0;
                    if (dates[i].Durations.includes(seven)) {
                        val = seven;
                    } else if (dates[i].Durations.includes(fourteen)) {
                        val = fourteen;
                    } else {
                        val = dates[i].Durations[0];
                    }
                    this.updateProperty('Duration', val);
                    break;
                }
            }
        }
    }
    getDurations() {
        if (this.state.useDealFinder && this.props.search.searchDetails.SearchMode !== 'Hotel') {
            const dates = this.props.dealFinderFlightCache.departureDates;
            const details = this.props.search.searchDetails;
            const searchDate = new Date(details.DepartureDate).toDateString();
            for (let i = 0; i < dates.length; i++) {
                const date = new Date(dates[i].DepartureDate).toDateString();
                if (date === searchDate) {
                    return dates[i].Durations;
                }
            }
        } else {
            const durations = [];
            const thirty = 30;
            for (let i = 1; i <= thirty; i++) {
                durations.push(i);
            }
            return durations;
        }
        return [];
    }
    getHighlightDates() {
        let highlightDates = [];
        const search = this.props.search.searchDetails;
        if (this.state.useDealFinder) {
            const dealFinderDates = this.props.dealFinderFlightCache;
            if (dealFinderDates.isInitialised && dealFinderDates.isLoaded) {
                highlightDates = dealFinderDates.departureDates;
            }
        } else {
        const key = `${search.ArrivalType}_${search.DepartureId}_${search.ArrivalId}`;
        const flightCacheRouteDates = this.props.flightCacheRouteDates.items[key];
        if (flightCacheRouteDates && flightCacheRouteDates.isLoaded) {
            highlightDates = flightCacheRouteDates.departureDates;
        }
        }
        return highlightDates;
    }
    getAirport(airportId) {
        const airports = this.props.airports.items ? this.props.airports.items : [];
        let selectedAirport = {};
        for (let i = 0; i < airports.length; i++) {
            if (airports[i].Id === airportId) {
                selectedAirport = airports[i];
                break;
            }
        }
        return selectedAirport;
    }
    searchToolProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        let airports = this.props.airports.items ? this.props.airports.items : [];
        const airportResortGroups = this.props.airportResortGroups.items
            ? this.props.airportResortGroups.items : [];
        const airportGroups = this.props.airportGroups.items
                                && this.props.airportGroups.items.AirportGroups
                                    ? this.props.airportGroups.items.AirportGroups : {};
        if (configuration.DepartureSearchOptions.ShowDepartureAirportGroups
            && this.props.airportGroups.items && this.props.airportGroups.items.Airports
            && this.props.airportGroups.items.Airports.length > 0) {
            airports = this.props.airportGroups.items.Airports;
        }
        const countries = this.props.countries.items ? this.props.countries.items : [];
        if (this.props.search.searchDetails.ArrivalId === null
            && this.props.page.EntityInformations
            && this.props.page.EntityInformations.some(i => i.Name === 'Resort')) {
            const resortId
                = this.props.page.EntityInformations.filter(i => i.Name === 'Resort')[0].Id;
            if (countries.some(
                i => i.Regions.some(
                    r => r.Resorts.some(
                        o => o.Id === resortId)))) {
                this.props.search.searchDetails.ArrivalId = resortId;
                this.props.search.searchDetails.ArrivalType = 'resort';
            }
        }
        const details = this.props.search.searchDetails;
        const durations = this.getDurations();
        let highlightDates = [];
        let excludedDates = [];
        if (this.state.useDealFinder && details.SearchMode === 'Hotel') {
            const date = new Date();
            const endDate = new Date();
            const sixteen = 16;
            endDate.setMonth(endDate.getMonth() + sixteen);
            endDate.setDate(1);
            while (date < endDate) {
                highlightDates.push({
                    DepartureDate: new Date(date),
                    OwnStock: false,
                });
                date.setDate(date.getDate() + 1);
            }
        } else if (this.state.useDealFinder) {
            highlightDates = this.props.dealFinderFlightCache.departureDates;
            excludedDates = this.props.dealFinderFlightCache.emptyDates;
        }
        const destinations = configuration.Destinations
            ? configuration.Destinations : {};
        const packageDestinations = destinations.PackageDestinations
            ? destinations.PackageDestinations : [];
        const flightDestinations = destinations.FlightDestinations
            ? destinations.FlightDestinations : [];
        const moreDestinations = destinations.MoreDestinations
            ? destinations.MoreDestinations : [];
        const searchToolProps = {
            searchDetails: this.props.search.searchDetails,
            searchModes: this.props.site.SiteConfiguration.SearchConfiguration.SearchModes,
            errors: this.state.errors,
            airports,
            airportGroups,
            airportResortGroups,
            flightClasses: this.props.flightClasses.items ? this.props.flightClasses.items : [],
            countries,
            routes: Array.isArray(this.props.routes) ? this.props.routes : [],
            minDuration: 1,
            maxDuration: 30,
            onChange: this.setSearchDetails,
            onChangeDate: this.changeDate,
            updatePropertyFunction: this.updateProperty,
            onSearch: this.handleSearch,
            isSearching: this.props.search.isSearching,
            startCollapsed: contentModel.Configuration.StartCollapsed,
            durationDisabled: this.state.useDealFinder
                && (details.SearchMode !== 'Hotel' || details.ArrivalType === 'airport')
                && (!this.props.search.searchDetails.ArrivalId
                    || !this.props.search.searchDetails.DepartureId
                    || (details.SearchMode !== 'Flight') !== (details.ArrivalType !== 'airport')),
            durations,
            searchDisabled: this.state.useDealFinder
                && (details.SearchMode !== 'Hotel' || details.ArrivalType === 'airport')
                && (!this.props.search.searchDetails.ArrivalId
                    || !this.props.search.searchDetails.DepartureId
                    || !this.props.search.searchDetails.DepartureDate
                    || this.props.dealFinderFlightCache.departureDates.length === 0
                    || !this.props.search.searchDetails.Duration
                    || durations.length === 0
                    || (details.SearchMode !== 'Flight') !== (details.ArrivalType !== 'airport')),
            placeholders: {
                departure: contentModel.Placeholders.Departure,
                arrivalSelect: contentModel.Placeholders.ArrivalSelect,
                arrivalAutoSuggest: contentModel.Placeholders.ArrivalAutoSuggest,
                arrivalAirport: contentModel.Placeholders.ArrivalAirport,
                duration: contentModel.Placeholders.Duration,
                property: contentModel.Placeholders.Property,
            },
            buttons: {
                search: contentModel.Buttons.Search,
                searchAgain: contentModel.Buttons.SearchAgain,
                clearSearch: contentModel.Buttons.ClearSearch,
            },
            departureSearchOptions: {
                showDepartureAirportGroups:
                    configuration.DepartureSearchOptions.ShowDepartureAirportGroups,
                groupByAirportGroups: configuration.DepartureSearchOptions.GroupByAirportGroups,
                showPreferredAirportsFirst:
                    configuration.DepartureSearchOptions.ShowPreferredAirportsFirst,
            },
            searchModeOptions: {
                tabPackage: configuration.SearchModeOptions.Package,
                tabHotel: configuration.SearchModeOptions.Hotel,
                tabFlight: configuration.SearchModeOptions.Flight,
            },
            destinationSearchOptions: {
                displayAutoComplete: configuration.DestinationSearchOptions.DisplayAutoComplete,
                displayDropdown: configuration.DestinationSearchOptions.DisplayDropdown,
                displayAirportGroups: configuration.DestinationSearchOptions.DisplayAirportGroups,
                airportGroupsFormat: configuration.DestinationSearchOptions.AirportGroupsFormat,
                filterByAirportRoute: configuration.DestinationSearchOptions.FilterByAirportRoute,
            },
            additionalSearchOptions: {
                displayText: configuration.AdditionalSearchOptions.DisplayText,
                hideText: configuration.AdditionalSearchOptions.HideText,
                property: configuration.AdditionalSearchOptions.Property,
                mealBasis: configuration.AdditionalSearchOptions.MealBasis,
                minRating: configuration.AdditionalSearchOptions.MinRating,
                flightClass: configuration.AdditionalSearchOptions.FlightClass,
            },
            calendarOptions: {
                highlightDates,
                excludedDates,
                displayKey: configuration.CalendarOptions
                    ? configuration.CalendarOptions.HighlightFlightCacheDates
                    : false,
                highlightKeyText: configuration.CalendarOptions
                    ? configuration.CalendarOptions.HighlightKeyText
                    : '',
                defaultKeyText: configuration.CalendarOptions
                    ? configuration.CalendarOptions.DefaultKeyText : '',
                disabled: this.state.useDealFinder
                    && (details.SearchMode !== 'Hotel' || details.ArrivalType === 'airport')
                    && (!details.ArrivalId
                        || !details.DepartureId
                        || !details.DepartureDate
                    || this.props.dealFinderFlightCache.departureDates.length === 0
                        || (details.SearchMode !== 'Flight')
                        !== (details.ArrivalType !== 'airport')),
                loading: this.state.useDealFinder
                    && this.props.dealFinderFlightCache.isFetching
                    && details.SearchMode !== 'Hotel',
                useDealFinder: this.state.useDealFinder,
            },
            childAgeText: contentModel.Information.ChildAgeText,
            packageDestinations,
            flightDestinations,
            moreDestinations,
            getAirport: this.getAirport,
            calendarMessages: this.props.calendarMessages,
            setUseDealFinder: this.setUseDealFinder,
        };
        return searchToolProps;
    }
    setUseDealFinder(useDealFinder, after) {
        this.setState({ useDealFinder }, after);
    }
    render() {
        let SearchToolComponent = SearchTool;
        if (this.props.customerComponents.hasOwnProperty('SearchTool')) {
            SearchToolComponent = this.props.customerComponents.SearchTool;
        }
        return (
            <div>
                {this.isInitialised()
                    && <SearchToolComponent {...this.searchToolProps()} />}
            </div>
        );
    }
}
SearchToolContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    airportResortGroups: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    airportGroups: React.PropTypes.object.isRequired,
    countries: React.PropTypes.object.isRequired,
    routes: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.object.isRequired,
    flightCacheRouteDates: React.PropTypes.object,
    page: React.PropTypes.object.isRequired,
    dealFinderFlightCache: React.PropTypes.object,
    customerComponents: React.PropTypes.object,
    calendarMessages: React.PropTypes.array.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    const countries = state.countries ? state.countries : {};
    const airports = state.airports ? state.airports : {};
    const flightClasses = state.flightClasses ? state.flightClasses : {};
    const airportResortGroups = state.airportResortGroups ? state.airportResortGroups : {};
    const airportGroups = state.airportGroups ? state.airportGroups : {};

    let routes = [];
    if (state.routes && Array.isArray(state.routes)) {
        routes = state.routes;
    }
    const flightCacheRouteDates = state.flightCacheRouteDates ? state.flightCacheRouteDates : {};
    const dealFinderFlightCache = state.dealFinderFlightCache ? state.dealFinderFlightCache : {};
    const calendarMessages
        = state.models
            && state.models.Calendar
            && state.models.Calendar.isLoaded
            ? state.models.Calendar.models
            : [];
    return {
        search,
        airportResortGroups,
        airportGroups,
        airports,
        calendarMessages,
        countries,
        flightClasses,
        dealFinderFlightCache,
        routes,
        flightCacheRouteDates,
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
        AirportGroupActions,
        AirportResortGroupActions,
        AlertActions,
        BrandActions,
        CountryActions,
        FlightClassActions,
        DFFlightCacheActions,
        EntityActions,
        FlightCacheRouteActions,
        RouteAvailabilityActions,
        SearchActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SearchToolContainer);
