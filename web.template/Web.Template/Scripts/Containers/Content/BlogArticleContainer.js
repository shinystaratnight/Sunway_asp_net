import * as EntityActions from 'actions/content/entityActions';

import BlogArticle from 'widgets/content/blogarticle';
import DateFunctions from 'library/datefunctions';

import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BlogArticleContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            blogInit: false,
        };
    }
    getBlogName() {
        const blogItems = this.props.page.EntityInformations.filter(entityInfo =>
            entityInfo.Name === 'blogitem');
        const blogName = blogItems[0] ? blogItems[0].Value : '';
        return blogName;
    }
    componentDidMount() {
        if (!this.state.blogInit) {
            const blogName = this.getBlogName();
            if (blogName) {
                this.props.actions.loadEntity(this.props.site.Name, 'BlogItem', blogName, 'live');
                this.setState({
                    blogInit: true,
                });
            }
        }
    }
    redirectToHomePage() {
        window.location = '/blog';
    }
    render() {
        const blogArticleProps = this.props.blog ? this.props.blog.model : {};
        const renderArticle = this.state.blogInit
                            && this.props.blog.isLoaded
                            && blogArticleProps.MarkdownText;
        blogArticleProps.siteName = this.props.session.UserSession.SelectedCmsWebsite.Name;
        const liveBlog = this.state.blogInit
            && DateFunctions.hasValidPublishDates(this.props.blog);
        if (this.state.blogInit && !liveBlog) {
            this.redirectToHomePage();
        }
        return (
            <div className="widget widget-blog-item">
                {renderArticle
                    && <BlogArticle {...blogArticleProps} />}
            </div>
        );
    }
}

BlogArticleContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    blog: React.PropTypes.object,
    page: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const blogItems = state.page.EntityInformations.filter(entityInfo =>
        entityInfo.Name === 'blogitem');
    const blogName = blogItems[0] ? blogItems[0].Value : '';
    const blogKey = `BlogItem-${blogName}`;
    const blog = state.entities[blogKey] ? state.entities[blogKey] : {};
    return {
        blog,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions
        );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BlogArticleContainer);
