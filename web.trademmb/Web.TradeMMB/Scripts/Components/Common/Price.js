import NumberFunctions from 'library/numberfunctions';
import React from 'react';

export default class Price extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    render() {
        const formattedAmount = NumberFunctions.formatPrice(this.props.amount,
            this.props.pricingConfiguration, this.props.currency);

        let classes = 'price';
        if (this.props.classes) {
            classes = this.props.classes;
        }

        let priceAmountClass = 'price-amount';
        if (this.props.classes) {
            priceAmountClass += ` ${this.props.classes}`;
        }

        return (
            <p className={classes}>
                {this.props.prependText
                && this.props.prependText}
                <span className={priceAmountClass}>{formattedAmount}</span>
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
