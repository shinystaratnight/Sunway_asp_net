import React from 'react';

export default class DateInputTextInput extends React.Component {
    render() {
        const inputProps = {
            type: 'text',
            className: 'react-datepicker__input',
            value: this.props.value,
            onClick: this.props.onClick,
            onChange: this.props.onChange,
            disabled: this.props.disabled,
            readOnly: this.props.readOnly,
        };
        const iconClass = `${this.props.loading ? 'fa-spin loading-' : ''}icon`;
        return (
            <div className="date-input-text-input">
                <input {...inputProps} />
                <span className={iconClass} onClick={this.props.onClick}></span>
            </div>
        );
    }
}

DateInputTextInput.propTypes = {
    onClick: React.PropTypes.func,
    onChange: React.PropTypes.func,
    value: React.PropTypes.string,
    disabled: React.PropTypes.bool,
    loading: React.PropTypes.bool,
    readOnly: React.PropTypes.bool,
};
