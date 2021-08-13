import 'widgets/bookingjourney/_insuranceUpsell.scss';

import MarkdownText from 'components/common/MarkdownText';
import Price from 'components/common/price';
import RadioInput from 'components/form/radioinput';
import React from 'react';

export default class InsuranceUpsell extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            addingComponent: false,
        };
        this.handleResultSelect = this.handleResultSelect.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (this.props.selectedComponentToken !== nextProps.selectedComponentToken
                || this.props.selectedSubComponentToken !== nextProps.selectedSubComponentToken) {
            this.state.addingComponent = false;
        }
    }
    handleResultSelect(componentToken, subComponentToken) {
        if (!this.state.addingComponent) {
            this.state.addingComponent = true;
            this.props.onResultSelect(componentToken, subComponentToken);
        }
    }
    renderResults() {
        const tableHeadings = this.props.tableHeadings;

        const options = [{
            componentToken: 0,
            subComponentToken: 0,
            price: 0,
            name: 'None',
        }];

        this.props.results.forEach(result => {
            result.SubResults.forEach(subResult => {
                const option = {
                    componentToken: result.ComponentToken,
                    subComponentToken: subResult.ComponentToken,
                    price: subResult.TotalPrice,
                    name: result.DisplayName,
                };
                options.push(option);
            });
        });

        return (
            <div className="table mt-2">
                <div className="table-row">
                    <div className="table-header extra-description">
                        {tableHeadings.Description}
                    </div>
                    <div className="table-header extra-price text-right">
                        {tableHeadings.Price}
                    </div>
                    <div className="table-header extra-select text-right">
                        {tableHeadings.Selected}
                    </div>
                </div>
                {options.map(this.renderOption, this)}
            </div>
        );
    }
    renderOption(option) {
        const key = `extra-option-${option.componentToken}`;
        let containerClass = 'table-row extra-option';
        const isSelected = option.componentToken === this.props.selectedComponentToken
            && option.subComponentToken === this.props.selectedSubComponentToken;
        if (isSelected) {
            containerClass += ' selected';
        }
        const containerProps = {
            key,
            className: containerClass,
            onClick: () => this.handleResultSelect(option.componentToken,
                option.subComponentToken),
        };
        const priceProps = {
            currency: this.props.selectedCurrency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: '',
            appendText: '',
            classes: '',
            amount: option.price,
            displayTotalGroupPrice: false,
            displayPerPersonText: false,
        };
        const radioInputProps = {
            name: 'insurance-select',
            label: '',
            onChange: (event) => {
                event.preventDefault();
                this.handleResultSelect(option.componentToken, option.subComponentToken);
            },
            value: option.subComponentToken,
            checked: isSelected,
            error: '',
            required: false,
            description: '',
        };
        return (
            <div {...containerProps}>
                <div className="table-cell extra-description">
                    <p>{option.name}</p>
                </div>
                <div className="table-cell extra-price text-right">
                    <Price {...priceProps} />
                </div>
                <div className="table-cell extra-select text-right">
                    <RadioInput {...radioInputProps} />
                </div>
            </div>
        );
    }
    renderWaitMessage() {
        return (
            <div className="wait-message-container">
                <i className="loading-icon" aria-hidden="true"></i>
                <div className="wait-message">
                    <p>{this.props.waitMessage}</p>
                </div>
            </div>
        );
    }
    render() {
        const markdownProps = {
            markdown: this.props.description,
            containerStyle: 'mb-2',
        };
        const renderResults = this.props.results.length > 0;
        return (
            <div className="widget-insurance-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">eafsrergh{this.props.title}</h3>
                 </div>
                 <div className="panel-body">
                     {!this.props.isSearching
                        && this.props.description
                        && <MarkdownText {...markdownProps} />}
                     {this.props.isSearching
                        && this.renderWaitMessage()}
                     {renderResults
                        && this.renderResults()}
                 </div>
            </div>
        );
    }
}

InsuranceUpsell.propTypes = {
    title: React.PropTypes.string,
    description: React.PropTypes.string,
    tableHeadings: React.PropTypes.object,
    selectedCurrency: React.PropTypes.object,
    pricingConfiguration: React.PropTypes.object,
    results: React.PropTypes.array,
    selectedComponentToken: React.PropTypes.number,
    selectedSubComponentToken: React.PropTypes.number,
    isSearching: React.PropTypes.bool,
    onResultSelect: React.PropTypes.func,
    waitMessage: React.PropTypes.string,
};
