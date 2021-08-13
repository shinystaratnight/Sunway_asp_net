import '../../../styles/widgets/bookingjourney/_searchTool.scss';

import * as SearchConstants from 'constants/search';

import ArrayFunctions from 'library/arrayfunctions';
import AutoCompleteInput from 'components/form/autocompleteinput';
import DateInput from 'components/form/dateinput';
import GuestsInput from 'components/form/guestsinput';
import React from 'react';
import SelectInput from 'components/form/selectinput';
import Tabs from 'components/common/tabs';
import moment from 'moment';

export default class SearchTool extends React.Component {
    constructor(props) {
        super(props);
        this.searchModes = {
            FLIGHT_PLUS_HOTEL: 'FlightPlusHotel',
            HOTEL: 'Hotel',
            FLIGHT: 'Flight',
        };
        this.destinationModes = {
            DROPDOWN: 'Dropdown',
            AUTOSUGGEST: 'AutoSuggest',
        };
        this.inputSearchModes = {
            DepartureId: ['FlightPlusHotel', 'Flight'],
        };
        this.state = {
            errors: {},
            valid: false,
            collapsed: this.props.startCollapsed,
            displayAdditionalSearch: false,
            collapseOnMobile: this.props.collapseOnMobile,
            mobileCollapsed: this.props.collapseOnMobile,
        };

        this.state.destinationMode = this.props.destinationSearchOptions.displayAutoComplete
            ? this.destinationModes.AUTOSUGGEST
            : this.destinationModes.DROPDOWN;

        this.isValidRegion = this.isValidRegion.bind(this);
        this.isValidAirportRegion = this.isValidAirportRegion.bind(this);
        this.isDepartureAirport = this.isDepartureAirport.bind(this);
        this.isDepartureAirportGroup = this.isDepartureAirportGroup.bind(this);
        this.updateProperty = this.updateProperty.bind(this);
        this.setSearchDate = this.setSearchDate.bind(this);
        this.setRooms = this.setRooms.bind(this);
        this.setDestinationMode = this.setDestinationMode.bind(this);
        this.handleDepartureChange = this.handleDepartureChange.bind(this);
        this.handleSearchAgain = this.handleSearchAgain.bind(this);
        this.handleToggleSearchTool = this.handleToggleSearchTool.bind(this);
        this.handleResortChange = this.handleResortChange.bind(this);
        this.handleRegionResortChange = this.handleRegionResortChange.bind(this);
        this.toggleAdditionalSearch = this.toggleAdditionalSearch.bind(this);
    }
    displayAutoSuggest() {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        const displayAutoSuggest = this.props.destinationSearchOptions.displayAutoComplete
            && this.state.destinationMode === this.destinationModes.AUTOSUGGEST
            && currentSearchMode !== this.searchModes.FLIGHT;
        return displayAutoSuggest;
    }
    displayDropdown() {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        const displayDropdown = (this.props.destinationSearchOptions.displayDropdown
            && this.state.destinationMode === this.destinationModes.DROPDOWN)
            || currentSearchMode === this.searchModes.FLIGHT;
        return displayDropdown;
    }
    displayToggle() {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        const displayToggle = this.props.destinationSearchOptions.displayAutoComplete
            && this.props.destinationSearchOptions.displayDropdown
            && currentSearchMode !== this.searchModes.FLIGHT;
        return displayToggle;
    }
    displayResortDropdown() {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        const arrivalId = this.props.searchDetails.ArrivalId;
        const arrivalType = this.props.searchDetails.ArrivalType;
        const displayResortDropdown = this.displayDropdown()
            && currentSearchMode !== this.searchModes.FLIGHT
            && (arrivalType === 'resort'
                || (arrivalType === 'region' && arrivalId > 0));
        return displayResortDropdown;
    }
    getTabProps() {
        const tabProps = {
            selectedTab: this.props.searchDetails.SearchMode,
            tabs: [],
        };

        if (this.props.searchModes.indexOf(this.searchModes.FLIGHT_PLUS_HOTEL) !== -1) {
            tabProps.tabs.push(
                {
                    name: this.props.searchModeOptions.tabPackage !== ''
                        ? this.props.searchModeOptions.tabPackage : 'Package',
                    value: this.searchModes.FLIGHT_PLUS_HOTEL,
                    onClick: () =>
                        this.updateProperty('SearchMode', this.searchModes.FLIGHT_PLUS_HOTEL),
                });
        }

        if (this.props.searchModes.indexOf(this.searchModes.HOTEL) !== -1) {
            tabProps.tabs.push(
                {
                    name: this.props.searchModeOptions.tabHotel !== ''
                        ? this.props.searchModeOptions.tabHotel : 'Hotel',
                    value: this.searchModes.HOTEL,
                    onClick: () => this.updateProperty('SearchMode', this.searchModes.HOTEL),
                });
        }

        if (this.props.searchModes.indexOf(this.searchModes.FLIGHT) !== -1) {
            tabProps.tabs.push(
                {
                    name: this.props.searchModeOptions.tabFlight !== ''
                        ? this.props.searchModeOptions.tabFlight : 'Flight',
                    value: this.searchModes.FLIGHT,
                    onClick: () => this.updateProperty('SearchMode', this.searchModes.FLIGHT),
                });
        }

        return tabProps;
    }
    getDepartureAirportRoutes(airportId) {
        let departureAirportRoutes = {
            ArrivalRegions: [],
        };
        for (let i = 0; i < this.props.routes.length; i++) {
            const airportRoutes = this.props.routes[i];
            if (airportRoutes.DepartureAirportId === airportId) {
                departureAirportRoutes = airportRoutes;
                break;
            }
        }
        return departureAirportRoutes;
    }
    getDepartureAirportGroupRoutes(airportGroupId) {
        let departureAirportRoutes = {
            ArrivalRegions: [],
        };
        for (let i = 0; i < this.props.routes.length; i++) {
            const airportRoutes = this.props.routes[i];
            if (airportRoutes.DepartureAirportGroupId === airportGroupId) {
                departureAirportRoutes = airportRoutes;
                break;
            }
        }
        return departureAirportRoutes;
    }
    getRegion(regionId) {
        let region = {};
        for (let i = 0; i < this.props.countries.length; i++) {
            const country = this.props.countries[i];
            for (let j = 0; j < country.Regions.length; j++) {
                const currentRegion = country.Regions[j];
                if (currentRegion.Id === regionId) {
                    region = currentRegion;
                    break;
                }
            }
            if (region.Id) {
                break;
            }
        }
        return region;
    }
    getRegionFromResort(resortId) {
        const parsedResortId = parseInt(resortId, 10);
        let region = {};
        for (let i = 0; i < this.props.countries.length; i++) {
            const country = this.props.countries[i];
            for (let j = 0; j < country.Regions.length; j++) {
                const currentRegion = country.Regions[j];
                const resortIndex = currentRegion.Resorts.findIndex(resort =>
                    resort.Id === parsedResortId);
                if (resortIndex > -1) {
                    region = currentRegion;
                    break;
                }
            }
            if (region.Id) {
                break;
            }
        }
        return region;
    }
    isDepartureAirport(airport) {
        let isDepartureAirport = false;
        if (airport.hasOwnProperty('Type')) {
            isDepartureAirport = airport.Type.toLowerCase().indexOf('departure') > -1;
        }
        if (isDepartureAirport) {
            const airportRoutes = this.getDepartureAirportRoutes(airport.Id);
            isDepartureAirport = airportRoutes.hasOwnProperty('ArrivalRegions')
                && airportRoutes.ArrivalRegions.length > 0;
        }
        return isDepartureAirport;
    }
    isDepartureAirportGroup(airportGroup) {
        let isDepartureAirportGroup = false;
        if (airportGroup.hasOwnProperty('Type')) {
            isDepartureAirportGroup = airportGroup.Type.toLowerCase().indexOf('departure') > -1;
        }
        if (isDepartureAirportGroup) {
            const airportRoutes = this.getDepartureAirportGroupRoutes(airportGroup.Id);
            isDepartureAirportGroup = airportRoutes.hasOwnProperty('ArrivalRegions')
                && airportRoutes.ArrivalRegions.length > 0;
        }
        return isDepartureAirportGroup;
    }
    isArrivalAirport(airport) {
        let isArrivalAirport = false;
        if (airport.hasOwnProperty('Type')) {
            isArrivalAirport = airport.Type.toLowerCase().indexOf('arrival') > -1;
        }
        return isArrivalAirport;
    }
    isValidAirportRegion(airportId, regionId) {
        const airportRoutes = this.getDepartureAirportRoutes(airportId);
        let isValidAirportRegion = false;
        for (let i = 0; i < airportRoutes.ArrivalRegions.length; i++) {
            const arrivalRegion = airportRoutes.ArrivalRegions[i];
            if (arrivalRegion.RegionId === regionId) {
                isValidAirportRegion = true;
                break;
            }
        }
        return isValidAirportRegion;
    }
    isValidRegion(region) {
        let validRegion = false;
        if (this.props.searchDetails.SearchMode === this.searchModes.HOTEL
            || !this.props.destinationSearchOptions.filterByAirportRoute) {
            validRegion = true;
        } else {
            validRegion
                = this.isValidAirportRegion(this.props.searchDetails.DepartureId, region.Id);
        }
        return validRegion;
    }
    handleDepartureChange(event) {
        const value = event.target.value;
        const typeAdapter
            = SearchConstants.ID_ADAPTERS.filter(ida => ida.min < value && ida.max > value)[0];
        if (typeAdapter && typeAdapter.type) {
            this.props.updatePropertyFunction('DepartureType', typeAdapter.type);
        } else if (this.props.searchDetails.DepartureType !== 'airport') {
            this.props.updatePropertyFunction('DepartureType', 'airport');
        }
        this.props.onChange(event);
        if (this.props.searchDetails.ArrivalId) {
            this.props.updatePropertyFunction('ArrivalType', 'region');
            this.props.updatePropertyFunction('ArrivalId', 0);
        }
    }
    getDepartureAirportProps() {
        const searchDetails = this.props.searchDetails;
        let value = searchDetails.DepartureId;
        if (searchDetails.DepartureType !== '') {
            const idAdapter
                = SearchConstants.ID_ADAPTERS.filter(ida =>
                    ida.type === searchDetails.DepartureType)[0];
            value = idAdapter.shift(searchDetails.DepartureId);
        }
        const departureAirportProps = {
            name: 'DepartureId',
            options: [],
            placeholder: this.props.placeholders.departure,
            selectClass: 'departure-airport',
            onChange: this.handleDepartureChange,
            value,
            error: this.props.errors.DepartureId,
        };
        if (this.props.departureSearchOptions.showDepartureAirportGroups
            && this.props.airportGroups.length > 0) {
            const groupIdAdapter
                = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === 'airportgroup')[0];
            this.props.airportGroups.filter(this.isDepartureAirportGroup).forEach((aGroup) => {
                const airportGroupOption = {
                    Id: groupIdAdapter.shift(aGroup.Id),
                    Name: `${aGroup.Name}`,
                };
                departureAirportProps.options.push(airportGroupOption);
                if (aGroup.Airports && aGroup.Airports.length > 0) {
                    const airportIdAdapter
                        = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === 'airport')[0];
                    aGroup.Airports.filter(this.isDepartureAirport).forEach((airport) => {
                        const airportOption = {
                            Id: airportIdAdapter.shift(airport.Id),
                            Name: `${airport.Name} (${airport.IATACode})`,
                        };
                        departureAirportProps.options.push(airportOption);
                    });
                }
            });
        }
        const airports = [];
        if (this.props.airports.length > 0) {
            const airportIdAdapter
                = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === 'airport')[0];
            this.props.airports.filter(this.isDepartureAirport).forEach((airport) => {
                const airportOption = {
                    Id: airportIdAdapter.shift(airport.Id),
                    Name: `${airport.Name} (${airport.IATACode})`,
                    PreferredAirport: airport.PreferredAirport,
                };
                airports.push(airportOption);
            });
        }
        if (!this.props.departureSearchOptions.showPreferredAirportsFirst) {
            airports.sort(this.sortByName);
        } else {
            airports.sort(this.sortByPreferred);
        }
        departureAirportProps.options = departureAirportProps.options.concat(airports);
        return departureAirportProps;
    }
    handleResortChange(event) {
        let value = event.target.value;
        if (value && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        if (value > 0) {
            if (this.props.searchDetails.ArrivalType !== 'resort') {
                this.props.updatePropertyFunction('ArrivalType', 'resort');
            }
            this.props.updatePropertyFunction('ArrivalId', value);
        } else if (value < 0) {
            this.props.updatePropertyFunction('ArrivalType', 'region');
            this.props.updatePropertyFunction('ArrivalId', value * -1);
        }
    }
    handleRegionResortChange(key, value) {
        if (value < 0) {
            if (this.props.searchDetails.ArrivalType !== 'resort') {
                this.props.updatePropertyFunction('ArrivalType', 'resort');
            }
            this.props.updatePropertyFunction(key, value * -1);
        } else {
            if (this.props.searchDetails.ArrivalType !== 'region') {
                this.props.updatePropertyFunction('ArrivalType', 'region');
            }
            this.props.updatePropertyFunction(key, value);
        }
    }
    handleSearchAgain() {
        this.setState({ collapsed: false });
    }
    handleToggleSearchTool() {
        const mobileCollapsed = !this.state.mobileCollapsed;
        this.setState({ mobileCollapsed });
    }
    addAirportMixedAirportGroupsToDestination(oldDestination) {
        const destination = Object.assign({}, oldDestination);
        const options = destination.Options;
        let airportGroups = this.props.airportResortGroups.filter(arg =>
            arg.AssociatedGeographyLevel1Id === destination.Id && arg.DisplayOnSearch
            && arg.Resorts.length > 0);
        airportGroups = ArrayFunctions.sortByPropertyAscending(airportGroups, 'AirportGroupName');
        const airportGroupOptions = [];
        const idAdapter
            = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === 'airportgroup')[0];
        airportGroups.forEach(ag => {
            const ariportGroupOption = {
                Name: ag.AirportGroupName,
                Id: idAdapter.shift(ag.AirportGroupId),
                Resorts: ag.Resorts,
                IsAirportGroup: true,
            };
            airportGroupOptions.push(ariportGroupOption);
        });
        destination.Options = airportGroupOptions.length > 0
            ? airportGroupOptions.concat(options) : options;
        return destination;
    }
    setupDestinationsProps() {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        let value = this.props.searchDetails.ArrivalId;
        const arrivalType = this.props.searchDetails.ArrivalType;
        if (arrivalType === 'resort') {
            const region = this.getRegionFromResort(value);
            value = region.Id;
        }

        const idAdapter = SearchConstants.ID_ADAPTERS.filter(ida => ida.type === arrivalType)[0];
        if (typeof idAdapter !== 'undefined' && idAdapter !== null
            && (value <= idAdapter.min || idAdapter.max <= value)) {
            value = idAdapter.shift(value);
        }

        const destinationProps = {
            name: 'ArrivalId',
            options: [],
            selectClass: 'destination',
            onChange: this.props.onChange,
            value,
            error: this.props.errors.ArrivalId,
        };

        if (currentSearchMode === this.searchModes.FLIGHT) {
            destinationProps.placeholder = this.props.placeholders.arrivalAirport;
            if (this.props.airports.length > 0) {
                this.props.airports.filter(this.isArrivalAirport).forEach((airport) => {
                    if (airport.BrandValidAirport) {
                        const airportOption = {
                            Id: airport.Id + SearchConstants.flightIDModifier,
                            Name: `${airport.Name} (${airport.IATACode})`,
                        };
                        destinationProps.options.push(airportOption);
                    }
                });
            }
        } else {
            destinationProps.placeholder = this.props.placeholders.arrivalSelect;
            destinationProps.renderOptionGroups = true;
            const countries = this.props.countries.sort(this.sortByName);
            const addMixedAirportGroupsToDestination
                = this.props.destinationSearchOptions.airportGroupsFormat === 'Mixed'
                && this.props.destinationSearchOptions.displayAirportGroups;
            countries.forEach(country => {
                let destination = {
                    Id: country.Id,
                    Name: country.Name,
                    Options: country.Regions.filter(this.isValidRegion).sort(this.sortByName),
                };
                destination = addMixedAirportGroupsToDestination
                    ? this.addAirportMixedAirportGroupsToDestination(destination) : destination;
                if (destination.Options.length > 0) {
                    destinationProps.options.push(destination);
                }
            });
        }
        destinationProps.options.sort(this.sortByName);
        return destinationProps;
    }
    getResortDestinationProps() {
        let regionId = this.props.searchDetails.ArrivalId;
        if (this.props.searchDetails.ArrivalType === 'resort') {
            const region = this.getRegionFromResort(this.props.searchDetails.ArrivalId);
            regionId = region.Id;
        }

        const region = this.getRegion(regionId);
        let options = [
            {
                Id: region.Id * -1,
                Name: 'Any Resort',
            },
        ];
        options = options.concat(region.Resorts);

        let value = 0;
        if (this.props.searchDetails.ArrivalType === 'resort') {
            value = this.props.searchDetails.ArrivalId;
        }
        const destinationProps = {
            name: 'ArrivalId',
            options,
            selectClass: 'destination',
            onChange: this.handleResortChange,
            value,
            error: this.props.errors.ArrivalId,
        };
        return destinationProps;
    }
    getFlightClassProps() {
        let options = [
            {
                Id: 0,
                Name: 'Any Cabin Class',
            },
        ];
        options = options.concat(this.props.flightClasses);

        const flightClassProps = {
            name: 'FlightClassId',
            options,
            onChange: this.props.onChange,
            value: this.props.searchDetails.FlightClassId,
        };
        return flightClassProps;
    }
    setupDateInputProps() {
        const maxDate = new Date();
        const monthsAhead = 16;
        maxDate.setMonth(maxDate.getMonth() + monthsAhead);
        maxDate.setDate(0);
        let defaultDate = this.props.searchDetails.DepartureDate;

        if (typeof this.props.searchDetails.DepartureDate === 'string') {
            const departureDate = moment(this.props.searchDetails.DepartureDate);
            defaultDate = new Date(departureDate.format('YYYY-MM-DD'));
        }
        const dateInputProps = {
            name: 'departuredate',
            onChange: this.props.onChangeDate,
            defaultDate,
            maxDate,
            highlightDates: this.props.calendarOptions
                ? this.props.calendarOptions.highlightDates : [],
            excludedDates: this.props.calendarOptions
                ? this.props.calendarOptions.excludedDates : [],
            disabled: this.props.calendarOptions.disabled,
            calendarOpen: true,
            loading: this.props.calendarOptions.loading,
            useDealFinder: this.props.calendarOptions.useDealFinder,
        };
        return dateInputProps;
    }
    sortByName(a, b) {
        const nameA = a.Name.toLowerCase();
        const nameB = b.Name.toLowerCase();
        if (nameA < nameB) {
            return -1;
        }
        if (nameA > nameB) {
            return 1;
        }
        return 0;
    }
    sortByPreferred(a, b) {
        const prefA = a.PreferredAirport;
        const prefB = b.PreferredAirport;
        if (!prefA && prefB) {
            return 1;
        }
        if (prefA && !prefB) {
            return -1;
        }
        return 0;
    }
    updateProperty(field, value) {
        this.props.updatePropertyFunction(field, value);
    }
    displayInput(input) {
        const currentSearchMode = this.props.searchDetails.SearchMode;
        const validSearchModes = this.inputSearchModes[input];

        const displayInput = validSearchModes.indexOf(currentSearchMode) !== -1;
        return displayInput;
    }
    setSearchDate(dateString) {
        this.props.updatePropertyFunction('DepartureDate', dateString);
    }
    setRooms(rooms) {
        this.props.updatePropertyFunction('Rooms', rooms);
    }
    setDestinationMode(mode) {
        this.setState({ destinationMode: mode });
    }
    toggleAdditionalSearch() {
        const displayAdditionalSearch = !this.state.displayAdditionalSearch;
        this.setState({ displayAdditionalSearch });
    }
    renderAdditionalSearchToggle() {
        const toggleProps = {
            className: 'additional-search-toggle',
            onClick: this.toggleAdditionalSearch,
        };
        toggleProps.className += this.state.displayAdditionalSearch ? ' displayed' : '';

        const toggleText = this.state.displayAdditionalSearch
            ? this.props.additionalSearchOptions.hideText
            : this.props.additionalSearchOptions.displayText;

        return (
            <span {...toggleProps}>{toggleText}</span>
        );
    }
    renderDepartureOption() {
        return (
            <div className="search-option search-departure">
                <SelectInput {...this.getDepartureAirportProps()} />

                {this.props.additionalSearchOptions.flightClass
                    && this.state.displayAdditionalSearch
                    && <SelectInput {...this.getFlightClassProps()} />}

                {this.props.additionalSearchOptions.flightClass
                    && this.renderAdditionalSearchToggle()}
            </div>
        );
    }
    renderArrivalOption() {
        const destinationProps = this.setupDestinationsProps();
        const arrivalId = this.props.searchDetails.ArrivalId;
        const arrivalType = this.props.searchDetails.ArrivalType;

        let autoCompleteValue = arrivalId;
        if (arrivalType === 'resort') {
            autoCompleteValue = arrivalId * -1;
        }

        const regionAutoComplete = {
            name: 'ArrivalId',
            lookup: 'Geography/RegionResort',
            onChange: this.handleRegionResortChange,
            placeholder: this.props.placeholders.arrivalAutoSuggest,
            value: autoCompleteValue,
            error: this.props.errors.ArrivalId,
            containerClass: 'search-arrival-input',
        };

        const propertyAutoComplete = {
            name: 'PropertyReferenceId',
            lookup: 'Property/PropertyReference',
            onChange: this.updateProperty,
            placeholder: this.props.placeholders.property,
            value: this.props.searchDetails.PropertyReferenceId,
            containerClass: 'mt-3',
        };

        const dropdownToggleProps = {
            className: 'link',
            onClick: () => this.setDestinationMode(this.destinationModes.DROPDOWN),
        };

        const autoSuggestToggleProps = {
            className: 'link',
            onClick: () => this.setDestinationMode(this.destinationModes.AUTOSUGGEST),
        };

        const currentSearchMode = this.props.searchDetails.SearchMode;
        return (
            <div className="search-option search-arrival">
                {this.displayAutoSuggest()
                    && <AutoCompleteInput {...regionAutoComplete} />}
                {this.displayDropdown()
                    && <SelectInput {...destinationProps} />}
                {this.displayResortDropdown()
                    && <SelectInput {...this.getResortDestinationProps()} />}

                {this.displayAutoSuggest() && this.displayToggle()
                    && <span {...dropdownToggleProps}>Or select destination from a list</span>}
                {this.displayDropdown() && this.displayToggle()
                    && <span {...autoSuggestToggleProps}>Or type in using auto-suggest</span>}

                {this.props.additionalSearchOptions.property
                    && currentSearchMode !== this.searchModes.FLIGHT
                    && <AutoCompleteInput {...propertyAutoComplete} />}
            </div>
        );
    }
    renderDateInput() {
        const dateInputProps = this.setupDateInputProps();
        if (this.props.calendarOptions.displayKey) {
            return (
                <DateInput {...dateInputProps}>{this.renderDateInputKey()}</DateInput>
            );
        }
        return (
            <DateInput {...dateInputProps} />
        );
    }
    renderDateInputKey() {
        return (
            <div className="calendar-key">
                <span className="calendar-key-item highlighted">
                    {this.props.calendarOptions.highlightKeyText}
                </span>
                <span className="calendar-key-item default">
                    {this.props.calendarOptions.defaultKeyText}
                </span>
            </div>
        );
    }
    renderTabs() {
        const tabContainerClass = this.props.startCollapsed && this.props.sidebarDisplay
            ? '' : 'container';
        return (
            <div className={tabContainerClass}>
                <Tabs {...this.getTabProps()} />
            </div>
        );
    }
    renderSearchForm() {
        const options = [];
        const priorityOptions = [];
        const seven = 7;
        const fourteen = 14;
        for (let i = 0; i <= this.props.durations.length; i++) {
            if (this.props.durations[i] !== seven && this.props.durations[i] !== fourteen) {
                options.push(this.props.durations[i]);
            } else {
                priorityOptions.push(this.props.durations[i]);
            }
        }
        const durationProps = {
            name: 'Duration',
            placeholder: this.props.placeholders.duration,
            options,
            priorityOptions,
            selectClass: 'duration',
            onChange: this.props.onChange,
            value: this.props.searchDetails.Duration,
            error: this.props.errors.Duration,
            disabled: this.props.durationDisabled || this.props.durations.length === 0,
        };
        const guestInputProps = {
            name: 'guests',
            guestText: this.props.searchDetails.SearchMode === this.searchModes.FLIGHT
                ? 'Passengers' : 'Guests',
            rooms: this.props.searchDetails.Rooms,
            onChange: this.setRooms,
            showRooms: this.props.searchDetails.SearchMode === this.searchModes.FLIGHT_PLUS_HOTEL
                || this.props.searchDetails.SearchMode === this.searchModes.HOTEL,
            childAgeText: this.props.childAgeText,
        };
        const buttonAttributes = {
            type: 'button',
            className: 'btn btn-primary btn-lg btn-block-sm',
            onClick: this.props.onSearch,
        };
        if (this.props.searchDisabled || this.props.isSearching) {
            buttonAttributes.disabled = true;
        }

        const searchFormContentClass = this.props.sidebarDisplay
            ? 'sidebar-container' : 'container';
        return (
            <div className="search-form">
                <div className={searchFormContentClass}>
                    {this.displayInput('DepartureId')
                        && this.renderDepartureOption()}
                    {this.renderArrivalOption()}
                    <div className="search-option search-departure-date">
                        {this.renderDateInput()}
                    </div>
                    <div className="search-option search-duration">
                        <SelectInput {...durationProps} />
                    </div>
                    <div className="search-option search-guests">
                        <GuestsInput {...guestInputProps} />
                    </div>
                    <button {...buttonAttributes}>{this.props.buttons.search}</button>
                </div>
            </div>
        );
    }
    renderCollapsedView() {
        const buttonAttributes = {
            type: 'button',
            className: 'btn btn-primary btn-lg btn-block-sm',
            onClick: this.handleSearchAgain,
        };

        let containerClass = 'search-again-container';
        containerClass += !this.props.sidebarDisplay ? ' container' : '';

        return (
            <div className={containerClass}>
                <div className="row">
                    <div className="col-xs-12">
                        <button {...buttonAttributes}>{this.props.buttons.searchAgain}</button>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        let containerClass = 'widget-search-tool';
        containerClass += this.props.startCollapsed ? ' collapsed-view' : '';
        containerClass += this.props.sidebarDisplay ? ' sidebar-view' : '';

        const buttonAttributes = {
            type: 'button',
            className: 'mobile-toggle btn-primary btn btn-lg btn-block-sm hidden-md-up',
            onClick: this.handleToggleSearchTool,
        };

        const mobileCollapsed = this.state.mobileCollapsed;
        let searchToolClass = 'search-tool-holder mt-3';
        if (mobileCollapsed) {
            searchToolClass += ' hidden-sm-down';
        }

        return (
            <div className={containerClass}>
                {this.state.collapseOnMobile
                  && <button {...buttonAttributes}>Edit Search</button>}
                <div className={searchToolClass}>
                {!this.state.collapsed
                    && this.renderTabs()}
                {!this.state.collapsed
                    && this.renderSearchForm()}
                {this.state.collapsed
                    && this.renderCollapsedView()}
            </div>
            </div>
        );
    }
}

SearchTool.propTypes = {
    searchDetails: React.PropTypes.object.isRequired,
    searchModes: React.PropTypes.array.isRequired,
    errors: React.PropTypes.object.isRequired,
    airports: React.PropTypes.array.isRequired,
    airportGroups: React.PropTypes.array.isRequired,
    airportResortGroups: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array,
    countries: React.PropTypes.array.isRequired,
    minDuration: React.PropTypes.number,
    maxDuration: React.PropTypes.number,
    routes: React.PropTypes.array.isRequired,
    onChange: React.PropTypes.func.isRequired,
    updatePropertyFunction: React.PropTypes.func.isRequired,
    onSearch: React.PropTypes.func.isRequired,
    isSearching: React.PropTypes.bool,
    startCollapsed: React.PropTypes.bool,
    collapseOnMobile: React.PropTypes.bool,
    sidebarDisplay: React.PropTypes.bool,
    durationDisabled: React.PropTypes.bool,
    durations: React.PropTypes.array,
    onChangeDate: React.PropTypes.func,
    searchDisabled: React.PropTypes.bool,
    placeholders: React.PropTypes.shape({
        departure: React.PropTypes.string,
        arrivalSelect: React.PropTypes.string,
        arrivalAutoSuggest: React.PropTypes.string,
        arrivalAirport: React.PropTypes.string,
        duration: React.PropTypes.string,
        property: React.PropTypes.string,
    }),
    buttons: React.PropTypes.shape({
        search: React.PropTypes.string,
        searchAgain: React.PropTypes.string,
        clearSearch: React.PropTypes.string,
    }),
    departureSearchOptions: React.PropTypes.shape({
        showDepartureAirportGroups: React.PropTypes.bool,
        groupByAirportGroups: React.PropTypes.bool,
        showPreferredAirportsFirst: React.PropTypes.bool,
    }),
    searchModeOptions: React.PropTypes.shape({
        tabPackage: React.PropTypes.string,
        tabHotel: React.PropTypes.string,
        tabFlight: React.PropTypes.string,
    }),
    destinationSearchOptions: React.PropTypes.shape({
        displayAutoComplete: React.PropTypes.bool,
        displayDropdown: React.PropTypes.bool,
        displayAirportGroups: React.PropTypes.bool,
        airportGroupsFormat: React.PropTypes.oneOf(['Top', 'Mixed']),
        filterByAirportRoute: React.PropTypes.bool,
    }),
    additionalSearchOptions: React.PropTypes.shape({
        displayText: React.PropTypes.string,
        hideText: React.PropTypes.string,
        property: React.PropTypes.bool,
        mealBasis: React.PropTypes.bool,
        minRating: React.PropTypes.bool,
        flightClass: React.PropTypes.bool,
    }),
    calendarOptions: React.PropTypes.shape({
        highlightDates: React.PropTypes.array,
        excludedDates: React.PropTypes.array,
        displayKey: React.PropTypes.bool,
        highlightKeyText: React.PropTypes.string,
        defaultKeyText: React.PropTypes.string,
        disabled: React.PropTypes.bool,
        loading: React.PropTypes.bool,
        useDealFinder: React.PropTypes.bool,
    }),
    childAgeText: React.PropTypes.string,
};

SearchTool.defaultProps = {
    minDuration: 1,
    maxDuration: 30,
    startCollapsed: false,
    collapseOnMobile: false,
    calendarOptions: {
        highlightDates: [],
    },
};
