import ArrayFunctions from '../library/arrayfunctions';
import DateFunctions from '../library/datefunctions';
import ObjectFunctions from '../library/objectfunctions';
import StringFunctions from '../library/stringfunctions';

import fetch from 'isomorphic-fetch';
import moment from 'moment';

const FILTER_TYPES = {
    DATE: 'DATE',
    ARRAY: 'ARRAY',
};

const FILTER_OPTIONS = [
    {
        name: 'archive',
        type: FILTER_TYPES.DATE,
        title: 'Archive',
        field: 'Date',
        selectedValue: -1,
        defaultValue: -1,
        options: [],
    },
    {
        name: 'category',
        type: FILTER_TYPES.ARRAY,
        title: 'Categories',
        field: 'Categories',
        selectedValue: 'Categories',
        defaultValue: 'Categories',
        options: [],
    },
];


class BlogListAPI {
    constructor() {
        this.items = [];
        this.filters = [];
        this.resultsPerPage = 5;
        this.totalPages = 1;
    }
    setupBlogList(site, user, categories, overloadFilterOptions = []) {
        const filterOptions = FILTER_OPTIONS;
        if (overloadFilterOptions) {
            for (let i = 0; i < overloadFilterOptions.length; i++) {
                for (let j = 0; j < FILTER_OPTIONS.length; j++) {
                    if (overloadFilterOptions[i].name === filterOptions[j].name) {
                        filterOptions[j].selectedValue = overloadFilterOptions[i].selectedValue;
                    }
                }
            }
        }
        return this.getBlogs(site, user, categories)
            .then(blogs => this.setupFilters(blogs, filterOptions))
            .then(() => this.filter(this.items, this.filters))
            .then(() => {
                const blogList = {
                    items: this.items,
                    filters: this.filters,
                    totalPages: this.totalPages,
                };
                return blogList;
            });
    }
    getBlogs(site, user, categories) {
        const blogsURL = `/api/sitebuilder/models/${site}/BlogItem`;
        this.items = [];
        return fetch(blogsURL)
            .then(response => response.json())
            .then(blogs => {
                Object.keys(blogs)
                    .map(key => {
                        const item = JSON.parse(blogs[key]);
                        if (item !== null && typeof item === 'object') {
                            const blogDateMoment = moment(item.Date);
                            item.dateId = parseInt(blogDateMoment.format('YYYYMMDD'), 10);
                            item.context = key;
                            const inDate = DateFunctions.hasValidPublishDates(item);

                            let hasInvalidCategory = false;
                            if (!user.TradeSession.LoggedIn) {
                                const invalidCategories
                                    = categories.filter(category => category.AgentsOnly);
                                for (let i = 0; i < item.Categories.length; i++) {
                                    const categoryName = item.Categories[i];
                                    if (invalidCategories.findIndex(category =>
                                            category.Name === categoryName) > -1) {
                                        hasInvalidCategory = true;
                                        break;
                                    }
                                }
                            }

                            if (inDate && !hasInvalidCategory) {
                                this.items.push(item);
                            }
                        }
                    });
                this.items.sort((a, b) => {
                    if (a.dateId > b.dateId) {
                        return -1;
                    }
                    if (a.dateId < b.dateId) {
                        return 1;
                    }
                    return 0;
                });
                return this.items;
            });
    }
    setupFilters(blogs, filterOptions) {
        this.filters = [];
        return new Promise((resolve) => {
            filterOptions.forEach(filterOption => {
                this.addFilter(blogs, filterOption);
            });
            resolve(this.filters);
        });
    }
    addFilter(blogs, filterOption) {
        switch (filterOption.type) {
            case FILTER_TYPES.DATE:
                this.setupDateFilter(blogs, filterOption);
                break;
            case FILTER_TYPES.ARRAY:
                this.setupArrayFilter(blogs, filterOption);
                break;
            default:
        }
    }
    setupDateFilter(blogs, filterOption) {
        const filter = filterOption;
        const ids = [];

        const defaultOption = {
            Id: -1,
            Name: filterOption.title,
        };
        filter.options.push(defaultOption);

        blogs.forEach(blog => {
            const blogDateMoment = moment(blog[filterOption.field]);
            const monthYearId = parseInt(blogDateMoment.format('YYYYMM'), 10);
            if (ids.indexOf(monthYearId) === -1) {
                const option = {
                    Id: monthYearId,
                    Name: blogDateMoment.format('MMM YYYY'),
                };
                ids.push(monthYearId);
                filter.options.push(option);
            }
        });

        ArrayFunctions.sortByPropertyDescending(filter.options, 'id');
        this.filters.push(filter);
    }
    setupArrayFilter(blogs, filterOption) {
        const filter = filterOption;
        const options = [];

        options.push(filterOption.title);

        blogs.forEach(blog => {
            blog[filterOption.field].forEach(item => {
                if (options.indexOf(item) === -1) {
                    options.push(item);
                }
            });
        });
        filter.options = options;
        this.filters.push(filter);
    }
    updateFilter(filters, key, value) {
        const updatedFilters = ObjectFunctions.setValueByStringPath(filters, key, value);
        return updatedFilters;
    }
    filter(blogs, filters) {
        for (let i = 0; i < blogs.length; i++) {
            const blog = blogs[i];
            const validReturn = this.validBlog(blog, filters);
            blog.Display = validReturn.valid;
            blog.ValidExcludes = validReturn.validExcludes;
        }
        return blogs;
    }
    validBlog(blog, filters) {
        const validReturn = {
            valid: true,
            validExcludes: [],
        };

        filters.forEach(filter => {
            validReturn.validExcludes.push({ name: filter.name, display: true });
        });

        for (let i = 0; i < filters.length; i++) {
            const filter = filters[i];
            const valid = this.validFilter(blog, filter);
            if (!valid) {
                validReturn.valid = false;
                for (let j = 0; j < validReturn.validExcludes.length; j++) {
                    const exclude = validReturn.validExcludes[j];
                    if (exclude.name !== filter.name) {
                        exclude.display = false;
                    }
                }
            }
        }
        return validReturn;
    }
    validFilter(blog, filter) {
        let valid = true;
        switch (filter.type) {
            case FILTER_TYPES.DATE:
                valid = this.validDateValue(blog, filter);
                break;
            case FILTER_TYPES.ARRAY:
                valid = this.validArrayValue(blog, filter);
                break;
            default:
        }
        return valid;
    }
    validDateValue(blog, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(blog, filter.field);
        const blogDateMoment = moment(value);
        const monthYearId = parseInt(blogDateMoment.format('YYYYMM'), 10);
        const selectedValue = filter.selectedValue;
        if (selectedValue === -1 || selectedValue === monthYearId) {
            valid = true;
        }
        return valid;
    }
    validArrayValue(blog, filter) {
        let valid = false;
        const value = ObjectFunctions.getValueByStringPath(blog, filter.field);
        const safeUrlValue = [];
        value.forEach(v => {
            safeUrlValue.push(StringFunctions.safeUrl(v));
        });
        const selectedValue = StringFunctions.safeUrl(filter.selectedValue.toString());
        if (StringFunctions.safeUrl(selectedValue) === StringFunctions.safeUrl(filter.title)
            || safeUrlValue.indexOf(selectedValue) !== -1) {
            valid = true;
        }
        return valid;
    }
}

export default BlogListAPI;
