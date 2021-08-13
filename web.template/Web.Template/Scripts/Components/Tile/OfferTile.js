import Price from '../../components/common/price';
import Rating from 'components/common/rating';
import React from 'react';

export default class OfferTile extends React.Component {
    constructor(props) {
        super(props);
        this.renderOfferText = this.renderOfferText.bind(this);
        this.renderPropertyContent = this.renderPropertyContent.bind(this);
    }
    getPriceProps(price, customerCurrency, appendText, classes) {
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
            appendText,
            classes,
        };
        return priceProps;
    }
    renderOfferText() {
        const property = this.props.tileItem.Property;

        return (
            <div className="offer-price">
                <p>
                    <span className="offer-text">{`${property.Duration} night stay from `}</span>
                </p>
                <Price {... this.getPriceProps(property.Price, property.Currency,
                        ' per person',
                        '')} />
            </div>
        );
    }
    renderRating() {
        const starRatingConfiguration = this.props.siteConfiguration.StarRatingConfiguration;
        const ratingProps = {
            rating: this.props.tileItem.Property.Rating,
            appendText: starRatingConfiguration.AppendText,
            displayHalfRatings: starRatingConfiguration.DisplayHalfRatings,
            size: 'xs',
        };
        return (
            <Rating {...ratingProps} />
        );
    }
    renderPropertyContent() {
        const property = this.props.tileItem.Property;
        const showHighlightCircle = property.Source === 'Special Offer'
            || property.Source === 'Chosen By Us';
        const highlightText = property.Source === 'Special Offer'
            ? 'Special Offer' : 'Chosen By Us';
        const highlightClass = property.Source === 'Special Offer'
            ? 'highlight-circle' : 'standout highlight-circle';
        return (
            <div className="offer-property">
                {showHighlightCircle
                    && <div className={highlightClass}>
                        <span className={`${highlightClass}-text`}>{highlightText}</span>
                    </div>}
                <h3 className="h-tertiary" itemProp="name">{property.Name}</h3>
                <p className="offer-location"
                    itemProp="address"
                    itemScope itemType="http://schema.org/PostalAddress">
                    <span itemProp="addressLocality">{property.Resort}</span>,&nbsp;
                    <span itemProp="addressCountry">{property.Country}</span>
                </p>
                {property.Rating
                    && property.Rating > 0
                    && this.renderRating()}
            </div>
        );
    }
    render() {
        const property = this.props.tileItem.Property;
        const containerProps = {
            className: 'offer-tile',
            itemType: 'http://schema.org/Hotel',
        };
        const backgroundDiv = {
            className: 'img-background',
            style: {},
        };

        if (property && property.Image.Source !== '') {
            backgroundDiv.style.backgroundImage = ` url('${property.Image.Source}')`;
        }

        return (
            <section itemScope {...containerProps}>
                {this.props.tileItem.URL
                    && <a className="a-container"
                          href={this.props.tileItem.URL}
                          itemProp="url"></a>}

                {this.props.tileItem.Property
                    && this.props.tileItem.Property.Image.Source !== ''
                    && <div className="img-container img-zoom-hover">
                      <div {...backgroundDiv}></div>
                    </div>}

                <div className="offer-content highlight">
                    {property
                        && this.renderPropertyContent()}

                    {this.renderOfferText()}
                </div>
            </section>
        );
    }
}

OfferTile.propTypes = {
    tileItem: React.PropTypes.shape({
        Property: React.PropTypes.object,
        OfferText: React.PropTypes.string.isRequired,
        OfferPricing: React.PropTypes.string,
        URL: React.PropTypes.string.isRequired,
    }).isRequired,
    siteConfiguration: React.PropTypes.object,
};
