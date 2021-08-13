import * as types from './actionTypes';
import SearchAPI from '../../api/searchAPI';

/**
 * Redux Action method for successfully loading search details.
 * @param {object} searchDetails - The search details object to return
 * @return {object} The action type and search details object
 */
function searchDetailsLoadSuccess(searchDetails) {
    return { type: types.SEARCHDETAILS_LOAD_SUCCESS, searchDetails };
}

/**
 * Redux Action method for loading search details.
 * @param {object} searchConfig - The search configuration
 * @return {object} The search details
 */
export function loadSearchDetails(searchConfig) {
    const searchAPI = new SearchAPI();
    return function load(dispatch) {
        return searchAPI.getSearchDetails(searchConfig)
            .then(searchDetails => {
                dispatch(searchDetailsLoadSuccess(searchDetails));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Helper function to determine if search details should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadSearchDetails(state) {
    const search = state.search;
    const shouldLoad = search && !search.isFetching && !search.isLoaded && !search.isSearching;
    return shouldLoad;
}

/**
 * Redux Action method for loading search details if needed.
 * @param {object} searchConfig - The search configuration
 * @return {object} The search details
 */
export function loadSearchDetailsIfNeeded(searchConfig) {
    return (dispatch, getState) => {
        if (shouldLoadSearchDetails(getState())) {
            return dispatch(loadSearchDetails(searchConfig));
        }
        return '';
    };
}

/**
 * Redux Action method for starting a search request.
 * @param {string} key - The property to update
 * @param {*} value - The new value
 * @return {object} the action
 */
export function searchUpdateValue(key, value) {
    return { type: types.SEARCH_UPDATE_VALUE, key, value };
}


/**
 * Redux Action method for starting a search request.
 * @return {object} the action
 */
function searchRequest() {
    return { type: types.SEARCH_REQUEST };
}


/**
 * Redux Action method for starting a search request.
 * @return {object} the action
 */
function extraSearchRequest() {
    return { type: types.EXTRA_SEARCH_REQUEST };
}

/**
 * Redux Action method for returning the search result
 * @param {string} result - The search result
 * @return {object} The search result
 */
function searchResult(result) {
    return { type: types.SEARCH_RESULT, result };
}

/**
 * Redux Action method for returning the search result
 * @param {string} result - The search result
 * @return {object} The search result
 */
function extraSearchResult(result) {
    return { type: types.EXTRA_SEARCH_RESULT, result };
}

/**
 * Redux Action method for performing search.
 * @param {object} searchDetails - The search details
 * @return {object} The search result
 */
export function performSearch(searchDetails) {
    return function load(dispatch) {
        dispatch(searchRequest());
        const searchAPI = new SearchAPI();
        return searchAPI.performSearch(searchDetails, true)
            .then(result => {
                dispatch(searchResult(result));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for performing extra searches.
 * @param {object} searchDetails - The search details
 * @param {string} isMainSearch - Flag, true if this is the search mode to maintain in local store
 * @return {object} The search result
 */
export function performExtraSearch(searchDetails) {
    return function load(dispatch) {
        dispatch(extraSearchRequest());
        const searchAPI = new SearchAPI();
        return searchAPI.performSearch(searchDetails, false)
            .then(result => {
                dispatch(extraSearchResult(result));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for performing a deeplink search.
 * @param {string} url - The deeplink url
 * @return {object} The search result
 */
export function performDeeplinkSearch(url) {
    return function load(dispatch) {
        dispatch(searchRequest());
        const searchAPI = new SearchAPI();
        return searchAPI.performDeeplinkSearch(url)
            .then(result => {
                dispatch(searchResult(result));
            }).catch(error => {
                throw (error);
            });
    };
}
