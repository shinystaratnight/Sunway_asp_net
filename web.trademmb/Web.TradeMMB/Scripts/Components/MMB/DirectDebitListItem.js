import Price from '../common/price';

import React from 'react';
import moment from 'moment';

export default class DirectDebitListItem extends React.Component {
    constructor() {
        super();
        this.state = {};
    }
    renderCell(value) {
        return (
            <div key={value} className="table-cell">
                {value}
            </div>
        );
    }
    render() {
        const directDebit = this.props.directDebit;
        const departs = moment(directDebit.DepartureDate).format('DD MMM \'YY');
        const priceProps = {
            amount: directDebit.AmountDue,
            pricingConfiguration: this.props.pricingConfiguration,
            currency: this.props.selectCurrency,
        };
        const amountDue = <Price {...priceProps} />;
        const cells = {
            reference: directDebit.BookingReference,
            leadName: directDebit.LeadGuestLastName,
            brandName: this.props.brandName,
            departs,
            amountDue,
        };
        const content = [];
        Object.keys(cells).forEach(k => {
            const contentItem = this.renderCell(cells[k]);
            content.push(contentItem);
        });
        return (
            <div className="direct-debit-list-item table-row">
                {content}
            </div>
        );
    }
}

DirectDebitListItem.propTypes = {
    brandName: React.PropTypes.string,
    directDebit: React.PropTypes.object,
    pricingConfiguration: React.PropTypes.object,
    selectCurrency: React.PropTypes.object,
};
