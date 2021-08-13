import 'components/booking/_extraComponent.scss';

import React from 'react';
import StringFunctions from 'library/stringfunctions';
import moment from 'moment';

export default class ExtraComponent extends React.Component {
    getPaxSummary(adults, children, infants) {
        let paxSummary = StringFunctions.getPluralisationValue(adults, 'Adult', 'Adults');
        paxSummary += children > 0
            ? ` ${StringFunctions.getPluralisationValue(children, 'Child', 'Children')}` : '';
        paxSummary += infants > 0
            ? ` ${StringFunctions.getPluralisationValue(infants, 'Infant', 'Infants')}` : '';
        return paxSummary;
    }
    render() {
        const extra = this.props.component;
        const startDate = moment(extra.ArrivalDate).format('ll');
        const expiryDate = moment(extra.ArrivalDate).add(extra.Duration, 'd').format('ll');
        const pax = this.getPaxSummary(extra.Adults, extra.Children, extra.Infants);

        const dtClass = 'extra-label font-weight-bold';
        const ddClass = 'extra-detail';
        return (
            <div className="booking-extra-details">
                <h4>{extra.ExtraName}</h4>
                <dl className="data-list mt-0">
                    <dt className={dtClass}>{extra.Duration > 0 ? 'Start Date' : 'Date'}</dt>
                    <dd className={ddClass}>{startDate}</dd>
                    {extra.Duration > 0
                        && <dt className={dtClass}>Expiry Date</dt>}
                    {extra.Duration > 0
                        && <dd className={ddClass}>{expiryDate}</dd>}
                    <dt className={dtClass}>Guests</dt>
                    <dd className={ddClass}>{pax}</dd>
                </dl>
            </div>
        );
    }
}

ExtraComponent.propTypes = {
    component: React.PropTypes.object.isRequired,
};
