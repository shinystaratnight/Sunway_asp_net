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
    if (updatedAlerts.findIndex(a => a.name === alert.name) === -1) {
        updatedAlerts.push(alert);
    }
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
 * Function to update an alert acceptance
 * @param {object} state - The current state.
 * @param {string} name - The name of the alert to update
 * @param {boolean} value - The new value
 * @return {object} the updated collection.
 */
function updateAcceptance(state, name, value) {
    const updatedAlerts = Object.assign([], state);
    const updatedAlert = updatedAlerts.find(a => a.name === name);
    updatedAlert.acceptance.value = value;
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
        case types.ALERT_ADD: {
            return addAlert(state, action.alert);
        }
        case types.ALERT_REMOVE: {
            return removeAlert(state, action.name);
        }
        case types.ALERT_ACCEPTANCE_CHANGE: {
            return updateAcceptance(state, action.name, action.value);
        }
        default:
            return state;
    }
}
