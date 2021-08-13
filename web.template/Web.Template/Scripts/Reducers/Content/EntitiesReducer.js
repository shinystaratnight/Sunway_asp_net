import * as types from '../../actions/content/actionTypes';

const entityInitialState = {
    context: '',
    isFetching: false,
    isLoaded: false,
    isEditing: false,
    isDeleted: false,
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
export default function entitiesReducer(state = {}, action) {
    const updatedEntities = Object.assign({}, state);
    switch (action.type) {
        case types.ENTITY_REQUEST: {
            if (!updatedEntities.hasOwnProperty(action.key)) {
                updatedEntities[action.key] = entityInitialState;
            }
            updatedEntities[action.key].isFetching = true;
            updatedEntities[action.key].isLoaded = false;
            break;
        }
        case types.ENTITY_LOAD_SUCCESS: {
            updatedEntities[action.key] = Object.assign({},
                updatedEntities[action.key], action.entity);
            updatedEntities[action.key].isFetching = false;
            updatedEntities[action.key].isLoaded = true;
            break;
        }
        case types.ENTITY_EDIT: {
            Object.keys(updatedEntities)
                .map(key => {
                    updatedEntities[key].isEditing = false;
                });
            updatedEntities[action.key] = Object.assign({},
                updatedEntities[action.key], action.entity);
            updatedEntities[action.key].isEditing = true;
            break;
        }
        case types.ENTITY_EDIT_CANCEL: {
            updatedEntities[action.key] = Object.assign({}, updatedEntities[action.key]);
            updatedEntities[action.key].isEditing = false;
            break;
        }
        case types.ENTITY_SAVE_SUCCESS: {
            updatedEntities[action.key] = Object.assign({},
                updatedEntities[action.key], action.entity);
            updatedEntities[action.key].isEditing = false;
            const entityNewItemKey = `${action.entity.name}-entity-new-item`;
            delete updatedEntities[entityNewItemKey];
            break;
        }
        case types.ENTITY_UPDATED: {
            updatedEntities[action.key] = Object.assign({},
               updatedEntities[action.key], action.entity);
            break;
        }
        case types.ENTITY_DELETE_REQUEST: {
            if (!updatedEntities.hasOwnProperty(action.key)) {
                updatedEntities[action.key] = entityInitialState;
            }
            break;
        }
        case types.ENTITY_DELETED: {
            updatedEntities[action.key] = entityInitialState;
            updatedEntities[action.key].isDeleted = true;
            break;
        }
        default:
    }
    return updatedEntities;
}
