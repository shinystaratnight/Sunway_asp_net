import * as types from './actionTypes';
import PropertiesApi from '../../api/PropertiesAPI';

/**
 * Redux Action method for requesting properties.
 * @param {string} key - The key of the properties object to return
 * @return {object} The action type and properties object
 */
function propertiesRequest(key) {
    return { type: types.PROPERTIES_LOAD_REQUEST, key };
}

/**
 * Redux Action method for successfully loading properties.
 * @param {object} properties - The properties object to return
 * @param {string} key - The key of the properties object to return
 * @return {object} The action type and properties object
 */
function propertiesLoadSuccess(properties, key) {
    return { type: types.PROPERTIES_LOAD_SUCCESS, properties, key };
}

/**
 * Redux Action method for updating the current page
 * @param {number} page - The selected page number
 * @param {string} key - The unique ID of the offers concerned
 * @return {object} The action type and page number
 */
export function propertiesUpdatePage(page, key) {
    return { type: types.PROPERTIES_UPDATE_PAGE, page, key };
}

/**
 * Redux Action method for updating the current page
 * @param {string} key - The unique ID of the offers concerned
 * @param {array} properties - The now sorted properties
 * @param {string} selectedSort - The selected sort option
 * @return {object} The action type and page number
 */
export function propertiesSorted(key, properties, selectedSort) {
    return { type: types.PROPERTIES_UPDATE_SORT, key, items: properties, selectedSort };
}

/**
 * Redux Action method for loading the current properties.
 * @param {object} options - The values that will be used to build up the url.
 * @return {object} The properties
 */
export function getProperties(options) {
    return function load(dispatch) {
        dispatch(propertiesRequest(options.key));
        const propertyApi = new PropertiesApi();
        return propertyApi.setupProperties(options)
            .then(properties => {
                dispatch(propertiesLoadSuccess(properties, options.key));
            })
            .catch(error => {
                throw (error);
            });
    };
}


/**
 * Redux Action method for sorting the current properties.
 * @param {string} key - The key of the properties on the state
 * @param {array} properties - The properties to be sorted
 * @param {string} selectedSort - The sort option
 * @return {object} The properties
 */
export function sortProperties(key, properties, selectedSort) {
    return function load(dispatch) {
        const propertyApi = new PropertiesApi();
        const sortedProperties = propertyApi.sortProperties(properties, selectedSort);
        dispatch(propertiesSorted(key, sortedProperties, selectedSort));
    };
}
