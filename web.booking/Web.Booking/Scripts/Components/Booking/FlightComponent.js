import 'components/booking/_flightComponent.scss';

import React from 'react';
import moment from 'moment';

export default class FlightComponent extends React.Component {
    getAirport(id) {
        return this.props.airports.find(airport => airport.Id === id);
    }
    getFlightCarrier(id) {
        return this.props.flightCarriers.find(carrier => carrier.Id === id);
    }
    getFlightClass(id) {
        return this.props.flightClasses.find(flightClass => flightClass.Id === id);
    }
    getBaggage() {
        return this.props.component.SubComponents.find(subComponent =>
            subComponent.ExtraType === 'Baggage');
    }
    renderFlightDetails(title, component, flightDetails) {
        const departureDate = moment(flightDetails.DepartureDate).format('ll');
        let departureDateTime = moment(flightDetails.DepartureDate).format('YYYY-MM-DD');
        departureDateTime = `${departureDateTime}T${flightDetails.DepartureTime}`;

        const arrivalDate = moment(flightDetails.ArrivalDate).format('ll');
        let arrivalDateTime = moment(flightDetails.ArrivalDate).format('YYYY-MM-DD');
        arrivalDateTime = `${arrivalDateTime}T${flightDetails.ArrivalTime}`;

        const flightClass = this.getFlightClass(flightDetails.FlightClassId);

        const flightCarrierId = flightDetails.OperatingFlightCarrierId > 0
            ? flightDetails.OperatingFlightCarrierId
            : component.FlightCarrierId;
        const flightCarrier = this.getFlightCarrier(flightCarrierId);
        const flightCarrierLogo = `${this.props.cmsBaseUrl}${flightCarrier.Logo}`;

        const dtClass = 'flight-label hidden-sm-up';
        const ddClass = 'flight-detail';
        return (
            <div className="flight-leg">
                <h4 className="hidden-md-up">{title}</h4>
                <div className="flight-leg-details">
                    <div className="flight-direction flight-label hidden-sm-down">{title}</div>
                    <div className="flight-carrier">
                        <img className="flight-carrier-logo"
                                src={flightCarrierLogo}
                                alt={flightCarrier.Name} />
                    </div>
                    <div className="flight-code">
                        <p>{flightDetails.FlightCode}</p>
                        <p>{flightClass && flightClass.Name}</p>
                    </div>
                    <div className="flight-depart text-sm-right">
                        <dl className="data-list my-0">
                            <dt className={dtClass}>Departs</dt>
                            <dd className={ddClass}>
                                <time className="flight-time" dateTime={departureDateTime}>
                                    {departureDate} <strong>{flightDetails.DepartureTime}</strong>
                                </time>
                            </dd>
                            <dt className={dtClass}>From</dt>
                            <dd className={ddClass}>
                                <p className="flight-airport">
                                    {flightDetails.DepartureAirport.Name} (
                                    <strong>{flightDetails.DepartureAirport.IATACode}</strong>)
                                </p>
                            </dd>
                        </dl>
                    </div>
                    <div className="flight-journey-details">
                        <span className="flight-journey-arrow"></span>
                    </div>
                    <div className="flight-arrival">
                        <dl className="data-list my-0">
                            <dt className={dtClass}>Arrives</dt>
                            <dd className={ddClass}>
                                <time className="flight-time" dateTime={arrivalDateTime}>
                                    {arrivalDate} <strong>{flightDetails.ArrivalTime}</strong>
                                </time>
                            </dd>
                            <dt className={dtClass}>At</dt>
                            <dd className={ddClass}>
                                <p className="flight-airport">{flightDetails.ArrivalAirport.Name} (
                                    <strong>{flightDetails.ArrivalAirport.IATACode}</strong>)</p>
                            </dd>
                        </dl>
                    </div>
                </div>
            </div>
        );
    }
    renderBaggage() {
        const baggage = this.getBaggage();
        let baggageDisplay = '';
        if (baggage) {
            if (baggage.QuantitySelected > 0) {
                baggageDisplay = `${baggage.QuantitySelected} x ${baggage.Description}`;
            } else {
                baggageDisplay = 'No bags selected';
            }
        }
        return baggageDisplay;
    }
    renderErrata() {
        return (
            <div className="property-errata col-xs-12">
                {this.props.errata.map(this.renderErratum, this)}
            </div>
        );
    }
    renderErratum(erratum, index) {
        return (
            <div key={`errata_${index}`} className="alert alert-info mb-0 mt-3">
                <i className="alert-icon" aria-hidden="true"></i>
                <div className="alert-message">
                    <p>{erratum.Description}</p>
                </div>
            </div>
        );
    }
    render() {
        const component = this.props.component;
        const departureAirport = this.getAirport(component.DepartureAirportId);
        const arrivalAirport = this.getAirport(component.ArrivalAirportId);

        const outboundFlightDetails = component.OutboundFlightDetails;
        const returnFlightDetails = component.ReturnFlightDetails;

        outboundFlightDetails.DepartureAirport = departureAirport;
        outboundFlightDetails.ArrivalAirport = arrivalAirport;

        returnFlightDetails.DepartureAirport = arrivalAirport;
        returnFlightDetails.ArrivalAirport = departureAirport;

        const passengerTotal = component.Adults + component.Children + component.Infants;

        const dtClass = 'flight-label';
        const ddClass = 'flight-detail';

        return (
            <div className="booking-flight-details">
                <dl className="data-list mt-0">
                     <dt className={dtClass}>Passengers</dt>
                    <dd className={ddClass}>{passengerTotal}</dd>
                    <dt className={dtClass}>Baggage</dt>
                    <dd className={ddClass}>{this.renderBaggage()}</dd>
                </dl>
                {this.renderFlightDetails('Outbound', component, outboundFlightDetails)}
                {returnFlightDetails.DepartureTime
                    && this.renderFlightDetails('Return', component, returnFlightDetails)}

                {this.props.errata.length > 0 && this.renderErrata()}
            </div>
        );
    }
}

FlightComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    airports: React.PropTypes.array.isRequired,
    errata: React.PropTypes.array,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,
    cmsBaseUrl: React.PropTypes.string.isRequired,
};
