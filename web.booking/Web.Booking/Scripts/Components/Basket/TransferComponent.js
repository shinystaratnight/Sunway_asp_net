import Price from 'components/common/price';
import React from 'react';
import moment from 'moment';

export default class TransferComponent extends React.Component {
    render() {
        const component = this.props.component;
        const dtClass = 'col-xs-5 font-weight-bold';
        const ddClass = 'col-xs-7';

        const subTotalProps = {
            amount: component.TotalPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: 'Sub Total',
            classes: 'basket-subtotal text-right font-weight-bold',
            displayTotalGroupPrice: false,
        };

        const passengerTotal = component.Adults + component.Children + component.Infants;

        const outboundJourney = component.OutboundJourneyDetails;
        const returnJourney = component.ReturnJourneyDetails;

        const departureDate = moment(outboundJourney.Date).format('ll');
        const returnDate = moment(returnJourney.Date).format('ll');

        return (
            <div className={this.props.containerClass}>
                <h4 className="basket-item-header">{this.props.title}</h4>
                <dl className="data-list row">
                    <dt className={dtClass}>Type</dt>
                    <dd className={ddClass}>{component.Vehicle}</dd>
                     <dt className={dtClass}>Passengers</dt>
                    <dd className={ddClass}>{passengerTotal}</dd>
                </dl>

                <h5 className="header-outbound">Outbound</h5>
                <p>{component.DepartureParentName} to {component.ArrivalParentName}</p>
                <dl className="data-list row mt-0">
                    <dt className={dtClass}>Date</dt>
                    <dd className={ddClass}>{departureDate}</dd>
                    <dt className={dtClass}>Flight</dt>
                    <dd className={ddClass}>
                        {outboundJourney.FlightCode}, Arriving {outboundJourney.Time}</dd>
                    {outboundJourney.JourneyTime
                        && parseInt(outboundJourney.JourneyTime, 10) !== 0
                        && <dt className={dtClass}>Journey Time</dt>}
                    {outboundJourney.JourneyTime
                        && parseInt(outboundJourney.JourneyTime, 10) !== 0
                        && <dd className={ddClass}>{outboundJourney.JourneyTime} mins</dd>}
                </dl>

                <h5 className="header-return">Return</h5>
                <p>{component.ArrivalParentName} to {component.DepartureParentName}</p>
                <dl className="data-list row mt-0">
                    <dt className={dtClass}>Date</dt>
                    <dd className={ddClass}>{returnDate}</dd>
                    <dt className={dtClass}>Flight</dt>
                    <dd className={ddClass}>
                        {returnJourney.FlightCode}, Departing {returnJourney.Time}</dd>
                    {returnJourney.JourneyTime
                        && parseInt(returnJourney.JourneyTime, 10) !== 0
                        && <dt className={dtClass}>Journey Time</dt>}
                    {returnJourney.JourneyTime
                        && parseInt(returnJourney.JourneyTime, 10) !== 0
                        && <dd className={ddClass}>{returnJourney.JourneyTime} mins</dd>}
                </dl>

                <Price {...subTotalProps} />
            </div>
        );
    }
}

TransferComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    title: React.PropTypes.string.isRequired,
    containerClass: React.PropTypes.string.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
};
