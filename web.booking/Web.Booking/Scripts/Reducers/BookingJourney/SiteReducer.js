import * as types from '../../actions/bookingjourney/actionTypes';

const initialState = {
    isFetching: false,
    isLoaded: false,
};

/**
 * Redux Reducer for the site
 * @param {object} state - The current state.
 * @param {string} action - The action to act on.
 * @return {object} the updated object.
 */
export default function siteReducer(state = initialState, action) {
    switch (action.type) {
        case types.SITE_REQUEST:
            return Object.assign({}, state, {
                isFetching: true,
                isLoaded: false,
            });
        case types.SITE_LOAD_SUCCESS:
            return Object.assign({},
                state,
                action.site,
                {
                    isFetching: false,
                    isLoaded: true,
                });
        case types.SEARCHDETAILS_LOAD_SUCCESS: {
            const stateCopy = Object.assign({}, state);
            let numberOfPeople = 0;
            action.searchDetails.Rooms.forEach(r => {
                numberOfPeople += (r.Adults + r.Children);
            });
            stateCopy.SiteConfiguration.PricingConfiguration.NumberOfPeople = numberOfPeople;
            return stateCopy;
        }
        case types.SEARCH_RESULT: {
            const stateCopy = Object.assign({}, state);
            let numberOfPeople = 0;
            action.result.searchDetails.Rooms.forEach(r => {
                numberOfPeople += (r.Adults + r.Children);
            });
            stateCopy.SiteConfiguration.PricingConfiguration.NumberOfPeople = numberOfPeople;
            return stateCopy;
        }
        default:
            return state;
    }
}
