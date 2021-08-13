import Price from '../../components/common/price';
import Rating from 'components/common/rating';
import React from 'react';

export default class FeaturedHotel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getPriceProps(price, customerCurrency, prependText, appendText, classes) {
        const currency = {
            CustomerSymbolOverride: customerCurrency.CustomerSymbolOverride,
            SymbolPosition: customerCurrency.SymbolPosition,
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
    renderRating() {
        const starRatingConfiguration = this.props.siteConfiguration.StarRatingConfiguration;
        const ratingProps = {
            rating: this.props.carouselItem.Property.Rating,
            appendText: starRatingConfiguration.AppendText,
            displayHalfRatings: starRatingConfiguration.DisplayHalfRatings,
            size: 'xs',
        };
        return (
            <Rating {...ratingProps} />
        );
    }
    renderOffer() {
        return (
            <div className="hotel-offer-price highlight">
                <a href={this.props.carouselItem.URL}>
                    <Price {... this.getPriceProps(
                    this.props.carouselItem.Property.Price,
                    this.props.carouselItem.Property.Currency,
                    `${this.props.carouselItem.Property.Duration} night stay from `,
                    ' per person',
                    '')} />
                </a>
            </div>
        );
    }
    render() {
        const backgroundDiv = {
            className: 'img-background',
            style: {},
        };

        if (this.props.carouselItem.Property
            && this.props.carouselItem.Property.Image.Source !== '') {
            backgroundDiv.style.backgroundImage
                = `url('${this.props.carouselItem.Property.Image.Source}')`;
        }

        const offerInfoClass = this.props.carouselItem.Property.Summary
            ? 'offer-info'
            : 'offer-info-condensed';
        const containerclass = `featured-hotel ${this.props.carouselItem.Property.Mode}`;
        const property = this.props.carouselItem.Property;
        const showHighlightCircle = property.Source === 'Special Offer'
            || property.Source === 'Chosen By Us';
        const highlightText = property.Source === 'Special Offer'
            ? 'Special Offer' : 'Chosen By Us';
        const highlightClass = property.Source === 'Special Offer'
            ? 'highlight-circle' : 'standout highlight-circle';

        return (
            <div className={containerclass}>
            {this.props.carouselItem.Property
                && <div className="hotel-image-recommends">
                    <div {...backgroundDiv}></div>
                </div>}
                <div className={offerInfoClass}>
                    <div className="offer-details highlight">
                        {showHighlightCircle
                            && <div className={highlightClass}>
                                <span className={`${highlightClass}-text`}>{highlightText}</span>
                        </div>}
                        <a href={this.props.carouselItem.URL}>
                            {this.props.carouselItem.Property
                            && <h3 className="hotel-name h-tertiary h-link"
                                itemProp="name">
                                    {this.props.carouselItem.Property.Name}
                                </h3>}
                        </a>
                        <p className="hotel-location" itemProp="address" itemScope itemType="http://schema.org/PostalAddress">
                            <span itemProp="addressLocality">
                                {this.props.carouselItem.Property
                                    && this.props.carouselItem.Property.Resort}
                            </span>,
                            {' '}
                            <span itemProp="addressCountry">
                                {this.props.carouselItem.Property
                                    && this.props.carouselItem.Property.Country}
                            </span>
                        </p>
                            {this.props.carouselItem.Property
                            && this.props.carouselItem.Property.Rating
                            && this.props.carouselItem.Property.Rating > 0
                            && this.renderRating()}
                                {this.props.carouselItem.Property
                                    && this.props.carouselItem.Property.Summary
                                    && <p className="offer-description">
                                    {this.props.carouselItem.Property.Summary}
                                </p>}
                    </div>
                    {this.props.carouselItem.Property.Price
                        && this.renderOffer()}
                </div>
            </div>
        );
    }
}

FeaturedHotel.propTypes = {
    carouselItem: React.PropTypes.shape({
        Property: React.PropTypes.object,
        URL: React.PropTypes.string,
    }).isRequired,
    siteConfiguration: React.PropTypes.object,
};
