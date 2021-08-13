import '../../../styles/widgets/content/_blogList.scss';

import React from 'react';
import StringFunctions from '../../library/stringfunctions';
import moment from 'moment';

export default class BlogList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            columns: 3,
            displayedPages: 1,
        };
        this.loadNextPage = this.loadNextPage.bind(this);
        this.handleResize = this.handleResize.bind(this);
    }
    componentDidMount() {
        window.addEventListener('resize', this.handleResize);
        this.handleResize();
    }
    componentWillUnmount() {
        window.removeEventListener('resize', this.handleResize);
    }
    handleResize() {
        const windowWidth = window.innerWidth;
        const mdWidth = 992;
        const smWidth = 768;
        const mdColumns = 3;
        const smColumns = 2;

        let columns = 1;
        if (windowWidth >= mdWidth) {
            columns = mdColumns;
        } else if (windowWidth >= smWidth) {
            columns = smColumns;
        }

        if (columns !== this.state.columns) {
            this.setState({ columns, displayedPages: 1 });
        }
    }
    loadNextPage() {
        const displayedPages = this.state.displayedPages + 1;
        this.setState({ displayedPages });
    }
    renderFeaturedBlogs() {
        const primaryBlog = this.props.featuredBlogItems[0];
        const secondaryBlogs = this.props.featuredBlogItems.slice(1);
        return (
            <div className="featured-blog">
                {this.renderPrimaryTile(primaryBlog)}
                <div className="row">
                    {secondaryBlogs.map(this.renderSecondaryTile, this)}
                </div>
            </div>
        );
    }
    renderPrimaryTile(blogItem) {
        const blogTileImage = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${blogItem.TileImage}')`,
            },
        };
        const momentDate = moment(blogItem.Date, 'YYYY-MM-DD');
        const displayDate = momentDate.format('dddd[,] D MMMM YYYY');
        const urlTitle = StringFunctions.safeUrl(blogItem.context);
        const blogUrl = `/blog/${urlTitle}`;
        return (
            <article className="featured-blog-primary">
                <div className="row">
                    <div className="col-xs-12 col-md-5">
                        <div className="img-container img-zoom-hover">
                            <a href={blogUrl}>
                                <div {...blogTileImage}></div>
                            </a>
                        </div>
                    </div>
                    <div className="col-xs-12 col-md-7 blog-content-container">
                        <div className="blog-content">
                            <time dateTime={blogItem.Date}
                                className="blog-date">{displayDate}</time>
                            {blogItem.Categories
                                && blogItem.Categories.length > 0
                                && this.renderBlogTags(blogItem.Categories)}
                            <a href={blogUrl} className="h-secondary">
                                <h1 className="h-secondary h-link">{blogItem.Title}</h1>
                            </a>
                            <p className="preamble">{blogItem.LeadParagraph}</p>
                            <a className="arrow-link" href={blogUrl}>Read More</a>
                        </div>
                    </div>
                </div>
            </article>
        );
    }
    renderSecondaryTile(blogItem, index) {
        const blogTileImage = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${blogItem.TileImage}')`,
            },
        };
        const momentDate = moment(blogItem.Date, 'YYYY-MM-DD');
        const displayDate = momentDate.format('dddd[,] D MMMM YYYY');
        const urlTitle = StringFunctions.safeUrl(blogItem.context);
        const blogUrl = `/blog/${urlTitle}`;
        const imgContainerClasses
            = 'featured-blog-secondary img-container img-zoom-hover gradient-overlay';
        return (
            <div key={index} className="col-xs-12 col-md-6">
                <article className={imgContainerClasses}>
                    <div {...blogTileImage}></div>
                    <a className="a-container" href={blogUrl}></a>
                    <div className="blog-content">
                        <time dateTime={blogItem.Date}
                            className="blog-date">{displayDate}</time>
                        {blogItem.Categories
                            && blogItem.Categories.length > 0
                            && this.renderBlogTags(blogItem.Categories)}
                        <a href={blogUrl}>
                            <h1 className="h-tertiary h-link">{blogItem.Title}</h1>
                        </a>
                    </div>
                </article>
            </div>
        );
    }
    renderBlogTiles() {
        const columns = this.state.columns;
        const pageSections = [];
        for (let i = 0; i < this.state.displayedPages; i++) {
            const pageSection = {
                key: `blog-page-${i}`,
                columns: [],
            };
            for (let j = 0; j < columns; j++) {
                const index = (i * columns) + j;
                const column = {
                    key: `blog-page-${i}-col-${j}`,
                    blogItem: this.props.blogItems[index],
                    tweet: this.props.tweets[index],
                };
                pageSection.columns.push(column);
            }
            pageSections.push(pageSection);
        }
        return (
            <div className="blog-list-tiles">
                <h3 className="h-secondary center-block">{this.props.tilesHeader}</h3>
                {pageSections.map(this.renderPageSection, this)}
            </div>
        );
    }
    renderPageSection(pageSection) {
        return (
            <div key={pageSection.key} className="blog-list-page-section">
                {pageSection.columns.map(this.renderColumn, this)}
            </div>
        );
    }
    renderColumn(column, index) {
        const even = index % 2 === 0;
        return (
            <div key={column.key} className="col-xs-12 col-sm-6 col-md-4">
                {this.props.displayTweets
                    && !even
                    && column.tweet
                    && this.renderTweet(column.tweet)}
                {column.blogItem
                    && this.renderBlogTile(column.blogItem)}
                {this.props.displayTweets
                    && even
                    && column.tweet
                    && this.renderTweet(column.tweet)}
            </div>
        );
    }
    renderBlogTile(blogItem) {
        const blogTileImage = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${blogItem.TileImage}')`,
            },
        };
        const momentDate = moment(blogItem.Date, 'YYYY-MM-DD');
        const displayDate = momentDate.format('dddd[,] D MMMM YYYY');
        const urlTitle = StringFunctions.safeUrl(blogItem.context);
        const blogUrl = `/blog/${urlTitle}`;
        return (
            <div className="blog-list-tile">
                <div className="img-container img-zoom-hover">
                    <div {...blogTileImage}></div>
                </div>
                <div className="blog-content">
                    <time dateTime={blogItem.Date}
                        className="blog-date">{displayDate}</time>
                    {blogItem.Categories
                        && blogItem.Categories.length > 0
                        && this.renderBlogTags(blogItem.Categories)}
                    <a href={blogUrl}>
                        <h1 className="h-tertiary h-link">{blogItem.Title}</h1>
                    </a>
                    <p className="blog-summary">{blogItem.LeadParagraph}</p>
                    <a className="arrow-link" href={blogUrl}>Read More</a>
                </div>
            </div>
        );
    }
    renderTweet(tweet) {
        const momentDate = moment(tweet.CreatedDate, 'YYYY-MM-DD');
        const displayDate = momentDate.format('dddd[,] D MMMM YYYY');
        const tweetText = tweet.Text.replace('&amp;', '&');
        return (
            <a href={tweet.Url} target="_blank">
                <div className="blog-list-tweet">
                    <time dateTime={tweet.CreatedDate}
                        className="tweet-date">{displayDate}</time>
                    <p className="h-tertiary">{tweetText}</p>
                </div>
            </a>
        );
    }
    renderBlogTags(categories) {
        return (
            <ul className="blog-tags">
                {categories.map(this.renderBlogTag, this)}
            </ul>
        );
    }
    renderBlogTag(category, index) {
        const safeUrlCategory = StringFunctions.safeUrl(category);
        const linkUrl = `?category=${safeUrlCategory}`;
        return (
            <li key={index} className="blog-tag">
                <a href={linkUrl}>
                    {category}
                </a>
            </li>
        );
    }
    renderPagingButton() {
        const buttonProps = {
            className: 'btn btn-default',
            onClick: this.loadNextPage,
        };
        return (
            <div className="center-block blog-list-show-more">
                <button {...buttonProps}>{this.props.tilesButtonText}</button>
            </div>
        );
    }
    render() {
        const totalPages = Math.ceil(this.props.blogItems.length / this.state.columns);
        return (
            <div className="blog-list-content">
                {this.props.featuredBlogItems.length > 0
                    && this.renderFeaturedBlogs()}
                {this.props.blogItems.length > 0
                    && this.renderBlogTiles()}
                {this.state.displayedPages < totalPages
                    && this.renderPagingButton()}
            </div>
        );
    }
}

BlogList.propTypes = {
    featuredBlogItems: React.PropTypes.array,
    blogItems: React.PropTypes.array.isRequired,
    tweets: React.PropTypes.array.isRequired,
    displayTweets: React.PropTypes.bool.isRequired,
    tilesHeader: React.PropTypes.string.isRequired,
    tilesButtonText: React.PropTypes.string.isRequired,
};
