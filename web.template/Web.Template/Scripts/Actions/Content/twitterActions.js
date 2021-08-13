import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for requesting user tweets.
 * @return {object} The action type and page object
 */
export function userTweetsRequest() {
    return { type: types.TWITTER_USER_TWEETS_REQUEST };
}

/**
 * Redux Action method for successfully loading a page.
 * @param {object} tweets - The tweets to return
 * @return {object} The action type and tweets
 */
export function userTweetsLoaded(tweets) {
    return { type: types.TWITTER_USER_TWEETS_LOADED, tweets };
}

/**
 * Redux Action method for loading user tweets
 * @param {string} userName - The twitter user name
 * @return {object} The tweets
 */
export function getUserTweets(userName) {
    return function load(dispatch) {
        dispatch(userTweetsRequest);
        const apiURL = `/api/twitter/tweets/${userName}`;
        return fetch(apiURL).then(response => response.json()).then(result => {
            dispatch(userTweetsLoaded(result));
        });
    };
}
