import * as types from '../../actions/bookingjourney/actionTypes';
import ObjectFunctions from '../../library/objectfunctions';

const initialState = {
    isFetching: false,
    isLoaded: false,
    isSearching: false,
    isSearchingForAdjustments: false,
    searchComplete: false,
    adjustmentSearchComplete: false,
    searchDetails: {},
    searchResult: {
        ResultCounts: {},
        ResultTokens: {},
    },
    extraSearch: {},
};

/**
 * Redux Reducer for search
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function searchReducer(state = initialState, action) {
    switch (action.type) {
        case types.SEARCHDETAILS_LOAD_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.SEARCHDETAILS_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                searchDetails: action.searchDetails,
            });
        case types.SEARCHDETAILS_UPDATED:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                searchDetails: action.searchDetails,
            });
        case types.SEARCH_UPDATE_VALUE: {
            const searchDetails = Object.assign({}, state.searchDetails);
            const updatedSearchDetails = ObjectFunctions.setValueByStringPath(
                searchDetails, action.key, action.value);
            return Object.assign({}, state, {
                searchDetails: updatedSearchDetails,
            });
        }
        case types.SEARCH_REQUEST:
            return Object.assign({}, state, {
                isSearching: true,
                searchComplete: false,
            });
        case types.SEARCH_RESULT:
            return Object.assign({}, state, {
                isLoaded: true,
                isSearching: false,
                searchComplete: true,
                searchResult: action.result.searchResult,
                searchDetails: action.result.searchDetails,
            });
        case types.EXTRA_SEARCH_REQUEST: {
            const extraSearch = Object.assign({}, state.extraSearch);
            extraSearch[action.identifier] = {
                isSearching: true,
                searchComplete: false,
            };
            return Object.assign({},
                state,
                {
                    extraSearch,
                });
        }
        case types.EXTRA_SEARCH_RESULT: {
            const extraSearch = Object.assign({}, state.extraSearch);
            extraSearch[action.identifier] = {
                isSearching: false,
                searchComplete: true,
            };
            const searchResult = Object.assign({}, state.searchResult);
            searchResult.ResultCounts = Object.assign({},
                searchResult.ResultCounts, action.result.searchResult.ResultCounts);
            searchResult.ResultTokens = Object.assign({},
                searchResult.ResultTokens, action.result.searchResult.ResultTokens);
            return Object.assign({},
                state,
                {
                    isLoaded: true,
                    searchResult,
                    extraSearch,
                });
        }
        case types.SEARCHRESULTS_LOAD_SUCCESS: {
            const searchResult = Object.assign({}, state.searchResult);
            searchResult.ResultCounts[action.searchMode]
                = action.searchResults.results.length ? action.searchResults.results.length : 0;
            searchResult.ResultTokens[action.searchMode] = action.searchResults.token;

            return Object.assign({},
                state,
                {
                    isLoaded: true,
                    isSearching: false,
                    searchComplete: true,
                    searchResult,
                });
        }
        case types.ADJUSTMENT_SEARCH_REQUEST:
            return Object.assign({}, state, {
                isSearchingForAdjustments: true,
                adjustmentSearchComplete: false,
            });
        case types.ADJUSTMENT_SEARCH_RESULT: {
            const searchResult = Object.assign({}, state.searchResult);
            searchResult.ResultTokens.BookingAdjustments = action.result.ResultToken;
            return Object.assign({},
                state,
                {
                    isSearchingForAdjustments: false,
                    adjustmentSearchComplete: true,
                    searchResult,
                });
        }
        default:
            return state;
    }
}
