import * as EntityActions from '../../actions/content/entityActions';
import * as SearchActions from '../../actions/bookingjourney/searchActions';

import React from 'react';
import SalesMessage from '../../widgets/bookingjourney/salesmessage';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SalesMessageContainer extends React.Component {
    render() {
        const contentModel = this.props.entity.model;
        const salesMessageProps = {
            message: contentModel.Message,
            image: contentModel.Image,
            phoneNumber: contentModel.PhoneNumber,
        };
        return (
            <div>
                {!this.props.search.isSearching
                    && contentModel.message
                    && <SalesMessage {...salesMessageProps} />}
            </div>
        );
    }
}

SalesMessageContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    return {
        search,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions,
        SearchActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SalesMessageContainer);
