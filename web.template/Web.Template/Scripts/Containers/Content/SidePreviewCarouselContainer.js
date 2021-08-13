import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

class SidePreviewCarouselContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = this.props.entity.model;
        const carouselItems = [];
        for (let i = 0; i < contentModel.CarouselItems.length; i++) {
            const contentModelItem = contentModel.CarouselItems[i];
            const configuration = contentModel.Configuration;
            const carouselItem = {
                Image: contentModelItem.Image,
                URL: '',
                Caption: {
                    Header: contentModelItem.Caption.Header,
                    SubHeader: contentModelItem.Caption.SubHeader,
                    Text: contentModelItem.Caption.Text,
                },
                Configuration: {
                    Sequence: 1,
                    Caption: {
                        VerticalPosition: configuration.Caption.VerticalPosition,
                        HorizontalPosition: configuration.Caption.HorizontalPosition,
                        DesktopTextWidth: 12,
                    },
                },
            };
            carouselItems.push(carouselItem);
        }

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: contentModel.Title,
            leadParagraph: '',
            backgroundImage: '',
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 1,
            carouselHeight: 'Medium',
            carouselHeightMobile: 'Medium',
            autoScroll: false,
            pauseOnHover: false,
            delay: 5000,
            displayArrows: true,
            arrowOffsetY: 0,
            indicatorType: 'Circles',
            indicatorPosition: 'Center',
            sidePreview: true,
            transitionType: 'Scroll',
            transitionDuration: 1000,
            transitionTiming: 'Ease',
        };
        return carouselContentProps;
    }
    render() {
        const contentModel = this.props.entity.model;
        return (
            <div>
                {contentModel.hasOwnProperty('CarouselItems')
                    && contentModel.CarouselItems.length > 0
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

SidePreviewCarouselContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object,
};

export default SidePreviewCarouselContainer;
