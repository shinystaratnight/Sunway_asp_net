import React from 'react';

export default class DateSelectInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            day: this.props.value ? this.props.value.getDate() : null,
            month: this.props.value ? this.props.value.getMonth() : null,
            year: this.props.value ? this.props.value.getFullYear() : null,
        };

        this.handleDateChange = this.handleDateChange.bind(this);
    }
    handleDateChange(event) {
        const field = event.target.name;
        const value = parseInt(event.target.value, 10);
        const defaultYear = 1900;

        this.setState({ [field]: value });

        if (this.props.fields[field]) {
            this.props.updateValue(this.props.fields[field], value);
        } else {
            const day = this.state.day ? this.state.day : 1;
            const month = this.state.month ? this.state.month : 1;
            const year = this.state.year ? this.state.year : defaultYear;

            const date = new Date(day, month, year);
            if (this.props.onChange) {
                this.props.onChange(date, this.props.name);
            }
        }
    }
    getWrapperClass() {
        let wrapperClass = 'form-group';

        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }

        if (this.props.containerClass) {
            wrapperClass += ` ${this.props.containerClass}`;
        }

        return wrapperClass;
    }
    getSelectContainerClass() {
        let selectContainerClass = '';
        if (this.props.selectContainerClass) {
            selectContainerClass += ` ${this.props.selectContainerClass}`;
        }
        return selectContainerClass;
    }
    renderOption(option) {
        const key = `${this.props.name}_${option}`;
        return (
            <option key={key} className="option">{option}</option>
        );
    }
    renderDaySelect() {
        const selectProps = {
            name: 'day',
            ref: 'day',
            className: this.props.selectClass
                ? `form-control ${this.props.selectClass}`
                : 'form-control',
            onChange: this.handleDateChange,
        };

        if (this.state.day) {
            selectProps.value = this.state.day;
        } else if (this.props.placeholders.day) {
            selectProps.defaultValue = this.props.placeholders.day;
            selectProps.className += ' has-placeholder';
        }

        const options = [];
        const monthMaxDays = 31;
        for (let i = 1; i <= monthMaxDays; i++) {
            options.push(i);
        }
        return (
            <div className={this.getSelectContainerClass()}>
                <div className="custom-select">
                    <select {...selectProps}>
                        {this.props.placeholders.day
                            && <option className="placeholder" disabled>
                                    {this.props.placeholders.day}</option>}
                        {options.map(this.renderOption, this)}
                    </select>
                 </div>
            </div>
        );
    }
    renderMonthSelect() {
        const selectProps = {
            name: 'month',
            ref: 'month',
            className: this.props.selectClass
                ? `form-control ${this.props.selectClass}`
                : 'form-control',
            onChange: this.handleDateChange,
        };

        if (this.state.month) {
            selectProps.value = this.state.month;
        } else if (this.props.placeholders.month) {
            selectProps.defaultValue = this.props.placeholders.month;
            selectProps.className += ' has-placeholder';
        }

        const options = [];
        const monthMax = 12;
        for (let i = 1; i <= monthMax; i++) {
            options.push(i);
        }
        return (
            <div className={this.getSelectContainerClass()}>
                <div className="custom-select">
                     <select {...selectProps}>
                        {this.props.placeholders.month
                            && <option className="placeholder" disabled>
                                {this.props.placeholders.month}</option>}
                        {options.map(this.renderOption, this)}
                    </select>
                 </div>
            </div>
        );
    }
    renderYearSelect() {
        const selectProps = {
            name: 'year',
            ref: 'year',
            className: this.props.selectClass
                ? `form-control ${this.props.selectClass}`
                : 'form-control',
            onChange: this.handleDateChange,
        };

        if (this.state.year) {
            selectProps.value = this.state.year;
        } else if (this.props.placeholders.year) {
            selectProps.defaultValue = this.props.placeholders.year;
            selectProps.className += ' has-placeholder';
        }

        const options = [];
        for (let i = this.props.minYear; i <= this.props.maxYear; i++) {
            options.push(i);
        }
        return (
            <div className={this.getSelectContainerClass()}>
                <div className="custom-select">
                     <select {...selectProps}>
                        {this.props.placeholders.year
                            && <option className="placeholder" disabled>
                                {this.props.placeholders.year}</option>}
                        {options.map(this.renderOption, this)}
                    </select>
                 </div>
            </div>
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

        const label = this.props.required ? `${this.props.label} *` : this.props.label;
        const labelClass = this.props.labelClass ? this.props.labelClass : '';

        return (
            <div className={wrapperClass}>
                {this.props.label
                    && <label className={labelClass} htmlFor={this.props.name}>{label}</label>}

                {!this.props.hideDaySelect
                    && this.renderDaySelect()}

                {this.renderMonthSelect()}
                {this.renderYearSelect()}

                {this.props.description
                    && <p className="help-block">{this.props.description}</p>}
                {this.props.error
                    && this.renderError()}
            </div>
        );
    }
}

DateSelectInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string.isRequired,
    value: React.PropTypes.object,
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
    onChange: React.PropTypes.func,
    updateValue: React.PropTypes.func,
    placeholders: React.PropTypes.shape({
        day: React.PropTypes.string,
        month: React.PropTypes.string,
        year: React.PropTypes.string,
    }),
    fields: React.PropTypes.shape({
        day: React.PropTypes.string,
        month: React.PropTypes.string,
        year: React.PropTypes.string,
    }),
    minYear: React.PropTypes.number.isRequired,
    maxYear: React.PropTypes.number.isRequired,
    containerClass: React.PropTypes.string,
    labelClass: React.PropTypes.string,
    selectContainerClass: React.PropTypes.string,
    selectClass: React.PropTypes.string,
    warningTextClass: React.PropTypes.string,
    hideDaySelect: React.PropTypes.bool,
};

DateSelectInput.defaultProps = {
    hideDaySelect: false,
};
