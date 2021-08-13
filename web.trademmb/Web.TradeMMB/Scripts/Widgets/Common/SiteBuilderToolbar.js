import React from 'react';

class SiteBuilderToolbar extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderNav() {
        return (
            <nav className="toolbar-nav">
                <ul className="nav">
                    {this.props.navItems.map(this.renderNavItem, this)}
                </ul>
            </nav>
        );
    }
    renderNavItem(navItem) {
        const iconClass = `fa ${navItem.iconClass}`;
        return (
            <li key={navItem.name}
                className="nav-item"
                onClick={navItem.onClick}>
                <span className={iconClass}></span>
                <span>{navItem.name}</span>
            </li>
        );
    }
    renderViewSelect() {
        return (
            <div className="toolbar-view">
                <nav>
                    <ul className="nav">
                        <li className="nav-label">
                            <span className="fa fa-eye"></span>
                            <span>View</span>
                        </li>
                        <li className="nav-item">
                            <div className="status-container live">
                                <span className="status"></span>
                            </div>
                            <span>Live</span>
                        </li>
                        <li className="nav-item">
                            <div className="status-container draft active">
                                <span className="status"></span>
                            </div>
                            <span>Draft</span>
                        </li>
                    </ul>
                </nav>
            </div>
        );
    }
    renderUser() {
        return (
            <div className="toolbar-user">
                <span className="user-icon"></span>
                <span className="user-name">{this.props.user.FirstName}</span>
                <span className="user-logout" onClick={this.props.onLogOut}>Log Out</span>
            </div>
        );
    }
    render() {
        const renderViewSelect = false;
        return (
            <div className="site-builder-toolbar">
                <div className="toolbar-logo">
                    <p>Site Builder Toolbar</p>
                </div>
                {this.renderNav()}
                {renderViewSelect
                    && this.renderViewSelect()}
                {this.renderUser()}
            </div>
        );
    }
}

SiteBuilderToolbar.propTypes = {
    navItems: React.PropTypes.array.isRequired,
    user: React.PropTypes.object.isRequired,
    onLogOut: React.PropTypes.func.isRequired,
};

export default SiteBuilderToolbar;
