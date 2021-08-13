import * as types from '../../actions/mmb/actionTypes';

const directDebitInitialState = {
    isFetching: false,
    isLoaded: false,
    directDebits: [],
};

/**
 * Redux Reducer for direct debits
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function directDebitReducer(state = {}, action) {
    let updatedState = Object.assign({}, state);
    switch (action.type) {
        case types.DIRECT_DEBIT_LOAD_REQUEST: {
            if (!updatedState.isLoaded) {
                updatedState = directDebitInitialState;
            }
            updatedState.isFetching = true;
            updatedState.isLoaded = false;
            break;
        }
        case types.DIRECT_DEBIT_LOAD_SUCCESS: {
            const response = action.directDebitRetrieval;
            const tradeId = action.tradeId;
            const directDebits = response.BookingLine.filter(bl => bl.TradeId === tradeId);
            updatedState.directDebits = directDebits;
            updatedState = Object.assign(updatedState, response);
            updatedState.isFetching = false;
            updatedState.isLoaded = true;
            break;
        }
        default:
    }
    return updatedState;
}
