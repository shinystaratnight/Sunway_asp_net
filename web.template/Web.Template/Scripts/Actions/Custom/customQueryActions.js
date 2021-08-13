import * as types from './actionTypes';
import CustomQueryAPI from '../../api/customQueryAPI';

/**
 * Redux Action method for successfully loading a user.
 * @param {string} key - The key of the custom query object to return.
 * @return {object} The action type and custom query object.
 */
function customQueryRequest(key) {
    return { type: types.CUSTOMQUERY_REQUEST, key };
}

/**
 * Redux Action method for successfully loading a user.
 * @param {string} key - The key of the custom query object to return.
 * @param {object} content - The offers custom query content to return.
 * @return {object} The action type and custom query object.
 */
function customQueryLoadSuccess(key, content) {
    return { type: types.CUSTOMQUERY_LOAD_SUCCESS, key, content };
}

/**
 * Redux Action method for loading the current offers.
 * @param {string} key - The key of the custom query object.
 * @param {string} queryName - the custom query name.
 * @param {array} prams - the parms of the custom query.
 * @return {object} The custom query content.
 */
export function getCustomQueryContent(key, queryName, prams) {
    return function load(dispatch) {
        dispatch(customQueryRequest(key));
        const customQueryApi = new CustomQueryAPI();
        return customQueryApi.getCustomQueryContent(queryName, prams)
            .then(content => {
                dispatch(customQueryLoadSuccess(key, content));
            })
            .catch(error => {
                throw (error);
            }
        );
    };
}

