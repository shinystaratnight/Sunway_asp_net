import 'widgets/bookingjourney/_transferUpsell.scss';

import MarkdownText from 'components/common/MarkdownText';
import Price from 'components/common/price';
import RadioInput from 'components/form/radioinput';
import React from 'react';
import SelectInput from 'components/form/selectinput';
import TextInput from 'components/form/textinput';

export default class TransferUpsell extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
        this.handleResultSelect = this.handleResultSelect.bind(this);
    }
    validateFlightTimeProps(flightTimeProps, flightTime, direction) {
        const props = Object.assign({}, flightTimeProps);
        const maxLengthWithColon = 5;
        const positionOfColon = 3;
        props.maxLength = maxLengthWithColon;
        let value = flightTime;
        const hoursInDay = 24;
        const minutesInHour = 60;
        if (value.length === 1) {
            value += '0';
        }
        if (parseInt(value.slice(0, 2), 10) >= hoursInDay) {
            value = '23:59';
        }
        const missingColon = value.indexOf(':') === -1;
        if (missingColon) {
            value = `${value.slice(0, 2)}:${value.slice(2)}`;
        }
        if (value.length !== maxLengthWithColon) {
            const additionalText = (maxLengthWithColon - value.length === 2) ? '00' : '0';
            value += additionalText;
        }
        if (parseInt(value.slice(positionOfColon, maxLengthWithColon), 10) >= minutesInHour) {
            value = `${value.slice(0, positionOfColon)}59`;
        }
        props.value = value;
        this.props.updateFlightTime(value, direction);
        return props;
    }
    getOutboundFlightNumberProps() {
        const outbound = this.props.outbound;
        const validateInputs = this.props.validateInputs;
        const outboundFlightNumberError = validateInputs && !outbound.flightNumber
            ? this.props.flightCodeErrorMessage : '';
        const outboundFlightNumberProps = {
            label: 'Flight Number',
            name: 'outboundFlightNumber',
            onChange: (ev) => this.props.handleFlightNumberUpdate(ev.target.value, 'outbound'),
            error: outboundFlightNumberError,
            value: this.props.outbound.flightNumber,
        };
        return outboundFlightNumberProps;
    }
    getOutboundFlightTimeProps() {
        const outbound = this.props.outbound;
        const validateInputs = this.props.validateInputs;
        const outboundFlightTimeError = validateInputs && !outbound.flightTime
            ? this.props.flightTimeErrorMessage : '';
        let outboundFlightTimeProps = {
            label: 'Flight Time',
            name: 'outboundFlightTime',
            formatAsTime: true,
            placeholder: 'HH:MM',
            onChange: (ev) => this.props.handleFlightTimeUpdate(ev.target.value, 'outbound'),
            onBlur: () => this.props.handleFlightTimeFocusChange('outbound', 'blur'),
            onFocus: () => this.props.handleFlightTimeFocusChange('outbound', 'focus'),
            value: outbound.flightTime,
            error: outboundFlightTimeError,
        };
        if (outbound.applyFormatting && outbound.flightTime.length >= 1) {
            outboundFlightTimeProps
                = this.validateFlightTimeProps(outboundFlightTimeProps,
                    outbound.flightTime, 'outbound');
        }
        return outboundFlightTimeProps;
    }
    getInboundFlightNumberProps() {
        const inbound = this.props.inbound;
        const validateInputs = this.props.validateInputs;
        const inboundFlightNumberError = validateInputs && !inbound.flightNumber
            ? this.props.flightCodeErrorMessage : '';
        const inboundFlightNumberProps = {
            label: 'Flight Number',
            name: 'inboundFlightNumber',
            onChange: (ev) => this.props.handleFlightNumberUpdate(ev.target.value, 'inbound'),
            error: inboundFlightNumberError,
            value: this.props.inbound.flightNumber,
        };
        return inboundFlightNumberProps;
    }
    getInboundFlightTimeProps() {
        const inbound = this.props.inbound;
        const validateInputs = this.props.validateInputs;
        const inboundFlightTimeError = validateInputs && !inbound.flightTime
            ? this.props.flightTimeErrorMessage : '';
        let inboundFlightTimeProps = {
            label: 'Flight Time',
            name: 'inboundFlightTime',
            formatAsTime: true,
            placeholder: 'HH:MM',
            onChange: (ev) => this.props.handleFlightTimeUpdate(ev.target.value, 'inbound'),
            onBlur: () => this.props.handleFlightTimeFocusChange('inbound', 'blur'),
            onFocus: () => this.props.handleFlightTimeFocusChange('inbound', 'focus'),
            value: inbound.flightTime,
            error: inboundFlightTimeError,
        };
        if (inbound.applyFormatting && inbound.flightTime.length >= 1) {
            inboundFlightTimeProps
                = this.validateFlightTimeProps(inboundFlightTimeProps,
                    inbound.flightTime, 'inbound');
        }
        return inboundFlightTimeProps;
    }
    getAirportSelectProps() {
        const airportOptions = [];
        this.props.departureAirports.forEach(a => {
            const option = {
                Id: a.Id,
                Name: a.Name,
            };
            airportOptions.push(option);
        });
        const airportSelectProps = {
            name: 'departureAirport',
            options: airportOptions,
            value: this.props.selectedAirport,
            onChange: (event) => this.props.handleDepartureAirportUpdate(event),
        };
        return airportSelectProps;
    }
    handleResultSelect(componentToken) {
        if (!this.props.addingComponent) {
            this.props.addingComponent = true;
            this.props.handleModifyBasket(componentToken);
        }
    }
    renderHotelContent() {
        const renderAirportSelect = this.props.departureAirports.length > 1;
        const airportSelectProps = this.getAirportSelectProps();

        const airport = this.props.departureAirports[0].Name;
        return (
            <div className="transferdetails">
                <div className="row">
                    <div className="col-xs-12">
                        {renderAirportSelect
                            && <div>
                                    <span>{this.props.airportSelectText}</span>
                                    <SelectInput {...airportSelectProps} />
                                </div>}
                        {!renderAirportSelect
                            && <p className="mb-1">
                                {this.props.airportPrependedText} <strong>{airport}</strong>
                               </p>}
                    </div>
                </div>
                <div className="row">
                    <div className="col-xs-12 col-sm-6">
                        <h4>{this.props.outboundText}</h4>
                        <TextInput {...this.getOutboundFlightNumberProps()} />
                        <TextInput {...this.getOutboundFlightTimeProps()} />
                    </div>
                    <div className="col-xs-12 col-sm-6">
                        <h4>{this.props.inboundText}</h4>
                        <TextInput {...this.getInboundFlightNumberProps()} />
                        <TextInput {...this.getInboundFlightTimeProps()} />
                    </div>
                </div>
                <div className="row">
                    <div className="col-xs-12">
                        <a className="btn btn-default "
                        onClick={() => this.props.performSearch()}>
                            {this.props.searchButton}
                        </a>
                    </div>
                </div>
            </div>
        );
    }
    renderBody() {
        let content = '';
        switch (this.props.searchMode) {
            case 'Hotel':
            case 'Transfer':
                content = this.renderHotelContent();
                break;
            default:
        }
        return content;
    }
    renderFailedSearchBody() {
        return (
            <div className="alert alert-warning mb-0 transfer-no-results">
                <i className="alert-icon" aria-hidden="true"></i>
                <div className="alert-message">
                    <p>{this.props.failedSearchMessage}</p>
                 </div>
            </div>
        );
    }
    renderResults() {
        const transferOptions = [];
        const noneResult = {
            ComponentToken: 0,
            Price: 0,
            DisplayName: 'No transfer required',
            MetaData: {
                VehicleName: 'No transfer required',
                Quantity: '-',
            },
        };
        const results = [noneResult].concat(this.props.results);
        results.forEach(r => {
            const transferOption = this.renderResult(r);
            transferOptions.push(transferOption);
        });
        const tableHeadings = this.props.tableHeadings;
        return (
            <div className="table mt-2">
                <div className="table-row">
                    <div className="table-header transfer-vehicle">
                        {tableHeadings.Description}
                    </div>
                    <div className="table-header transfer-price">
                        {tableHeadings.Price}
                    </div>
                    <div className="table-header transfer-quantity">
                        {tableHeadings.Quantity}
                    </div>
                    <div className="table-header transfer-selected">
                        {tableHeadings.Selected}
                    </div>
                </div>
                {transferOptions}
            </div>
        );
    }
    renderResult(result) {
        const radioInputProps = {
            name: 'transfer-select',
            label: '',
            onChange: (event) => this.handleResultSelect(event.target.value),
            value: result.ComponentToken,
            checked: result.ComponentToken === this.props.selectedComponentToken,
            error: '',
            required: false,
            description: '',
        };
        const currency = this.props.selectedCurrency;
        const priceProps = {
            currency,
            pricingConfiguration: this.props.pricingConfiguration,
            prependText: '',
            appendText: '',
            classes: '',
            amount: result.Price,
            displayTotalGroupPrice: false,
            displayPerPersonText: false,
        };
        const key = `transfer-option-${result.ComponentToken}`;

        let containerClass = 'table-row transfer-option';
        if (result.ComponentToken === this.props.selectedComponentToken) {
            containerClass += ' selected';
        }

        let displayQuantity = result.MetaData.Quantity;

        if (result.Price === 0 && result.MetaData.Quantity === 1) {
            displayQuantity = '-';
        }

        const containerProps = {
            key,
            className: containerClass,
            onClick: () => this.handleResultSelect(result.ComponentToken),
        };

        return (
            <div {...containerProps}>
                <div className="table-cell transfer-vehicle">
                    <p>{result.MetaData.VehicleName}</p>
                </div>
                <div className="table-cell transfer-price text-right">
                    <Price {...priceProps} />
                </div>
                <div className="table-cell transfer-quantity text-right">
                    <p>{displayQuantity}</p>
                </div>
                <div className="table-cell transfer-select text-right">
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
        const searchFailed = this.props.searchFailed;
        const haveResults = this.props.results.length > 0;
        const renderResults = haveResults;
        const renderFailedSearch = searchFailed && !haveResults;
        const markdownProps = {
            markdown: this.props.description,
            containerStyle: 'mb-2',
        };
        return (
            <div className="widget-transfer-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">{this.props.title}</h3>
                </div>
                <div className="panel-body">
                    {!this.props.isSearching
                        && !this.props.searchFailed
                        && this.props.description
                        && <MarkdownText {...markdownProps} />}
                    {!this.props.isSearching
                       && !this.props.searchFailed
                       && this.renderBody()}
                    {renderFailedSearch
                        && this.renderFailedSearchBody()}
                    {renderResults
                        && this.renderResults()}
                     {this.props.isSearching
                        && this.renderWaitMessage()}
                </div>
            </div>
        );
    }
}

TransferUpsell.propTypes = {
    title: React.PropTypes.string,
    description: React.PropTypes.string,
    tableHeadings: React.PropTypes.object,
    outboundText: React.PropTypes.string,
    inboundText: React.PropTypes.string,
    airportSelectText: React.PropTypes.string,
    airportPrependedText: React.PropTypes.string,
    searchButton: React.PropTypes.string,
    failedSearchMessage: React.PropTypes.string,
    flightCodeErrorMessage: React.PropTypes.string,
    flightTimeErrorMessage: React.PropTypes.string,
    outbound: React.PropTypes.object,
    inbound: React.PropTypes.object,
    selectedCurrency: React.PropTypes.object,
    pricingConfiguration: React.PropTypes.object,
    performSearch: React.PropTypes.func,
    handleDepartureAirportUpdate: React.PropTypes.func,
    handleFlightNumberUpdate: React.PropTypes.func,
    handleFlightTimeUpdate: React.PropTypes.func,
    updateFlightTime: React.PropTypes.func,
    handleFlightTimeFocusChange: React.PropTypes.func,
    handleModifyBasket: React.PropTypes.func,
    searchMode: React.PropTypes.oneOf(['Hotel', 'FlightPlusHotel', 'Transfer']),
    departureAirports: React.PropTypes.array,
    selectedAirport: React.PropTypes.number,
    validateInputs: React.PropTypes.bool,
    searchFailed: React.PropTypes.bool,
    results: React.PropTypes.array,
    selectedComponentToken: React.PropTypes.number,
    addingComponent: React.PropTypes.bool,
    isSearching: React.PropTypes.bool,
    waitMessage: React.PropTypes.string,
};

TransferUpsell.defaultProps = {
};
