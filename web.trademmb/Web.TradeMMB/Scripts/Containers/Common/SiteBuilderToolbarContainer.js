import * as EntityActions from '../../actions/content/entityActions';
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
        this.handleThemeClick = this.handleThemeClick.bind(this);
        this.handleSettingsClick = this.handleSettingsClick.bind(this);
        this.handleLogOut = this.handleLogOut.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadEntityList(this.props.site.Name, 'custom');
    }
    componentWillReceiveProps(nextProps) {
        if (!nextProps.session.UserSession.LoggedIn) {
            location.reload();
        }
    }
    handlePageClick() {
        let pageUrl = window.location.pathname;
        pageUrl = pageUrl === '/' ? 'homepage' : pageUrl;
        this.props.actions.editEntity(this.props.site.Name, 'Page', pageUrl);
    }
    handleEntitiesClick() {
        this.props.actions.editEntityList();
    }
    handleThemeClick() {
        this.props.actions.editEntity(this.props.site.Name, 'Theme', 'default');
    }
    handleSettingsClick() {
        this.props.actions.editEntity(this.props.site.Name, 'SiteSettings', 'default');
    }
    handleLogOut() {
        this.props.actions.userLogOut();
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
        const toolbarProps = {
            navItems,
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
    const site = state.site ? state.site : {};

    return {
        entities,
        entityList,
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
        UserActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SiteBuilderToolbarContainer);
