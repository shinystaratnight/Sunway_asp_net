import ClimateGraph from '../../components/content/monthgraph';
import React from 'react';

export default class LocationInformation extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getImageUrl(config) {
        const googleMapsBaseUrl = 'https://maps.googleapis.com/maps/api/staticmap?';
        let url = googleMapsBaseUrl;
        config.forEach(setting => {
            url += `${setting.key}=${setting.value}&`;
        });
        return url;
    }
    renderMap() {
        const location = `${this.props.map.latitude},${this.props.map.longitude}`;
        const imgConfiguration = [
            {
                key: 'center',
                value: location,
            },
            {
                key: 'size',
                value: '500x250',
            },
            {
                key: 'zoom',
                value: this.props.map.zoomLevel,
            },
            {
                key: 'markers',
                value: `color:red%7C${location}`,
            },
            {
                key: 'key',
                value: this.props.map.apiKey,
            },
        ];
        const imgUrl = this.getImageUrl(imgConfiguration);
        const altText = 'altText';
        return (
            <div className="col-xs-12 col-md-5 map-image-container">
                <img src={imgUrl} alt={altText} />
            </div>
        );
    }
    renderTravelInfoItem(item, index) {
        const key = `info-item-${item.Title}-${index}`;
        return (
            <div key={key}>
                <h4 className="h-tertiary map-widget-subtitle">{item.Title}</h4>
                <p className="map-widget-info">{item.Information}</p>
            </div>
        );
    }
    renderTravelInfo() {
        const numberOfBlocks = 4;
        const columnLeftInformation = this.props.travelInformation.slice(0, 2);
        const columnRightInformation = this.props.travelInformation.slice(2, numberOfBlocks);
        return (
            <div className="col-xs-12 col-md-6">
                    <h2 className="h-secondary">
                        Getting there
                    </h2>
                    <div className="row">
                        <div className="col-sm-6">
                            {columnLeftInformation.map(this.renderTravelInfoItem, this)}
                        </div>
                        <div className="col-sm-6">
                            {columnRightInformation.map(this.renderTravelInfoItem, this)}
                        </div>
                    </div>
                </div>
        );
    }
    renderClimateCopy() {
        return (
            <div className="col-xs-12 col-md-5">
                <h2 className="h-secondary">
                    Climate
                </h2>
                <p>{this.props.climateCopy}</p>
            </div>
        );
    }
    renderTemperatureChart() {
        const graphProps = {
            values: this.props.temperatures,
            yRangeMinimum: 0,
            yRangeMaximum: 50,
            dataType: 'Temperature',
        };
        return (
            <div className="col-xs-12 col-md-7 v-spacing-mobile-top v-spacing-mobile-bottom">
                <ClimateGraph {...graphProps}/>
            </div>
        );
    }
    render() {
        const renderMap = this.props.map;
        const renderTravelInfo = this.props.travelInformation;
        const renderClimateCopy = this.props.climateCopy;
        const renderTemperatureChart = this.props.temperatures;
        return (
            <div className="container location-information-content">
                <div className="row">
                        {this.props.title
                            && <div className="col-xs-12 center-block">
                                    <h2 className="h-secondary location-title">
                                        {this.props.title}
                                    </h2>
                               </div>}
                        {this.props.leadParagraph
                            && <div className="col-xs-12 center-block">
                                    <p className="preamble">
                                        {this.props.leadParagraph}
                                    </p>
                               </div>}
                        {renderMap
                            && this.renderMap()}
                        <div className="hidden-sm-down col-md-1"></div>
                        {renderTravelInfo
                            && this.renderTravelInfo()}
                </div>
                <div className="row climate-row">
                    {renderClimateCopy
                        && this.renderClimateCopy()}
                    {renderTemperatureChart
                        && this.renderTemperatureChart()}
                </div>
            </div>
        );
    }
}

LocationInformation.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    map: React.PropTypes.object,
    travelInformation: React.PropTypes.object,
    climateCopy: React.PropTypes.string,
    temperatures: React.PropTypes.string,
};
