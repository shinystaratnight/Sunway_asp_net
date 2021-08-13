import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';

export default class CarouselContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const entityTypes = [
            {
                name: 'siteBuilderProperty',
                type: 'Property',
                source: 'SiteBuilder',
            },
            {
                name: 'iVectorProperty',
                type: 'Property',
                source: 'iVector',
            },
        ];
        const specificEntities
            = ServerContainerFunctions.getSpecificEntities(entityTypes,
                this.props.specificEntitiesCollection);
        const siteBuilderProperty = specificEntities.siteBuilderProperty;
        const iVectorProperty = specificEntities.iVectorProperty.Property;
        const contentModel = JSON.parse(this.props.contentJSON);

        const carouselItems = [
            {
                Image: siteBuilderProperty.HeroImage,
                Caption: {
                    Header: iVectorProperty.Name,
                    SubHeader: iVectorProperty.Country,
                    Rating: parseFloat(iVectorProperty.Rating),
                    Text: iVectorProperty.Summary,
                },
                Configuration: {
                    Caption: {
                        VerticalPosition: 'Middle',
                        HorizontalPosition: 'Left',
                        DesktopTextWidth: 5,
                    },
                },
            },
        ];
        let carouselHeight = 'Extra Large';
        const carouselHeightMobile = 'Extra Small';
        if (contentModel.Configuration.Style === 'Small') {
            carouselHeight = 'Small';
        }

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 0,
            carouselHeight,
            carouselHeightMobile,
            arrowOffsetY: 0,
            autoScroll: true,
        };
        return carouselContentProps;
    }
    render() {
        return (
            <div className="widget padding-none carousel">
                <CarouselContent {...this.setupCarouselContentProps()} />
            </div>
        );
    }
}

CarouselContentContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
    site: React.PropTypes.object,
};
