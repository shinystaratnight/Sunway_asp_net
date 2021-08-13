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
        this.renderNavListItem = this.renderNavListItem.bind(this);
        this.render = this.render.bind(this);
    }
    componentDidMount() {
        window.addEventListener('scroll', this.handleScroll);
    }
    componentWillUnmount() {
        window.removeEventListener('scroll', this.handleScroll);
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
        const navPosition
            = document.getElementById(this.props.containerId).offsetHeight + window.scrollY;
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
            sectionsOnPage.push(sectionOnPage);
        }
        return (
            sectionsOnPage
        );
    }
    getSectionInfo(section, index) {
        const sectionIndex = index;
        const sectionPosition = section.getBoundingClientRect().top + window.scrollY;
        const sectionContext = section.dataset.context;
        const sectionId = section.getAttribute('id');
        const widgetSection = {
            index: sectionIndex,
            position: sectionPosition,
            context: sectionContext,
            id: sectionId,
        };
        return widgetSection;
    }

    scrollTo(scrollToPosition) {
        let scrollTotal = scrollToPosition - window.scrollY;
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

    renderNavListItem(navItem, index) {
        const key = `stickynavitem-${index}`;
        const navItemClass = 'sticky-nav-item';
        let navItemTextClass = '';
        const activeContext = this.state.contextActiveStickyNavItem;
        const contextMatches = (activeContext === navItem.AssociatedContext);
        let yPositionOfNavItem = 0;
        let navItemId = '';
        const stickyNavPositions
            = document.getElementById(this.props.containerId).getBoundingClientRect();
        const stickyNavHeight = stickyNavPositions.bottom - stickyNavPositions.top;
        for (let i = 0; i < this.state.sectionsOnPage.length; i++) {
            if (this.state.sectionsOnPage[i].context.indexOf(navItem.AssociatedContext) > -1) {
                yPositionOfNavItem = this.state.sectionsOnPage[i].position;
                yPositionOfNavItem = yPositionOfNavItem - stickyNavHeight + 1;
                navItemId = this.state.sectionsOnPage[i].id;
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
        const navClass = 'nav-sticky hidden-sm-down';
        const navItems = this.props.navItems;
        const sortedNavItems = navItems.sort(this.sortNavItems);
        return (
            <nav className={navClass}>
                <div className="container">
                    <ul className="nav navbar">
                        {navItems.length > 0
                        && sortedNavItems.map(this.renderNavListItem, this)}
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
            AssociatedContext: React.PropTypes.string.isRequired,
            Sequence: React.PropTypes.number.isRequired,
        })
    ),
    containerId: React.PropTypes.string.isRequired,
};
