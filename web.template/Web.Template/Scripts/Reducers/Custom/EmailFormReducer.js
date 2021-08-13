/* eslint-disable complexity */

import * as types from '../../actions/custom/actionTypes';

const initialState = {
    isLoaded: false,
    isSubmitted: false,
    passwordsShown: false,
    fields: {},
    warnings: {
    },
};

/**
 * Redux Reducer for basket
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function emailFormReducer(state = initialState, action) {
    const updatedForm = Object.assign({}, state);
    const updatedWarnings = state.warnings;
    switch (action.type) {
        case types.EMAILFORM_UPDATED:
            updatedForm.fields[action.field] = action.value;

            return updatedForm;
        case types.FORM_ADD_WARNING:
            updatedWarnings[action.key] = action.value;
            return Object.assign({}, state, {
                warnings: updatedWarnings,
            });

        case types.FORM_INITIALIZE:
            updatedForm.fields = action.form;
            return updatedForm;
        case types.FORM_RESET_WARNINGS:
            return Object.assign({}, state, {
                warnings: {},
            });

        case types.FORM_SUBMITTED:
            return Object.assign({}, state, {
                isSubmitted: true,
            });
        case types.FORM_TOGGLE_PASSWORD:
            return Object.assign({}, state, {
                passwordsShown: action.value,
            });
        default:
            return state;
    }
}
