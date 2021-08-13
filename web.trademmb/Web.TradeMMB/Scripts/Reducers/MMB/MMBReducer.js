import * as MMBConstants from '../../constants/mmb';
import * as types from '../../actions/mmb/actionTypes';

import ArrayFunctions from '../../library/arrayfunctions';

const mmbInitialState = {
    isFetching: false,
    isLoaded: false,
    bookings: [],
    sortField: 'Reference',
    resultsPerPage: 20,
    currentPage: 1,
    documentationWarning: {},
    selectedCancellationBooking: '',
    selectedComponentTokens: [],
    cancellationInformation: {},
    cancellationStatus: {},
    filters: {
        bookingReference: {
            name: 'bookingReference',
            value: '',
            label: 'Reference',
            type: 'text',
            field: 'BookingReference',
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
        booked: {
            name: 'booked',
            startDate: '',
            endDate: '',
            label: 'Booked',
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
 * function to sort results by selected sort order
 * @param {object} bookings - The bookings
 * @param {string} field - The sort field
 * @param {string} direction - The direction
 * @return {object} the sorted results.
 */
function sortBookings(bookings, field, direction) {
    const sortFieldKey = MMBConstants.SORT_FIELDS[field];
    let sortedBookings = Object.assign([], bookings);
    sortedBookings = direction.toLowerCase() === 'ascending'
        ? ArrayFunctions.sortByPropertyAscending(sortedBookings, sortFieldKey)
        : ArrayFunctions.sortByPropertyDescending(sortedBookings, sortFieldKey);
    return sortedBookings;
}

/**
 * function to overrride default filter settings
 * @param {object} filters - The exisiting filters
 * @param {object} filterOverrides - The overrides
 * @return {object} the sorted results.
 */
function assignOverrides(filters, filterOverrides) {
    const newFilters = Object.assign({}, filters);
    newFilters.bookingReference.label = filterOverrides.Reference.Label;
    newFilters.bookingReference.placeholder = filterOverrides.Reference.Placeholder;
    newFilters.guestName.label = filterOverrides.GuestName.Label;
    newFilters.guestName.placeholder = filterOverrides.GuestName.Placeholder;
    newFilters.booked.label = filterOverrides.Booked.Label;
    newFilters.travelling.label = filterOverrides.Travelling.Label;
    return newFilters;
}

/**
 * Redux Reducer for entities
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function mmbReducer(state = {}, action) {
    let updatedState = Object.assign({}, state);
    switch (action.type) {
        case types.MMB_LOAD_BOOKINGS_REQUEST: {
            if (!updatedState.isLoaded) {
                updatedState = mmbInitialState;
            }
            updatedState.filters = assignOverrides(updatedState.filters, action.filterOverrides);
            updatedState.isFetching = true;
            updatedState.isLoaded = false;
            break;
        }
        case types.MMB_LOAD_BOOKINGS_SUCCESS: {
            const noShellBookings = action.bookings
                .filter(b => b.Status !== 'Shell' && b.Status !== 'Enquiry');
            updatedState.bookings
                = sortBookings(noShellBookings, updatedState.sortField, 'descending');
            for (let i = 0; i < updatedState.bookings.length; i++) {
                updatedState.bookings[i].display = true;
                updatedState.bookings[i].BookingReference
                    = updatedState.bookings[i].BookingReference.trim();
            }
            updatedState.isFetching = false;
            updatedState.isLoaded = true;
            break;
        }
        case types.MMB_SORT_BOOKINGS: {
            updatedState.sortField = action.sortField;
            updatedState.bookings
                = sortBookings(updatedState.bookings, action.sortField, action.direction);
            break;
        }
        case types.MMB_UPDATE_PAGE: {
            updatedState.currentPage = action.newPage;
            break;
        }
        case types.MMB_UPDATE_RESULTS_PER_PAGE: {
            updatedState.resultsPerPage = action.resultsPerPage;
            const numberValidBookings = updatedState.bookings.filter(b => b.display).length;
            const lastPage = numberValidBookings === 0
                ? 1 : Math.ceil(numberValidBookings / updatedState.resultsPerPage);
            const currentPage = updatedState.currentPage <= lastPage
                ? updatedState.currentPage : lastPage;
            updatedState.currentPage = currentPage;
            break;
        }
        case types.MMB_FILTER_BOOKINGS: {
            updatedState.bookings = action.bookings;
            updatedState.filters = action.filters;
            const numberValidBookings = action.bookings.filter(b => b.display).length;
            const lastPage = numberValidBookings === 0
                ? 1 : Math.ceil(numberValidBookings / updatedState.resultsPerPage);
            const currentPage = updatedState.currentPage <= lastPage
                ? updatedState.currentPage : lastPage;
            updatedState.currentPage = currentPage;
            break;
        }
        case types.MMB_RESET_FILTERS: {
            for (let i = 0; i < updatedState.bookings.length; i++) {
                updatedState.bookings[i].display = true;
            }
            updatedState.filters = mmbInitialState.filters;
            break;
        }
        case types.MMB_UPDATE_CANCELLATION_BOOKING: {
            const reference = updatedState.selectedCancellationBooking === action.bookingReference
                ? '' : action.bookingReference;
            updatedState.selectedCancellationBooking = reference;
            updatedState.selectedComponentTokens = [];
            break;
        }
        case types.MMB_UPDATE_CANCELLATION_COMPONENTS: {
            const exists = updatedState.selectedComponentTokens.indexOf(action.token) !== -1;
            if (exists) {
                updatedState.selectedComponentTokens = updatedState.selectedComponentTokens
                    .filter(t => t !== action.token);
            } else {
                updatedState.selectedComponentTokens.push(action.token);
            }
            if (action.componentType === 'All' && !exists) {
                updatedState.selectedComponentTokens = [action.token];
            }
            break;
        }
        case types.MMB_UPDATE_CANCELLATION_INFORMATION: {
            const cancellationInformation = {
                components: action.information.components,
                all: {
                    CancellationCost: action.information.all.Cost,
                    Token: action.information.all.Token,
                    Type: 'All',
                },
            };
            updatedState.cancellationInformation[action.bookingReference]
                = cancellationInformation;
            break;
        }
        case types.MMB_CANCEL_BOOKING: {
            const status = action.status;
            status.bookingReference = action.bookingReference;
            updatedState.cancellationStatus = status;
            if (status.type !== 'All') {
                const bookings = updatedState.bookings;
                const indexOfBooking
                    = bookings.findIndex(b => b.BookingReference === action.bookingReference);
                bookings[indexOfBooking].TotalPrice = action.status.Cost;
                updatedState.bookings = bookings;
            }
            updatedState.selectedCancellationBooking = '';
            break;
        }
        case types.MMB_CLEAR_CANCELLATION_STATUS: {
            updatedState.cancellationStatus = {};
            break;
        }
        case types.MMB_DOCUMENTATION_WARNING: {
            const documentationWarning = {
                bookingReference: action.bookingReference,
                success: action.success,
            };
            updatedState.documentationWarning = documentationWarning;
            break;
        }
        default:
    }
    return updatedState;
}
