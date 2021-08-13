import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading mealbasis.
 * @param {object} mealBasis - The mealbasis object to return
 * @return {object} The action type and mealbasis object
 */
export function mealBasisLoadSuccess(mealBasis) {
    return { type: types.MEALBASIS_LOAD_SUCCESS, mealBasis };
}

/**
 * Redux Action method for requesting loading mealbasis.
 * @return {object} The action type
 */
export function mealBasisRequest() {
    return { type: types.MEALBASIS_REQUEST };
}

/**
 * Redux Action method for loading mealbasis.
 * @return {object} Array of mealbasis
 */
export function loadMealBasis() {
    return function load(dispatch) {
        dispatch(mealBasisRequest());
        const lookupApi = new LookupAPI();
        return lookupApi.getLookup('property/mealbasis').then(mealbasis => {
            dispatch(mealBasisLoadSuccess(mealbasis));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if mealbasis should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadMealBasis(state) {
    const mealBasis = state.mealBasis;
    const shouldLoad = mealBasis && !mealBasis.isFetching && !mealBasis.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading mealbasis if needed.
 * @return {object} Array of mealbasis
 */
export function loadMealBasisIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadMealBasis(getState())) {
            return dispatch(loadMealBasis());
        }
        return '';
    };
}
