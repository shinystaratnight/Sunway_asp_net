import React from 'react';

export default class Breadcrumb extends React.Component {
    constructor(props) {
        super(props);
        this.breadcrumbStates = {
            LINK: 'LINK',
            ACTIVE: 'ACTIVE',
            DISABLED: 'DISABLED',
            DISABLEDPAST: 'DISABLEDPAST',
        };
        this.state = {
            activeIndex: this.getActiveIndex(),
        };
    }
    componentWillReceiveProps(nextProps) {
        if (this.props.currentPageUrl !== nextProps.currentPageUrl) {
            const activeIndex = this.getActiveIndex();
            this.setState({ activeIndex });
        }
    }
    getActiveIndex() {
        const activeIndex = this.props.pages.findIndex(page =>
            page.pageUrl === this.props.currentPageUrl);
        return activeIndex;
    }
    getBreadcrumbState(pageIndex) {
        let breadcrumbState = '';

        if (pageIndex === this.state.activeIndex) {
            breadcrumbState = this.breadcrumbStates.ACTIVE;
        } else if (this.state.activeIndex === (this.props.pages.length - 1)) {
            breadcrumbState = this.breadcrumbStates.DISABLEDPAST;
        } else if (pageIndex < this.state.activeIndex) {
            breadcrumbState = this.breadcrumbStates.LINK;
        } else {
            breadcrumbState = this.breadcrumbStates.DISABLED;
        }

        return breadcrumbState;
    }
    renderBreadcrumb(page, pageIndex) {
        let breadcrumb = '';
        const breadcrumbState = this.getBreadcrumbState(pageIndex);
        switch (breadcrumbState) {
            case this.breadcrumbStates.LINK:
                breadcrumb = this.renderLinkBreadcrumb(page, pageIndex);
                break;
            case this.breadcrumbStates.ACTIVE:
                breadcrumb = this.renderActiveBreadcrumb(page, pageIndex);
                break;
            case this.breadcrumbStates.DISABLED:
                breadcrumb = this.renderDisabledBreadcrumb(page, pageIndex, '');
                break;
            case this.breadcrumbStates.DISABLEDPAST:
                breadcrumb = this.renderDisabledBreadcrumb(page, pageIndex, 'past');
                break;
            default:
        }
        return breadcrumb;
    }
    renderActiveBreadcrumb(page, pageIndex) {
        return (
            <li key={`breadcrumb-${pageIndex}`} className="breadcrumb-item active">
                <p><span className="breadcrumb-index">{pageIndex + 1}</span>{page.name}</p>
            </li>
        );
    }
    renderLinkBreadcrumb(page, pageIndex) {
        return (
            <li key={`breadcrumb-${pageIndex}`} className="breadcrumb-item">
                <a className="breadcrumb-link" href={page.url}>
                    <span className="breadcrumb-index">{pageIndex + 1}</span>{page.name}</a>
            </li>
        );
    }
    renderDisabledBreadcrumb(page, pageIndex, diabledclass) {
        const crumbClass = `breadcrumb-item disabled ${diabledclass}`;
        return (
            <li key={`breadcrumb-${pageIndex}`} className={crumbClass}>
                <p><span className="breadcrumb-index">{pageIndex + 1}</span>{page.name}</p>
            </li>
        );
    }
    render() {
        return (
            <div className="breadcrumb-content hidden-xs">
                <ol className="breadcrumb breadcrumb-indexed"
                    data-pages={this.props.pages.length}> hello from bread
                    {this.props.pages.map(this.renderBreadcrumb, this)}
                </ol>
            </div>
        );
    }
}

Breadcrumb.propTypes = {
    pages: React.PropTypes.arrayOf(React.PropTypes.shape({
        name: React.PropTypes.string,
        pageUrl: React.PropTypes.string,
        url: React.PropTypes.string,
    })),
    currentPageUrl: React.PropTypes.string,
};
