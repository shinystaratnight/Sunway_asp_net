import * as CmsWebsiteActions from '../../actions/bookingjourney/cmsWebsiteActions';
import * as EntityActions from '../../actions/content/entityActions';
import * as RedirectActions from '../../actions/content/redirectActions';
import * as UserActions from '../../actions/bookingjourney/userActions';

import React from 'react';
import SiteBuilderToolbar from '../../widgets/common/sitebuildertoolbar';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SiteBuilderToolbarContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
        this.handlePageClick = this.handlePageClick.bind(this);
        this.handleEntitiesClick = this.handleEntitiesClick.bind(this);
        this.handleRedirectClick = this.handleRedirectClick.bind(this);
        this.handleThemeClick = this.handleThemeClick.bind(this);
        this.handleSettingsClick = this.handleSettingsClick.bind(this);
        this.handleLogOut = this.handleLogOut.bind(this);
        this.handleSiteChange = this.handleSiteChange.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadEntityList(this.props.site.Name, 'custom');
        this.props.actions.loadWebsites();
    }
    componentWillReceiveProps(nextProps) {
        if (this.props.session.isLoaded
            && this.props.session.UserSession.AdminSession.LoggedIn
            && !nextProps.session.UserSession.AdminSession.LoggedIn) {
            location.reload();
        }
    }
    handlePageClick() {
        let pageUrl = window.location.pathname;
        pageUrl = pageUrl === '/' ? 'homepage'
            : pageUrl.substr(1).replace(new RegExp('\/', 'g'), '-');
        this.props.actions.editEntity(this.props.site.Name, 'Page', pageUrl);
    }
    handleEntitiesClick() {
        this.props.actions.editEntityList();
    }
    handleRedirectClick() {
        this.props.actions.displayRedirects();
    }
    handleThemeClick() {
        this.props.actions.editEntity(this.props.site.Name, 'Theme', 'default');
    }
    handleSettingsClick() {
        this.props.actions.editEntity(this.props.site.Name, 'SiteSettings', 'default');
    }
    handleLogOut() {
        this.props.actions.userLogOutAdmin();
    }
    handleSiteChange(event) {
        this.props.actions.userChangeSite(event.target.value);
    }
    toolbarProps() {
        const navItems = [
            {
                name: 'Page',
                iconClass: 'fa-file-text-o',
                onClick: this.handlePageClick,
            },
            {
                name: 'Entities',
                iconClass: 'fa-list-alt',
                onClick: this.handleEntitiesClick,
            },
            {
                name: 'Redirects',
                iconClass: 'fa-arrows-alt',
                onClick: this.handleRedirectClick,
            },
            // {
            //    name: 'Theme',
            //    iconClass: 'fa-paint-brush',
            //    onClick: this.handleThemeClick,
            // },
            // {
            //    name: 'Settings',
            //    iconClass: 'fa-cog',
            //    onClick: this.handleSettingsClick,
            // },
        ];
        const sites = [];
        if (this.props.websites.isLoaded) {
            this.props.websites.websites.forEach(website => {
                sites.push(website.Name);
            });
        }
        const currentSite = this.props.session.UserSession.SelectedCmsWebsite.Name;
        const toolbarProps = {
            navItems,
            sites,
            currentSite,
            onSiteChange: this.handleSiteChange,
            onLogOut: this.handleLogOut,
            user: this.props.session.UserSession,
        };
        return toolbarProps;
    }
    render() {
        return (
            <div className="site-builder-toolbar-container">
                <SiteBuilderToolbar {...this.toolbarProps()} />
            </div>
        );
    }
}

SiteBuilderToolbarContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    entityList: React.PropTypes.object.isRequired,
    websites: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
* map state to props
* @param {object} state - The current state.
* @return {object} the props mapped to state.
*/
function mapStateToProps(state) {
    const entities = state.entities ? state.entities : {};
    const entityList = state.entityList ? state.entityList : {};
    const websites = state.cmsWebsites ? state.cmsWebsites : {};
    const site = state.site ? state.site : {};

    return {
        entities,
        entityList,
        websites,
        site,
    };
}

/**
* map dispatch to props
* @param {object} dispatch - redux dispatch
* @return {object} entity actions
*/
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions,
        CmsWebsiteActions,
        UserActions,
        RedirectActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SiteBuilderToolbarContainer);
