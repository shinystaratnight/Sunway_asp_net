import * as types from './actionTypes';
import RedirectAPI from '../../api/redirectAPI';

/**
 * Redux Action method for editing a redirect
 * @return {object} The action type
 */
function redirectEdit() {
    return { type: types.REDIRECT_EDIT };
}

/**
 * Redux Action method for displaying a redirect
 * @return {object} The action type
 */
function redirectDisplay() {
    return { type: types.REDIRECT_DISPLAY };
}

/**
 * Redux Action method for updating an the value on a redirect
 * @param {string} field - The field to update
 * @param {string} value - The new value to set
 * @return {object} The action type
 */
function redirectUpdated(field, value) {
    return { type: types.REDIRECT_UPDATED, field, value };
}

/**
 * Redux Action method for setting the selected redirect
 * @param {integer} redirectId - the identifier of the redirect to select
 * @return {object} The action type
 */
function redirectSelect(redirectId) {
    return { type: types.REDIRECT_SELECT, redirectId };
}

/**
 * Redux Action method for requesting an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type and key
 */
function redirectListRequest() {
    return { type: types.REDIRECT_LOAD_LIST_REQUEST };
}

/**
 * Redux Action method for updating an entity
 * @param {string} field - The field to update
 * @param {string} value - The new value to set
 * @return {object} The entity
 */
export function updateEntityValue(field, value) {
    return function load(dispatch) {
        dispatch(redirectUpdated(field, value));
    };
}

/**
 * Redux Action method for editing a list of entities
 * @return {object} The entities
 */
export function editRedirects() {
    return function load(dispatch) {
        dispatch(redirectEdit());
    };
}

/**
 * Redux Action method for editing a list of entities
 * @return {object} The entities
 */
export function displayRedirects() {
    return function load(dispatch) {
        dispatch(redirectDisplay());
    };
}

/**
 * Method for setting the selected redirect
 * @param {integer} redirectId - the identifier of the redirect to select
 * @return {object} The redirect
 */
export function selectRedirect(redirectId) {
    return function load(dispatch) {
        dispatch(redirectSelect(redirectId));
    };
}

/**
 * Redux Action method for successfully loading an entity.
 * @param {object} redirects - The list of redirects successfully returned
 * @return {object} The action type and entity object
 */
function redirectListLoadSuccess(redirects) {
    return { type: types.REDIRECT_LOAD_LIST_SUCCESS, redirects };
}

/**
 * Redux Action method for adding a warning.
 * @param {string} key - The key assoaciated with that warning e.g. 'lead guest details'.
 * @param {string} value - The value of the warning e.g. 'this field is blank'.
 * @return {object} The updated basket
 */
export function addRedirectWarning(key, value) {
    return { type: types.REDIRECT_ADD_WARNING, key, value };
}

/**
 * Redux Action method for loading an entity.
 * @param {string} site - The site of the entity
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @param {string} mode - The entity mode (draft or live).
 * @return {object} The entity
 */
export function loadRedirectList() {
    return function load(dispatch) {
        dispatch(redirectListRequest());
        const redirectAPI = new RedirectAPI();

        return redirectAPI.loadRedirectList().then(redirects => {
            dispatch(redirectListLoadSuccess(redirects));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Method for modifying a redirect
 * @param {object} redirect - the redirect to be modified
 * @param {string} site - The site of the entity
 * @return {object} The redirect that is being modified
 */
export function modifyRedirect(redirect, site) {
    return function load(dispatch) {
        dispatch(redirectListRequest());
        const redirectAPI = new RedirectAPI();

        return redirectAPI.modifyRedirect(redirect, site).then(response => {
            if (response.Success) {
                dispatch(loadRedirectList());
            } else if (response.WarningList.length > 0) {
                dispatch(addRedirectWarning('alert', response.WarningList[0]));
            } else {
                const warning = 'There has been an error modifying the redirect';
                dispatch(addRedirectWarning('alert', warning));
            }
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Method for adding a redirect
 * @param {object} redirect - the redirect to be added
 * @param {string} site - The site of the entity
 * @return {object} The redirect that is being added
 */
export function addRedirect(redirect, site) {
    return function load(dispatch) {
        dispatch(redirectListRequest());
        const redirectAPI = new RedirectAPI();

        return redirectAPI.addRedirect(redirect, site).then(response => {
            if (response.Success) {
                dispatch(loadRedirectList());
            } else if (response.WarningList.length > 0) {
                dispatch(addRedirectWarning('alert', response.WarningList[0]));
            } else {
                const warning = 'There has been an error adding the redirect';
                dispatch(addRedirectWarning('alert', warning));
            }
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Method for deleting a redirect
 * @param {object} redirect - the redirect to be deleted
 * @param {string} site - The site of the entity
 * @return {object} The redirect that is being deleted
 */
export function deleteRedirect(redirect, site) {
    return function load(dispatch) {
        dispatch(redirectListRequest());
        const redirectAPI = new RedirectAPI();

        return redirectAPI.deleteRedirect(redirect, site).then(response => {
            if (response.Success) {
                dispatch(loadRedirectList());
            } else if (response.WarningList.length > 0) {
                dispatch(addRedirectWarning('alert', response.WarningList[0]));
            } else {
                const warning = 'There has been an error adding the redirect';
                dispatch(addRedirectWarning('alert', warning));
            }
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function resetRedirectWarnings() {
    return { type: types.REDIRECT_RESET_WARNINGS };
}


/**
 * Redux Action method for cancelling editing an redirect.
 * @return {object} The action type, key and entity
 */
function redirectEditCancel() {
    return { type: types.REDIRECT_EDIT_CANCEL };
}

/**
 * Method for cancelling editing an redirect.
 * @return {object} The action type, key and entity
 */
export function cancelEditRedirects() {
    return function load(dispatch) {
        dispatch(redirectEditCancel());
    };
}

/**
 * Redux Action method for cancelling displayibg an redirect.
 * @return {object} The action type, key and entity
 */
function redirectDisplayCancel() {
    return { type: types.REDIRECT_DISPLAY_CANCEL };
}

/**
 * Method for cancelling displaying an redirect.
 * @return {object} The action type, key and entity
 */
export function cancelDisplayRedirects() {
    return function load(dispatch) {
        dispatch(redirectDisplayCancel());
    };
}
