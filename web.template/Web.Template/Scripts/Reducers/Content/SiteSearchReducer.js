import * as types from '../../actions/content/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    pages: [],
    enabled: false,
    searchTerm: '',
};

/**
 * Redux Reducer for twitter
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function SiteSearchReducer(state = initialState, action) {
    switch (action.type) {
        case types.SITE_SEARCH_PAGES_REQUEST:
            return Object.assign({},
                state,
                {
                    isFetching: true,
                    isLoaded: false,
                });
        case types.SITE_SEARCH_TOGGLE:
            return Object.assign({},
                state,
                {
                    enabled: !state.enabled,
                });
        case types.SITE_SEARCH_CHANGE_SEARCH:
            return Object.assign({},
                state,
                {
                    searchTerm: action.searchTerm,
                });
        case types.SITE_SEARCH_PAGES_LOADED:
            return Object.assign({},
                state,
                {
                    isFetching: false,
                    isLoaded: true,
                    pages: action.pages,
                });
        default:
            return state;
    }
}
