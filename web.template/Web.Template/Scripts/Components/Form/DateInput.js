import 'react-datepicker/dist/react-datepicker.css';

import DateInputTextInput from './dateInputTextInput';
import DatePicker from 'react-datepicker';
import React from 'react';
import moment from 'moment';

export default class DateInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedDate: this.props.defaultDate ? moment(this.props.defaultDate) : null,
        };
        this.handleChange = this.handleChange.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (nextProps.defaultDate !== this.props.defaultDate) {
            this.setState({
                selectedDate: moment(nextProps.defaultDate),
            });
        }
    }
    handleChange(selectedDate) {
        this.setState({ selectedDate });
        if (this.props.onChange) {
            this.props.onChange(selectedDate.format('YYYY-MM-DD'), this.props.name);
        }
    }
    render() {
        const ownStockDates = [];
        const cachedDates = [];
        const highlightDates = [];
        if (this.props.useDealFinder) {
            if (this.props.highlightDates) {
                this.props.highlightDates.forEach(highlightDate => {
                    if (highlightDate.OwnStock) {
                        ownStockDates.push(moment(highlightDate.DepartureDate));
                    } else {
                        cachedDates.push(moment(highlightDate.DepartureDate));
                    }
                });
            }
            highlightDates.push({ 'datepicker-circle datepicker-highlight': ownStockDates });
            highlightDates.push({ 'datepicker-highlight': cachedDates });
        } else {
            this.props.highlightDates.forEach(highlightDate => {
                highlightDates.push(moment(highlightDate));
            });
        }
        let departureDate = null;
        if (!this.props.disabled
            && (!this.props.excludedDates
                || !this.props.excludedDates.includes(this.state.selectedDate))) {
            departureDate = this.state.selectedDate;
        }
        const excludeDates = [];
        if (this.props.excludedDates) {
            this.props.excludedDates.forEach(date => {
                excludeDates.push(moment(date.DepartureDate));
            });
        }
        const datePickerProps = {
            dateFormat: this.props.dateFormat,
            onChange: this.handleChange,
            minDate: moment(this.props.minDate),
            maxDate: moment(this.props.maxDate),
            selected: departureDate,
            locale: 'en-gb',
            className: 'react-datepicker__input',
            monthsShown: this.props.numberOfMonths,
            highlightDates,
            excludeDates,
            disabled: this.props.disabled,
            readOnly: this.props.readOnly,
        };
        let dateClass = 'date-input form-group';
        if (this.props.useDealFinder) {
            dateClass = `${dateClass} dealFinder`;
        }
        return (
            <div className={dateClass}>
                {this.props.label
                    && <label>{this.props.label}</label>}
                <DatePicker {...datePickerProps} customInput={
                    <DateInputTextInput loading={this.props.loading} />
                }>
                    {this.props.children}
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
    excludedDates: React.PropTypes.array,
    circledDates: React.PropTypes.array,
    children: React.PropTypes.object,
    disabled: React.PropTypes.bool,
    calendarOpen: React.PropTypes.bool,
    loading: React.PropTypes.bool,
    useDealFinder: React.PropTypes.bool,
    readOnly: React.PropTypes.bool,
};

DateInput.defaultProps = {
    dateFormat: 'DD MMM YY',
    weekStartDay: 1,
    minDate: new Date(),
    defaultDate: new Date(),
    numberOfMonths: 1,
};
