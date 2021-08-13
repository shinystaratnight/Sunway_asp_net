import Price from 'components/common/price';
import React from 'react';
import moment from 'moment';

export default class PropertyComponent extends React.Component {
    getMealBasis(id) {
        return this.props.mealBasis.find(m => m.Id === id);
    }
    renderPropertyRoom(subComponent, index) {
        const mealBasis = this.getMealBasis(subComponent.MealBasisId);
        let roomDescription = subComponent.RoomType;
        if (subComponent.RoomView) {
            roomDescription += `, ${subComponent.RoomView}`;
        }
        roomDescription += `, ${mealBasis.Name}`;

        return (
            <div key={`room-${index}`} className="basket-sub-component">
                <h5>{`Room ${index + 1}`}</h5>
                <p className="description">{roomDescription}</p>
            </div>
        );
    }
    render() {
        const component = this.props.component;
        const content = component.content ? component.content : { Image: {} };
        const subname = content.Resort && content.Region
            ? `${content.Resort}, ${content.Region}`
            : '';
        const arrivalDate = moment(component.ArrivalDate).format('ll');
        const subTotalProps = {
            amount: component.TotalPrice,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: 'Sub Total',
            classes: 'basket-subtotal text-right font-weight-bold',
            displayTotalGroupPrice: false,
        };
        const dtClass = 'col-xs-5 font-weight-bold';
        const ddClass = 'col-xs-7';

        if (this.props.flightComponent
                && this.props.pricingConfiguration.PackagePrice) {
            subTotalProps.amount += this.props.flightComponent.TotalPrice;
            subTotalProps.prependText = 'Package Price';
        }

        return (
             <div className="basket-item">
                <h5 className="basket-item-header">{this.props.title}</h5>
                <p className="basket-item-name">{component.Name}</p>
                <span
                    data-rating={component.Rating}
                    itemProp="award"
                    className="rating">{component.Rating} Stars</span>
                <p className="basket-item-subname">{subname}</p>
                <dl className="data-list row mt-0 mb-0">
                    <dt className={dtClass}>Arrival Date</dt>
                    <dd className={ddClass}>{arrivalDate}</dd>
                </dl>
                <dl className="data-list row mt-0 mb-1">
                    <dt className={dtClass}>Duration: </dt>
                    <dd className={ddClass}>{this.props.component.Duration} Nights</dd>
                </dl>
                {component.SubComponents.map(this.renderPropertyRoom, this)}
                <Price {...subTotalProps} />
            </div>
        );
    }
}

PropertyComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    title: React.PropTypes.string.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    flightComponent: React.PropTypes.object,
};
