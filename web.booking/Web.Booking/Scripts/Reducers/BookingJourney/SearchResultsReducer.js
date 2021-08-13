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
    adjustments: [],
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
 * function to to determine if current mode is a main search mode.
 * @param {string} mode - The search mode in question
 * @return {boolean} true if a main search mode
 */
function isMainSearchMode(mode) {
    const mainSearchModes = ['flight', 'hotel', 'flightplushotel'];
    const isMainMode = mainSearchModes.indexOf(mode.toLowerCase()) !== -1;
    return isMainMode;
}

/**
 * function to toggle the expansion of summary information
 * @param {object} state - The current state.
 * @param {string} searchMode - The current search mode
 * @param {number} id - The id of the update to apply
 * @return {object} the updated results
 */
function toggleSummaryExpansion(state, searchMode, id) {
    const results = state[searchMode].results;
    const modifiedResults = [];
    switch (searchMode.toString()) {
        case 'Hotel': {
            results.forEach(r => {
                const result = Object.assign({}, r);
                if (r.MetaData.PropertyReferenceId === id) {
                    result.DisplayExpandedSummary = !result.DisplayExpandedSummary;
                }
                modifiedResults.push(result);
            });
            break;
        }
        case 'Flight': {
            results.forEach(r => {
                const result = Object.assign({}, r);
                const componentToken = parseInt(id.split('_')[0], 10);
                const direction = id.split('_')[1];
                if (r.ComponentToken === componentToken) {
                    result.ExpandedDirections
                        = result.ExpandedDirections
                        ? result.ExpandedDirections : [];
                    const directionIndex = result.ExpandedDirections.indexOf(direction);
                    if (directionIndex === -1) {
                        result.ExpandedDirections.push(direction);
                    } else {
                        result.ExpandedDirections.splice(directionIndex);
                    }
                }
                modifiedResults.push(result);
            });
            break;
        }
        default:
    }
    return modifiedResults;
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
            const sortOptions = action.searchResults.sortOptions
                    ? action.searchResults.sortOptions : [];
            const totalPages = action.searchResults.totalPages
                    ? action.searchResults.totalPages : 1;
            const filters = action.searchResults.filters ? action.searchResults.filters : [];
            const updates = {
                isFetching: false,
                isLoaded: true,
                isUpdating: false,
                filters,
                sortOptions,
                currentPage: 1,
                totalPages,
            };
            const updatedState = updateState(state, action.searchMode, updates);
            const sortedResults = isMainSearchMode(action.searchMode)
                ? sortResults(updatedState, action.searchMode, action.searchResults.results)
                : action.searchResults.results;
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
        case types.SEARCHRESULTS_TOGGLE_EXPANDED_SUMMARY: {
            const updates = {
                results: toggleSummaryExpansion(state, action.searchMode, action.id),
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
        case types.ADJUSTMENT_SEARCH_RESULT:
            return Object.assign({}, state, {
                adjustments: action.result.BookingAdjustments,
            });
        default:
            return state;
    }
}
