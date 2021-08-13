import * as searchActions from '../bookingjourney/searchActions';
import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully flight cache route dates.
 * @param {object} dates - An array of dates
 * @return {object} The action type, key and dates
 */
function dealFinderFlightCacheRouteLoadDatesSuccess(dates) {
    return { type: types.DEALFINDERFLIGHTCACHEROUTE_LOAD_DATES_SUCCESS, dates };
}

/**
 * Redux Action method for requesting flight cache route dates.
 * @param {string} month - The month to search
 * @return {object} The action type and key
 */
function dealFinderFlightCacheRouteLoadDatesRequest(month) {
    return { type: types.DEALFINDERFLIGHTCACHEROUTE_LOAD_DATES_REQUEST, month };
}

/**
 * Redux Action method for clearing flight cache route dates.
 * @return {object} The action type
 */
function dealFinderFlightCacheRouteClear() {
    return { type: types.DEALFINDERFLIGHTCACHEROUTE_CLEAR };
}

/**
 * Method to determine if dates should be loaded
 * @param {object} state - The current state
 * @return {boolean} The result
 */
function shouldLoadDates(state) {
    const flightCacheRouteDates = state.flightCacheRouteDates;
    const item = flightCacheRouteDates.items;
    const shouldLoad = !item.isFetching;
    return shouldLoad;
}

/**
 * Method to determine if dates should be cleared
 * @param {object} state - The current state
 * @return {boolean} The result
 */
function shouldClearDates(state) {
    const flightCacheRouteDates = state.flightCacheRouteDates;
    const item = flightCacheRouteDates.items;
    const shouldLoad = !item.isFetching;
    return shouldLoad;
}

/**
 * Method to load flight cache route dates.
 * @param {number} depAirportId - The departure airport id
 * @param {string} arrivalType - The arrival type
 * @param {number} arrivalId - The arrival id
 * @param {date} searchDate - The search date
 * @param {boolean} changeDepDate - Whether to change the departure date
 * @param {boolean} updateDuration - Whether to change the duration of the search
 * @return {object} The action type and key
 */
function loadDealFinderDates(depAirportId, arrivalType, arrivalId, searchDate, changeDepDate,
    updateDuration) {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        const seven = 7;
        const fourteen = 14;
        const date = new Date();
        date.setDate(1);
        const month = `${date.getMonth()}-${date.getFullYear()}`;
        dispatch(dealFinderFlightCacheRouteLoadDatesRequest(month));
        const path = `${arrivalType}/${depAirportId}/${arrivalId}`;
        const dateString = date.toLocaleDateString('en-US');
        const lookupUrl = `dealfinderflightcacheroute/${path}?startDate=${dateString}`;
        return lookupApi.getLookup(lookupUrl)
            .then(dates => {
                let depart = new Date();
                depart.setHours(0, 0, 0, 0);
                let duration = null;
                let i = 0;
                for (i = 0; i < dates.length; i++) {
                    const checkDate = new Date(dates[i].DepartureDate);
                    checkDate.setHours(0, 0, 0, 0);
                    if (checkDate >= depart && dates[i].Durations.length) {
                        depart = checkDate;
                        const d = dates[i].Durations;
                        if (d.includes(seven)) {
                            duration = seven;
                        } else if (d.includes(fourteen)) {
                            duration = fourteen;
                        } else {
                            duration = d[0];
                        }
                        break;
                    }
                }
                const year = depart.getFullYear();
                let departureMonth = (depart.getMonth() + 1).toString();
                if (departureMonth < 10) {
                    departureMonth = `0${departureMonth}`;
                }
                let departureDay = depart.getDate().toString();
                if (departureDay < 10) {
                    departureDay = `0${departureDay}`;
                }
                const departStr = `${year}-${departureMonth}-${departureDay}`;
                const promises = [];
                promises.push(dispatch(dealFinderFlightCacheRouteLoadDatesSuccess(dates)));
                if (changeDepDate) {
                    promises.push(
                        dispatch(searchActions.searchUpdateValue('DepartureDate', departStr))
                    );
                }
                if (updateDuration) {
                    promises.push(dispatch(searchActions.searchUpdateValue('Duration', duration)));
                }
                if (changeDepDate) {
                    Promise.all(promises);
                } else {
                    Promise.all(promises)
                        .then(() => {
                            let j = 0;
                            const searchDateStr = new Date(searchDate).toDateString();
                            for (j = 0; j < dates.length; j++) {
                                const dept = new Date(dates[j].DepartureDate).toDateString();
                                if (dept === searchDateStr && dates[j].Durations.length) {
                                    return '';
                                }
                            }
                            const c = 'react-datepicker__input';
                            document.getElementsByClassName(c)[0].click();
                            return '';
                        });
                }
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for requesting flight cache route dates.
 * @param {number} departureAirportId - The departure airport id
 * @param {string} arrivalType - The arrival type
 * @param {number} arrivalId - The arrival id
 * @param {date} searchDate - The search date
 * @param {boolean} changeDepartureDate - Whether to change the departure date
 * @param {boolean} updateDuration - Whether to change the duration of the search
 * @return {object} The action type and key
 */
export function loadDealFinderFlightCacheRouteDates(departureAirportId, arrivalType,
    arrivalId, searchDate, changeDepartureDate, updateDuration) {
    return (dispatch, getState) => {
        if (shouldLoadDates(getState())) {
            return dispatch(loadDealFinderDates(departureAirportId, arrivalType, arrivalId,
                searchDate, changeDepartureDate, updateDuration));
        }
        return '';
    };
}

/**
 * Redux Action method for clearing flight cache route dates.
 * @param {number} departureAirportId - The departure airport id
 * @param {string} arrivalType - The arrival type
 * @param {number} arrivalId - The arrival id
 * @param {date} date - The search date
 * @param {boolean} changeDepartureDate - Whether to change the departure date
 * @param {boolean} updateDuration - Whether to change the duration of the search
 * @return {object} The action type
 * */
export function clearAndLoadDealFinderFlightCacheDates(departureAirportId, arrivalType,
    arrivalId, date, changeDepartureDate, updateDuration) {
    return (dispatch, getState) => {
        if (shouldClearDates(getState())) {
            dispatch(dealFinderFlightCacheRouteClear());
        }

        return dispatch(loadDealFinderDates(departureAirportId, arrivalType, arrivalId,
            date, changeDepartureDate, updateDuration));
    };
}

/**
 * Redux Action method for clearing flight cache route dates.
 * @return {object} The action type
 * */
export function clearDealFinderFlightCacheRouteDates() {
    return (dispatch) => dispatch(dealFinderFlightCacheRouteClear());
}
