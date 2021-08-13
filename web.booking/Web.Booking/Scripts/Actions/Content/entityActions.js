import * as types from './actionTypes';
import EntityAPI from '../../api/entityAPI';
import StringFunctions from '../../library/stringfunctions';

/**
 * Redux Action method for successfully loading an entity.
 * @param {string} key - Unique key to identify the entity
 * @param {object} entity - The entity object to return
 * @return {object} The action type and entity object
 */
function entityLoadSuccess(key, entity) {
    return { type: types.ENTITY_LOAD_SUCCESS, key, entity };
}

/**
 * Redux Action method for successfully loading an entity.
 * @param {string} key - Unique key to identify the entity
 * @param {object} entity - The entity object to return
 * @return {object} The action type and entity object
 */
function cmsLoadSuccess(key, entity) {
    return { type: types.CMS_LOAD_SUCCESS, key, entity };
}

/**
 * Redux Action method for requesting an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type and key
 */
function entityRequest(key) {
    return { type: types.ENTITY_REQUEST, key };
}

/**
 * Redux Action method for requesting an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type and key
 */
function cmsRequest(key) {
    return { type: types.CMS_REQUEST, key };
}

/**
 * Redux Action method for editing an entity.
 * @param {string} key - Unique key to identify the entity
 * @param {object} entity - The entity object to return
 * @return {object} The action type, key and entity
 */
function entityEdit(key, entity) {
    return { type: types.ENTITY_EDIT, key, entity };
}


/**
 * Redux Action method for updating an entity.
 * @param {string} key - Unique key to identify the entity
 * @param {object} entity - The entity object to return
 * @return {object} The action type, key and entity
 */
function entityUpdated(key, entity) {
    return { type: types.ENTITY_UPDATED, key, entity };
}

/**
 * Redux Action method for requesting deleting an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type and key
 */
function entityDeleteRequest(key) {
    return { type: types.ENTITY_DELETE_REQUEST, key };
}

/**
 * Redux Action method for deleting an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type, key and entity
 */
function entityDeleted(key) {
    return { type: types.ENTITY_DELETED, key };
}

/**
 * Redux Action method for cancelling editing an entity.
 * @param {string} key - Unique key to identify the entity
 * @return {object} The action type, key and entity
 */
function entityEditCancel(key) {
    return { type: types.ENTITY_EDIT_CANCEL, key };
}

/**
 * Redux Action method for successfully saving an entity.
 * @param {string} key - Unique key to identify the entity
 * @param {object} entity - The entity object to return
 * @return {object} The action type and entity object
 */
function entitySaveSuccess(key, entity) {
    return { type: types.ENTITY_SAVE_SUCCESS, key, entity };
}

/**
 * Redux Action method for loading a list of entities.
 * @return {object} The action type
 */
function entityLoadListRequest() {
    return { type: types.ENTITY_LOAD_LIST_REQUEST };
}

/**
 * Redux Action method for loading custom entities.
 * @param {object} entities - The entities object to return
 * @return {object} The action type and entities
 */
function entityLoadListSuccess(entities) {
    return { type: types.ENTITY_LOAD_LIST_SUCCESS, entities };
}


/**
 * Redux Action method for editing a list of entities
 * @return {object} The action type
 */
function entityListEdit() {
    return { type: types.ENTITY_LIST_EDIT };
}

/**
 * Redux Action method for cancelling editing a list of entities
 * @return {object} The action type
 */
export function entityListCancel() {
    return { type: types.ENTITY_LIST_CANCEL };
}

/**
 * Redux Action method for loading an entity contexts
 * @param {string} entity - The entity
 * @return {object} The action type
 */
function entityLoadContextsRequest(entity) {
    return { type: types.ENTITY_LOAD_CONTEXTS_REQUEST, entity };
}

/**
 * Redux Action method for successfully loading entity contexts
 * @param {string} entity - The entity
 * @param {object} contexts - The array of contexts to return
 * @return {object} The action type
 */
function entityLoadContextsSuccess(entity, contexts) {
    return { type: types.ENTITY_LOAD_CONTEXTS_SUCCESS, entity, contexts };
}

/**
 * Redux Action method for loading a list of entities.
 * @param {string} key - The entity key
 * @return {object} The action type
 */
function entityLoadModelsFromEntityTypeRequest(key) {
    return { type: types.ENTITY_LOAD_MODELS_REQUEST, key };
}

/**
 * Redux Action method for loading custom entities.
 * @param {string} key - The entity key
 * @param {object} models - The models to be loaded
 * @return {object} The action type, key and entities
 */
function entityLoadModelsFromEntityTypeSuccess(key, models) {
    return {
        type: types.ENTITY_LOAD_MODELS_SUCCESS, key, models };
}

/**
 * Redux Action method for loading an entity.
 * @param {string} site - The site of the entity
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @param {string} mode - The entity mode (draft or live).
 * @return {object} The entity
 */
export function loadEntity(site, name, context, mode) {
    const key = `${name}-${context}`;
    return function load(dispatch) {
        dispatch(entityRequest(key));
        const entityAPI = new EntityAPI(site);
        return entityAPI.loadEntity(site, name, context, mode).then(entity => {
            dispatch(entityLoadSuccess(key, entity));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Redux Action method for loading a cms model.
 * @param {string} objectType - The object type of the cms model
 * @param {string} id - The id of the model
 * @return {object} The entity
 */
export function loadCMSModel(objectType, id) {
    const key = `${objectType}-${id}`;
    return function load(dispatch) {
        dispatch(cmsRequest(key));
        const entityAPI = new EntityAPI('');
        return entityAPI.loadCMSModel(objectType, id).then(entity => {
            dispatch(cmsLoadSuccess(key, entity));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Helper function to determine if an entity should be loaded
 * @param {object} state - The current state
 * @param {string} key - The entity key
 * @return {boolean} the result
 */
function shouldLoadEntity(state, key) {
    const entities = state.entities;
    const shouldLoad = !entities[key] || !entities[key].isLoaded;
    return shouldLoad;
}

/**
 * Redux Action method for loading an entity if needed.
 * @param {string} site - The site of the entity
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @param {string} mode - The entity mode (draft or live).
 * @return {object} The entity
 */
export function loadEntityIfNeeded(site, name, context, mode) {
    const key = `${name}-${context}`;
    return (dispatch, getState) => {
        if (shouldLoadEntity(getState(), key)) {
            return dispatch(loadEntity(site, name, context, mode));
        }
        return '';
    };
}

/**
 * Redux Action method for loading an entity.
 * @param {string} site - The site
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @return {object} The entity
 */
export function editEntity(site, name, context) {
    const key = `${name}-${context}`;
    return function load(dispatch) {
        const entityAPI = new EntityAPI(site);
        return entityAPI.loadEntity(site, name, context, 'draft').then(entity => {
            dispatch(entityEdit(key, entity));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Redux Action method for updating an entity
 * @param {object} entity - The entity to update
 * @param {string} field - The field to update
 * @param {string} value - The new value to set
 * @return {object} The entity
 */
export function updateEntityValue(entity, field, value) {
    const key = `${entity.name}-${entity.context}`;
    return function load(dispatch) {
        const entityAPI = new EntityAPI(entity.site);
        return entityAPI.updateEntityValue(entity, field, value)
            .then(updatedEntity => {
                dispatch(entityUpdated(key, updatedEntity));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for cancelling editing an entity
 * @param {string} site - The site
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @param {string} mode - The entity mode (draft or live).
 * @return {object} The entities
 */
export function cancelEditEntity(site, name, context, mode) {
    const key = `${name}-${context}`;
    return function load(dispatch) {
        dispatch(entityEditCancel(key));
        dispatch(loadEntity(site, name, context, mode));
    };
}

/**
 * Redux Action method for saving an entity.
 * @param {object} entity - The entity to update
 * @return {object} The entity
 */
export function saveEntity(entity) {
    const key = `${entity.name}-${StringFunctions.safeUrl(entity.context)}`;
    return function load(dispatch) {
        if (entity.context === 'entity-new-item') {
            return dispatch(entitySaveSuccess(key, null));
        }

        const entityAPI = new EntityAPI(entity.site);
        return entityAPI.saveEntity(entity)
            .then(savedEntity => {
                dispatch(entitySaveSuccess(key, savedEntity));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for publishing an entity.
 * @param {string} site - The site
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @param {object} entity - The entity to update
 * @return {object} The entity
 */
export function publishEntity(site, name, context, entity) {
    const key = `${name}-${context}`;
    return function load(dispatch) {
        const entityAPI = new EntityAPI(site);
        return entityAPI.publishEntity(site, name, context, entity).then(publishedEntity => {
            dispatch(entitySaveSuccess(key, publishedEntity));
        }).catch(error => {
            throw (error);
        });
    };
}

/**
 * Redux Action method for deleting an entity.
 * @param {string} site - The site
 * @param {string} name - The name of the entity
 * @param {string} context - The entity context
 * @return {object} The entity
 */
export function deleteEntity(site, name, context) {
    const key = `${name}-${context}`;
    return function load(dispatch) {
        dispatch(entityDeleteRequest(key));
        const entityAPI = new EntityAPI(site);
        return entityAPI.deleteEntity(site, name, context).then(result => {
            if (result.ok) {
                dispatch(entityDeleted(key));
            }
        }).catch(error => {
            throw (error);
        });
    };
}


/**
 * Redux Action method for loading a list of entities.
 * @param {string} site - The site
 * @param {string} type - The type of entity
 * @return {object} The entities
 */
export function loadEntityList(site, type) {
    return function load(dispatch) {
        dispatch(entityLoadListRequest());
        const entityAPI = new EntityAPI(site);
        return entityAPI.loadEntityList(site, type)
            .then(entities => {
                dispatch(entityLoadListSuccess(entities));
            }).catch(error => {
                throw (error);
            });
    };
}


/**
 * Redux Action method for editing a list of entities
 * @return {object} The entities
 */
export function editEntityList() {
    return function load(dispatch) {
        dispatch(entityListEdit());
    };
}

/**
 * Redux Action method for loading a list of contexts of an entity
 * @param {string} site - The site
 * @param {string} entity - The entity
 * @param {object} newContextModel - Model for a context the list must include
 * @return {object} The contexts
 */
export function loadEntityContexts(site, entity, newContextModel) {
    return function load(dispatch) {
        dispatch(entityLoadContextsRequest(entity));
        const entityAPI = new EntityAPI(site);
        return entityAPI.loadContextsList(site, entity, newContextModel)
            .then(contexts => {
                dispatch(entityLoadContextsSuccess(entity, contexts));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for loading a list of entities.
 * @param {string} site - The site
 * @param {string} type - The type of entity
 * @return {object} The models for that entity type
 */
export function loadModelsFromEntityType(site, type) {
    return function load(dispatch) {
        dispatch(entityLoadModelsFromEntityTypeRequest(type));
        const entityAPI = new EntityAPI(site);
        return entityAPI.loadModelsFromEntityType(site, type)
            .then(models => {
                dispatch(entityLoadModelsFromEntityTypeSuccess(type, models));
            }).catch(error => {
                throw (error);
            });
    };
}
