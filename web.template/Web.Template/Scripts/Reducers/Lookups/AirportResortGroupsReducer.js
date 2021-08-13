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
export default function airportResortGroupsReducer(state = initialState, action) {
    switch (action.type) {
        case types.AIRPORT_RESORT_GROUPS_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.AIRPORT_RESORT_GROUPS_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.airportResortGroups,
            });
        default:
            return state;
    }
}
