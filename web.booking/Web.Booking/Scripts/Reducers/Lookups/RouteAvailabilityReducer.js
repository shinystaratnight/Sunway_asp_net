import * as types from '../../actions/lookups/actionTypes';

/**
 * Redux Reducer for airport routes
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function routeAvailabilityReducer(state = [], action) {
    switch (action.type) {
        case types.ROUTEAVAILABILITY_LOAD_SUCCESS:
            return action.routes;
        default:
            return state;
    }
}
