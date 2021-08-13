import * as types from '../../actions/content/actionTypes';

const modelsInitialState = {
    isFetching: false,
    isLoaded: false,
    models: [],
};

/**
 * Redux Reducer for entities
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function modelsReducer(state = {}, action) {
    const updatedModels = Object.assign({}, state);
    switch (action.type) {
        case types.ENTITY_LOAD_MODELS_REQUEST: {
            if (!updatedModels.hasOwnProperty(action.key)) {
                updatedModels[action.key] = JSON.parse(JSON.stringify(modelsInitialState));
            }
            updatedModels[action.key].isFetching = true;
            updatedModels[action.key].isLoaded = false;
            break;
        }
        case types.ENTITY_LOAD_MODELS_SUCCESS: {
            updatedModels[action.key].models = action.models;
            updatedModels[action.key].isFetching = false;
            updatedModels[action.key].isLoaded = true;
            break;
        }
        default:
    }
    return updatedModels;
}
