import * as types from './actionTypes';
import BlogListApi from '../../api/blogListAPI';

/**
 * Redux Action method for requesting loading blogs.
 * @return {object} The action type
 */
function blogListLoadRequest() {
    return { type: types.BLOGLIST_LOAD_REQUEST };
}

/**
 * Redux Action method for successfully loading blogs.
 * @param {object} blogList - The blog list
 * @return {object} The action type and blog list object
 */
function blogListLoadSuccess(blogList) {
    return { type: types.BLOGLIST_LOAD_SUCCESS, blogList };
}

/**
 * Redux Action method for updating filters
 * @param {object} filters - The filters to return
 * @return {object} the action type and filters object
 */
function blogListFilterUpdate(filters) {
    return { type: types.BLOGLIST_FILTER_UPDATE, filters };
}

/**
 * Redux Action method for updating blogs
 * @param {object} blogs - The blogs to return
 * @return {object} the action type and blogs
 */
function blogListUpdated(blogs) {
    return { type: types.BLOGLIST_RESULTS_UPDATED, blogs };
}

/**
 * Redux Action method for loading blogs.
 * @param {string} site - The site
 * @param {object} user - The user
 * @param {array} categories - Array of blog item categories
 * @param {array} overloadFilterOptions - An array of overload settings
 * @return {object} The blog list
 */
export function loadBlogList(site, user, categories, overloadFilterOptions = []) {
    const overloadDefaultFilters = [];
    overloadFilterOptions.forEach(option => {
        const filter = {
            name: option.name,
            selectedValue: option.selectedValue,
        };
        overloadDefaultFilters.push(filter);
    });
    return function load(dispatch) {
        dispatch(blogListLoadRequest());
        const blogListApi = new BlogListApi();
        return blogListApi.setupBlogList(site, user, categories, overloadDefaultFilters)
            .then(blogList => {
                dispatch(blogListLoadSuccess(blogList));
            }).catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for filtering blogs
 * @param {object} blogs - The blogs to filter
 * @param {object} filters - The filters to apply
 * @return {object} The filtered results
 */
function filterBlogList(blogs, filters) {
    return function load(dispatch) {
        const blogListApi = new BlogListApi();
        const filteredBlogs = blogListApi.filter(blogs, filters);
        dispatch(blogListUpdated(filteredBlogs));
    };
}

/**
 * Redux Action method for updating blog list filter value
 * @param {string} field - The property to update
 * @param {*} value - The new value
 * @return {object} the action
 */
export function blogListFilterUpdateValue(field, value) {
    return function load(dispatch, getState) {
        const blogListApi = new BlogListApi();
        const state = getState();
        const filters = state.blogList.filters;
        const updatedFilters = blogListApi.updateFilter(filters, field, value);
        dispatch(blogListFilterUpdate(updatedFilters));
        dispatch(filterBlogList(state.blogList.items, updatedFilters));
    };
}
