import ArrayFunctions from '../library/arrayfunctions';
import ObjectFunctions from '../library/objectfunctions';
import fetch from 'isomorphic-fetch';

const FILTER_TYPES = {
    ID: 'ID',
    ARRAY: 'ARRAY',
    RESULTS_RANGE: 'RESULTS_RANGE',
    SUB_RESULTS_RANGE: 'SUB_RESULTS_RANGE',
    SUB_RESULTS_ID: 'SUB_RESULTS_ID',
};

const FILTER_OPTIONS = [
    {
        name: 'departureAirport',
        type: FILTER_TYPES.SUB_RESULTS_ID,
        isSubResultsFilter: true,
        title: 'Departure Airport',
        field: 'Airport',
        label: '##lookup##',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
    },
    {
        name: 'country',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Destination',
        field: 'GeographyLevel1ID',
        label: '##lookup##',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
    },
    {
        name: 'mealBasis',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Board Basis',
        field: 'AdditionalInformation.MealBasisID',
        label: '##lookup##',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
    },
    {
        name: 'productAttribute',
        type: FILTER_TYPES.ARRAY,
        isSubResultsFilter: false,
        title: 'Experiences',
        field: 'ProductAttributeCSV',
        label: '##lookup##',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: true,
    },
    {
        name: 'adults',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Adults',
        field: 'AdditionalInformation.Adults',
        label: '##value##',
        labelZero: '0',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
        format: 'guests',
    },
    {
        name: 'children',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Children',
        field: 'AdditionalInformation.Children',
        label: '##value##',
        labelZero: '0',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
        format: 'guests',
    },
    {
        name: 'infants',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Infants',
        field: 'AdditionalInformation.Infants',
        label: '##value##',
        labelZero: '0',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
        format: 'guests',
    },
    {
        name: 'starRating',
        type: FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Star Rating',
        field: 'HotelRating',
        label: '##value## Stars',
        labelFunction: (value, siteConfiguration) => {
            const ratingConfig = siteConfiguration.StarRatingConfiguration;
            let text = '';
            ratingConfig.AppendText.forEach(appendText => {
                if (parseFloat(appendText.Rating) === parseFloat(value)) {
                    text = appendText.Text;
                }
            });

            const displayValue = ratingConfig.DisplayHalfRatings ? value : Math.floor(value);
            let label = `${displayValue} Star`;
            label += text ? ` ${text}` : '';
            return label;
        },
        labelZero: 'Unrated',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
        optional: false,
    },
    {
        name: 'price',
        type: FILTER_TYPES.SUB_RESULTS_RANGE,
        isSubResultsFilter: true,
        title: 'Price per person',
        field: 'PerPersonPrice',
        minValue: null,
        maxValue: null,
        defaultMinValue: 0,
        defaultMaxValue: 1000000,
    },
];

/**
  * Function to do the default offer sort
  * @param {array} offers - The unsorted offers
  * @return {array} The sorted offers
*/
function defaultOfferSort(offers) {
    const initalSortedOffers = ArrayFunctions
        .sortByPropertyAscending(offers, 'HotelName');
    const sortField = 'LastModifiedDateTime';
    const dateSortedOffers = ArrayFunctions
        .sortByDatePropertyDescending(initalSortedOffers, sortField);
    const chosenByUsOffers = dateSortedOffers.filter(o => o.ChosenByUs === '1');
    const notChosenByUsOffers = dateSortedOffers.filter(o => o.ChosenByUs !== '1');

    return chosenByUsOffers.concat(notChosenByUsOffers);
}


const SORT_OPTIONS = [
    {
        name: 'Default',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return defaultSortedOffers;
        },
    },
    {
        name: 'Price (Low to High)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyAscending(defaultSortedOffers,
            'Price');
        },
    },
    {
        name: 'Price (High to Low)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyDescending(defaultSortedOffers,
            'Price');
        },
    },
    {
        name: 'Hotel name (A-Z)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyAscending(defaultSortedOffers,
            'HotelName');
        },
    },
    {
        name: 'Hotel name (Z-A)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyDescending(defaultSortedOffers,
            'HotelName');
        },
    },
    {
        name: 'Star rating (Low to High)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyAscending(defaultSortedOffers,
            'HotelRating');
        },
    },
    {
        name: 'Star rating (High to Low)',
        sortFunction: (offers) => {
            const defaultSortedOffers = defaultOfferSort(offers);
            return ArrayFunctions.sortByPropertyDescending(defaultSortedOffers,
            'HotelRating');
        },
    },
];

class SpecialOffersApi {
    constructor() {
        this.sortOptions = [];
        this.selectedSort = 'Most recent';
        this.specialOffers = [];
        this.resultsPerPage = 5;
        this.totalPages = 1;
        this.filters = [];
        this.siteConfiguration = {};
    }
    setupSpecialOffers(filterOptions, siteConfiguration) {
        this.siteConfiguration = siteConfiguration;
        return this.getOffers(filterOptions)
            .then(offers => this.setupFilters(offers))
            .then(() => this.filter(this.specialOffers, this.filters))
            .then(() => this.setupSortOptions())
            .then(() => {
                const offers = {
                    items: this.specialOffers,
                    filters: this.filters,
                    sortOptions: this.sortOptions,
                    selectedSort: this.selectedSort,
                    totalPages: this.totalPages,
                };
                return offers;
            });
    }
    getOffers(filterOptions) {
        let apiURL = '/api/offers/query?';

        const propertyReference = filterOptions.propertyReference
            ? filterOptions.propertyReference
            : 0;

        apiURL += `geographylevel1=${filterOptions.geographyLevel1}`;
        apiURL += `&geographylevel2=${filterOptions.geographyLevel2}`;
        apiURL += `&numberofoffers=${filterOptions.numberOfOffers}`;
        apiURL += `&productattribute=${filterOptions.productAtttribute}`;
        apiURL += `&orderby=${filterOptions.orderBy}`;
        apiURL += `&propertyReference=${propertyReference}`;
        apiURL += `&includeLeadInHotels=${filterOptions.includeLeadIns}`;
        apiURL += `&highlightedPropertiesOnly=${filterOptions.highlightedPropertiesOnly}`;

        return fetch(apiURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(result => {
                let specialOffers = [];
                if (result.CustomQueryResponse && result.CustomQueryResponse.CustomXML) {
                    specialOffers
                        = result.CustomQueryResponse.CustomXML.SpecialOffers.SpecialOffer;
                }
                if (!Array.isArray(specialOffers)) {
                    specialOffers = [specialOffers];
                }
                this.specialOffers = specialOffers;
                return specialOffers;
            });
    }
    getOffer(id) {
        const apiURL = `/api/offer/${id}`;
        return fetch(apiURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(result => {
                let specialOffers = {};
                if (result.CustomQueryResponse && result.CustomQueryResponse.CustomXML) {
                    specialOffers = result.CustomQueryResponse.CustomXML.SpecialOffer;
                }
                return specialOffers;
            }
        );
    }

    getOfferPoster(offerId, airportId, location, type) {
        const apiQuery = `?airportID=${airportId}&location=${location}&type=${type}`;
        const apiURL = `/api/offer/poster/${offerId}${apiQuery}`;
        return fetch(apiURL, { credentials: 'same-origin' })
            .then(response => response.json());
    }
    setupFilters(results) {
        return new Promise((resolve) => {
            FILTER_OPTIONS.forEach(filterOption => {
                this.addFilter(results, filterOption);
            });
            resolve(this.filters);
        });
    }
    addFilter(results, filterOptions) {
        switch (filterOptions.type) {
            case FILTER_TYPES.ID:
                this.setupIdFilter(results, filterOptions);
                break;
            case FILTER_TYPES.SUB_RESULTS_ID:
                this.setupSubResultsIdFilter(results, filterOptions);
                break;
            case FILTER_TYPES.RESULTS_RANGE:
                this.setupResultsRangeFilter(results, filterOptions);
                break;
            case FILTER_TYPES.SUB_RESULTS_RANGE:
                this.setupSubResultsRangeFilter(results, filterOptions);
                break;
            case FILTER_TYPES.ARRAY:
                this.setupArrayFilter(results, filterOptions);
                break;
            default:
        }
    }
    setupIdFilter(results, filterOptions) {
        const filter = filterOptions;
        const ids = [];
        results.forEach(offer => {
            const result = Object.assign({}, offer);
            const value = parseFloat(ObjectFunctions.getValueByStringPath(result,
                filterOptions.field));
            if (ids.indexOf(value) === -1) {
                ids.push(value);

                let label = '';
                if (typeof filterOptions.labelFunction === 'function') {
                    label = filterOptions.labelFunction(value, this.siteConfiguration);
                } else {
                    label = (value === 0.0
                    && filterOptions.hasOwnProperty('labelZero'))
                    ? filterOptions.labelZero : filterOptions.label;
                    label = label.replace('##value##', value);
                }

                const option = {
                    id: value,
                    label,
                };
                filter.options.push(option);
            }
        });
        this.filters.push(filter);
    }
    setupSubResultsIdFilter(results, filterOptions) {
        const filter = filterOptions;
        const ids = [];
        results.forEach(offer => {
            const result = Object.assign({}, offer);
            if (!Array.isArray(result.AirportPrice.AirportPrice)) {
                result.AirportPrice.AirportPrice = [result.AirportPrice.AirportPrice];
            }
            result.AirportPrice.AirportPrice.forEach(airportPrice => {
                const value
                    = ObjectFunctions.getValueByStringPath(airportPrice, filterOptions.field);
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
            if (filter.minValue === null || Math.ceil(value) > filter.maxValue) {
                filter.maxValue = Math.ceil(value);
            }
        });
        this.filters.push(filter);
    }
    setupSubResultsRangeFilter(results, filterOptions) {
        const filter = filterOptions;
        results.forEach(offer => {
            const result = Object.assign({}, offer);
            if (!Array.isArray(result.AirportPrice.AirportPrice)) {
                result.AirportPrice.AirportPrice = [result.AirportPrice.AirportPrice];
            }
            result.AirportPrice.AirportPrice.forEach(airportPrice => {
                const value
                    = ObjectFunctions.getValueByStringPath(airportPrice, filterOptions.field);
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
    setupArrayFilter(results, filterOptions) {
        const filter = filterOptions;
        const ids = [];
        results.forEach(result => {
            const valueCsv = ObjectFunctions.getValueByStringPath(result, filterOptions.field);
            let valueArray = [];
            if (valueCsv) {
                valueArray = valueCsv.split(',');
            }
            if (valueCsv) {
                valueArray.forEach(value => {
                    if (ids.indexOf(value) === -1) {
                        ids.push(value);
                        const label
                            = (parseInt(value, 10) === 0.0
                            && filterOptions.hasOwnProperty('labelZero'))
                            ? filterOptions.labelZero
                            : filterOptions.label;
                        const option = {
                            id: value,
                            label: label.replace('##value##', value),
                        };
                        if (option.id) {
                            filter.options.push(option);
                        }
                    }
                });
            }
        });
        this.filters.push(filter);
    }
    setupPaging(specialOffers) {
        const resultCount = specialOffers.filter(offer => offer.Display === true).length;
        const totalPages = Math.ceil(resultCount / this.resultsPerPage);
        this.totalPages = totalPages;
    }
    setupSortOptions() {
        return new Promise((resolve) => {
            this.sortOptions = SORT_OPTIONS;
            this.selectedSort = 'Most recent';
            resolve(this.sortOptions);
        });
    }
    filter(specialOffers, filters) {
        for (let i = 0; i < specialOffers.length; i++) {
            const specialOffer = specialOffers[i];
            const validReturn = this.validOffer(specialOffer, filters);
            specialOffer.Display = validReturn.valid;
            specialOffer.ValidExcludes = validReturn.validExcludes;
            if (Array.isArray(specialOffer.AirportPrice.AirportPrice)) {
                const minPrice = this.subResultMinPrice(specialOffer);
                specialOffer.Price = minPrice;
            }
        }
        this.setupPaging(specialOffers);
        return {
            specialOffers,
            totalPages: this.totalPages,
        };
    }
    subResultMinPrice(offer) {
        const result = Object.assign({}, offer);
        let minPrice = null;
        if (!Array.isArray(result.AirportPrice.AirportPrice)) {
            result.AirportPrice.AirportPrice = [result.AirportPrice.AirportPrice];
        }
        result.AirportPrice.AirportPrice.forEach(airportPrice => {
            if (airportPrice.Display === true
                    && (minPrice === null
                || parseFloat(airportPrice.PerPersonPrice) < parseFloat(minPrice))) {
                minPrice = airportPrice.PerPersonPrice;
            }
        });
        return minPrice;
    }
    updateFilter(filters, key, value) {
        const updatedFilters = ObjectFunctions.setValueByStringPath(filters, key, value);
        return updatedFilters;
    }
    validOffer(specialOffer, filters) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };

        filters.filter(filter => filter.isSubResultsFilter === false).forEach(filter => {
            validReturn.validExcludes.push({ name: filter.name, display: true });
        });

        if (Array.isArray(specialOffer.AirportPrice.AirportPrice)) {
            const validSubResult = this.filterSubResults(specialOffer, filters);
            if (!validSubResult) {
                validReturn.valid = false;
                for (let j = 0; j < validReturn.validExcludes.length; j++) {
                    const exclude = validReturn.validExcludes[j];
                    exclude.display = false;
                }
            }
        }

        for (let i = 0; i < filters.length; i++) {
            const filter = filters[i];
            const valid = this.validFilter(specialOffer, filter);
            if (!valid) {
                validReturn.valid = false;
                for (let j = 0; j < validReturn.validExcludes.length; j++) {
                    const exclude = validReturn.validExcludes[j];
                    if (exclude.name !== filter.name) {
                        exclude.display = false;
                    }
                }

                if (Array.isArray(specialOffer.AirportPrice.AirportPrice)) {
                    for (let j = 0; j < specialOffer.AirportPrice.AirportPrice.length; j++) {
                        const subResult = specialOffer.AirportPrice.AirportPrice[j];
                        subResult.Display = false;
                        for (let k = 0; k < subResult.ValidExcludes.length; k++) {
                            const exclude = subResult.ValidExcludes[k];
                            exclude.display = false;
                        }
                    }
                }
            }
        }

        return validReturn;
    }
    filterSubResults(result, filters) {
        let validSubResult = false;

        for (let i = 0; i < result.AirportPrice.AirportPrice.length; i++) {
            const subResult = result.AirportPrice.AirportPrice[i];
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
                case FILTER_TYPES.SUB_RESULTS_ID:
                    valid = this.validSubResultValue(subResult, filter);
                    break;
                case FILTER_TYPES.SUB_RESULTS_RANGE:
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
    validFilter(result, filter) {
        let valid = true;
        switch (filter.type) {
            case FILTER_TYPES.ID:
                valid = this.validIdValue(result, filter);
                break;
            case FILTER_TYPES.ARRAY:
                valid = this.validArrayValue(result, filter);
                break;
            case FILTER_TYPES.RESULTS_RANGE:
                valid = this.validResultRangeValue(result, filter);
                break;
            default:
        }
        return valid;
    }
    validIdValue(specialOffer, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(specialOffer, filter.field);
        if (filter.selectedValue === -1 || parseFloat(value) === parseFloat(filter.selectedValue)) {
            valid = true;
        }
        return valid;
    }
    validSubResultValue(subResult, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(subResult, filter.field);
        if (filter.selectedValue === -1 || parseInt(value, 10) === filter.selectedValue) {
            valid = true;
        }
        return valid;
    }
    validArrayValue(specialOffer, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(specialOffer, filter.field);
        const valueArray = value ? value.split(',') : [];
        const selectedValue = filter.selectedValue.toString();
        if (selectedValue === '-1' || valueArray.indexOf(selectedValue) !== -1) {
            valid = true;
        }
        return valid;
    }
    validResultRangeValue(specialOffer, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(specialOffer, filter.field);
        if ((filter.minValue === 0 && filter.maxValue === 0)
            || (value >= filter.minValue && value <= filter.maxValue)) {
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
    resetFilters(filters) {
        const updatedFilters = [];
        filters.forEach(filter => {
            const newFilter = filter;
            switch (filter.type) {
                case FILTER_TYPES.ID:
                case FILTER_TYPES.SUB_RESULTS_ID:
                case FILTER_TYPES.ARRAY:
                    newFilter.selectedValue = filter.defaultValue;
                    break;
                case FILTER_TYPES.RESULTS_RANGE:
                case FILTER_TYPES.SUB_RESULTS_RANGE:
                    newFilter.maxValue = newFilter.defaultMaxValue;
                    newFilter.minValue = newFilter.defaultMinValue;
                    break;
                default:
            }
            updatedFilters.push(newFilter);
        }
        );
        return updatedFilters;
    }
}

export default SpecialOffersApi;
