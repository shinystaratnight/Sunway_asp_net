/* eslint-disable complexity */

import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isLoaded: false,
    isLoading: false,
    isPreBooking: false,
    isBooking: false,
    acceptedTAndC: false,
    acceptedErrata: false,
    acceptedCancellation: false,
    requiresTradeReference: false,
    requiresLeadGuest: false,
    priceIncreaseAmount: 0,
    hasAcceptedPriceIncrease: false,
    hotelRequest: '',
    workingPromoCode: '',
    basket: {
        PreBooked: false,
        Compononents: [],
    },
    warnings: {
    },
};

/**
 * takes an object the name of a field and a value and scans through
 * the model looking to update the field
 * @param {object} obj - The current state.
 * @param {string} field - The field we're looking to update.
 * @param {string} val - the value we will assign it.
 * @return {val} the updated object.
 */
function setObjectByString(obj, field, val) {
    let path = field.replace(/\[(\w+)\]/g, '.$1');
    path = path.replace(/^\./, '');
    const fields = path.split('.');
    let model = obj;
    while (fields.length > 1) {
        model = model[fields.shift()];
    }
    model[fields.shift()] = val;
    return obj;
}

/**
 * Redux Reducer for basket
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function basketReducer(state = initialState, action) {
    let updatedBasket = {};
    const updatedWarnings = state.warnings;
    switch (action.type) {
        case types.BASKET_UPDATE_STATE_VALUE:
            setObjectByString(state, action.field, action.value);
            return Object.assign({}, state);
        case types.BASKET_UPDATE_VALUE:
            updatedBasket = setObjectByString(state.basket, action.field, action.value);
            return Object.assign({}, state, {
                isLoaded: true,
                basket: updatedBasket,
            });
        case types.BASKET_UPDATE_TERMS:
            return Object.assign({}, state, {
                acceptedTAndC: action.value,
            });
        case types.BASKET_UPDATE_REQUEST:
            return Object.assign({}, state, {
                hotelRequest: action.value,
            });
        case types.BASKET_ADD_WARNING:
            updatedWarnings[action.key] = action.value;
            return Object.assign({}, state, {
                warnings: updatedWarnings,
            });
        case types.BASKET_RESET_WARNINGS:
            return Object.assign({}, state, {
                warnings: {},
            });
        case types.BASKET_BOOK_ERROR:
            return Object.assign({}, state, {
                warnings: {},
                isBooking: false,
            });
        case types.BASKET_BOOK_SUCCESS:
            return Object.assign({}, state, {
                warnings: {},
                isBooking: false,
            });
        case types.BASKET_BOOK_REQUEST:
            return Object.assign({}, state, {
                isBooking: true,
            });
        case types.BASKET_UPDATE_CANCELLATION:
            return Object.assign({}, state, {
                acceptedCancellation: action.value,
            });
        case types.BASKET_UPDATE_ERRATA:
            return Object.assign({}, state, {
                acceptedErrata: action.value,
            });
        case types.BASKET_UPDATE_PROMOCODE:
            updatedBasket = state.basket;
            updatedBasket.PromoCode = action.basket.PromoCode;
            updatedBasket.PromoCodeDiscount = action.basket.PromoCodeDiscount;
            updatedBasket.TotalPrice = action.basket.TotalPrice;
            return Object.assign({}, state, {
                basket: updatedBasket,
            });
        case types.BASKET_UPDATE_REFERENCE_REQUIREMENT:
            return Object.assign({}, state, {
                requiresTradeReference: action.value,
            });
        case types.BASKET_LOAD_REQUEST:
            return Object.assign({}, state, {
                isLoading: true,
            });
        case types.BASKET_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isLoading: false,
                isLoaded: true,
                basket: action.basket,
            });
        case types.BASKET_PREBOOK_REQUEST:
            return Object.assign({}, state, {
                isPreBooking: true,
            });
        case types.BASKET_PREBOOK_ERROR:
            return Object.assign({}, state, {
                isPreBooking: false,
            });
        case types.BASKET_PREBOOK_SUCCESS:
            return Object.assign({}, state, {
                isPreBooking: false,
            });
        default:
            return state;
    }
}
