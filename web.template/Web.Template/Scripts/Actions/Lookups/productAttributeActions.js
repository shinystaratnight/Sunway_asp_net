import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading productAttributes.
 * @param {object} productAttributes - The productAttributes object to return
 * @return {object} The action type and productAttributes object
 */
export function productAttributesLoadSuccess(productAttributes) {
    return { type: types.PRODUCTATTRIBUTES_LOAD_SUCCESS, productAttributes };
}

/**
 * Redux Action method for requesting loading productAttributes.
 * @return {object} The action type
 */
export function productAttributesRequest() {
    return { type: types.PRODUCTATTRIBUTES_REQUEST };
}

/**
 * Redux Action method for loading productAttributes.
 * @return {object} Array of productAttributes
 */
export function loadProductAttributes() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        return lookupApi.getLookup('property/productAttribute').then(productAttributes => {
            dispatch(productAttributesLoadSuccess(productAttributes));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if productAttributes should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadProductAttributes(state) {
    const productAttributes = state.productAttributes;
    const shouldLoad = productAttributes
                        && !productAttributes.isFetching
                        && !productAttributes.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading productAttributes if needed.
 * @return {object} Array of productAttributes
 */
export function loadProductAttributesIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadProductAttributes(getState())) {
            return dispatch(loadProductAttributes());
        }
        return '';
    };
}
