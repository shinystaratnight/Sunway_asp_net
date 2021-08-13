import * as BasketActions from '../../actions/bookingjourney/basketactions';

import Errata from '../../widgets/bookingjourney/errata';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class ErrataContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setErrata = this.setErrata.bind(this);
    }
    componentWillReceiveProps(nextProps) {
        if (!this.props.basket.isLoaded && nextProps.basket.isLoaded) {
            const hasErrata = nextProps.basket.basket.Errata.length > 0;
            if (!hasErrata) {
                this.props.actions.updateBasketValue('acceptedErrata', true);
            }
        }
    }
    getErrataProps() {
        const errataProps = {
            onChange: this.setErrata,
            accepted: this.props.basket.acceptedErrata,
            title: this.props.entity.model.Title,
            message: this.props.entity.model.Message,
            acceptanceCheckboxLabel: this.props.entity.model.AcceptanceCheckboxLabel
                ? this.props.entity.model.AcceptanceCheckboxLabel : '',
            errata: this.props.basket.basket.Errata,
            components: this.props.basket.basket.Components,
            warnings: this.props.basket.warnings,
        };
        return errataProps;
    }
    setErrata(event) {
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        this.props.actions.updateBasketValue('acceptedErrata', value);
    }
    render() {
        let hasErrata = false;
        if (this.props.basket.isLoaded) {
            hasErrata = (this.props.basket.basket.Errata.length > 0);
        }

        return (
            <div>
                {this.props.basket.isLoaded
                    && hasErrata
                    && <Errata {...this.getErrataProps()} />}
            </div>
        );
    }
}

ErrataContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
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

export default connect(mapStateToProps, mapDispatchToProps)(ErrataContainer);
