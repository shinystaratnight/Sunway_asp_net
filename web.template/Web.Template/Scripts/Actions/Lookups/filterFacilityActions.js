import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading filter facilities.
 * @param {object} filterFacilities - The filter facilities object to return
 * @return {object} The action type and filter facilities object
 */
export function filterFacilitiesLoadSuccess(filterFacilities) {
    return { type: types.FILTERFACILITIES_LOAD_SUCCESS, filterFacilities };
}

/**
 * Redux Action method for requesting loading countries.
 * @return {object} The action type
 */
export function filterFacilitiesRequest() {
    return { type: types.FILTERFACILITIES_REQUEST };
}

/**
 * Redux Action method for loading countries.
 * @return {object} Array of countries
 */
export function loadFilterFacilities() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(filterFacilitiesRequest());
        return lookupApi.getLookup('property/filterfacility').then(filterFacilities => {
            dispatch(filterFacilitiesLoadSuccess(filterFacilities));
        }).catch(error => {
            throw (error);
        });
    };
}
