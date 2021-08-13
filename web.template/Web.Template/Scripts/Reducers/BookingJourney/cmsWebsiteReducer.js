import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    websites: [],
};

/**
 * Redux Reducer for cms websites
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function cmsWebsiteReducer(state = initialState, action) {
    switch (action.type) {
        case types.CMS_WEBSITE_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.CMS_WEBSITE_LOAD_SUCCESS:
            return Object.assign({},
                state,
                {
                    isFetching: false,
                    isLoaded: true,
                    websites: action.cmsWebsites,
                });
        default:
            return state;
    }
}
