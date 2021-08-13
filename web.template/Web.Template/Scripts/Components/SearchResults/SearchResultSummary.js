import * as SearchConstants from '../../constants/search';
import React from 'react';
import moment from 'moment';

export default class SearchResultSummary extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
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
                for (let k = 0; k < region.Resorts; k++) {
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
            default:
        }

        const component = this.props.totalResults > 1 ? 'hotels' : 'hotel';

        const resultCount = `${this.props.totalResults} ${component} in ${location}`;
        return resultCount;
    }
    flightResultCount() {
        const component = this.props.totalResults > 1 ? 'flights' : 'flight';
        const alternative = this.props.changeFlight ? 'alternative ' : '';
        const resultCount = `${this.props.totalResults} ${alternative} ${component}`;
        return resultCount;
    }
    renderSearchSummary() {
        const searchDetails = this.props.searchDetails;
        const departureDate = moment(searchDetails.DepartureDate).format('ll');
        const returnDate = moment(searchDetails.DepartureDate)
            .add(searchDetails.Duration, 'days').format('ll');
        const duration = `${searchDetails.Duration} nightsss`;

        let adultCount = 0;
        let childCount = 0;
        let infantCount = 0;

        for (let i = 0; i < searchDetails.Rooms.length; i++) {
            const room = searchDetails.Rooms[i];
            adultCount += room.Adults;
            childCount += room.Children;
            infantCount += room.Infants;
        }

        let pax = searchDetails.Rooms.length > 1 ? `${searchDetails.Rooms.length} rooms, ` : '';
        pax = adultCount > 0 ? `${pax} ${adultCount} adults, ` : `${pax}`;
        pax = childCount > 0 ? `${pax} ${childCount} children, ` : `${pax}`;
        pax = infantCount > 0 ? `${pax} ${infantCount} infants, ` : `${pax}`;
        const searchSummary = `For ${pax} ${duration} (${departureDate} - ${returnDate})`;

        return (
            <p className="search-summary">{searchSummary}</p>
        );
    }
    render() {
        return (
            <div className="search-result-summary">
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
};
