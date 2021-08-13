import * as types from 'actions/lookups/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    items: [],
};

/**
 * Redux Reducer for card types
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function cardTypesReducer(state = initialState, action) {
    switch (action.type) {
        case types.CARDTYPES_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
            });
        case types.CARDTYPES_LOAD_SUCCESS:
            return Object.assign({}, state, {
                isFetching: false,
                isLoaded: true,
                items: action.cardTypes,
            });
        default:
            return state;
    }
}
