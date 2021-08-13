import * as types from '../../actions/custom/actionTypes';

const initialOffersState = {
    key: '',
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    items: [],
    filters: [],
    sortOptions: ['Default', 'Price (Low to High)', 'Price (High to Low)',
        'Hotel name (A-Z)', 'Hotel name (Z-A)', 'Star rating (Low to High)',
        'Star rating (High to Low)'],
    selectedSort: 'Default',
    currentPage: 1,
    totalPages: 1,
    offer: {},
};

/**
 * function to sort results by selected sort oder
 * @param {object} offers - The relevant offers object
 * @return {object} the sorted results.
 */
function sortResults(offers) {
    const sortFunction
        = offers.sortOptions.filter(option => option.name
                            === offers.selectedSort)[0].sortFunction;
    const sortedResults = sortFunction(offers.items);
    return sortedResults;
}

/**
 * Redux Reducer for the page
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function specialOffersReducer(state = {}, action) {
    const updatedOffers = Object.assign({}, state);
    switch (action.type) {
        case types.SPECIALOFFERS_REQUEST:
            if (!updatedOffers.hasOwnProperty(action.key)) {
                updatedOffers[action.key] = initialOffersState;
            }
            updatedOffers[action.key].isFetching = true;
            updatedOffers[action.key].isLoaded = false;
            break;
        case types.SPECIALOFFERS_LOAD_SUCCESS: {
            const updates = {
                items: action.offers.items,
                filters: action.offers.filters,
                isFetching: false,
                isLoaded: true,
                sortOptions: action.offers.sortOptions,
                currentPage: 1,
                totalPages: action.offers.totalPages,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            updatedOffers[action.key].items = sortResults(updatedOffers[action.key]);
            break;
        }
        case types.SPECIALOFFER_LOAD_SUCCESS: {
            const updates = {
                offer: action.offer,
                isLoaded: true,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            break;
        }
        case types.SPECIALOFFERS_UPDATE_PAGE: {
            const updates = {
                currentPage: action.page,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            break;
        }
        case types.SPECIALOFFERS_SORT: {
            const updates = {
                selectedSort: action.sortOption,
                currentPage: 1,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            updatedOffers[action.key].items = sortResults(updatedOffers[action.key]);
            break;
        }
        case types.SPECIALOFFERS_FILTER_UPDATE: {
            const updates = {
                isUpdating: true,
                filters: action.filters,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            break;
        }
        case types.SPECIALOFFERS_RESET_FILTERS: {
            const updates = {
                isUpdating: true,
                filters: action.filters,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            break;
        }
        case types.SPECIALOFFERS_UPDATED: {
            const updates = {
                isUpdating: false,
                items: action.offers.specialOffers,
                currentPage: 1,
                totalPages: action.offers.totalPages,
            };
            updatedOffers[action.key] = Object.assign({}, updatedOffers[action.key], updates);
            updatedOffers[action.key].items = sortResults(updatedOffers[action.key]);
            break;
        }
        default:
    }
    return updatedOffers;
}
