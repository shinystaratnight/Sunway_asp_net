import React from 'react';

export default class Tabs extends React.Component {
    constructor(props) {
        super(props);
        this.screenSmMin = 768;
        this.state = {
            mobileDisplay: false,
            mobileMenuDisplay: false,
        };
        this.handleClick = this.handleClick.bind(this);
        this.handleResize = this.handleResize.bind(this);
        this.setMobileMenuDisplay = this.setMobileMenuDisplay.bind(this);
    }
    componentDidMount() {
        if (this.props.enableMobileTabs) {
            const mobileDisplay = window.innerWidth < this.screenSmMin;
            this.state.mobileDisplay = mobileDisplay;
            this.setState({ mobileDisplay });
            window.addEventListener('resize', this.handleResize);
        }
    }
    handleClick(tabItem) {
        if (tabItem.onClick) {
            tabItem.onClick();
        }

        if (this.state.mobileMenuDisplay) {
            this.setMobileMenuDisplay(false);
        }
    }
    handleResize() {
        const mobileDisplay = window.innerWidth < this.screenSmMin;
        if (this.state.mobileDisplay !== mobileDisplay) {
            this.setState({ mobileDisplay, mobileMenuDisplay: false });
        }
    }
    renderTabItem(tabItem) {
        let tabItemClass = 'tab-item';
        if (this.props.selectedTab === tabItem.value
                || this.props.selectedTab === tabItem.name) {
            tabItemClass += ' tab-selected';
        }
        const iconLocation = tabItem.iconLocation && tabItem.iconLocation !== ''
            ? tabItem.iconLocation : 'Separate';
        const boundItemClick = this.handleClick.bind(this, tabItem);
        let tabClass = 'tab-item-name';
        if (iconLocation === 'Tab' || iconLocation === 'Both') {
            tabClass = `${tabClass} ${tabItem.iconClass}`;
        }
        return (
            <li key={tabItem.name}
                role="presentation"
                className={tabItemClass}
                onClick={boundItemClick}>
                {tabItem.iconClass && tabItem.iconClass !== ''
                    && (iconLocation === 'Separate' || iconLocation === 'Both')
                    && <span className={tabItem.iconClass}></span>}
                <span className={tabClass}>{tabItem.name}</span>
            </li>
        );
    }
    getTabClass() {
        let tabClass = 'tabs';
        if (this.props.tabContainerStyle) {
            tabClass += ` container-${this.props.tabContainerStyle.toLowerCase()}`;
        }
        if (this.state.mobileDisplay) {
            tabClass += ' tabs-mobile';
        }
        if (this.props.selectedTabStyle) {
            tabClass += ` selected-${this.props.selectedTabStyle.toLowerCase()}`;
        }
        if (this.props.tabAlignment === 'center') {
            tabClass += ' align-center';
        }
        return tabClass;
    }
    setMobileMenuDisplay(value) {
        this.setState({ mobileMenuDisplay: value });
    }
    renderMobileSelectedTab() {
        const selectedTab = this.props.tabs.find(tab => tab.value === this.props.selectedTab);
        return (
             <li className="tab-selected">{selectedTab.name}</li>
        );
    }
    renderMobileMenuLink() {
        const props = {
            role: 'presentation',
            className: this.state.mobileMenuDisplay ? 'tab-menu displayed' : 'tab-menu',
            onClick: () => {
                this.setMobileMenuDisplay(!this.state.mobileMenuDisplay);
            },
        };
        return (
            <li {...props}>
                <span className="tab-menu-link"></span>
            </li>
        );
    }
    renderMobileMenu() {
        const tabOptions = this.props.tabs.filter(tab => tab.value !== this.props.selectedTab);
        return (
            <ul className="tabs-mobile-menu">
                {tabOptions.map(this.renderMobileMenuItem, this)}
            </ul>
        );
    }
    renderMobileMenuItem(tabItem) {
        const boundItemClick = this.handleClick.bind(this, tabItem);
        return (
            <li key={tabItem.name}
                role="presentation"
                className="tab-item"
                onClick={boundItemClick}>
                {tabItem.iconClass && tabItem.iconClass !== ''
                    && <span className={tabItem.iconClass}></span>}
                <span className="tab-item-name">{tabItem.name}</span>
            </li>
        );
    }
    render() {
        return (
            <nav>
                <ul className={this.getTabClass()}>
                    {!this.state.mobileDisplay
                        && this.props.tabs.map(this.renderTabItem, this)}

                    {this.state.mobileDisplay
                        && this.renderMobileSelectedTab()}

                    {this.state.mobileDisplay
                        && this.renderMobileMenuLink()}
                </ul>
                {this.state.mobileMenuDisplay
                    && this.renderMobileMenu()}
            </nav>

        );
    }
}

Tabs.propTypes = {
    selectedTab: React.PropTypes.string.isRequired,
    tabAlignment: React.PropTypes.string,
    tabContainerStyle: React.PropTypes.string,
    selectedTabStyle: React.PropTypes.string,
    tabs: React.PropTypes.arrayOf(React.PropTypes.shape({
        name: React.PropTypes.string.isRequired,
        value: React.PropTypes.string,
        iconClass: React.PropTypes.string,
        onClick: React.PropTypes.func,
        iconLocation: React.PropTypes.oneOf(['Separate', 'Tab', 'Both']),
    })),
    enableMobileTabs: React.PropTypes.bool,
};

Tabs.defaultProps = {
    enableMobileTabs: false,
};
