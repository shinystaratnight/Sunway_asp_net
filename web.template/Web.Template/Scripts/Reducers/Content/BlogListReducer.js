import * as types from '../../actions/content/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    items: [],
    filters: [],
    currentPage: 1,
    totalPages: 1,
};

/**
 * Redux Reducer for Blog List
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function BlogListReducer(state = initialState, action) {
    switch (action.type) {
        case types.BLOGLIST_LOAD_REQUEST:
            return Object.assign({},
                state,
                {
                    isFetching: true,
                    isLoaded: false,
                });
        case types.BLOGLIST_LOAD_SUCCESS:
            return Object.assign({}, state,
                {
                    isFetching: false,
                    isLoaded: true,
                },
                action.blogList);
        case types.BLOGLIST_FILTER_UPDATE:
            return Object.assign({}, state,
                {
                    isUpdating: true,
                    filters: action.filters,
                });
        case types.BLOGLIST_RESULTS_UPDATED:
            return Object.assign({}, state,
                {
                    isUpdating: false,
                    items: action.blogs,
                });
        default:
            return state;
    }
}
