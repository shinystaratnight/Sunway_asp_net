import '../../../styles/widgets/bookingjourney/_offsitePayment.scss';

import * as BasketActions from 'actions/bookingjourney/basketactions';

import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class OffsitePaymentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            paymentDetailsInitialised: false,
        };
        this.setBasket = this.setBasket.bind(this);
        this.updateBasketValue = this.updateBasketValue.bind(this);
    }
    isInitialised() {
        const isInitialised = this.props.basket.isLoaded;
        return isInitialised;
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

    getOffsiteHTML() {
        const htmlString = this.props.basket.basket.PaymentDetails.OffsitePaymentHtml;
        return { __html: htmlString };
    }

    render() {
        const contentModel = this.props.entity.model;

        return (
            <div className="offsite-payment-container">
                {this.isInitialised()
                    && <div className="panel panel-basic">
                        <header className="panel-header">
                            <h2 className="h-tertiary">{contentModel.Title}</h2>
                        </header>
                        <div className="panel-body">
                            <div dangerouslySetInnerHTML={this.getOffsiteHTML()} />
                        </div>
                    </div>
                }
            </div>
        );
    }
}

OffsitePaymentContainer.propTypes = {
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

export default connect(mapStateToProps, mapDispatchToProps)(OffsitePaymentContainer);
