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
    {
        name: 'Star Rating High to Low',
        sortFunction: (a, b) => {
            if (parseFloat(a.MetaData.Rating) > parseFloat(b.MetaData.Rating)) {
                return -1;
            }
            if (parseFloat(a.MetaData.Rating) < parseFloat(b.MetaData.Rating)) {
                return 1;
            }
            return 0;
        },
    },
    {
        name: 'Star Rating Low to High',
        sortFunction: (a, b) => {
            if (parseFloat(a.MetaData.Rating) > parseFloat(b.MetaData.Rating)) {
                return 1;
            }
            if (parseFloat(a.MetaData.Rating) < parseFloat(b.MetaData.Rating)) {
                return -1;
            }
            return 0;
        },
    },
    {
        name: 'Name (A to Z)',
        sortFunction: (a, b) => {
            const nameA = a.DisplayName.toLowerCase();
            const nameB = b.DisplayName.toLowerCase();
            if (nameA > nameB) {
                return 1;
            }
            if (nameA < nameB) {
                return -1;
            }
            return 0;
        },
    },
    {
        name: 'Name (Z to A)',
        sortFunction: (a, b) => {
            const nameA = a.DisplayName.toLowerCase();
            const nameB = b.DisplayName.toLowerCase();
            if (nameA > nameB) {
                return -1;
            }
            if (nameA < nameB) {
                return 1;
            }
            return 0;
        },
    },
];

export const FILTER_OPTIONS = [
    {
        name: 'price',
        type: SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_RANGE,
        isSubResultsFilter: true,
        title: 'Price',
        field: 'TotalPrice',
        minValue: null,
        maxValue: null,
        expanded: true,
    },
    {
        name: 'rating',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Star Rating',
        field: 'MetaData.Rating',
        label: '##value## Star',
        labelFunction: (value, siteConfiguration) => {
            const appendTextList = siteConfiguration.StarRatingConfiguration.AppendText;
            let textToAppend = '';

            appendTextList.forEach(appendText => {
                if (parseFloat(appendText.Rating) === parseFloat(value)) {
                    textToAppend = appendText.Text;
                }
            });

            const wholeValue = value.replace('.0', '')
                                    .replace('.5', '');

            let ratingLabel = `${wholeValue} Star ${textToAppend}`;

            if ((parseInt(value, 10) === 0)) {
                ratingLabel = 'Unrated';
            }
            return ratingLabel;
        },
        labelZero: 'Unrated',
        labelValueAttribute: 'rating',
        labelClass: 'rating',
        options: [],
        expanded: true,
    },
    {
        name: 'resort',
        type: SearchResultsConstants.FILTER_TYPES.ID,
        isSubResultsFilter: false,
        title: 'Resort',
        field: 'MetaData.GeographyLevel3Id',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
    {
        name: 'hotelname',
        type: SearchResultsConstants.FILTER_TYPES.TEXT,
        isSubResultsFilter: false,
        title: 'Hotel Name',
        field: 'DisplayName',
        value: '',
        expanded: true,
    },
    {
        name: 'mealBasis',
        type: SearchResultsConstants.FILTER_TYPES.SUB_RESULTS_ID,
        isSubResultsFilter: true,
        title: 'Meal Basis',
        field: 'MealBasisId',
        label: '##lookup##',
        options: [],
        expanded: true,
    },
];
