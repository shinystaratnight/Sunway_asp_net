import * as BasketActions from '../../actions/bookingjourney/basketactions';

import PromoCode from '../../widgets/bookingjourney/promocode';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PromoCodeContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleChange = this.handleChange.bind(this);
        this.handleApply = this.handleApply.bind(this);
        this.handleRemove = this.handleRemove.bind(this);
        this.handleError = this.handleError.bind(this);
    }
    handleApply() {
        this.props.actions.resetWarnings();
        this.props.actions.basketApplyPromoCode(
            this.props.basket.basket.BasketToken,
            this.props.basket.workingPromoCode);
    }
    handleRemove() {
        this.props.actions.basketRemovePromoCode(this.props.basket.basket.BasketToken);
    }
    handleError(key, value) {
        this.props.actions.addWarning(key, value);
    }
    handleChange(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.props.actions.updateBasketValue(field, value);
    }
    isInitialised() {
        const tradeSession = this.props.session.UserSession
            ? this.props.session.UserSession.TradeSession : {};
        const stateInitialised = this.props.basket.isLoaded && tradeSession;
        const isInitialised = stateInitialised;
        return isInitialised;
    }
    render() {
        const promoProps = {
            workingPromoCode: this.props.basket.workingPromoCode,
            currentPromoCode: this.props.basket.basket.PromoCode,
            onApply: this.handleApply,
            onRemove: this.handleRemove,
            onChange: this.handleChange,
            currentPromoDiscount: this.props.basket.basket.PromoCodeDiscount,
            currency: this.props.session.UserSession.SelectCurrency,
            onError: this.handleError,
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            invalidCodeWarning: this.props.entity.model.InvalidCodeWarning,
            promoCodeMessage: this.props.entity.model.PromoCodeMessage,
            applyButton: this.props.entity.model.ApplyButton,
            removeButton: this.props.entity.model.RemoveButton,
            warnings: this.props.basket.warnings,
        };

        return (
            <div>
                {this.isInitialised()
                && <PromoCode {...promoProps} />}
            </div>
        );
    }
}

PromoCodeContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
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

export default connect(mapStateToProps, mapDispatchToProps)(PromoCodeContainer);
