import 'components/booking/_propertyComponent.scss';

import React from 'react';
import StringFunctions from 'library/stringfunctions';
import moment from 'moment';

export default class PropertyComponent extends React.Component {
    getMealBasis(id) {
        return this.props.mealBasis.find(m => m.Id === id);
    }
    getRoomPaxSummary(room) {
        let paxSummary = StringFunctions.getPluralisationValue(room.Adults, 'Adult', 'Adults');
        paxSummary += room.Children > 0
            ? ` ${StringFunctions.getPluralisationValue(room.Children, 'Child', 'Children')}` : '';
        paxSummary += room.Infants > 0
            ? ` ${StringFunctions.getPluralisationValue(room.Infants, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    renderPropertyImage() {
        const component = this.props.component;
        const content = component.content ? component.content : { Image: {} };

        const imageProps = {
            className: 'property-image',
            src: content.Image && content.Image.Source
                ? content.Image.Source
                : 'http://placehold.it/210x140?text=No%20Image',
            alt: content.Name,
        };

        return (
            <div className="property-image-container col-xs-12 col-sm-4 col-md-2">
                <img {...imageProps} />
            </div>
        );
    }
    renderPropertySummary() {
        const component = this.props.component;
        const content = component.content ? component.content : { Image: {} };
        const subname = content
            ? `${content.Resort}, ${content.Region}`
            : '';

        const dtClass = 'property-label';
        const ddClass = 'property-detail';

        const checkInDate = moment(component.ArrivalDate).format('ll');
        const checkOutDate = moment(component.ArrivalDate)
            .add(component.Duration, 'd').format('ll');

        return (
            <div className="property-summary col-xs-12 col-sm-8 col-md-4">
                <h4>{content.Name}</h4>
                <p className="">{subname}</p>
                <span
                    data-rating={content.Rating}
                    itemProp="award"
                    className="rating">{content.Rating} Stars</span>
                <dl className="data-list">
                    <dt className={dtClass}>Check In Date</dt>
                    <dd className={ddClass}>{checkInDate}</dd>
                    <dt className={dtClass}>Check Out Date</dt>
                    <dd className={ddClass}>{checkOutDate}</dd>
                    <dt className={dtClass}>Nightsss</dt>
                    <dd className={ddClass}>{component.Duration}</dd>
                </dl>
            </div>
        );
    }
    renderPropertyRooms() {
        const component = this.props.component;
        const roomCount = component.SubComponents.length;
        const roomHeader = StringFunctions.getPluralisationValue(roomCount, 'Room', 'Rooms');
        return (
            <div className="property-rooms col-xs-12 col-md-6">
                <h5>{roomHeader}</h5>
                {component.SubComponents.map(this.renderPropertyRoom, this)}
            </div>
        );
    }
    renderPropertyRoom(subComponent, index) {
        const mealBasis = this.getMealBasis(subComponent.MealBasisId);
        let roomDescription = subComponent.RoomType;
        if (subComponent.RoomView) {
            roomDescription += `, ${subComponent.RoomView}`;
        }
        return (
            <div key={`room-${index}`} className="property-room row">
                <div className="col-xs-12 col-sm-6">
                    <p>{this.getRoomPaxSummary(subComponent)}</p>
                </div>
                <div className="col-xs-12 col-sm-6">
                    <p className="description">{roomDescription}</p>
                    <p>{mealBasis.Name}</p>
                </div>
            </div>
        );
    }
    renderErrata() {
        return (
            <div className="property-errata col-xs-12">
                {this.props.errata.map(this.renderErratum, this)}
            </div>
        );
    }
    renderErratum(erratum, index) {
        return (
            <div key={`errata_${index}`} className="alert alert-info mb-0 mt-3">
                <i className="alert-icon" aria-hidden="true"></i>
                <div className="alert-message">
                    <p>{erratum.Description}</p>
                </div>
            </div>
        );
    }
    render() {
        return (
            <div className="booking-property-details row">
                {this.renderPropertyImage()}
                {this.renderPropertySummary()}
                {this.renderPropertyRooms()}
                {this.props.errata.length > 0 && this.renderErrata()}
            </div>
        );
    }
}

PropertyComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
    errata: React.PropTypes.array,
    mealBasis: React.PropTypes.array.isRequired,
};
