import 'widgets/bookingjourney/_guestDetails.scss';

import DateSelectInput from 'components/form/dateselectinput';
import React from 'react';
import SelectInput from '../../components/form/selectinput';
import TextInput from '../../components/form/textinput';
import moment from 'moment';

export default class GuestDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    getDateOfBirthProps(guest, guestKey) {
        const today = new Date();
        const minYear = 1900;
        const maxYear = today.getFullYear();

        let value = guest.DateOfBirth;
        if (typeof value === 'string') {
            const dateOfBirth = moment(value);
            value = dateOfBirth.toDate();
        }

        const props = {
            name: `basket.${guestKey}.DateOfBirth`,
            label: '',
            required: false,
            updateValue: this.props.updateValue,
            value,
            error: this.props.warnings[`${guestKey}.DateOfBirth`],
            minYear,
            maxYear,
            containerClass: 'guest-date-input row',
            dayMonthSelectContainerClass: 'guest-date-select col-xs-4 col-md-3',
            yearSelectContainerClass: 'guest-date-select col-xs-4 col-md-4',
            placeholders: {
                day: 'DD',
                month: 'MM',
                year: 'YYYY',
            },
            appendText: guest.Age,
            appendTextPlaceholder: guest.Age,
            appendTextContainerClass: 'col-xs-6 col-md-2',
            appendTextDisplay: true,
        };
        return props;
    }
    setupTitleProps(guest, guestKey) {
        const TitleProps = {
            name: `${guestKey}.Title`,
            selectClass: '',
            onChange: this.props.onChange,
            value: guest.Title !== null ? guest.Title : '',
            options: [],
            placeholder: 'Title',
            error: this.props.warnings[`${guestKey}.Title`],
            containerClass: 'col-xs-12 col-md-2 guest-title',
        };

        if (guest.Type === 'Adult') {
            TitleProps.options = ['Mr', 'Mrs', 'Miss', 'Ms'];
        } else {
            TitleProps.options = ['Master', 'Miss'];
        }

        return TitleProps;
    }
    setupFirstNameProps(guest, guestKey) {
        const firstNameProps = {
            name: `${guestKey}.FirstName`,
            placeholder: this.props.firstNameLabel,
            type: 'text',
            onChange: this.props.onChange,
            value: guest.FirstName !== null ? guest.FirstName : '',
            error: this.props.warnings[`${guestKey}.FirstName`],
            containerClass: 'col-xs-12 col-md-5 col-lg-3',
        };

        return firstNameProps;
    }
    setupLastNameProps(guest, guestKey) {
        const lastNameProps = {
            name: `${guestKey}.LastName`,
            placeholder: this.props.lastNameLabel,
            type: 'text',
            onChange: this.props.onChange,
            value: guest.LastName !== null ? guest.LastName : '',
            error: this.props.warnings[`${guestKey}.LastName`],
            containerClass: 'col-xs-12 col-md-5 col-lg-3',
        };
        return lastNameProps;
    }
    renderRoom(room, index) {
        const roomKey = `Rooms[${index}]`;
        return (
            <div key={roomKey}>
                {this.props.roomLabel
                    && <h4 className="room">{this.props.roomLabel} {room.RoomNumber}</h4>}
                {room.Guests !== undefined
                    && room.Guests.map((guest, guestIndex) =>
                        this.renderGuest(guest, index, guestIndex)
                    )}
            </div>);
    }
    renderGuest(guest, roomIndex, guestIndex) {
        const guestKey = `Rooms[${roomIndex}].Guests[${guestIndex}]`;
        const firstNameInputProps = this.setupFirstNameProps(guest, guestKey);
        const lastNameInputProps = this.setupLastNameProps(guest, guestKey);
        const titleProps = this.setupTitleProps(guest, guestKey);
        return (
            <div key={guestKey} className="guest form row" >
                <div className="col-xs-12">
                    <h5 className="guest-details">{guest.Type} {guestIndex + 1}</h5>
                    {this.props.isTrade && roomIndex === 0 && guestIndex === 0
                        && <p>(Lead Guest)</p>}
                </div>
                <SelectInput {...titleProps} />
                <TextInput {...firstNameInputProps} />
                <TextInput {...lastNameInputProps} />
                {this.props.showDateOfBirth
                    && <div className="col-xs-12 col-md-4 guest-dob">
                        <DateSelectInput {...this.getDateOfBirthProps(guest, guestKey)} />
                    </div>}

            </div>
        );
    }
    render() {
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.rooms
                        && this.props.rooms.map(this.renderRoom, this)}
                </div>
            </div>
        );
    }
}

GuestDetails.propTypes = {
    rooms: React.PropTypes.array,
    onChange: React.PropTypes.func.isRequired,
    showDateOfBirth: React.PropTypes.bool,
    errors: React.PropTypes.object,
    title: React.PropTypes.string.isRequired,
    roomLabel: React.PropTypes.string,
    firstNameLabel: React.PropTypes.string.isRequired,
    lastNameLabel: React.PropTypes.string.isRequired,
    warnings: React.PropTypes.object,
    updateValue: React.PropTypes.func,
    isTrade: React.PropTypes.bool,
};

GuestDetails.defaultProps = {
    rooms: [],
    showDateOfBirth: false,
};
