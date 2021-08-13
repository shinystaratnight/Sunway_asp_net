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
    static formatTotalPrice(amount, pricingConfiguration, currency) {
        let parsedAmount = amount;
        if (typeof parsedAmount === 'string') {
            parsedAmount = parseFloat(parsedAmount);
        }

        let price = (pricingConfiguration.PriceFormatDisplay === 'Rounded')
            ? Math.ceil(parsedAmount.toFixed(2))
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
        const numberOfPeople = pricingConfiguration.NumberOfPeople;
        parsedAmount = pricingConfiguration.PerPersonPricing
            ? parsedAmount / numberOfPeople
            : parsedAmount;

        const roundPrice = (pricingConfiguration.PriceFormatDisplay === 'Rounded'
            && !pricingConfiguration.PerPersonPricing)
            || ((pricingConfiguration.PerPersonPriceFormat === 'Rounded'
                || pricingConfiguration.PerPersonPriceFormatDisplay === 'Rounded')
                && pricingConfiguration.PerPersonPricing);
        let price = roundPrice ? Math.ceil(parsedAmount.toFixed(2))
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

    /**
     * Function to format an amount into a price with the currency symbol
     * @param {number} amount - The amount to format
     * @param {object} pricingConfiguration - The pricing configuration
     * @param {object} currency = The currency object
     * @param {boolean} isMax = True if the value is a maximum value
     * @return {string} the number formatted with the currency symbol.
    */
    static formatPriceSliderPrice(amount, pricingConfiguration, currency, isMax) {
        let parsedAmount = amount;
        if (typeof parsedAmount === 'string') {
            parsedAmount = parseFloat(parsedAmount);
        }
        const numberOfPeople = pricingConfiguration.NumberOfPeople;
        parsedAmount = pricingConfiguration.PerPersonPricing
            ? parsedAmount / numberOfPeople
            : parsedAmount;

        const roundPrice = pricingConfiguration.PriceSliderFormatDisplay === 'Rounded';

        let price = parsedAmount.toFixed(2);
        if (roundPrice) {
            price = isMax ? Math.ceil(price) : Math.floor(price);
        }

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
