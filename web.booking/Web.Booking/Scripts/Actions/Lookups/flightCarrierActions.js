import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading flight carriers.
 * @param {object} flightCarriers - The flight carriers object to return
 * @return {object} The action type and flight carriers object
 */
export function flightCarriersLoadSuccess(flightCarriers) {
    return { type: types.FLIGHTCARRIERS_LOAD_SUCCESS, flightCarriers };
}

/**
 * Redux Action method for requesting loading flight carriers.
 * @return {object} The action type
 */
export function flightCarriersRequest() {
    return { type: types.FLIGHTCARRIERS_REQUEST };
}

/**
 * Redux Action method for loading flight carriers.
 * @return {object} Array of countries
 */
export function loadFlightCarriers() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(flightCarriersRequest());
        return lookupApi.getLookup('flightcarrier').then(flightCarriers => {
            dispatch(flightCarriersLoadSuccess(flightCarriers));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if flight carriers should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadFlightCarriers(state) {
    const flightCarriers = state.flightCarriers;
    const shouldLoad = flightCarriers && !flightCarriers.isFetching && !flightCarriers.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading countries if needed.
 * @return {object} Array of airports
 */
export function loadFlightCarriersIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadFlightCarriers(getState())) {
            return dispatch(loadFlightCarriers());
        }
        return '';
    };
}
