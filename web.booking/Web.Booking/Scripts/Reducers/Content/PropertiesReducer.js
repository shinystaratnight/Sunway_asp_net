import * as types from 'actions/content/actionTypes';

const initialPropertyState = {
    isFetching: false,
    isLoaded: false,
    content: {},
};

/**
 * Redux Reducer for the properties
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function propertiesReducer(state = {}, action) {
    const updatedProperties = Object.assign({}, state);
    switch (action.type) {
        case types.PROPERTY_LOAD_REQUEST:
            if (!updatedProperties.hasOwnProperty(action.propertyId)) {
                updatedProperties[action.propertyId] = initialPropertyState;
            }
            updatedProperties[action.propertyId].isFetching = true;
            updatedProperties[action.propertyId].isLoaded = false;
            break;
        case types.PROPERTY_LOAD_SUCCESS: {
            const updates = {
                isFetching: false,
                isLoaded: true,
                content: action.property,
            };
            updatedProperties[action.propertyId]
                = Object.assign({}, updatedProperties[action.propertyId], updates);
            break;
        }
        default:
    }
    return updatedProperties;
}
