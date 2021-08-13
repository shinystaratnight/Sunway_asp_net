import * as types from '../../actions/content/actionTypes';

const entityInitialState = {
    context: '',
    isFetching: false,
    isLoaded: false,
    isEditing: false,
    name: '',
    lastModifiedDate: new Date(),
    lastModifiedUser: '',
    jsonSchema: {},
    model: {},
    status: '',
};

/**
 * Redux Reducer for entities
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function CMSContentReducer(state = {}, action) {
    const updatedEntities = Object.assign({}, state);
    switch (action.type) {
        case types.CMS_REQUEST: {
            if (!updatedEntities.hasOwnProperty(action.key)) {
                updatedEntities[action.key] = entityInitialState;
            }
            updatedEntities[action.key].isFetching = true;
            updatedEntities[action.key].isLoaded = false;
            break;
        }
        case types.CMS_LOAD_SUCCESS: {
            updatedEntities[action.key] = Object.assign({},
                updatedEntities[action.key], action.entity);
            updatedEntities[action.key].isFetching = false;
            updatedEntities[action.key].isLoaded = true;
            break;
        }
        default:
    }
    return updatedEntities;
}
