import 'widgets/bookingjourney/_searchResultsList.scss';

import * as SearchConstants from '../../constants/search';
import FlightResult from '../../components/searchresults/flightresult';
import Paging from '../../components/common/paging';
import PropertyResult from '../../components/searchresults/propertyresult';
import React from 'react';
import SearchResultSummary from '../../components/searchresults/searchresultsummary';
import SelectInput from '../../components/form/selectinput';

export default class SearchResultsList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderResult(result, index) {
        const key = `result-${index}`;
        const props = {
            key,
            result,
            onToggleSummary: this.props.onToggleSummary,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            charterPackageText: this.props.charterPackageText,
            searchMode: this.props.searchDetails.SearchMode,
            rooms: this.props.searchDetails.Rooms,
            updatingPrice: this.props.updatingPrice,
            adjustmentAmount: this.props.adjustmentAmount,
            searchDetails: this.props.searchDetails,
        };
        let additionalProps = {};
        let input = '';
        switch (this.props.renderedSearchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL: {
                const cancellationCharges = this.props.cancellationCharges[result.ComponentToken];
                const charges = cancellationCharges ? cancellationCharges.cancellationCharges : {};
                const payments = cancellationCharges ? cancellationCharges.payments : {};
                const displayCancellationCharges
                    = this.props.cancellationChargesToDisplay.indexOf(result.ComponentToken) !== -1;
                additionalProps = {
                    resultTokens: this.props.resultTokens,
                    rooms: this.props.searchDetails.Rooms,
                    countries: this.props.countries,
                    mealBasis: this.props.mealBasis,
                    renderSubResults: this.props.renderSubResults,
                    selectedFlight: this.props.selectedFlight,
                    onComponentAdd: this.props.onComponentAdd,
                    onQuoteShow: this.props.onQuoteShow,
                    summaryBehaviour: 'Link',
                    renderPaxSummary: this.props.renderPaxSummary,
                    starRatingConfiguration: this.props.starRatingConfiguration,
                    roomTypeCharacterLimit: this.props.roomTypeCharacterLimit,
                    onGetCancellationCharges: this.props.onGetCancellationCharges,
                    onHideCancellationCharges: this.props.handleHideCancellationCharges,
                    cancellationCharges: charges,
                    payments,
                    displayCancellationCharges,
                    displayQuoteEmail: this.props.displayQuoteEmail,
                    quoteEmailTooltip: this.props.quoteEmailTooltip,
                };
                const propertyProps = Object.assign(props, additionalProps);

                const basketFlight = this.props.basket.Components
                        .find(component => component.ComponentType === 'Flight');
                if (basketFlight) {
                    propertyProps.basketFlight = basketFlight;
                    propertyProps.basketToken = this.props.basket.BasketToken;
                }

                let PropertyComponent = PropertyResult;
                if (this.props.customerComponents.hasOwnProperty('PropertyResult')) {
                    PropertyComponent = this.props.customerComponents.PropertyResult;
                }
                input = <PropertyComponent {...propertyProps} />;
                break;
            }
            case SearchConstants.SEARCH_MODES.FLIGHT: {
                additionalProps = {
                    airports: this.props.airports,
                    flightCarriers: this.props.flightCarriers,
                    flightClasses: this.props.flightClasses,
                    flightButtons: this.props.flightButtons,
                    changeFlight: this.props.changeFlight,
                    selectedFlight: this.props.selectedFlight,
                    onChangeSelect: this.props.onChangeFlightSelect,
                    onSelect: this.props.onComponentAdd,
                    cmsBaseUrl: this.props.cmsBaseUrl,
                    charterFlightText: this.props.charterFlightText,
                    totalResults: this.props.totalResults,
                };
                const flightProps = Object.assign(props, additionalProps);

                let FlightComponent = FlightResult;
                if (this.props.customerComponents.hasOwnProperty('FlightResult')) {
                    FlightComponent = this.props.customerComponents.FlightResult;
                }

                input = <FlightComponent {...flightProps} />;
                break;
            }
            default:
        }
        return input;
    }
    renderSortOptions() {
        const sortOptionsProps = {
            name: 'SortOption',
            options: [],
            containerClass: 'inline',
            onChange: this.props.onSortChange,
            value: this.props.selectedSort,
            label: 'Sort by',
            labelClass: 'sort-label',
        };

        this.props.sortOptions.forEach(sortOption => {
            sortOptionsProps.options.push(sortOption.name);
        });

        return (
            <div className="search-results-list-sort">
                <SelectInput {...sortOptionsProps} />
            </div>
        );
    }
    renderSearchSummary() {
        const searchResultSummaryProps = {
            searchDetails: this.props.searchDetails,
            searchMode: this.props.renderedSearchMode,
            totalResults: this.props.totalResults,
            countries: this.props.countries,
            changeFlight: this.props.changeFlight,
            airportResortGroups: this.props.airportResortGroups,
        };
        if (this.props.searchSummaryHeaderFormat) {
            searchResultSummaryProps.headerFormat = this.props.searchSummaryHeaderFormat;
        }
        const renderMobileSummary = this.props.renderMobileSummary;
        let summaryClass = '';
        if (renderMobileSummary) {
            summaryClass += ' hidden-sm-down';
        }
        return (
            <div className="search-result-summary-container">
                <div className={summaryClass}>
                    <SearchResultSummary {...searchResultSummaryProps} />
                </div>
                {this.props.searchResults.length > 0
                    && this.renderSortOptions()}
            </div>
        );
    }
    renderPaging() {
        const pagingProps = {
            totalPages: this.props.totalPages,
            currentPage: this.props.currentPage,
            pageLinks: this.props.pageLinks,
            onPageClick: this.props.onPageClick,
        };
        return (
            <Paging {...pagingProps} />
        );
    }
    render() {
        return (
            <div className="search-results-list">
                {this.props.searchResults.length > 0
                    && this.renderSearchSummary()}

                {this.props.searchResults.length > 0
                    && this.props.searchResults.map(this.renderResult, this)}

                {this.props.searchResults.length > 0
                    && this.props.totalPages > 1
                    && this.renderPaging()}
            </div>
        );
    }
}

SearchResultsList.propTypes = {
    searchDetails: React.PropTypes.object.isRequired,
    renderedSearchMode: React.PropTypes.string.isRequired,
    resultTokens: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.array.isRequired,
    totalResults: React.PropTypes.number.isRequired,

    sortOptions: React.PropTypes.array.isRequired,
    selectedSort: React.PropTypes.string.isRequired,
    onSortChange: React.PropTypes.func.isRequired,

    currentPage: React.PropTypes.number.isRequired,
    totalPages: React.PropTypes.number.isRequired,
    pageLinks: React.PropTypes.number.isRequired,
    onPageClick: React.PropTypes.func.isRequired,

    countries: React.PropTypes.array.isRequired,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    starRatingConfiguration: React.PropTypes.object.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,
    flightButtons: React.PropTypes.object.isRequired,
    airportResortGroups: React.PropTypes.array,

    renderSubResults: React.PropTypes.bool,
    changeFlight: React.PropTypes.bool,
    selectedFlight: React.PropTypes.object,
    onChangeFlightSelect: React.PropTypes.func,
    onToggleSummary: React.PropTypes.func,
    onComponentAdd: React.PropTypes.func,
    cmsBaseUrl: React.PropTypes.string,
    searchSummaryHeaderFormat: React.PropTypes.string,

    renderPaxSummary: React.PropTypes.bool,
    charterPackageText: React.PropTypes.string,
    charterFlightText: React.PropTypes.string,

    customerComponents: React.PropTypes.object,

    updatingPrice: React.PropTypes.bool,
    adjustmentAmount: React.PropTypes.number,
    basket: React.PropTypes.object,
    roomTypeCharacterLimit: React.PropTypes.number,

    cancellationCharges: React.PropTypes.object,
    cancellationChargesToDisplay: React.PropTypes.array,
    onGetCancellationCharges: React.PropTypes.func,
    handleHideCancellationCharges: React.PropTypes.func,
    renderMobileSummary: React.PropTypes.bool,
    displayQuoteEmail: React.PropTypes.bool,
    quoteEmailTooltip: React.PropTypes.string,

    onQuoteShow: React.PropTypes.func,
};

SearchResultsList.defaultProps = {
    renderSubResults: false,
    changeFlight: false,
    charterPackageText: '',
    charterFlightText: '',
    renderPaxSummary: false,
    updatingPrice: false,
};

