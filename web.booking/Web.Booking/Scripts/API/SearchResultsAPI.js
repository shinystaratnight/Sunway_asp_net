import * as FlightResultsConstants from '../constants/flightresults';
import * as PropertyResultsConstants from '../constants/propertyresults';
import * as SearchConstants from '../constants/search';
import * as SearchResultsConstants from '../constants/searchresults';
import ArrayFunctions from '../library/arrayfunctions';
import ObjectFunctions from '../library/objectfunctions';
import fetch from 'isomorphic-fetch';

class SearchResultsAPI {
    constructor() {
        this.isPackageSearch = false;
        this.results = [];
        this.filters = [];
        this.sortOptions = [];
        this.resultsPerPage = 5;
        this.totalPages = 1;
        this.siteConfiguration = {};
    }
    storageAvailable(type) {
        try {
            const storage = window[type];
            const x = '__storage_test__';
            storage.setItem(x, x);
            storage.removeItem(x);
            return true;
        } catch (exception) {
            return false;
        }
    }
    setupSearchResults(token, searchMode, flightPlusHotelType, selectedToken, state, customSorts,
    filterKey) {
        const flightPlusHotelTypes = SearchResultsConstants.FLIGHT_PLUS_HOTEL_TYPES;
        const isPackage = flightPlusHotelType === flightPlusHotelTypes.PACKAGESEARCH
            || flightPlusHotelType === flightPlusHotelTypes.PACKAGEPRICE;

        this.siteConfiguration = state.site.SiteConfiguration;
        console.log('this.siteConfiguration: ', this.siteConfiguration);

        return this.getResults(token)
            .then(results => this.preSelectResults(results, searchMode, isPackage, selectedToken))
            .then(results => this.setupFilters(searchMode, results, filterKey))
            .then(() => this.filter(this.results, this.filters,
                searchMode, flightPlusHotelType, state))
            .then(() => this.setupSortOptions(searchMode, customSorts))
            .then(() => {
                const searchResults = {
                    token,
                    results: this.results,
                    filters: this.filters,
                    sortOptions: this.sortOptions,
                    totalPages: this.totalPages,
                };
                console.log(searchResults);
                return searchResults;
            });
    }
    setupExtraSearchResults(token) {
        return this.getResults(token)
            .then((results) => {
                const searchResults = {
                    token,
                    results,
                };
                return searchResults;
            });
    }
    getResultsGiata(token) {
        const encodedToken = encodeURIComponent(token);
        const resultsURL = `http://10.36.5.117:5000/Country_Hotels/es/0/10?searchToken=${encodedToken}`;
        return fetch(resultsURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(results => {
                this.results = results.SearchResults;
                console.log(results.SearchResults);
                console.log('giata results: ', results.SearchResults);
                return results.SearchResults;
            });
    }
    getResults(token) {
        const encodedToken = encodeURIComponent(token);
        const resultsURL = `/booking/api/searchResults?searchToken=${encodedToken}`;

        return fetch(resultsURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(results => {
                this.results = results.SearchResults;
                console.log(results.SearchResults);
                return results.SearchResults;
            });
    }
    preSelectResults(results, searchMode, isPackage, selectedToken) {
        return new Promise((resolve) => {
            let updatedResults = Object.assign([], results);
            for (let i = 0; i < updatedResults.length; i++) {
                const result = updatedResults[i];
                result.isSelected = result.ComponentToken === selectedToken;
            }
            if (!selectedToken && searchMode === SearchConstants.SEARCH_MODES.FLIGHT
                && isPackage && updatedResults.length) {
                updatedResults = ArrayFunctions.sortByPropertyAscending(updatedResults, 'Price');
                const cheapestResult = updatedResults[0];
                cheapestResult.isSelected = true;
            }
            this.results = updatedResults;
            resolve(this.results);
        });
    }
    setupFilters(searchMode, results, filterKey) {
        return (this.getStoredFilters(searchMode, filterKey)
            .then(filters => {
                switch (searchMode) {
                    case SearchConstants.SEARCH_MODES.HOTEL:
                        return this.addFilterOptions(results,
                            PropertyResultsConstants.FILTER_OPTIONS,
                            filters);
                    case SearchConstants.SEARCH_MODES.FLIGHT:
                        return this.addFilterOptions(results,
                            FlightResultsConstants.FILTER_OPTIONS,
                            filters);
                    default:
                        return this.filters;
                }
            }));
    }
    setupSortOptions(searchMode, customSorts) {
        switch (searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL:
                this.sortOptions = PropertyResultsConstants.SORT_OPTIONS;
                break;
            case SearchConstants.SEARCH_MODES.FLIGHT:
                this.sortOptions = FlightResultsConstants.SORT_OPTIONS;
                break;
            default:
        }
        if (customSorts !== undefined) {
            this.sortOptions = this.sortOptions.concat(customSorts);
        }
        return this.sortOptions;
    }
    setupDefaultFilterOptions(filters, options) {
        const modifiedFilters = [];
        const overrides = [];
        Object.keys(options).forEach(k => {
            const optionHasChange = options[k].DefaultCollapsed;
            if (optionHasChange) {
                overrides.push({
                    filterName: k.toLowerCase(),
                    defaultCollapsed: options[k].DefaultCollapsed,
                });
            }
        });
        filters.forEach(f => {
            const newFilter = f;
            const filterName = f.name.toLowerCase();
            const relevantOverrideIndex = overrides.findIndex(o => o.filterName === filterName);
            if (relevantOverrideIndex > -1) {
                const override = overrides[relevantOverrideIndex];
                if (override.defaultCollapsed) {
                    newFilter.expanded = false;
                }
            }
            modifiedFilters.push(newFilter);
        });
        return modifiedFilters;
    }
    getStoredFilters(searchMode, filterKey) {
        return new Promise((resolve) => {
            let filters = {};
            if (this.storageAvailable('sessionStorage')) {
                const filtersJson = sessionStorage.getItem(`__searchfilters_${searchMode}__`);
                if (filtersJson) {
                    let filterStore;
                    try {
                        filterStore = JSON.parse(filtersJson);
                    } catch (exception) {
                        filterStore = {};
                    }
                    filters = filterStore.filters;
                    if (filterStore.key !== filterKey) {
                        filters = {};
                    }
                }
                if (!filters) {
                    filters = {};
                }
            } else {
                filters = {};
            }
            resolve(Object.assign({}, filters));
        });
    }
    addFilterOptions(results, filterOptions, storedFilters) {
        return new Promise((resolve) => {
            filterOptions.forEach(filterOption => {
                let storedfilter = {};
                Object.values(storedFilters).forEach((filter) => {
                    if (filter.name === filterOption.name) {
                        storedfilter = filter;
                    }
                });
                this.addFilter(results, filterOption, storedfilter);
            });
            resolve(this.filters);
        });
    }
    addFilter(results, filterOptions, storedfilter) {
        switch (filterOptions.type) {
            case SearchResultsConstants.FILTER_TYPES.ID:
                this.setupIdFilter(results, filterOptions, storedfilter);
                break;
            case SearchResultsConstants.FILTER_TYPES.RESULTS_RANGE:
                this.setupResultsRangeFilter(results, filterOptions, storedfilter);
                break;
            case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_ID:
                this.setupSubResultsIdFilter(results, filterOptions, storedfilter);
                break;
            case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_RANGE:
                this.setupSubResultsRangeFilter(results, filterOptions, storedfilter);
                break;
            case SearchResultsConstants.FILTER_TYPES.TEXT:
                {
                    const filter = filterOptions;
                    if (storedfilter) {
                        if (storedfilter.value) {
                            filter.value = storedfilter.value;
                        }
                    }
                    this.filters.push(filter);
                    break;
                }
            default:
        }
    }
    setupIdFilter(results, filterOptions, storedFilter) {
        const filter = filterOptions;
        const ids = [];

        results.forEach(result => {
            const value = ObjectFunctions.getValueByStringPath(result, filterOptions.field);
            if (ids.indexOf(value) === -1) {
                ids.push(value);
                const label = (parseInt(value, 10) === 0.0
                    && filterOptions.hasOwnProperty('labelZero'))
                    ? filterOptions.labelZero : filterOptions.label;
                const option = {
                    id: value,
                    selected: false,
                };
                if (filter.labelFunction) {
                    option.label = filter.labelFunction(value, this.siteConfiguration);
                } else {
                    option.label = label.replace('##value##', value)
                                        .replace('.0', '')
                                        .replace('.5', '½');
                }
                if (storedFilter.options && Array.isArray(storedFilter.options)) {
                    storedFilter.options.forEach(storedOption => {
                        if (storedOption.id === option.id) {
                            option.selected = storedOption.selected;
                        }
                    });
                }

                filter.options.push(option);
            }
        });
        this.filters.push(filter);
    }
    setupSubResultsIdFilter(results, filterOptions, storedFilter) {
        const filter = filterOptions;
        const ids = [];
        results.forEach(result => {
            result.SubResults.forEach(subResult => {
                const value = ObjectFunctions.getValueByStringPath(subResult, filterOptions.field);
                if (ids.indexOf(value) === -1) {
                    ids.push(value);
                    const option = {
                        id: value,
                        label: filterOptions.label.replace('##value##', value),
                        selected: false,
                    };
                    if (storedFilter.options && Array.isArray(storedFilter.options)) {
                        storedFilter.options.forEach(storedOption => {
                            if (storedOption.id === option.id) {
                                option.selected = storedOption.selected;
                            }
                        });
                    }
                    filter.options.push(option);
                }
            });
        });
        this.filters.push(filter);
    }
    setupResultsRangeFilter(results, filterOptions, storedFilter) {
        const filter = filterOptions;

        if (storedFilter.hasOwnProperty('minValue') && storedFilter.hasOwnProperty('maxValue')) {
            filter.minValue = storedFilter.minValue;
            filter.maxValue = storedFilter.maxValue;
        } else {
            results.forEach(result => {
                const value = ObjectFunctions.getValueByStringPath(result, filterOptions.field);
                if (filter.minValue === null || Math.floor(value) < filter.minValue) {
                    filter.minValue = Math.floor(value);
                }
                if (filter.maxValue === null || Math.ceil(value) > filter.maxValue) {
                    filter.maxValue = Math.ceil(value);
                }
            });
        }
        this.filters.push(filter);
    }
    setupSubResultsRangeFilter(results, filterOptions, storedFilter) {
        const filter = filterOptions;
        if (storedFilter.hasOwnProperty('minValue') && storedFilter.hasOwnProperty('maxValue')) {
            filter.minValue = storedFilter.minValue;
            filter.maxValue = storedFilter.maxValue;
        } else {
            results.forEach(result => {
                result.SubResults.forEach(subResult => {
                    const value = ObjectFunctions.getValueByStringPath(subResult,
                        filterOptions.field);
                    if (filter.minValue === null || Math.floor(value) < filter.minValue) {
                        filter.minValue = Math.floor(value);
                    }
                    if (filter.maxValue === null || Math.ceil(value) > filter.maxValue) {
                        filter.maxValue = Math.ceil(value);
                    }
                });
            });
        }

        this.filters.push(filter);
    }
    updateFilter(filters, key, value, searchMode, filterKey) {
        const updatedFilters = ObjectFunctions.setValueByStringPath(filters, key, value);

        const filterStore = {
            key: filterKey,
            filters: updatedFilters,
        };

        sessionStorage.setItem(`__searchfilters_${searchMode}__`, JSON.stringify(filterStore));

        return updatedFilters;
    }
    filter(results, filters, searchMode, flightPlusHotelType, state) {
        for (let i = 0; i < results.length; i++) {
            const result = results[i];
            const validReturn = this.validResult(result, filters, searchMode,
                flightPlusHotelType, state);
            result.Display = validReturn.valid;
            result.ValidExcludes = validReturn.validExcludes;
            if (Array.isArray(result.SubResults)) {
                const minPrice = this.subResultMinPrice(result);
                result.Price = minPrice;
            }
        }

        const resultCount = results.filter(result => result.Display === true).length;
        const totalPages = resultCount > this.resultsPerPage
            ? Math.ceil(resultCount / this.resultsPerPage) : 1;
        this.totalPages = totalPages;

        return {
            results,
            totalPages,
        };
    }
    setSelected(results, componentToken, filters) {
        const updatedResults = Object.assign([], results);
        for (let i = 0; i < updatedResults.length; i++) {
            const result = updatedResults[i];
            result.isSelected = result.ComponentToken === componentToken;
        }
        const filteredResults = this.filter(updatedResults, filters);
        filteredResults.isChangingFlight = false;
        return filteredResults;
    }
    subResultMinPrice(result) {
        let minPrice = null;
        result.SubResults.forEach(subResult => {
            if (subResult.Display === true
                    && (minPrice === null || subResult.TotalPrice < minPrice)) {
                minPrice = subResult.TotalPrice;
            }
        });
        return minPrice;
    }
    validIdValue(result, filter) {
        let valid = false;
        const selectedOptions = filter.options.filter(option => option.selected === true);
        if (selectedOptions.length > 0) {
            for (let i = 0; i < selectedOptions.length; i++) {
                const option = selectedOptions[i];
                const value = ObjectFunctions.getValueByStringPath(result, filter.field);
                if (value === option.id) {
                    valid = true;
                    break;
                }
            }
        } else {
            valid = true;
        }
        return valid;
    }
    validTextValue(result, filter) {
        let valid = true;
        if (filter.value !== '') {
            const value = ObjectFunctions.getValueByStringPath(result, filter.field).toLowerCase();
            const filterValue = filter.value.toLowerCase();
            valid = value === filterValue || value.indexOf(filterValue) !== -1;
        }
        return valid;
    }
    validSubResultValue(subResult, filter) {
        let valid = false;
        const selectedOptions = filter.options.filter(option => option.selected === true);
        if (selectedOptions.length > 0) {
            const value = ObjectFunctions.getValueByStringPath(subResult, filter.field);
            for (let j = 0; j < selectedOptions.length; j++) {
                const option = selectedOptions[j];
                if (value === option.id) {
                    valid = true;
                    break;
                }
            }
        } else {
            valid = true;
        }
        return valid;
    }
    validResultRangeValue(result, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(result, filter.field);
        if (value >= filter.minValue && value <= filter.maxValue) {
            valid = true;
        }
        return valid;
    }
    validSubResultRangeValue(subResult, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(subResult, filter.field);
        if (value >= filter.minValue && value <= filter.maxValue) {
            valid = true;
        }
        return valid;
    }
    validFilter(result, filter) {
        let valid = true;
        switch (filter.type) {
            case SearchResultsConstants.FILTER_TYPES.ID:
                valid = this.validIdValue(result, filter);
                break;
            case SearchResultsConstants.FILTER_TYPES.TEXT:
                valid = this.validTextValue(result, filter);
                break;
            case SearchResultsConstants.FILTER_TYPES.RESULTS_RANGE:
                valid = this.validResultRangeValue(result, filter);
                break;
            default:
        }
        return valid;
    }
    getSelectedResult(searchMode, state) {
        const searchResults = state.searchResults;
        const searchModeResults = searchResults[searchMode];
        let selectedResult = null;

        for (let i = 0; i < searchModeResults.results.length; i++) {
            const result = searchModeResults.results[i];
            if (result.isSelected) {
                selectedResult = result;
                break;
            }
        }
        return selectedResult;
    }
    getAirportById(state, id) {
        let airport = {};
        for (let i = 0; i < state.airports.items.length; i++) {
            const lookupAirport = state.airports.items[i];
            if (lookupAirport.Id === id) {
                airport = lookupAirport;
                break;
            }
        }
        return airport;
    }
    validAirportResort(result, state) {
        let validResort = false;

        const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT, state);
        if (selectedFlight) {
            const arrivalAirportId = selectedFlight.MetaData.ArrivalAirportId;
            const arrivalAirport = this.getAirportById(state, arrivalAirportId);
            if (arrivalAirport && arrivalAirport.Resorts) {
                for (let i = 0; i < arrivalAirport.Resorts.length; i++) {
                    const resort = arrivalAirport.Resorts[i];
                    if (resort.Id === result.MetaData.GeographyLevel3Id) {
                        validResort = true;
                        break;
                    }
                }
            }
        } else {
            validResort = true;
        }

        return validResort;
    }
    validFlightResult(subResult, state) {
        let validResult = true;
        const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT, state);

        if (selectedFlight) {
            const id = selectedFlight.MetaData.Id;
            const index = subResult.InvalidFlightResultIds.indexOf(id);
            validResult = index === -1;
        }

        return validResult;
    }
    validResult(result, filters, searchMode, flightPlusHotelType, state) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };
        let validResult = true;

        const flightPlusHotelTypes = SearchResultsConstants.FLIGHT_PLUS_HOTEL_TYPES;
        if (flightPlusHotelType === flightPlusHotelTypes.PACKAGEPRICE
                && searchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            validResult = this.validAirportResort(result, state);
        }

        if (!result.isSelected && validResult) {
            filters.filter(filter => filter.isSubResultsFilter === false).forEach(filter => {
                validReturn.validExcludes.push({ name: filter.name, display: true });
            });

            for (let i = 0; i < filters.length; i++) {
                const filter = filters[i];
                const valid = this.validFilter(result, filter);
                if (!valid) {
                    validReturn.valid = false;
                    for (let j = 0; j < validReturn.validExcludes.length; j++) {
                        const exclude = validReturn.validExcludes[j];
                        if (exclude.name !== filter.name) {
                            exclude.display = false;
                        }
                    }
                }
            }
            if (Array.isArray(result.SubResults)) {
                const validSubResult = this.filterSubResults(result, filters, searchMode,
                        flightPlusHotelType, state, validReturn.valid, validReturn.validExcludes);
                if (!validSubResult) {
                    validReturn.valid = false;
                }
            }
        } else {
            validReturn.valid = false;
            for (let i = 0; i < validReturn.validExcludes.length; i++) {
                const exclude = validReturn.validExcludes[i];
                exclude.display = false;
            }
        }
        return validReturn;
    }
    filterSubResults(result, filters, searchMode, flightPlusHotelType, state, validParent,
                        parentExcludes) {
        const validSubResults = {};
        for (let i = 0; i < result.SubResults.length; i++) {
            const subResult = result.SubResults[i];
            if (!validSubResults.hasOwnProperty(subResult.Sequence)) {
                validSubResults[subResult.Sequence] = false;
            }
            const validReturn = this.validSubResult(subResult, filters, searchMode,
                flightPlusHotelType, state, validParent, parentExcludes);
            subResult.Display = validReturn.valid;
            subResult.ValidExcludes = validReturn.validExcludes;
            if (validReturn.valid) {
                validSubResults[subResult.Sequence] = true;
            }
        }
        let validSubResult = true;
        Object.keys(validSubResults).map((sequence) => {
            if (!validSubResults[sequence]) {
                validSubResult = false;
            }
        });
        return validSubResult;
    }
    validSubResult(subResult, filters, searchMode, flightPlusHotelType, state, validParent,
                    parentExcludes) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };

        filters.filter(filter => filter.isSubResultsFilter === true).forEach(filter => {
            validReturn.validExcludes.push({ name: filter.name, display: true });
        });

        let validResult = validParent;

        if (validResult) {
            const flightPlusHotelTypes = SearchResultsConstants.FLIGHT_PLUS_HOTEL_TYPES;
            if (flightPlusHotelType === flightPlusHotelTypes.PACKAGESEARCH
                    && searchMode === SearchConstants.SEARCH_MODES.HOTEL) {
                validResult = this.validFlightResult(subResult, state);
            }
        }

        if (validResult) {
            for (let i = 0; i < filters.length; i++) {
                let valid = true;
                const filter = filters[i];
                switch (filter.type) {
                    case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_ID:
                        valid = this.validSubResultValue(subResult, filter);
                        break;
                    case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_RANGE:
                        valid = this.validSubResultRangeValue(subResult, filter);
                        break;
                    default:
                }
                if (!valid) {
                    validReturn.valid = false;
                    for (let j = 0; j < validReturn.validExcludes.length; j++) {
                        const exclude = validReturn.validExcludes[j];
                        if (exclude.name !== filter.name) {
                            exclude.display = false;
                        }
                    }
                    for (let j = 0; j < parentExcludes.length; j++) {
                        const exclude = parentExcludes[j];
                        exclude.display = false;
                    }
                }
            }
        } else {
            validReturn.valid = false;
            for (let i = 0; i < validReturn.validExcludes.length; i++) {
                const exclude = validReturn.validExcludes[i];
                exclude.display = false;
            }
        }
        return validReturn;
    }
}

export default SearchResultsAPI;
