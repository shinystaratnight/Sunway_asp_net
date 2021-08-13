import * as types from '../../actions/content/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    tweets: [],
};

/**
 * Redux Reducer for twitter
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function TwitterReducer(state = initialState, action) {
    switch (action.type) {
        case types.TWITTER_USER_TWEETS_REQUEST:
            return Object.assign({},
                state,
                {
                    isFetching: true,
                    isLoaded: false,
                });
        case types.TWITTER_USER_TWEETS_LOADED:
            return Object.assign({},
                state,
                {
                    isFetching: false,
                    isLoaded: true,
                    tweets: action.tweets,
                });
        default:
            return state;
    }
}
