import * as AlertConstants from 'constants/alerts';
import * as types from 'actions/bookingjourney/actionTypes';

import ArrayFunctions from 'library/arrayfunctions';

const initialState = [];

/**
 * Function to add an alert to the current state
 * @param {object} state - The current state.
 * @param {object} alert - The alert to add
 * @return {object} the updated collection.
 */
function addAlert(state, alert) {
    const updatedAlerts = Object.assign([], state);
    updatedAlerts.push(alert);
    return updatedAlerts;
}

/**
 * Function to remove an alert from the current state
 * @param {object} state - The current state.
 * @param {string} name - The name of the alert to remove
 * @return {object} the updated collection.
 */
function removeAlert(state, name) {
    const updatedAlerts = ArrayFunctions.removeItem(state, 'name', name);
    return updatedAlerts;
}

/**
 * Redux Reducer for alerts
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated collection.
 */
export default function alertsReducer(state = initialState, action) {
    switch (action.type) {
        case types.BASKET_BOOK_ERROR: {
            const alert = {
                type: AlertConstants.ALERT_TYPES.DANGER,
                message: action.warning,
                dismissible: true,
            };
            return addAlert(state, alert);
        }
        case types.BASKET_PREBOOK_ERROR: {
            const alert = {
                type: AlertConstants.ALERT_TYPES.DANGER,
                message: action.warning,
                dismissible: true,
            };
            return addAlert(state, alert);
        }
        case types.ALERT_ADD: {
            return addAlert(state, action.alert);
        }
        case types.ALERT_REMOVE: {
            return removeAlert(state, action.name);
        }
        default:
            return state;
    }
}
