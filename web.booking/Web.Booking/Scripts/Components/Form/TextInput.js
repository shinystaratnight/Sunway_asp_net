import React from 'react';
import TextareaAutoSize from 'react-textarea-autosize';
import ValidateFunctions from 'library/validateFunctions';

export default class TextInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleOnEnter = this.handleOnEnter.bind(this);
        this.handleOnKeyDown = this.handleOnKeyDown.bind(this);
    }
    handleOnKeyDown(event) {
        if (this.props.numericOnly) {
            const isValidKey = ValidateFunctions.isNumericOrEditKey(event.which);
            if (!isValidKey) {
                event.preventDefault();
            }
        }
        if (this.props.formatAsTime) {
            const maxCharactersWithoutColon = 4;
            const maxCharactersWithColon = 5;
            let isValidKey
                = ValidateFunctions.isNumericOrEditKey(event.which)
                || (ValidateFunctions.isColonKey(event.which) && event.target.value.length === 2);
            let allowFifthChar = true;
            if (event.target.value.length === maxCharactersWithoutColon) {
                allowFifthChar
                    = ValidateFunctions.isEditKey(event.which)
                    || (event.target.value.indexOf(':') !== -1);
            }
            if (event.target.value.length === maxCharactersWithColon) {
                isValidKey = isValidKey && ValidateFunctions.isEditKey(event.which);
            }
            isValidKey = isValidKey && allowFifthChar;

            if (!isValidKey) {
                event.preventDefault();
            }
        }

        if (this.props.onKeyDown) {
            this.props.onKeyDown(event);
        }
    }
    handleOnEnter(event) {
        const code = 13;
        if (event.charCode === code) {
            this.props.onEnter(event);
        }
    }
    getWrapperClass() {
        let wrapperClass = 'form-group';
        if (this.props.containerClass) {
            wrapperClass += ` ${this.props.containerClass}`;
        }
        if (this.props.error && this.props.error.length > 0) {
            wrapperClass += ' has-error';
        }
        return wrapperClass;
    }
    getInputContainerClass() {
        let inputContainerClass = 'text-input-container';
        if (this.props.inputContainerClass) {
            inputContainerClass += ` ${this.props.inputContainerClass}`;
        }
        if (this.props.prependText) {
            inputContainerClass += ' has-prepend-text';
        }
        if (this.props.inputIconClass) {
            inputContainerClass += ' has-icon';
        }
        return inputContainerClass;
    }
    renderInput() {
        const inputProps = {
            type: this.props.type ? this.props.type : 'text',
            id: this.props.name,
            name: this.props.name,
            ref: this.props.name,
            className: 'form-control',
            placeholder: this.props.placeholder,
            onChange: this.props.onChange,
            autoComplete: 'off',
            value: this.props.value,
            onKeyDown: this.handleOnKeyDown,
            onKeyUp: this.props.onKeyUp ? this.props.onKeyUp : null,
            onKeyPress: this.props.onKeyPress ? this.props.onKeyPress : null,
            onBlur: this.props.onBlur ? this.props.onBlur : null,
            onFocus: this.props.onFocus ? this.props.onFocus : null,
        };
        if (this.props.maxLength) {
            inputProps.maxLength = this.props.maxLength;
        }
        if (this.props.onEnter) {
            inputProps.onKeyPress = this.handleOnEnter;
        }

        const dataAttributes = this.props.dataAttributes ? this.props.dataAttributes : [];
        for (let i = 0; i < dataAttributes.length; i++) {
            const dataAttribute = dataAttributes[i];
            inputProps[`data-${dataAttribute.key}`] = dataAttribute.value;
        }

        let input = <input {...inputProps} />;
        if (inputProps.type === 'textarea') {
            inputProps.rows = 3;
            input = <TextareaAutoSize {...inputProps} />;
        }
        return input;
    }
    renderLabel() {
        const label = this.props.required ? `${this.props.label} *` : this.props.label;
        const labelClass = this.props.labelClass ? this.props.labelClass : '';
        return (
            <label className={labelClass} htmlFor={this.props.name}>
                {this.props.labelIconClass
                    && <span className={this.props.labelIconClass}></span>}
                {label}
            </label>
        );
    }
    renderError() {
        let textClass = 'text-danger';
        if (this.props.warningTextClass) {
            textClass += ` ${this.props.warningTextClass}`;
        }
        return (
            <p className={textClass}>{this.props.error}</p>
        );
    }
    render() {
        const wrapperClass = this.getWrapperClass();
        const inputContainerClass = this.getInputContainerClass();
        return (
            <div className={wrapperClass}>
                {this.props.label
                    && this.renderLabel()}
                <div className={inputContainerClass}>
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
                    && this.renderError()}
            </div>
        );
    }
}

TextInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    labelIconClass: React.PropTypes.string,
    value: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
    ]),
    onChange: React.PropTypes.func,
    onEnter: React.PropTypes.func,
    onKeyDown: React.PropTypes.func,
    onKeyUp: React.PropTypes.func,
    onKeyPress: React.PropTypes.func,
    onBlur: React.PropTypes.func,
    onFocus: React.PropTypes.func,
    error: React.PropTypes.string,
    placeholder: React.PropTypes.string,
    required: React.PropTypes.bool,
    maxLength: React.PropTypes.number,
    description: React.PropTypes.string,
    type: React.PropTypes.string,
    prependText: React.PropTypes.string,
    containerClass: React.PropTypes.string,
    labelClass: React.PropTypes.string,
    inputContainerClass: React.PropTypes.string,
    inputIconClass: React.PropTypes.string,
    warningTextClass: React.PropTypes.string,
    dataAttributes: React.PropTypes.array,
    numericOnly: React.PropTypes.bool,
    formatAsTime: React.PropTypes.bool,
};
