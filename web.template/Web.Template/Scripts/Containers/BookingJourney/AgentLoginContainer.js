import * as AlertActions from 'actions/bookingjourney/alertActions';
import * as AlertConstants from 'constants/alerts';
import * as UserActions from 'actions/bookingjourney/userActions';

import AgentLogin from 'widgets/bookingjourney/agentlogin';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class AgentLoginContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            userLogin: {
                errors: {},
                username: '',
                password: '',
                saveLoginDetails: false,
            },
        };
        this.handleLoginChange = this.handleLoginChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }
    componentDidMount() {
        const shouldLogout = UrlFunctions.getQueryStringValue('logout');
        if (shouldLogout === 'true') {
            this.handleLogOut();
        } else if (this.props.session.UserSession.LoggedIn
            && !this.props.session.UserSession.AdminMode) {
            window.location = '/';
        }

        this.loginWarningCheck();
    }
    componentWillReceiveProps(nextProps) {
        if (nextProps.session.UserSession.LoggedIn && !nextProps.session.UserSession.AdminMode) {
            window.location = '/';
        }
    }
    loginWarningCheck() {
        const warning = UrlFunctions.getQueryStringValue('warning');
        if (warning && warning === 'login') {
            this.props.actions.addAlert('Login_Required',
                AlertConstants.ALERT_TYPES.WARNING,
                'You must be logged in to access this website.');
        }
    }
    handleLoginChange(event) {
        const field = event.target.name;
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;

        const userLogin = this.state.userLogin;
        userLogin[field] = value;

        this.setState({ userLogin });
    }
    handleLogin() {
        const contentModel = this.props.entity.model;
        let validLogin = true;
        const userLogin = this.state.userLogin;
        userLogin.errors = {};
        if (this.state.userLogin.username === '') {
            validLogin = false;
            userLogin.errors.username = contentModel.InvalidUsernameWarning;
        }
        if (this.state.userLogin.password === '') {
            validLogin = false;
            userLogin.errors.password = contentModel.InvalidPasswordWarning;
        }
        this.setState({ userLogin });
        console.log('VALIDLOGIN:', validLogin);
        if (validLogin) {
            this.props.actions.userLoginUsername(this.state.userLogin.username,
                this.state.userLogin.password, this.state.userLogin.saveLoginDetails);
        }
    }
    handleLogOut() {
        this.props.actions.userLogOut();
    }
    render() {
        const contentModel = this.props.entity.model;
        const agentLoginProps = {
            title: contentModel.Title,
            saveLoginText: contentModel.SaveLoginText,
            loginButtonText: contentModel.LoginButtonText,
            onLogin: this.handleLogin,
            onChange: this.handleLoginChange,
            onEnter: this.handleLogin,
            userLogin: this.state.userLogin,
            warning: this.props.session.Warnings && this.props.session.Warnings.length > 0
                ? contentModel.FailedLoginWarning : '',
        };
        return (
            <AgentLogin {...agentLoginProps} />
        );
    }
}

AgentLoginContainer.propTypes = {
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
    const actions = Object.assign({},
        AlertActions,
        UserActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(AgentLoginContainer);
