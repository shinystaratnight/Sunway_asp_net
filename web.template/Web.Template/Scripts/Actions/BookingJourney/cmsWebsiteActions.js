import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for requesting loading the site.
 * @return {object} The action type
 */
export function websitesRequest() {
    return { type: types.CMS_WEBSITE_REQUEST };
}

/**
 * Redux Action method for successfully loading a user.
 * @param {object} cmsWebsites - The site object to return
 * @return {object} The action type and site object
 */
export function siteLoadSuccess(cmsWebsites) {
    return { type: types.CMS_WEBSITE_LOAD_SUCCESS, cmsWebsites };
}

/**
 * Redux Action method for loading the current site.
 * @return {object} The site
 */
export function loadWebsites() {
    const siteURL = '/api/websites';
    return function load(dispatch) {
        dispatch(websitesRequest());
        return fetch(siteURL).then(response => response.json()).then(result => {
            dispatch(siteLoadSuccess(result));
        });
    };
}
