import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for requesting site search pages
 * @return {object} The action type and page object
 */
function siteSearchPagesRequest() {
    return { type: types.SITE_SEARCH_PAGES_REQUEST };
}

/**
 * Redux Action method for successfully loading site search pages.
 * @param {object} pages - The pages to return
 * @return {object} The action type and pages
 */
function siteSearchPagesLoaded(pages) {
    return { type: types.SITE_SEARCH_PAGES_LOADED, pages };
}

/**
 * Redux Action method for enabling and diabling the site search
 * @param {object} pages - The pages to return
 * @return {object} The action type and pages
 */
export function toggleSiteSearch() {
    return { type: types.SITE_SEARCH_TOGGLE };
}

/**
 * Redux Action method for changing the search term
 * @param {string} searchTerm - The new search term
 * @return {object} The action type and search term
 */
export function updateSiteSearchTerm(searchTerm) {
    return { type: types.SITE_SEARCH_CHANGE_SEARCH, searchTerm };
}

/**
 * Redux Action method for loading site search pages
 * @return {object} The pages
 */
export function getSiteSearchPages() {
    return function load(dispatch) {
        dispatch(siteSearchPagesRequest);
        const apiURL = '/tradebookings/api/page/all';
        return fetch(apiURL).then(response => response.json()).then(result => {
            dispatch(siteSearchPagesLoaded(result));
        });
    };
}
