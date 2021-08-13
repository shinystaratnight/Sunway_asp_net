import 'components/_price.scss';

import NumberFunctions from 'library/numberfunctions';
import React from 'react';

export default class Price extends React.Component {
    constructor() {
        super();
        this.state = {
        };
    }
    renderTotalGroupPrice() {
        const summaryString = `Total Party: ${NumberFunctions.formatTotalPrice(this.props.amount,
            this.props.pricingConfiguration, this.props.currency)}`;
        return (
            <span className="total-price-summary block">
                 {summaryString}
            </span>
        );
    }
    renderLoadingDot(index) {
        return (
            <span key={`dot-${index}`} className={`dot dot-${index}`}>.</span>
        );
    }
    renderLoader() {
        const elements = [];
        const numberOfDots = 3;

        const currency = this.props.currency;
        const symbol = currency.CustomerSymbolOverride
            ? currency.CustomerSymbolOverride
            : currency.Symbol;

        if (currency.SymbolPosition === 'Prepend') {
            const symbolElement = <span key="symbol">{symbol}</span>;
            elements.push(symbolElement);
        }

        for (let i = 1; i <= numberOfDots; i++) {
            const dotElement = this.renderLoadingDot(i);
            elements.push(dotElement);
        }

        if (currency.SymbolPosition === 'Append') {
            const symbolElement = <span key="symbol">{symbol}</span>;
            elements.push(symbolElement);
        }

        return elements;
    }
    render() {
        const formattedAmount = this.props.forceTotalGroupPrice
            ? NumberFunctions.formatTotalPrice(this.props.amount,
            this.props.pricingConfiguration, this.props.currency)
            : NumberFunctions.formatPrice(this.props.amount,
            this.props.pricingConfiguration, this.props.currency);

        let classes = 'price';
        if (this.props.classes) {
            classes = this.props.classes;
        }
        if (this.props.loading) {
            classes += ' loading';
        }

        let priceAmountClass = 'price-amount';
        if (this.props.classes) {
            priceAmountClass += ` ${this.props.classes}`;
        }

        const prependTextClass = this.props.inlinePrependText ? this.props.prependTextClasses
            : `${this.props.prependTextClasses} block`;

        const perPersonText = this.props.pricingConfiguration.PerPersonPricing
            && this.props.displayPerPersonText
            && !this.props.forceTotalGroupPrice
            ? ' pp'
            : '';

        const renderTotalGroupPrice = this.props.displayTotalGroupPrice
            && this.props.pricingConfiguration.PerPersonPricing;
        return (
            <p className={classes}>
                {this.props.prependText
                    && <span className={prependTextClass}>{this.props.prependText}</span>}
                {!this.props.loading
                    && <span className={priceAmountClass}>{formattedAmount}</span>}
                {!this.props.loading && perPersonText}
                {this.props.appendText
                    && this.props.appendText}
                {renderTotalGroupPrice
                    && !this.props.loading
                    && this.renderTotalGroupPrice()}
                {this.props.loading
                    && this.renderLoader()}
            </p>
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
        PerPersonPricing: React.PropTypes.bool,
        PerPersonPriceFormat: React.PropTypes.oneOf(['TwoDP', 'Rounded']),
        NumberOfPeople: React.PropTypes.number,
    }).isRequired,
    prependText: React.PropTypes.string,
    appendText: React.PropTypes.string,
    classes: React.PropTypes.string,
    prependTextClasses: React.PropTypes.string,
    displayTotalGroupPrice: React.PropTypes.bool,
    displayPerPersonText: React.PropTypes.bool,
    inlinePrependText: React.PropTypes.bool,
    forceTotalGroupPrice: React.PropTypes.bool,
    loading: React.PropTypes.bool,

};

Price.defaultProps = {
    forceTotalGroupPrice: false,
    displayTotalGroupPrice: true,
    displayPerPersonText: true,
    inlinePrependText: true,
    prependTextClasses: '',
    loading: false,
};
