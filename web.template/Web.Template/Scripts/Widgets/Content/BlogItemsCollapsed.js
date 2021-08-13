import '../../../styles/widgets/content/_blogList.scss';

import React from 'react';
import StringFunctions from '../../library/stringfunctions';
import moment from 'moment';

export default class BlogItemsCollapsed extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderBlogTag(category, index) {
        const key = `$blog-tag-${category}-${index}`;
        const linkUrl = `/blog?category=${StringFunctions.safeUrl(category)}`;
        return (
            <li key={key} className="blog-tag">
                <a href={linkUrl}>
                    {category}
                </a>
            </li>
        );
    }
    renderBlogHeader(blog) {
        const dateTime = blog.Date;
        const momentDate = moment(blog.Date, 'YYYY-MM-DD');
        const displayDate = momentDate.format('dddd[,] D MMMM YYYY');
        const urlTitle = StringFunctions.safeUrl(blog.context);
        const blogUrl = `/blog/${urlTitle}`;
        const blogName = blog.Title;
        return (
            <header className="blog-header">
                <time dateTime={dateTime} className="blog-date">{displayDate}</time>
                {blog.Categories
                    && blog.Categories.length > 0
                    && <ul className="blog-tags">
                           {blog.Categories.map(this.renderBlogTag, this)}
                       </ul>}
                <a href={blogUrl}>
                    <h3 className="h-tertiary h-link blog-title">{blogName}</h3>
                </a>
            </header>
        );
    }
    renderBlog(blog, index) {
        const key = `blog-${index}`;
        const imageAttributes = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${blog.TileImage}')`,
            },
        };
        const urlTitle = StringFunctions.safeUrl(blog.context);
        const blogUrl = `/blog/${urlTitle}`;
        let containerClasses = 'col-xs-12 col-md-4 article-preview-container';
        containerClasses
            = index === (this.props.blogs.length - 1)
            ? `${containerClasses} hidden-sm`
            : `${containerClasses} col-sm-6`;
        return (
            <div key={key} className={containerClasses}>
                <div className="article-preview highlight">
                    <div className="img-container">
                        <div {...imageAttributes}></div>
                    </div>
                    <div className="blog-copy">
                        {this.renderBlogHeader(blog)}
                        <a href={blogUrl} className="arrow-link">
                            Read More
                        </a>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        const renderBlogs = this.props.blogs.length > 0;
        return (
            <div className="blog-items-collapsed container">
                <div className="row">
                    {this.props.title
                        && <div className="col-xs-12 articles-preview-header">
                               <h3 className="h-tertiary text-align-left-xs">
                                   {this.props.title}
                               </h3>
                           </div>}
                    {renderBlogs
                        && this.props.blogs.map(this.renderBlog, this)}
                </div>
            </div>
        );
    }
}

BlogItemsCollapsed.propTypes = {
    title: React.PropTypes.string,
    blogs: React.PropTypes.array.isRequired,
};
