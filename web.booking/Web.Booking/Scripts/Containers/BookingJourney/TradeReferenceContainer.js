import * as BasketActions from '../../actions/bookingjourney/basketactions';

import React from 'react';
import TradeReference from '../../widgets/bookingjourney/tradeReference';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class TradeReferenceContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setReference = this.setReference.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isLoaded && nextProps.basket.isLoaded) {
            if (this.isTrade()) {
                this.props.actions.updateBasketValue('requiresTradeReference', true);
            }
        }
    }
    setReference(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.props.actions.updateBasketValue(`basket.${field}`, value);
    }
    isInitialised() {
        const isInitialised = this.props.basket.isLoaded;
        return isInitialised;
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
        const referenceProps = {
            onChange: this.setReference,
            title: this.props.entity.model.Title,
            reference: this.props.basket.basket.TradeReference
                ? this.props.basket.basket.TradeReference : '',
            warnings: this.props.basket.warnings,
        };
        return (
            <div>
                {this.isInitialised()
                    && this.isTrade()
                    && <TradeReference {...referenceProps} />}
            </div>
        );
    }
}

TradeReferenceContainer.propTypes = {
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

export default connect(mapStateToProps, mapDispatchToProps)(TradeReferenceContainer);
