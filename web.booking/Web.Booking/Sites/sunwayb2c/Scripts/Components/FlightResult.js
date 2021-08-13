import * as SearchConstants from '../../../../scripts/constants/search';
import FlightSector from './flightsector';
import Price from 'components/common/price';
import React from 'react';
import StringFunctions from 'library/stringfunctions';

export default class FlightResult extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            outboundSectorsExpanded: false,
            returnSectorsExpanded: false,
            collapsed: this.props.collapseMobile,
        };
        this.toggleFlightResults = this.toggleFlightResults.bind(this);
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
    getFlightClass(id) {
        let flightClassName = '';
        for (let i = 0; i < this.props.flightClasses.length; i++) {
            const flightClass = this.props.flightClasses[i];
            if (flightClass.Id === id) {
                flightClassName = flightClass.Name;
                break;
            }
        }
        return flightClassName;
    }
    renderFlightSummary(sectors, direction, isExpanded, index) {
        const departingSector = sectors[0];
        const arrivingSector = sectors[sectors.length - 1];
        const departureAirport = this.getAirport(departingSector.DepartureAirportID);
        const arrivalAirport = this.getAirport(arrivingSector.ArrivalAirportID);
        const flightCarrier = this.getFlightCarrier(departingSector.FlightCarrierID);
        const flightClass
            = this.getFlightClass(this.props.result.MetaData[`${direction}FlightClassId`]);

        const flightSectorProps = {
            direction,
            result: this.props.result,
            sectors,
            flightCarrier,
            flightClass,
            arrivalAirport,
            departureAirport,
            logoBaseUrl: this.props.cmsBaseUrl,
            toggleSectors: this.props.onToggleSummary,
            isExpanded,
            index,
            key: index,
        };
        return (
            <FlightSector {...flightSectorProps} />
        );
    }
    renderFlightSectors(direction) {
        const result = this.props.result;
        const metaData = result.MetaData;
        const sectors = metaData.FlightSectors.filter(sector => sector.Direction === direction);
        const expanded
            = result.ExpandedDirections && result.ExpandedDirections.indexOf(direction) > -1;
        const sectorHtml = [];
        if (expanded) {
            for (let i = 0; i < sectors.length; i++) {
                sectorHtml.push(this.renderFlightSummary([sectors[i]], direction, true, i));
            }
        } else {
            sectorHtml.push(this.renderFlightSummary(sectors, direction, false, 0));
        }
        return (sectorHtml);
    }
    getRoomPaxSummary() {
        const guests = {
            adults: 0,
            children: 0,
            infants: 0,
        };
        this.props.rooms.forEach(r => {
            guests.adults += r.Adults;
            guests.children += r.Children;
            guests.infants += r.Infants;
        });
        let paxSummary = StringFunctions.getPluralisationValue(guests.adults, 'Adult', 'Adults');
        paxSummary += guests.children > 0
            ? `, ${StringFunctions.getPluralisationValue(guests.children, 'Child', 'Children')}`
            : '';
        paxSummary += guests.infants > 0
            ? `, ${StringFunctions.getPluralisationValue(guests.infants, 'Infant', 'Infants')}`
            : '';
        return paxSummary;
    }
    renderResultPrice() {
        const result = this.props.result;
        const metaData = this.props.result.MetaData;
        const includedBaggageMessage = 'Includes bags';

        const priceProps = {
            amount: result.Price,
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            classes: 'price h-secondary h-alt mb-1',
        };

        const priceDiffProps = {
            currency: this.props.currency,
            pricingConfiguration: this.props.pricingConfiguration,
            classes: 'price h-secondary h-alt mb-0',
        };

        if (this.props.selectedFlight) {
            const priceDiff = result.Price - this.props.selectedFlight.Price;

            if (priceDiff < 0) {
                priceDiffProps.amount = priceDiff * -1;
                priceDiffProps.prependText = '- ';
                priceDiffProps.prependTextClasses = 'symbol';
            } else if (priceDiff > 0) {
                priceDiffProps.amount = priceDiff;
                priceDiffProps.prependText = '+ ';
                priceDiffProps.prependTextClasses = 'symbol';
            } else {
                priceDiffProps.amount = priceDiff;
            }
        }

        let containerClass = 'result-price';
        if (this.props.changeFlight) {
            containerClass += ' price-diff';
        }
        const flightButtons = this.props.flightButtons;

        const renderCharterPackageText = this.props.charterPackageText
                  && this.props.changeFlight && metaData.Source === 'Own'
                  && this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL;
        const renderCharterFlightText = this.props.charterFlightText && metaData.Source === 'Own'
                && this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT;

        const guests = this.getRoomPaxSummary();

        return (
            <div className={containerClass}>
                {!this.props.changeFlight
                    && <Price {...priceProps} />}

                {this.props.changeFlight
                    && !result.isSelected
                    && <Price {...priceDiffProps} />}
                {guests
                    && <span className="block mt-0 mb-0">{guests}</span>}

                {this.props.result.MetaData.IncludesSupplierBaggage
                    && <p>{includedBaggageMessage}</p>}

                {renderCharterFlightText
                  && <p>{this.props.charterFlightText}</p>}

                {renderCharterPackageText
                  && <p>{this.props.charterPackageText}</p>}

                {!this.props.changeFlight
                    && <button className="btn btn-alt"
                        onClick={() => this.props.onSelect(result.ComponentToken)}>
                        {flightButtons.BookNow}</button>}

                {this.props.changeFlight
                    && !result.isSelected
                    && <button className="btn btn-alt"
                                onClick={() => this.props.onChangeSelect(result.ComponentToken)}>
                        {flightButtons.SelectFlight}</button>}

                {this.props.changeFlight
                    && result.isSelected
                    && this.props.totalResults > 0
                    && !this.props.isChangingFlight
                    && <button className="btn btn-alt btn-change-flight"
                    onClick={this.props.onChangeFlight}>
                        {flightButtons.ChangeSelectedFlight}</button>}

                {this.props.changeFlight
                    && result.isSelected
                    && this.props.isChangingFlight
                    && <button className="btn btn-alt btn-change-flight"
                        onClick={this.props.onChangeFlight}>
                        {flightButtons.Cancel}</button>}
            </div>
        );
    }
    toggleFlightResults() {
        const collapsed = this.state.collapsed;
        this.setState({
            collapsed: !collapsed,
        });
    }
    render() {
        const collapsed = this.state.collapsed;
        let flightCssClass = 'flight-result';
        let containerClass = 'flight-toggle hidden-md-up h-secondary';
        let buttonClass = '';
        if (collapsed) {
            buttonClass = 'hidden-sm-down';
            flightCssClass += ' hidden-sm-down';
            containerClass += ' collapsed';
        }
        const result = this.props.result;
        const flightButtons = this.props.flightButtons;

        return (
            <div className="flight-result-container">
                <div>
                    <h3 className={containerClass}
                        onClick={this.toggleFlightResults}>Flight Details</h3>
                </div>
                <div className={flightCssClass}>
                    <div className="flight-information">
                        <div className="flight-journey flight-outbound">
                            {this.renderFlightSectors('Outbound')}
                        </div>
                        <div className="flight-journey flight-return">
                            {this.renderFlightSectors('Return')}
                        </div>
                    </div>
                    {this.props.displayPrice
                        && !result.isSelected
                        && this.renderResultPrice()}
                </div>
                <div className={buttonClass}>
                    {this.props.changeFlight
                        && result.isSelected
                        && this.props.totalResults > 0
                        && !this.props.isChangingFlight
                        && <button className="btn btn-alt float-right btn-change-flight"
                            onClick={this.props.onChangeFlight}>
                                {flightButtons.ChangeSelectedFlight}</button>}

                    {this.props.changeFlight
                        && result.isSelected
                        && this.props.isChangingFlight
                        && <button className="btn btn-alt float-right btn-change-flight"
                            onClick={this.props.onChangeFlight}>
                                {flightButtons.Cancel}</button>}
                </div>
            </div>
        );
    }
}

FlightResult.propTypes = {
    result: React.PropTypes.object.isRequired,
    airports: React.PropTypes.array.isRequired,
    flightCarriers: React.PropTypes.array.isRequired,
    flightClasses: React.PropTypes.array,
    flightButtons: React.PropTypes.object,
    onSelect: React.PropTypes.func,
    changeFlight: React.PropTypes.bool,
    selectedFlight: React.PropTypes.object,
    isChangingFlight: React.PropTypes.bool,
    onChangeFlight: React.PropTypes.func,
    onChangeSelect: React.PropTypes.func,
    onToggleSummary: React.PropTypes.func,
    cmsBaseUrl: React.PropTypes.string,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    searchMode: React.PropTypes.string,
    charterPackageText: React.PropTypes.string,
    charterFlightText: React.PropTypes.string,
    displayPrice: React.PropTypes.bool,
    showGuests: React.PropTypes.bool,
    rooms: React.PropTypes.array,
    totalResults: React.PropTypes.number,
    collapseMobile: React.PropTypes.bool,
};

FlightResult.defaultProps = {
    displayPrice: true,
};
