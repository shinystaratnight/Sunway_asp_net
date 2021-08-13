import '../../../styles/widgets/common/_adminLogin.scss';

import React from 'react';
import TextInput from '../../components/form/textinput';

export default class AdminLogin extends React.Component {
    componentDidUpdate() {
        if (this.props.focusObject !== '') {
            document.getElementById(this.props.focusObject).focus();
        }
    }
    renderWarnings() {
        const emailWarning = 'The email address entered is not recognised';
        const passwordWarning = 'Incorrect Password. Please try again.';

        const emailInvalid = this.props.warnings.indexOf('Invalid Email.') > -1;
        const passwordInvalid = this.props.warnings.indexOf('Invalid Password.') > -1;

        return (
            <div className="warning">
                <span className="icon fa fa-exclamation-triangle"></span>
                {emailInvalid
                    && <p className="warning-text">{emailWarning}</p>}
                {passwordInvalid
                    && <p className="warning-text">{passwordWarning}</p>}
            </div>
        );
    }
    emailInputProps() {
        const emailInputProps = {
            name: 'email',
            placeholder: 'Email',
            type: 'text',
            inputIconClass: 'fa fa-envelope-o',
            onChange: this.props.onChange,
            onEnter: this.props.onEnter,
            value: this.props.userLogin.email,
            error: this.props.userLogin.errors.email,
        };
        return emailInputProps;
    }
    passwordInputProps() {
        const passwordInputProps = {
            name: 'password',
            placeholder: 'Password',
            type: 'password',
            inputIconClass: 'fa fa-lock',
            onChange: this.props.onChange,
            onEnter: this.props.onEnter,
            value: this.props.userLogin.password,
            error: this.props.userLogin.errors.password,
        };
        return passwordInputProps;
    }
    render() {
        return (
            <form className="admin-login-form">
                <h2 className="h-secondary">Site Admin Login</h2>
                {this.props.warnings
                    && this.props.warnings.length > 0
                    && this.renderWarnings()}
                <TextInput {...this.emailInputProps()} />
                <TextInput {...this.passwordInputProps()} />
                <button className="btn btn-sm btn-primary"
                    onClick={this.props.onLogin}
                    type="button">Login</button>
            </form>
        );
    }
}

AdminLogin.propTypes = {
    userLogin: React.PropTypes.object.isRequired,
    onChange: React.PropTypes.func.isRequired,
    onLogin: React.PropTypes.func.isRequired,
    onEnter: React.PropTypes.func.isRequired,
    warnings: React.PropTypes.array,
    focusObject: React.PropTypes.string,
};
