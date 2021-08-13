import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading airports.
 * @param {object} airports - The airports object to return
 * @return {object} The action type and airports object
 */
export function airportsLoadSuccess(airports) {
    return { type: types.AIRPORTS_LOAD_SUCCESS, airports };
}

/**
 * Redux Action method for requesting loading airports.
 * @return {object} The action type
 */
export function airportsRequest() {
    return { type: types.AIRPORTS_REQUEST };
}

/**
 * Redux Action method for loading airports.
 * @return {object} Array of airports
 */
export function loadAirports() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup('airport').then(airports => {
            dispatch(airportsLoadSuccess(airports));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if airports should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadAirports(state) {
    const airports = state.airports;
    const shouldLoad = airports && !airports.isFetching && !airports.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading airports if needed.
 * @return {object} Array of airports
 */
export function loadAirportsIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadAirports(getState())) {
            return dispatch(loadAirports());
        }
        return '';
    };
}
