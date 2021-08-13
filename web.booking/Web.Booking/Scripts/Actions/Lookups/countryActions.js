import * as types from './actionTypes';
import LookupAPI from '../../api/lookupAPI';

/**
 * Redux Action method for successfully loading countries.
 * @param {object} countries - The countries object to return
 * @return {object} The action type and countries object
 */
export function countriesLoadSuccess(countries) {
    return { type: types.COUNTRIES_LOAD_SUCCESS, countries };
}

/**
 * Redux Action method for requesting loading countries.
 * @return {object} The action type
 */
export function countriesRequest() {
    return { type: types.COUNTRIES_REQUEST };
}

/**
 * Redux Action method for loading countries.
 * @return {object} Array of countries
 */
export function loadCountries() {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(countriesRequest());
        return lookupApi.getLookup('geography/country').then(countries => {
            dispatch(countriesLoadSuccess(countries));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if countries should be loaded
 * @param {object} state - The current state
 * @return {boolean} the result
 */
function shouldLoadCountries(state) {
    const countries = state.countries;
    const shouldLoad = countries && !countries.isFetching && !countries.isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading countries if needed.
 * @return {object} Array of countries
 */
export function loadCountriesIfNeeded() {
    return (dispatch, getState) => {
        if (shouldLoadCountries(getState())) {
            return dispatch(loadCountries());
        }
        return '';
    };
}

/**
 * Redux Action method for loading countries.
 * @param {integer} id - The id of the country to load
 * @return {object} Array of countries
 */
export function loadCountry(id) {
    const lookupApi = new LookupAPI();
    return function load(dispatch) {
        dispatch(countriesRequest());
        const url = `geography/country/${id}`;
        return lookupApi.getLookup(url).then(countries => {
            dispatch(countriesLoadSuccess(countries));
        }).catch(error => {
            throw (error);
        });
    };
}
