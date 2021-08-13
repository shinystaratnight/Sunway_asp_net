import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading flight classes.
 * @param {object} flightClasses - The flight classes object to return
 * @return {object} The action type and flight classes object
 */
export function flightClassesLoadSuccess(flightClasses) {
    return { type: types.FLIGHTCLASSES_LOAD_SUCCESS, flightClasses };
}

/**
 * Redux Action method for requesting loading flight classes.
 * @return {object} The action type
 */
export function flightClassesRequest() {
    return { type: types.FLIGHTCLASSES_REQUEST };
}

/**
 * Redux Action method for loading flight classes.
 * @return {object} Array of flight classes
 */
export function loadFlightClasses() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(flightClassesRequest());
        return lookupApi.getLookup('flightclass').then(flightClasses => {
            dispatch(flightClassesLoadSuccess(flightClasses));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if flight classes should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadFlightClasses(state) {
    const flightClasses = state.flightClasses;
    const shouldLoad = flightClasses && !flightClasses.isFetching && !flightClasses.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading flight classes if needed.
 * @return {object} Array of flight classes
 */
export function loadFlightClassesIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadFlightClasses(getState())) {
            return dispatch(loadFlightClasses());
        }
        return '';
    };
}
