import * as types from '../../actions/content/actionTypes';
import ObjectFunctions from 'library/objectfunctions';

const initialState = {
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    isEditing: false,
    isDisplaying: false,
    selectedRedirect: {
        Url: '',
        RedirectUrl: '',
        RedirectId: 0,
    },
    redirects: [],
    warnings: {
    },
};

/**
 * Redux Reducer for Entity list
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function RedirectReducer(state = initialState, action) {
    const updatedWarnings = state.warnings;

    const redirect = Object.assign({}, state.redirects.filter(r =>
        r.RedirectId === action.redirectId)[0]);
    const selectedRedirect = Object.assign({}, state.selectedRedirect);

    switch (action.type) {
        case types.REDIRECT_LOAD_LIST_REQUEST:
            return Object.assign({},
                state,
                {
                    isFetching: true,
                    isLoaded: false,
                });
        case types.REDIRECT_LOAD_LIST_SUCCESS:
            return Object.assign({},
                state,
                {
                    isFetching: false,
                    isLoaded: true,
                    isEditing: false,
                    selectedRedirect: {
                        Url: '',
                        RedirectUrl: '',
                        RedirectId: 0,
                    },
                    redirects: action.redirects,
                });
        case types.REDIRECT_SELECT:
            return Object.assign({},
                state,
                {
                    selectedRedirect: redirect,
                    isEditing: true,
                    warnings: {},
                });
        case types.REDIRECT_UPDATED:
            return Object.assign({}, state,
                {
                    selectedRedirect: ObjectFunctions.setValueByStringPath(
                        selectedRedirect, action.field, action.value),
                    isEditing: true,
                });
        case types.REDIRECT_EDIT:
            return Object.assign({}, state,
                {
                    isEditing: true,
                    selectedRedirect: {
                        Url: '',
                        RedirectUrl: '',
                        RedirectId: 0,
                    },
                });
        case types.REDIRECT_EDIT_CANCEL:
            return Object.assign({}, state,
                {
                    isEditing: false,
                    selectedRedirect: {
                        Url: '',
                        RedirectUrl: '',
                        RedirectId: 0,
                    },
                    warnings: {},
                });
        case types.REDIRECT_DISPLAY:
            return Object.assign({}, state,
                {
                    isDisplaying: true,
                });
        case types.REDIRECT_DISPLAY_CANCEL:
            return Object.assign({}, state,
                {
                    isDisplaying: false,
                });
        case types.REDIRECT_ADD_WARNING:
            updatedWarnings[action.key] = action.value;
            return Object.assign({}, state, {
                warnings: updatedWarnings,
            });
        case types.REDIRECT_RESET_WARNINGS:
            return Object.assign({}, state, {
                warnings: {},
            });
        default:
            return state;
    }
}
