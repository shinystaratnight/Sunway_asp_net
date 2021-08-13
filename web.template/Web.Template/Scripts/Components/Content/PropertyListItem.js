import MarkdownText from '../common/MarkdownText';
import Price from '../common/price';
import Rating from 'components/common/rating';
import React from 'react';

export default class PropertyListItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderHighlightCircle() {
        const highlightText = this.props.chosenByUs ? 'Chosen By Us' : 'Special Offer';
        const highlightClass = this.props.chosenByUs
            ? 'highlight-circle standout' : 'highlight-circle';
        return (
            <div className={highlightClass}>
                <span className="highlight-circle-text">{highlightText}</span>
            </div>
        );
    }
    renderImage() {
        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.hotelImage}')`,
            },
        };
        const renderHighlightCircle = this.props.chosenByUs || this.props.hasSpecialOffer;
        return (
            <div className="offer-image-container img-container">
                {renderHighlightCircle
                    && this.renderHighlightCircle()}
                <div {...backgroundDiv}></div>
            </div>
        );
    }
    renderStarRating() {
        const starRatingConfiguration = this.props.siteConfiguration.StarRatingConfiguration;
        const ratingProps = {
            rating: this.props.starRating,
            size: 'sm',
            appendText: starRatingConfiguration.AppendText,
            displayHalfRatings: starRatingConfiguration.DisplayHalfRatings,
        };
        return (
            <Rating {...ratingProps} />
        );
    }
    renderOfferContent() {
        let address = this.props.resort;
        address = this.props.region ? `${this.props.region}, ${address}` : address;
        address = this.props.country ? `${this.props.country}, ${address}` : address;
        return (
            <div className="offer-content">
                <div className="offer-content-text">
                    <h3 className="h-tertiary">{this.props.hotelName}</h3>
                    <address className="address">
                        {this.props.address}
                    </address>
                    {this.props.starRating
                        && this.props.starRating !== '0.0'
                        && this.renderStarRating()}
                    <MarkdownText markdown={this.props.hotelSummary} />
                </div>
                <div className="tablet-button">
                    <a href={this.props.url} className="btn btn-default hidden-xs">
                        View hotel
                    </a>
                </div>
            </div>
        );
    }
    getPriceProps() {
        const currency = {
            CustomerSymbolOverride: this.props.currency.CustomerSymbolOverride,
            SymbolPosition: this.props.currency.SymbolPosition,
        };
        const pricingConfiguration = {
            PriceFormatDisplay: 'Rounded',
            ShowGroupSeperator: true,
        };
        const amount = this.props.hasSpecialOffer
            ? parseFloat(this.props.specialOffer.PerPersonPrice) : this.props.price;
        const priceProps = {
            amount,
            currency,
            pricingConfiguration,
            prependText: 'From ',
            appendText: ' ',
            classes: 'from-price h-secondary h-alt mb-0',
        };
        return priceProps;
    }
    renderOfferPricing() {
        const renderPrice = parseFloat(this.props.price) > 0 || this.props.hasSpecialOffer;
        const fallBackMessage = this.props.fallbackPriceText;
        const duration = this.props.hasSpecialOffer
            ? this.props.specialOffer.Duration : this.props.duration;
        const nightStay = renderPrice ? `${duration} night stay` : '';
        return (
            <div className="offer-pricing alt">
                {renderPrice
                    && <Price {... this.getPriceProps()} />}
                {renderPrice
                    && <p className="h-secondary h-alt mt-0 mb-0 per-person">Per Person</p>}
                {!renderPrice
                    && <p className="h-secondary">{fallBackMessage}</p>}
                {renderPrice
                    && <p className="duration-of-stay">{nightStay}</p>}
                <div className="offer-pricing-bottom">
                    <button className="btn btn-alt hidden-sm-up mobile-button" type="button">
                        View hotel
                    </button>
                </div>
            </div>
        );
    }
    render() {
        return (
            <article className="offer-horizontal row">
                <a href={this.props.url} itemProp="url" className="offer-link"></a>
                {this.renderImage()}
                {this.renderOfferContent()}
                {this.renderOfferPricing()}
            </article>
        );
    }
}

PropertyListItem.propTypes = {
    currency: React.PropTypes.object,
    hotelName: React.PropTypes.string,
    hotelImage: React.PropTypes.string,
    address: React.PropTypes.string,
    duration: React.PropTypes.string,
    hotelSummary: React.PropTypes.string,
    country: React.PropTypes.string,
    region: React.PropTypes.string,
    resort: React.PropTypes.string,
    price: React.PropTypes.number,
    starRating: React.PropTypes.string,
    url: React.PropTypes.string,
    fallbackPriceText: React.PropTypes.string,
    siteConfiguration: React.PropTypes.object,
    chosenByUs: React.PropTypes.bool,
    hasSpecialOffer: React.PropTypes.bool,
    specialOffer: React.PropTypes.object,
};
