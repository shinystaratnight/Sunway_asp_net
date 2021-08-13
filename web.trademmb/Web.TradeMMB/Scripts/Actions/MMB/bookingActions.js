import * as types from './actionTypes';
import MMBAPI from '../../api/MMBAPI';

/**
 * Redux Action method for loading all entity models
 * @param {object} filterOverrides - overrides
 * @return {object} The action type
 */
function loadBookingsRequest(filterOverrides) {
    return { type: types.MMB_LOAD_BOOKINGS_REQUEST, filterOverrides };
}

/**
 * Redux Action method for loading custom entity models.
 * @param {object} bookings - The models for that entity type
 * @return {object} The action type and models
 */
function entityLoadModelsFromEntityTypeSuccess(bookings) {
    return { type: types.MMB_LOAD_BOOKINGS_SUCCESS, bookings };
}

/**
 * Redux Action method for updating filters
 * @param {object} bookings - The bookings
 * @param {object} filters - The filters
 * @return {object} The action type and models
 */
function bookingFilterUpdate(bookings, filters) {
    return { type: types.MMB_FILTER_BOOKINGS, bookings, filters };
}

/**
 * Redux Action method for adding cancellation information
 * @param {object} bookingReference - The booking reference
 * @param {object} information - The cancellation information
 * @return {object} The action type, booking reference and cancellation information
 */
function cancellationInformationUpdate(bookingReference, information) {
    return { type: types.MMB_UPDATE_CANCELLATION_INFORMATION, bookingReference, information };
}

/**
 * Redux Action method for adding cancellation status
 * @param {object} bookingReference - The booking reference
 * @param {object} status - The cancellation status
 * @return {object} The action type, booking reference and cancellation status
 */
function cancellationStatusUpdate(bookingReference, status) {
    return { type: types.MMB_CANCEL_BOOKING, bookingReference, status };
}

/**
 * Redux Action method for adding cancellation information
 * @param {object} bookingReference - The booking reference
 * @param {object} success - If it was a success
 * @return {object} The action type, booking reference and cancellation information
 */
export function updateDocumentationWarning(bookingReference, success) {
    return { type: types.MMB_DOCUMENTATION_WARNING, bookingReference, success };
}

/**
 * Redux Action method for adding cancellation information
 * @param {object} bookingReference - The booking reference
 * @param {object} information - The cancellation information
 * @return {object} The action type, booking reference and cancellation information
 */
export function clearCancellationResponse() {
    return { type: types.MMB_CLEAR_CANCELLATION_STATUS };
}

/**
 * Redux Action method for loading a list of entities.
 * @param {object} filterOverrides - overrides
 * @return {object} The bookings
 */
export function loadAllBookings(filterOverrides) {
    return function load(dispatch) {
        dispatch(loadBookingsRequest(filterOverrides));
        const mmbAPI = new MMBAPI();
        return mmbAPI.loadAllBookings()
            .then(bookings => {
                dispatch(entityLoadModelsFromEntityTypeSuccess(bookings));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for sorting the bookings
 * @param {string} sortField - The sort field to use
 * @param {string} direction - The direction, ascending or descending
 * @return {object} The action type and sort option
 */
export function sortBookings(sortField, direction) {
    return { type: types.MMB_SORT_BOOKINGS, sortField, direction };
}

/**
 * Redux Action method for sorting the bookings
 * @param {string} bookingReference - The booking reference
 * @return {object} The action type and booking reference
 */
export function updateCancellationBooking(bookingReference) {
    return { type: types.MMB_UPDATE_CANCELLATION_BOOKING, bookingReference };
}

/**
 * Redux Action method for updating which components are selected for cancellation
 * @param {string} componentType - The component type
 * @param {string} token - The component token
 * @return {object} The action type, component type and component token
 */
export function updateCancellationComponents(componentType, token) {
    return { type: types.MMB_UPDATE_CANCELLATION_COMPONENTS, componentType, token };
}

/**
 * Redux Action method for changing the current page
 * @param {number} newPage - The new page
 * @return {object} The action type and new page number
 */
export function changePage(newPage) {
    return { type: types.MMB_UPDATE_PAGE, newPage };
}

/**
 * Redux Action method for changing the current page
 * @param {number} newPage - The new page
 * @return {object} The action type and new page number
 */
export function resetFilters() {
    return { type: types.MMB_RESET_FILTERS };
}

/**
 * Redux Action method for changing the current page
 * @param {number} resultsPerPage - The new results per page
 * @return {object} The action type and new results per page
 */
export function updateResultsPerPage(resultsPerPage) {
    return { type: types.MMB_UPDATE_RESULTS_PER_PAGE, resultsPerPage };
}

/**
 * Redux Action method for filtering the results
 * @param {object} filter - The updated filter
 * @return {object} The filteredbookings and updated filters
 */
export function filterResults(filter) {
    return function load(dispatch, getState) {
        const mmbAPI = new MMBAPI();
        const state = getState();
        const filters = Object.assign({}, state.mmb.filters);
        const bookings = Object.assign([], state.mmb.bookings);
        const updatedFilters = mmbAPI.updateFilters(filters, filter);
        const filteredBookings = mmbAPI.filterBookings(bookings, updatedFilters);
        dispatch(bookingFilterUpdate(filteredBookings, updatedFilters));
    };
}

/**
 * Redux Action method for getting cancellation details from the precancel
 * @param {string} bookingReference - The booking reference
 * @return {object} The action type and new information
 */
export function getCancellationDetails(bookingReference) {
    return function load(dispatch, getState) {
        const state = getState();
        const currentInfoKeys = Object.keys(state.mmb.cancellationInformation);
        if (currentInfoKeys.indexOf(bookingReference) === -1) {
            const mmbAPI = new MMBAPI();
            mmbAPI.getCancellationDetails(bookingReference, state.mmb.bookings)
                .then(cancellationInformation => {
                    dispatch(cancellationInformationUpdate(bookingReference,
                        cancellationInformation));
                });
        }
    };
}

/**
 * Redux Action method for getting cancellation details from the precancel
 * @param {string} bookingReference - The booking reference
 * @return {object} The action type and new information
 */
export function cancelComponents(bookingReference) {
    return function load(dispatch, getState) {
        const state = getState();
        const mmbAPI = new MMBAPI();
        mmbAPI.sendCancellationRequests(state, bookingReference)
            .then(cancelResponse => {
                dispatch(cancellationStatusUpdate(bookingReference, cancelResponse));
            });
    };
}

/**
 * Redux Action method for getting cancellation details from the precancel
 * @param {string} documentId - The document id
 * @param {string} type - The booking type e.g. booking or quote
 * @param {string} reference - The booking/quote reference
 * @return {object} The action type and new information
 */
export function viewBookingDocumentation(documentId, type, reference) {
    return function load(dispatch) {
        const mmbAPI = new MMBAPI();
        mmbAPI.viewBookingDocumentation(documentId, type, reference)
            .then(documentationResponse => {
                const success = documentationResponse.Success
                    && documentationResponse.Warnings.length === 0
                    && documentationResponse.DocumentPaths.length > 0;
                if (success) {
                    documentationResponse.DocumentPaths.forEach(dp => {
                        window.open(dp);
                    });
                } else {
                    dispatch(updateDocumentationWarning(bookingReference, false));
                }
            });
    };
}


/**
 * Redux Action method for getting cancellation details from the precancel
 * @param {string} documentId - The documentation id
 * @param {string} type - The booking type e.g. booking or quote
 * @param {string} reference - The booking/quote reference
 * @param {string} overrideEmail - The override email - may be blank
 * @return {object} The action type and new information
 */
export function sendBookingDocumentation(documentId, type, reference, overrideEmail) {
    return function load(dispatch) {
        const mmbAPI = new MMBAPI();
        mmbAPI.sendBookingDocumentation(documentId, type, reference, overrideEmail)
            .then(documentationResponse => {
                const success = documentationResponse.Success
                    && documentationResponse.Warnings.length === 0;
                dispatch(updateDocumentationWarning(reference, success));
            });
    };
}
