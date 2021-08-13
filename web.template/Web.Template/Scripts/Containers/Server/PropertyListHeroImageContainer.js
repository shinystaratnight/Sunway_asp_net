import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';

export default class CarouselContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getMostSpecifcGeographyName() {
        const countryInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Country')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Country')[0]
                : false;
        const regionInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Region')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Region')[0]
                : false;
        const resortInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Resort')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Resort')[0]
                : false;

        let geographyInformation = regionInformation ? regionInformation : countryInformation;
        geographyInformation = resortInformation ? resortInformation : geographyInformation;
        return geographyInformation.Value;
    }
    setupCarouselContentProps() {
        const entityTypes = [
            {
                name: 'siteBuilderCountry',
                type: 'Country',
                source: 'SiteBuilder',
            },
        ];
        const specificEntities
            = ServerContainerFunctions.getSpecificEntities(entityTypes,
                this.props.specificEntitiesCollection);
        const siteBuilderCountry = specificEntities.siteBuilderCountry;
        const contentModel = JSON.parse(this.props.contentJSON);

        const carouselItems = [
            {
                Image: siteBuilderCountry.HeroImage,
                Caption: {
                    Header: this.getMostSpecifcGeographyName(),
                    SubHeader: '',
                    Text: contentModel.HeroImageText,
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
        const renderContent = true;
        return (
            <div className=" widget padding-none carousel">
                {renderContent
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
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
