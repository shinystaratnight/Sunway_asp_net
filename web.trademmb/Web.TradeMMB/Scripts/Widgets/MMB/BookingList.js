import '../../../styles/widgets/mmb/bookinglist.scss';

import Alert from '../common/alert';
import BookingListItem from '../../components/mmb/bookinglistitem';

import Paging from '../../components/common/paging';
import React from 'react';

export default class BookingList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderBookingListItem(booking, index) {
        const key = `bookinglistitem-${index}`;
        const renderCancellationCharges
            = booking.BookingReference === this.props.selectedCancellationBooking;
        const cancellationInformation = renderCancellationCharges
            ? this.props.cancellationInformation[booking.BookingReference] : {};
        const renderDocsPopup = this.props.docsPopupReference === booking.BookingReference;
        const props = {
            booking,
            pricingConfiguration: this.props.pricingConfiguration,
            currency: this.props.currency,
            headings: this.props.headings,
            index,
            renderCancellationCharges,
            cancellationInformation,
            showCancelRequestColumn: this.props.showCancelRequestColumn,
            selectedComponentTokens: this.props.selectedComponentTokens,
            handleToggleCancellationCharges: this.props.handleToggleCancellationCharges,
            handleComponentToggle: this.props.handleComponentToggle,
            handleCancelComponents: this.props.handleCancelComponents,
            handleViewDocument: this.props.handleViewDocument,
            handleSendDocument: this.props.handleSendDocument,
            handleShowDocumentPopup: this.props.handleShowDocumentPopup,
            mmbDocumentation: this.props.mmbDocumentation,
            renderDocsPopup,
            tradeSession: this.props.tradeSession,
            cancellationPopup: this.props.cancellationPopup,
            onChange: this.props.onChange,
            onError: this.props.onError,
            resetWarnings: this.props.resetWarnings,
            sendCancellationEmail: this.props.sendCancellationEmail,
            emailForm: this.props.emailForm,
            cancellationRequestEmailAddress: this.props.cancellationRequestEmail,
            emailResponseMessage: this.props.emailResponseMessage,
            resetResponseMessage: this.props.resetResponseMessage,
            resetFormValues: this.props.resetFormValues,
        };
        return (
            <BookingListItem key={key} {...props} />
        );
    }
    renderHeaderCell(key, heading) {
        const sortFields = this.props.sortFields;
        const sortable = sortFields.indexOf(key) > -1;
        return (
            <div key={key} className="table-header hidden-xs">
                {heading}
                {sortable
                    && <div className="sort-btn-container">
                        <div className="sort-btn-group">
                            <span className="sort-btn sort-ascending fa fa-chevron-up"
                            onClick={() => this.props.handleSort(key, 'ascending')}></span>
                            <span className="sort-btn sort-descending fa fa-chevron-down"
                            onClick={() => this.props.handleSort(key, 'descending')}></span>
                        </div>
                    </div>}
            </div>
        );
    }
    renderHeader() {
        const headings = this.props.headings;
        const notInArray = ['configuration'];

        if (!this.props.showCancelRequestColumn) {
            notInArray.push('CancelRequest');
        }

        const keys = Object.keys(headings).filter(key => notInArray.indexOf(key) === -1);
        const headingHtml = [];
        keys.forEach(key => {
            headingHtml.push(this.renderHeaderCell(key, headings[key]));
        });
        return (
            <div className="table-row">
                {headingHtml}
            </div>
        );
    }
    getPagingProps() {
        const props = {
            totalPages: this.props.paging.totalPages,
            currentPage: this.props.paging.currentPage,
            pageLinks: 3,
            onPageClick: this.props.handlePageChange,
        };
        return props;
    }
    renderBookingContent() {
        return (
            <div className="table">
                {this.renderHeader()}
                {this.props.bookings.map(this.renderBookingListItem, this)}
            </div>
        );
    }
    renderWarningMessage() {
        const alertProps = {
            name: this.props.warningMessage,
            type: 'warning',
            message: this.props.warningMessage,
            dismissible: false,
            acceptance: {},
        };
        return (
            <Alert {...alertProps} />
        );
    }
    render() {
        const displayPaging = this.props.paging.totalPages > 1;
        const renderBookings = this.props.bookings.length !== 0;
        const content = renderBookings ? this.renderBookingContent() : this.renderWarningMessage();
        return (
            <div className="container mb-2">
                {content}
                {displayPaging
                    && <Paging {...this.getPagingProps() } />}
            </div>
        );
    }
}

BookingList.propTypes = {
    headings: React.PropTypes.object,
    paging: React.PropTypes.object,
    bookings: React.PropTypes.array,
    sortFields: React.PropTypes.array,
    handleSort: React.PropTypes.func,
    handlePageChange: React.PropTypes.func,
    warningMessage: React.PropTypes.string,
    selectedCancellationBooking: React.PropTypes.string,
    cancellationInformation: React.PropTypes.object,
    showCancelRequestColumn: React.PropTypes.bool,
    selectedComponentTokens: React.PropTypes.array,
    handleToggleCancellationCharges: React.PropTypes.func,
    handleComponentToggle: React.PropTypes.func,
    handleCancelComponents: React.PropTypes.func,
    handleViewDocument: React.PropTypes.func,
    handleSendDocument: React.PropTypes.func,
    handleShowDocumentPopup: React.PropTypes.func,
    mmbDocumentation: React.PropTypes.object,
    docsPopupReference: React.PropTypes.string,
    pricingConfiguration: React.PropTypes.object.isRequired,
    currency: React.PropTypes.object.isRequired,
    tradeSession: React.PropTypes.object,
    cancellationPopup: React.PropTypes.object,
    onChange: React.PropTypes.func,
    onError: React.PropTypes.func,
    resetWarnings: React.PropTypes.func,
    sendCancellationEmail: React.PropTypes.bool,
    emailForm: React.PropTypes.object,
    cancellationRequestEmail: React.PropTypes.string,
    emailResponseMessage: React.PropTypes.func,
    resetResponseMessage: React.PropTypes.func,
    resetFormValues: React.PropTypes.func,
};
