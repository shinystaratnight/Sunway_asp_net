import * as BasketActions from '../../actions/bookingjourney/basketactions';
import * as SearchActions from '../../actions/bookingjourney/searchActions';
import * as SearchResultActions from '../../actions/bookingjourney/searchResultActions';

import InsuranceUpsell from 'widgets/bookingjourney/insuranceUpsell';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class InsuranceUpsellContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isInitialised: false,
            isLoadingResultsFromBasket: false,
            searchFailed: false,
            selectedComponentToken: 0,
            selectedSubComponentToken: 0,
        };
        this.handleModifyBasket = this.handleModifyBasket.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.state.isInitialised
            && nextProps.search.isLoaded
            && nextProps.basket.isLoaded) {
            this.init(nextProps);
        }

        if (this.isSearchComplete(nextProps)
                && !nextProps.searchResults.isFetching
                && !nextProps.searchResults.isLoaded) {
            this.loadSearchResults(nextProps);
        }

        if (this.props.searchResults.isFetching
            && nextProps.searchResults.isLoaded) {
            this.handleResultsLoaded(nextProps);
        }
    }
    init(props) {
        this.setState({
            isInitialised: true,
        });

        const basketInsurance = this.getBasketInsurance(props);

        if (basketInsurance) {
            this.loadSearchResultsFromBasket(basketInsurance);
        } else {
            this.performExtraSearch();
        }
    }
    isInitialised() {
        const isInitialised = this.props.search.isLoaded
            && this.props.basket.isLoaded
            && !this.state.isLoadingResultsFromBasket;
        return isInitialised;
    }
    getBasketInsurance(props) {
        const basketComponents = props.basket.basket.Components;
        const contentModel = props.entity.model;
        const extraTypes = contentModel.Configuration.ExtraTypes;

        const basketInsurance = basketComponents.find(c =>
            c.ComponentType === 'Extra' && extraTypes.indexOf(c.ExtraTypeId) > -1);
        return basketInsurance;
    }
    getInsuranceProps() {
        const contentModel = this.props.entity.model;
        const props = {
            title: contentModel.Title,
            description: contentModel.Description,
            tableHeadings: contentModel.TableHeadings,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            selectedCurrency: this.props.session.UserSession.SelectCurrency,
            selectedComponentToken: this.state.selectedComponentToken,
            selectedSubComponentToken: this.state.selectedSubComponentToken,
            isSearching: this.props.search.extraSearch.Insurance
                && this.props.search.extraSearch.Insurance.isSearching,
            results: this.getValidResults(),
            onResultSelect: this.handleModifyBasket,
            waitMessage: contentModel.SearchWaitMessage,
            searchFailed: this.state.searchFailed,
            searchFailedMessage: contentModel.searchFailedMessage,
        };
        return props;
    }
    getValidResults() {
        const validResults = [];
        const searchDetails = this.props.search.searchDetails;

        let componentToCheck = null;
        let departureDate = moment(searchDetails.DepartureDate, 'YYYY-MM-DD');
        let returnDate = moment(departureDate).add(searchDetails.Duration, 'd');
        this.props.basket.basket.Components.forEach(component => {
            if (componentToCheck === null && component.ComponentType === 'Flight') {
                componentToCheck = component;
                departureDate = moment(component.OutboundFlightDetails.DepartureDate);
                returnDate = moment(component.ReturnFlightDetails.ArrivalDate);
            }
        });
        if (componentToCheck === null) {
            this.props.basket.basket.Components.forEach(component => {
                if (componentToCheck === null && component.ComponentType === 'Property') {
                    componentToCheck = component;
                    departureDate = moment(component.ArrivalDate);
                    returnDate = moment(component.ArrivalDate).add(component.Duration, 'd');
                }
            });
        }
        this.props.searchResults.results.forEach(result => {
            const validOptions = result.SubResults.filter(subResult =>
                departureDate.isSame(subResult.StartDate, 'YYYY-MM-DD')
                    && returnDate.isSame(subResult.EndDate, 'YYYY-MM-DD'));
            if (validOptions.length > 0) {
                const validResult = Object.assign({}, result);
                validResult.SubResults = validOptions;
                validResults.push(validResult);
            }
        });
        return validResults;
    }
    handleResultsLoaded(props) {
        const searchFailed = props.searchResults.results.length === 0;

        if (this.state.isLoadingResultsFromBasket && searchFailed) {
            this.performExtraSearch();
        } else if (!this.state.isLoadingResultsFromBasket && searchFailed) {
            this.setState({ searchFailed });
        }

        this.setState({ isLoadingResultsFromBasket: false });
    }
    handleModifyBasket(componentToken, subComponentToken) {
        const basket = this.props.basket.basket;
        const basketToken = basket.BasketToken;
        const searchToken = this.props.search.searchResult.ResultTokens.Insurance;

        const componentModel = {
            BasketToken: basketToken,
            SearchToken: searchToken,
        };

        const existingComponent = basket.Components.find(c => c.ComponentType === 'Extra');
        const shouldRemoveAndAdd = existingComponent
            && existingComponent.ComponentToken !== componentToken;
        const shouldRemoveOnly = existingComponent
            && existingComponent.ComponentToken === componentToken;
        const shouldAddOnly = !existingComponent;

        if (shouldRemoveOnly || shouldRemoveAndAdd) {
            componentModel.ComponentToken = existingComponent.ComponentToken;
            this.props.actions.basketRemoveComponent(componentModel);
        }

        if (shouldRemoveAndAdd || shouldAddOnly) {
            componentModel.ComponentToken = componentToken;
            componentModel.SubComponentTokens = [subComponentToken];
            componentModel.MetaData = {
                IncludeOptions: false,
            };

            this.props.actions.basketAddComponent(componentModel);

            const selectedComponentToken = parseInt(componentToken, 10);
            const selectedSubComponentToken = parseInt(subComponentToken, 10);
            this.setState({ selectedComponentToken, selectedSubComponentToken });
        }
    }
    isSearchComplete(props) {
        const searchComplete = props.search.extraSearch.Insurance
                && !props.search.extraSearch.Insurance.isSearching
                && props.search.extraSearch.Insurance.searchComplete
                && props.search.searchResult.ResultTokens.Insurance;
        return searchComplete;
    }
    loadSearchResults(props) {
        const resultToken = props.search.searchResult.ResultTokens.Insurance;
        this.props.actions.loadExtraSearchResults(resultToken, 'Insurance');
    }
    loadSearchResultsFromBasket(basketInsurance) {
        const searchToken = basketInsurance.SearchToken;
        this.props.actions.loadExtraSearchResults(searchToken, 'Insurance');
        this.setState({
            isLoadingResultsFromBasket: true,
        });
    }
    performExtraSearch() {
        const basketToken = UrlFunctions.getQueryStringValue('t');
        const contentModel = this.props.entity.model;
        const searchDetails = {
            basketToken,
            extraId: 0,
            extraGroupId: 0,
            extraTypes: contentModel.Configuration.ExtraTypes,
        };
        this.props.actions.performExtraBasketSearch('Insurance', searchDetails);
    }
    renderPlaceholder() {
        const contentModel = this.props.entity.model;
        return (
            <div className="widget-transfer-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">{contentModel.Title}</h3>
                </div>
                <div className="panel-body">
                    <div className="wait-message-container">
                        <i className="loading-icon" aria-hidden="true"></i>
                        <div className="wait-message">
                            <p>Loading...</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        return (
            <div>
                {this.isInitialised()
                    && <InsuranceUpsell {...this.getInsuranceProps()} />}
                {!this.isInitialised()
                    && this.renderPlaceholder()}
            </div>
        );
    }
}

InsuranceUpsellContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const search = state.search ? state.search : {};
    const searchResults = state.searchResults && state.searchResults.Insurance
            ? state.searchResults.Insurance : { results: [] };
    return {
        basket,
        search,
        searchResults,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BasketActions,
        SearchActions,
        SearchResultActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(InsuranceUpsellContainer);
