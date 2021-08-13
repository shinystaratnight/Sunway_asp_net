import AgentLogin from 'widgets/bookingjourney/agentlogin';
import React from 'react';

class AgentLoginContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const agentLoginProps = {
            title: contentModel.Title,
            saveLoginText: contentModel.SaveLoginText,
            loginButtonText: contentModel.LoginButtonText,
            onLogin: () => {},
            onChange: () => {},
            onEnter: () => {},
            userLogin: {
                errors: {},
                username: '',
                password: '',
                saveLoginDetails: false,
            },
            warning: '',
        };
        return (
            <AgentLogin {...agentLoginProps} />
        );
    }
}

AgentLoginContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    user: React.PropTypes.object,
};

export default AgentLoginContainer;
