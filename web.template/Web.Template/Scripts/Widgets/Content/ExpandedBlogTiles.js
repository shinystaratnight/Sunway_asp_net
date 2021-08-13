import '../../../styles/widgets/content/_blogList.scss';

import React from 'react';
import StringFunctions from '../../library/stringfunctions';
import moment from 'moment';

export default class ExpandedBlogTiles extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            columns: 3,
        };
        this.handleResize = this.handleResize.bind(this);
    }
    componentDidMount() {
        window.addEventListener('resize', this.handleResize);
        this.handleResize();
    }
    componentWillUnmount() {
        window.removeEventListener('resize', this.handleResize);
    }
    componentWillReceiveProps(nextProps) {
        const totalPages
            = Math.ceil(nextProps.blogItems.length / this.state.columns);
        this.setState({ totalPages });
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
            this.setState({ columns });
        }
    }
    renderBlogTiles() {
        const columns = this.state.columns;
        const pageSections = [];
        const totalBlogs = 6;
        const numberOfBlocks = totalBlogs / columns;
        for (let i = 0; i < numberOfBlocks; i++) {
            const pageSection = {
                key: `blog-block-${i}`,
                columns: [],
            };
            for (let j = 0; j < columns; j++) {
                const index = (i * columns) + j;
                const column = {
                    key: `blog-block-${i}-col-${j}`,
                    blogItem: this.props.blogItems[index],
                };
                pageSection.columns.push(column);
            }
            pageSections.push(pageSection);
        }
        return (
            <div className="blog-list-tiles">
                {this.props.title
                    && <h3 className="h-secondary center-block">{this.props.title}</h3>}
                {this.props.leadParagraph
                    && <p className="preamble center-block">{this.props.leadParagraph}</p>}
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
    renderColumn(column) {
        return (
            <div key={column.key} className="col-xs-12 col-sm-6 col-md-4">
                {column.blogItem
                    && this.renderBlogTile(column.blogItem)}
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
                    <h1 className="h-tertiary">{blogItem.Title}</h1>
                    <p className="blog-summary">{blogItem.LeadParagraph}</p>
                    <a className="arrow-link" href={blogUrl}>Read More</a>
                 </div>
            </div>
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
        const safeUrlCategory = category.replace(' ', '-');
        const linkUrl = `/blog?category=${safeUrlCategory}`;
        return (
            <li key={index} className="blog-tag">
                <a href={linkUrl}>
                    {category}
                </a>
            </li>
        );
    }
    renderButton() {
        const buttonProps = {
            className: 'btn btn-default',
        };
        return (
            <div className="center-block blog-list-show-more">
                <a {...buttonProps} href="/blog">{this.props.tilesButtonText}</a>
            </div>
        );
    }
    render() {
        return (
            <div className="blog-list-content">
                {this.props.blogItems.length > 0
                    && this.renderBlogTiles()}
                {this.props.tilesButtonText
                    && this.renderButton()}
            </div>
        );
    }
}

ExpandedBlogTiles.propTypes = {
    blogItems: React.PropTypes.array.isRequired,
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    tilesButtonText: React.PropTypes.string.isRequired,
};
