import * as BasketActions from '../../actions/bookingjourney/basketactions';

import PaymentBreakdown from '../../widgets/bookingjourney/paymentbreakdown';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PaymentBreakdownContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    isInitialised() {
        const isStateInitialised = this.props.basket.isLoaded && this.props.session.isLoaded;
        let isTradeSession = false;
        let transactingAgent = false;

        if (isStateInitialised) {
            isTradeSession = this.isTrade();
            const trade = this.props.tradeSession.Trade;
            transactingAgent = trade && !trade.NonTransacting;
        }

        const isInitialised = isStateInitialised && isTradeSession && transactingAgent;
        return isInitialised;
    }
    render() {
        const paymentBreakdownProps = {
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            paymentScheduleTitle: this.props.entity.model.PaymentScheduleTitle
                ? this.props.entity.model.PaymentScheduleTitle : '',
            payments: this.props.basket.basket.Payments,
            currency: this.props.session.UserSession.SelectCurrency,
        };
        return (
            <div>
                {this.isInitialised()
                && <PaymentBreakdown {...paymentBreakdownProps} />}
            </div>
        );
    }
}

PaymentBreakdownContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    session: React.PropTypes.object,
    tradeSession: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const tradeSession = state.session.UserSession && state.session.UserSession.TradeSession
        ? state.session.UserSession.TradeSession : {};

    return {
        basket,
        tradeSession,
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

export default connect(mapStateToProps, mapDispatchToProps)(PaymentBreakdownContainer);
