import React from 'react';

import moment from 'moment';

export default class PaymentBreakdown extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    formatCurrency(amount) {
        const currencySymbol = this.props.currency.CustomerSymbolOverride
            ? this.props.currency.CustomerSymbolOverride : this.props.currency.Symbol;
        let formattedAmount = '';
        if (this.props.currency.SymbolPosition === 'Prepend') {
            formattedAmount = `${currencySymbol}${amount.toLocaleString()}`;
        } else {
            formattedAmount = `${amount.toLocaleString()}${currencySymbol}`;
        }
        return formattedAmount;
    }
    renderPayment(payment, index) {
        const dateDueMoment = moment(payment.DateDue);
        return (
            <tr key={`payment-${index}`}>
                <td className="date">
                    {dateDueMoment.format('ll')}
                </td>
                <td className="price">
                    {this.formatCurrency(payment.Amount)}
                </td>
            </tr>
        );
    }
    render() {
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.message !== ''
                        && <p className="mb-2">{this.props.message}</p>}
                    <table>
                        <tbody>
                            {this.props.paymentScheduleTitle
                                &&	<tr>
                                        <td>{this.props.paymentScheduleTitle}</td>
                                    </tr>}
                            {this.props.payments
                                && this.props.payments.map(this.renderPayment, this)}
                        </tbody>
                    </table>
                </div>
            </div>
        );
    }
}

PaymentBreakdown.propTypes = {
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    paymentScheduleTitle: React.PropTypes.string.isRequired,
    payments: React.PropTypes.array,
    currency: React.PropTypes.object,
};

PaymentBreakdown.defaultProps = {
};
