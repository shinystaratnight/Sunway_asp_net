import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    UserSession: {
        LoggedIn: false,
    },
};

/**
 * Redux Reducer for a user
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function UserReducer(state = initialState, action) {
    switch (action.type) {
        case types.USER_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.USER_LOAD_SUCCESS:
            return Object.assign({}, state,
                action.user,
                {
                    isFetching: false,
                    isLoaded: true,
                });
        case types.USER_LOGIN_SUCCESS:
            return Object.assign({}, state, action.user);
        case types.USER_LOGOUT_SUCCESS:
            return Object.assign({}, state, action.user);
        default:
            return state;
    }
}
