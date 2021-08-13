import * as types from './actionTypes';
import WebRequest from 'library/webrequest';

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
    const siteUrl = '/api/site';
    return function load(dispatch) {
        dispatch(siteRequest());
        WebRequest.send(siteUrl)
            .then(response => {
                if (response.ok) {
                    dispatch(siteLoadSuccess(response.object));
                }
            });
    };
}
