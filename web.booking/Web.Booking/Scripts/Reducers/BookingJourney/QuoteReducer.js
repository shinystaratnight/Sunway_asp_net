import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isRetrieving: false,
    retrieveComplete: false,
    QuoteEmailPopupShown: false,
    QuoteEmailPropertyToken: '',
    QuoteEmailPropertyCheapestResults: {},
    result: {},
};

/**
 * Redux Reducer for quote
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function searchReducer(state = initialState, action) {
    switch (action.type) {
        case types.QUOTE_RETRIEVE_REQUEST:
            return Object.assign({}, state,
                {
                    isRetrieving: true,
                    retrieveComplete: false,
                });
        case types.QUOTE_RETRIEVE_SUCCESS:
            return Object.assign({}, state,
                {
                    isRetrieving: false,
                    retrieveComplete: true,
                    result: action.result,
                });
        case types.QUOTE_EMAIL_SHOW:
            return Object.assign({}, state,
                {
                    QuoteEmailPopupShown: true,
                    QuoteEmailPropertyToken: action.token,
                    QuoteEmailPropertyCheapestResults: action.cheapestResults,
                });
        case types.QUOTE_EMAIL_HIDE:
            return Object.assign({}, state,
                {
                    QuoteEmailPopupShown: false,
                    QuoteEmailPropertyToken: '',
                });
        default:
            return state;
    }
}
