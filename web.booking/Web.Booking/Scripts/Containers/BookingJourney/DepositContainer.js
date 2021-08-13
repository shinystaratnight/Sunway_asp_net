import * as BasketActions from 'actions/bookingjourney/basketactions';

import Deposit from 'widgets/bookingjourney/deposit';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class DepositContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handlePaymentChange = this.handlePaymentChange.bind(this);
    }
    handlePaymentChange(amount) {
        const paymentDetails = Object.assign({}, this.props.basket.basket.PaymentDetails);
        paymentDetails.Amount = amount;
        paymentDetails.TotalAmount = amount;
        this.props.actions.updateBasketValue('basket.PaymentDetails', paymentDetails);
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    getDepositProps() {
        // const contentModel = this.props.entity.model;
        const depositProps = {
            title: 'Deposit',
            message: 'Please select from the payment options below',
            basket: this.props.basket.basket,
            onChange: this.handlePaymentChange,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
        };
        return depositProps;
    }
    shouldRenderDeposit() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};

        return this.props.basket.isLoaded
            && this.props.basket.basket.AmountDueToday > 0
            && (this.props.basket.basket.AmountDueToday < this.props.basket.basket.TotalAmountDue)
            && (!this.isTrade()
                    || (tradeSession.CreditCardAgent && !tradeSession.Trade.NonTransacting));
    }
    render() {
        return (
            <div>
                {this.shouldRenderDeposit()
                    && <Deposit {...this.getDepositProps()} />}
            </div>
        );
    }
}

DepositContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    session: React.PropTypes.object,
    site: React.PropTypes.object,
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
        BasketActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(DepositContainer);
