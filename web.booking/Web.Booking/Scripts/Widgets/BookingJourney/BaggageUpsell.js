import 'widgets/bookingjourney/_baggageUpsell.scss';

import Price from '../../components/common/price';
import React from 'react';

import SelectInput from '../../components/form/selectinput';

export default class BaggageUpsell extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
        this.renderBaggageComponent = this.renderBaggageComponent.bind(this);
    }
    /* eslint-disable no-param-reassign */
    componentDidMount() {
        this.props.baggageComponents.forEach(bc => {
            if (bc.TotalPrice === 0) {
                this.performBaggageUpdate(
                    this.props.flight.Adults + this.props.flight.Children, bc);
            }
        });
    }
    /* eslint-enable no-param-reassign */
    getTotalBaggageSelected(baggageComponent) {
        let totalSelected = 0;

        this.props.baggageComponents.forEach(bc => {
            if (bc.ExtraType === 'Baggage' && (!this.isMixAndMatchFlight()
                || bc.ReturnMultiCarrierExtra === baggageComponent.ReturnMultiCarrierExtra)) {
                totalSelected += parseInt(bc.QuantitySelected, 10);
            }
        });
        return parseInt(totalSelected, 10);
    }
    getBaggageComponentInfo(baggageComponent) {
        const bc = baggageComponent;
        const costingBasis = bc.CostingBasis;

        const minimumBaggage = 0;
        let defaultMaximumBaggage = bc.QuantityAvailable;
        const pricePerItem = parseFloat(bc.TotalPrice);
        const selectedQuantity = parseInt(bc.QuantitySelected, 10);

        const passengerKeys = ['Adults', 'Children', 'Infants'];
        let totalPassengers = 0;
        passengerKeys.forEach(key => {
            if (this.props.flight[key]) {
                totalPassengers += parseInt(this.props.flight[key], 10);
            }
        });
        let totalBaggageAvailable = totalPassengers;
        const totalBaggageSelected = this.getTotalBaggageSelected(baggageComponent);

        switch (costingBasis) {
            case 'Per Passenger':
                defaultMaximumBaggage = defaultMaximumBaggage > totalPassengers
                    ? totalPassengers : defaultMaximumBaggage;
                break;
            case 'Per Booking':
                defaultMaximumBaggage = 1;
                totalBaggageAvailable = 1;
                break;
            case 'None': {
                totalBaggageAvailable = 0;
                const baggageComponents = this.props.baggageComponents;
                baggageComponents.forEach(sbc => {
                    totalBaggageAvailable += parseInt(sbc.QuantityAvailable, 10);
                });
                break;
            }
            default:
        }

        const remainingTotalAllowance = totalBaggageAvailable - totalBaggageSelected;
        let selectableBaggage = totalBaggageAvailable;
        if (baggageComponent.ExtraType === 'Baggage') {
            selectableBaggage = selectedQuantity + remainingTotalAllowance;
            selectableBaggage
                = selectableBaggage > defaultMaximumBaggage
                    ? defaultMaximumBaggage : selectableBaggage;
        }
        const isDisabled
          = baggageComponent.ExtraType === 'Baggage' ? selectableBaggage <= minimumBaggage : false;
        const totalComponentPrice = selectedQuantity * pricePerItem;
        const info = {
            description: bc.Description,
            minimumBaggage,
            maximumBaggage: selectableBaggage,
            pricePerItem,
            selectedQuantity,
            isDisabled,
            totalComponentPrice,
        };
        return info;
    }
    isMixAndMatchFlight() {
        const isMixAndMatchFlight = this.props.flight.ReturnMultiCarrierDetails
            && this.props.flight.ReturnMultiCarrierDetails.BookingToken;
        return isMixAndMatchFlight;
    }
    handleBaggageUpdate(event, baggageComponent) {
        const quantity = event.target.value;
        this.performBaggageUpdate(quantity, baggageComponent);
    }
    performBaggageUpdate(quantity, baggageComponent) {
        const componentToken = this.props.flight.ComponentToken;
        const subComponentToken = baggageComponent.ComponentToken;
        this.props.handleBaggageUpdate(componentToken, subComponentToken, quantity);
    }
    renderBaggageDetails(baggageComponents) {
        return (
            <div className="table mt-2">
                <div className="table-row">
                    <div className="table-header baggage-description">Description</div>
                    <div className="table-header baggage-quantity">Quantity</div>
                    <div className="table-header baggage-price text-right hidden-xs">
                        Price Per Item
                        </div>
                    <div className="table-header baggage-total text-right">Total</div>
                </div>
                {baggageComponents.map(this.renderBaggageComponent)}
            </div>
        );
    }
    renderMixAndMatchBaggageDetails() {
        const outboundBaggageComponents = this.props.baggageComponents.filter(bc =>
            !bc.ReturnMultiCarrierExtra);
        const returnBaggageComponents = this.props.baggageComponents.filter(bc =>
            bc.ReturnMultiCarrierExtra);

        let outboundTitle = 'Outbound';
        const outboundFlightDetails = this.props.flight.OutboundFlightDetails;
        if (outboundFlightDetails && outboundFlightDetails.DepartureAirport) {
            outboundTitle += ` - ${outboundFlightDetails.DepartureAirport.Name}`;
            outboundTitle += ` (${outboundFlightDetails.DepartureAirport.IATACode})`;
            outboundTitle += ` to ${outboundFlightDetails.ArrivalAirport.Name}`;
            outboundTitle += ` (${outboundFlightDetails.ArrivalAirport.IATACode})`;
        }

        let returnTitle = 'Return';
        const returnFlightDetails = this.props.flight.ReturnFlightDetails;
        if (returnFlightDetails && returnFlightDetails.DepartureAirport) {
            returnTitle += ` - ${returnFlightDetails.DepartureAirport.Name}`;
            returnTitle += ` (${returnFlightDetails.DepartureAirport.IATACode})`;
            returnTitle += ` to ${returnFlightDetails.ArrivalAirport.Name}`;
            returnTitle += ` (${returnFlightDetails.ArrivalAirport.IATACode})`;
        }

        return (
            <div className="baggage-mix-and-match mt-2">
                <p className="font-weight-bold">{outboundTitle}</p>
                {this.renderBaggageDetails(outboundBaggageComponents)}
                <p className="mt-2 font-weight-bold">{returnTitle}</p>
                {this.renderBaggageDetails(returnBaggageComponents)}
            </div>
        );
    }
    renderBaggageComponent(baggageComponent, index) {
        const key = `baggage-component-row-${baggageComponent.Description}-${index}`;
        const baggageInfo = this.getBaggageComponentInfo(baggageComponent);
        const selectInputProps = {
            name: 'baggage-quantity',
            optionsRangeMin: baggageInfo.minimumBaggage,
            optionsRangeMax: baggageInfo.maximumBaggage,
            renderOptionGroups: false,
            value: baggageInfo.selectedQuantity,
            onChange: (event) => this.handleBaggageUpdate(event, baggageComponent),
            disabled: baggageInfo.isDisabled,
        };
        const totalRawCost = baggageInfo.totalComponentPrice;
        const currency = this.props.selectedCurrency;
        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        const priceProps = {
            currency,
            pricingConfiguration,
            prependText: '',
            appendText: '',
            classes: '',
            displayTotalGroupPrice: false,
            displayPerPersonText: false,
        };
        const perItemPriceProps = Object.assign({ amount: baggageInfo.pricePerItem }, priceProps);

        const totalPriceProps = Object.assign({
            amount: totalRawCost,
            forceTotalGroupPrice: true,
        }, priceProps);
        totalPriceProps.pricingConfiguration.PerPersonPricing = false;
        totalPriceProps.pricingConfiguration.PriceFormatDisplay = 'TwoDP';
        const perItemPriceMobileProps
            = Object.assign({
                amount: baggageInfo.pricePerItem,
                forceTotalGroupPrice: false,
            }, priceProps);
        perItemPriceMobileProps.prependText = '(';
        perItemPriceMobileProps.appendText = ' per item)';
        perItemPriceMobileProps.classes = 'hidden-sm-up';

        return (
            <div key={key} className="table-row baggage-option">
                <div className="table-cell baggage-description">
                    <p>{baggageInfo.description}</p>
                </div>
                <div className="table-cell baggage-quantity">
                    <SelectInput {...selectInputProps} />
                </div>
                <div className="table-cell baggage-price text-right hidden-xs">
                    <Price {...perItemPriceProps} />
                </div>
                <div className="table-cell baggage-total text-right">
                    <Price {...totalPriceProps} />
                    <Price {...perItemPriceMobileProps} />
                </div>
            </div>
        );
    }
    render() {
        return (
            <div className="widget-baggage-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </div>
                <div className="baggage-upsell-details panel-body">
                    <p>{this.props.leadParagraph}</p>
                    {this.isMixAndMatchFlight()
                        && this.renderMixAndMatchBaggageDetails()}
                    {!this.isMixAndMatchFlight()
                        && this.renderBaggageDetails(this.props.baggageComponents)}
                </div>
            </div>
        );
    }
}

BaggageUpsell.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    flight: React.PropTypes.object,
    baggageComponents: React.PropTypes.array,
    selectedCurrency: React.PropTypes.object,
    pricingConfiguration: React.PropTypes.object,
    handleBaggageUpdate: React.PropTypes.func,
};

BaggageUpsell.defaultProps = {
};
