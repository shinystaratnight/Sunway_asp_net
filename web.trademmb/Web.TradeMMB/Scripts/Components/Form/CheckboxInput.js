import React from 'react';

export default class CheckboxInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getContainerClass() {
        let wrapperClass = this.props.containerClass
                ? `form-check ${this.props.containerClass}` : 'form-check';
        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }
        wrapperClass = this.props.displayInline
            ? `${wrapperClass} form-check-inline` : wrapperClass;
        wrapperClass = this.props.label ? wrapperClass : `${wrapperClass} no-label`;
        return wrapperClass;
    }
    getInputProps() {
        const inputProps = {
            type: 'checkbox',
            className: 'form-check-input',
            id: this.props.name,
            name: this.props.name,
            ref: this.props.name,
            onChange: this.props.onChange,
            autoComplete: 'off',
            checked: this.props.value,
            disabled: this.props.disabled,
        };
        return inputProps;
    }
    renderInput() {
        return (
            <input {...this.getInputProps()} />
        );
    }
    renderLabel() {
        const label = this.props.required ? `${this.props.label} *` : this.props.label;
        return (
            <label htmlFor={this.props.name} className="form-check-label">
                {this.renderInput()}{label}
            </label>
        );
    }
    render() {
        return (
            <div className={this.getContainerClass()}>
                {this.props.label
                    && this.renderLabel()}
                {!this.props.label
                    && this.renderInput()}
                {this.props.description
                    && <p className="help-block">{this.props.description}</p>}
                {this.props.error
                    && <p className="text-danger">{this.props.error}</p>}
            </div>
        );
    }
}

CheckboxInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    disabled: React.PropTypes.bool,
    onChange: React.PropTypes.func,
    value: React.PropTypes.bool,
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
    displayInline: React.PropTypes.bool,
    containerClass: React.PropTypes.string,
};

CheckboxInput.defaultProps = {
    disabled: false,
};
