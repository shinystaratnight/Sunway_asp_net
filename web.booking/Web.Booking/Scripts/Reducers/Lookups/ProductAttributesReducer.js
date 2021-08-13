import * as types from '../../actions/lookups/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    items: [],
};

/**
 * Redux Reducer for productAttributes
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function productAttributesReducer(state = initialState, action) {
    switch (action.type) {
        case types.PRODUCTATTRIBUTES_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.PRODUCTATTRIBUTES_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.productAttributes,
            });
        default:
            return state;
    }
}
