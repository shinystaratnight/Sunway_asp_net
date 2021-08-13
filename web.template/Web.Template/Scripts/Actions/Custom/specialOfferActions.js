import * as types from './actionTypes';
import SpecialOffersApi from '../../api/specialOffersAPI';

/**
 * Redux Action method for successfully loading a user.
 * @param {string} key - The key of the offers object to return
 * @return {object} The action type and offers object
 */
function specialOffersRequest(key) {
    return { type: types.SPECIALOFFERS_REQUEST, key };
}

/**
 * Redux Action method for successfully loading a user.
 * @param {object} offers - The offers object to return
 * @param {string} key - The key of the offers object to return
 * @return {object} The action type and offers object
 */
function specialOffersLoadSuccess(offers, key) {
    return { type: types.SPECIALOFFERS_LOAD_SUCCESS, offers, key };
}

/**
 * Redux Action method for successfully loading a user.
 * @param {object} offer - The offers object to return
 * @param {string} key - The key of the offers object to return
 * @return {object} The action type and offers object
 */
function specialOfferLoadSuccess(offer, key) {
    return { type: types.SPECIALOFFER_LOAD_SUCCESS, offer, key };
}

/**
 * Redux Action method for updating the current page
 * @param {number} page - The selected page number
 * @param {string} key - The unique ID of the offers concerned
 * @return {object} The action type and page number
 */
export function specialOffersUpdatePage(page, key) {
    return { type: types.SPECIALOFFERS_UPDATE_PAGE, page, key };
}

/**
 * Redux Action method for sorting the search results
 * @param {string} sortOption - The sort option to use
 * @param {key} key - The unique identifier to identify special offers
 * @return {object} The action type and sort option
 */
export function specialOffersSort(sortOption, key) {
    return { type: types.SPECIALOFFERS_SORT, sortOption, key };
}

/**
 * Redux Action method for updating filters
 * @param {string} key - The unique id of the special offers
 * @param {object} filters - The filters to return
 * @return {object} the action type and filters object
 */
function specialOffersFilterUpdate(key, filters) {
    return { type: types.SPECIALOFFERS_FILTER_UPDATE, key, filters };
}

/**
 * Redux Action method for updating special offers
 * @param {string} key - The unique id for the special offers
 * @param {object} offers - The special offers to return
 * @return {object} the action type and offers
 */
function specialOffersUpdated(key, offers) {
    return { type: types.SPECIALOFFERS_UPDATED, key, offers };
}

/**
 * Redux Action method for loading the current offers.
 * @param {object} filterOptions - The values that will be used to build up the url.
 * @param {object} siteConfiguration - The site configuration
 * @return {object} The offers
 */
export function getSpecialOffers(filterOptions, siteConfiguration) {
    return function load(dispatch) {
        dispatch(specialOffersRequest(filterOptions.key));
        const specialOffersApi = new SpecialOffersApi();
        return specialOffersApi.setupSpecialOffers(filterOptions, siteConfiguration)
            .then(offers => {
                dispatch(specialOffersLoadSuccess(offers, filterOptions.key));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for loading the current offers.
 * @param {object} id - The id to retrieve offers relat	ing to.
 * @param {object} key - the key asociated with the offers.
 * @return {object} The offers
 */
export function getSpecialOffer(id, key) {
    return function load(dispatch) {
        dispatch(specialOffersRequest(key));
        const specialOffersApi = new SpecialOffersApi();
        return specialOffersApi.getOffer(id, key)
            .then(offer => {
                dispatch(specialOfferLoadSuccess(offer, key));
            })
            .catch(error => {
                throw (error);
            });
    };
}

/**
 * Redux Action method for filtering search results
 * @param {string} key - The unique id for the offers
 * @param {object} offers - The special offer results to filter
 * @param {object} filters - The filters to apply
 * @return {object} The filtered results
 */
export function filterSpecialOffers(key, offers, filters) {
    return function load(dispatch) {
        const specialOffersApi = new SpecialOffersApi();
        const filteredOffers = specialOffersApi.filter(offers, filters);
        dispatch(specialOffersUpdated(key, filteredOffers));
    };
}


/**
 * Redux Action method for starting a search request.
 * @param {string} key - The special offers key
 * @param {string} field - The property to update
 * @param {*} value - The new value
 * @return {object} the action
 */
export function specialOffersFilterUpdateValue(key, field, value) {
    return function load(dispatch, getState) {
        const specialOffersApi = new SpecialOffersApi();
        const state = getState();
        const filters = state.specialOffers[key].filters;
        const updatedFilters = specialOffersApi.updateFilter(filters, field, value);
        dispatch(specialOffersFilterUpdate(key, updatedFilters));
        dispatch(filterSpecialOffers(key,
            state.specialOffers[key].items, updatedFilters));
    };
}

/**
 * Redux Action method for updating filters
 * @param {string} key - The unique id of the special offers
 * @return {object} the action
 */
export function specialOffersResetFilters(key) {
    return function load(dispatch, getState) {
        const specialOffersApi = new SpecialOffersApi();
        const state = getState();
        const filters = state.specialOffers[key].filters;
        const updatedFilters = specialOffersApi.resetFilters(filters);
        dispatch(specialOffersFilterUpdate(key, updatedFilters));
        dispatch(filterSpecialOffers(key,
            state.specialOffers[key].items, updatedFilters));
    };
}

/**
 * Redux Action method for loading the current offers.
 * @param {object} offerId - The offerId to retrieve offers relating to.
 * @param {object} airportId - the airportId associated with the offer.
 * @param {object} location - the location of the offer.
 * @param {object} type - the filetype of the poster to be generated.
 * @return {object} The offers
 */
export function getSpecialOfferPoster(offerId, airportId, location, type) {
    return function load() {
        const specialOffersApi = new SpecialOffersApi();
        let posterWindow;
        if (type === 'pdf') {
            posterWindow = window.open();
        }
        return specialOffersApi.getOfferPoster(offerId, airportId, location, type)
            .then(offer => {
                if (offer.Success && type === 'pdf') {
                    posterWindow.location = offer.DocumentUrl;
                } else {
                    const link = document.createElement('a');
                    link.download = offer.DocumentUrl.split('\\').pop();
                    link.href = offer.DocumentUrl;
                    if (window.navigator.userAgent.match(/(MSIE|Trident)/)) {
                        link.target = '_blank';
                    }
                    document.body.appendChild(link);
                    link.click();
                    link.remove();
                }
            })
            .catch(error => {
                posterWindow.close();
                throw (error);
            });
    };
}
