import * as types from './actionTypes';
import BasketAPI from 'api/basketAPI';

/**
 * Redux Action method for basket create request
 * @return {object} The action type and basket details object
 */
function basketCreateRequest() {
    return { type: types.BASKET_CREATE_REQUEST };
}

/**
 * Redux Action method for successfully creating a basket.
 * @param {string} token - The basket token to return
 * @return {object} The action type and basket token
 */
function basketCreateSuccess(token) {
    return { type: types.BASKET_CREATE_SUCCESS, token };
}

/**
 * Redux Action method for successfully loading basket details.
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
function basketLoadRequest(basket) {
    return { type: types.BASKET_LOAD_REQUEST, basket };
}

/**
 * Redux Action method for successfully loading basket details.
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
function basketLoadSuccess(basket) {
    return { type: types.BASKET_LOAD_SUCCESS, basket };
}

/**
 * Redux Action method for updating the basket
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
function basketUpdated(basket) {
    return { type: types.BASKET_UPDATED, basket };
}

/**
 * Redux Action method for updating the promocode values on the basket
 * @param {object} basket - The basket object to return
 * @return {object} The action type and basket details object
 */
function basketPromoCodeUpdated(basket) {
    return { type: types.BASKET_PROMOCODE_UPDATE, basket };
}

/**
 * Redux Action method for when we want to attempt to prebbook
 * @return {object} The action type
 */
function basketPrebookRequest() {
    return { type: types.BASKET_PREBOOK_REQUEST };
}

/**
 * Redux Action method for when the prebook succeeds
 * @param {string} prebookReturn - The prebook return, returned from the api.
 * @return {object} The action type and prebook return
 */
function basketPrebookSuccess(prebookReturn) {
    return { type: types.BASKET_PREBOOK_SUCCESS, prebookReturn };
}

/**
 * Redux Action method for when we want to attempt to prebbook a flight only
 * @return {object} The action type
 */
function basketPrebookFlightRequest() {
    return { type: types.BASKET_PREBOOK_FLIGHT_REQUEST };
}

/**
 * Redux Action method for when the prebook succeeds for a flight only
 * @return {object} The action type
 */
function basketPrebookFlightSuccess() {
    return { type: types.BASKET_PREBOOK_FLIGHT_SUCCESS };
}

/**
 * Redux Action method for when there is an error at prebook
 * @return {object} The action type
 */
function basketPrebookError() {
    return { type: types.BASKET_PREBOOK_ERROR };
}

/**
 * Redux Action method for when we want to attempt to book
 * @return {object} The action type
 */
function basketBookRequest() {
    return { type: types.BASKET_BOOK_REQUEST };
}

/**
 * Redux Action method for when the book succeeds
 * @param {string} bookReturn - The book return, returned from the api.
 * @return {object} The action type and book return
 */
function basketBookSuccess(bookReturn) {
    return { type: types.BASKET_BOOK_SUCCESS, bookReturn };
}

/**
 * Redux Action method for when there is an error at book
 * @param {array} warnings - The warnings returned from the book response
 * @return {object} The action type
 */
function basketBookError(warnings) {
    return { type: types.BASKET_BOOK_ERROR, warnings };
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
 * Redux Action method for retrieving cancellation charges
 * @param {int} cancellationCharges - The cancellation charges.
 * @return {object} The action type
 */
export function cancellationChargesRetrieved(cancellationCharges) {
    return { type: types.CANCELLATION_CHARGES_RETRIEVED, cancellationCharges };
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
 * Redux Action method for releasing seat lock on basket flight
 * @return {object} The action type
 */
function basketFlightSeatLockReleased() {
    return { type: types.BASKET_FLIGHT_SEAT_LOCK_RELEASED };
}


/**
 * Redux Action method for creating the basket.
 * @return {object} The basket
 */
export function createBasket() {
    return function load(dispatch) {
        dispatch(basketCreateRequest());
        const basketAPI = new BasketAPI();
        return basketAPI.createBasket()
            .then(basketToken => {
                dispatch(basketCreateSuccess(basketToken));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for loading the basket.
 * @param {object} token - The token unique to the basket
 * @return {object} The entity
 */
export function loadBasket(token) {
    return function load(dispatch) {
        dispatch(basketLoadRequest());
        const basketAPI = new BasketAPI();
        return basketAPI.loadBasket(token)
            .then(basket => {
                dispatch(basketLoadSuccess(basket));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
* Redux action method for adding a new component to the basket
* @param {object} componentModel - The component model
* @return {object} The updated basket
*/
export function basketAddComponent(componentModel) {
    return function load(dispatch) {
        if (componentModel.componentType === 'Flight') {
            dispatch(basketPrebookFlightRequest());
        }
        const basketAPI = new BasketAPI();
        return basketAPI.addComponent(componentModel)
            .then(basket => {
                if (componentModel.componentType === 'Flight') {
                    dispatch(basketPrebookFlightSuccess());
                }
                dispatch(basketUpdated(basket));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
* Redux action method for adding a new component to the basket
* @param {object} componentModel - The component model
* @return {object} The updated basket
*/
export function basketGetComponentCancellationCharges(componentModel) {
    return function load(dispatch) {
        const basketAPI = new BasketAPI();
        return basketAPI.getComponentCancellationCharges(componentModel)
            .then(cancellationCharges => {
                dispatch(cancellationChargesRetrieved(cancellationCharges));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
* Redux action method for removing a component from the basket
* @param {object} componentModel - The component model
* @return {object} The updated basket
*/
export function basketRemoveComponent(componentModel) {
    return function load(dispatch) {
        const basketAPI = new BasketAPI();
        return basketAPI.removeComponent(componentModel)
            .then(basket => {
                dispatch(basketUpdated(basket));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
* Redux action method for updating the selected extra on a flight component
* @param {string} componentToken - The component token
* @param {string} subComponentToken - The sub component token
* @param {number} quantity - The new quantity
* @return {object} The updated basket
*/
export function basketFlightUpdateExtra(componentToken, subComponentToken, quantity) {
    return (dispatch, getState) => {
        const state = getState();
        const basket = state.basket.basket;
        const basketAPI = new BasketAPI();
        return basketAPI.updateFlightExtra(basket, componentToken, subComponentToken, quantity)
            .then(updatedBasket => {
                dispatch(basketUpdated(updatedBasket));
            })
            .catch(error => {
                throw (error);
            }
            );
    };
}

/**
* Redux Action method for the basket prebook.
* @param {string} token - the token unique to the basket they want to prebook
* @return {object} The user
*/
export function basketPreBook(token) {
    return function load(dispatch) {
        dispatch(basketPrebookRequest());
        const basketAPI = new BasketAPI();
        return basketAPI.preBookBasket(token)
            .then(preBookReturn => {
                if (preBookReturn.Success) {
                    dispatch(basketPrebookSuccess(preBookReturn));
                } else {
                    // dispatch(basketPrebookError(failWarning));
                    dispatch(basketPrebookError('failed to pre book'));
                }
            });
    };
}

/**
* Redux Action method for the basket book.
* @param {string} token - the token unique to the basket they want to book
* @param {string} basket - the current client side basket, that will have values we need on it.
* @param {string} request - the hotel request
* @return {object} The user
*/
export function basketBook(token, basket, request) {
    return function load(dispatch) {
        dispatch(basketBookRequest());
        const basketAPI = new BasketAPI();
        return basketAPI.bookBasket(token, basket, request)
            .then(bookReturn => {
                if (bookReturn.ThreeDSecureEnrollment) {
                    window.location.href = `/booking/3dsecure/submit?baskettoken=${token}`;
                } else if (bookReturn.Success) {
                    if (bookReturn.OffsiteRedirect) {
                        const url = window.location.pathname;
                        window.location.href
                            = `${url.replace('/conditions', '/offsitepayment')}?t=${token}`;
                    } else {
                        dispatch(basketBookSuccess(bookReturn));
                        window.location.href = `/booking/confirmation?t=${token}`;
                    }
                } else {
                    dispatch(basketBookError(bookReturn.Warnings));
                    window.scrollTo(0, 0);
                }
            });
    };
}

/**
* Redux Action method for applying a promocode to the basket
* @param {string} basketToken - the token unique to the basket
* @param {string} promoCode - the promocode itself.
* @return {object} The updated basket
*/
export function basketApplyPromoCode(basketToken, promoCode) {
    return function load(dispatch) {
        const basketAPI = new BasketAPI();
        return basketAPI.applyPromoCode(basketToken, promoCode)
            .then(promoReturn => {
                if (!promoReturn.Success) {
                    dispatch(addWarning('PromoCode', 'Invalid Code'));
                }
                dispatch(basketPromoCodeUpdated(promoReturn.Basket));
            });
    };
}

/**
* Redux Action method for removing a promocode from the basket
* @param {string} basketToken - the token unique to the basket
* @return {object} The updated basket
*/
export function basketRemovePromoCode(basketToken) {
    return function load(dispatch) {
        const basketAPI = new BasketAPI();
        return basketAPI.removePromoCode(basketToken)
            .then(basket => {
                dispatch(basketPromoCodeUpdated(basket));
            });
    };
}

/**
* Redux Action method for releasing a flight seat lock
* @param {string} basketToken - the token unique to the basket
* @return {object} The updated basket
*/
export function basketReleaseFlightSeatLock(basketToken) {
    return function load(dispatch) {
        const basketAPI = new BasketAPI();
        return basketAPI.releaseFlightSeatLock(basketToken)
            .then(response => {
                if (response.ok) {
                    dispatch(basketFlightSeatLockReleased());
                }
            });
    };
}
