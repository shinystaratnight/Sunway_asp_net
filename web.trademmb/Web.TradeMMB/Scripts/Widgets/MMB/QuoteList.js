import Alert from 'widgets/common/alert';
import ModalPopup from 'components/common/modalpopup';
import Price from 'components/common/price';
import React from 'react';
import SendCustomerDocumentation from 'components/mmb/sendcustomerdocumentation';
import moment from 'moment';

export default class QuoteList extends React.Component {
    getPropertyName(quote) {
        let propertyName = 'N/A';
        quote.Components.forEach(component => {
            if (component.ComponentType === 'QuoteProperty') {
                propertyName = component.Name;
            }
        });
        return propertyName;
    }
    renderMobileHeader(key, heading) {
        return (
            <div key={key} className="col-xs-6 mobile-table-cell">{heading}</div>
        );
    }
    renderMobileValue(key, value) {
        return (
            <div key={key} className="col-xs-6 hidden-sm-up mobile-table-cell">{value}</div>
        );
    }
    renderMobileContent(values) {
        const headings = this.props.headings;
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
    renderDocumentionButtons(quote) {
        const docs = this.props.documentation;
        const quoteDocument = docs.QuoteDocument;
        const quoteDocumentProps = {
            className: 'fa fa-file mr-1',
            onClick: () =>
                this.props.handleViewDocument(quoteDocument.Document, 'quote',
                    quote.QuoteReference.replace('/', '-')),
            title: quoteDocument.Name,
        };
        const sendDocumentProps = {
            className: 'fa fa-envelope',
            onClick: () => this.props.handleShowDocumentPopup(quote.QuoteReference),
        };

        return (
            <div>
                <span {...quoteDocumentProps} aria-hidden="true"></span>
                <span {...sendDocumentProps} aria-hidden="true"></span>
            </div>
        );
    }
    renderDesktopValue(key, value) {
        return (
            <div key={key} className="table-cell hidden-xs">{value}</div>
        );
    }
    renderQuoteItem(quote, index) {
        const enquiryDate = moment(quote.BookingDate).format('D MMM \'YY');
        const departureDate = moment(quote.ArrivalDate).format('D MMM \'YY');
        const priceProps = {
            pricingConfiguration: this.props.pricingConfiguration,
            currency: this.props.currency,
        };
        const totalPrice = <Price {...priceProps} amount={quote.TotalPrice} />;
        const totalCommission = <Price {...priceProps} amount={quote.TotalCommission} />;
        const docButtons = this.renderDocumentionButtons(quote);


        const linkProps = {
            href: `/booking/quote/${quote.QuoteReference.replace('/', '-')}`,
            target: '_blank',
        };
        const repriceLink = <a {...linkProps}>Reprice</a>;
        const values = {
            quoteReference: quote.QuoteReference,
            customerName: `${quote.LeadCustomerFirstName} ${quote.LeadCustomerLastName}`,
            enquiryDate,
            departureDate,
            resort: quote.Resort,
            propertyName: this.getPropertyName(quote),
            totalPrice,
            totalCommission,
            docButtons,
            repriceLink,
        };
        const className = (index % 2 === 0)
            ? 'table-row' : 'table-row table-row-highlight';

        const valueKeys = Object.keys(values);
        const desktopContent = [];
        valueKeys.forEach(key => {
            desktopContent.push(this.renderDesktopValue(key, values[key]));
        });
        return (
            <div className={className}>
                {desktopContent}
                {this.renderMobileContent(values)}
            </div>
        );
    }
    renderHeaderCell(key, heading) {
        return (
            <div key={key} className="table-header hidden-xs">{heading}</div>
        );
    }
    renderHeader() {
        const keys = Object.keys(this.props.headings);
        const headingHtml = [];
        keys.forEach(key => {
            headingHtml.push(this.renderHeaderCell(key, this.props.headings[key]));
        });
        return (
            <div className="table-row">
                {headingHtml}
            </div>
        );
    }
    renderQuotes() {
        return (
            <div className="table">
                {this.renderHeader()}
                {this.props.quotes.map(this.renderQuoteItem, this)}
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
    renderDocsPopup() {
        const quoteDocument = this.props.documentation.QuoteDocument;
        let guidance = 'Please specify an email below if you wish for '
            + 'your documents to be sent to a different address';
        if (this.props.tradeSession.TradeContact && this.props.tradeSession.TradeContact.Email) {
            guidance += ` than ${this.props.tradeSession.TradeContact.Email}`;
        }
        const docsSendProps = {
            type: 'quote',
            reference: this.props.docsPopupReference.replace('/', '-'),
            title: 'Email documents',
            guidance,
            emailLabel: 'Alternative Email (optional):',
            handleSendDocumentation: this.props.handleSendDocument,
            documents: {
                FirstDocument: {
                    Document: quoteDocument.Document,
                    Name: quoteDocument.Name,
                    Show: true,
                },
                SecondDocument: {
                    Show: false,
                },
            },
            handleShowDocumentPopup: this.props.handleShowDocumentPopup,
        };
        return (
            <ModalPopup>
                <SendCustomerDocumentation {...docsSendProps}/>
            </ModalPopup>
        );
    }
    render() {
        return (
            <div className="container mb-2">
                {this.props.quotes.length > 0
                    && this.renderQuotes()}
                {this.props.quotes.length === 0
                    && this.renderWarningMessage()}
                {this.props.docsPopupReference !== ''
                    && this.renderDocsPopup()}
            </div>
        );
    }
}

QuoteList.propTypes = {
    quotes: React.PropTypes.array.isRequired,
    headings: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    currency: React.PropTypes.object.isRequired,
    warningMessage: React.PropTypes.string,
    documentation: React.PropTypes.object,
    handleViewDocument: React.PropTypes.func,
    handleShowDocumentPopup: React.PropTypes.func,
    handleSendDocument: React.PropTypes.func,
    docsPopupReference: React.PropTypes.string,
    tradeSession: React.PropTypes.object,
};
