import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading booking countries.
 * @param {object} bookingCountries - The booking countries object to return
 * @return {object} The action type and booking countries object
 */
export function bookingCountriesLoadSuccess(bookingCountries) {
    return { type: types.BOOKINGCOUNTRIES_LOAD_SUCCESS, bookingCountries };
}

/**
 * Redux Action method for requesting loading booking countries.
 * @return {object} The action type
 */
export function bookingCountriesRequest() {
    return { type: types.BOOKINGCOUNTRIES_REQUEST };
}

/**
 * Redux Action method for loading booking countries.
 * @return {object} Array of booking countries
 */
export function loadBookingCountries() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(bookingCountriesRequest());
        return lookupApi.getLookup('booking/countries').then(bookingCountries => {
            dispatch(bookingCountriesLoadSuccess(bookingCountries));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if booking countries should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadBookingCountries(state) {
    const bookingCountries = state.bookingCountries;
    const shouldLoad = bookingCountries && !bookingCountries.isFetching
        && !bookingCountries.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading booking countries if needed.
 * @return {object} Array of countries
 */
export function loadBookingCountriesIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadBookingCountries(getState())) {
            return dispatch(loadBookingCountries());
        }
        return '';
    };
}
