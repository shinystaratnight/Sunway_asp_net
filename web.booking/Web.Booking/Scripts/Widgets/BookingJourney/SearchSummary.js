import '../../../styles/widgets/bookingjourney/_searchSummary.scss';

import React from 'react';
import StringFunctions from 'library/stringfunctions';
import moment from 'moment';

export default class SearchSummary extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getRoomPaxSummary() {
        const room = this.props.rooms[0];
        let paxSummary = StringFunctions.getPluralisationValue(room.Adults, 'Adult', 'Adults');
        paxSummary += room.Children > 0
            ? `, ${StringFunctions.getPluralisationValue(room.Children, 'Child', 'Children')}` : '';
        paxSummary += room.Infants > 0
            ? `, ${StringFunctions.getPluralisationValue(room.Infants, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    renderMultiRoomPaxSummary() {
        const roomCount = this.props.rooms.length;
        let paxTotal = 0;
        this.props.rooms.forEach(room => {
            paxTotal += room.Adults;
            paxTotal += room.Children;
            paxTotal += room.Infants;
        });
        return (
            `${paxTotal} guests in ${roomCount} rooms`
        );
    }
    render() {
        const searchDetails = this.props.search.searchDetails;
        const rooms = this.props.rooms;
        let paxSummary;
        if (rooms && rooms.length > 1) {
            paxSummary = this.renderMultiRoomPaxSummary();
        } else if (rooms) {
            paxSummary = this.getRoomPaxSummary();
        }
        const departureDate = moment(searchDetails.DepartureDate).format('DD MMMM Y');
        const arrivalDate = moment(searchDetails.DepartureDate)
            .add(searchDetails.Duration, 'days').format('DD MMMM Y');
        const dateSearch = `${departureDate} - ${arrivalDate} (${searchDetails.Duration} Nights)`;
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h3 className="h-tertiary">Search Details</h3>
                </header>
                <div className="panel-body search-summary">
                    <p className="font-weight-bold">{paxSummary}</p>
                    <p>{dateSearch}</p>
                </div>
            </div>
        );
    }
}

SearchSummary.propTypes = {
    rooms: React.PropTypes.array.isRequired,
    search: React.PropTypes.object.isRequired,
};
