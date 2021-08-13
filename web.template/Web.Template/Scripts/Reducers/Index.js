import airportGroups from './lookups/airportGroupsReducer';
import airportResortGroups from './lookups/airportResortGroupsReducer';
import airports from './lookups/airportsReducer';
import alerts from './bookingjourney/alertsReducer';
import basket from './bookingjourney/basketReducer';
import blogList from './content/blogListReducer';
import brands from './lookups/brandsReducer';
import cmsContent from './content/cmscontentreducer';
import cmsWebsites from './bookingjourney/cmswebsitereducer';
import { combineReducers } from 'redux';
import countries from './lookups/countriesReducer';
import customQuery from './custom/customQueryReducer';
import dealFinderFlightCache from './lookups/dealFinderFlightCacheRouteDatesReducer';
import emailForm from './custom/emailFormReducer';
import entities from './content/entitiesReducer';
import entityList from './content/entityListReducer';
import filterFacilities from './lookups/filterFacilitiesReducer';
import flightCacheRouteDates from './lookups/flightCacheRouteDatesReducer';
import flightCarriers from './lookups/flightCarriersReducer';
import flightClasses from './lookups/flightClassesReducer';
import mealBasis from './lookups/mealBasisReducer';
import models from './content/ModelsReducer';
import page from './content/pageReducer';
import productAttributes from './lookups/productAttributesReducer';
import properties from './content/propertiesReducer';
import redirects from './content/RedirectReducer';
import routes from './lookups/routeAvailabilityReducer';
import search from './bookingjourney/searchReducer';
import searchResults from './bookingjourney/searchResultsReducer';
import session from './bookingjourney/userReducer';
import site from './bookingjourney/siteReducer';
import siteSearch from './content/siteSearchReducer';
import specialOffers from './custom/specialOffersReducer';
import twitter from './content/twitterReducer';

const rootReducer = combineReducers({
    airportGroups,
    airportResortGroups,
    airports,
    alerts,
    basket,
    blogList,
    brands,
    cmsContent,
    cmsWebsites,
    countries,
    customQuery,
    dealFinderFlightCache,
    entities,
    entityList,
    emailForm,
    filterFacilities,
    flightCacheRouteDates,
    flightCarriers,
    flightClasses,
    mealBasis,
    models,
    page,
    productAttributes,
    properties,
    redirects,
    routes,
    search,
    searchResults,
    site,
    siteSearch,
    session,
    specialOffers,
    twitter,
});

export default rootReducer;
