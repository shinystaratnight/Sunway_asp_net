import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

class CarouselContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const carouselOptions = configuration.CarouselOptions;

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            backgroundImage: contentModel.BackgroundImage,
            carouselItemType: contentModel.CarouselItemType,
            carouselItems: contentModel.CarouselItems,
            carouselTiles: parseInt(configuration.CarouselTiles, 10) || 0,
            carouselHeight: configuration.CarouselHeight,
            carouselHeightMobile: configuration.CarouselHeightMobile,
            carouselItemHeight: parseInt(configuration.CarouselItemHeight, 10),
            carouselItemMobileHeight: parseInt(configuration.CarouselItemMobileHeight, 10),
            autoScroll: carouselOptions.AutoScroll,
            pauseOnHover: carouselOptions.PauseOnHover,
            delay: parseInt(carouselOptions.Delay, 10),
            displayArrows: carouselOptions.DisplayArrows,
            arrowOffsetY: parseInt(carouselOptions.ArrowOffsetY, 10),
            indicatorType: carouselOptions.IndicatorType,
            indicatorPosition: carouselOptions.IndicatorPosition,
            sidePreview: carouselOptions.SidePreview,
            transitionType: carouselOptions.TransitionType,
            transitionDuration: carouselOptions.TransitionDuration,
            transitionTiming: carouselOptions.TransitionTiming,
        };
        if (contentModel.Button) {
            carouselContentProps.buttonText = contentModel.Button.Text;
            carouselContentProps.buttonURL = contentModel.Button.URL;
        }
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

CarouselContentContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

export default CarouselContentContainer;
