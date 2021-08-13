import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for requesting loading the site.
 * @return {object} The action type
 */
export function siteRequest() {
    return { type: types.SITE_REQUEST };
}

/**
 * Redux Action method for successfully loading a user.
 * @param {object} site - The site object to return
 * @return {object} The action type and site object
 */
export function siteLoadSuccess(site) {
    return { type: types.SITE_LOAD_SUCCESS, site };
}

/**
 * Redux Action method for loading the current site.
 * @return {object} The site
 */
export function loadSite() {
    const siteURL = '/tradebookings/api/site';
    return function load(dispatch) {
        dispatch(siteRequest());
        return fetch(siteURL).then(response => response.json()).then(result => {
            dispatch(siteLoadSuccess(result));
        });
    };
}
