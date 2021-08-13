import * as types from './actionTypes';

/**
 * Redux Action method for requesting a property
 * @param {number} propertyId - The property id
 * @return {object} The action type and properties object
 */
function propertyRequest(propertyId) {
    return { type: types.PROPERTY_LOAD_REQUEST, propertyId };
}

/**
 * Redux Action method for successfully loading a property.
 * @param {object} property - The property object to return
 * @param {number} propertyId - The property id
 * @return {object} The action type and properties object
 */
function propertyLoadSuccess(property, propertyId) {
    return { type: types.PROPERTY_LOAD_SUCCESS, property, propertyId };
}


/**
 * Redux Action method for loading a property
 * @param {number} propertyId - The property id
 * @return {object} The property
 */
export function loadProperty(propertyId) {
    return function load(dispatch) {
        dispatch(propertyRequest(propertyId));
        let propertyUrl = `/booking/api/cms/propertyfull/${propertyId}`;
        if (document.getElementById('hidApiBaseUrl')) {
            const apiBaseUrl = document.getElementById('hidApiBaseUrl').value;
            propertyUrl = `${apiBaseUrl}${propertyUrl}`;
        }
        return fetch(propertyUrl)
            .then(response => response.json())
            .then(result => {
                const propertyContent = JSON.parse(result.ContentJSON);
                dispatch(propertyLoadSuccess(propertyContent.Property, propertyId));
            })
            .catch(error => {
                throw (error);
            });
    };
}
