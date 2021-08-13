import * as SearchConstants from 'constants/search';
import React from 'react';

export default class DestinationInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            expanded: false,
            gridElements: [],
            selectedDestinationGroup: {},
        };
        this.toggleExpanded = this.toggleExpanded.bind(this);
        this.handleClick = this.handleClick.bind(this);
        this.addDestinationGroup = this.addDestinationGroup.bind(this);
        this.addDestination = this.addDestination.bind(this);
        this.renderElement = this.renderElement.bind(this);
        this.destinationSort = this.destinationSort.bind(this);
        this.renderSelectedGroup = this.renderSelectedGroup.bind(this);
        this.updateSelectedDestinationGroup = this.updateSelectedDestinationGroup.bind(this);
        this.getDestinationString = this.getDestinationString.bind(this);
        this.getDestinationName = this.getDestinationName.bind(this);
        this.handleDestinationClick = this.handleDestinationClick.bind(this);
    }
    handleClick(event) {
        let containerClicked = false;
        for (let element = event.target; element; element = element.parentNode) {
            if (element.id === 'destinations-container') {
                containerClicked = true;
                break;
            } else if (element.classList && element.classList.contains('see-more')) {
                containerClicked = true;
                break;
            } else if (element.classList && element.classList.contains('destination')
                && element.classList.contains('valid')) {
                break;
            } else if (element.classList && element.classList.contains('destination-close')) {
                break;
            } else if (element.classList && element.classList.contains('destination-back')) {
                containerClicked = true;
                this.setState({ selectedDestinationGroup: {} });
                break;
            }
        }
        if (containerClicked !== this.state.expanded) {
            this.setState({ expanded: containerClicked });
        }
    }
    handleDestinationClick(native, destination) {
        let useDealFinder = false;
        if (!destination.ShowAllDatesAndDurations && destination.URL === '') {
            useDealFinder = true;
        }
        let after;
        native.persist();
        if (destination.hasOwnProperty('Region')) {
            after = () => { this.props.onRegionClick(native); };
        } else if (destination.hasOwnProperty('Airport')) {
            after = () => { this.props.onAirportClick(native); };
        }
        let brandOverride = 0;
        let searchURL = '';
        if (destination.hasOwnProperty('URL')) {
            searchURL = destination.URL;
        }
        if (destination.hasOwnProperty('Brand Override')) {
            brandOverride = destination['Brand Override'];
        }
        this.props.updatePropertyFunction('SearchURL', searchURL);
        this.props.updatePropertyFunction('BrandOverride', brandOverride);
        this.props.setUseDealFinder(useDealFinder, after);
    }
    toggleExpanded() {
        const expanded = this.props.searchDetails.DepartureId ? !this.state.expanded : false;
        if (expanded) {
            window.addEventListener('click', this.handleClick);
        } else {
            window.removeEventListener('click', this.handleClick);
        }
        this.setState({ expanded, selectedDestinationGroup: {} });
    }
    updateSelectedDestinationGroup(destinationGroup) {
        return (previousState) => {
            const state = Object.assign({}, ...previousState);
            return { state, selectedDestinationGroup: destinationGroup };
        };
    }
    renderDestinations() {
        let destinations = [];
        if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL) {
            destinations = this.props.packageDestinations;
        } else if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            destinations = this.props.flightDestinations;
        }
        let groupSelected = false;
        let selectedGroupParent;
        destinations.sort(this.destinationSort);
        if (this.state.selectedDestinationGroup.hasOwnProperty('Destinations')
            && this.state.selectedDestinationGroup.Destinations.length > 0) {
            groupSelected = true;
            for (let i = 0; i < destinations.length; i++) {
                for (let j = 0; j < destinations[i].DestinationGroups.length; j++) {
                    if (JSON.stringify(destinations[i].DestinationGroups[j])
                        === JSON.stringify(this.state.selectedDestinationGroup)) {
                        selectedGroupParent = destinations[i];
                    }
                }
            }
        }
        return (
            <div className="destinations-content">
                {groupSelected
                    && this.renderSelectedGroup(this.state.selectedDestinationGroup,
                        selectedGroupParent)}
                {!groupSelected
                    && destinations.map(this.renderDestinationGroupList, this, destinations)}
            </div>
        );
    }
    renderDestinationGroupList(destination, index, destinationsList) {
        const colNumber = index + 1;
        this.state.gridElements = [];
        let destinationType = 'Package';
        if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            destinationType = 'Flight';
        }
        const title = destination[`${destinationType} Type Display Name`];
        const tagLine = destination['Tag Line'];
        const titleStyle = {
            color: destination['Font Colour Code'],
        };
        destination.DestinationGroups.sort(this.destinationSort);
        destination.DestinationGroups.map(
            this.addDestinationGroup, this, destination.DestinationGroups);

        const mobileRowNum = Math.ceil(this.state.gridElements.filter(
            o => !o.mobileHide).length / 2);
        const destinationClassName = `destination-groups mobile-row-${mobileRowNum}`;
        const className = `destination-group-list col-${colNumber}`;
        const lastGroup = destinationsList.length - 1 === index;
        const closeClass = 'destination-close-container';
        let titleClass = 'destination-title';
        if (lastGroup) {
            titleClass = `${titleClass} last`;
        }
        return (
            <div key={className} className={className}>
                <div className={titleClass}>
                    {title !== ''
                        && <h2 className="package-title" style={titleStyle}>{title}</h2>}
                    <p className="clearFix">{tagLine}</p>
                </div>
                {lastGroup
                    && <div className={closeClass}>
                        <a className="destination-close fa-times"></a>
                    </div>}
                <div className={destinationClassName}>
                    {this.state.gridElements.map(this.renderElement, this)}
                </div>
            </div>
        );
    }
    addDestinationGroup(destinationGroup, index, groupList, propertyOverrides) {
        const propOverrides = propertyOverrides || {};
        const maxItems = propOverrides.hasOwnProperty('maxItems')
            ? propOverrides.maxItems : destinationGroup['Number of Items to Display'];
        let blankLines = destinationGroup['Blank Rows Below (Desktop Only)']
            ? destinationGroup['Blank Rows Below (Desktop Only)'] : 1;

        if (propOverrides.hasOwnProperty('blankLines')) {
            blankLines = propOverrides.blankLines;
        }
        const lastGroup = groupList.length - 1 === index;
        const titleStyle = {
            color: destinationGroup['Font Colour Code'],
        };
        this.state.gridElements.push({
            type: 'h3',
            value: destinationGroup['Destination Group Name'],
            style: titleStyle,
        });
        let destinationList = destinationGroup.Destinations;
        destinationList.sort(this.destinationSort);
        const chopDestinations = maxItems > 0 && destinationList.length > maxItems;
        if (chopDestinations) {
            destinationList = destinationList.slice(0, maxItems);
        }
        destinationList.map(this.addDestination);
        if (chopDestinations) {
            this.state.gridElements.push({
                type: 'a',
                value: 'See More >',
                class: 'see-more',
                onClick: () => this.setState({ selectedDestinationGroup: destinationGroup }),
            });
        }

        if (blankLines > 0) {
            for (let i = 0; i < blankLines; i++) {
                this.state.gridElements.push({
                    class: !lastGroup && i === 0 ? 'clearFix' : 'hidden-sm-down clearFix',
                    mobileHide: lastGroup || i !== 0,
                });
            }
        }
    }
    addDestination(destination) {
        let valid = true;
        let itemValue;
        let destinationName = destination['Display Name'];
        let onClick;
        if (destination.hasOwnProperty('Region')) {
            if (destination.Region > 0) {
                valid = this.props.checkValidRegion(
                    this.props.searchDetails.DepartureId, destination.Region);
                itemValue = destination.Region * -1;
            } else if (!destination.URL) {
                valid = false;
            }
        } else if (destination.hasOwnProperty('Airport')) {
            if (destination.Airport > 0) {
                valid = this.props.checkValidAirport(
                    this.props.searchDetails.DepartureId, destination.Airport);
                const adapter = SearchConstants.ID_ADAPTERS.filter(ida => ida.type
                    === 'airport')[0];
                itemValue = adapter.shift(destination.Airport);
            } else if (!destination.URL) {
                valid = false;
            }
        }
        if (destinationName === '') {
            destinationName = this.getDestinationName(destination);
        }

        if (valid) {
            onClick = (ev) => this.handleDestinationClick(ev, destination);
        }
        if (destinationName !== '') {
            this.state.gridElements.push({
                type: 'a',
                value: destinationName,
                class: valid ? 'valid' : 'invalid',
                onClick,
                itemValue,
                name: 'ArrivalId',
            });
        }
    }
    destinationSort(a, b) {
        const bigNumber = 999;
        const aSequence = a.Sequence > 0 ? a.Sequence : bigNumber;
        const bSequence = b.Sequence > 0 ? b.Sequence : bigNumber;

        if (aSequence > bSequence) {
            return 1;
        }
        if (aSequence < bSequence) {
            return -1;
        }
        return 0;
    }
    renderElement(element, index) {
        const maxRows = 18;
        const rowNumber = ((index + maxRows) % maxRows) + 1;
        const colNumber = (parseInt(index / maxRows, 10) + 1);
        let component = {};
        let className = `destination row-${rowNumber} col-${colNumber}`;
        if (element.class && element.class !== '') {
            className = `${className} ${element.class}`;
        }
        if (element.type === 'h3') {
            component = <h3 key={className}
                className={className}
                style={element.style}>{element.value}</h3>;
        } else if (element.type === 'a') {
            component = <a key={className}
                className={className}
                name={element.name}
                onClick={element.onClick}
                value={element.itemValue}>{element.value}</a>;
        } else {
            component = <p key={className} className={className}>{element.value}</p>;
        }
        return (component);
    }
    renderSelectedGroup(destinationGroup, parent) {
        this.state.gridElements = [];
        const groupList = [];
        groupList.push(destinationGroup);
        const propOverrides = {
            maxItems: 0,
            blankLines: 0,
        };
        this.addDestinationGroup(destinationGroup, 0, groupList, propOverrides);
        let destinationType = 'Package';
        if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
            destinationType = 'Flight';
        }
        const title = parent[`${destinationType} Type Display Name`];
        const tagLine = parent['Tag Line'];
        const titleStyle = {
            color: parent['Font Colour Code'],
        };

        const mobileRowNum = Math.ceil(this.state.gridElements.filter(
            o => !o.mobileHide).length / 2);
        const destinationClassName = `destination-groups mobile-row-${mobileRowNum} selected-group`;
        const className = 'destination-group-list col-1';

        return (
            <div className={className}>
                <div className="destination-back-container">
                    <a className="destination-back fa-arrow-left"></a>
                </div>
                <div className="destination-title selected">
                    {title !== ''
                        && <h2 className="package-title" style={titleStyle}>{title}</h2>}
                    <p className="clearFix">{tagLine}</p>
                </div>
                <div className="destination-close-container">
                    <a className="destination-close fa-times"></a>
                </div>
                <div className={destinationClassName}>
                    {this.state.gridElements.map(this.renderElement, this)}
                </div>
            </div>
        );
    }
    getDestinationName(destination) {
        let destinationName = '';
        if (destination.hasOwnProperty('Region')) {
            const region = this.props.getRegion(destination.Region);
            destinationName = region.Name ? region.Name : '';
        } else if (destination.hasOwnProperty('Airport')) {
            const airport = this.props.getAirport(destination.Airport);
            destinationName = airport.Name ? airport.Name : '';
        }
        return destinationName;
    }
    getDestinationString() {
        let destinationString = this.props.placeholder;
        const arrivalType = this.props.searchDetails.ArrivalType;
        const searchValue = this.props.searchDetails.SearchURL
            ? this.props.searchDetails.SearchURL
            : this.props.searchDetails.ArrivalId;
        let destinations = [];
        let property = '';
        if (searchValue && searchValue !== 0) {
            if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT_PLUS_HOTEL) {
                if (arrivalType === 'region' || this.props.searchDetails.SearchURL) {
                    destinations = this.props.packageDestinations;
                    property = this.props.searchDetails.SearchURL
                        ? 'URL' : 'Region';
                }
            } else if (this.props.searchMode === SearchConstants.SEARCH_MODES.FLIGHT) {
                if (arrivalType === 'airport' || this.props.searchDetails.SearchURL) {
                    destinations = this.props.flightDestinations;
                    property = this.props.searchDetails.SearchURL
                        ? 'URL' : 'Airport';
                }
            }
        }
        for (let i = 0; i < destinations.length; i++) {
            for (let j = 0; j < destinations[i].DestinationGroups.length; j++) {
                const group = destinations[i].DestinationGroups[j];
                for (let k = 0; k < group.Destinations.length; k++) {
                    if (group.Destinations[k][property] === searchValue) {
                        destinationString = group.Destinations[k]['Display Name'];
                        if (destinationString === '') {
                            destinationString = this.getDestinationName(group.Destinations[k]);
                        }
                        break;
                    }
                }
            }
        }
        return destinationString;
    }
    render() {
        const destinationClasses = 'form-group destinations-container';
        const destinationString = this.getDestinationString();
        return (
            <div id="destinations-container" className={destinationClasses}>
                {this.props.label
                    && <label>{this.props.label}</label>}
                <div className="destinations"
                    onClick={this.toggleExpanded}>{destinationString}</div>
                {this.state.expanded
                    && this.renderDestinations()}
            </div>
        );
    }
}

DestinationInput.propTypes = {
    label: React.PropTypes.string,
    placeholder: React.PropTypes.string,
    packageDestinations: React.PropTypes.array,
    flightDestinations: React.PropTypes.array,
    searchMode: React.PropTypes.oneOf(['Flight', 'FlightPlusHotel']),
    checkValidAirport: React.PropTypes.func,
    checkValidRegion: React.PropTypes.func,
    searchDetails: React.PropTypes.object.isRequired,
    onRegionClick: React.PropTypes.func,
    onAirportClick: React.PropTypes.func,
    getAirport: React.PropTypes.func,
    getRegion: React.PropTypes.func,
    setUseDealFinder: React.PropTypes.func,
    updatePropertyFunction: React.PropTypes.func,
};

DestinationInput.defaultProps = {
    placeholder: 'Going To',
};
