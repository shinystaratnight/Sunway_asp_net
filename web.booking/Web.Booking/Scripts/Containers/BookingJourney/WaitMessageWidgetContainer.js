import '../../../styles/widgets/bookingjourney/_waitmessage.scss';

import * as EntityActions from '../../actions/content/entityActions';
import * as SearchActions from '../../actions/bookingjourney/searchActions';

import ModalPopup from '../../components/common/modalpopup';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class WaitMessageWidgetContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    componentDidMount() {
        this.props.actions.loadModelsFromEntityType(this.props.entity.site, 'WaitMessage');
    }
    getCorrectWaitMessage(key) {
        const waitMessages
            = this.props.waitMessages
            .filter(m => m.AssociatedUserStory.toLowerCase() === key.toLowerCase());
        const waitMessage = waitMessages[0] ? waitMessages[0] : {};
        return waitMessage;
    }
    renderWaitMessageContent(waitMessage, format) {
        let containerClass = 'waitmessage-content';
        containerClass += format === 'Popup' ? ' modal-container' : '';
        return (
            <div className={containerClass}>
                <img src={waitMessage.Image} className="mx-auto"/>
                <p>
                    {waitMessage.Message}
                </p>
            </div>
        );
    }
    getFormat(key) {
        const formatPairs = this.props.entity.model.Formats;
        const formatPair = formatPairs
            .filter(fp => fp.AssociatedUserStory.toLowerCase() === key.toLowerCase())[0];
        const format = formatPair.Format;
        return format;
    }
    render() {
        const messageTriggers = {
            search: this.props.search.isSearching,
            preBook: this.props.basket ? this.props.basket.isPreBooking : false,
            book: this.props.basket ? this.props.basket.isBooking : false,
            preBookFlight: this.props.basket ? this.props.basket.isPreBookingFlight : false,
            extraSearch: this.props.search.isSearchingForExtras,
            quoteCreate: this.props.basket ? this.props.basket.isCreatingQuote : false,
            quoteRetrieve: this.props.quote ? this.props.quote.isRetrieving : false,
        };
        let waitMessage = {};
        let format = 'Popup';
        let shouldRenderWaitMessage = false;
        Object.keys(messageTriggers).map((key) => {
            if (this.props.waitMessages.length > 0 && messageTriggers[key]) {
                waitMessage = this.getCorrectWaitMessage(key);
                format = this.getFormat(key);
                shouldRenderWaitMessage = true;
            }
        });

        return (
            <div>
            {shouldRenderWaitMessage
                && format === 'Popup'
                && <ModalPopup>
                  {this.renderWaitMessageContent(waitMessage, format)}
                </ModalPopup>}

            {shouldRenderWaitMessage
                && format === 'Embeded'
                && this.renderWaitMessageContent(waitMessage, format)}

            </div>
        );
    }
}

WaitMessageWidgetContainer.propTypes = {
    actions: React.PropTypes.object,
    search: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    waitMessages: React.PropTypes.array.isRequired,
    entity: React.PropTypes.object.isRequired,
    quote: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const search = state.search ? state.search : {};
    const waitMessages
        = state.models
            && state.models.WaitMessage
            && state.models.WaitMessage.isLoaded
        ? state.models.WaitMessage.models
        : [];
    const basket = state.basket ? state.basket : {};
    const quote = state.quote ? state.quote : {};

    return {
        search,
        waitMessages,
        basket,
        quote,
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

export default connect(mapStateToProps, mapDispatchToProps)(WaitMessageWidgetContainer);
