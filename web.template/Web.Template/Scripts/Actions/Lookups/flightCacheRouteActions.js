import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully flight cache route dates.
 * @param {string} key - The unique key
 * @param {object} dates - An array of dates
 * @return {object} The action type, key and dates
 */
function flightCacheRouteLoadDatesSuccess(key, dates) {
    return { type: types.FLIGHTCACHEROUTE_LOAD_DATES_SUCCESS, key, dates };
}

/**
 * Redux Action method for requesting flight cache route dates.
 * @param {string} key - The unique key
 * @return {object} The action type and key
 */
function flightCacheRouteLoadDatesRequest(key) {
    return { type: types.FLIGHTCACHEROUTE_LOAD_DATES_REQUEST, key };
}

/**
 * Method to determine if dates should be loaded
 * @param {string} key - The unique key
 * @param {object} state - The current state
 * @return {boolean} The result
 */
function shouldLoadDates(key, state) {
    const flightCacheRouteDates = state.flightCacheRouteDates;
    const item = flightCacheRouteDates.items[key];
    const shouldLoad = !item || (!item.isFetching && !item.isLoaded);
    return shouldLoad;
}

/**
 * Method to load flight cache route dates.
 * @param {string} key - The unique key
 * @param {number} departureAirportId - The departure airport id
 * @param {string} arrivalType - The arrival type
 * @param {number} arrivalId - The arrival id
 * @return {object} The action type and key
 */
function loadDates(key, departureAirportId, arrivalType, arrivalId) {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(flightCacheRouteLoadDatesRequest(key));
        const lookupUrl = `flightcacheroute/${arrivalType}/${departureAirportId}/${arrivalId}`;
        return lookupApi.getLookup(lookupUrl)
            .then(dates => {
                dispatch(flightCacheRouteLoadDatesSuccess(key, dates));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for requesting flight cache route dates.
 * @param {number} departureAirportId - The departure airport id
 * @param {string} arrivalType - The arrival type
 * @param {number} arrivalId - The arrival id
 * @return {object} The action type and key
 */
export function loadFlightCacheRouteDates(departureAirportId, arrivalType, arrivalId) {
    const key = `${arrivalType}_${departureAirportId}_${arrivalId}`;
    return (dispatch, getState) => {
        if (shouldLoadDates(key, getState())) {
            return dispatch(loadDates(key, departureAirportId, arrivalType, arrivalId));
        }
        return '';
    };
}
