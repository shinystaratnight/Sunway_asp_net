import * as types from './actionTypes';

/**
 * Redux Action method for adding an alert to the alert collection.
 * @param {string} name - Name to identify the alert
 * @param {string} type - The Type of alert
 * @param {string} message - The message to display
 * @param {boolean} dismissible - Sets whether or not the alert can be dismissed
 * @return {object} The action type and alert object
 */
export function addAlert(name, type, message, dismissible) {
    const isDismissible = typeof dismissible === 'boolean' ? dismissible : true;
    const alert = {
        name,
        type,
        message,
        dismissible: isDismissible,
        acceptance: {},
    };
    return { type: types.ALERT_ADD, alert };
}

/**
 * Redux Action method for adding an alert to the alert collection.
 * @param {object} alert - The alert object to add
 * @return {object} The action type and alert object
 */
export function addAlertObject(alert) {
    return { type: types.ALERT_ADD, alert };
}

/**
 * Redux Action method for adding an alert to the alert collection.
 * @param {string} name - Name to identify the alert
 * @return {object} The action type and alert object
 */
export function removeAlert(name) {
    return { type: types.ALERT_REMOVE, name };
}


/**
 * Redux Action method for updating acceptance on an alert.
 * @param {string} name - Name to identify the alert
 * @param {boolean} value - True or False whether the alert is accepted
 * @return {object} The action type, name and value
 */
export function alertAcceptanceChange(name, value) {
    return { type: types.ALERT_ACCEPTANCE_CHANGE, name, value };
}
