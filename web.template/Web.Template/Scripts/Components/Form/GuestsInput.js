import '../../../styles/components/_guestsinput.scss';
import React from 'react';
import SelectInput from './selectinput';

export default class GuestsInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            expanded: false,
        };
        this.addRoom = this.addRoom.bind(this);
        this.removeRoom = this.removeRoom.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.toggleExpanded = this.toggleExpanded.bind(this);
        this.renderChildAge = this.renderChildAge.bind(this);
        this.handleClick = this.handleClick.bind(this);
    }
    addRoom(event) {
        event.stopPropagation();
        const rooms = this.props.rooms;
        const room = {
            Id: this.props.rooms.length + 1,
            Adults: 2,
            Children: 0,
            Infants: 0,
            ChildAges: [],
        };
        rooms.push(room);
        if (this.props.onChange) {
            this.props.onChange(rooms);
        }
    }
    removeRoom(removedRoom) {
        const rooms = this.props.rooms;
        const updatedRooms = [];
        for (let i = 0; i < rooms.length; i++) {
            const room = rooms[i];
            if (room.Id !== removedRoom.Id) {
                room.Id = updatedRooms.length + 1;
                updatedRooms.push(room);
            }
        }
        if (this.props.onChange) {
            this.props.onChange(updatedRooms);
        }
    }
    renderChildAge(childAgeItem, index) {
        const key = `childage_${childAgeItem.roomId}_${index}`;
        const ageProps = {
            key,
            name: key,
            optionsRangeMin: this.props.minChildAge,
            optionsRangeMax: this.props.maxChildAge,
            onChange: this.handleChange,
            value: childAgeItem.age,
        };

        return (
            <SelectInput {...ageProps} />
        );
    }
    totalGuests() {
        let totalGuests = 0;
        this.props.rooms.forEach(room => {
            totalGuests = totalGuests + parseInt(room.Adults, 10)
                + parseInt(room.Children, 10) + parseInt(room.Infants, 10);
        });
        return totalGuests;
    }
    handleChange(event) {
        const name = event.target.name;
        let value = event.target.value;

        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }

        const roomNumber = name.split('_')[1];
        const field = name.split('_')[0];

        const rooms = this.props.rooms;
        const room = rooms[roomNumber - 1];

        if (field === 'childage') {
            const childIndex = name.split('_')[2];
            room.ChildAges[childIndex] = value;
        } else {
            room[field] = value > -1 ? value : 0;
        }
        if (this.props.showChildAges && field === 'Children') {
            const minChildAge = 2;
            while (room.ChildAges.length < value) {
                room.ChildAges.push(minChildAge);
            }
            while (room.ChildAges.length > value) {
                room.ChildAges.pop();
            }
        }

        if (this.props.onChange) {
            this.props.onChange(rooms);
        }
    }
    handleClick(event) {
        let containerClicked = false;
        for (let element = event.target; element; element = element.parentNode) {
            if (element.id === 'guests-container') {
                containerClicked = true;
                break;
            }
        }
        if (containerClicked !== this.state.expanded) {
            this.setState({ expanded: containerClicked });
        }
    }
    toggleExpanded() {
        const expanded = !this.state.expanded;
        if (expanded) {
            window.addEventListener('click', this.handleClick);
        } else {
            window.removeEventListener('click', this.handleClick);
        }
        this.setState({ expanded });
    }
    getGuestsProps(type, room) {
        const guestPropsNameId = room.Id ? room.Id : room.id;
        const guestsProps = {
            onChange: this.handleChange,
            name: `${type}_${guestPropsNameId}`,
        };

        const options = this.props[`${type}Options`];
        guestsProps.value = room[`${type}`];

        if (options) {
            guestsProps.options = options;
        } else {
            guestsProps.optionsRangeMin = type === 'Adults' ? 1 : 0;
            guestsProps.optionsRangeMax = this.props.maxGuestsPerType;
        }

        return guestsProps;
    }
    renderRoom(room) {
        const adultProps = this.getGuestsProps('Adults', room);
        const childProps = this.getGuestsProps('Children', room);
        const infantProps = this.getGuestsProps('Infants', room);
        const rows = [];
        if (adultProps.options && adultProps.options.length <= 2) {
            adultProps.options = adultProps.options.filter(option => option.Id !== -1);
        }
        if (childProps.options && childProps.options.length <= 2) {
            childProps.options = childProps.options.filter(option => option.Id !== -1);
        }
        if (infantProps.options && infantProps.options.length <= 2) {
            infantProps.options = infantProps.options.filter(option => option.Id !== -1);
        }
        rows.push(
            <tr key={room.Id}>
                {this.props.showRooms
                    && <td className="guests-label">Room {room.Id}</td>}
                <td className="guests-input">
                    <SelectInput {...adultProps} />
                </td>
                <td className="guests-input">
                    <SelectInput {...childProps} />
                </td>
                <td className="guests-input">
                    <SelectInput {...infantProps} />
                </td>
                {this.props.showRooms
                    && <td className="guests-remove-room text-right">
                        {room.Id > 1
                            && <span
                                className="guests-remove-icon"
                                onClick={(event) => {
                                    event.stopPropagation();
                                    this.removeRoom(room);
                                }}>
                            </span>}
                    </td>}
            </tr>);
        if (this.props.showChildAges && room.Children > 0) {
            const childAgeItems = [];
            for (let i = 0; i < room.Children; i++) {
                const childAgeItem = {
                    roomId: room.Id,
                    age: room.ChildAges[i] ? room.ChildAges[i] : 2,
                };
                childAgeItems.push(childAgeItem);
            }
            const childAges = <tr>
                <td>
                    Child Ages
                    <div className="info-tooltip fa fa-info-circle">
                        <div className="info-tooltip-text">
                            {this.props.childAgeText}
                        </div>
                    </div>
                </td>
                <td className="guest-input-ages" colSpan="4">
                    {childAgeItems.map(this.renderChildAge, this)}
                </td>
            </tr>;
            rows.push(childAges);
        }
        return rows;
    }
    render() {
        let guestClasses = 'form-group guests-container';
        guestClasses = this.props.showRooms
            ? guestClasses
            : `${guestClasses} hide-rooms`;
        const rooms = this.props.rooms.length;
        const roomText = rooms > 0 && this.props.roomText !== ''
            ? `${rooms} ${this.props.roomText} ` : '';
        const totalGuests = this.totalGuests();
        const guestsString = totalGuests > 0
            ? `${roomText}${totalGuests} ${this.props.guestText}`
            : this.props.placeholder;
        return (
            <div id="guests-container" className={guestClasses}>
                {this.props.label
                    && <label>{this.props.label}</label>}
                <div className="guests"
                    onClick={this.toggleExpanded}>{guestsString}</div>

                {this.state.expanded
                    && <div className="guests-option">
                        <table className="guests-table">
                            <thead>
                                <tr>
                                    {this.props.showRooms
                                        && <th className="guests-label">Room</th>}
                                    <th>Adults</th>
                                    <th>Children</th>
                                    <th>Infants</th>
                                    {this.props.showRooms
                                        && <th></th>}
                                </tr>
                            </thead>
                            <tbody>
                                {this.props.rooms.map(this.renderRoom, this)}
                            </tbody>
                        </table>
                        {this.props.showRooms
                            && this.props.rooms.length < this.props.maxRooms
                            && <span className="guests-add" onClick={this.addRoom}>Add Room</span>}
                    </div>}
            </div>
        );
    }
}

GuestsInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    placeholder: React.PropTypes.string,
    guestText: React.PropTypes.string,
    rooms: React.PropTypes.array.isRequired,
    minChildAge: React.PropTypes.number,
    maxChildAge: React.PropTypes.number,
    maxGuestsPerType: React.PropTypes.number,
    maxRooms: React.PropTypes.number,
    onChange: React.PropTypes.func,
    showRooms: React.PropTypes.bool,
    showChildAges: React.PropTypes.bool,
    AdultsOptions: React.PropTypes.array,
    ChildrenOptions: React.PropTypes.array,
    InfantsOptions: React.PropTypes.array,
    childAgeText: React.PropTypes.string,
    roomText: React.PropTypes.string,
};

GuestsInput.defaultProps = {
    placeholder: 'Select Guests',
    guestText: 'Guests',
    minChildAge: 2,
    maxChildAge: 17,
    maxGuestsPerType: 9,
    maxRooms: 4,
    showRooms: true,
    showChildAges: true,
};
