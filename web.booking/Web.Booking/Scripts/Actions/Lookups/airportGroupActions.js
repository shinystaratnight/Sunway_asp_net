import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading airports.
 * @param {object} airportGroups - The airportGroups object to return
 * @return {object} The action type and airports object
 */
export function airportsLoadSuccess(airportGroups) {
    return { type: types.AIRPORT_GROUPS_LOAD_SUCCESS, airportGroups };
}

/**
 * Redux Action method for loading airports.
 * @param {object} groupByAirportGroup - group airports into airport groups
 * @param {object} sortByPreferred - Sort airports by the preferred parameter
 * @return {object} Array of airports
 */
export function loadAirportGroups(groupByAirportGroup, sortByPreferred) {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup(`AirportsByGroup/${groupByAirportGroup}/${sortByPreferred}`)
                .then(airportGroups => {
                    dispatch(airportsLoadSuccess(airportGroups));
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
function shouldLoadAirportGroups(state) {
    const airportGroups = state.airportGroups;
    const shouldLoad = airportGroups && !airportGroups.isFetching && !airportGroups.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading airports if needed.
 * @param {object} groupByAirportGroup - group airports into airport groups
 * @param {object} sortByPreferred - Sort airports by the preferred parameter
 * @return {object} Array of airports
 */
export function loadAirportGroupsIfNeeded(groupByAirportGroup, sortByPreferred) {
    return (dispatch, getState) => {
        if (shouldLoadAirportGroups(getState())) {
            return dispatch(loadAirportGroups(groupByAirportGroup, sortByPreferred));
        }
        return '';
    };
}
