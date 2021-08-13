import * as BasketActions from '../../actions/bookingjourney/basketactions';

import HotelRequests from '../../widgets/bookingjourney/hotelrequests';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class HotelRequestsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setBasket = this.setBasket.bind(this);
    }
    setBasket(event) {
        const value = event.target.value;
        this.props.actions.updateBasketValue('hotelRequest', value);
    }
    render() {
        let hasProperty = false;
        if (this.props.basket.isLoaded) {
            this.props.basket.basket.Components.forEach(
                component => {
                    if (component.ComponentType === 'Hotel') {
                        hasProperty = true;
                    }
                });
        }
        const hotelRequestProps = {
            onChange: this.setBasket,
            hotelRequest: this.props.basket.hotelRequest,
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            placeholder: this.props.entity.model.RequestPlaceholder,
        };
        return (
            <div>
                {hasProperty && this.props.basket.isLoaded
                    && <HotelRequests {...hotelRequestProps} />
                }
            </div>
        );
    }
}

HotelRequestsContainer.propTypes = {
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
    entity: React.PropTypes.object.isRequired,
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

export default connect(mapStateToProps, mapDispatchToProps)(HotelRequestsContainer);
