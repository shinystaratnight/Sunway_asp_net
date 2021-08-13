import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
};

/**
 * Redux Reducer for the site
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function siteReducer(state = initialState, action) {
    switch (action.type) {
        case types.SITE_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.SITE_LOAD_SUCCESS:
            return Object.assign({},
                state,
                action.site,
                {
                    isFetching: false,
                    isLoaded: true,
                });
        default:
            return state;
    }
}
