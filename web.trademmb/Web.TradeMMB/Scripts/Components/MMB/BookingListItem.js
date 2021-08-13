import ModalPopup from '../common/modalpopup';
import Price from '../common/price';
import React from 'react';
import RequestCancellation from './requestcancellation';
import SendCustomerDocumentation from './sendcustomerdocumentation';
import moment from 'moment';

export default class BookingListItem extends React.Component {
    constructor() {
        super();
        this.state = {};
    }
    getPropertyVehicleName() {
        let propertyVehicleName = '';
        const booking = this.props.booking;
        const propertyBooking
            = booking.ComponentList.filter(c => c.ComponentType === 'PropertyBooking');
        if (propertyBooking.length > 0) {
            propertyVehicleName = propertyBooking[0].hlpComponentName;
        } else {
            const transferBooking
                = booking.ComponentList.filter(c => c.ComponentType === 'TransferBooking');
            if (transferBooking.length > 0) {
                propertyVehicleName = transferBooking[0].hlpComponentName;
            }
        }
        return propertyVehicleName;
    }
    formatBookingDate(date) {
        const formattedDate = moment(date).format('D MMM \'YY');
        return (formattedDate);
    }
    renderMobileHeader(key, heading) {
        return (
            <div key={key} className="col-xs-6 mobile-table-cell">
                {heading}
            </div>
        );
    }
    renderMobileValue(key, value) {
        return (
            <div key={key} className="col-xs-6 hidden-sm-up mobile-table-cell">
                {value}
            </div>
        );
    }
    renderMobileContent(headings, values) {
        const valueKeys = Object.keys(values);
        const headingKeys = Object.keys(headings);
        const html = [];
        for (let i = 0; i < valueKeys.length && i < headingKeys.length; i++) {
            const rowHtml = [];
            const headingKey = headingKeys[i];
            rowHtml.push(this.renderMobileHeader(headingKey, headings[headingKey]));
            const valueKey = valueKeys[i];
            rowHtml.push(this.renderMobileValue(valueKey, values[valueKey]));
            const className = 'row mobile-table-row';
            html.push(<div key={`${i}-mobile-content`} className={className}>{rowHtml}</div>);
        }
        return (
            <div className="hidden-sm-up">
                {html}
            </div>
        );
    }
    renderDesktopValue(key, value) {
        return (
            <div key={key} className="table-cell hidden-xs">
                {value}
            </div>
        );
    }
    renderCancellationButton() {
        const iconProps = {
            className: 'fa fa-times',
            onClick: () =>
                this.props.handleToggleCancellationCharges(this.props.booking.BookingReference),
        };
        return (
            <span {...iconProps} aria-hidden="true"></span>
        );
    }
    renderDocsPopup() {
        let guidance = 'Please specify an email below if you wish for '
            + 'your documents to be sent to a different address';

        if (this.props.tradeSession.TradeContact && this.props.tradeSession.TradeContact.Email) {
            guidance += ` than ${this.props.tradeSession.TradeContact.Email}`;
        }

        const docsSendProps = {
            type: 'booking',
            reference: this.props.booking.BookingReference,
            title: 'Email documents',
            guidance,
            emailLabel: 'Alternative Email (optional):',
            handleSendDocumentation: this.props.handleSendDocument,
            documents: this.props.mmbDocumentation.CustomerDocuments,
            handleShowDocumentPopup: this.props.handleShowDocumentPopup,
        };
        return (
            <ModalPopup>
                <SendCustomerDocumentation {...docsSendProps}/>
            </ModalPopup>
        );
    }
    renderDocumentionButtons() {
        const docs = this.props.mmbDocumentation;
        const firstDocument = docs.FirstDocument;
        const secondDocument = docs.SecondDocument;
        const reference = this.props.booking.BookingReference;
        const firstDocumentProps = {
            className: 'fa fa-file mr-1',
            onClick: () =>
                this.props.handleViewDocument(firstDocument.Document, 'booking', reference),
        };
        const secondDocumentProps = {
            className: 'fa fa-file',
            onClick: () =>
                this.props.handleViewDocument(secondDocument.Document, 'booking', reference),
        };
        const sendDocumentProps = {
            className: 'fa fa-envelope',
            onClick: () => this.props.handleShowDocumentPopup(reference),
        };

        return (
            <div>
                <span {...firstDocumentProps} aria-hidden="true">
                    <span className="document-letter">
                        {firstDocument.DocumentLetter}
                    </span>
                </span>
                <span {...secondDocumentProps} aria-hidden="true">
                    <span className="document-letter">
                        {secondDocument.DocumentLetter}</span>
                    </span>
                <span {...sendDocumentProps} aria-hidden="true"></span>
            </div>
        );
    }
    renderCancellationPopup() {
        const requestCancellationProps = {
            currencySymbol: this.props.booking.CurrencySymbol,
            bookingReference: this.props.booking.BookingReference.toString(),
            cancellationInformation: this.props.cancellationInformation,
            selectedComponentTokens: this.props.selectedComponentTokens,
            handleToggleCancellationCharges: this.props.handleToggleCancellationCharges,
            handleComponentToggle: this.props.handleComponentToggle,
            handleCancelComponents: this.props.handleCancelComponents,
            labels: this.props.cancellationPopup,
            onChange: this.props.onChange,
            onError: this.props.onError,
            resetWarnings: this.props.resetWarnings,
            sendCancellationEmail: this.props.sendCancellationEmail,
            emailForm: this.props.emailForm,
            cancellationRequestEmailAddress: this.props.cancellationRequestEmailAddress,
            emailResponseMessage: this.props.emailResponseMessage,
            resetResponseMessage: this.props.resetResponseMessage,
            resetFormValues: this.props.resetFormValues,
        };
        return (
            <ModalPopup>
                <RequestCancellation {...requestCancellationProps}/>
            </ModalPopup>
        );
    }
    render() {
        const booking = this.props.booking;

        const pricingConfiguration = this.props.pricingConfiguration;
        pricingConfiguration.PriceFormatDisplay = 'TwoDP';

        const customerName = `${booking.LeadCustomerLastName}, ${booking.LeadCustomerFirstName}`;
        const bookingDate = this.formatBookingDate(booking.BookingDate);
        const arrivalDate = this.formatBookingDate(booking.ArrivalDate);

        const priceProps = {
            pricingConfiguration: this.props.pricingConfiguration,
            currency: this.props.currency,
        };

        const totalPrice = booking.TotalPrice
            ? <Price {...priceProps} amount={booking.TotalPrice} /> : '';

        const totalCommission = <Price {...priceProps} amount={booking.TotalCommission} />;

        const headings = this.props.headings;
        const docs = this.props.mmbDocumentation
            && booking.Status !== 'Cancelled' ? this.renderDocumentionButtons() : '';

        const values = {
            bookingReference: booking.BookingReference,
            customerName,
            bookingDate,
            arrivalDate,
            resort: booking.Resort,
            propertyVehicleName: this.getPropertyVehicleName(),
            totalPrice,
            totalCommission,
            status: booking.Status,
            accountStatus: booking.AccountStatus,
            docs,
        };

        if (this.props.showCancelRequestColumn) {
            values.cancelRequest = booking.Status
                !== 'Cancelled' ? this.renderCancellationButton() : '';
        }

        const valueKeys = Object.keys(values);
        const desktopContent = [];
        valueKeys.forEach(key => {
            desktopContent.push(this.renderDesktopValue(key, values[key]));
        });
        const className = (this.props.index % 2 === 0)
            ? 'table-row' : 'table-row table-row-highlight';
        return (
            <div className={className}>
                {desktopContent}
                {this.renderMobileContent(headings, values)}
                {this.props.renderCancellationCharges
                    && this.props.cancellationInformation
                    && this.renderCancellationPopup()}
                {this.props.renderDocsPopup
                    && this.renderDocsPopup()}
            </div>
        );
    }
}

BookingListItem.propTypes = {
    booking: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    currency: React.PropTypes.object.isRequired,
    headings: React.PropTypes.object.isRequired,
    index: React.PropTypes.number.isRequired,
    renderCancellationCharges: React.PropTypes.bool.isRequired,
    cancellationInformation: React.PropTypes.object,
    showCancelRequestColumn: React.PropTypes.bool,
    mmbDocumentation: React.PropTypes.object,
    selectedComponentTokens: React.PropTypes.array,
    handleToggleCancellationCharges: React.PropTypes.func.isRequired,
    handleComponentToggle: React.PropTypes.func.isRequired,
    handleCancelComponents: React.PropTypes.func.isRequired,
    handleViewDocument: React.PropTypes.func.isRequired,
    handleSendDocument: React.PropTypes.func.isRequired,
    handleShowDocumentPopup: React.PropTypes.func.isRequired,
    renderDocsPopup: React.PropTypes.bool,
    tradeSession: React.PropTypes.object,
    cancellationPopup: React.PropTypes.object,
    sendCancellationEmail: React.PropTypes.bool,
    onChange: React.PropTypes.func,
    onError: React.PropTypes.func,
    resetWarnings: React.PropTypes.func,
    emailForm: React.PropTypes.object,
    cancellationRequestEmailAddress: React.PropTypes.string,
    emailResponseMessage: React.PropTypes.func,
    resetResponseMessage: React.PropTypes.func,
    resetFormValues: React.PropTypes.func,
};
