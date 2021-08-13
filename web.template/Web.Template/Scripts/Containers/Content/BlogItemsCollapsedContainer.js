import * as BlogListActions from '../../actions/content/blogListActions';
import * as EntityActions from '../../actions/content/entityActions';

import BlogItemsCollapsed from '../../widgets/content/blogitemscollapsed';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BlogItemsCollapsedContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    getBlogName() {
        const blogItems = this.props.page.EntityInformations.filter(entityInfo =>
            entityInfo.Name === 'blogitem');
        const blogName = blogItems[0] ? blogItems[0].Value : '';
        return blogName;
    }
    componentDidMount() {
        const blogName = this.getBlogName();
        if (blogName) {
            this.props.actions.loadEntity(this.props.site.Name, 'BlogItem', blogName, 'live');
        }
        this.props.actions.loadBlogList(this.props.site.Name);
    }
    getEntities() {
        const blogs = [];
        this.props.blogList.items.forEach(blogItem => {
            blogs.push(
                {
                    name: blogItem.context.toString(),
                    model: blogItem,
                }
            );
        });
        const returnObject = {
            blogs,
            parentBlog: this.props.parentBlog,
        };
        return returnObject;
    }
    matchingCategory(blogA, blogB) {
        for (let i = 0; i < blogA.Categories.length; i++) {
            for (let j = 0; j < blogB.model.Categories.length; j++) {
                if (blogA.Categories[i] === blogB.model.Categories[j]) {
                    return true;
                }
            }
        }
        return false;
    }
    getDisplayBlogs(blogs, parentBlog, numberOfBlogs) {
        let displayBlogs = [];
        const sortedBlogs = blogs.sort((a, b) => {
            if (a.model.Date > b.model.Date) {
                return -1;
            }
            if (a.model.Date < b.model.Date) {
                return 1;
            }
            return 0;
        });
        const matchingBlogs = sortedBlogs.filter(blog => this.matchingCategory(parentBlog, blog));
        if (matchingBlogs.length < numberOfBlogs) {
            const numberAdditionalBlogs = numberOfBlogs - matchingBlogs.length;
            const unusedBlogs
                = sortedBlogs.filter(blog => !this.matchingCategory(parentBlog, blog));
            const additionalBlogs = unusedBlogs.slice(0, numberAdditionalBlogs);
            displayBlogs = matchingBlogs.concat(additionalBlogs);
        } else {
            displayBlogs = matchingBlogs.slice(0, numberOfBlogs);
        }
        return displayBlogs;
    }
    getProps() {
        const contentModel = this.props.entity.model;
        const entities = this.getEntities();
        const blogs = entities.blogs;
        const parentBlog = entities.parentBlog;
        const filteredBlogs = blogs.filter(blog => blog.name !== parentBlog.name);
        const numberOfBlogsToDisplay = 3;
        const displayedBlogs
            = this.getDisplayBlogs(filteredBlogs, parentBlog, numberOfBlogsToDisplay);
        const propBlogs = [];
        displayedBlogs.forEach(blog => {
            propBlogs.push(blog.model);
        });
        const collapsedBlogsProps = {
            title: contentModel.Title,
            blogs: propBlogs,
        };
        return collapsedBlogsProps;
    }
    render() {
        return (
            <div>
                {this.props.blogList.isLoaded
                    && this.props.parentBlog
                    && this.props.parentBlog.MarkdownText
                    && <BlogItemsCollapsed {...this.getProps()} />}
            </div>
        );
    }
}

BlogItemsCollapsedContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    blogList: React.PropTypes.object.isRequired,
    parentBlog: React.PropTypes.object,
    page: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const blogList = state.blogList ? state.blogList : {};
    const blogItems = state.page.EntityInformations.filter(entityInfo =>
        entityInfo.Name === 'blogitem');
    const blogName = blogItems[0] ? blogItems[0].Value : '';
    const blogKey = `BlogItem-${blogName}`;
    const blogEntity = state.entities[blogKey] ? state.entities[blogKey] : {};
    return {
        blogList,
        parentBlog: blogEntity.model,
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

export default connect(mapStateToProps, mapDispatchToProps)(BlogItemsCollapsedContainer);
