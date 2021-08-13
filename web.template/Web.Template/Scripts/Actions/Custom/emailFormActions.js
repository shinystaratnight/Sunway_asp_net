import * as types from './actionTypes';
import fetch from 'isomorphic-fetch';


/**
 * Redux Action method for updating the basket
 * @param {string} field - The field on the basket we're looking to update
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function updateFieldValue(field, value) {
    return { type: types.EMAILFORM_UPDATED, field, value };
}

/**
 * Redux Action method for updating the basket
 * @param {string} field - The field on the basket we're looking to update
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function togglePasswordDisplay(field, value) {
    return { type: types.FORM_TOGGLE_PASSWORD, field, value };
}

/**
 * Redux Action method for adding a warning.
 * @param {string} key - The key assoaciated with that warning e.g. 'lead guest details'.
 * @param {string} value - The value of the warning e.g. 'this field is blank'.
 * @return {object} The updated basket
 */
export function addFormWarning(key, value) {
    return { type: types.FORM_ADD_WARNING, key, value };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function resetFormWarnings() {
    return { type: types.FORM_RESET_WARNINGS };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} form - The form object we will use to set hte initial state.
 * @return {object} The updated form
 */
export function initializeForm(form) {
    return { type: types.FORM_INITIALIZE, form };
}

/**
 * Redux Action method for updating whether we have accepted cancellation terms
 * @param {string} value - The value we're looking to update it with.
 * @return {object} The updated basket
 */
export function formSubmitted() {
    return { type: types.FORM_SUBMITTED };
}

/**
* Redux Action method for the basket book.
* @param {string} fields - the fields of form for the email
* @param {string} toEmail - the email the form will be sent to
* @param {string} emailSubject - subject of the email
* @return {object} The form
*/
export function submitForm(fields, toEmail, emailSubject) {
    const formUrl = '/api/email/plaintext/Send';
    const emailModel = {
        ToEmail: toEmail,
        EmailBody: fields,
        emailSubject,
    };
    const fetchOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
        method: 'Post',
        body: JSON.stringify(emailModel),
    };
    return function load(dispatch) {
        dispatch(formSubmitted());
        return fetch(formUrl, fetchOptions);
    };
}
