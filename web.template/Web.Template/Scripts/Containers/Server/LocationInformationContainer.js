import LocationInformation from '../../widgets/content/locationinformation';
import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';

export default class LocationInformationContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getTripAdvisorProps() {
        const entityTypes = [
            {
                name: 'siteBuilderRegion',
                type: 'Region',
                source: 'SiteBuilder',
            },
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
        const siteBuilderRegion = specificEntities.siteBuilderRegion;
        const siteBuilderProperty = specificEntities.siteBuilderProperty;
        const iVectorProperty = specificEntities.iVectorProperty.Property;

        const title = 'Location';
        const leadParagraph = `${iVectorProperty.Resort}, ${iVectorProperty.Country}`;

        const defaultGoogleMapsZoomLevel = 13;
        const zoomLevel = siteBuilderProperty.GoogleMapZoomLevel
            ? siteBuilderProperty.GoogleMapZoomLevel
            : defaultGoogleMapsZoomLevel;
        const map = {
            latitude: iVectorProperty.Latitude,
            longitude: iVectorProperty.Longitude,
            zoomLevel,
            apiKey: this.props.site.SiteConfiguration.MapConfiguration.Key,
        };
        let travelInformation = siteBuilderRegion.LocationInformation;
        if (this.props.user.SelectedCmsWebsite.CountryCode === 'GB') {
            travelInformation = travelInformation.filter(info => info.Key !== 'FlightTimesROI');
        } else {
            travelInformation = travelInformation.filter(info => info.Key !== 'FlightTimesUK');
        }

        const transferInformation = {
            Title: 'Transfers',
            Information: siteBuilderProperty.TransferInformation,
        };
        travelInformation.push(transferInformation);

        const climateCopy = siteBuilderRegion.Climate.Description;

        const measures = siteBuilderRegion.Climate.Measures;
        const measureTemperatures = measures.filter(measure => measure.Type === 'Temperature')[0];
        const temperatures
            = measureTemperatures && measureTemperatures.Values
            ? measureTemperatures.Values
            : '';

        const props = {
            title,
            leadParagraph,
            map,
            travelInformation,
            climateCopy,
            temperatures,
        };
        return props;
    }
    render() {
        return (
            <div className="widget padding-md highlight">
                <LocationInformation {...this.getTripAdvisorProps()}/>
            </div>
        );
    }
}

LocationInformationContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    site: React.PropTypes.object,
    user: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
};
