export const entities = {
    Country: {
        apiUrl: '/booking/api/geography/country',
    },
    Region: {
        apiUrl: '/booking/api/geography/region',
    },
    Property: {
        apiUrl: '/booking/api/property/propertyreference',
    },
    Trade: {
        apiUrl: '/booking/api/trade',
        context: (lookup) => `${lookup.Name} ${lookup.ABTAATOLNumber}`,
    },
    TradeGroup: {
        apiUrl: '/booking/api/tradegroup',
        context: (lookup) => `${lookup.Name} (${lookup.TradeParentGroup})`,
    },
    TradeParentGroup: {
        apiUrl: '/booking/api/tradeparentgroup',
    },
    TradeContactGroup: {
        apiUrl: '/booking/api/tradecontactgroup',
    },
};
