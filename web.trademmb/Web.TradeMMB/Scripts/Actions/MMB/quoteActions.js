import * as types from './actionTypes';
import QuoteAPI from '../../api/quoteapi';

/**
 * Redux dispatch method for requesting a quote search
 * @return {object} The action type
 */
function quoteSearchRequest() {
    return { type: types.QUOTE_SEARCH_REQUEST };
}

/**
 * Redux dispatch method for quote search result
 * @param {object} result - The result
 * @return {object} The action type
 */
function quoteSearchResult(result) {
    return { type: types.QUOTE_SEARCH_RESULT, result };
}

/**
 * Redux dispatch method for quote filters update
 * @param {object} filters - The updated filters
 * @return {object} The action type
 */
function quoteFilterUpdated(filters) {
    return { type: types.QUOTE_FILTER_UPDATED, filters };
}

/**
 * Redux dispatch method for quote results update
 * @param {object} results - The updated results
 * @return {object} The action type
 */
function quoteResultsUpdated(results) {
    return { type: types.QUOTE_RESULTS_UPDATED, results };
}

/**
 * Redux action method for filtering quotes
 * @param {object} quotes - The quotes to filter
 * @param {object} filters - The filters to apply
 * @return {object} The action type
 */
export function filterQuoteResults(quotes, filters) {
    return function load(dispatch) {
        const quoteApi = new QuoteAPI();
        const filteredQuotes = quoteApi.filterQuotes(quotes, filters);
        dispatch(quoteResultsUpdated(filteredQuotes));
    };
}

/**
 * Redux action method for quote search
 * @param {object} searchModel - The searchModel
 * @return {object} The action type
 */
export function quoteSearch(searchModel) {
    return function load(dispatch) {
        dispatch(quoteSearchRequest());
        const quoteApi = new QuoteAPI();
        return quoteApi.search(searchModel)
            .then(result => {
                dispatch(quoteSearchResult(result));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux action method for updating the quotes filter
 * @param {object} filter - The updated filter
 * @return {object} The action type
 */
export function quoteUpdateFilter(filter) {
    return function load(dispatch, getState) {
        const quoteApi = new QuoteAPI();
        const state = getState();
        const filters = Object.assign({}, state.quotes.filters);
        const quotes = Object.assign([], state.quotes.items);
        const updatedFilters = quoteApi.updateFilters(filters, filter);
        dispatch(quoteFilterUpdated(updatedFilters));
        dispatch(filterQuoteResults(quotes, updatedFilters));
    };
}

/**
 * Redux action method for resetting the quote filter
 * @return {object} The action type
 */
export function quoteFilterReset() {
    return { type: types.QUOTE_FILTER_RESET };
}
