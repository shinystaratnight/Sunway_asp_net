﻿import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';

/**
 * Redux Action method for requesting loading a page.
 * @return {object} The action type
 */
export function pageRequest() {
    return { type: types.PAGE_REQUEST };
}

/**
 * Redux Action method for adding page content
 * @param {object} key - The offers object to return
 * @param {string} content - The key of the offers object to return
 * @return {object} The action type
 */
export function addPageContent(key, content) {
    return { type: types.PAGE_ADD_CONTENT, key, content };
}

/**
 * Redux Action method for successfully loading a page.
 * @param {object} page - The page object to return
 * @return {object} The action type and page object
 */
export function pageLoadSuccess(page) {
    return { type: types.PAGE_LOAD_SUCCESS, page };
}

/**
 * Redux Action method for loading the current page.
 * @return {object} The page
 */
export function loadPage() {
    return function load(dispatch) {
        dispatch(pageRequest());

        let pageURL = window.location.pathname;
        pageURL = pageURL === '/booking' || pageURL === '/'
			? 'homepage' : pageURL.replace('/booking/', '');
		let apiURL = null;
		let apiResult = null;
		if (document.getElementById('SearchTool_Payload')) {
			apiResult = dispatch(pageLoadSuccess(
						JSON.parse(
							document.getElementById('SearchTool_Payload').textContent)));
		} else {
			apiURL = `/booking/api/page/${pageURL}`;
			apiResult = fetch(apiURL, { credentials: 'same-origin' })
				.then(response => response.json())
				.then(result => {
					dispatch(pageLoadSuccess(result));
				});
		}
		return apiResult;
	};
}
