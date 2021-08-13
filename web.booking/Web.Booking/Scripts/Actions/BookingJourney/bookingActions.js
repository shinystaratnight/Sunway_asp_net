import * as types from './actionTypes';
import BookingAPI from 'api/bookingAPI';

/**
 * Redux Action method for basket create request
 * @return {object} The action type and basket details object
 */
function bookingDetailsRequest() {
    return { type: types.BOOKINGDETAILS_REQUEST };
}

/**
 * Redux Action method for successfully loading the booking details.
 * @param {object} booking - The booking to return
 * @return {object} The action type and booking
 */
function bookingDetailsSuccess(booking) {
    return { type: types.BOOKINGDETAILS_SUCCESS, booking };
}

/**
 * Redux Action method for retrieving booking details.
 * @param {string} reference - The booking reference to retrieve
 * @return {object} The action type and booking
 */
export function retrieveBooking(reference) {
    return function load(dispatch) {
        dispatch(bookingDetailsRequest());
        const bookingAPI = new BookingAPI();
        return bookingAPI.retrieveBooking(reference)
            .then(booking => {
                dispatch(bookingDetailsSuccess(booking));
            })
            .catch(error => {
                throw (error);
            });
    };
}
