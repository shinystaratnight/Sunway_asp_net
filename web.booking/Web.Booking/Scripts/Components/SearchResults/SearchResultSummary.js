import * as SearchConstants from '../../constants/search';
import React from 'react';
import StringFunctions from 'library/stringfunctions';
import moment from 'moment';

export default class SearchResultSummary extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getPaxSummary(roomCount, adultCount, childCount, infantCount) {
        let paxSummary = roomCount > 1 ? `${roomCount} rooms, ` : '';
        paxSummary += StringFunctions.getPluralisationValue(adultCount, 'Adult', 'Adults');
        paxSummary += childCount > 0
            ? `, ${StringFunctions.getPluralisationValue(childCount, 'Child', 'Children')}` : '';
        paxSummary += infantCount > 0
            ? `, ${StringFunctions.getPluralisationValue(infantCount, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    airportGroupName(airportGroupId) {
        let name = '';
        if (this.props.airportResortGroups) {
            const airportGroup = this.props.airportResortGroups.find(group =>
                group.AirportGroupId === airportGroupId);
            if (airportGroup) {
                name = airportGroup.AirportGroupName;
            }
        }
        return name;
    }
    regionName(regionId) {
        let name = '';
        for (let i = 0; i < this.props.countries.length; i++) {
            const country = this.props.countries[i];
            for (let j = 0; j < country.Regions.length; j++) {
                const region = country.Regions[j];
                if (region.Id === regionId) {
                    name = region.Name;
                    break;
                }
            }
            if (name !== '') {
                break;
            }
        }
        return name;
    }
    resortName(resortId) {
        let name = '';
        for (let i = 0; i < this.props.countries.length; i++) {
            const country = this.props.countries[i];
            for (let j = 0; j < country.Regions.length; j++) {
                const region = country.Regions[j];
                for (let k = 0; k < region.Resorts.length; k++) {
                    const resort = region.Resorts[k];
                    if (resort.Id === resortId) {
                        name = resort.Name;
                        break;
                    }
                }
            }
            if (name !== '') {
                break;
            }
        }
        return name;
    }
    renderResultCount() {
        let resultCount = '';
        switch (this.props.searchMode) {
            case SearchConstants.SEARCH_MODES.HOTEL:
                resultCount = this.hotelResultCount();
                break;
            case SearchConstants.SEARCH_MODES.FLIGHT:
                resultCount = this.flightResultCount();
                break;
            default:
        }
        return (
            <h2 className="h-secondary">{resultCount}</h2>
        );
    }
    hotelResultCount() {
        const searchDetails = this.props.searchDetails;
        let location = '';
        switch (searchDetails.ArrivalType) {
            case 'region':
                location = this.regionName(searchDetails.ArrivalId);
                break;
            case 'resort':
                location = this.resortName(searchDetails.ArrivalId);
                break;
            case 'airportgroup':
                location = this.airportGroupName(searchDetails.ArrivalId);
                break;
            default:
        }

        const component = this.props.totalResults > 1 ? 'hotels' : 'hotel';

        const property = this.props.totalResults > 1 ? 'properties' : 'property';

        const resultCount = this.props.headerFormat
            .replace('#count#', this.props.totalResults)
            .replace('#component#', component)
            .replace('#property#', property)
            .replace('#location#', location);

        return resultCount;
    }
    flightResultCount() {
        const component = this.props.totalResults > 1 ? 'flights' : 'flight';
        const alternative = this.props.changeFlight ? 'alternative ' : '';
        const resultCount = this.props.headerFormat.includes('#count#')
            ? `${this.props.totalResults} ` : '';
        const summary = `${resultCount}${alternative} ${component}`;

        return summary;
    }
    renderSearchSummary() {
        const searchDetails = this.props.searchDetails;
        const departureDate = moment(searchDetails.DepartureDate).format('DD MMMM Y');
        const returnDate = moment(searchDetails.DepartureDate)
            .add(searchDetails.Duration, 'days').format('DD MMMM Y');
        const duration = `${searchDetails.Duration} nights`;

        const roomCount = searchDetails.Rooms.length;
        let adultCount = 0;
        let childCount = 0;
        let infantCount = 0;

        for (let i = 0; i < searchDetails.Rooms.length; i++) {
            const room = searchDetails.Rooms[i];
            adultCount += room.Adults;
            childCount += room.Children;
            infantCount += room.Infants;
        }

        const pax = this.getPaxSummary(roomCount, adultCount, childCount, infantCount);
        const searchSummary = `For ${pax}, ${duration} (${departureDate} - ${returnDate})`;

        return (
            <p className="search-summary">{searchSummary}</p>
        );
    }
    render() {
        const summaryClass = `search-result-summary ${this.props.searchDetails.SearchMode}`;
        return (
            <div className={summaryClass}>
                {this.renderResultCount()}
                {this.renderSearchSummary()}
            </div>
        );
    }
}

SearchResultSummary.propTypes = {
    searchDetails: React.PropTypes.object.isRequired,
    totalResults: React.PropTypes.number.isRequired,
    searchMode: React.PropTypes.string.isRequired,
    countries: React.PropTypes.array.isRequired,
    changeFlight: React.PropTypes.bool,
    headerFormat: React.PropTypes.string,
    airportResortGroups: React.PropTypes.array,
};

SearchResultSummary.defaultProps = {
    headerFormat: '#count# #component# in #location#',
};
