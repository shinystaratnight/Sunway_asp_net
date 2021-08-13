import * as types from 'actions/mmb/actionTypes';

const quotesInitialState = {
    isFetching: false,
    isLoaded: false,
    items: [],
    filters: {
        quoteReference: {
            name: 'quoteReference',
            value: '',
            label: 'Reference',
            type: 'text',
            field: 'QuoteReference',
            placeholder: 'Reference Number',
        },
        guestName: {
            name: 'guestName',
            value: '',
            label: 'Guest Name',
            type: 'multifieldText',
            fields: ['LeadCustomerFirstName', 'LeadCustomerLastName'],
            placeholder: 'Guest Name',
        },
        enquired: {
            name: 'enquired',
            startDate: '',
            endDate: '',
            label: 'Enquired',
            selectedOption: 'Any',
            options: ['Any', 'Today', 'Yesterday', 'Last Week', 'Last Fortnight', 'Range'],
            type: 'date',
            field: 'BookingDate',
        },
        travelling: {
            name: 'travelling',
            startDate: '',
            endDate: '',
            label: 'Travelling',
            selectedOption: 'Any',
            options: ['Any', 'Range'],
            type: 'date',
            field: 'ArrivalDate',
        },
    },
};

/**
 * Redux Reducer for direct debits
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function quotesReducer(state = quotesInitialState, action) {
    switch (action.type) {
        case types.QUOTE_SEARCH_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
                items: [],
            });
        case types.QUOTE_SEARCH_RESULT:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.result.Quotes,
            });
        case types.QUOTE_FILTER_UPDATED:
            return Object.assign({},
                state,
                {
                    filters: action.filters,
                });
        case types.QUOTE_FILTER_RESET: {
            const updatedItems = Object.assign([], state.items);
            for (let i = 0; i < updatedItems.length; i++) {
                updatedItems[i].display = true;
            }
            return Object.assign({},
                state,
                {
                    filters: quotesInitialState.filters,
                    items: updatedItems,
                });
        }
        case types.QUOTE_RESULTS_UPDATED:
            return Object.assign({},
                state,
                {
                    items: action.results,
                });
        default:
            return state;
    }
}
