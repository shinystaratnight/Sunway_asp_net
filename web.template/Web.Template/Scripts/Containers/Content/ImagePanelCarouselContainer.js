import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

class ImagePanelCarouselContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: '',
            leadParagraph: '',
            backgroundImage: '',
            carouselItemType: 'ImagePanel',
            carouselItems: contentModel.Images,
            carouselTiles: 0,
            carouselHeight: configuration.CarouselHeight,
            carouselHeightMobile: configuration.CarouselHeightMobile,
            displayArrows: false,
            autoScroll: true,
        };

        return carouselContentProps;
    }
    render() {
        const contentModel = this.props.entity.model;
        return (
            <div>
                {contentModel.hasOwnProperty('Images')
                    && contentModel.Images.length > 0
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

ImagePanelCarouselContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object,
};

export default ImagePanelCarouselContainer;
