import * as types from '../../actions/lookups/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    items: [],
};

/**
 * Redux Reducer for brands
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function brandsReducer(state = initialState, action) {
    switch (action.type) {
        case types.BRANDS_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.BRANDS_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.brands,
            });
        default:
            return state;
    }
}
