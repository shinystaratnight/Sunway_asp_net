import BlogItemsCollapsed from '../../widgets/content/blogitemscollapsed';
import DateFunctions from '../../library/datefunctions';
import React from 'react';
import StringFunctions from '../../library/stringfunctions';

export default class BlogItemsCollapsedContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getEntities() {
        const blogs = [];
        this.props.entitiesCollection.BlogItem.forEach(blogItem => {
            const model = JSON.parse(blogItem.ContentValue);
            model.context = blogItem.Name.toString();
            blogs.push(
                {
                    name: blogItem.Name.toString(),
                    model,
                }
            );
        });
        let parentBlog = {};
        const entityInfo
            = this.props.page.EntityInformations.filter(ei => ei.Name === 'blogitem')[0];
        for (let i = 0; i < blogs.length; i++) {
            const blog = blogs[i];
            if (StringFunctions.safeUrl(blog.name.toString()) === StringFunctions
                .safeUrl(entityInfo.Value.toString())) {
                parentBlog = blog;
                break;
            }
        }
        const returnObject = {
            blogs,
            parentBlog,
        };
        return returnObject;
    }
    matchingCategory(blogA, blogB) {
        if (blogA.hasOwnProperty('model') && blogA.model.hasOwnProperty('Categories')
            && blogB.hasOwnProperty('model') && blogB.model.hasOwnProperty('Categories')) {
            for (let i = 0; i < blogA.model.Categories.length; i++) {
                for (let j = 0; j < blogB.model.Categories.length; j++) {
                    if (blogA.model.Categories[i] === blogB.model.Categories[j]) {
                        return true;
                    }
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
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const entities = this.getEntities();
        const blogs = entities.blogs;
        const parentBlog = entities.parentBlog;
        const filteredBlogs = blogs.filter(blog =>
            (StringFunctions.safeUrl(blog.name) !== StringFunctions.safeUrl(parentBlog.name))
            && DateFunctions.hasValidPublishDates(blog.model));
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
        return (
            <BlogItemsCollapsed {...collapsedBlogsProps} />
        );
    }
}

BlogItemsCollapsedContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
};
