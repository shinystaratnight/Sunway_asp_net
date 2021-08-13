import React from 'react';
import moment from 'moment';

export default class FlightSummary extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    formatDuration(minutes) {
        const hourMinutes = 60;
        const hours = Math.floor(minutes / hourMinutes);
        const remainingMinutes = minutes % hourMinutes;

        const hourDisplay = hours > 0 ? `${hours}h ` : '';
        const minDisplay = remainingMinutes > 0 ? remainingMinutes : '';

        return `${hourDisplay}${minDisplay}`;
    }
    renderStopsDisplay() {
        const sectors = this.props.sectors;
        const result = this.props.result;
        let displayText = '';
        const expandable = sectors.length > 1;
        switch (sectors.length - 1) {
            case 0:
                displayText = 'Direct';
                break;
            case 1:
                displayText = '1 Stop';
                break;
            default:
                displayText = `${sectors.length - 1} Stops`;
        }
        const stopsDisplayProps = {
            className: 'flight-stops',
        };
        let stopDisplay = <p {...stopsDisplayProps}>{displayText}</p>;
        if (this.props.isExpanded) {
            const totalSectors
                = result.MetaData.FlightSectors
                .filter(fs => fs.Direction === this.props.direction).length;
            displayText = `Flight ${this.props.index + 1} of ${totalSectors}`;
            stopDisplay
                = <p {...stopsDisplayProps}>
                      {displayText}
                  </p>;
        } else if (expandable) {
            stopDisplay
                = <p {...stopsDisplayProps}>
                      {displayText}
                  </p>;
        }
        return stopDisplay;
    }
    render() {
        const sectors = this.props.sectors;
        const departingSector = sectors[0];
        const arrivingSector = sectors[sectors.length - 1];
        const departureAirport = this.props.departureAirport;
        const arrivalAirport = this.props.arrivalAirport;
        const flightCarrier = this.props.flightCarrier;
        const flightCarrierLogo = `${this.props.logoBaseUrl}${flightCarrier.Logo}`;

        const result = this.props.result;
        const toggleKey = `${result.ComponentToken}_${this.props.direction}`;
        const expandable = sectors.length > 1;
        const isExpanded = this.props.isExpanded;
        const iconType = isExpanded ? 'minus' : 'plus';
        const icon = {
            show: (expandable || isExpanded) && this.props.index === 0,
            className: `icon-toggle fa fa-${iconType}`,
        };
        let containerClass = 'flight-summary';
        containerClass
            = isExpanded && this.props.index > 0
            ? `${containerClass} additional-sector` : containerClass;
        const renderFlightDuration = !expandable || isExpanded;
        const flightDurationText = renderFlightDuration
            ? this.formatDuration(departingSector.TravelTime) : '';
        const departureDate = moment(departingSector.DepartureDate).format('ddd MMM Do');
        const arrivalDate = moment(arrivingSector.ArrivalDate).format('ddd MMM Do');
        return (
            <div className={containerClass}>
                {icon.show
                    && <span className={icon.className}
                        onClick={() => this.props.toggleSectors(toggleKey)}> </span>}
                <div className="flight-carrier">
                    <img className="flight-carrier-logo"
                            src={flightCarrierLogo}
                            alt={flightCarrier.Name} />
                    <p className="flight-code">{departingSector.FlightCode}</p>
                    <p className="flight-class">{this.props.flightClass}</p>
                    {departingSector.VehicleName
                        && <p className="flight-vehicle">{departingSector.VehicleName}</p>}
                </div>
                <div className="flight-depart">
                    <time className="flight-time" dateTime={departingSector.DepartureTime}>
                        {departingSector.DepartureTime}</time>
                    <time className="flight-time" dateTime={departingSector.DepartureDate}>
                        {departureDate}</time>
                    <p className="flight-iata-code">{departureAirport.IATACode}</p>
                    <p className="flight-airport">{departureAirport.Name}</p>
                </div>
                <div className="flight-journey-details">
                    <p className="flight-duration">
                            {flightDurationText}</p>
                    <span className="flight-journey-arrow"></span>
                    {this.renderStopsDisplay()}
                </div>
                <div className="flight-arrival">
                    <time className="flight-time" dateTime={arrivingSector.ArrivalTime}>
                        {arrivingSector.ArrivalTime}</time>
                    <time className="flight-time" dateTime={arrivingSector.ArrivalDate}>
                        {arrivalDate}</time>
                    <p className="flight-iata-code">{arrivalAirport.IATACode}</p>
                    <p className="flight-airport">{arrivalAirport.Name}</p>
                </div>
            </div>
        );
    }
}

FlightSummary.propTypes = {
    result: React.PropTypes.object.isRequired,
    sectors: React.PropTypes.array.isRequired,
    flightCarrier: React.PropTypes.object,
    flightClass: React.PropTypes.string,
    arrivalAirport: React.PropTypes.object,
    departureAirport: React.PropTypes.object,
    logoBaseUrl: React.PropTypes.string,
    direction: React.PropTypes.string,
    toggleSectors: React.PropTypes.func,
    isExpanded: React.PropTypes.bool,
    index: React.PropTypes.number,
};
