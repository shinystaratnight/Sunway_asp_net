import DateInput from '../../components/form/dateinput';
import React from 'react';
import SelectInput from '../../components/form/selectinput';
import TextInput from '../../components/form/textinput';

export default class QuoteListFilter extends React.Component {
    renderTextFilter(filter) {
        const textInputProps = {
            name: filter.name,
            label: filter.label,
            value: filter.value,
            onChange: (event) => this.props.handleTextFilterChange(filter, event.target.value),
            placeholder: filter.placeholder,
        };
        return (
            <div className="col-xs-12 col-md-3">
                <TextInput {...textInputProps} />
            </div>
        );
    }
    renderRangeDatePicker(filter) {
        const field = filter.field;
        const dates = field === 'ArrivalDate' ? this.props.arrivalDates : this.props.bookedDates;
        const minDate = dates.minDate;
        const maxDate = dates.maxDate;
        const minDateInputProps = {
            name: '',
            label: 'Start date',
            onChange: (date) => this.props.handleDatePickerChange(filter, 'start', date),
            minDate,
            maxDate,
            defaultDate: minDate,
            isRange: true,
        };
        const maxDateInputProps = {
            name: '',
            label: 'End date',
            onChange: (date) => this.props.handleDatePickerChange(filter, 'end', date),
            minDate,
            maxDate,
            defaultDate: maxDate,
            isRange: true,
        };
        return (
            <div>
                <DateInput {...minDateInputProps} />
                <DateInput {...maxDateInputProps} />
            </div>
        );
    }
    renderDateFilter(filter) {
        const selectInputProps = {
            name: filter.name,
            label: filter.label,
            options: filter.options,
            value: filter.selectedOption,
            onChange: (event) => this.props.handleDateFilterChange(filter, event.target.value),
        };
        const renderDatePickers = filter.selectedOption === 'Range';

        return (
            <div className="col-xs-12 col-md-3">
                <SelectInput {...selectInputProps} />
                {renderDatePickers
                    && this.renderRangeDatePicker(filter)}
            </div>
        );
    }
    renderResetButton() {
        const buttonProps = {
            className: 'btn btn-default float-left float-sm-right reset-btn',
            onClick: this.props.handleResetFilters,
        };
        return (
            <button {...buttonProps}>Reset Filters</button>
        );
    }
    render() {
        return (
            <div className="container quote-list-filter mt-2 mb-2">
                <div className="row">
                    {this.renderTextFilter(this.props.filters.quoteReference)}
                    {this.renderTextFilter(this.props.filters.guestName)}
                    {this.renderDateFilter(this.props.filters.enquired)}
                    {this.renderDateFilter(this.props.filters.travelling)}
                 </div>
                 <div className="row">
                    <div className="col-xs-12">
                        {this.renderResetButton()}
                    </div>
                 </div>
            </div>
        );
    }
}

QuoteListFilter.propTypes = {
    filters: React.PropTypes.object,
    arrivalDates: React.PropTypes.object,
    bookedDates: React.PropTypes.object,
    handleTextFilterChange: React.PropTypes.func,
    handleDateFilterChange: React.PropTypes.func,
    handleDatePickerChange: React.PropTypes.func,
    handleResetFilters: React.PropTypes.func,
};
