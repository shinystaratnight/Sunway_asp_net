import 'widgets/bookingjourney/_confirmation.scss';

import ExtraComponent from 'components/booking/extracomponent';
import FlightComponent from 'components/booking/flightcomponent';
import Price from 'components/common/price';
import PropertyComponent from 'components/booking/propertycomponent';
import React from 'react';
import TransferComponent from 'components/booking/transfercomponent';

import moment from 'moment';

export default class Confirmation extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderHeader() {
        const bookingDetails = this.props.bookingDetails;
        const title = this.props.title.replace('##reference##', bookingDetails.Reference);
        return (
            <div className="container">
                  <div className="panel panel-basic">
                    <header className="panel-header">
                        <h2 className="h-tertiary">{title}</h2>
                    </header>
                    <div className="panel-body">
                        <p>{this.props.message}</p>
                    </div>
                </div>
            </div>
        );
    }
    renderBookingSummary() {
        const bookingSummary = this.props.bookingSummary;
        const bookingDetails = this.props.bookingDetails;

        const flights = bookingDetails.Components
             .filter(component => component.ComponentType === 'Flight');

        const properties = bookingDetails.Components
            .filter(component => component.ComponentType === 'Hotel');

        const transfers = bookingDetails.Components
            .filter(component => component.ComponentType === 'Transfer');

        const extras = bookingDetails.Components
            .filter(component => component.ComponentType === 'Extra');

        const printButtonProps = {
            className: 'btn btn-default hidden-xs',
            onClick: () => {
                window.print();
            },
        };

        return (
            <div className="confirmation-booking-summary">
                <div className="container">
                    <div className="row mb-2">
                        <div className="col-xs-12 col-sm-6">
                            <h3 className="h-tertiary btn-text-md">{bookingSummary.title}</h3>
                        </div>
                        <div className="booking-options col-xs-12 col-sm-6 text-sm-right">
                            <button {...printButtonProps}>
                                <i className="print-icon" aria-hidden="true"></i>
                                Print
                            </button>
                        </div>
                    </div>
                </div>
                {flights.map(this.renderFlightComponent, this)}
                {properties.map(this.renderPropertyComponent, this)}
                {transfers.map(this.renderTransferComponent, this)}
                {extras.length > 0
                    && this.renderExtras(extras)}
                {this.renderPaymentBreakdown()}
            </div>
        );
    }
    renderFlightComponent(flight, index) {
        const key = `flight-${index}`;
        const bookingSummary = this.props.bookingSummary;
        const bookingDetails = this.props.bookingDetails;

        const flightErrata = bookingDetails.Errata
            .filter(component => component.ComponentType === 'Flight');

        const flightComponentProps = {
            component: flight,
            airports: this.props.lookups.airports,
            errata: flightErrata,
            flightCarriers: this.props.lookups.flightCarriers,
            flightClasses: this.props.lookups.flightClasses,
            cmsBaseUrl: this.props.cmsBaseUrl,
        };

        return (
            <div className="container" key={key}>
                 <div className="panel panel-basic">
                    <header className="panel-header">
                        <h2 className="h-tertiary">{bookingSummary.componentTypeTitles.flight}</h2>
                    </header>
                    <div className="panel-body">
                        <FlightComponent {...flightComponentProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderPropertyComponent(property, index) {
        const bookingSummary = this.props.bookingSummary;
        const bookingDetails = this.props.bookingDetails;

        const propertyErrata = bookingDetails.Errata
            .filter(component => component.ComponentType === 'Hotel');

        const propertyComponentProps = {
            component: property,
            errata: propertyErrata,
            mealBasis: this.props.lookups.mealBasis,
        };

        const key = `property-${index}`;
        return (
            <div className="container" key={key}>
                <div className="panel panel-basic">
                    <header className="panel-header">
                        <h2 className="h-tertiary">
                            {bookingSummary.componentTypeTitles.property}</h2>
                    </header>
                    <div className="panel-body">
                        <PropertyComponent {...propertyComponentProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderTransferComponent(transfer, index) {
        const bookingSummary = this.props.bookingSummary;
        const transferProps = {
            component: transfer,
        };
        const key = `transfer-${index}`;
        return (
            <div className="container" key={key}>
                <div className="panel panel-basic">
                    <header className="panel-header">
                        <h2 className="h-tertiary">
                            {bookingSummary.componentTypeTitles.transfer}</h2>
                    </header>
                    <div className="panel-body">
                        <TransferComponent {...transferProps} />
                    </div>
                </div>
            </div>
        );
    }
    renderExtras(extras) {
        const bookingSummary = this.props.bookingSummary;
        return (
            <div className="container">
                <div className="panel panel-basic">
                    <header className="panel-header">
                        <h2 className="h-tertiary">
                            {bookingSummary.componentTypeTitles.extras}</h2>
                    </header>
                    <div className="panel-body">
                        {extras.map(this.renderExtraComponent, this)}
                    </div>
                </div>
            </div>
        );
    }
    renderExtraComponent(extra, index) {
        const key = `extra-${index}`;
        const extraProps = {
            key,
            component: extra,
        };
        return (
            <ExtraComponent {...extraProps} />
        );
    }
    getPriceProps() {
        const pricingConfigurationOverrides = {
            PerPersonPricing: false,
            PriceFormatDisplay: 'TwoDP',
        };
        const pricingConfiguration = Object.assign({},
            this.props.pricingConfiguration, pricingConfigurationOverrides);
        const priceProps = {
            currency: this.props.currency,
            displayPerPersonText: false,
            displayTotalGroupPrice: false,
            pricingConfiguration,
        };
        return priceProps;
    }
    renderPaymentBreakdown() {
        const bookingDetails = this.props.bookingDetails;
        const priceProps = this.getPriceProps();
        const paymentsDue = bookingDetails.CustomerPayments.filter(p => p.Outstanding > 0);
        return (
            <div className="container">
                <div className="row">
                    <div className="col-xs-12 col-sm-8 col-md-6 col-lg-5 float-right">
                        <div className="booking-payment-details mb-3">
                            <dl className="data-list row">
                                <dt className="total-price col-xs-8 font-weight-bold">
                                    Total Price</dt>
                                <dd className="total-price col-xs-4 text-right font-weight-bold">
                                    <Price {...priceProps} amount={bookingDetails.TotalPrice} />
                                </dd>
                                <dt className="col-xs-8">Amount Paid</dt>
                                <dd className="col-xs-4 text-right">
                                    <Price {...priceProps} amount={bookingDetails.TotalPaid} />
                                </dd>
                                {paymentsDue.map(this.renderPayment, this)}
                            </dl>
                         </div>
                    </div>
                </div>
            </div>
        );
    }
    renderPayment(payment) {
        const priceProps = this.getPriceProps();

        const paymentDate = moment(payment.DateDue);
        const label = `Amount due by ${paymentDate.format('ll')}`;

        const paymentDisplay = [];
        paymentDisplay.push(
            <dt className="col-xs-8">{label}</dt>
        );
        paymentDisplay.push(
            <dd className="col-xs-4 text-right">
                <Price {...priceProps} amount={payment.Outstanding} />
            </dd>
        );

        return paymentDisplay;
    }
    renderDisclaimer() {
        return (
            <div className="container">
                <div className="panel panel-basic">
                    <div className="panel-body">
                        <p>{this.props.disclaimer}</p>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        return (
            <div className="confirmation-content">
                {this.renderHeader()}
                {this.renderBookingSummary()}
                {this.props.disclaimer
                    && this.renderDisclaimer()}
            </div>
        );
    }
}

Confirmation.propTypes = {
    bookingDetails: React.PropTypes.object.isRequired,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string,
    bookingSummary: React.PropTypes.shape({
        title: React.PropTypes.string,
        componentTypeTitles: React.PropTypes.shape({
            flight: React.PropTypes.string,
            property: React.PropTypes.string,
            transfer: React.PropTypes.string,
            extras: React.PropTypes.string,
        }),
    }),
    disclaimer: React.PropTypes.string,
    pricingConfiguration: React.PropTypes.object.isRequired,
    currency: React.PropTypes.object.isRequired,
    lookups: React.PropTypes.shape({
        mealBasis: React.PropTypes.array.isRequired,
        airports: React.PropTypes.array.isRequired,
        flightCarriers: React.PropTypes.array.isRequired,
        flightClasses: React.PropTypes.array.isRequired,
    }).isRequired,
    cmsBaseUrl: React.PropTypes.string.isRequired,
};
