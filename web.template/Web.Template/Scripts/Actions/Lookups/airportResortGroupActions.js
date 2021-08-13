import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading airports.
 * @param {object} airportResortGroups - The airportResortGroups object to return
 * @return {object} The action type and airportResortGroups object
 */
export function airportResortGroupsLoadSuccess(airportResortGroups) {
    return { type: types.AIRPORT_RESORT_GROUPS_LOAD_SUCCESS, airportResortGroups };
}

/**
 * Redux Action method for requesting loading airports.
 * @return {object} The action type
 */
export function airportResortGroupsRequest() {
    return { type: types.AIRPORT_RESORT_GROUPS_REQUEST };
}

/**
 * Redux Action method for loading airports.
 * @return {object} Array of airports
 */
export function loadAirportResortGroups() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup('airportResortGroup').then(airportResortGroups => {
            dispatch(airportResortGroupsLoadSuccess(airportResortGroups));
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
function shouldLoadAirportResortGroups(state) {
    const airportResortGroups = state.airportResortGroups;
    const shouldLoad = airportResortGroups
        && !airportResortGroups.isFetching && !airportResortGroups.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading airports if needed.
 * @return {object} Array of airports
 */
export function loadAirportResortGroupsIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadAirportResortGroups(getState())) {
            return dispatch(loadAirportResortGroups());
        }
        return '';
    };
}
