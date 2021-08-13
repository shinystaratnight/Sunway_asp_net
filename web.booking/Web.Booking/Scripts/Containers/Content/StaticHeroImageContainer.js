import CarouselContent from '../../widgets/common/CarouselContent';
import React from 'react';

class StaticHeroImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = this.props.entity.model;
        const carouselItems = [];
        const contentModelItem = contentModel.HeroBanner;
        const configuration = contentModel.Configuration;
        const carouselItem = {
            Image: contentModelItem.Image,
            URL: '',
            Caption: {
                Header: contentModelItem.Caption.Header,
                SubHeader: contentModelItem.Caption.SubHeader,
                Text: contentModelItem.Caption.Text,
                Rating: contentModelItem.Caption.Rating,
            },
            Configuration: {
                Sequence: 1,
                Caption: {
                    VerticalPosition: configuration.Caption.VerticalPosition,
                    HorizontalPosition: configuration.Caption.HorizontalPosition,
                    DesktopTextWidth: configuration.Caption.DesktopTextWidth,
                },
            },
        };
        carouselItems.push(carouselItem);

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: '',
            leadParagraph: '',
            backgroundImage: '',
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 0,
            carouselHeight: configuration.CarouselHeight,
            carouselHeightMobile: configuration.CarouselHeightMobile,
            displayArrows: false,
            arrowOffsetY: 0,
            sidePreview: false,
            autoScroll: true,
        };
        return carouselContentProps;
    }
    render() {
        const contentModel = this.props.entity.model;
        return (
            <div>
                {contentModel.hasOwnProperty('HeroBanner')
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

StaticHeroImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object,
};

export default StaticHeroImageContainer;
