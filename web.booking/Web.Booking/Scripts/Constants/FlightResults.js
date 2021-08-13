import * as SearchResultsConstants from './searchresults';

const timeOfDayOrder = {
    Morning: 1,
    Afternoon: 2,
    Evening: 3,
    Night: 4,
};

/**
* Sort method to sort option label by time of day
* @param {object} a - First option to compare
* @param {object} b - Second option to compare
* @return {number} The sort value
*/
function timeOfDaySort(a, b) {
    if (timeOfDayOrder[a.label] > timeOfDayOrder[b.label]) {
        return 1;
    }
    if (timeOfDayOrder[a.label] < timeOfDayOrder[b.label]) {
        return -1;
    }
    return 0;
}

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
        name: 'outboundTimeOfDay',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Outbound Departure Time',
        field: 'MetaData.OutboundTimeOfDay',
        label: '##value##',
        options: [],
        expanded: true,
        sortFunction: timeOfDaySort,
    },
    {
        name: 'returnTimeOfDay',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Return Departure Time',
        field: 'MetaData.ReturnTimeOfDay',
        label: '##value##',
        options: [],
        expanded: true,
        sortFunction: timeOfDaySort,
    },
    {
        name: 'flightClass',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Flight Class',
        field: 'MetaData.OutboundFlightClassId',
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
