import airportGroups from './lookups/airportGroupsReducer';
import airportResortGroups from './lookups/airportResortGroupsReducer';
import airports from './lookups/airportsReducer';
import alerts from './bookingjourney/alertsReducer';
import basket from './bookingjourney/basketReducer';
import booking from './bookingjourney/bookingReducer';
import bookingCountries from './lookups/bookingCountriesReducer';
import brands from './lookups/brandsReducer';
import cardTypes from './lookups/cardTypesReducer';
import { combineReducers } from 'redux';
import countries from './lookups/countriesReducer';
import dealFinderFlightCache from './lookups/dealFinderFlightCacheRouteDatesReducer';
import entities from './content/entitiesReducer';
import entityList from './content/entityListReducer';
import filterFacilities from './lookups/filterFacilitiesReducer';
import flightCacheRouteDates from './lookups/flightCacheRouteDatesReducer';
import flightCarriers from './lookups/flightCarriersReducer';
import flightClasses from './lookups/flightClassesReducer';
import mealBasis from './lookups/mealBasisReducer';
import models from './content/modelsReducer';
import page from './content/pageReducer';
import productAttributes from './lookups/productAttributesReducer';
import properties from './content/propertiesReducer';
import quote from './bookingjourney/quoteReducer';
import routes from './lookups/routeAvailabilityReducer';
import search from './bookingjourney/searchReducer';
import searchResults from './bookingjourney/searchResultsReducer';
import session from './bookingjourney/userReducer';
import site from './bookingjourney/siteReducer';
import siteSearch from './content/siteSearchReducer';

const rootReducer = combineReducers({
    airportGroups,
    airportResortGroups,
    airports,
    alerts,
    basket,
    booking,
    bookingCountries,
    brands,
    cardTypes,
	countries,
	dealFinderFlightCache,
    entities,
    entityList,
    filterFacilities,
    flightCacheRouteDates,
    flightCarriers,
    flightClasses,
    mealBasis,
    models,
    page,
    productAttributes,
    properties,
    quote,
    routes,
    search,
    searchResults,
    site,
    siteSearch,
    session,
});

export default rootReducer;
