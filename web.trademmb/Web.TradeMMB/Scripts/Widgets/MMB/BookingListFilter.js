import '../../../styles/widgets/mmb/bookinglist.scss';

import DateInput from '../../components/form/dateinput';
import React from 'react';
import SelectInput from '../../components/form/selectinput';
import TextInput from '../../components/form/textinput';

export default class BookingListFilter extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderResultsPerPage() {
        const validResultOptions
            = this.props.resultsPerPage.filter(r => r < this.props.numberOfResults);
        const invalidResultOptions
            = this.props.resultsPerPage.filter(r => r > this.props.numberOfResults);
        if (invalidResultOptions.length > 0) {
            validResultOptions.push(invalidResultOptions[0]);
        }
        const listItems = [];
        validResultOptions.forEach(o => {
            listItems.push(this.renderResultPerPage(o));
        });
        return (
            <div className="results-per-page float-left">
                {listItems}
                <p>Results per page</p>
            </div>
        );
    }
    renderResultPerPage(resultsPerPage) {
        const className = resultsPerPage === this.props.selectedResultsPerPage
            ? 'result-per-page selected' : 'result-per-page';
        const listItemAttributes = {
            key: resultsPerPage,
            className,
            onClick: () => this.props.handleResultsPerPage(resultsPerPage),
        };
        return (
            <span {...listItemAttributes}>
                {resultsPerPage}
            </span>
        );
    }
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
            onClick: () => this.props.handleResetFilters(),
        };
        return (
            <button {...buttonProps}>Reset Filters</button>
        );
    }
    renderFilters() {
        return (
            <div className="row">
                {this.renderTextFilter(this.props.filters.bookingReference)}
                {this.renderTextFilter(this.props.filters.guestName)}
                {this.renderDateFilter(this.props.filters.booked)}
                {this.renderDateFilter(this.props.filters.travelling)}
            </div>
        );
    }
    renderHeader() {
        return (
            <div className="row">
                <div className="col-xs-12">
                    <h2 className="h-tertiary mb-2">Find a booking</h2>
                </div>
            </div>
        );
    }
    render() {
        const shouldRenderResultsPerPage
            = this.props.resultsPerPage[0] < this.props.numberOfResults;
        return (
            <div className="container booking-list-filter mt-2">
                {this.props.displayHeader
                    && this.renderHeader()}
                {this.renderFilters()}
                <div className="row">
                    <div className="col-xs-12 col-sm-6">
                        {shouldRenderResultsPerPage
                            && this.renderResultsPerPage()}
                    </div>
                    <div className="col-xs-12 col-sm-6">
                        {this.renderResetButton()}
                    </div>
                </div>
            </div>
        );
    }
}

BookingListFilter.propTypes = {
    resultsPerPage: React.PropTypes.array,
    bookings: React.PropTypes.array,
    arrivalDates: React.PropTypes.object,
    bookedDates: React.PropTypes.object,
    filters: React.PropTypes.object,
    numberOfResults: React.PropTypes.number,
    selectedResultsPerPage: React.PropTypes.number,
    handleResultsPerPage: React.PropTypes.func,
    handleTextFilterChange: React.PropTypes.func,
    handleDateFilterChange: React.PropTypes.func,
    handleDatePickerChange: React.PropTypes.func,
    handleResetFilters: React.PropTypes.func,
    displayHeader: React.PropTypes.bool,
};
