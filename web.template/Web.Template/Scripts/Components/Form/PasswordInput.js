import CheckboxInput from '../../components/form/checkboxinput';
import React from 'react';
import TextareaAutoSize from 'react-textarea-autosize';

export default class PasswordInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleKeyPress = this.handleKeyPress.bind(this);
    }
    handleKeyPress(event) {
        const code = 13;
        if (event.charCode === code) {
            this.props.onEnter(event);
        }
    }
    renderInput() {
        const inputProps = {
            type: this.props.passwordDisplayed ? 'text' : 'password',
            id: this.props.name,
            name: this.props.name,
            ref: this.props.name,
            className: 'form-control',
            placeholder: this.props.placeholder,
            onChange: this.props.onChange,
            autoComplete: 'off',
            value: this.props.value,
            onKeyDown: this.props.onKeyDown ? this.props.onKeyDown : null,
            onKeyUp: this.props.onKeyUp ? this.props.onKeyUp : null,
            onBlur: this.props.onBlur ? this.props.onBlur : null,
        };
        if (this.props.maxLength) {
            inputProps.maxLength = this.props.maxLength;
        }
        if (this.props.onEnter) {
            inputProps.onKeyPress = this.handleKeyPress;
        }

        let input = <input {...inputProps} />;
        if (inputProps.type === 'textarea') {
            inputProps.rows = 3;
            input = <TextareaAutoSize {...inputProps} />;
        }
        return input;
    }
    renderCheckbox() {
        const passwordCheckProps = {
            name: `${this.props.name}_check`,
            label: 'Show Password',
            onChange: this.props.onPasswordToggle,
            value: this.props.passwordDisplayed,
        };
        return (<CheckboxInput {...passwordCheckProps} />);
    }
    renderLabel() {
        const label = this.props.required ? `${this.props.label} *` : this.props.label;
        return (
            <label htmlFor={this.props.name}>
                {this.props.labelIconClass
                    && <span className={this.props.labelIconClass}></span>}
                {label}
            </label>
        );
    }
    render() {
        let wrapperClass = 'form-group';
        wrapperClass = this.props.containerClasses
            ? `${wrapperClass} ${this.props.containerClasses}`
            : wrapperClass;
        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }

        let inputContainerClasses = 'text-input-container';
        if (this.props.prependText) {
            inputContainerClasses = `${inputContainerClasses} has-prepend-text`;
        }
        if (this.props.inputIconClass) {
            inputContainerClasses = `${inputContainerClasses} has-icon`;
        }

        return (
            <div>
                <div className={wrapperClass}>
                    {this.props.label
                        && this.renderLabel()}
                    <div className={inputContainerClasses}>
                        {this.props.prependText
                            && <span className="input-prepend-text">
                            {this.props.prependText}
                            </span>}
                        {this.props.inputIconClass
                            && <span className={`icon ${this.props.inputIconClass}`}></span>}
                            {this.renderInput()}

                    </div>
                    {this.props.description
                        && <p className="help-block">{this.props.description}</p>}
                    {this.props.error
                        && <p className="text-danger">{this.props.error}</p>}
                </div>
                {this.renderCheckbox()}
            </div>
        );
    }
}

PasswordInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    labelIconClass: React.PropTypes.string,
    value: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
    ]),
    onChange: React.PropTypes.func,
    onPasswordToggle: React.PropTypes.func,
    onEnter: React.PropTypes.func,
    error: React.PropTypes.string,
    placeholder: React.PropTypes.string,
    required: React.PropTypes.bool,
    maxLength: React.PropTypes.number,
    description: React.PropTypes.string,
    type: React.PropTypes.string,
    onKeyDown: React.PropTypes.func,
    onKeyUp: React.PropTypes.func,
    onBlur: React.PropTypes.func,
    prependText: React.PropTypes.string,
    containerClasses: React.PropTypes.string,
    inputIconClass: React.PropTypes.string,
    passwordDisplayed: React.PropTypes.bool,
};
