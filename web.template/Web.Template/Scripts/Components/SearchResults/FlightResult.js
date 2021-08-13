import React from 'react';

export default class FlightResult extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            outboundSectorsExpanded: false,
            returnSectorsExpanded: false,
        };
    }
    getAirport(id) {
        let airportReturn = {};
        for (let i = 0; i < this.props.airports.length; i++) {
            const airport = this.props.airports[i];
            if (airport.Id === id) {
                airportReturn = airport;
                break;
            }
        }
        return airportReturn;
    }
    getFlightCarrier(id) {
        let flightCarrierReturn = {};
        for (let i = 0; i < this.props.flightCarriers.length; i++) {
            const flightCarrier = this.props.flightCarriers[i];
            if (flightCarrier.Id === id) {
                flightCarrierReturn = flightCarrier;
                break;
            }
        }
        return flightCarrierReturn;
    }
    formatPrice(price) {
        const parts = price.toFixed(2).toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
    formatDuration(minutes) {
        const hourMinutes = 60;
        const hours = Math.floor(minutes / hourMinutes);
        const remainingMinutes = minutes % hourMinutes;

        const hourDisplay = hours > 0 ? `${hours}h ` : '';
        const minDisplay = remainingMinutes > 0 ? remainingMinutes : '';

        return `${hourDisplay}${minDisplay}`;
    }
    renderFlightSummary(direction) {
        const metaData = this.props.result.MetaData;
        const sectors = metaData.FlightSectors.filter(sector => sector.Direction === direction);

        const departingSector = sectors[0];
        const arrivingSector = sectors[sectors.length - 1];

        const departureAirport = this.getAirport(departingSector.DepartureAirportID);
        const arrivalAirport = this.getAirport(arrivingSector.ArrivalAirportID);

        const flightCarrier = this.getFlightCarrier(departingSector.FlightCarrierID);
        const flightCarrierLogo = `http://imperatoretestadmin.ivector.co.uk/Content/Carriers/${flightCarrier.Logo}`;

        let stopsDisplay = '';
        switch (sectors.length - 1) {
            case 0:
                stopsDisplay = 'Direct';
                break;
            case 1:
                stopsDisplay = '1 Stop';
                break;
            default:
                stopsDisplay = `${sectors.length - 1} Stops`;
        }

        return (
            <div className="flight-summary">
                <div className="flight-carrier">
                    <img className="flight-carrier-logo"
                            src={flightCarrierLogo}
                            alt={flightCarrier.Name} />
                    <p className="flight-code">{departingSector.FlightCode}</p>
                    {departingSector.VehicleName
                        && <p className="flight-vehicle">{departingSector.VehicleName}</p>}
                </div>
                <div className="flight-depart">
                    <time className="flight-time" dateTime={departingSector.DepartureTime}>
                        {departingSector.DepartureTime}</time>
                    <p className="flight-iata-code">{departureAirport.IATACode}</p>
                    <p className="flight-airport">{departureAirport.Name}</p>
                </div>
                <div className="flight-journey-details">
                    <p className="flight-duration">
                        {this.formatDuration(departingSector.TravelTime)}</p>
                    <span className="flight-journey-arrow"></span>
                    <p className="flight-stops">{stopsDisplay}</p>
                </div>
                <div className="flight-arrival">
                    <time className="flight-time" dateTime={departingSector.DepartureTime}>
                        {arrivingSector.ArrivalTime}</time>
                    <p className="flight-iata-code">{arrivalAirport.IATACode}</p>
                    <p className="flight-airport">{arrivalAirport.Name}</p>
                </div>
            </div>
        );
    }
    renderFlightSectors() {
        return (
            <div className="flight-sectors">
            </div>
        );
    }
    renderResultPrice() {
        const result = this.props.result;

        let priceDiffDisplay = '';
        if (this.props.selectedFlight) {
            const priceDiff = result.Price - this.props.selectedFlight.Price;

            if (priceDiff < 0) {
                priceDiffDisplay = `- £${this.formatPrice(priceDiff * -1)}`;
            } else if (priceDiff > 0) {
                priceDiffDisplay = `+ £${this.formatPrice(priceDiff)}`;
            } else {
                priceDiffDisplay = `£${this.formatPrice(priceDiff)}`;
            }
        }

        return (
            <div className="result-price">
                {!this.props.changeFlight
                    && <p className="h-secondary h-alt">£{this.formatPrice(result.Price)}</p>}

                {this.props.changeFlight
                    && !result.isSelected
                    && <p className="h-secondary h-alt">{priceDiffDisplay}</p>}

                {!this.props.changeFlight
                    && <button className="btn btn-alt"
                    onClick={this.props.onSelect}>Book Now</button>}

                {this.props.changeFlight
                    && !result.isSelected
                    && <button className="btn btn-alt"
                                onClick={() => this.props.onChangeSelect(result.ComponentToken)}>
                        Select</button>}

                {this.props.changeFlight
                    && result.isSelected
                    && !this.props.isChangingFlight
                    && <button className="btn btn-alt btn-change-flight"
                        onClick={this.props.onChangeFlight}>Change</button>}

                {this.props.changeFlight
                    && result.isSelected
                    && this.props.isChangingFlight
                    && <button className="btn btn-alt btn-change-flight"
                        onClick={this.props.onChangeFlight}>Cancel</button>}
            </div>
        );
    }
    render() {
        return (
            <div className="flight-result">
                <div className="flight-information">
                    <div className="flight-journey flight-outbound">
                        {this.renderFlightSummary('Outbound')}
                    </div>
                    <div className="flight-journey flight-return">
                          {this.renderFlightSummary('Return')}
                    </div>
                </div>
                {this.renderResultPrice()}
            </div>
        );
    }
}

FlightResult.propTypes = {
    result: React.PropTypes.object.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    onSelect: React.PropTypes.func,
    changeFlight: React.PropTypes.bool,
    selectedFlight: React.PropTypes.object,
    isChangingFlight: React.PropTypes.bool,
    onChangeFlight: React.PropTypes.func,
    onChangeSelect: React.PropTypes.func,
};
