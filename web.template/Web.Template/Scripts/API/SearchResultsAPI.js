import * as FlightResultsConstants from '../constants/flightresults';
import * as PropertyResultsConstants from '../constants/propertyresults';
import * as SearchConstants from '../constants/search';
import * as SearchResultsConstants from '../constants/searchresults';
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
    }
    setupSearchResults(token, searchMode, isPackage, state) {
        return this.getResults(token)
            .then(results => this.preSelectResults(results, searchMode, isPackage))
            .then(results => this.setupFilters(searchMode, results))
            .then(() => this.filter(this.results, this.filters, isPackage, state))
            .then(() => this.setupSortOptions(searchMode))
            .then(() => {
                const searchResults = {
                    results: this.results,
                    filters: this.filters,
                    sortOptions: this.sortOptions,
                    totalPages: this.totalPages,
                };
                return searchResults;
            });
    }
    getResults(token) {
        const encodedToken = encodeURIComponent(token);
        const resultsURL = `/api/searchResults?searchToken=${encodedToken}`;
        return fetch(resultsURL).then(response => response.json()).then(results => {
            this.results = results.SearchResults;
            return results.SearchResults;
        });
    }
    preSelectResults(results, searchMode, isPackage) {
        return new Promise((resolve) => {
            const updatedResults = Object.assign([], results);
            for (let i = 0; i < updatedResults.length; i++) {
                const result = updatedResults[i];
                result.isSelected = false;
            }
            if (searchMode === SearchConstants.SEARCH_MODES.FLIGHT && isPackage) {
                const cheapestResult = updatedResults[0];
                cheapestResult.isSelected = true;
            }
            this.results = updatedResults;
            resolve(this.results);
        });
    }
    setupFilters(searchMode, results) {
        switch (searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL:
                return this.setupHotelFilters(results);
            case SearchConstants.SEARCH_MODES.FLIGHT:
                return this.setupFlightFilters(results);
            default:
                return this.filters;
        }
    }
    setupSortOptions(searchMode) {
        switch (searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL:
                this.sortOptions = PropertyResultsConstants.SORT_OPTIONS;
                break;
            case SearchConstants.SEARCH_MODES.FLIGHT:
                this.sortOptions = FlightResultsConstants.SORT_OPTIONS;
                break;
            default:
        }
        return this.sortOptions;
    }
    setupHotelFilters(results) {
        return new Promise((resolve) => {
            PropertyResultsConstants.FILTER_OPTIONS.forEach(filterOption => {
                this.addFilter(results, filterOption);
            });
            resolve(this.filters);
        });
    }
    setupFlightFilters(results) {
        return new Promise((resolve) => {
            FlightResultsConstants.FILTER_OPTIONS.forEach(filterOption => {
                this.addFilter(results, filterOption);
            });
            resolve(this.filters);
        });
    }
    addFilter(results, filterOptions) {
        switch (filterOptions.type) {
            case SearchResultsConstants.FILTER_TYPES.ID:
                this.setupIdFilter(results, filterOptions);
                break;
            case SearchResultsConstants.FILTER_TYPES.RESULTS_RANGE:
                this.setupResultsRangeFilter(results, filterOptions);
                break;
            case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_ID:
                this.setupSubResultsIdFilter(results, filterOptions);
                break;
            case SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_RANGE:
                this.setupSubResultsRangeFilter(results, filterOptions);
                break;
            case SearchResultsConstants.FILTER_TYPES.TEXT: {
                const filter = filterOptions;
                this.filters.push(filter);
                break;
            }
            default:
        }
    }
    setupIdFilter(results, filterOptions) {
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
                    label: label.replace('##value##', value),
                    selected: false,
                };
                filter.options.push(option);
            }
        });
        this.filters.push(filter);
    }
    setupSubResultsIdFilter(results, filterOptions) {
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
                    filter.options.push(option);
                }
            });
        });
        this.filters.push(filter);
    }
    setupResultsRangeFilter(results, filterOptions) {
        const filter = filterOptions;
        results.forEach(result => {
            const value = ObjectFunctions.getValueByStringPath(result, filterOptions.field);
            if (filter.minValue === null || Math.floor(value) < filter.minValue) {
                filter.minValue = Math.floor(value);
            }
            if (filter.maxValue === null || Math.ceil(value) > filter.maxValue) {
                filter.maxValue = Math.ceil(value);
            }
        });
        this.filters.push(filter);
    }
    setupSubResultsRangeFilter(results, filterOptions) {
        const filter = filterOptions;
        results.forEach(result => {
            result.SubResults.forEach(subResult => {
                const value = ObjectFunctions.getValueByStringPath(subResult, filterOptions.field);
                if (filter.minValue === null || Math.floor(value) < filter.minValue) {
                    filter.minValue = Math.floor(value);
                }
                if (filter.maxValue === null || Math.ceil(value) > filter.maxValue) {
                    filter.maxValue = Math.ceil(value);
                }
            });
        });
        this.filters.push(filter);
    }
    updateFilter(filters, key, value) {
        const updatedFilters = ObjectFunctions.setValueByStringPath(filters, key, value);
        return updatedFilters;
    }
    filter(results, filters, isPackage, state) {
        for (let i = 0; i < results.length; i++) {
            const result = results[i];
            const validReturn = this.validResult(result, filters, isPackage, state);
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
    getSelectedResult(searchMode) {
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
        const selectedFlight = this.getSelectedResult(SearchConstants.SEARCH_MODES.FLIGHT);
        const arrivalAirportId = selectedFlight.MetaData.ArrivalAirportId;
        const arrivalAirport = this.getAirportById(state, arrivalAirportId);

        let validResort = false;
        for (let i = 0; i < arrivalAirport.Resorts.length; i++) {
            const resort = arrivalAirport.Resorts[i];
            if (resort.Id === result.MetaData.GeographyLevel3Id) {
                validResort = true;
                break;
            }
        }
        return validResort;
    }
    validResult(result, filters, searchMode, isPackage) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };

        filters.filter(filter => filter.isSubResultsFilter === false).forEach(filter => {
            validReturn.validExcludes.push({ name: filter.name, display: true });
        });

        let validResort = true;
        if (isPackage && searchMode === SearchConstants.SEARCH_MODES.HOTEL) {
            validResort = this.validAirportResort(result, state);
        }

        if (!result.isSelected && validResort) {
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
                const validSubResult = this.filterSubResults(result, filters);
                if (!validSubResult) {
                    validReturn.valid = false;
                    for (let j = 0; j < validReturn.validExcludes.length; j++) {
                        const exclude = validReturn.validExcludes[j];
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
    filterSubResults(result, filters) {
        let validSubResult = false;

        for (let i = 0; i < result.SubResults.length; i++) {
            const subResult = result.SubResults[i];
            const validReturn = this.validSubResult(subResult, filters);
            subResult.Display = validReturn.valid;
            subResult.ValidExcludes = validReturn.validExcludes;
            if (validReturn.valid) {
                validSubResult = true;
            }
        }

        return validSubResult;
    }
    validSubResult(subResult, filters) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };

        filters.filter(filter => filter.isSubResultsFilter === true).forEach(filter => {
            validReturn.validExcludes.push({ name: filter.name, display: true });
        });

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
            }
        }
        return validReturn;
    }
}

export default SearchResultsAPI;
