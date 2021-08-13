import React from 'react';

export default class CheckboxInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        let wrapperClass = 'form-check';
        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }
        wrapperClass = this.props.displayInline
            ? `${wrapperClass} form-check-inline` : wrapperClass;

        const label = this.props.required ? `${this.props.label} *` : this.props.label;

        const inputProps = {
            type: 'checkbox',
            className: 'form-check-input',
            id: this.props.name,
            name: this.props.name,
            ref: this.props.name,
            onChange: this.props.onChange,
            autoComplete: 'off',
            checked: this.props.value,
        };

        return (
            <div className={wrapperClass}>
                <label htmlFor={this.props.name} className="form-check-label">
                    <input {...inputProps} />
                    {label}
                 </label>
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
    label: React.PropTypes.string.isRequired,
    onChange: React.PropTypes.func,
    value: React.PropTypes.bool,
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
    displayInline: React.PropTypes.bool,
};
