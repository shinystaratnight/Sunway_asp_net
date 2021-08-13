import Price from 'components/common/price';
import React from 'react';
import StringFunctions from 'library/stringfunctions';
import moment from 'moment';

export default class ExtraComponent extends React.Component {
    getPaxSummary(adults, children, infants) {
        let paxSummary = StringFunctions.getPluralisationValue(adults, 'Adult', 'Adults');
        paxSummary += children > 0
            ? ` ${StringFunctions.getPluralisationValue(children, 'Child', 'Children')}` : '';
        paxSummary += infants > 0
            ? ` ${StringFunctions.getPluralisationValue(infants, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    render() {
        const component = this.props.component;

        const startDate = moment(component.ArrivalDate).format('ll');
        const expiryDate = moment(component.ArrivalDate).add(component.Duration, 'd').format('ll');
        const pax = this.getPaxSummary(component.Adults, component.Children, component.Infants);

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

        return (
            <div className={this.props.containerClass}>
                <h5 className="basket-sub-item-header">{component.ExtraName}</h5>
                <dl className="data-list row mt-0">
                    <dt className={dtClass}>{component.Duration > 0 ? 'Start Date' : 'Date'}</dt>
                    <dd className={ddClass}>{startDate}</dd>
                    {component.Duration > 0
                        && <dt className={dtClass}>Expiry Date</dt>}
                    {component.Duration > 0
                        && <dd className={ddClass}>{expiryDate}</dd>}
                    <dt className={dtClass}>Guests</dt>
                    <dd className={ddClass}>{pax}</dd>
                </dl>
                <Price {...subTotalProps} />
            </div>
        );
    }
}

ExtraComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    containerClass: React.PropTypes.string.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
};
