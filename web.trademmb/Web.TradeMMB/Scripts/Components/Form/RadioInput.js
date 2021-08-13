import React from 'react';

export default class RadioInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        let wrapperClass = 'checkbox';
        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }
        const label = this.props.required ? `${this.props.label} *` : this.props.label;

        const inputProps = {
            type: 'radio',
            name: this.props.name,
            ref: this.props.name,
            onChange: this.props.onChange,
            autoComplete: 'off',
            value: this.props.value,
            checked: this.props.checked,
        };

        return (
            <div className={wrapperClass}>
                <label htmlFor={this.props.name}>
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

RadioInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    checked: React.PropTypes.bool.isRequired,
    onChange: React.PropTypes.func,
    value: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
    ]),
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
};
