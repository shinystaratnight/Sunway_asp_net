import * as BasketActions from '../../actions/bookingjourney/basketactions';

import BaggageUpsell from '../../widgets/bookingjourney/baggageUpsell';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BaggageUpsellContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.handleBaggageUpdate = this.handleBaggageUpdate.bind(this);
    }
    handleBaggageUpdate(componentToken, subComponentToken, quantity) {
        const parsedQuantity = parseInt(quantity, 10);
        this.props.actions
            .basketFlightUpdateExtra(componentToken, subComponentToken, parsedQuantity);
    }
    stateBasketLoaded() {
        const stateBasket = this.props.basket;
        const basket = stateBasket.basket;
        const shouldRender
            = stateBasket && stateBasket.isLoaded
            && basket.Components && basket.Components.length > 0;
        return shouldRender;
    }
    shouldRenderBaggageUpsell() {
        const stateBasket = this.props.basket;
        const basket = stateBasket.basket;
        let shouldRender = this.stateBasketLoaded();

        if (shouldRender) {
            shouldRender = basket.Components.filter(c => c.ComponentType === 'Flight')
                && basket.Components.filter(c => c.ComponentType === 'Flight')[0];
        }

        if (shouldRender) {
            const flightComponent
                = basket.Components.filter(c => c.ComponentType === 'Flight')[0];
            const baggageComponents
                = flightComponent.SubComponents.filter(sc => !sc.Mandatory);
            shouldRender = !flightComponent.IncludesSupplierBaggage
                && baggageComponents.filter(bc => bc.QuantityAvailable > 0).length > 0;
        }

        return shouldRender;
    }
    buildBaggageUpsellProps() {
        const stateBasket = this.props.basket;
        const basket = stateBasket.basket;
        const contentModel = this.props.entity.model;
        const flightComponent
            = basket.Components.filter(c => c.ComponentType === 'Flight')[0];
        const baggageComponents
            = flightComponent.SubComponents.filter(sc => !sc.Mandatory);
        const props = {
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            flight: flightComponent,
            baggageComponents,
            selectedCurrency: this.props.session.UserSession.SelectCurrency,
            pricingConfiguration: this.props.site.SiteConfiguration.PricingConfiguration,
            handleBaggageUpdate: this.handleBaggageUpdate,
        };
        return props;
    }
    renderPlaceholder() {
        const contentModel = this.props.entity.model;
        return (
            <div className="widget-transfer-upsell panel panel-basic">
                <div className="panel-header">
                    <h3 className="h-tertiary">{contentModel.Title}</h3>
                </div>
                <div className="panel-body">
                    <div className="wait-message-container">
                        <i className="loading-icon" aria-hidden="true"></i>
                        <div className="wait-message">
                            <p>Loading...</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    render() {
        const shouldRenderBaggageUpsell = this.shouldRenderBaggageUpsell();
        const hasFlight
            = this.props.basket.basket.Components.filter(c => c.ComponentType === 'Flight')
            && this.props.basket.basket.Components.filter(c => c.ComponentType === 'Flight')[0];

        const shouldRenderPlaceholder = !shouldRenderBaggageUpsell && hasFlight;
        return (
            <div>
                {shouldRenderBaggageUpsell
                    && <BaggageUpsell {...this.buildBaggageUpsellProps()} />}
                {shouldRenderPlaceholder && !this.stateBasketLoaded()
                    && this.renderPlaceholder()}
            </div>
        );
    }
}

BaggageUpsellContainer.propTypes = {
    context: React.PropTypes.string,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    site: React.PropTypes.object,
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

export default connect(mapStateToProps, mapDispatchToProps)(BaggageUpsellContainer);
