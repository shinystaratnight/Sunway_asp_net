import * as UserActions from '../../actions/bookingjourney/userActions';

import AdminLoginContent from '../../widgets/common/adminlogin';
import React from 'react';
import ValidateFunctions from '../../library/validateFunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class AdminLoginContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            userLogin: {
                errors: {},
                email: '',
                password: '',
            },
            focusObject: '',
        };
        this.handleLoginChange = this.handleLoginChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }
    componentDidMount() {
        this.setState({ focusObject: 'email' });
    }
    componentWillReceiveProps(nextProps) {
        if (nextProps.session.UserSession.AdminSession
            && nextProps.session.UserSession.AdminSession.LoggedIn) {
            window.location = '/';
        }

        const emailInvalid = nextProps.session.Warnings
            && nextProps.session.Warnings.indexOf('Invalid Email.') > -1;
        const passwordInvalid = nextProps.session.Warnings
            && nextProps.session.Warnings.indexOf('Invalid Password.') > -1;

        if (emailInvalid) {
            this.state.userLogin.errors.email = 'Please enter a valid email address';
            this.state.focusObject = 'email';
        } else if (passwordInvalid) {
            this.state.userLogin.errors.password = 'Please re-enter your password';
            this.state.focusObject = 'password';
        }
    }
    componentDidUpdate() {
        if (this.state.focusObject !== '') {
            this.state.focusObject = '';
        }
    }
    handleLoginChange(event) {
        const field = event.target.name;
        const value = event.target.value;

        const userLogin = this.state.userLogin;
        userLogin[field] = value;

        this.setState({ userLogin });
    }
    handleLogin() {
        let validLogin = true;
        const userLogin = this.state.userLogin;
        userLogin.errors = {};
        let focusObject = '';
        if (!ValidateFunctions.isEmail(userLogin.email)) {
            validLogin = false;
            userLogin.errors.email = 'A valid email is required';
            focusObject = 'email';
        }
        if (userLogin.password === '') {
            validLogin = false;
            userLogin.errors.password = 'A password is required';
            focusObject = focusObject ? focusObject : 'password';
        }
        this.setState({ userLogin, focusObject });

        if (validLogin) {
            this.props.actions.adminLogin(userLogin.email, userLogin.password);
        }
    }
    contentProps() {
        const props = {
            userLogin: this.state.userLogin,
            onChange: this.handleLoginChange,
            onLogin: this.handleLogin,
            onEnter: this.handleLogin,
            warnings: this.props.session.Warnings ? this.props.session.Warnings : [],
            focusObject: this.state.focusObject,
        };
        return props;
    }
    render() {
        return (
            <div className="admin-login-container container">
                <AdminLoginContent {...this.contentProps()} />
            </div>
        );
    }
}

AdminLoginContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
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
        UserActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(AdminLoginContainer);
