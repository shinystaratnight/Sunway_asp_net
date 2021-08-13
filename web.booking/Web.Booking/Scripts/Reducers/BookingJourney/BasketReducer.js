/* eslint-disable complexity */

import * as types from '../../actions/bookingjourney/actionTypes';
import ObjectFunctions from 'library/objectfunctions';

const initialState = {
    isLoaded: false,
    isLoading: false,
    isPreBooking: false,
    isPreBooked: false,
    isPreBookFailed: false,
    isPreBookingFlight: false,
    isBooking: false,
    isBookingFailed: false,
    isPaymentFailed: false,
    isCreatingQuote: false,
    isQuoteCreated: false,
    acceptedTAndC: false,
    acceptedErrata: false,
    acceptedCancellation: false,
    requiresTradeReference: false,
    requiresLeadGuest: false,
    requiresPaymentDetails: false,
    priceChangeAmount: 0,
    hasAcceptedPriceIncrease: false,
    hotelRequest: '',
    workingPromoCode: '',
    basket: {
        AllComponentsPreBooked: false,
        BasketToken: '',
        Components: [],
    },
    cancellationCharges: {
    },
    warnings: {
    },
};

/**
 * Check for payment failure warnings
 * @param {array} warnings - The book response warnings
 * @return {boolean} whether or not payment failed
 */
function checkPaymentWarning(warnings) {
    let paymentFailed = false;
    for (let i = 0; i < warnings.length; i++) {
        const warning = warnings[i];
        if (warning.toLowerCase().indexOf('card') !== -1
                || warning.toLowerCase().indexOf('declined') !== -1) {
            paymentFailed = true;
            continue;
        }
    }
    return paymentFailed;
}

/**
 * Redux Reducer for basket
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function basketReducer(state = initialState, action) {
    switch (action.type) {
        case types.BASKET_CREATE_REQUEST:
            return Object.assign({}, state, {
                isLoading: true,
                isLoaded: false,
            });
        case types.BASKET_CREATE_SUCCESS: {
            const updatedBasket = Object.assign({}, state.basket);
            updatedBasket.BasketToken = action.token;
            return Object.assign({}, state, {
                isLoading: false,
                isLoaded: true,
                basket: updatedBasket,
            });
        }
        case types.BASKET_LOAD_REQUEST:
            return Object.assign({}, state, {
                isLoading: true,
                isLoaded: false,
            });
        case types.BASKET_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isLoading: false,
                isLoaded: true,
                basket: action.basket,
            });
        case types.BASKET_UPDATED:
            return Object.assign({}, state, {
                basket: action.basket,
            });
        case types.BASKET_PROMOCODE_UPDATE: {
            const updatedBasket = Object.assign({}, state.basket);
            updatedBasket.PromoCode = action.basket.PromoCode;
            updatedBasket.PromoCodeDiscount = action.basket.PromoCodeDiscount;
            updatedBasket.TotalAmountDue = action.basket.TotalAmountDue;
            updatedBasket.TotalPrice = action.basket.TotalPrice;

            return Object.assign({},
                state,
                {
                    basket: updatedBasket,
                });
        }
        case types.BASKET_UPDATE_VALUE: {
            const updatedState = Object.assign({}, state);
            ObjectFunctions.setValueByStringPath(updatedState, action.field, action.value);
            return updatedState;
        }
        case types.BASKET_PREBOOK_REQUEST:
            return Object.assign({}, state, {
                isPreBooking: true,
                isPreBooked: false,
                isPreBookFailed: false,
            });
        case types.BASKET_PREBOOK_FLIGHT_REQUEST:
            return Object.assign({}, state, {
                isPreBookingFlight: true,
            });
        case types.BASKET_PREBOOK_FLIGHT_SUCCESS:
            return Object.assign({}, state, {
                isPreBookingFlight: false,
            });
        case types.BASKET_PREBOOK_ERROR:
            return Object.assign({}, state, {
                isPreBooking: false,
                isPreBookFailed: true,
            });
        case types.BASKET_PREBOOK_SUCCESS:
            return Object.assign({}, state, {
                isPreBooking: false,
                isPreBooked: true,
                isPreBookFailed: false,
                priceChangeAmount: action.prebookReturn.PriceChange,
            });
        case types.BASKET_BOOK_REQUEST:
            return Object.assign({}, state, {
                isBooking: true,
                isBookingFailed: false,
                isPaymentFailed: false,
            });
        case types.BASKET_BOOK_ERROR: {
            const isPaymentFailed = checkPaymentWarning(action.warnings);
            return Object.assign({},
                state,
                {
                    warnings: {},
                    isBooking: false,
                    isBookingFailed: true,
                    isPaymentFailed,
                });
        }
        case types.BASKET_BOOK_SUCCESS:
            return Object.assign({}, state, {
                warnings: {},
                isBooking: false,
            });
        case types.BASKET_RESET_WARNINGS:
            return Object.assign({}, state, {
                warnings: {},
            });
        case types.BASKET_ADD_WARNING: {
            const warnings = Object.assign({}, state.warnings);
            warnings[action.key] = action.value;
            return Object.assign({}, state, {
                warnings,
            });
        }
        case types.QUOTE_CREATE_REQUEST: {
            return Object.assign({}, state, {
                isCreatingQuote: true,
                isQuoteCreated: false,
            });
        }
        case types.QUOTE_CREATE_SUCCESS: {
            return Object.assign({}, state, {
                isCreatingQuote: false,
                isQuoteCreated: true,
                basket: action.basket,
            });
        }
        case types.CANCELLATION_CHARGES_RETRIEVED: {
            const cancellationCharges = Object.assign({}, state.cancellationCharges);
            const key = action.cancellationCharges.ComponentToken;
            const payments = action.cancellationCharges.Payments;
            const charges = action.cancellationCharges.CancellationCharges;
            cancellationCharges[key] = { payments, cancellationCharges: charges };
            return Object.assign({}, state, {
                cancellationCharges,
            });
        }
        default:
            return state;
    }
}
