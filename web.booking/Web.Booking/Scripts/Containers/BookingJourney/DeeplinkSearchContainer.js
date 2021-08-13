import * as EntityActions from 'actions/content/entityActions';
import * as SearchActions from 'actions/bookingjourney/searchActions';
import * as SearchResultActions from 'actions/bookingjourney/searchResultActions';

import ExpiredWarning from 'components/bookingjourney/expiredwarning';
import ModalPopup from 'components/common/modalpopup';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class DeeplinkSearchContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            resultExpired: false,
            messagesLoaded: false,
        };
        this.resultTimeout = {};
        this.resultExpiryTime = 1800000;
        this.deeplinkSearch = this.deeplinkSearch.bind(this);
        this.resultExpired = this.resultExpired.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (this.props.search.isSearching && nextProps.search.searchComplete) {
            this.checkNoResults(nextProps);
        }

        if (!this.props.waitMessageLoaded && nextProps.waitMessageLoaded) {
            const hotelResultToken = UrlFunctions.getQueryStringValue('h');
            const basketToken = UrlFunctions.getQueryStringValue('t');
            if (!hotelResultToken && !basketToken) {
                this.deeplinkSearch();
            }
        }

        if (nextProps.search.searchComplete
                && nextProps.search.searchResult.Success) {
            this.resultTimeout = setTimeout(this.resultExpired, this.resultExpiryTime);
        }

        if (!this.state.messagesLoaded) {
            const context = `default-${this.props.site.Name.toLowerCase()}`;
            this.state.messagesLoaded = true;
            this.props.actions.loadEntity('Shared', 'Warnings', context, 'dev');
        }
    }
    checkNoResults(nextProps) {
        const resultCounts = nextProps.search.searchResult.ResultCounts;
        const searchDetails = nextProps.search.searchDetails;
        let noResults = false;
        if ((searchDetails.SearchMode === 'FlightPlusHotel'
                || searchDetails.SearchMode === 'Hotel')
            && (!resultCounts || resultCounts.Hotel === 0)) {
            noResults = true;
        }
        if ((searchDetails.SearchMode === 'FlightPlusHotel'
                || searchDetails.SearchMode === 'Flight')
            && (!resultCounts || resultCounts.Flight === 0)) {
            noResults = true;
        }
        if (noResults) {
            const failedSearchUrl = this.getFailedSearchUrl();
            window.location = `${failedSearchUrl}?warning=noresults`;
        }
    }
    deeplinkSearch() {
        const deeplinkUrl = window.location.pathname;
        this.props.actions.performDeeplinkSearch(deeplinkUrl);
        this.setState({ resultExpired: false });
    }
    getFailedSearchUrl() {
        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;
        let failedSearchUrl = searchConfig.FailedSearchUrl;

        const userSession = this.props.session.UserSession;
        if (userSession.OverBranded) {
            failedSearchUrl = userSession.TradeSession.Trade.Website;
        } else if (window.location.host === 'localhost:58633') {
            failedSearchUrl = 'http://localhost:64351/';
        }

        return failedSearchUrl;
    }
    resultExpired() {
        this.setState({ resultExpired: true });
    }
    renderExpiredPopup() {
        const expiredWarningProps = {
            onClickFunction: this.deeplinkSearch,
            title: this.props.warningMessage.Title,
            body: this.props.warningMessage.Message,
            button: this.props.warningMessage.Button,
        };
        return (
            <ModalPopup>
                <ExpiredWarning {...expiredWarningProps} />
            </ModalPopup>
        );
    }
    render() {
        return (
            <div>
                {this.state.resultExpired
                    && this.renderExpiredPopup()}
            </div>
        );
    }
}

DeeplinkSearchContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.object.isRequired,
    warningMessage: React.PropTypes.object,
    waitMessageLoaded: React.PropTypes.bool,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    const site = state.site ? state.site : {};
    const searchResults = state.searchResults ? state.searchResults : {};

    let warningMessage = {};
    if (site) {
        const warningEntityKey = `Warnings-default-${site.Name.toLowerCase()}`;
        warningMessage
            = state.entities[warningEntityKey] && state.entities[warningEntityKey].isLoaded
            ? state.entities[warningEntityKey].model.BasketExpired.SearchResults : {};
    }

    const wmEntity = 'WaitMessageWidget-default';
    const waitMessageLoaded = state.entities[wmEntity] ? state.entities[wmEntity].isLoaded : false;

    return {
        search,
        site,
        searchResults,
        warningMessage,
        waitMessageLoaded,
    };
}


/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
    SearchActions,
    SearchResultActions,
    EntityActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(DeeplinkSearchContainer);
