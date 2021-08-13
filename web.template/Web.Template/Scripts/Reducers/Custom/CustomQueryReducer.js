import * as types from '../../actions/custom/actionTypes';

/**
 * Redux Reducer for the page
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function customQueryReducer(state = {}, action) {
    const updatedCustomQuery = Object.assign({}, state);
    switch (action.type) {
        case types.CUSTOMQUERY_REQUEST:
            updatedCustomQuery[action.key] = {
                isFetching: true,
                isLoaded: false,
            };
            break;
        case types.CUSTOMQUERY_LOAD_SUCCESS: {
            const updates = {
                content: action.content,
                isFetching: false,
                isLoaded: true,
            };
            updatedCustomQuery[action.key]
                = Object.assign({}, updatedCustomQuery[action.key], updates);
            break;
        }
        default:
    }
    return updatedCustomQuery;
}
