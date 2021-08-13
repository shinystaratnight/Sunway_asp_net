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
    getSelectedBaggage() {
        return this.props.component.SubComponents.filter(subComponent =>
            subComponent.ExtraType === 'Baggage'
                && subComponent.QuantitySelected > 0);
    }
    getSelectedExtras() {
        return this.props.component.SubComponents.filter(subComponent =>
            subComponent.ExtraType !== 'Baggage'
            && subComponent.QuantitySelected > 0);
    }
    renderFlightDetails(title, component, flightDetails) {
        const dtClass = 'col-xs-4 font-weight-bold';
        const ddClass = 'col-xs-8';

        const departureDate = moment(flightDetails.DepartureDate).format('ll');
        const arrivalDate = moment(flightDetails.ArrivalDate).format('ll');
        const flightClass = this.getFlightClass(flightDetails.FlightClassId);

        const flightCarrierId = flightDetails.OperatingFlightCarrierId > 0
            ? flightDetails.OperatingFlightCarrierId
            : component.FlightCarrierId;
        const flightCarrier = this.getFlightCarrier(flightCarrierId);
        const flightCarrierLogo = `${this.props.cmsBaseUrl}${flightCarrier.Logo}`;

        return (
            <div className="basket-flight-detail">
                <h5>{title}</h5>

                <dl className="data-list row">
                    <dt className={dtClass}>Carrier</dt>
                    <dd className={ddClass}>
                        <img className="flight-carrier-logo"
                            src={flightCarrierLogo}
                            alt={flightCarrier.Name} />
                    </dd>

                    <dt className={dtClass}>Code</dt>
                    <dd className={ddClass}>{flightDetails.FlightCode}</dd>

                    <dt className={dtClass}>Class</dt>
                    <dd className={ddClass}>{flightClass.Name}</dd>

                    <dt className={dtClass}>From</dt>
                    <dd className={ddClass}>{flightDetails.DepartureAirport.Name}</dd>
                    <dt className={dtClass}></dt>
                    <dd className={ddClass}>{departureDate} {flightDetails.DepartureTime}</dd>

                    <dt className={dtClass}>To</dt>
                    <dd className={ddClass}>{flightDetails.ArrivalAirport.Name}</dd>
                    <dt className={dtClass}></dt>
                    <dd className={ddClass}>{arrivalDate} {flightDetails.ArrivalTime}</dd>
                </dl>
            </div>
        );
    }
    renderBaggageItem(baggage, index) {
        const baggageDisplay = `${baggage.QuantitySelected} x ${baggage.Description}`;
        return (
            <p key={`baggage_${index}`}>{baggageDisplay}</p>
        );
    }
    renderIncludedBaggageItem(component) {
        return (
            <p key={`baggage_${0}`}>{component.IncludedBaggageText}</p>
        );
    }
    renderExtraItem(extras, index) {
        const extraDisplay = `${extras.QuantitySelected} x ${extras.Description}`;
        return (
            <p key={`extra_${index}`}>{extraDisplay}</p>
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

        const dtClass = 'col-xs-4 font-weight-bold';
        const ddClass = 'col-xs-8';

        const baggage = this.getSelectedBaggage();
        const extras = this.getSelectedExtras();

        return (
            <div className="basket-item">
                <h5 className="basket-item-header">{this.props.title}</h5>
                <dl className="data-list row">
                     <dt className={dtClass}>Passengers</dt>
                    <dd className={ddClass}>{passengerTotal}</dd>
                    <dt className={dtClass}>Baggage</dt>
                    <dd className={ddClass}>
                        {baggage.length === 0 && !component.IncludesSupplierBaggage
                            && <p>No bags selected</p>}
                        {baggage.map(this.renderBaggageItem, this)}
                        {component.IncludesSupplierBaggage
                            && this.renderIncludedBaggageItem(component)}
                    </dd>
                    {extras.length !== 0 && <dt className={dtClass}>Extra</dt>}
                    {extras.length !== 0
                        && <dd className={ddClass}>{extras.map(this.renderExtraItem, this)}</dd>}
                </dl>
                {this.renderFlightDetails('Outbound', component, outboundFlightDetails)}
                {this.renderFlightDetails('Return', component, returnFlightDetails)}
            </div>
        );
    }
}

FlightComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    title: React.PropTypes.string.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,
    cmsBaseUrl: React.PropTypes.string.isRequired,
};
