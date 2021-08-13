import React from 'react';

export default class Paging extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    renderPageLink(page) {
        const pageLinkAttributes = {
            key: `page-link-${page}`,
            className: 'paging-page-link',
            onClick: () => this.props.onPageClick(page),
        };

        if (page === this.props.currentPage) {
            pageLinkAttributes.className = `${pageLinkAttributes.className} active`;
        }

        return (
            <li {...pageLinkAttributes}>{page}</li>
        );
    }
    render() {
        const { totalPages, currentPage, pageLinks } = this.props;

        const displayLinks = [];

        let firstLink = 1;
        let lastLink = totalPages < pageLinks ? totalPages : pageLinks;

        const linksEitherSide = (pageLinks - pageLinks % 2) / 2;

        if (currentPage - linksEitherSide > 1) {
            firstLink = currentPage - linksEitherSide;
            lastLink = currentPage + linksEitherSide;
        }

        if (lastLink > totalPages) {
            lastLink = totalPages;
            firstLink = lastLink - (pageLinks - 1);
            firstLink = firstLink < 1 ? 1 : firstLink;
        }

        for (let i = firstLink; i <= lastLink; i++) {
            displayLinks.push(i);
        }

        return (
            <div className="paging">
                {this.props.currentPage !== 1
                    && <span className="arrow-link arrow-link-reverse paging-link-previous"
                            onClick={() => this.props.onPageClick(currentPage - 1)}>Previous</span>}

                <ul>
                    {displayLinks.map(this.renderPageLink, this)}
                </ul>

                {this.props.currentPage !== this.props.totalPages
                    && <span className="arrow-link paging-link-next"
                              onClick={() => this.props.onPageClick(currentPage + 1)}>Next</span>}
            </div>
        );
    }
}

Paging.propTypes = {
    totalPages: React.PropTypes.number.isRequired,
    currentPage: React.PropTypes.number.isRequired,
    pageLinks: React.PropTypes.number.isRequired,
    onPageClick: React.PropTypes.func.isRequired,
};
