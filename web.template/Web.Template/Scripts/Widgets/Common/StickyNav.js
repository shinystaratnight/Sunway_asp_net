import React from 'react';

export default class StickyNav extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            contextActiveStickyNavItem: '',
            sectionsOnPage: [],
        };
        this.handleScroll = this.handleScroll.bind(this);
        this.handleDOMLoad = this.handleDOMLoad.bind(this);
        this.updateSectionsOnPage = this.updateSectionsOnPage.bind(this);
        this.getSectionsOnPage = this.getSectionsOnPage.bind(this);
        this.getSectionInfo = this.getSectionInfo.bind(this);
        this.renderNavBookButton = this.renderNavBookButton.bind(this);
        this.renderNavListItem = this.renderNavListItem.bind(this);
        this.render = this.render.bind(this);
    }
    componentDidMount() {
        window.addEventListener('scroll', this.handleScroll);
    }
    componentWillUnmount() {
        window.removeEventListener('scroll', this.handleScroll);
    }
    componentWillReceiveProps(nextProps) {
        if (nextProps.newWidgets !== this.props.newWidgets) {
            this.updateSectionsOnPage();
        }
    }
    handleDOMLoad() {
        this.updateSectionsOnPage();
        this.updateContextActiveNavItem();
    }
    handleScroll() {
        this.updateSectionsOnPage();
        this.updateContextActiveNavItem();
    }
    updateSectionsOnPage() {
        const tempSections = this.getSectionsOnPage();
        if (tempSections !== this.state.sectionsOnPage) {
            this.setState({
                sectionsOnPage: tempSections,
            });
        }
    }
    updateContextActiveNavItem() {
        let activeContext = '';
        const morePixelsThanPageLength = 1000000;
        const scrollY = window.scrollY || window.pageYOffset;
        const navPosition
            = document.getElementById(this.props.containerId).offsetHeight + scrollY;
        for (let i = 0; i < this.state.sectionsOnPage.length; i++) {
            const currentElement = this.state.sectionsOnPage[i];
            const currentElementY = currentElement.position;
            const nextElement = this.state.sectionsOnPage[i + 1];
            const nextElementY = this.state.sectionsOnPage.length > i + 1
                ? nextElement.position : morePixelsThanPageLength;
            if (currentElementY < navPosition && nextElementY > navPosition) {
                activeContext = currentElement.context;
                break;
            }
        }
        this.setState({
            contextActiveStickyNavItem: activeContext,
        });
    }
    getSectionsOnPage() {
        const elements = document.getElementsByClassName('section-widget');
        const sectionsOnPage = [];
        for (let i = 0; i < elements.length; i++) {
            const sectionOnPage = this.getSectionInfo(elements[i], i);
            if (sectionOnPage.hasContent) {
                sectionsOnPage.push(sectionOnPage);
            }
        }
        return (
            sectionsOnPage
        );
    }
    getSectionInfo(section, index) {
        const sectionIndex = index;
        const scrollY = window.scrollY || window.pageYOffset;
        const sectionPosition = section.getBoundingClientRect().top + scrollY;
        const sectionContext = section.dataset.context;
        const sectionId = section.getAttribute('id');
        const minimumLength = 150;
        const hasContent = section.innerHTML.length > minimumLength;
        const widgetSection = {
            index: sectionIndex,
            position: sectionPosition,
            context: sectionContext,
            id: sectionId,
            hasContent,
        };
        return widgetSection;
    }

    scrollTo(scrollToPosition) {
        const scrollY = window.scrollY || window.pageYOffset;
        let scrollTotal = scrollToPosition - scrollY;
        const timeToScroll = 150;
        const scrollStep = Math.round(scrollTotal / timeToScroll);
        let finalStep = false;
        const timePerStep = 1;
        const scrollInterval = setInterval(() => {
            if (finalStep === false) {
                window.scrollBy(0, scrollStep);
            } else {
                clearInterval(scrollInterval);
            }
            if (Math.abs(scrollTotal) > Math.abs(scrollStep)) {
                scrollTotal = scrollTotal - scrollStep;
            } else {
                window.scrollTo(0, scrollToPosition + 1);
                finalStep = true;
            }
        }, timePerStep);
    }
    renderNavBookButton() {
        let link;
        if (this.props.navBookButton.URL !== '') {
            link = this.props.navBookButton.URL;
            if (this.props.navBookButton.ConfigureDestination
                && this.props.entityInformations
                && this.props.entityInformations.some(i => i.Name === 'Resort')) {
                const resortId = this.props.entityInformations
                                            .filter(i => i.Name === 'Resort')[0].Id;
                link += `?resortid=${resortId}`;
            }
        }
        return (
            <li role="button" className="sticky-nav-item">
                <a className="btn btn-primary" href={link}>{this.props.navBookButton.ButtonText}</a>
            </li>
        );
    }
    renderNavListItem(navItem, index) {
        const key = `stickynavitem-${index}`;
        const navItemClass = 'sticky-nav-item';
        let navItemTextClass = '';
        if (navItem.URL) {
            return (
                <li key={key} role="presentation" className={navItemClass}>
                    <a href={navItem.URL} className={navItemTextClass}>
                    {navItem.Title}
                    </a>
                </li>
            );
        }
        const activeContext = this.state.contextActiveStickyNavItem;
        const contextMatches = (activeContext === navItem.AssociatedContext);
        let yPositionOfNavItem = 0;
        let navItemId = '';
        const stickyNavPositions
            = document.getElementById(this.props.containerId).getBoundingClientRect();
        const stickyNavHeight = stickyNavPositions.bottom - stickyNavPositions.top;
        const sectionsOnPage = this.state.sectionsOnPage;
        for (let i = 0; i < sectionsOnPage.length; i++) {
            const sectionOnPage = sectionsOnPage[i];
            if (sectionOnPage.context.indexOf(navItem.AssociatedContext) > -1) {
                yPositionOfNavItem = sectionOnPage.position;
                yPositionOfNavItem = yPositionOfNavItem - stickyNavHeight + 1;
                navItemId = sectionOnPage.id;
                navItemId = `#${navItemId}`;
                break;
            }
        }
        if (contextMatches) {
            navItemTextClass = `${navItemTextClass} active`;
        }

        return (
            <li key={key} role="presentation" className={navItemClass}>
                <a href={navItemId} className={navItemTextClass}
                onClick={(event) => {
                    event.preventDefault(); this.scrollTo(parseInt(yPositionOfNavItem, 10));
                }}>
                    {navItem.Title}
                </a>
            </li>
        );
    }
    sortNavItems(navItemA, navItemB) {
        return (
            (navItemA.Sequence - navItemB.Sequence)
        );
    }
    render() {
        const navItems = this.props.navItems;
        const navClass = 'nav-sticky hidden-sm-down';
        const sectionsOnPage = this.state.sectionsOnPage;
        const filteredNavItems = navItems.filter(ni => {
            let valid = false;
            if (ni.URL) {
                valid = true;
            } else {
                for (let i = 0; i < sectionsOnPage.length; i++) {
                    if (sectionsOnPage[i].context.indexOf(ni.AssociatedContext) > -1) {
                        valid = true;
                        break;
                    }
                }
            }
            return valid;
        });
        const sortedNavItems = filteredNavItems.sort(this.sortNavItems);
        const renderBookButton = this.props.navBookButton.ButtonText !== ''
                            && (!this.props.navBookButton.LoggedIn || this.props.userLoggedIn);
        return (
            <nav className={navClass}>
                <div className="container">
                    <ul className="nav navbar">
                        {navItems.length > 0
                        && sortedNavItems.map(this.renderNavListItem, this)}
                        {renderBookButton
                            && this.renderNavBookButton()}
                    </ul>
                </div>
            </nav>
        );
    }
}

StickyNav.propTypes = {
    navItems: React.PropTypes.arrayOf(
        React.PropTypes.shape({
            Title: React.PropTypes.string.isRequired,
            AssociatedContext: React.PropTypes.string,
            URL: React.PropTypes.string,
            Sequence: React.PropTypes.number.isRequired,
        })
    ),
    navBookButton: React.PropTypes.shape({
        ButtonText: React.PropTypes.string,
        URL: React.PropTypes.string,
        LoggedIn: React.PropTypes.bool,
        ConfigureDestination: React.PropTypes.bool,
    }),
    entityInformations: React.PropTypes.arrayOf(
            React.PropTypes.shape({
                Id: React.PropTypes.number,
                Name: React.PropTypes.string,
                Value: React.PropTypes.string,
            })),
    userLoggedIn: React.PropTypes.bool,
    containerId: React.PropTypes.string.isRequired,
    newWidgets: React.PropTypes.bool.isRequired,
};
