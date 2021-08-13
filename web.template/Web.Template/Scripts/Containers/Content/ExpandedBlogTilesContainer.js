import * as BlogListActions from '../../actions/content/blogListActions';
import * as EntityActions from '../../actions/content/entityActions';

import BlogList from '../../widgets/content/expandedblogtiles';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class ExpandedBlogTilesContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
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
            this.props.actions.loadBlogList(this.props.site.Name, user, categories);
        }
    }
    blogListProps() {
        const blogs = this.props.blogList.items;
        const featuredTileCount = 6;
        const displayedBlogItems = blogs.filter(blog => blog.Display);
        const blogItems = displayedBlogItems.slice(0, featuredTileCount);
        const props = {
            blogItems,
            title: this.props.entity.model.Title,
            leadParagraph: this.props.entity.model.LeadParagraph,
            tilesButtonText: this.props.entity.model.TilesButtonText,
        };
        return props;
    }
    render() {
        return (
            <div className="blog-list-container container">
                {this.props.blogList.isLoaded
                    && <BlogList {...this.blogListProps()} />}
            </div>
        );
    }
}

ExpandedBlogTilesContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    blogList: React.PropTypes.object.isRequired,
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
    const blogCategories = state.entities['CustomLookup-blogitemcategories']
        ? state.entities['CustomLookup-blogitemcategories'] : { isLoaded: false };

    return {
        blogList,
        blogCategories,
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
        EntityActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(ExpandedBlogTilesContainer);
