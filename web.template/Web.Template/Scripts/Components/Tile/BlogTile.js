import React from 'react';

export default class BlogTile extends React.Component {
    render() {
        let containerClass = 'flex-item col-xs-11 col-sm-6 col-md-4';
        if (!this.props.tileItem.DisplayOnMobile) {
            containerClass = `${containerClass} hidden-xs`;
        }
        return (
            <div className={containerClass}>
                <article className="article">
                    <img className="blog-image" src={this.props.tileItem.Image} />
                    <div className="flex-item-content">
                        <header>
                            <time dateTime="{this.props.tileItem.Date}"
                                  className="blog-date">{this.props.tileItem.Date}</time>
                            <a href={this.props.tileItem.URL}>
                                <h3 className="h-tertiary h-link">{this.props.tileItem.Title}</h3>
                            </a>
                            <span className="blog-author">By {this.props.tileItem.Author}</span>
                        </header>
                        <p className="blog-copy">{this.props.tileItem.Summary}</p>
                        <a href={this.props.tileItem.URL}
                            className="arrow-link">{this.props.tileItem.LinkText}</a>
                    </div>
                </article>
            </div>
        );
    }
}

BlogTile.propTypes = {
    tileItem: React.PropTypes.shape({
        Image: React.PropTypes.string.isRequired,
        Date: React.PropTypes.string.isRequired,
        Title: React.PropTypes.string,
        Author: React.PropTypes.string,
        Summary: React.PropTypes.string,
        LinkText: React.PropTypes.string,
        DisplayOnMobile: React.PropTypes.bool,
        URL: React.PropTypes.string,
    }).isRequired,
};
