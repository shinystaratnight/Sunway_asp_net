import * as types from '../../actions/lookups/actionTypes';

const initialState = {
    isInitialised: false,
    isFetching: false,
    departureDates: [],
    emptyDates: [],
};

/**
 * Redux Reducer for flight cache routes
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function dealFinderFlightCacheRouteDatesReducer(state = initialState, action) {
    switch (action.type) {
        case types.DEALFINDERFLIGHTCACHEROUTE_LOAD_DATES_REQUEST: {
            const updatedState = Object.assign({}, state);
            Object.assign(updatedState, {
                isInitialised: true,
                isFetching: true,
                departureDates: [],
                emptyDates: [],
            });
            return updatedState;
        }
        case types.DEALFINDERFLIGHTCACHEROUTE_LOAD_DATES_SUCCESS: {
            const updatedState = Object.assign({}, state);
            Object.assign(updatedState, {
                isInitialised: true,
                isFetching: false,
            });
            if (action.dates.length > 0) {
                action.dates.forEach(date => {
                    if (date.Durations.length === 0) {
                        updatedState.emptyDates.push(date);
                    } else {
                        updatedState.departureDates.push(date);
                    }
                });
            }
            return updatedState;
        }
        case types.DEALFINDERFLIGHTCACHEROUTE_CLEAR: {
            const updatedState = Object.assign({}, state);
            Object.assign(updatedState, {
                isInitialised: true,
                isFetching: false,
                departureDates: [],
                emptyDates: [],
            });
            return updatedState;
        }
        default:
            return state;
    }
}
