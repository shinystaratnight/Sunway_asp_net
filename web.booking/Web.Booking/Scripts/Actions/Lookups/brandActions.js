import * as types from './actionTypes';
import LookupAPI from 'api/lookupAPI';

/**
 * Redux Action method for requesting loading brands.
 * @return {object} The action type
 */
export function brandsRequest() {
    return { type: types.BRANDS_REQUEST };
}

/**
 * Redux Action method for successfully loading brands.
 * @param {object} brands - The brands object to return
 * @return {object} The action type and airports object
 */
export function brandsLoadSuccess(brands) {
    return { type: types.BRANDS_LOAD_SUCCESS, brands };
}

/**
 * Redux Action method for loading brands.
 * @return {object} Array of brands
 */
export function loadBrands() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(brandsRequest());
        return lookupApi.getLookup('booking/brand').then(brands => {
            dispatch(brandsLoadSuccess(brands));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if brands should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadBrands(state) {
    const brands = state.brands;
    const shouldLoad = brands && !brands.isFetching && !brands.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading brands if needed.
 * @return {object} Array of brands
 */
export function loadBrandsIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadBrands(getState())) {
            return dispatch(loadBrands());
        }
        return '';
    };
}
