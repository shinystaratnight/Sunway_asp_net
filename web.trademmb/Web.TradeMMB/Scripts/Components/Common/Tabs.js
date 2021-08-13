import React from 'react';

export default class Tabs extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
        this.handleClick = this.handleClick.bind(this);
    }
    handleClick(tabItem) {
        if (tabItem.onClick) {
            tabItem.onClick();
        }
    }
    renderTabItem(tabItem) {
        let tabItemClass = 'tab-item';
        if (this.props.selectedTab === tabItem.value
                || this.props.selectedTab === tabItem.name) {
            tabItemClass += ' tab-selected';
        }
        const boundItemClick = this.handleClick.bind(this, tabItem);
        return (
            <li key={tabItem.name}
                role="presentation"
                className={tabItemClass}
                onClick={boundItemClick}>
                {tabItem.iconClass && tabItem.iconClass !== ''
                    && <span className={tabItem.iconClass}></span>}
                <span className="tab-item-name">{tabItem.name}</span>
            </li>
        );
    }
    render() {
        let tabClasses = 'tabs';
        if (this.props.tabContainerStyle) {
            tabClasses += ` container-${this.props.tabContainerStyle.toLowerCase()}`;
        }
        if (this.props.selectedTabStyle) {
            tabClasses += ` selected-${this.props.selectedTabStyle.toLowerCase()}`;
        }
        if (this.props.tabAlignment === 'center') {
            tabClasses += ' align-center';
        }
        return (
            <ul className={tabClasses}>
                {this.props.tabs.map(this.renderTabItem, this)}
            </ul>
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
    })),
};
