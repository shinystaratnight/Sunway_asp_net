import CheckboxInput from 'components/form/checkboxinput';
import NumberFunctions from 'library/numberfunctions';
import React from 'react';

export default class Deposit extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const basket = this.props.basket;
        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        pricingConfiguration.PerPersonPricing = false;

        const amountDueToday = NumberFunctions.formatPrice(basket.AmountDueToday,
            pricingConfiguration, this.props.currency);
        const totalPrice = NumberFunctions.formatPrice(basket.TotalAmountDue,
            pricingConfiguration, this.props.currency);

        const depositCheckbox = {
            name: 'DepositAmount',
            label: `Secure your booking with a deposit of ${amountDueToday}`,
            onChange: () => this.props.onChange(basket.AmountDueToday),
            value: basket.PaymentDetails.Amount === basket.AmountDueToday,
        };
        const fullAmountCheckbox = {
            name: 'TotalAmount',
            label: `Pay the full amount of ${totalPrice}`,
            onChange: () => this.props.onChange(basket.TotalAmountDue),
            value: basket.PaymentDetails.Amount === basket.TotalAmountDue,
        };
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.message !== ''
                        && <p className="mb-2">{this.props.message}</p>}
                   <CheckboxInput {...depositCheckbox} />
                   <CheckboxInput {...fullAmountCheckbox} />
                </div>
            </div>
        );
    }
}

Deposit.propTypes = {
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    basket: React.PropTypes.object.isRequired,
    onChange: React.PropTypes.func.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    warnings: React.PropTypes.object,
};

Deposit.defaultProps = {
};
