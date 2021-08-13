import React from 'react';

export default class Price extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    formatPrice(price) {
        const parts = price.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
    render() {
        const pricingConfiguration = this.props.pricingConfiguration;
        let amount = (pricingConfiguration.PriceFormatDisplay === 'Rounded')
            ? Math.ceil(this.props.amount.toFixed(0))
            : this.props.amount.toFixed(2);

        amount = pricingConfiguration.ShowGroupSeperator
            ? this.formatPrice(amount)
            : amount;

        const symbol = this.props.currency.CustomerSymbolOverride
            ? this.props.currency.CustomerSymbolOverride
            : this.props.currency.Symbol;

        let formattedAmount = '';
        if (this.props.currency.SymbolPosition === 'Prepend') {
            formattedAmount = `${symbol}${amount}`;
        } else {
            formattedAmount = `${amount}${symbol}`;
        }

        let classes = 'price';
        if (this.props.classes) {
            classes = this.props.classes;
        }
        return (
            <p className={classes}>
                {this.props.prependText
                && this.props.prependText}
                {formattedAmount}
                {this.props.appendText
                && this.props.appendText}</p>
        );
    }
}

Price.propTypes = {
    amount: React.PropTypes.number,
    currency: React.PropTypes.shape({
        CustomerSymbolOverride: React.PropTypes.string,
        SymbolPosition: React.PropTypes.string,
        Symbol: React.PropTypes.string,
    }),
    pricingConfiguration: React.PropTypes.shape({
        PriceFormatDisplay: React.PropTypes.oneOf(['TwoDP', 'Rounded']),
        ShowGroupSeperator: React.PropTypes.bool,
    }).isRequired,
    prependText: React.PropTypes.string,
    appendText: React.PropTypes.string,
    classes: React.PropTypes.string,
};
