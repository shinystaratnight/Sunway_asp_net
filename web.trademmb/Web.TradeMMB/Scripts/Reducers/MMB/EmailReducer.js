import * as types from '../../actions/mmb/actionTypes';

const initialState = {
    isLoaded: false,
    isSubmitted: false,
    fields: {
        Message: '',
        ContactName: '',
        ContactTelephone: '',
        ContactEmailAddress: '',
    },
    warnings: {
    },
    message: '',
};

/**
 * Redux Reducer for email form
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function emailReducer(state = initialState, action) {
    const updatedForm = Object.assign({}, state);
    const updatedWarnings = state.warnings;
    let updatedMessage;
    switch (action.type) {
        case types.EMAILFORM_UPDATED:
            updatedForm.fields[action.field] = action.value;
            return updatedForm;
        case types.FORM_ADD_WARNING:
            updatedWarnings[action.key] = action.value;
            return Object.assign({}, state, {
                warnings: updatedWarnings,
            });
        case types.FORM_RESET_WARNINGS:
            return Object.assign({}, state, {
                warnings: {},
            });
        case types.FORM_SET_MESSAGE:
            updatedMessage = action.value;
            return Object.assign({}, state, {
                message: updatedMessage,
            });
        case types.FORM_RESET_MESSAGE:
            return Object.assign({}, state, {
                message: '',
            });
        case types.EMAILFORM_RESET:
            return Object.assign({}, state, {
                fields: {
                    Message: '',
                    ContactName: '',
                    ContactTelephone: '',
                    ContactEmailAddress: '',
                },
            });
        default:
            return state;
    }
}

