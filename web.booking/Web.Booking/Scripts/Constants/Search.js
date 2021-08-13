export const SEARCH_MODES = {
    FLIGHT: 'Flight',
    HOTEL: 'Hotel',
    FLIGHT_PLUS_HOTEL: 'FlightPlusHotel',
    TRANSFER: 'Transfer',
    EXTRA: 'Extra',
    CAR_HIRE: 'CarHire',
};

const million = 1000000;
const airportMax = 2;
const airportGroupMin = 2;
const airportGroupMax = 3;

export const flightIDModifier = million;

const noChangeShift = (value) => value;

export const ID_ADAPTERS = [
    {
        type: 'resort',
        max: 0,
        min: -million,
        shift: noChangeShift,
        inverseShift: noChangeShift,
    },
    {
        type: 'region',
        max: million,
        min: 0,
        shift: noChangeShift,
        inverseShift: noChangeShift,
    },
    {
        type: 'airport',
        max: airportMax * million,
        min: million,
        shift: (value) => value + million,
        inverseShift: (value) => value - million,
    },
    {
        type: 'airportgroup',
        max: airportGroupMax * million,
        min: airportGroupMin * million,
        shift: (value) => value + (airportGroupMin * million),
        inverseShift: (value) => value - (airportGroupMin * million),
    },
];
