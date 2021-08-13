import DirectDebitListItem from '../../components/mmb/directdebitlistitem';
import Price from '../../components/common/price';
import React from 'react';

export default class DirectDebitList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderListItem(directDebit, index) {
        const key = `bookinglistitem-${index}`;
        const brand = this.props.brands.filter(b => b.Id === directDebit.BrandId);
        const brandName = brand[0] ? brand[0].Name : '';
        const pricingConfiguration = this.props.pricingConfiguration;
        pricingConfiguration.PriceFormatDisplay = 'TwoDP';

        const props = {
            brandName,
            directDebit,
            pricingConfiguration,
            selectCurrency: this.props.selectCurrency,
        };
        return (
            <DirectDebitListItem key={key} {...props} />
        );
    }
    renderHeaderRow() {
        return (
            <div className="table-row">
                <div className="table-header">
                    Reference
                </div>
                <div className="table-header">
                    Lead Name
                </div>
                <div className="table-header">
                    Brand
                </div>
                <div className="table-header">
                    Departs
                </div>
                <div className="table-header">
                    Amount Due
                </div>
            </div>
        );
    }
    renderFooterRow() {
        let amount = 0;
        this.props.directDebits.forEach(dd => {
            amount += dd.AmountDue;
        });
        const pricingConfiguration = this.props.pricingConfiguration;
        pricingConfiguration.PriceFormatDisplay = 'TwoDP';

        const priceProps = {
            amount,
            pricingConfiguration,
            currency: this.props.selectCurrency,
            classes: 'inline',
        };
        return (
            <div className="direct-debit-list-item table-row">
                <div className="table-cell"></div>
                <div className="table-cell"></div>
                <div className="table-cell"></div>
                <div className="table-cell"></div>
                <div className="table-cell font-weight-bold">
                    Total <Price {...priceProps} />
                </div>
            </div>
        );
    }
    render() {
        return (
            <div className="table">
                {this.renderHeaderRow()}
                {this.props.directDebits.map(this.renderListItem, this)}
                {this.renderFooterRow()}
            </div>
        );
    }
}

DirectDebitList.propTypes = {
    directDebits: React.PropTypes.array,
    brands: React.PropTypes.array,
    pricingConfiguration: React.PropTypes.object,
    selectCurrency: React.PropTypes.object,
};
