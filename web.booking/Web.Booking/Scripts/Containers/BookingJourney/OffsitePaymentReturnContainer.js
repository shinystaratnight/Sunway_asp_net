import '../../../styles/widgets/bookingjourney/_offsitePaymentReturn.scss';

import React from 'react';

import UrlFunctions from '../../library/urlfunctions';
import { connect } from 'react-redux';

class OffsitePaymentReturnContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    componentDidMount() {
        const basketToken = UrlFunctions.getQueryStringValue('t');
        const page = window.location.pathname.split('/')[2];
        let paymentUrl = window.location.pathname.replace(`/booking/${page}`,
            '/booking/completebooking');

        paymentUrl += `?t=${basketToken}`;
        window.location.href = paymentUrl;
    }

    render() {
        return (
            <div>
                <p className="notice col-md-12">Please wait while we confirm your payment</p>
            </div>
        );
    }
}

OffsitePaymentReturnContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entity: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const site = state.site ? state.site : {};

    return {
        basket,
        site,
    };
}

export default connect(mapStateToProps)(OffsitePaymentReturnContainer);
