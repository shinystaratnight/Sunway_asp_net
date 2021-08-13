import 'components/booking/_transferComponent.scss';

import React from 'react';
import moment from 'moment';

export default class TransferComponent extends React.Component {
    render() {
        const component = this.props.component;
        const passengerTotal = component.Adults + component.Children + component.Infants;
        const dtClass = 'transfer-label font-weight-bold';
        const ddClass = 'transfer-detail';
        const outboundJourney = component.OutboundJourneyDetails;
        const returnJourney = component.ReturnJourneyDetails;

        const departureDate = moment(outboundJourney.Date).format('ll');
        const returnDate = moment(returnJourney.Date).format('ll');

        return (
            <div className="booking-transfer-details row">
                <div className="transfer-vehicle col-xs-12 col-md-4 mb-3">
                    <h4>Vehicle</h4>
                    <p>{component.Vehicle}</p>
                    <p>{passengerTotal} passengers</p>
                </div>
                <div className="col-xs-12 col-md-4">
                    <h4>Outbound</h4>
                    <dl className="data-list mt-0">
                        <dt className={dtClass}>Date</dt>
                        <dd className={ddClass}>{departureDate}</dd>
                        <dt className={dtClass}>Pick up</dt>
                        <dd className={ddClass}>{component.DepartureParentName}</dd>
                        <dt className={dtClass}>Flight</dt>
                        <dd className={ddClass}>
                            {outboundJourney.FlightCode}, Arriving {outboundJourney.Time}</dd>
                        <dt className={dtClass}>Drop off</dt>
                        <dd className={ddClass}>{component.ArrivalParentName}</dd>
                    </dl>
                </div>
                <div className="col-xs-12 col-md-4">
                    <h4>Return</h4>
                    <dl className="data-list mt-0">
                        <dt className={dtClass}>Date</dt>
                        <dd className={ddClass}>{returnDate}</dd>
                        <dt className={dtClass}>Pick up</dt>
                        <dd className={ddClass}>{component.ArrivalParentName}</dd>
                        <dt className={dtClass}>Drop off</dt>
                        <dd className={ddClass}>{component.DepartureParentName}</dd>
                        <dt className={dtClass}>Flight</dt>
                        <dd className={ddClass}>
                            {returnJourney.FlightCode}, Departing {returnJourney.Time}</dd>
                    </dl>
                </div>
            </div>
        );
    }
}

TransferComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
};
