import * as BookingActions from 'actions/mmb/bookingActions';
import * as EntityActions from 'actions/content/entityActions';
import * as FormActions from 'actions/mmb/emailActions';
import * as MMBConstants from 'constants/mmb';
import * as QuoteActions from 'actions/mmb/quoteActions';

import Alert from 'widgets/common/alert';
import BookingList from 'widgets/mmb/bookinglist';
import BookingListFilter from 'widgets/mmb/bookinglistfilter';
import QuoteList from 'widgets/mmb/quotelist';
import QuoteListFilter from 'widgets/mmb/quotelistfilter';

import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BookingListContainer extends React.Component {
    constructor(props) {
        super(props);
        this.tabTypes = {
            BOOKINGS: 'BOOKINGS',
            QUOTES: 'QUOTES',
        };
        this.state = {
            sendCustomerDocsPopup: '',
            selectedTab: this.tabTypes.BOOKINGS,
        };
        this.handleError = this.handleError.bind(this);
        this.handleSort = this.handleSort.bind(this);
        this.handlePageChange = this.handlePageChange.bind(this);
        this.handleResultsPerPage = this.handleResultsPerPage.bind(this);
        this.handleTextFilterChange = this.handleTextFilterChange.bind(this);
        this.handleDateFilterChange = this.handleDateFilterChange.bind(this);
        this.handleResetFilters = this.handleResetFilters.bind(this);
        this.handleDatePickerChange = this.handleDatePickerChange.bind(this);
        this.handleToggleCancellationCharges = this.handleToggleCancellationCharges.bind(this);
        this.handleComponentToggle = this.handleComponentToggle.bind(this);
        this.handleCancelComponents = this.handleCancelComponents.bind(this);
        this.handleViewDocument = this.handleViewDocument.bind(this);
        this.handleSendDocument = this.handleSendDocument.bind(this);
        this.handleShowDocumentPopup = this.handleShowDocumentPopup.bind(this);
        this.handleQuoteFilterReset = this.handleQuoteFilterReset.bind(this);
        this.setForm = this.setForm.bind(this);
        this.setResponseMessage = this.setResponseMessage.bind(this);
        this.resetResponseMessage = this.resetResponseMessage.bind(this);
        this.resetFormValues = this.resetFormValues.bind(this);
    }
    componentDidMount() {
        console.log('BookingListContainerLoaded');
        this.props.actions.loadAllBookings(this.props.entity.model.Filters);
        this.props.actions.loadEntity(this.props.site.Name, 'MMBDocumentation', 'default', 'live');
        this.loadQuotes();
    }
    getCancellationStatusMessage() {
        const status = this.props.mmb.cancellationStatus;
        const fullBookingCancellation = status.type === 'All';
        let message = status.Success
            ? `Booking ${status.bookingReference} has been successfully cancelled`
            : `Booking ${status.bookingReference} was not cancelled`;
        if (!fullBookingCancellation) {
            const base = `The following components on booking ${status.bookingReference}`;
            message = status.Success
                ? `${base} have been cancelled:` : `${base} were not cancelled:`;
            status.CancellationComponents.forEach(c => {
                message += ` ${c.Type},`;
            });
            message = message.slice(0, -1);
            message += '.';
        }
        return message;
    }
    getMinMaxDates(items, field) {
        const now = new Date();
        const unlikelyDaysAhead = 10000;
        let minDate = new Date();
        minDate.setDate(now.getDate() + unlikelyDaysAhead);
        minDate = minDate.toISOString();
        let maxDate = new Date(0).toISOString();
        const defaultDateString = '0001-01-01T00:00:00';
        for (let i = 0; i < items.length; i++) {
            const testValue = items[i][field];
            minDate = (minDate > testValue) && (testValue !== defaultDateString)
                ? testValue : minDate;
            maxDate = maxDate < testValue ? testValue : maxDate;
        }
        minDate = new Date(Date.parse(minDate));
        minDate.setDate(minDate.getDate() - 1);
        maxDate = new Date(Date.parse(maxDate));
        maxDate.setDate(maxDate.getDate() + 1);
        return ({ minDate, maxDate });
    }
    getDateRangeFromOption(option) {
        const now = new Date();
        let startDate = now;
        let endDate = now;
        switch (option) {
            case 'Today':
                startDate = this.setTimeToStart(now);
                endDate = this.setTimeToEnd(now);
                break;
            case 'Yesterday': {
                const yesterday = this.setDateDaysAgo(1);
                startDate = this.setTimeToStart(yesterday);
                endDate = this.setTimeToEnd(yesterday);
                break;
            }
            case 'Last Week': {
                const daysAgo = 7;
                const sevenDaysAgo = this.setDateDaysAgo(daysAgo);
                startDate = this.setTimeToStart(sevenDaysAgo);
                endDate = this.setTimeToEnd(now);
                break;
            }
            case 'Last Fortnight': {
                const daysAgo = 14;
                const fourteenDaysAgo = this.setDateDaysAgo(daysAgo);
                startDate = this.setTimeToStart(fourteenDaysAgo);
                endDate = this.setTimeToEnd(now);
                break;
            }
            default:
        }
        return ({
            startDate,
            endDate,
        });
    }
    getValidBookings() {
        const mmb = this.props.mmb;
        const validBookings = mmb.bookings.filter(b => b.display);
        return validBookings;
    }
    getValidQuotes() {
        const quotes = this.props.quotes.items;
        const validQuotes = quotes.filter(quote => quote.display);
        return validQuotes;
    }
    getBookingListProps() {
        const contentModel = this.props.entity.model;
        const sortFields = Object.keys(MMBConstants.SORT_FIELDS);
        const validBookings = this.getValidBookings();
        const mmb = this.props.mmb;
        const startIndex = (mmb.currentPage - 1) * mmb.resultsPerPage;
        let endIndex = mmb.currentPage * mmb.resultsPerPage;
        endIndex = endIndex > validBookings.length ? validBookings.length : endIndex;
        const shownBookings = validBookings.slice(startIndex, endIndex);
        const totalPages = Math.ceil(validBookings.length / mmb.resultsPerPage);
        const tradeSession = this.props.session.UserSession.TradeSession;
        const paging = {
            totalPages,
            currentPage: mmb.currentPage,
        };
        const props = {
            headings: contentModel.TableHeadings,
            bookings: shownBookings,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            currency: this.props.session.UserSession.SelectCurrency,
            paging,
            handleSort: this.handleSort,
            handlePageChange: this.handlePageChange,
            sortFields,
            warningMessage: contentModel.NoBookingsWarning,
            selectedCancellationBooking: mmb.selectedCancellationBooking,
            cancellationInformation: mmb.cancellationInformation,
            showCancelRequestColumn: contentModel.Configuration.DisplayCancelRequestColumn,
            selectedComponentTokens: mmb.selectedComponentTokens,
            handleToggleCancellationCharges: this.handleToggleCancellationCharges,
            handleComponentToggle: this.handleComponentToggle,
            handleCancelComponents: this.handleCancelComponents,
            handleViewDocument: this.handleViewDocument,
            handleSendDocument: this.handleSendDocument,
            handleShowDocumentPopup: this.handleShowDocumentPopup,
            mmbDocumentation: this.props.mmbDocumentation,
            docsPopupReference: this.state.sendCustomerDocsPopup,
            tradeSession,
            cancellationPopup: contentModel.CancellationPopup,
            onChange: this.setForm,
            onError: this.handleError,
            resetWarnings: this.props.actions.resetFormWarnings,
            sendCancellationEmail: contentModel.Configuration.SendEmailCancellationRequests,
            cancellationRequestEmail: contentModel.Configuration.CancellationRequestEmailAddress,
            emailForm: this.props.emailForm,
            emailResponseMessage: this.setResponseMessage,
            resetResponseMessage: this.resetResponseMessage,
            resetFormValues: this.resetFormValues,
        };
        return props;
    }
    getBookingListFilterProps() {
        const contentModel = this.props.entity.model;
        const validBookings = this.getValidBookings();
        const bookings = this.props.mmb.bookings;
        const arrivalDates = this.getMinMaxDates(bookings, 'ArrivalDate');
        const bookedDates = this.getMinMaxDates(bookings, 'BookingDate');
        const props = {
            resultsPerPage: MMBConstants.RESULTS_PER_PAGE,
            bookings: validBookings,
            numberOfResults: validBookings.length,
            selectedResultsPerPage: this.props.mmb.resultsPerPage,
            handleResultsPerPage: this.handleResultsPerPage,
            handleTextFilterChange: this.handleTextFilterChange,
            handleDateFilterChange: this.handleDateFilterChange,
            filters: this.props.mmb.filters,
            handleResetFilters: this.handleResetFilters,
            handleDatePickerChange: this.handleDatePickerChange,
            arrivalDates,
            bookedDates,
            displayHeader: !contentModel.Configuration.FindQuoteOption,
        };
        return props;
    }
    getQuoteListProps() {
        const contentModel = this.props.entity.model;
        const validQuotes = this.getValidQuotes();
        const tradeSession = this.props.session.UserSession.TradeSession;
        const props = {
            quotes: validQuotes,
            headings: contentModel.Quote.TableHeadings,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            currency: this.props.session.UserSession.SelectCurrency,
            warningMessage: contentModel.Quote.NoQuotesWarning,
            documentation: this.props.mmbDocumentation,
            handleViewDocument: this.handleViewDocument,
            handleSendDocument: this.handleSendDocument,
            handleShowDocumentPopup: this.handleShowDocumentPopup,
            docsPopupReference: this.state.sendCustomerDocsPopup,
            tradeSession,
        };
        return props;
    }
    getQuoteListFilterProps() {
        const quotes = this.props.quotes.items;
        const arrivalDates = this.getMinMaxDates(quotes, 'ArrivalDate');
        const bookedDates = this.getMinMaxDates(quotes, 'BookingDate');
        const props = {
            filters: this.props.quotes.filters,
            arrivalDates,
            bookedDates,
            handleTextFilterChange: this.handleTextFilterChange,
            handleDateFilterChange: this.handleDateFilterChange,
            handleDatePickerChange: this.handleDatePickerChange,
            handleResetFilters: this.handleQuoteFilterReset,
        };
        return props;
    }
    handleError(key, value) {
        this.props.actions.addFormWarning(key, value);
    }
    handleSort(field, direction) {
        this.props.actions.sortBookings(field, direction);
    }
    handleTextFilterChange(filter, value) {
        const newFilter = Object.assign({}, filter);
        newFilter.value = value;
        this.updateFilter(newFilter);
    }
    handleDateFilterChange(filter, value) {
        const newFilter = Object.assign({}, filter);
        newFilter.selectedOption = value;
        const maxStringLength = 19;
        if (value === 'Range') {
            const items = this.state.selectedTab === this.tabTypes.QUOTES
                ? this.props.quotes.items : this.props.mmb.bookings;
            const dates = this.getMinMaxDates(items, filter.field);
            newFilter.startDate = dates.minDate.toISOString().substring(0, maxStringLength);
            newFilter.endDate = dates.maxDate.toISOString().substring(0, maxStringLength);
        } else {
            const dates = this.getDateRangeFromOption(value);
            newFilter.startDate = dates.startDate.toISOString().substring(0, maxStringLength);
            newFilter.endDate = dates.endDate.toISOString().substring(0, maxStringLength);
        }
        this.updateFilter(newFilter);
    }
    handleDatePickerChange(filter, field, value) {
        const newFilter = Object.assign({}, filter);
        if (field === 'start') {
            newFilter.startDate = value;
        } else {
            newFilter.endDate = value;
        }
        this.updateFilter(newFilter);
    }
    handlePageChange(newPage) {
        this.props.actions.changePage(newPage);
    }
    handleResultsPerPage(resultsPerPage) {
        this.props.actions.updateResultsPerPage(resultsPerPage);
    }
    handleResetFilters() {
        this.props.actions.resetFilters();
    }
    handleToggleCancellationCharges(bookingReference) {
        this.props.actions.getCancellationDetails(bookingReference);
        this.props.actions.updateCancellationBooking(bookingReference);
    }
    handleComponentToggle(type, token) {
        this.props.actions.updateCancellationComponents(type, token);
    }
    handleCancelComponents(bookingReference) {
        this.props.actions.cancelComponents(bookingReference);
    }
    handleViewDocument(documentId, type, reference) {
        this.props.actions.viewBookingDocumentation(documentId, type, reference);
    }
    handleShowDocumentPopup(reference) {
        this.setState({ sendCustomerDocsPopup: reference });
    }
    handleSendDocument(documentId, type, reference, overrideEmail) {
        this.props.actions.sendBookingDocumentation(documentId, type, reference, overrideEmail);
    }
    handleQuoteFilterReset() {
        this.props.actions.quoteFilterReset();
    }
    loadQuotes() {
        const tradeSession = this.props.session.UserSession.TradeSession;
        const search = {
            TradeContactId: tradeSession.TradeContact.Id,
            BrandIds: [this.props.site.BrandId],
            Source: 'Web',
            EarliestDepartureDate: new Date(),
        };
        this.props.actions.quoteSearch(search);
    }
    setTimeToStart(date) {
        const newDate = new Date(date.getTime());
        newDate.setHours(0, 0, 0);
        return newDate;
    }
    setTimeToEnd(date) {
        const newDate = new Date(date.getTime());
        const maxHours = 23;
        const maxMinsSecs = 59;
        newDate.setHours(maxHours, maxMinsSecs, maxMinsSecs);
        return newDate;
    }
    setDateDaysAgo(days) {
        const now = new Date();
        const dayOfStart = now.getDate() - days;
        now.setDate(dayOfStart);
        return now;
    }
    setResponseMessage(value) {
        this.props.actions.setResponseMessage(value);
    }
    resetResponseMessage() {
        this.props.actions.resetResponseMessage();
    }
    updateFilter(filter) {
        if (this.state.selectedTab === this.tabTypes.QUOTES) {
            this.props.actions.quoteUpdateFilter(filter);
        } else {
            this.props.actions.filterResults(filter);
        }
    }
    renderBookingList() {
        return (
            <div>
                <BookingListFilter {...this.getBookingListFilterProps()} />
                <BookingList {...this.getBookingListProps()} />
            </div>
        );
    }
    renderNoBookingsWarning() {
        const alertProps = {
            name: 'No results warning',
            type: 'warning',
            message: this.props.entity.model.NoBookingsForTrade,
            dismissible: false,
            acceptance: {},
        };
        return (
            <div className="container">
                <div className="row">
                    <div className="col-xs-12">
                        <Alert {...alertProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderCancellationStatus() {
        const status = this.props.mmb.cancellationStatus;
        const type = status.Success ? 'success' : 'warning';
        const message = this.getCancellationStatusMessage();
        const alertProps = {
            name: 'Cancellation request response',
            type,
            message,
            dismissible: true,
            acceptance: {},
            onClose: () => this.props.actions.clearCancellationResponse(),
        };
        return (
            <div className="container">
                <div className="row">
                    <div className="col-xs-12">
                        <Alert {...alertProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderDocumentationWarning() {
        const docWarning = this.props.mmb.documentationWarning;
        const type = docWarning.success ? 'success' : 'warning';
        const reference = docWarning.bookingReference;
        const warningMessage = `Sorry, the documentation request for booking ${reference} failed`;
        const successMessage = `The documentation has been sent for booking ${reference}`;
        const message = docWarning.success ? successMessage : warningMessage;
        const alertProps = {
            name: 'Cancellation request response',
            type,
            message,
            dismissible: true,
            acceptance: {},
            onClose: () => this.props.actions.updateDocumentationWarning(''),
        };
        return (
            <div className="container">
                <div className="row">
                    <div className="col-xs-12">
                        <Alert {...alertProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderQuotes() {
        return (
            <div>
                <QuoteListFilter {...this.getQuoteListFilterProps()} />
                <QuoteList {...this.getQuoteListProps()} />
            </div>
        );
    }
    renderTabs() {
        const tabs = [
            {
                tabType: this.tabTypes.BOOKINGS,
                display: 'Find a booking',
            },
            {
                tabType: this.tabTypes.QUOTES,
                display: 'Find a quote',
            },
        ];
        return (
            <div className="container">
                 <ul className="tabs mt-2 mb-2">
                    {tabs.map(this.renderTabItem, this)}
                </ul>
            </div>
        );
    }
    renderTabItem(tab, index) {
        const isSelected = this.state.selectedTab === tab.tabType;
        const props = {
            key: `tab_${index}`,
            className: 'tab-item',
            onClick: () => { this.setState({ selectedTab: tab.tabType }); },
        };
        if (isSelected) {
            props.className += ' tab-selected';
        }
        return (
            <li {...props}>
                <span className="tab-item-name">{tab.display}</span>
            </li>
        );
    }
    render() {
        const contentModel = this.props.entity.model;
        const mmb = this.props.mmb;
        const renderContent = mmb.bookings.length > 0;
        const renderNoBookingsWarning
            = mmb.isLoaded && mmb.bookings.length === 0
                && this.state.selectedTab === this.tabTypes.BOOKINGS;
        const renderCancellationStatus = mmb.cancellationStatus
            && mmb.cancellationStatus.CancellationComponents
            && mmb.cancellationStatus.CancellationComponents.length;
        const renderDocumentationWarning = mmb.documentationWarning
            && mmb.documentationWarning.bookingReference;
        return (
            <div>
                {contentModel.Configuration.FindQuoteOption
                    && this.renderTabs()}
                {renderDocumentationWarning
                    && this.renderDocumentationWarning()}
                {renderCancellationStatus
                    && this.renderCancellationStatus()}
                {renderContent
                    && this.state.selectedTab === this.tabTypes.BOOKINGS
                    && this.renderBookingList()}
                {renderNoBookingsWarning
                    && this.renderNoBookingsWarning()}
                {this.state.selectedTab === this.tabTypes.QUOTES
                    && this.renderQuotes()}
            </div>
        );
    }
    setForm(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateFormValue(field, value);
    }
    resetFormValues() {
        this.props.actions.resetEmailForm();
    }
    updateFormValue(field, value) {
        this.props.actions.updateFieldValue(field, value);
    }
}

BookingListContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    mmb: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    mmbDocumentation: React.PropTypes.object,
    quotes: React.PropTypes.object,
    emailForm: React.PropTypes.object,
    resetResponseMessage: React.PropTypes.func,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const mmb = state.mmb.hasOwnProperty('bookings') ? state.mmb : { bookings: [] };
    const mmbDocumentation = state.entities['MMBDocumentation-default']
        && state.entities['MMBDocumentation-default'].isLoaded
        ? state.entities['MMBDocumentation-default'].model : {};
    const quotes = state.quotes ? state.quotes : {};
    const emailForm = state.emailForm ? state.emailForm : {};
    return {
        mmb,
        mmbDocumentation,
        quotes,
        emailForm,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BookingActions,
        EntityActions,
        FormActions,
        QuoteActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BookingListContainer);
