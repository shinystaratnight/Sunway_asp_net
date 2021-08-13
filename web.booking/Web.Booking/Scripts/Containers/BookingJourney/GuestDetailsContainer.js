import * as BasketActions from '../../actions/bookingjourney/basketactions';

import GuestDetails from '../../widgets/bookingjourney/guestDetails';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class GuestDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setBasket = this.setBasket.bind(this);
        this.updateBasketValue = this.updateBasketValue.bind(this);
    }
    setBasket(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.updateBasketValue(`basket.${field}`, value);
    }
    updateBasketValue(field, value) {
        this.props.actions.updateBasketValue(field, value);
    }
    getGuestDetailsProps() {
        const dobRequired = this.props.basket.basket.GuestsRequireDoB;

        const guestDetailsProps = {
            rooms: this.props.basket.basket.Rooms ? this.props.basket.basket.Rooms : [],
            onChange: this.setBasket,
            showDateOfBirth: dobRequired,
            title: this.props.entity.model.Title,
            roomLabel: this.props.entity.model.RoomLabel,
            firstNameLabel: this.props.entity.model.FirstNameLabel,
            lastNameLabel: this.props.entity.model.LastNameLabel,
            warnings: this.props.basket.warnings,
            updateValue: this.updateBasketValue,
            isTrade: this.isTrade(),
            validateChildAges: this.props.entity.model.Configuration,
        };
        return guestDetailsProps;
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    render() {
        return (
            <div>
                {this.props.basket.isLoaded
                    && <GuestDetails {...this.getGuestDetailsProps()} />}
            </div>
        );
    }
}

GuestDetailsContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
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
    return {
        basket,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BasketActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(GuestDetailsContainer);
