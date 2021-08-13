import * as SearchResultsConstants from './searchresults';

export const SORT_OPTIONS = [
    {
        name: 'Price Ascending',
        sortFunction: (a, b) => {
            if (a.Price > b.Price) {
                return 1;
            }
            if (a.Price < b.Price) {
                return -1;
            }
            return 0;
        },
    },
    {
        name: 'Price Descending',
        sortFunction: (a, b) => {
            if (a.Price > b.Price) {
                return -1;
            }
            if (a.Price < b.Price) {
                return 1;
            }
            return 0;
        },
    },
];

export const FILTER_OPTIONS = [
    {
        name: 'price',
        type: SearchResultsConstants.FILTER_TYPES.RESULTS_RANGE,
        isSubResultsFilter: false,
        title: 'Price',
        field: 'Price',
        minValue: null,
        maxValue: null,
        expanded: true,
    },
    {
        name: 'maxStops',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Number of Stops',
        field: 'MetaData.MaxStops',
        label: '##value##',
        labelZero: 'Direct',
        options: [],
        expanded: true,
    },
    {
        name: 'flightCarrier',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Flight Carrier',
        field: 'MetaData.FlightCarrierId',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
    {
        name: 'flightClass',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Flight Class',
        field: 'MetaData.FlightClassId',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
    {
        name: 'departureAirport',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Departure Airport',
        field: 'MetaData.DepartureAirportId',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
    {
        name: 'arrivalAirport',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Arrival Airport',
        field: 'MetaData.ArrivalAirportId',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
];
