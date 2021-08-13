class NumberFunctions {
    /**
     * Function to add a group separator to a given number
     * @param {number} number - The number to update
     * @return {string} the number with the group separator added.
    */
    static addGroupSeparator(number) {
        const parts = number.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }

    /**
     * Function to format an amount into a price with the currency symbol
     * @param {number} amount - The amount to format
     * @param {object} pricingConfiguration - The pricing configuration
     * @param {object} currency = The currency object
     * @return {string} the number formatted with the currency symbol.
    */
    static formatPrice(amount, pricingConfiguration, currency) {
        let parsedAmount = amount;
        if (typeof parsedAmount === 'string') {
            parsedAmount = parseFloat(parsedAmount);
        }

        let price = (pricingConfiguration.PriceFormatDisplay === 'Rounded')
            ? Math.ceil(parsedAmount.toFixed(0))
            : parsedAmount.toFixed(2);

        price = pricingConfiguration.ShowGroupSeparator
            ? NumberFunctions.addGroupSeparator(price)
            : price;

        const symbol = currency.CustomerSymbolOverride
            ? currency.CustomerSymbolOverride
            : currency.Symbol;

        let formattedPrice = '';
        if (currency.SymbolPosition === 'Prepend') {
            formattedPrice = `${symbol}${price}`;
        } else {
            formattedPrice = `${price}${symbol}`;
        }
        return formattedPrice;
    }
}

export default NumberFunctions;
