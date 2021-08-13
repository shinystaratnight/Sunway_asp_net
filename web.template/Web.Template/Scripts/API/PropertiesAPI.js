import ArrayFunctions from '../library/arrayfunctions';

import fetch from 'isomorphic-fetch';

class PropertiesApi {
    constructor() {
        this.properties = [];
        this.totalPages = 1;
        this.resultsPerPage = 10;
        this.selectedSort = 'Default';
    }
    setupProperties(options) {
        this.resultsPerPage = options.resultsPerPage;
        return this.getProperties(options.params)
            .then(() => this.setupPaging(this.properties, options.resultsPerPage))
            .then(() => this.addSpecialOfferData(this.properties))
            .then(() => this.sortProperties(this.properties, this.selectedSort))
            .then((sortedProperties) => {
                const properties = {
                    items: sortedProperties,
                    totalPages: this.totalPages,
                    resultsPerPage: this.resultsPerPage,
                    selectedSort: this.selectedSort,
                };
                return properties;
            });
    }
    addSpecialOfferData(properties) {
        const propertiesWithComparison = [];
        properties.forEach(p => {
            let comparisonPrice = p.hasOwnProperty('SpecialOffer')
                && p.SpecialOffer.hasOwnProperty('PerPersonPrice')
                ? p.SpecialOffer.PerPersonPrice
                : p.FromPrice;
            comparisonPrice = parseFloat(comparisonPrice) > 0 ? comparisonPrice : 'Unset';
            const addedDate = p.hasOwnProperty('SpecialOffer')
                ? p.SpecialOffer.StartPublishDate : '';
            const propertyWithComparison = Object.assign({
                ComparisonPrice: comparisonPrice,
                AddedDate: addedDate,
            }, p);
            propertiesWithComparison.push(propertyWithComparison);
        });
        this.properties = propertiesWithComparison;
    }
    getProperties(params) {
        let apiURL = '/api/customquery?';

        apiURL += 'queryName=Properties%20By%20GeoID';
        apiURL += '&parameterCsv=';
        params.forEach(param => {
            apiURL += `${param},`;
        });

        return fetch(apiURL, { credentials: 'same-origin' })
            .then(response => response.json())
            .then(result => {
                let properties = [];
                if (result.CustomQueryResponse.CustomXML) {
                    properties = result.CustomQueryResponse.CustomXML.Properties.Property;
                }
                const propertyArray = Array.isArray(properties) ? properties : [properties];
                this.properties = propertyArray;
                return properties;
            });
    }
    setupPaging(properties, resultsPerPage) {
        const totalPages = Math.ceil(properties.length / resultsPerPage);
        this.resultsPerPage = resultsPerPage;
        this.totalPages = totalPages;
    }
    sortPropertiesByPrice(properties) {
        const propertiesWithPrices = properties.filter(p => p.ComparisonPrice !== 'Unset');
        const propertiesWithoutPrices = properties.filter(p => p.ComparisonPrice === 'Unset');
        const sortedProperties
            = ArrayFunctions.sortByPropertyAscending(propertiesWithPrices, 'ComparisonPrice');
        return sortedProperties.concat(propertiesWithoutPrices);
    }
    sortPropertiesByPriceDescending(properties) {
        const propertiesWithPrices = properties.filter(p => p.ComparisonPrice !== 'Unset');
        const propertiesWithoutPrices = properties.filter(p => p.ComparisonPrice === 'Unset');
        const sortedProperties
            = ArrayFunctions.sortByPropertyDescending(propertiesWithPrices, 'ComparisonPrice');
        return sortedProperties.concat(propertiesWithoutPrices);
    }
    sortPropertiesByName(properties) {
        const sortedProperties
            = ArrayFunctions.sortByPropertyAscending(properties, 'HotelName');
        return sortedProperties;
    }
    sortPropertiesByRating(properties) {
        const propertiesWithRatings = properties.filter(p =>
            typeof p.HotelRating !== 'undefined' && p.HotelRating !== '0.0');
        const propertiesWithoutRatings = properties.filter(p =>
            typeof p.HotelRating === 'undefined' || p.HotelRating === '0.0');
        const sortedProperties
            = ArrayFunctions.sortByPropertyAscending(propertiesWithRatings, 'HotelRating');
        return sortedProperties.concat(propertiesWithoutRatings);
    }
    sortPropertiesByRatingDescending(properties) {
        const propertiesWithRatings = properties.filter(p => p.HotelRating !== '0.0');
        const propertiesWithoutRatings = properties.filter(p => p.HotelRating === '0.0');
        const sortedProperties
            = ArrayFunctions.sortByPropertyDescending(propertiesWithRatings, 'HotelRating');
        return sortedProperties.concat(propertiesWithoutRatings);
    }
    sortPropertiesByOfferDate(properties) {
        return ArrayFunctions.sortByPropertyDescending(properties, 'AddedDate');
    }
    sortPropertiesByDefault(properties) {
        const priceSortedProperties = this.sortPropertiesByPrice(properties);
        const chosenByUsProperties = priceSortedProperties.filter(p => p.ChosenByUs === '1');
        const notChosenByUsProperties = priceSortedProperties.filter(p => p.ChosenByUs !== '1');
        const specialOfferProperties = notChosenByUsProperties
            .filter(p => p.hasOwnProperty('SpecialOffer'));
        const dateSortedSpecialOfferProperties
            = this.sortPropertiesByOfferDate(specialOfferProperties);
        const otherProperties = notChosenByUsProperties
            .filter(p => !p.hasOwnProperty('SpecialOffer'));
        return chosenByUsProperties.concat(dateSortedSpecialOfferProperties)
            .concat(otherProperties);
    }
    sortProperties(properties, selectedSort) {
        let sortedProperties = this.sortPropertiesByDefault(properties);
        switch (selectedSort.toLowerCase()) {
            case 'priceascending':
                sortedProperties = this.sortPropertiesByPrice(properties);
                break;
            case 'pricedescending':
                sortedProperties = this.sortPropertiesByPriceDescending(properties);
                break;
            case 'nameascending':
                sortedProperties = this.sortPropertiesByName(properties);
                break;
            case 'namedescending':
                sortedProperties = this.sortPropertiesByName(properties).reverse();
                break;
            case 'starascending':
                sortedProperties = this.sortPropertiesByRating(properties);
                break;
            case 'stardescending':
                sortedProperties = this.sortPropertiesByRatingDescending(properties);
                break;
            default:
        }
        return sortedProperties;
    }
}

export default PropertiesApi;
