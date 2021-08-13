import * as BlogListActions from '../../actions/content/blogListActions';
import * as EntityActions from '../../actions/content/entityActions';
import * as TwitterActions from '../../actions/content/twitterActions';

import BlogList from '../../widgets/content/bloglist';
import BlogListFilter from '../../widgets/content/bloglistfilter';
import React from 'react';
import UrlFunctions from '../../library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BlogListContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
        this.handleFilterChange = this.handleFilterChange.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadEntity(this.props.site.Name,
            'CustomLookup', 'blogitemcategories', 'live');
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.blogCategories.isLoaded
            && nextProps.blogCategories.isLoaded) {
            const user = this.props.session.UserSession;
            const categories = nextProps.blogCategories.model.Items;
            const queryString = UrlFunctions.getQueryStringValue('category');
            const initialFilterSettings = [];
            if (queryString) {
                const selectedValue = queryString.replace(/-/g, ' ');
                const filterSettings = {
                    name: 'category',
                    selectedValue,
                };
                initialFilterSettings.push(filterSettings);
            }
            this.props.actions.loadBlogList(this.props.site.Name,
                user, categories, initialFilterSettings);
        }
    }
    handleFilterChange(event) {
        const field = event.target.name;
        let value = event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateFilterValue(field, value);
    }
    updateFilterValue(field, value) {
        this.props.actions.blogListFilterUpdateValue(field, value);
    }
    blogListFilterProps() {
        const blogs = this.props.blogList.items.slice(1);
        const props = {
            blogItems: blogs,
            filters: this.props.blogList.filters,
            title: this.props.entity.model.FilterTitle,
            summary: this.props.entity.model.FilterSummary,
            onChange: this.handleFilterChange,
        };
        return props;
    }
    isFiltered() {
        let isFiltered = false;
        for (let i = 0; i < this.props.blogList.filters.length; i++) {
            const filter = this.props.blogList.filters[i];
            if (filter.selectedValue !== filter.defaultValue) {
                isFiltered = true;
                break;
            }
        }
        return isFiltered;
    }
    blogListProps() {
        // first item displayed in hero banner so will not be used here
        const blogs = this.props.blogList.items.slice(1);
        const featuredTileCount = 3;
        const displayedBlogItems = blogs.filter(blog => blog.Display);
        const featuredBlogItems = displayedBlogItems.slice(0, featuredTileCount);
        const blogItems = displayedBlogItems.slice(featuredTileCount);
        const props = {
            featuredBlogItems,
            blogItems,
            tweets: this.props.twitter.tweets,
            displayTweets: !this.isFiltered(),
            tilesHeader: this.props.entity.model.TilesHeader,
            tilesButtonText: this.props.entity.model.TilesButtonText,
        };
        return props;
    }
    render() {
        return (
            <div className="blog-list-container container">
                {this.props.blogList.isLoaded
                    && <BlogListFilter {...this.blogListFilterProps()} />}
                {this.props.blogList.isLoaded
                    && this.props.twitter.isLoaded
                    && <BlogList {...this.blogListProps()} />}
            </div>
        );
    }
}

BlogListContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    blogList: React.PropTypes.object.isRequired,
    twitter: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    blogCategories: React.PropTypes.object.isRequired,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const blogList = state.blogList ? state.blogList : {};
    const twitter = state.twitter ? state.twitter : {};
    const blogCategories = state.entities['CustomLookup-blogitemcategories']
        ? state.entities['CustomLookup-blogitemcategories'] : { isLoaded: false };
    return {
        blogCategories,
        blogList,
        twitter,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BlogListActions,
        EntityActions,
        TwitterActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BlogListContainer);
