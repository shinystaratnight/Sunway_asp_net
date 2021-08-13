import * as ContextLookups from 'constants/contextLookups';

import DateFunctions from 'library/datefunctions';
import EntityValidator from 'library/entityvalidator';
import ObjectFunctions from 'library/objectfunctions';
import StringFunctions from 'library/stringfunctions';
import WebRequest from 'library/webrequest';

class EntityAPI {
    constructor(site) {
        this.entityValidator = new EntityValidator(site);
        this.mode = '';
        this.site = StringFunctions.safeUrl(site);
    }

    /**
     * Function for loading an entity.
     * @param {string} site - The site
     * @param {string} name - The name of the entity
     * @param {string} context - The entity context
     * @param {string} mode - The entity mode (draft or live).
     * @return {object} The entity
     */
    loadEntity(site, name, context, mode) {
        const safeUrlContext = StringFunctions.safeUrl(context);
        const entityParams = `${site}/${name}/${safeUrlContext}/${mode}`;
        const entityUrl = `/booking/api/sitebuilder/entity/${entityParams}`;
        const fetchOptions = {
            credentials: 'omit',
        };
        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                let entity = {};
                if (response.ok) {
                    const result = response.object;
                    entity = {
                        site,
                        context: result.Context,
                        name: result.Name,
                        jsonSchema: JSON.parse(result.JsonSchema),
                        model: JSON.parse(result.Model),
                        lastModifiedDate: DateFunctions.parseISOLocal(result.LastModifiedDate),
                        lastModifiedUser: result.LastModifiedUser,
                        status: result.Status,
                        type: result.Type,
                    };
                    this.mode = mode;
                    this.site = site;
                    entity = this.entityValidator.validateEntity(entity);
                }
                return entity;
            });
    }

    /**
     * Function for updating an entity.
     * @param {object} entity - The entity to update
     * @param {string} field - The field to update
     * @param {string} value - The new value to set
     * @return {object} The updated entity
     */
    updateEntityValue(entity, field, value) {
        const updatedEntity = ObjectFunctions.setValueByStringPath(entity, field, value);
        updatedEntity.lastModifiedDate = new Date();
        return this.entityValidator.validateEntity(updatedEntity);
    }

    /**
     * Function for saving an entity.
     * @param {object} entity - The entity to update
     * @return {object} The entity
     */
    saveEntity(entity) {
        let entityUrl = '/booking/api/sitebuilder/model/';
        entityUrl += `${entity.site}/${entity.name}/${entity.context}`;
        if (entity.type.toLowerCase() === 'custom'
            || entity.type.toLowerCase() === 'configuration') {
            entityUrl = `${entityUrl}?publish=true`;
        }
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'PUT',
            credentials: 'same-origin',
            body: JSON.stringify(entity.model),
        };

        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                const updatedEntity = Object.assign({}, entity);
                if (response.ok) {
                    updatedEntity.lastModifiedDate = new Date();
                    updatedEntity.status = 'draft';
                    this.mode = 'draft';
                }
                return this.entityValidator.validateEntity(updatedEntity);
            });
    }

    /**
     * Function for publishing an entity.
     * @param {string} site - The site
     * @param {string} name - The name of the entity
     * @param {string} context - The entity context
     * @param {object} entity - The entity to update
     * @return {object} The entity
     */
    publishEntity(site, name, context, entity) {
        const entityUrl = `/booking/api/sitebuilder/model/${site}/${name}/${context}/publish`;
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'PUT',
            credentials: 'same-origin',
        };

        return WebRequest.send(entityUrl, fetchOptions)
            .then(response => {
                const updatedEntity = Object.assign({}, entity);
                if (response.ok) {
                    updatedEntity.lastModifiedDate = new Date();
                    updatedEntity.status = 'live';
                    this.mode = 'live';
                }
                return this.entityValidator.validateEntity(updatedEntity);
            });
    }

    /**
     * Function for deleting an entity.
     * @param {string} site - The site
     * @param {string} name - The name of the entity
     * @param {string} context - The entity context
     * @return {object} The result
     */
    deleteEntity(site, name, context) {
        const entityUrl = `/booking/api/sitebuilder/model/${site}/${name}/${context}`;
        const fetchOptions = {
            headers: {
                'Content-Type': 'application/json',
            },
            method: 'DELETE',
            credentials: 'same-origin',
        };

        return WebRequest.send(entityUrl, fetchOptions)
            .then(result => result);
    }


    /**
     * Function for loading a list of entities.
     * @param {string} site - The site
     * @param {string} type - The entity type
     * @return {object} The entities
     */
    loadEntityList(site, type) {
        let entitiesUrl = `/booking/api/sitebuilder/entities/${site}`;
        if (type) {
            entitiesUrl = `${entitiesUrl}?type=${type}`;
        }

        return WebRequest.send(entitiesUrl)
            .then(response => {
                const entities = [];
                if (response.ok) {
                    response.object.Entities.forEach(entity => {
                        const customEntity = {
                            name: entity.Name,
                            type: entity.Type,
                            site,
                            contexts: [],
                        };
                        entities.push(customEntity);
                    });
                }
                return entities;
            });
    }

    /**
     * Function for loading a list of contexts from an entity
     * @param {string} site - The site
     * @param {string} entity - The entity
     * @param {object} newContextModel - Model for a context the list must include
     * @return {object} The contexts
     */
    loadContextsList(site, entity, newContextModel) {
        if (ContextLookups.entities.hasOwnProperty(entity)) {
            const contextLookup = ContextLookups.entities[entity];
            const options = {
                credentials: 'omit',
            };
            return WebRequest.send(contextLookup.apiUrl, options)
                .then(response => {
                    const contexts = [];
                    if (response.ok) {
                        response.object.forEach(lookup => {
                            let context = lookup.Name;
                            if (contextLookup.context) {
                                context = contextLookup.context(lookup);
                            }
                            contexts.push(context);
                        });
                    }
                    return contexts;
                });
        }
        const contextsUrl = `/booking/api/sitebuilder/${site}/${entity}/contexts`;
        return WebRequest.send(contextsUrl)
            .then(response => {
                let contexts = [];
                if (response.ok) {
                    contexts = response.object;
                }
                if (newContextModel
                    && entity === newContextModel.name
                    && contexts.findIndex(c => c === newContextModel.context) === -1) {
                    contexts.push(newContextModel.context);
                }
                return contexts;
            });
    }

    /**
    * Function for loading models of a given entity type
    * @param {string} site - The site
    * @param {string} entityType - The entity type
    * @return {object} The contexts
    */
    loadModelsFromEntityType(site, entityType) {
        const modelsUrl = `/booking/api/sitebuilder/models/${site}/${entityType}`;
        this.items = [];
        const options = {
            credentials: 'omit',
        };

        return WebRequest.send(modelsUrl, options)
            .then(response => {
                if (response.ok) {
                    const models = response.object;
                    Object.keys(models)
                        .map(key => {
                            const item = JSON.parse(models[key]);
                            if (item && typeof item === 'object') {
                                item.context = key;
                                this.items.push(item);
                            }
                        });
                }
                return this.items;
            });
    }
}

export default EntityAPI;
