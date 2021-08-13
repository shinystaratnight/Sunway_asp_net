import 'react-datepicker/dist/react-datepicker.css';

import DateInputTextInput from './dateInputTextInput';
import DatePicker from 'react-datepicker';
import React from 'react';
import moment from 'moment';

export default class DateInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedDate: moment(this.props.defaultDate),
        };
        this.handleChange = this.handleChange.bind(this);
    }
    handleChange(selectedDate) {
        this.setState({ selectedDate });
        if (this.props.onChange) {
            this.props.onChange(selectedDate.format('YYYY-MM-DD'), this.props.name);
        }
    }
    render() {
        const highlightDates = [];
        if (this.props.highlightDates) {
            this.props.highlightDates.forEach(highlightDate => {
                highlightDates.push(moment(highlightDate));
            });
        }
        const datePickerProps = {
            dateFormat: this.props.dateFormat,
            onChange: this.handleChange,
            minDate: moment(this.props.minDate),
            maxDate: moment(this.props.maxDate),
            selected: this.state.selectedDate,
            locale: 'en-gb',
            className: 'react-datepicker__input',
            monthsShown: this.props.numberOfMonths,
            highlightDates,
        };
        return (
            <div className="date-input form-group">
                {this.props.label
                    && <label>{this.props.label}</label>}
                <DatePicker {...datePickerProps} customInput={<DateInputTextInput />}>
                    { this.props.children }
                </DatePicker>
            </div>
        );
    }
}

DateInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    dateFormat: React.PropTypes.string,
    label: React.PropTypes.string,
    onChange: React.PropTypes.func,
    weekStartDay: React.PropTypes.number,
    minDate: React.PropTypes.object,
    maxDate: React.PropTypes.object,
    defaultDate: React.PropTypes.object,
    numberOfMonths: React.PropTypes.number,
    highlightDates: React.PropTypes.array,
    children: React.PropTypes.object,
};

DateInput.defaultProps = {
    dateFormat: 'DD MMM YY',
    weekStartDay: 1,
    minDate: new Date(),
    defaultDate: new Date(),
    numberOfMonths: 1,
};
