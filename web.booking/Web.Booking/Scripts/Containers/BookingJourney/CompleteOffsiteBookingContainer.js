import * as BasketActions from 'actions/bookingjourney/basketactions';

import React from 'react';
import UrlFunctions from 'library/urlfunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class CompleteOffsiteBookingContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    componentDidMount() {
        if (!this.props.basket.isLoaded && !this.props.basket.isLoading) {
            const basketToken = UrlFunctions.getQueryStringValue('t');
            this.props.actions.loadBasket(basketToken);
        }
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isLoaded && nextProps.basket.isLoaded) {
            this.handleBook(nextProps.basket);
        }
    }
    handleBook(basket) {
        if (!basket.isBooking) {
            this.props.actions.basketBook(
                basket.basket.BasketToken,
                basket.basket,
                basket.hotelRequest);
        }
    }
    render() {
        return (
            <div></div>
        );
    }
}

CompleteOffsiteBookingContainer.propTypes = {
    context: React.PropTypes.string,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
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

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BasketActions
    );

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(CompleteOffsiteBookingContainer);
