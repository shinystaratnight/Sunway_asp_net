import * as types from '../../actions/content/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    content: {},
};

/**
 * Redux Reducer for the page
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function PageReducer(state = initialState, action) {
    const updatedcontent = Object.assign({}, state);
    switch (action.type) {
        case types.PAGE_ADD_CONTENT:
            updatedcontent.content[action.key] = action.content;
            return updatedcontent;
        case types.PAGE_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.PAGE_LOAD_SUCCESS:
            return Object.assign({},
                state,
                action.page,
                {
                    isFetching: false,
                    isLoaded: true,
                });
        default:
            return state;
    }
}
