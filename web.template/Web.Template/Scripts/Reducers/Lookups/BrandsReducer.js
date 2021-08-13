import * as types from '../../actions/lookups/actionTypes';

/**
 * Redux Reducer for brands
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function brandsReducer(state = [], action) {
    switch (action.type) {
        case types.BRANDS_LOAD_SUCCESS:
            return action.brands;
        default:
            return state;
    }
}
