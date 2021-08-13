import * as types from '../../actions/lookups/actionTypes';

const initialState = {
    items: {},
};

/**
 * Redux Reducer for flight cache routes
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function flightCacheRouteDatesReducer(state = initialState, action) {
    switch (action.type) {
        case types.FLIGHTCACHEROUTE_LOAD_DATES_REQUEST: {
            const updatedState = Object.assign({}, state);
            updatedState.items[action.key] = {
                isLoaded: false,
                isFetching: true,
                departureDates: [],
            };
            return updatedState;
        }
        case types.FLIGHTCACHEROUTE_LOAD_DATES_SUCCESS: {
            const updatedState = Object.assign({}, state);
            updatedState.items[action.key] = {
                isLoaded: true,
                isFetching: false,
                departureDates: action.dates,
            };
            return updatedState;
        }
        default:
            return state;
    }
}
