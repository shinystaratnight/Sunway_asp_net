import CheckboxInput from '../../components/form/checkboxinput';
import React from 'react';

export default class Alert extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderAcceptanceMessage() {
        const warningCheckProps = {
            name: this.props.name,
            label: this.props.message,
            onChange: this.props.onAcceptanceChange,
            value: this.props.acceptance.value,
        };
        return (
            <CheckboxInput {...warningCheckProps} />
        );
    }
    renderMessage() {
        return (
            <p>{this.props.message}</p>
        );
    }
    render() {
        const containerClass = `alert alert-${this.props.type.toLowerCase()}`;
        return (
            <div className={containerClass}>
                <i className="alert-icon" aria-hidden="true"></i>
                <div className="alert-message">
                    {this.props.acceptance.onChange
                        && this.renderAcceptanceMessage()}
                    {!this.props.acceptance.onChange
                        && this.renderMessage()}
                </div>
                {this.props.dismissible
                    && <span className="alert-close"
                            onClick={() => this.props.onClose(this.props.name)}></span>}
            </div>
        );
    }
}

Alert.propTypes = {
    name: React.PropTypes.string.isRequired,
    type: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    dismissible: React.PropTypes.bool,
    onClose: React.PropTypes.func,
    acceptance: React.PropTypes.shape({
        onChange: React.PropTypes.func,
        value: React.PropTypes.bool,
    }),
    onAcceptanceChange: React.PropTypes.func,
};

Alert.defaultProps = {
};
