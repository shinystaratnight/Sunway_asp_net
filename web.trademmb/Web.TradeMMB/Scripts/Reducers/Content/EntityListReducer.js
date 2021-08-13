import * as types from '../../actions/content/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    isEditing: false,
    entities: [],
};

/**
 * Redux Reducer for Entity list
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function EntityListReducer(state = initialState, action) {
    switch (action.type) {
        case types.ENTITY_LOAD_LIST_REQUEST:
            return Object.assign({},
                state,
                {
                    isFetching: true,
                    isLoaded: false,
                });
        case types.ENTITY_LOAD_LIST_SUCCESS:
            return Object.assign({}, state,
                {
                    isFetching: false,
                    isLoaded: true,
                    entities: action.entities,
                });
        case types.ENTITY_LIST_EDIT:
            return Object.assign({}, state,
                {
                    isEditing: true,
                });
        case types.ENTITY_LIST_CANCEL:
            return Object.assign({}, state,
                {
                    isEditing: false,
                });
        case types.ENTITY_EDIT_CANCEL: {
            return Object.assign({}, state,
                {
                    isEditing: false,
                });
        }
        case types.ENTITY_LOAD_CONTEXTS_REQUEST: {
            const updatedState = Object.assign({}, state);

            const editingEntity = updatedState.entities.find(entity => entity.isEditing);
            if (editingEntity) {
                editingEntity.isEditing = false;
            }
            const updatedEntity = updatedState.entities.find(entity =>
                entity.name === action.entity);
            updatedEntity.fetchingContexts = true;
            updatedEntity.isEditing = true;

            return updatedState;
        }
        case types.ENTITY_LOAD_CONTEXTS_SUCCESS: {
            const updatedState = Object.assign({}, state);
            const updatedEntity = updatedState.entities.find(entity =>
                entity.name === action.entity);
            updatedEntity.contexts = action.contexts;
            updatedEntity.contextsLoaded = true;
            updatedEntity.fetchingContexts = false;
            return updatedState;
        }
        default:
            return state;
    }
}
