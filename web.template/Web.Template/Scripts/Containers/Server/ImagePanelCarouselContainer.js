import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';
import ServerContainerFunctions from 'library/servercontainerfunctions';

export default class ImagePanelCarouselContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = JSON.parse(this.props.contentJSON);
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
        const contentModel = JSON.parse(this.props.contentJSON);
        const containerAttributes
            = ServerContainerFunctions.setupContainerAttributes('imagepanelcarousel', contentModel);

        return (
            <div {...containerAttributes}>
                {contentModel.hasOwnProperty('Images')
                    && contentModel.Images.length > 0
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

ImagePanelCarouselContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    site: React.PropTypes.object,
};
