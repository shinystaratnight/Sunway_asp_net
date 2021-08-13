import 'widgets/bookingjourney/_searchResultsFilter.scss';

import CheckboxInput from '../../../scripts/components/form/checkboxinput';
import ObjectFunctions from 'library/objectfunctions';
import React from 'react';
import SliderInput from 'components/form/sliderinput';

export default class SearchResultsFilter extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.renderFilterOption = this.renderFilterOption.bind(this);
        this.toggleFilterDisplay = this.toggleFilterDisplay.bind(this);
    }
    componentWillMount() {
        this.props.filters.forEach(filter => {
            if (filter.hasOwnProperty('options')) {
                for (let i = 0; i < filter.options.length; i++) {
                    const option = filter.options[i];
                    option.label = this.generateLabel(filter.name, option);
                }
                if (filter.sortFunction) {
                    filter.options.sort(filter.sortFunction);
                } else {
                    filter.options.sort(this.sortByLabel);
                }
            }
        });
    }
    sortByLabel(a, b) {
        const labelA = a.label.toLowerCase();
        const labelB = b.label.toLowerCase();

        let sortReturn = 0;

        if (labelA < labelB) {
            sortReturn = -1;
        }

        if (labelA > labelB) {
            sortReturn = 1;
        }

        return sortReturn;
    }
    resortName(resortId) {
        for (let i = 0; i < this.props.countries.length; i++) {
            const country = this.props.countries[i];
            for (let j = 0; j < country.Regions.length; j++) {
                const region = country.Regions[j];
                for (let k = 0; k < region.Resorts.length; k++) {
                    const resort = region.Resorts[k];
                    if (resort.Id === resortId) {
                        return resort.Name;
                    }
                }
            }
        }
        return '';
    }
    mealBasisName(mealBasisId) {
        let name = '';
        for (let i = 0; i < this.props.mealBasis.length; i++) {
            const mealBasis = this.props.mealBasis[i];
            if (mealBasis.Id === mealBasisId) {
                name = mealBasis.Name;
                break;
            }
        }
        return name;
    }
    flightClassName(id) {
        let name = '';
        for (let i = 0; i < this.props.flightClasses.length; i++) {
            const flightClass = this.props.flightClasses[i];
            if (flightClass.Id === id) {
                name = flightClass.Name;
                break;
            }
        }
        return name;
    }
    getFlightCarrier(id) {
        let flightCarrierReturn = {};
        for (let i = 0; i < this.props.flightCarriers.length; i++) {
            const flightCarrier = this.props.flightCarriers[i];
            if (flightCarrier.Id === id) {
                flightCarrierReturn = flightCarrier;
                break;
            }
        }
        return flightCarrierReturn;
    }
    getAirport(id) {
        let airportReturn = {};
        for (let i = 0; i < this.props.airports.length; i++) {
            const airport = this.props.airports[i];
            if (airport.Id === id) {
                airportReturn = airport;
                break;
            }
        }
        return airportReturn;
    }
    getMinMaxPrice() {
        let minValue = null;
        let maxValue = null;
        this.props.searchResults.forEach(result => {
            const resultExclude = result.ValidExcludes.filter(validExclude =>
                validExclude.name === 'price')[0];

            if (resultExclude && resultExclude.display) {
                if (minValue === null || Math.floor(result.Price) < minValue) {
                    minValue = Math.floor(result.Price);
                }
                if (maxValue === null || Math.ceil(result.Price) > maxValue) {
                    maxValue = Math.ceil(result.Price);
                }
            }
        });
        return {
            minValue,
            maxValue,
        };
    }
    getSubResultsMinMaxPrice() {
        const minValues = {};
        const maxValues = {};
        this.props.searchResults.forEach(result => {
            result.SubResults.forEach(subResult => {
                if (subResult.ValidExcludes) {
                    let subResultExclude = subResult.ValidExcludes.filter(validExclude =>
                        validExclude.name === 'price')[0];
                    if (!subResultExclude) {
                        subResultExclude = {};
                    }
                    if (subResultExclude.display) {
                        if (!minValues[subResult.Sequence]
                            || Math.floor(subResult.TotalPrice) < minValues[subResult.Sequence]) {
                            minValues[subResult.Sequence] = Math.floor(subResult.TotalPrice);
                        }
                        if (!maxValues[subResult.Sequence]
                            || Math.ceil(subResult.TotalPrice) > maxValues[subResult.Sequence]) {
                            maxValues[subResult.Sequence] = Math.ceil(subResult.TotalPrice);
                        }
                    }
                }
            });
        });

        let minValue = 0;
        let maxValue = 0;
        Object.keys(minValues).map((sequence) => {
            minValue += minValues[sequence];
        });

        Object.keys(maxValues).map((sequence) => {
            maxValue += maxValues[sequence];
        });

        return {
            minValue,
            maxValue,
        };
    }
    itemCount(option, filter) {
        const itemCount = this.props.searchResults.filter(result => {
            if (result.hasOwnProperty('ValidExcludes')) {
                const exclude = result.ValidExcludes.filter(validExclude =>
                    validExclude.name === filter.name)[0];

                if (filter.type === 'ID' && exclude) {
                    const value = ObjectFunctions.getValueByStringPath(result, filter.field);
                    return exclude.display && value === option.id;
                }

                if (filter.type === 'SUB_RESULTS_ID') {
                    let hasValue = false;
                    for (let i = 0; i < result.SubResults.length; i++) {
                        const subResult = result.SubResults[i];
                        if (subResult.hasOwnProperty('ValidExcludes')) {
                            const subResultExclude = subResult.ValidExcludes.filter(validExclude =>
                                validExclude.name === filter.name)[0];
                            const value = ObjectFunctions.getValueByStringPath(
                                                                subResult, filter.field);

                            if (subResultExclude) {
                                if (subResultExclude.display && value === option.id) {
                                    hasValue = true;
                                    break;
                                }
                            }
                        }
                    }
                    return hasValue;
                }
                let display = false;
                if (exclude) {
                    display = exclude.display;
                }
                return display;
            }
            return true;
        }).length;
        return itemCount;
    }
    generateLabel(filterName, option) {
        let label = option.label;
        if (label.indexOf('##lookup##') !== -1) {
            let lookup = '';
            switch (filterName) {
                case 'resort': {
                    lookup = this.resortName(option.id);
                    break;
                }
                case 'mealBasis': {
                    lookup = this.mealBasisName(option.id);
                    break;
                }
                case 'flightCarrier': {
                    const flightCarrier = this.getFlightCarrier(option.id);
                    lookup = flightCarrier.Name;
                    break;
                }
                case 'flightClass': {
                    lookup = this.flightClassName(option.id);
                    break;
                }
                case 'departureAirport':
                case 'arrivalAirport': {
                    const airport = this.getAirport(option.id);
                    lookup = `${airport.Name} (${airport.IATACode})`;
                    break;
                }
                default:
            }
            label = label.replace('##lookup##', lookup);
        }
        return label;
    }
    toggleFilterDisplay(key) {
        const expanded = ObjectFunctions.getValueByStringPath(
            this.props.filters, `${key}.expanded`);
        this.props.updateValueFunction(`${key}.expanded`, !expanded);
    }
    shouldRenderRangeFilter(filter) {
        let shouldRender = true;
        if (filter.name === 'price'
                && this.props.renderedSearchMode === 'Flight'
                && this.props.isPackageSearch) {
            shouldRender = false;
        }
        return shouldRender;
    }
    renderFilter(filter, index) {
        let input = '';
        switch (filter.type) {
            case 'ID':
            case 'SUB_RESULTS_ID':
                if (filter.options.length > 1) {
                    input = this.renderIdFilter(filter, index);
                }
                break;
            case 'RESULTS_RANGE':
            case 'SUB_RESULTS_RANGE':
                if (this.shouldRenderRangeFilter(filter)) {
                    input = this.renderRangeFilter(filter, index);
                }
                break;
            case 'TEXT':
                input = this.renderTextFilter(filter, index);
                break;
            default:
        }
        return input;
    }
    renderIdFilter(filter, index) {
        const key = `[${index}]`;
        let containerClass = 'filter-type';
        if (filter.expanded) {
            containerClass += ' filter-expanded';
        }
        return (
            <div className={containerClass} key={key}>
                <h3 className="h-tertiary"
                    onClick={() => this.toggleFilterDisplay(key)}>{filter.title}</h3>
                {filter.expanded
                    && filter.options.map((option, optionIndex) =>
                        this.renderFilterOption(option, optionIndex, key, filter), this)}
            </div>
        );
    }
    renderFilterOption(option, index, key, filter) {
        const optionKey = `${key}.options[${index}].selected`;

        const itemCount = this.itemCount(option, filter);
        let disabled = false;

        let containerClass = 'form-check filter-option';
        if (itemCount === 0) {
            disabled = true;
            containerClass = `${containerClass} disabled`;
        }

        const labelAttributes = {
            className: `form-check-label ${filter.labelClass}`,
        };
        labelAttributes.className += !this.props.displayRatingAsStars ? ' no-stars' : '';

        if (filter.labelValueAttribute && this.props.displayRatingAsStars) {
            labelAttributes[`data-${filter.labelValueAttribute}`] = option.id;
        }

        const checkboxInputProps = {
            type: 'checkbox',
            className: 'form-check-input',
            name: optionKey,
            label: option.label,
            labelAttributes,
            onChange: this.props.onChange,
            value: option.selected,
            containerClass,
            key: optionKey,
            disabled,
        };
        if (option.label !== '') {
            return (
                <CheckboxInput {...checkboxInputProps} />
            );
        }
        return null;
    }
    renderRangeFilter(filter, index) {
        const key = `[${index}]`;
        let minMaxPrice = {};
        switch (filter.type) {
            case 'RESULTS_RANGE':
                minMaxPrice = this.getMinMaxPrice();
                break;
            case 'SUB_RESULTS_RANGE':
                minMaxPrice = this.getSubResultsMinMaxPrice();
                break;
            default:
        }
        const priceAdjustment = this.props.priceAdjustment ? this.props.priceAdjustment : 0;
        const appendedRangeText = this.props.pricingConfiguration.PerPersonPricing ? 'pp' : '';
        const sliderProps = {
            name: key,
            minValue: minMaxPrice.minValue,
            maxValue: minMaxPrice.maxValue,
            startMin: filter.minValue,
            startMax: filter.maxValue,
            onChange: this.props.onRangeChange,
            showPriceRange: true,
            rangePriceAdjustment: priceAdjustment,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            appendedRangeText,
            updatingPrice: this.props.updatingPrice,
        };
        let containerClass = 'filter-type';
        if (filter.expanded) {
            containerClass += ' filter-expanded';
        }
        return (
            <div className={containerClass} key={key}>
                <h3 className="h-tertiary"
                    onClick={() => this.toggleFilterDisplay(key)}>{filter.title}</h3>
                {filter.expanded
                    && <SliderInput {...sliderProps} />}
            </div>
        );
    }
    renderTextFilter(filter, index) {
        const key = `[${index}]`;
        const textInputProps = {
            type: 'text',
            className: 'form-control',
            id: `${key}.value`,
            name: `${key}.value`,
            ref: `${key}.value`,
            onChange: this.props.onChange,
            autoComplete: 'off',
            value: filter.value,
        };
        let containerClass = 'filter-type';
        if (filter.expanded) {
            containerClass += ' filter-expanded';
        }
        return (
            <div className={containerClass} key={key}>
                <h3 className="h-tertiary"
                    onClick={() => this.toggleFilterDisplay(key)}>{filter.title}</h3>
                {filter.expanded
                    && <div className="form-group"><input {...textInputProps} /></div>}
            </div>
        );
    }
    render() {
        const displayFilters = this.props.displayFilters;
        const filterClasses = displayFilters ? 'filters-container' : 'hidden-sm-down';
        let menuIconClass = 'filter-menu-icon hidden-md-up fa';
        menuIconClass
            = displayFilters ? `${menuIconClass} fa-minus` : `${menuIconClass} fa-plus`;
        let containerClass = `search-results-filter ${this.props.searchDetails.SearchMode}`;
        containerClass
            = displayFilters ? `${containerClass}` : `${containerClass} filter-collapsed`;
        return (
            <div className={containerClass}>
                <h2 className="h-secondary filter-title"
                onClick={() => { this.props.handleToggleFiltersDisplay(); }}>
                    Filter Results
                    <span className={menuIconClass}></span>
                </h2>
                <div className={filterClasses}>
                    {this.props.filters.map(this.renderFilter, this)}
                </div>
            </div>
        );
    }
}

SearchResultsFilter.propTypes = {
    searchResults: React.PropTypes.array.isRequired,
    searchDetails: React.PropTypes.object.isRequired,
    renderedSearchMode: React.PropTypes.string.isRequired,
    isPackageSearch: React.PropTypes.bool.isRequired,
    filters: React.PropTypes.array.isRequired,

    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    priceAdjustment: React.PropTypes.number,

    countries: React.PropTypes.array.isRequired,
    mealBasis: React.PropTypes.array.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array.isRequired,

    onChange: React.PropTypes.func.isRequired,
    onRangeChange: React.PropTypes.func.isRequired,
    updateValueFunction: React.PropTypes.func.isRequired,
    handleToggleFiltersDisplay: React.PropTypes.func.isRequired,
    displayFilters: React.PropTypes.bool,
    displayRatingAsStars: React.PropTypes.bool,
    updatingPrice: React.PropTypes.bool,
};
