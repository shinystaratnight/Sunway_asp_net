import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    details: {},
};

/**
 * Redux Reducer for a booking
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function BookingReducer(state = initialState, action) {
    switch (action.type) {
        case types.BOOKINGDETAILS_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.BOOKINGDETAILS_SUCCESS:
            return Object.assign({}, state,
                {
                    isFetching: false,
                    isLoaded: true,
                    details: action.booking,
                });
        default:
            return state;
    }
}
