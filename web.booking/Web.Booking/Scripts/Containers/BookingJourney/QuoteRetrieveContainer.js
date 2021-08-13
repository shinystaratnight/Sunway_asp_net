import 'widgets/bookingjourney/_quoteretrieve.scss';

import * as QuoteActions from 'actions/bookingjourney/quoteActions';
import * as SearchActions from 'actions/bookingjourney/searchActions';
import * as SearchConstants from 'constants/search';

import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class QuoteRetrieveContainer extends React.Component {
    componentDidMount() {
        this.retrieveQuote();
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.quote.retrieveComplete
            && nextProps.quote.retrieveComplete) {
            this.handleResult(nextProps);
        }
    }
    handleResult(nextProps) {
        const result = nextProps.quote.result;
        const searchUrl = this.searchDetailsToUrl(result.SearchModel);
        this.props.actions.setSearchDetailsFromUrl(searchUrl);

        const componentCount = result.QuoteComponentTypes.length;
        const repricedComponentCount = result.RepricedComponentTypes.length;

        const flightHotelCount = result.QuoteComponentTypes
            .filter(component => component === 'Flight' || component === 'Hotel').length;
        const repricedFlightHotelCount = result.RepricedComponentTypes
            .filter(component => component === 'Flight' || component === 'Hotel').length;

        let failedUrl = this.props.site.SiteConfiguration.SearchConfiguration.FailedSearchUrl;
        const userSession = this.props.session.UserSession;
        if (userSession.OverBranded) {
            failedUrl = userSession.TradeSession.Trade.Website;
        } else if (window.location.host === 'localhost:58633') {
            failedUrl = 'http://localhost:64351/';
        }

        // if no property or flight redirect to homepage
        if (repricedFlightHotelCount === 0) {
            window.location = `${failedUrl}?warning=quote-nopackage`;
        }

        // if all components repriced redirect to payment page
        if (componentCount === repricedComponentCount
            && repricedComponentCount > 0
            && result.BasketToken) {
            let paymentUrl = `/booking/payment${searchUrl}?t=${result.BasketToken}`;
            if (result.PriceChange !== 0) {
                paymentUrl += `&pricechange=${result.PriceChange}`;
            }
            window.location = paymentUrl;
        }

        // if flight and hotel repriced but transfers/extras not repriced redirect to extras page
        if (flightHotelCount === repricedFlightHotelCount
                && componentCount !== repricedComponentCount
                && result.BasketToken) {
            let extrasUrl = `/booking/extras${searchUrl}?t=${result.BasketToken}`;
            extrasUrl += '&warning=quote-extra';
            window.location = extrasUrl;
        }

        // if flight not repriced but results available redirect to property page
        if (result.QuoteComponentTypes.indexOf('Flight') > -1
            && result.RepricedComponentTypes.indexOf('Flight') === -1) {
            if (result.ResultCounts.Flight) {
                let propertyDetailsUrl = `/booking/details${searchUrl}?`;
                propertyDetailsUrl += `propertyId=${result.PropertyId}`;
                propertyDetailsUrl += `&t=${result.BasketToken}`;
                propertyDetailsUrl += `&f=${result.ResultTokens.Flight}`;
                propertyDetailsUrl += `&sf=${result.SelectedFlightToken}`;
                propertyDetailsUrl += '&warning=quote-flight';

                window.location = propertyDetailsUrl;
            } else {
                window.location = `${failedUrl}?warning=quote-noflight`;
            }
        }

        // if property not repriced but results available redirect to results page
        if (result.QuoteComponentTypes.indexOf('Hotel') > -1
            && result.RepricedComponentTypes.indexOf('Hotel') === -1) {
            if (result.ResultCounts.Hotel) {
                let resultsUrl = `/booking/results${searchUrl}?`;
                resultsUrl += `propertyId=${result.PropertyId}`;
                resultsUrl += `&h=${result.ResultTokens.Hotel}`;
                resultsUrl += `&t=${result.BasketToken}`;
                resultsUrl += '&warning=quote-hotel';

                window.location = resultsUrl;
            } else {
                window.location = `${failedUrl}?warning=quote-nohotel`;
            }
        }
    }
    retrieveQuote() {
        const url = window.location.pathname;
        const lastIndex = url.lastIndexOf('/');
        const quoteReference = url.substring(lastIndex + 1);
        this.props.actions.retrieveQuote(quoteReference);
    }
    searchDetailsToUrl(searchDetails) {
        let searchMode = searchDetails.SearchMode.toLowerCase();
        if (searchMode === 'flightplushotel' && searchDetails.PackageSearch) {
            searchMode = 'package';
        }

        let searchURL = `/${searchMode}`;

        if (searchDetails.SearchMode === 'Flight'
            || searchDetails.SearchMode === 'FlightPlusHotel') {
            searchURL = `${searchURL}/${searchDetails.DepartureType.toLowerCase()}`;
            searchURL = `${searchURL}/${searchDetails.DepartureID}`;
        }

        let arrivalId = searchDetails.ArrivalID;
        const idAdapter
            = SearchConstants.ID_ADAPTERS.filter(ida =>
                ida.type === searchDetails.ArrivalType.toLowerCase())[0];
        if (idAdapter) {
            arrivalId = idAdapter.inverseShift(arrivalId);
        }

        searchURL = `${searchURL}/${searchDetails.ArrivalType.toLowerCase()}/${arrivalId}`;

        const departureDate = moment(searchDetails.DepartureDate);
        searchURL = `${searchURL}/${departureDate.format('YYYY-MM-DD')}`;
        searchURL = `${searchURL}/${searchDetails.Duration}`;

        let adults = '';
        let children = '';
        let childAges = '';
        let infants = '';

        searchDetails.Rooms.forEach(room => {
            adults = adults !== '' ? `${adults}_${room.Adults}` : `${room.Adults}`;
            children = children !== '' ? `${children}_${room.Children}` : `${room.Children}`;
            infants = infants !== '' ? `${infants}_${room.Infants}` : `${room.Infants}`;

            let roomChildAges = '';
            room.ChildAges.forEach(childAge => {
                roomChildAges = roomChildAges !== ''
                    ? `${roomChildAges}-${childAge}` : `${childAge}`;
            });
            if (room.Id < searchDetails.Rooms.length) {
                roomChildAges += '_';
            }
            childAges += roomChildAges;
        });

        if (searchDetails.SearchMode !== 'Flight') {
            searchURL = `${searchURL}/${searchDetails.Rooms.length}`;
        }

        childAges = childAges ? childAges : '0';

        searchURL = `${searchURL}/${adults}/${children}/${infants}/${childAges}`;

        if ((searchDetails.SearchMode === 'FlightPlusHotel'
                || searchDetails.SearchMode === 'Flight')
                && searchDetails.FlightClassId) {
            searchURL += searchURL.indexOf('?') > -1 ? '&' : '?';
            searchURL += `flightclassid=${searchDetails.FlightClassId}`;
        }

        return searchURL;
    }
    render() {
        return (
            <div></div>
        );
    }
}

QuoteRetrieveContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    quote: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const quote = state.quote ? state.quote : {};
    return {
        quote,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
       QuoteActions,
       SearchActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(QuoteRetrieveContainer);
