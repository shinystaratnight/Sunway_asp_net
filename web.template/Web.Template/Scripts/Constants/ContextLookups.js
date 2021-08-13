export const entities = {
    Country: {
        apiUrl: '/api/geography/country',
    },
    Region: {
        apiUrl: '/api/geography/region',
    },
    Property: {
        apiUrl: '/api/property/propertyreference',
    },
    Trade: {
        apiUrl: '/api/trade',
        context: (lookup) => `${lookup.Name} ${lookup.ABTAATOLNumber}`,
    },
    TradeGroup: {
        apiUrl: '/api/tradegroup',
        context: (lookup) => `${lookup.Name} (${lookup.TradeParentGroup})`,
    },
    TradeParentGroup: {
        apiUrl: '/api/tradeparentgroup',
    },
    TradeContactGroup: {
        apiUrl: '/api/tradecontactgroup',
    },
};
