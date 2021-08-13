import React from 'react';

export default class MoreDestinations extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            expanded: false,
            gridElements: [],
            selectedDestinationGroup: {},
        };
        this.addDestinationGroup = this.addDestinationGroup.bind(this);
        this.addDestination = this.addDestination.bind(this);
        this.renderElement = this.renderElement.bind(this);
        this.destinationSort = this.destinationSort.bind(this);
    }
    renderDestinations() {
        let groupSelected = false;
        if (this.state.selectedDestinationGroup.hasOwnProperty('Destinations')
            && this.state.selectedDestinationGroup.Destinations.length > 0) {
            groupSelected = true;
        }
        return (
            <div className="destinations-content">
                {groupSelected
                    && this.renderSelectedGroup(this.state.selectedDestinationGroup)}
                {!groupSelected
                    && this.renderDestinationGroupList()}
            </div>
        );
    }
    renderDestinationGroupList() {
        const destinations = this.props.moreDestinations.DestinationGroups;
        this.state.gridElements = [];
        destinations.sort(this.destinationSort);
        destinations.map(
            this.addDestinationGroup, this, destinations);

        const mobileRowNum = Math.ceil(this.state.gridElements.filter(
            o => !o.mobileHide).length / 2);
        const destinationClassName = `destination-groups more mobile-row-${mobileRowNum}`;
        const className = 'destination-group-list col-1';
        return (
            <div className={className}>
                <div className={destinationClassName}>
                    {this.state.gridElements.map(this.renderElement, this)}
                </div>
            </div>
        );
    }
    renderSelectedGroup(destinationGroup) {
        this.state.gridElements = [];
        const groupList = [];
        groupList.push(destinationGroup);
        const propOverrides = {
            maxItems: 0,
            blankLines: 0,
        };
        this.addDestinationGroup(destinationGroup, 0, groupList, propOverrides);
        const mobileRowNum = Math.ceil(this.state.gridElements.filter(
            o => !o.mobileHide).length / 2);
        const destinationClassName
            = `destination-groups more mobile-row-${mobileRowNum} selected-group`;
        const className = 'destination-group-list col-1';
        const onClick = () => this.setState({ selectedDestinationGroup: {} });
        return (
            <div className={className}>
                <div className="destination-back-container">
                    <a className="destination-back fa-arrow-left"
                        onClick={onClick}></a>
                </div>
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
        let itemValue;
        let onClick;
        const destinationName = destination['Display Name'];
        if (destination.URL && destination.URL !== '') {
            onClick = () => { window.location = destination.URL; };
        }
        if (destinationName !== '') {
            this.state.gridElements.push({
                type: 'a',
                value: destinationName,
                onClick,
                itemValue,
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
            component = <h3 className={className} style={element.style}>{element.value}</h3>;
        } else if (element.type === 'a') {
            component = <a className={className} name={element.name} onClick={element.onClick}
                value={element.itemValue}>{element.value}</a>;
        } else {
            component = <p className={className}>{element.value}</p>;
        }
        return (component);
    }
    render() {
        const destinationClasses = 'form-group more-destinations-container';
        return (
            <div id="destinations-container" className={destinationClasses}>
                {this.props.expanded
                    && this.renderDestinations()}
            </div>
        );
    }
}

MoreDestinations.propTypes = {
    moreDestinations: React.PropTypes.object,
    expanded: React.PropTypes.bool,
};
