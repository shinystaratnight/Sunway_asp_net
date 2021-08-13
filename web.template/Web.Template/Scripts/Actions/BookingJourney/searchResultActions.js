import * as types from './actionTypes';
import SearchResultsAPI from '../../api/searchResultsAPI';

/**
 * Redux Action method for successfully loading search results.
 * @param {string} searchMode - The search mode
 * @return {object} The action type and results object
 */
function searchResultsRequest(searchMode) {
    return { type: types.SEARCHRESULTS_REQUEST, searchMode };
}

/**
 * Redux Action method for successfully loading search results.
 * @param {string} searchMode - The search mode
 * @param {object} searchResults - The results object to return
 * @return {object} The action type and results object
 */
function searchResultsLoadSuccess(searchMode, searchResults) {
    return { type: types.SEARCHRESULTS_LOAD_SUCCESS, searchMode, searchResults };
}

/**
 * Redux Action method for updating filters
 * @param {string} searchMode - The search mode
 * @param {object} filters - The filters to return
 * @return {object} the action type and filters object
 */
export function searchResultsFilterUpdate(searchMode, filters) {
    return { type: types.SEARCHRESULTS_FILTER_UPDATE, searchMode, filters };
}

/**
 * Redux Action method for updating search results
 * @param {string} searchMode - The search mode
 * @param {object} searchResults - The updated search results to return
 * @return {object} the action type and results object
 */
export function searchResultsUpdated(searchMode, searchResults) {
    return { type: types.SEARCHRESULTS_UPDATED, searchMode, searchResults };
}

/**
 * Redux Action method for sorting the search results
 * @param {string} searchMode - The search mode
 * @param {string} sortOption - The sort option to use
 * @return {object} The action type and sort option
 */
export function searchResultsSort(searchMode, sortOption) {
    return { type: types.SEARCHRESULTS_SORT, searchMode, sortOption };
}

/**
 * Redux Action method for updating the current page
 * @param {string} searchMode - The search mode
 * @param {number} page - The selected page number
 * @return {object} The action type and page number
 */
export function searchResultsUpdatePage(searchMode, page) {
    return { type: types.SEARCHRESULTS_UPDATE_PAGE, searchMode, page };
}

/**
 * Redux Action method for enabling/disabling changing flight
 * @param {string} searchMode - The search mode
 * @param {boolean} isChangingFlight - Whether or not changing flight
 * @return {object} The action type and page number
 */
export function searchResultsChangeFlight(searchMode, isChangingFlight) {
    return { type: types.SEARCHRESULTS_CHANGE_FLIGHT, searchMode, isChangingFlight };
}

/**
 * Redux Action method for setting the selected result
 * @param {string} searchMode - The search mode
 * @param {object} results - The results to update
 * @param {number} componentToken - The selected component token
 * @return {object} The updated results
 */
export function searchResultsSetSelected(searchMode, results, componentToken) {
    return function load(dispatch, getState) {
        const searchResultsAPI = new SearchResultsAPI();
        const state = getState();
        const filters = state.searchResults[searchMode].filters;
        const updatedResults = searchResultsAPI.setSelected(results, componentToken, filters);
        dispatch(searchResultsUpdated(searchMode, updatedResults));
    };
}

/**
 * Redux Action method for filtering search results
 * @param {string} searchMode - The search mode
 * @param {object} results - The search results to filter
 * @param {object} filters - The filters to apply
 * @return {object} The filtered results
 */
export function filterSearchResults(searchMode, results, filters) {
    return function load(dispatch) {
        const searchResultsAPI = new SearchResultsAPI();
        const filteredResults = searchResultsAPI.filter(results, filters);
        dispatch(searchResultsUpdated(searchMode, filteredResults));
    };
}

/**
 * Redux Action method for loading search results.
 * @param {string} searchToken - The search token
 * @param {string} searchMode - The search mode
 * @param {boolean} isPackage - Whether or not results are for a package
 * @param {object} filterFacilities - The filter facilities
 * @return {object} The entity
 */
export function loadSearchResults(searchToken, searchMode, isPackage) {
    return function load(dispatch, getState) {
        dispatch(searchResultsRequest(searchMode));
        const searchResultsAPI = new SearchResultsAPI();
        const state = getState();
        return searchResultsAPI.setupSearchResults(searchToken, searchMode, isPackage, state)
            .then(searchResults => {
                dispatch(searchResultsLoadSuccess(searchMode, searchResults));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for starting a search request.
 * @param {string} searchMode - The search mode
 * @param {string} key - The property to update
 * @param {*} value - The new value
 * @return {object} the action
 */
export function searchResultsFilterUpdateValue(searchMode, key, value) {
    return function load(dispatch, getState) {
        const searchResultsAPI = new SearchResultsAPI();
        const state = getState();
        const filters = state.searchResults[searchMode].filters;
        const updatedFilters = searchResultsAPI.updateFilter(filters, key, value);
        dispatch(searchResultsFilterUpdate(searchMode, updatedFilters));
        dispatch(filterSearchResults(searchMode,
            state.searchResults[searchMode].results, updatedFilters));
    };
}
