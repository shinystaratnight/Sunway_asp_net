import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading card types.
 * @param {object} cardTypes - The card types object to return
 * @return {object} The action type and card types object
 */
export function cardTypesLoadSuccess(cardTypes) {
    return { type: types.CARDTYPES_LOAD_SUCCESS, cardTypes };
}

/**
 * Redux Action method for requesting loading card types.
 * @return {object} The action type
 */
export function cardTypesRequest() {
    return { type: types.CARDTYPES_REQUEST };
}

/**
 * Redux Action method for loading card types.
 * @return {object} Array of card types
 */
export function loadCardTypes() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(cardTypesRequest());
        return lookupApi.getLookup('payment/creditcardtype').then(cardTypes => {
            dispatch(cardTypesLoadSuccess(cardTypes));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if card types should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadCardTypes(state) {
    const cardTypes = state.cardTypes;
    const shouldLoad = cardTypes && !cardTypes.isFetching && !cardTypes.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading card types if needed.
 * @return {object} Array of card types
 */
export function loadCardTypesIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadCardTypes(getState())) {
            return dispatch(loadCardTypes());
        }
        return '';
    };
}
