import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading brands.
 * @param {object} brands - The brands object to return
 * @return {object} The action type and airports object
 */
export function brandsLoadSuccess(brands) {
    return { type: types.BRANDS_LOAD_SUCCESS, brands };
}

/**
 * Redux Action method for loading airports.
 * @return {object} Array of airports
 */
export function loadBrands() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup('booking/brandandgeography').then(brands => {
            dispatch(brandsLoadSuccess(brands));
        }).catch(error => {
            throw (error);
        });
    };
}
