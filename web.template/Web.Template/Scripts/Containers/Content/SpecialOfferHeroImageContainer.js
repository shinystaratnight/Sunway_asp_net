import * as EntityActions from '../../actions/content/entityActions';

import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SpecialOfferHeroImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const property = this.props.property;
        const offer = this.props.offer;
        const carouselItems = [
            {
                Image: property.HeroImage,
                Caption: {
                    Header: offer.HotelName,
                    Text: offer.Summary,
                    Rating: parseFloat(offer.HotelRating),
                    SubHeader: offer.HotelCountry,
                },
                Configuration: {
                    Caption: {
                        VerticalPosition: 'Middle',
                        HorizontalPosition: 'Left',
                        DesktopTextWidth: 6,
                    },
                },
            },
        ];

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 0,
            carouselHeight: 'Medium',
            carouselHeightMobile: 'Medium',
            arrowOffsetY: 0,
            autoScroll: true,
        };
        return carouselContentProps;
    }
    render() {
        const renderContent
            = this.props.property && this.props.property.HeroImage && this.props.offer;
        return (
            <div>
                {renderContent
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

SpecialOfferHeroImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    blog: React.PropTypes.object,
    page: React.PropTypes.object,
    offer: React.PropTypes.object,
    property: React.PropTypes.object,
    site: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const property
        = state.page && state.page.content && state.page.content.property
        ? state.page.content.property
        : {};
    const offer = state.specialOffers
        && state.specialOffers.specialOfferDetails
        && state.specialOffers.specialOfferDetails.isLoaded
        ? state.specialOffers.specialOfferDetails.offer
        : {};
    return {
        property,
        offer,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions
        );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SpecialOfferHeroImageContainer);
