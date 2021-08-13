import * as types from '../../actions/bookingjourney/actionTypes';

const initialResultState = {
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    isChangingFlight: false,
    results: [],
    filters: [],
    sortOptions: [],
    selectedSort: 'Price Ascending',
    currentPage: 1,
    totalPages: 1,
};

const initialState = {
    Hotel: initialResultState,
    Flight: initialResultState,
};

/**
 * function to sort results by selected sort oder
 * @param {object} state - The current state.
 * @param {string} searchMode - The current search mode
 * @param {object} results - The results to sort
 * @return {object} the sorted results.
 */
function sortResults(state, searchMode, results) {
    const selectedSortOption = state[searchMode].sortOptions.filter(sortOption =>
        sortOption.name === state[searchMode].selectedSort)[0];
    const sortedResults = Object.assign([], results);
    sortedResults.sort(selectedSortOption.sortFunction);
    return sortedResults;
}

/**
 * function to get the current state and ensuring a default state
 * for the given search mode if it does not exist
 * @param {object} state - The current state.
 * @param {string} searchMode - The current search mode
 * @return {object} the current state
 */
function getState(state, searchMode) {
    const currentState = Object.assign({}, state);
    if (!currentState.hasOwnProperty(searchMode)) {
        currentState[searchMode] = initialResultState;
    }
    return currentState;
}

/**
 * function to update the current state with updates to a given search mode.
 * @param {object} state - The current state.
 * @param {string} searchMode - The current search mode
 * @param {objects} updates - The updates to apply
 * @return {object} the updated state
 */
function updateState(state, searchMode, updates) {
    const updatedState = getState(state, searchMode);
    const updatedSearchModeState = Object.assign({}, updatedState[searchMode], updates);
    updatedState[searchMode] = updatedSearchModeState;
    return updatedState;
}

/**
 * Redux Reducer for search results
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function searchResultsReducer(state = initialState, action) {
    switch (action.type) {
        case types.SEARCHRESULTS_REQUEST: {
            const updates = {
                isFetching: true,
            };
            const updatedState = updateState(state, action.searchMode, updates);
            return updatedState;
        }
        case types.SEARCHRESULTS_LOAD_SUCCESS: {
            const updates = {
                isFetching: false,
                isLoaded: true,
                isUpdating: false,
                filters: action.searchResults.filters,
                sortOptions: action.searchResults.sortOptions,
                currentPage: 1,
                totalPages: action.searchResults.totalPages,
            };
            const updatedState = updateState(state, action.searchMode, updates);

            const sortedResults
                = sortResults(updatedState, action.searchMode, action.searchResults.results);
            updatedState[action.searchMode].results = sortedResults;

            return updatedState;
        }
        case types.SEARCHRESULTS_FILTER_UPDATE: {
            const updates = {
                isUpdating: true,
                filters: action.filters,
            };
            const updatedState = updateState(state, action.searchMode, updates);
            return updatedState;
        }
        case types.SEARCHRESULTS_UPDATED: {
            const sortedResults = sortResults(state, action.searchMode,
                action.searchResults.results);
            const updates = {
                isUpdating: false,
                results: sortedResults,
                currentPage: 1,
            };
            if (action.searchResults.totalPages) {
                updates.totalPages = action.searchResults.totalPages;
            }
            if (action.searchResults.hasOwnProperty('isChangingFlight')) {
                updates.isChangingFlight = action.searchResults.isChangingFlight;
            }
            const updatedState = updateState(state, action.searchMode, updates);
            return updatedState;
        }
        case types.SEARCHRESULTS_SORT: {
            const updates = {
                isLoaded: true,
                isUpdating: false,
                selectedSort: action.sortOption,
                currentPage: 1,
            };
            const updatedState = updateState(state, action.searchMode, updates);

            const sortedResults = sortResults(updatedState, action.searchMode,
                updatedState[action.searchMode].results);
            updatedState[action.searchMode].results = sortedResults;

            return updatedState;
        }
        case types.SEARCHRESULTS_UPDATE_PAGE: {
            const updates = {
                currentPage: action.page,
            };
            const updatedState = updateState(state, action.searchMode, updates);
            return updatedState;
        }
        case types.SEARCHRESULTS_CHANGE_FLIGHT: {
            const updates = {
                isChangingFlight: action.isChangingFlight,
            };
            const updatedState = updateState(state, action.searchMode, updates);
            return updatedState;
        }
        default:
            return state;
    }
}
