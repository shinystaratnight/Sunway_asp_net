import 'widgets/bookingjourney/_basket.scss';

import DefaultExtraComponent from 'components/basket/extraComponent';
import DefaultFlightComponent from 'components/basket/flightcomponent';
import DefaultPropertyComponent from 'components/basket/propertycomponent';
import DefaultTransferComponent from 'components/basket/transferComponent';

import Price from 'components/common/price';
import React from 'react';

class Basket extends React.Component {
    constructor(props) {
        super(props);
        this.newComponentTimeout = 1000;
        this.state = {
            newComponentToken: '',
        };
        this.resetNewComponent = this.resetNewComponent.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        const componentCount = this.props.basket.basket.Components.length;
        const newComponentCount = nextProps.basket.basket.Components.length;
        if (newComponentCount > componentCount) {
            this.setNewComponent(nextProps.basket.basket);
        }
    }
    setNewComponent(updatedBasket) {
        const currentBasket = this.props.basket.basket;
        updatedBasket.Components.forEach(component => {
            const existingComponent = currentBasket.Components
                .find(item => item.ComponentToken === component.ComponentToken);
            if (!existingComponent) {
                this.setState({ newComponentToken: component.ComponentToken });
            }
        });
    }
    resetNewComponent() {
        this.setState({ newComponentToken: '' });
    }
    renderFlightComponent(component, index) {
        const flightComponentProps = {
            key: `flight-${index}`,
            component,
            title: this.props.componentTypeTitles.Flight,
            airports: this.props.airports,
            flightCarriers: this.props.flightCarriers,
            flightClasses: this.props.flightClasses,
            cmsBaseUrl: this.props.cmsBaseUrl,
        };
        const FlightComponent
            = this.props.customerComponents.hasOwnProperty('BasketFlightComponent')
                ? this.props.customerComponents.BasketFlightComponent : DefaultFlightComponent;

        return (
            <FlightComponent {...flightComponentProps} />
        );
    }
    renderPropertyComponent(component, index) {
        const basket = this.props.basket.basket;
        const flightComponent = basket.Components
            .find(c => c.ComponentType === 'Flight');
        const propertyComponentProps = {
            key: `property-${index}`,
            component,
            flightComponent,
            title: this.props.componentTypeTitles.Property,
            mealBasis: this.props.mealBasis,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
        };
        const PropertyComponent
            = this.props.customerComponents.hasOwnProperty('BasketPropertyComponent')
                ? this.props.customerComponents.BasketPropertyComponent : DefaultPropertyComponent;

        return (
            <PropertyComponent {...propertyComponentProps} />
        );
    }
    renderTransferComponent(component, index) {
        let containerClass = 'basket-item';
        if (component.ComponentToken === this.state.newComponentToken) {
            containerClass += ' basket-new-item';
            setTimeout(this.resetNewComponent, this.newComponentTimeout);
        }
        const transferComponentProps = {
            key: `transfer-${index}`,
            component,
            title: this.props.componentTypeTitles.Transfer,
            containerClass,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
        };

        const TransferComponent
            = this.props.customerComponents.hasOwnProperty('BasketTransferComponent')
                ? this.props.customerComponents.BasketTransferComponent : DefaultTransferComponent;

        return (
            <TransferComponent {...transferComponentProps} />
        );
    }
    renderExtras(extras) {
        return (
            <div className="basket-item">
                <h4 className="basket-item-header">{this.props.componentTypeTitles.Extras}</h4>
                {extras.map(this.renderExtraComponent, this)}
            </div>
        );
    }
    renderExtraComponent(component, index) {
        let containerClass = 'basket-sub-item';
        if (component.ComponentToken === this.state.newComponentToken) {
            containerClass += ' basket-new-item';
            setTimeout(this.resetNewComponent, this.newComponentTimeout);
        }

        const extraComponentProps = {
            key: `extra-${index}`,
            component,
            containerClass,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
        };

        const ExtraComponent
            = this.props.customerComponents.hasOwnProperty('BasketExtraComponent')
                ? this.props.customerComponents.BasketExtraComponent : DefaultExtraComponent;

        return (
            <ExtraComponent {...extraComponentProps} />
        );
    }
    renderAdjustments() {
        const adjustments = this.props.basket.basket.Adjustments;
        return (
            <div className="basket-item">
                 <h5 className="basket-item-header">Adjustments:</h5>
                <dl className="data-list row mb-0">
                    {adjustments.map(this.renderAdjustment, this)}
                </dl>
            </div>
        );
    }
    renderAdjustment(adjustment) {
        const adjustmentContent = [];
        const dtClass = 'col-xs-5 font-weight-bold';
        const ddClass = 'col-xs-7';

        adjustmentContent.push(
            <dt className={dtClass}>{adjustment.AdjustmentType}</dt>
        );

        const adjustmentPriceProps = {
            amount: adjustment.AdjustmentAmount,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            displayTotalGroupPrice: false,
            classes: 'basket-subtotal text-right',
        };

        adjustmentContent.push(
            <dd className={ddClass}><Price {...adjustmentPriceProps} /></dd>
        );

        return adjustmentContent;
    }
    renderPromocode() {
        const basket = this.props.basket.basket;
        const dtClass = 'col-xs-5 font-weight-bold';
        const ddClass = 'col-xs-7';
        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        pricingConfiguration.PerPersonPricing = false;

        const promoPriceProps = {
            amount: basket.PromoCodeDiscount,
            currency: this.props.currency,
            pricingConfiguration,
            displayTotalGroupPrice: false,
            classes: 'basket-subtotal text-right',
            prependText: '-',
        };

        return (
            <div className="basket-item">
                             <h5 className="basket-item-header">Promocode:</h5>
                <dl className="data-list row mb-0">
                    <dt className={dtClass}>{basket.PromoCode}</dt>
                    <dd className={ddClass}><Price {...promoPriceProps} /></dd>
                </dl>
            </div>
        );
    }
    render() {
        const basket = this.props.basket.basket;
        const flights = basket.Components
             .filter(component => component.ComponentType === 'Flight');

        const properties = basket.Components
            .filter(component => component.ComponentType === 'Hotel');

        const transfers = basket.Components
            .filter(component => component.ComponentType === 'Transfer');

        const extras = basket.Components
            .filter(component => component.ComponentType === 'Extra');

        const totalPriceProps = {
            amount: basket.TotalPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: this.props.totalText,
            displayTotalGroupPrice: false,
            forceTotalGroupPrice: true,
            classes: 'basket-total',
        };

        return (
            <div className="basket-content panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </header>
                <div className="panel-body">
                    {flights.map(this.renderFlightComponent, this)}
                    {properties.map(this.renderPropertyComponent, this)}
                    {transfers.map(this.renderTransferComponent, this)}
                    {extras.length > 0
                        && this.renderExtras(extras)}
                    {!this.props.hideAdjustments
                        && basket.Adjustments.length > 0
                        && this.renderAdjustments()}
                    {basket.PromoCodeDiscount > 0
                        && this.renderPromocode()}
                 </div>
                <footer className="panel-footer">
                    <Price {...totalPriceProps} />
                </footer>
            </div>
        );
    }
}

Basket.propTypes = {
    basket: React.PropTypes.object.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    title: React.PropTypes.string.isRequired,
    componentTypeTitles: React.PropTypes.shape({
        Flight: React.PropTypes.string,
        Property: React.PropTypes.string,
        Transfer: React.PropTypes.string,
        Extras: React.PropTypes.string,
    }).isRequired,
    totalText: React.PropTypes.string.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    cmsBaseUrl: React.PropTypes.string.isRequired,
    customerComponents: React.PropTypes.object,
    hideAdjustments: React.PropTypes.bool,
};

export default Basket;
