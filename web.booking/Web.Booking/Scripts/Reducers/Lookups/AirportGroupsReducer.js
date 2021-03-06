import * as types from '../../actions/lookups/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    items: [],
};

/**
 * Redux Reducer for airports
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function airportGroupsReducer(state = initialState, action) {
    switch (action.type) {
        case types.AIRPORT_GROUPS_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.AIRPORT_GROUPS_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.airportGroups,
            });
        default:
            return state;
    }
}
