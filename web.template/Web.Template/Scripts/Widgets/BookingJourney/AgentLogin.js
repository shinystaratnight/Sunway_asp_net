import 'widgets/bookingjourney/_agentlogin.scss';

import CheckboxInput from 'components/form/checkboxinput';
import React from 'react';
import TextInput from 'components/form/textinput';

export default class AgentLogin extends React.Component {
    renderWarning() {
        return (
            <div className="warning">
                <span className="icon fa fa-exclamation-triangle"></span>
                <p className="warning-text">{this.props.warning}</p>
            </div>
        );
    }
    render() {
        const usernameInputProps = {
            name: 'username',
            label: '',
            placeholder: 'Username',
            type: 'text',
            onChange: this.props.onChange,
            onEnter: this.props.onEnter,
            value: this.props.userLogin.username,
            error: this.props.userLogin.errors.username,
        };
        const passwordInputProps = {
            name: 'password',
            label: '',
            placeholder: 'Password',
            type: 'password',
            onChange: this.props.onChange,
            onEnter: this.props.onEnter,
            value: this.props.userLogin.password,
            error: this.props.userLogin.errors.password,
        };
        const saveLoginCheckboxProps = {
            name: 'saveLoginDetails',
            label: this.props.saveLoginText,
            onChange: this.props.onChange,
            value: this.props.userLogin.saveLoginDetails,
            displayInline: true,
        };
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </header>
                <div className="panel-body">
                    <form className="form">
                        {this.props.warning
                            && this.renderWarning()}
                        <TextInput {...usernameInputProps} />
                        <TextInput {...passwordInputProps} />
                        <CheckboxInput {...saveLoginCheckboxProps} />
                        <button
                            className="btn btn-sm btn-primary float-sm-right"
                            onClick={this.props.onLogin}
                            type="button">{this.props.loginButtonText}</button>
                    </form>
                </div>
            </div>
        );
    }
}

AgentLogin.propTypes = {
    title: React.PropTypes.string,
    saveLoginText: React.PropTypes.string,
    loginButtonText: React.PropTypes.string,
    userLogin: React.PropTypes.object,
    onLogin: React.PropTypes.func,
    onChange: React.PropTypes.func,
    onEnter: React.PropTypes.func,
    warning: React.PropTypes.string,
};
