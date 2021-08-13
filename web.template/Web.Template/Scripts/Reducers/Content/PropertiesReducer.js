import * as types from '../../actions/content/actionTypes';

const sortOptions = [
    { Id: 'Default', displayValue: 'Default' },
    { Id: 'PriceAscending', displayValue: 'Price (Low to High)' },
    { Id: 'PriceDescending', displayValue: 'Price (High to Low)' },
    { Id: 'NameAscending', displayValue: 'Hotel name (A-Z)' },
    { Id: 'NameDescending', displayValue: 'Hotel name (Z-A)' },
    { Id: 'StarAscending', displayValue: 'Star rating (Low to High)' },
    { Id: 'StarDescending', displayValue: 'Star rating (High to Low)' },
];

const initialPropertiesState = {
    isFetching: false,
    isLoaded: false,
    isUpdating: false,
    key: '',
    items: [],
    currentPage: 1,
    totalPages: 1,
    resultsPerPage: 10,
    sortOptions,
    selectedSort: 'default',
};

/**
 * Redux Reducer for the page
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function propertiesReducer(state = {}, action) {
    const updatedProperties = Object.assign({}, state);
    switch (action.type) {
        case types.PROPERTIES_LOAD_REQUEST:
            if (!updatedProperties.hasOwnProperty(action.key)) {
                updatedProperties[action.key] = initialPropertiesState;
            }
            updatedProperties[action.key].isFetching = true;
            updatedProperties[action.key].isLoaded = false;
            updatedProperties[action.key].key = action.key;
            break;
        case types.PROPERTIES_LOAD_SUCCESS: {
            const updates = {
                items: action.properties.items,
                isFetching: false,
                isLoaded: true,
                currentPage: 1,
                totalPages: action.properties.totalPages,
                resultsPerPage: action.properties.resultsPerPage,
                selectedSort: action.properties.selectedSort,
            };
            updatedProperties[action.key]
                = Object.assign({}, updatedProperties[action.key], updates);
            break;
        }
        case types.PROPERTIES_UPDATE_PAGE: {
            const updates = {
                currentPage: action.page,
            };
            updatedProperties[action.key]
                = Object.assign({}, updatedProperties[action.key], updates);
            break;
        }
        case types.PROPERTIES_UPDATE_SORT: {
            const updates = {
                selectedSort: action.selectedSort,
                currentPage: 1,
                items: action.items,
            };
            updatedProperties[action.key]
                = Object.assign({}, updatedProperties[action.key], updates);
        }
            break;
        default:
    }
    return updatedProperties;
}
