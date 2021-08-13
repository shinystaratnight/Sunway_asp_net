import * as AlertActions from 'actions/bookingjourney/alertActions';
import * as AlertConstants from 'constants/alerts';
import * as BasketActions from 'actions/bookingjourney/basketactions';

import Alert from 'widgets/bookingjourney/alert';
import NumberFunctions from 'library/numberfunctions';
import React from 'react';
import UrlFunctions from 'library/urlfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class AlertsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleBasketChange = this.handleBasketChange.bind(this);
        this.handleRemoveAlert = this.handleRemoveAlert.bind(this);
    }
    componentDidMount() {
        this.priceChangeCheck();
    }
    handleRemoveAlert(name) {
        this.props.actions.removeAlert(name);
    }
    handleBasketChange(event) {
        const field = event.target.name;
        let value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        if (value && typeof value === 'string' && !isNaN(value)) {
            value = parseInt(value, 10);
        }
        this.props.actions.updateBasketValue(field, value);
    }
    priceChangeCheck() {
        const priceChangeAmount = UrlFunctions.getQueryStringValue('pricechange');
        if (priceChangeAmount !== null && priceChangeAmount !== '') {
            const formattedAmount = NumberFunctions.formatPrice(priceChangeAmount,
                this.props.site.SiteConfiguration.PricingConfiguration,
                this.props.session.UserSession.SelectCurrency);

            const priceChangeWarning = 'Please note that the price has increased by'
            + ` ${formattedAmount} since the start of the booking process.  Please`
            + ' tick the box here to acknowledge that you accept the increase in price.';

            const alert = {
                name: 'hasAcceptedPriceIncrease',
                type: AlertConstants.ALERT_TYPES.DANGER,
                message: priceChangeWarning,
                dismissible: false,
                acceptance: {
                    onChange: this.handleBasketChange,
                    value: this.props.basket.hasAcceptedPriceIncrease,
                },
            };
            this.props.actions.addAlertObject(alert);
        }
    }
    renderAlert(alert, index) {
        const alertProps = {
            key: `alert_${index}`,
            name: alert.name,
            type: alert.type,
            message: alert.message,
            dismissible: alert.dismissible,
            acceptance: alert.acceptance ? alert.acceptance : {},
            onClose: this.handleRemoveAlert,
        };
        return (
            <Alert {...alertProps} />
        );
    }
    render() {
        return (
            <div className="alerts-container container">
                {this.props.alerts.map(this.renderAlert, this)}
            </div>
        );
    }
}

AlertsContainer.propTypes = {
    actions: React.PropTypes.object,
    alerts: React.PropTypes.array,
    basket: React.PropTypes.object,
    entity: React.PropTypes.object.isRequired,
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
    const alerts = state.alerts ? state.alerts : [];
    const basket = state.basket ? state.basket : {};
    const site = state.site ? state.site : {};

    return {
        alerts,
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
        AlertActions,
        BasketActions);

    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(AlertsContainer);
