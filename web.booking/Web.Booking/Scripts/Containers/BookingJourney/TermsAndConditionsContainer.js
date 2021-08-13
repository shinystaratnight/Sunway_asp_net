import * as BasketActions from '../../actions/bookingjourney/basketactions';

import React from 'react';
import TermsAndConditions from '../../widgets/bookingjourney/termsandconditions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class TermsAndConditionsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setTerms = this.setTerms.bind(this);
    }
    setTerms(event) {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        this.props.actions.updateBasketValue('acceptedTAndC', value);
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
        const termsProps = {
            onChange: this.setTerms,
            accepted: this.props.basket.acceptedTAndC,
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            components: this.props.basket.basket.Components,
            warnings: this.props.basket.warnings,
        };
        const trade = this.props.tradeSession.Trade;
        const shouldRender = this.props.basket.isLoaded
            && (!this.isTrade() || (trade && !trade.NonTransacting));
        return (
            <div>
                {shouldRender
                    && <TermsAndConditions {...termsProps} />}
            </div>
        );
    }
}

TermsAndConditionsContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    tradeSession: React.PropTypes.object,
    session: React.PropTypes.object,
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

export default connect(mapStateToProps, mapDispatchToProps)(TermsAndConditionsContainer);
