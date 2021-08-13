import * as BasketActions from '../../actions/bookingjourney/basketactions';

import CancellationCharges from '../../widgets/bookingjourney/cancellationcharges';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class CancellationChargesContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setCancellation = this.setCancellation.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isLoaded && nextProps.basket.isLoaded) {
            const hasCancellation = this.hasCancellation(nextProps.basket.basket);
            if (!hasCancellation) {
                this.props.actions.updateBasketValue('acceptedCancellation', true);
            }
        }
    }
    getCancellationProps() {
        const cancellationProps = {
            onChange: this.setCancellation,
            accepted: this.props.basket.acceptedCancellation,
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            acceptanceCheckboxLabel: this.props.entity.model.AcceptanceCheckboxLabel
                ? this.props.entity.model.AcceptanceCheckboxLabel : '',
            components: this.props.basket.basket.Components,
            currency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            warnings: this.props.basket.warnings,
            search: this.props.search,
        };
        return cancellationProps;
    }
    isTrade() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const isTrade = tradeSession
            && (tradeSession.TradeId !== 0 || tradeSession.TradeContactId !== 0)
            && !this.props.session.UserSession.OverBranded;
        return isTrade;
    }
    hasCancellation(basket) {
        let hasCancellation = false;
        basket.Components.forEach(
            component => {
                if (component.CancellationCharges
                    && component.CancellationCharges.length > 0) {
                    hasCancellation = true;
                }
            }
        );
        return hasCancellation;
    }
    setCancellation(event) {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        this.props.actions.updateBasketValue('acceptedCancellation', value);
    }
    render() {
        let hasCancellation = false;
        const siteConfig = this.props.site.SiteConfiguration;
        if (this.props.basket.isLoaded
                && !siteConfig.BookingJourneyConfiguration.HideCancellationCharges) {
            hasCancellation = this.hasCancellation(this.props.basket.basket);
        }
        const trade = this.props.tradeSession.Trade;
        const shouldRender = this.props.basket.isLoaded && hasCancellation
            && (!this.isTrade() || (trade && !trade.NonTransacting));
        return (
            <div>
               {shouldRender
                    && <CancellationCharges {...this.getCancellationProps()} />}
            </div>
        );
    }
}

CancellationChargesContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    tradeSession: React.PropTypes.object.isRequired,
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
    const search = state.search ? state.search : {};
    const tradeSession = state.session.UserSession && state.session.UserSession.TradeSession
        ? state.session.UserSession.TradeSession : {};

    return {
        basket,
        site,
        search,
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
        BasketActions
        );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(CancellationChargesContainer);
