import * as UserActions from 'actions/bookingjourney/userActions';

import Header from '../../widgets/header';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';


export class HeaderContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleLogOut = this.handleLogOut.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!nextProps.session.UserSession.LoggedIn
                && this.props.session.UserSession.LoggedIn) {
            window.location = '/login';
        }
    }
    handleLogOut() {
        this.props.actions.userLogOut();
    }
    render() {
        const contentModel = this.props.entity.model;
        const headerProps = {
            user: this.props.session.UserSession,
            logo: contentModel.Logo,
            logoUrl: contentModel.LogoUrl,
            bannerImage: contentModel.BannerImage,
            offer: contentModel.Offer,
            contact: contentModel.Contact,
            navigationLinks: contentModel.NavigationLinks,
            socialMedia: contentModel.SocialMedia,
            handleLogOut: this.handleLogOut,
        };
        return (
            <Header {...headerProps} />
        );
    }
}

HeaderContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps() {
    return {};
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(UserActions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(HeaderContainer);
