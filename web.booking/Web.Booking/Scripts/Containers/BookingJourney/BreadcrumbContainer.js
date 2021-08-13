import * as SearchActions from '../../actions/bookingjourney/searchActions';

import Breadcrumb from 'widgets/bookingjourney/breadcrumb';
import React from 'react';
import UrlFunctions from '../../library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BreadcrumbContainer extends React.Component {
    constructor(props) {
        super(props);
        const initialSearch = JSON.parse(JSON.stringify(this.props.search));
        this.state = {
            initialSearch,
        };
    }
    componentDidMount() {
        const searchConfig = this.props.site.SiteConfiguration.SearchConfiguration;
        this.props.actions.loadSearchDetailsIfNeeded(searchConfig);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.search.isLoaded
            && nextProps.search.isLoaded) {
            const initialSearch = JSON.parse(JSON.stringify(nextProps.search));
            this.setState({ initialSearch });
        }
    }
    hasOwnStockFlight() {
        let result = false;
        if (this.props.basket.isLoaded) {
            const index = this.props.basket.basket.Components.findIndex(component =>
                component.ComponentType === 'Flight' && component.Source === 'Own');
            result = index > -1;
        }
        return result;
    }
    isInitialised() {
        return this.props.site.isLoaded
            && this.props.search.isLoaded
            && !this.props.search.isSearching
            && (this.props.search.searchComplete
                || this.props.basket.isLoaded
                || this.props.booking.isLoaded);
    }
    getBreadcrumbProps() {
        const searchMode = this.state.initialSearch.searchDetails.SearchMode;
        const bookingJourneyConfig = this.props.site.SiteConfiguration.BookingJourneyConfiguration;
        const searchModeConfig = bookingJourneyConfig.SearchModes
            .find(config => config.SearchMode === searchMode);

        const pages = [];
        if (searchModeConfig) {
            searchModeConfig.Pages.forEach(searchModePage => {
                let pageName = searchModePage;
                const pageUrl = `/${searchModePage}`;
                let url = window.location.pathname.replace(`/${this.props.page.PageURL}`, pageUrl);

                const basketPages = ['/extras'];
                if (this.hasOwnStockFlight()) {
                    basketPages.push('/results');
                }

                if (searchModePage === 'payment'
                    && bookingJourneyConfig.PaymentMode !== 'Standard') {
                    pageName = 'your details';
                }

                if (basketPages.indexOf(pageUrl) > -1) {
                    const basketToken = UrlFunctions.getQueryStringValue('t');
                    url += `?t=${basketToken}`;
                }

                if (pageUrl === '/details') {
                    const hotelComponent = this.props.basket.basket.Components.find(component =>
                        component.ComponentType === 'Hotel');
                    const flightComponent = this.props.basket.basket.Components.find(component =>
                        component.ComponentType === 'Flight');
                    if (hotelComponent) {
                        url += `?propertyId=${hotelComponent.PropertyReferenceId}`;
                        url += `&h=${hotelComponent.SearchToken}`;
                    }
                    if (flightComponent) {
                        url += `&f=${flightComponent.SearchToken}`;
                        url += `&sf=${flightComponent.ComponentToken}`;
                    }
                }

                const page = {
                    name: pageName,
                    pageUrl,
                    url,
                };
                pages.push(page);

                if (searchModePage === 'payment'
                    && bookingJourneyConfig.PaymentMode !== 'Standard') {
                    const basketToken = UrlFunctions.getQueryStringValue('t');
                    let offsiteURL
                        = window.location.pathname.replace(`/${this.props.page.PageURL}`,
                                    '/offsitepayment');
                    offsiteURL += `?t=${basketToken}`;
                    const offsitePage = {
                        name: 'Payment',
                        pageUrl: '/offsitepayment',
                        url: offsiteURL,
                    };
                    pages.push(offsitePage);
                }
            });
        }

        const props = {
            pages,
            currentPageUrl: `/${this.props.page.PageURL}`,
        };
        return props;
    }
    render() {
        return (
            <div className="breadcrumb-container container">
                {this.isInitialised()
                    && <Breadcrumb {...this.getBreadcrumbProps()} />}
            </div>
        );
    }
}

BreadcrumbContainer.propTypes = {
    actions: React.PropTypes.object,
    page: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object,
    booking: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    const basket = state.basket ? state.basket : {};
    const booking = state.booking ? state.booking : {};

    return {
        search,
        basket,
        booking,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(SearchActions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BreadcrumbContainer);
