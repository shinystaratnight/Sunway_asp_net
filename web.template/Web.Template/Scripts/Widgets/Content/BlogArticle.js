import '../../../styles/widgets/content/_blogarticle.scss';

import MarkdownText from '../../components/common/MarkdownText';
import React from 'react';

export default class BlogArticle extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderArticleDetails() {
        const article = this.props;
        const tileTextKey = `TileText${this.props.siteName}`;
        const tileText = this.props[tileTextKey];
        return (
            <div className="article-details">
                <div className="col-md-5 col-lg-4 hidden-sm-down article-image">
                    {article.TileImage
                        && <img src={article.TileImage}/>}
                </div>
                <div className="col-xs-12 col-md-7 col-lg-8 copy">
                    <MarkdownText markdown={tileText} />
                </div>
            </div>
        );
    }
    renderAuthorDetails() {
        const author = this.props.Author;
        const authorName = author.FirstName || author.LastName
            ? `${author.FirstName} ${author.LastName}`
            : '';
        return (
            <section className="article-author">
                <div className="row">
                    <div className="tile-container-1 padding dividing-border col-xs-12">
                        <div className="col-sm-3 hidden-xs author-image">
                            <img className="img-circle" src={author.Image}/>
                        </div>
                        <div className="col-xs-12 col-sm-9 copy">
                            <div>
                                <h3 className="h-tertiary tile-header">Written by</h3>
                                <h3 className="h-secondary author">
                                    {authorName}
                                </h3>
                            </div>
                            <p className="main-content">
                                {author.Summary}
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        );
    }
    shouldRenderArticleDetails() {
        const renderArticleDetails
            = this.props.TileImage
            && (this.props.TileTextDefault || this.props.TileTextIrish);
        return renderArticleDetails;
    }
    shouldRenderAuthorDetails() {
        const author = this.props.Author;
        const renderAuthorDetails
            = author
            && author.FirstName
            && author.LastName
            && author.Image
            && author.Summary;
        return renderAuthorDetails;
    }
    renderArticleFooter() {
        return (
            <footer>
                {this.shouldRenderArticleDetails()
                    && this.renderArticleDetails()}
                {this.shouldRenderAuthorDetails()
                        && this.renderAuthorDetails()}
            </footer>
        );
    }
    renderArticleBody() {
        const renderArticleFooter
            = this.shouldRenderArticleDetails() || this.shouldRenderAuthorDetails();
        const article = this.props.MarkdownText;
        return (
            <article className="article col-xs-12 col-md-8">
                <div className ="article-body">
                    <MarkdownText markdown={article} />
                    {renderArticleFooter
                        && this.renderArticleFooter()}
                </div>
            </article>
        );
    }
    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2"> </div>
                        {this.props
                            && this.renderArticleBody()}
                    <div className="col-md-2"> </div>
                </div>
            </div>
        );
    }
}

BlogArticle.propTypes = {
    Author: React.PropTypes.shape({
        FirstName: React.PropTypes.string,
        LastName: React.PropTypes.string,
        Image: React.PropTypes.string,
        JobTitle: React.PropTypes.string,
        Summary: React.PropTypes.string,
    }),
    Date: React.PropTypes.string,
    Title: React.PropTypes.string,
    LeadParagraph: React.PropTypes.string,
    MarkdownText: React.PropTypes.string,
    TileImage: React.PropTypes.string,
    TileTextDefault: React.PropTypes.string,
    TileTextIrish: React.PropTypes.string,
    siteName: React.PropTypes.string,
};
