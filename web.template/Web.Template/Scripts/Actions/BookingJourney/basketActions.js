import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for successfully loading basket details.
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
export function loadBasketRequest(basket) {
    return { type: types.BASKET_LOAD_REQUEST, basket };
}

/**
 * Redux Action method for successfully loading basket details.
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
export function basketLoadSuccess(basket) {
    return { type: types.BASKET_LOAD_SUCCESS, basket };
}

/**
 * Redux Action method for loading the basket.
 * @param {object} userToken - The token unique to the user
 * @param {object} basketToken - The token unique to the basket
 * @return {object} The entity
 */
export function loadBasket(userToken, basketToken) {
    return function load(dispatch) {
        dispatch(loadBasketRequest());
        const basketURL = `/api/basket/${userToken}/${basketToken}`;
        return fetch(basketURL).then(response => response.json()).then(basket =>
            dispatch(basketLoadSuccess(basket)));
    };
}

/**
 * Redux Action method for updating the basket
 * @param {string} field - The field on the basket we're looking to update
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateBasketValue(field, value) {
    return { type: types.BASKET_UPDATE_VALUE, field, value };
}

/**
 * Redux Action method for updating the state values basket
 * @param {string} field - The field on the basket we're looking to update
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateBasketStateValue(field, value) {
    return { type: types.BASKET_UPDATE_STATE_VALUE, field, value };
}
/**
 * Redux Action method for updating the terms and conditions
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateTermsAndConditions(value) {
    return { type: types.BASKET_UPDATE_TERMS, value };
}

/**
 * Redux Action method for updating the errata
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateErrata(value) {
    return { type: types.BASKET_UPDATE_ERRATA, value };
}

/**
 * Redux Action method for updating the promocode
 * @param {string} basket - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updatePromocode(basket) {
    return { type: types.BASKET_UPDATE_PROMOCODE, basket };
}

/**
 * Redux Action method for updating whether we require a trade reference
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateTradeReferenceRequirement(value) {
    return { type: types.BASKET_UPDATE_REFERENCE_REQUIREMENT, value };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateCancellation(value) {
    return { type: types.BASKET_UPDATE_CANCELLATION, value };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateRequest(value) {
    return { type: types.BASKET_UPDATE_REQUEST, value };
}

/**
 * Redux Action method for adding a warning.
 * @param {string} key - The key assoaciated with that warning e.g. 'lead guest details'.
 * @param {string} value - The value of the warning e.g. 'this field is blank'.
 * @return {object} The updated basket
 */
export function addWarning(key, value) {
    return { type: types.BASKET_ADD_WARNING, key, value };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function resetWarnings() {
    return { type: types.BASKET_RESET_WARNINGS };
}

/**
 * Redux Action method for when there is an error at book
 * @param {string} warning - The warning we want to raise.
 * @return {object} The updated basket
 */
export function bookError(warning) {
    return { type: types.BASKET_BOOK_ERROR, warning };
}

/**
 * Redux Action method for when we want to attempt to book
 * @return {object} The updated basket
 */
export function bookRequest() {
    return { type: types.BASKET_BOOK_REQUEST };
}

/**
 * Redux Action method for when there is an error at prebook
 * @param {string} warning - The warning we want to raise.
 * @return {object} The updated basket
 */
export function prebookError(warning) {
    return { type: types.BASKET_PREBOOK_ERROR, warning };
}

/**
 * Redux Action method for when we want to attempt to prebbook
 * @return {object} The updated basket
 */
export function prebookRequest() {
    return { type: types.BASKET_PREBOOK_REQUEST };
}

/**
 * Redux Action method for when the prebook succeeds
 * @return {object} The updated basket
 */
export function prebookSuccess() {
    return { type: types.BASKET_PREBOOK_SUCCESS };
}

/**
 * Redux Action method for when the book succeeds
 * @param {string} bookReturn - The book return, returned from the api.
 * @return {object} The updated basket
 */
export function bookSuccess(bookReturn) {
    return { type: types.BASKET_BOOK_SUCCESS, bookReturn };
}

/**
 * Redux Action method for when we remove a promocode
 * @param {string} basket - The updated basket.
 * @return {object} The updated basket
 */
export function removePromoCode(basket) {
    return { type: types.BASKET_REMOVE_PROMOCODE, basket };
}

/**
* Redux Action method for the basket book.
* @param {string} userToken - the token unique to that user
* @param {string} basketToken - the token unique to the basket they want to book
* @param {string} basket - the current client side basket, that will have values we need on it.
* @param {string} request - the hotel request
* @param {string} failWarning - the warning to dispatch if this goes wrong
* @return {object} The user
*/
export function basketBook(userToken, basketToken, basket, request, failWarning) {
    const basketURL = `/api/basket/book/${userToken}/${basketToken}`;
    const basketModel = {
        GuestDetails: basket.Rooms,
        TradeReference: basket.TradeReference,
        HotelRequest: request,
        LeadGuest: basket.LeadGuest,
    };

    const fetchOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
        method: 'Post',
        body: JSON.stringify(basketModel),
    };
    return function load(dispatch) {
        dispatch(bookRequest());
        return fetch(basketURL, fetchOptions).then(response => response.json()).then(bookReturn => {
            if (bookReturn.Success) {
                dispatch(bookSuccess(bookReturn));
                window.location.href = '/confirmation';
            } else {
                dispatch(bookError(failWarning));
                window.scrollTo(0, 0);
            }
        });
    };
}

/**
* Redux Action method for the basket prebook.
* @param {string} userToken - the token unique to that user
* @param {string} basketToken - the token unique to the basket they want to prebook
* @param {string} failWarning - the warning to dispatch if this goes wrong
* @return {object} The user
*/
export function basketPreBook(userToken, basketToken, failWarning) {
    const basketURL = `/api/basket/prebook/${userToken}/${basketToken}`;

    const fetchOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
        method: 'Post',
    };
    return function load(dispatch) {
        dispatch(prebookRequest());
        return fetch(basketURL, fetchOptions).then(
            response => response.json()).then(prebookReturn => {
                if (prebookReturn.Success) {
                    dispatch(prebookSuccess(prebookReturn));

                    let redirectUrl = '/payment';
                    if (prebookReturn.PriceChange !== 0) {
                        redirectUrl += `?pricechange=${prebookReturn.PriceChange}`;
                    }

                    window.location.href = redirectUrl;
                } else {
                    dispatch(prebookError(failWarning));
                    window.scrollTo(0, 0);
                }
            });
    };
}

/**
* Redux Action method for applying a promocode to the basket
* @param {string} userToken - the token unique to that user
* @param {string} basketToken - the token unique to the basket they want to book
* @param {string} promoCode - the promocode itself.
* @return {object} The user
*/
export function basketApplyPromoCode(userToken, basketToken, promoCode) {
    const basketURL = 'api/basket/promocode/add';
    const changeBasketModel = {
        BasketToken: basketToken,
        UserToken: userToken,
        PromoCode: promoCode,
    };

    const fetchOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
        method: 'Post',
        body: JSON.stringify(changeBasketModel),
    };
    return function load(dispatch) {
        return fetch(basketURL, fetchOptions).then(
            response => response.json()).then(promoReturn => {
                if (promoReturn.Success) {
                    dispatch(updatePromocode(promoReturn.Basket));
                } else {
                    dispatch(addWarning('PromoCode', 'Invalid Code'));
                    dispatch(updatePromocode(promoReturn.Basket));
                }
            });
    };
}

/**
* Redux Action method for removing a promocode from the basket
* @param {string} userToken - the token unique to that user
* @param {string} basketToken - the token unique to the basket they want to book
* @return {object} The user
*/
export function basketRemovePromoCode(userToken, basketToken) {
    const basketURL = 'api/basket/promocode/remove';
    const changeBasketModel = {
        BasketToken: basketToken,
        UserToken: userToken,
    };

    const fetchOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
        method: 'Post',
        body: JSON.stringify(changeBasketModel),
    };
    return function load(dispatch) {
        return fetch(basketURL, fetchOptions).then(response => response.json()).then(basket => {
            dispatch(updatePromocode(basket));
        });
    };
}
