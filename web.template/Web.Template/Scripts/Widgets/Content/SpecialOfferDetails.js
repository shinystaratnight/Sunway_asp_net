/* eslint-disable no-script-url */
import '../../../styles/widgets/content/_specialofferdetails.scss';

import MarkdownText from '../../components/common/MarkdownText';
import Price from '../../components/common/price';
import React from 'react';

export default class SpecialOfferDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getPriceProps(price, prependText, appendText, classes) {
        const currency = {
            CustomerSymbolOverride: this.props.currency.CustomerSymbolOverride,
            SymbolPosition: this.props.currency.SymbolPosition,
        };
        const pricingConfiguration = {
            PriceFormatDisplay: 'Rounded',
            ShowGroupSeperator: true,
        };
        const priceProps = {
            amount: parseInt(price, 10),
            currency,
            pricingConfiguration,
            prependText,
            appendText,
            classes,
        };
        return priceProps;
    }
    getAirport(id) {
        let airportReturn = {};
        for (let i = 0; i < this.props.airports.length; i++) {
            const airport = this.props.airports[i];
            if (parseInt(airport.Id, 10) === parseInt(id, 10)) {
                airportReturn = airport;
                break;
            }
        }
        return airportReturn;
    }

    renderAirportRow(airport, index) {
        const airportKey = `airport[${index}]`;
        const airportLookup = this.getAirport(airport.Airport);
        return (
            <tr key={airportKey}>
                <td className="column-left">Flying from {airportLookup.Name}</td>
                <td className="column-middle">
                    {airport.PerPersonPrice > 0
                    && <Price {... this.getPriceProps(airport.PerPersonPrice,
                        'from only ',
                        ' per person')} />}
                </td>
                <td className="column-right">
                    <a href="javascript:void(0)" onClick={() =>
                        this.props.getOfferPoster(parseInt(this.props.offerID, 10),
                                                  parseInt(airport.Airport, 10),
                                                  'UK', 'pdf')}>
                    Print Poster (.pdf)
                    </a>
                </td>
                <td className="column-right">
                    <a href="javascript:void(0)" onClick={() =>
                        this.props.getOfferPoster(parseInt(this.props.offerID, 10),
                                                parseInt(airport.Airport, 10),
                                                'UK', 'jpg')}>
                        Print Poster (.jpg)
                    </a>
                </td>
            </tr>
        );
    }
    renderGallery() {
        return (
            <div className="grid-images col-xs-12 col-md-6">
                {this.props.property.PrimaryImage
                    && <div className="primary-image-container">
                        <img className="primary-image" src={this.props.property.PrimaryImage} />
                    </div>}
                <div className="secondary-images">
                    {this.props.property.SecondaryImage
                        && <div className="secondary-image-container left">
                        <img className="secondary-image" src={this.props.property.SecondaryImage} />
                    </div>}
                    {this.props.property.TertiaryImage
                        && <div className="secondary-image-container right">
                    <img className="secondary-image" src={this.props.property.TertiaryImage} />
                    </div>}
                </div>
            </div>);
    }
    renderPriceBreakdownPrices() {
        const includesMarkdownProps = {
            markdown: this.props.priceIncludes,
        };
        return (<div className="offer-footer logged-in col-xs-12">
                    <div className="offer-option-details-container">
                        <div className="offer-price-details-logged-in">
                            <h3 className="h-tertiary">Prices Include:</h3>
                            {this.props.priceIncludes
                                && <MarkdownText {...includesMarkdownProps} />}
                        </div>
                        <div className="offer-option-details-logged-in">
                            <h3 className="h-tertiary offer-option-details-header">
                                Departing From:
                            </h3>
                            <table className="offers-table">
                                <tbody>
                                    {this.props.airportPrices.map(this.renderAirportRow, this)}
                                </tbody>
                            </table>
                            <small>
                                {this.props.smallPrint}
                                {this.props.cmsWebsiteName === 'GB'
                                    && ` ${this.props.smallPrintATOL}`}
                            </small>
                        </div>
                    </div>
                </div>);
    }
    renderPriceBreakdownNoPrices() {
        const includesMarkdownProps = {
            markdown: this.props.priceIncludes,
        };
        const markdownProps = {
            markdown: this.props.loggedOutText,
            containerStyle: 'alt h-alt',
        };
        return (<footer className="offer-footer logged-out col-xs-12">
                    <div className="offer-price-details-container col-xs-12 col-md-6">
                        <div className="offer-price-details">
                            <h3 className="h-tertiary inclusion-header" >Price Includes*</h3>
                            {this.props.priceIncludes
                                && <MarkdownText {...includesMarkdownProps} />}
                                {this.props.smallPrint
                                    && <small className="offer-small-print">
                                    *{this.props.smallPrint}
                                     {this.props.cmsWebsiteName === 'GB'
                                        && ` ${this.props.smallPrintATOL}`}
                                </small>}
                        </div>
                    </div>
                    <a href="javascript:void(0)"
                       onClick={() => {
                           this.props.toggleLoginMenu();
                           this.props.scrollTo(0);
                       }}>
                        <div className="offer-contact-details-container col-xs-12 col-md-6">
                            <div className="offer-contact-details alt">
                                <MarkdownText {...markdownProps} />
                            </div>
                        </div>
                    </a>
                </footer>);
    }
    renderPriceBreakdown() {
        return (
            <div>
                {this.props.userLoggedIn
                    && this.props.airportPrices
                    && this.props.airportPrices.length > 0
                    && this.renderPriceBreakdownPrices()}
                {!this.props.userLoggedIn
                    && this.renderPriceBreakdownNoPrices()}
            </div>
        );
    }
    renderOfferHeader() {
        const siteName = this.props.cmsWebsiteName === 'GB'
            ? 'UK' : this.props.cmsWebsiteName;
        let leadInPrice = this.props.iVectorProperty[`${siteName}FromPerPersonPrice`]
            ? this.props.iVectorProperty[`${siteName}FromPerPersonPrice`] : '';
        leadInPrice = Math.ceil(parseFloat(leadInPrice, 10));
        const leadInDuration = this.props.iVectorProperty.Duration;
        const showHeader = parseFloat(leadInPrice, 10) > 0 && parseInt(leadInDuration, 10) > 0;
        const symbol = this.props.currency.CustomerSymbolOverride;
        const leadInMessage = showHeader
            ? `${leadInDuration} night stay from ${symbol}${leadInPrice} per person`
            : 'Call for prices';
        return (
            <header className="offer-content-header">
                <h2 className="h-secondary offer-content-header-heading">Prices</h2>
                <p className="preamble">
                    {leadInMessage}
                </p>
            </header>
        );
    }
    renderBrochurePrice() {
        const offerMessageMarkdownProps = {
            markdown: this.props.offerMessage,
        };
        const priceCopy = this.props.property.LeadInMessage;
        const offerFooter = {
            header: this.props.offerFooter ? this.props.offerFooter.Header : '',
            content: this.props.offerFooter ? this.props.offerFooter.Content : '',
        };
        return (
            <div className="offer-info col-xs-12 col-md-6">
                {this.renderOfferHeader()}
                {priceCopy
                    && <MarkdownText markdown={this.props.property.LeadInMessage} />}
                <div className="disclaimer">
                    {this.props.offerMessage
                        && <MarkdownText {...offerMessageMarkdownProps} />}
                </div>
                {offerFooter.header
                    && <h2 className="h-secondary offer-content-header-heading">
                           {offerFooter.header}
                       </h2>}
                {offerFooter.content
                    && <p className="preamble">{offerFooter.content}</p>}
            </div>
        );
    }
    renderOfferText() {
        const headline = 'What\'s on offer:';
        const resultMarkdownProps = {
            markdown: this.props.offerPageText,
        };
        return (
            <div>
                <p className="h-tertiary">{headline}</p>
            <MarkdownText {...resultMarkdownProps} />
            </div>
        );
    }
    renderOfferDetail() {
        const offerMessageMarkdownProps = {
            markdown: this.props.offerMessage,
        };
        return (
            <div className="offer-info col-xs-12 col-md-6">
                <header className="offer-content-header">
                    <h2 className="h-secondary offer-content-header-heading">
                        {this.props.offerName}
                    </h2>
                    <p className="preamble">
                        {this.props.durationOfStay}
                    </p>
                </header>
                {this.props.offerPageText
                    && this.renderOfferText()}
                <a className="btn btn-default" type="button" href={this.props.propertyURL}>
                    View hotel details
                </a>
                <div className="disclaimer">
                    {this.props.offerMessage
                        && <MarkdownText {...offerMessageMarkdownProps} />}
                </div>
                <Price {... this.getPriceProps(this.props.price,
                    'From ',
                    ' per person',
                    'price-hook h-secondary')} />
            </div>);
    }
    renderOfferListItem(offer, index) {
        const offerKey = `hotel-offer-list-item[${index}]`;
        const offerprefix = `${offer.Duration} night stay from `;
        return (
            <a href={`/${offer.AdditionalInformation.Url}`} className="offer-a" key={offerKey}>
                <div className="offer-info-arrow-link">
                    <p>
                        <span className="offer-text-headline offer-text-link">
                            {offer.AdditionalInformation.Name}
                        </span>
                        <span className="offer-text-detail preamble">
                            <Price {... this.getPriceProps(offer.Price, offerprefix,
                                ' per person', 'offer-text-detail preamble')} />
                        </span>
                    </p>
                </div>
            </a>
        );
    }
    renderOfferList() {
        return (
            <div className="offer-info col-xs-12 col-md-6">
                {this.props.offers.items.map(this.renderOfferListItem, this)}
                <div>
                    <span className="offer-text-headline">Travel agent hotline</span>
                    <p className="offer-text-detail preamble">0141 955 4000</p>
                </div>
            </div>);
    }
    render() {
        return (
            <section className="widget-offer-content">
                <div className="container">
                    {this.props.mode === 'offerList'
                        && this.renderOfferList()}
                    {this.props.mode === 'noOffer'
                        && this.renderBrochurePrice()}
                    {this.props.mode === 'singleOffer'
                        && this.renderOfferDetail()}
                    {this.props.property
                        && this.renderGallery()}
                    {this.props.mode === 'singleOffer'
                        && this.renderPriceBreakdown()}
                </div>
            </section>);
    }
}

SpecialOfferDetails.propTypes = {
    userLoggedIn: React.PropTypes.bool,
    currency: React.PropTypes.object,
    mode: React.PropTypes.string,
    offerID: React.PropTypes.number,
    offerPageText: React.PropTypes.string,
    priceIncludes: React.PropTypes.string,
    smallPrint: React.PropTypes.string,
    smallPrintATOL: React.PropTypes.string,
    propertyURL: React.PropTypes.string,
    airportPrices: React.PropTypes.array,
    durationOfStay: React.PropTypes.string,
    price: React.PropTypes.number,
    offerName: React.PropTypes.string,
    offerMessage: React.PropTypes.string,
    OfferSaving: React.PropTypes.string,
    getOfferPoster: React.PropTypes.func,
    airports: React.PropTypes.array,
    loggedOutText: React.PropTypes.string,
    property: React.PropTypes.object,
    cmsWebsiteName: React.PropTypes.string,
    offers: React.PropTypes.object,
    offerFooter: React.PropTypes.object,
    iVectorProperty: React.PropTypes.object,
    toggleLoginMenu: React.PropTypes.func,
    scrollTo: React.PropTypes.func,
};
