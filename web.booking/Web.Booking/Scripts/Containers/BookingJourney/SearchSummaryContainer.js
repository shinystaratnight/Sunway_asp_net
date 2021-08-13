import * as SearchActions from '../../actions/bookingjourney/searchActions';

import React from 'react';
import SearchSummary from '../../widgets/bookingjourney/searchsummary';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SearchSummaryContainer extends React.Component {
    render() {
        const searchSummaryProps = {
            rooms: this.props.search.searchDetails.Rooms,
            search: this.props.search,
        };
        const content = this.props.search.isLoaded
            ? <SearchSummary {...searchSummaryProps} /> : null;
        return (
            <div>{content}</div>
        );
    }
}

SearchSummaryContainer.propTypes = {
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
        SearchActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SearchSummaryContainer);
