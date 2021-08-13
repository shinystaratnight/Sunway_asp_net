import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading airport routes.
 * @param {object} routes - The routes object to return
 * @return {object} The action type and airports object
 */
export function routesLoadSuccess(routes) {
    return { type: types.ROUTEAVAILABILITY_LOAD_SUCCESS, routes };
}

/**
 * Redux Action method for loading airport routes.
 * @return {object} Array of route availability
 */
export function loadRoutes() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup('RouteAvailability').then(routes => {
            dispatch(routesLoadSuccess(routes));
        }).catch(error => {
            throw (error);
        });
    };
}
