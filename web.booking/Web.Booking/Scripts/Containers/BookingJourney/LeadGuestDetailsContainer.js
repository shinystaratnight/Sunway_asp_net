import * as BasketActions from 'actions/bookingjourney/basketactions';
import * as BookingCountryActions from 'actions/lookups/bookingCountryActions';

import LeadGuestDetails from 'widgets/bookingjourney/leadGuestDetails';
import React from 'react';
import ValidateFunctions from 'library/validateFunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class LeadGuestDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.setBasket = this.setBasket.bind(this);
        this.handlePhoneKeyDown = this.handlePhoneKeyDown.bind(this);
    }
    componentDidMount() {
        this.props.actions.loadBookingCountriesIfNeeded();
        if (!this.isTrade()) {
            this.props.actions.updateBasketValue('requiresLeadGuest', true);
        }
    }
    setBasket(event) {
        const field = event.target.name;
        const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;
        this.props.actions.updateBasketValue(`basket.${field}`, value);
    }
    handlePhoneKeyDown(event) {
        const newChar = event.key;
        const newValue = `${event.target.value}${newChar}`;
        const isValidKey = ValidateFunctions.isNumericPhoneNumberKey(newValue)
            || ValidateFunctions.isEditKey(event.which);
        if (!isValidKey) {
            event.preventDefault();
        }
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
        const contentModel = this.props.entity.model;
        const labels = contentModel.Labels;

        const guestDetailsProps = {
            leadGuest: this.props.basket.basket.LeadGuest ? this.props.basket.basket.LeadGuest : [],
            onChange: this.setBasket,
            handlePhoneKeyDown: this.handlePhoneKeyDown,
            title: contentModel.Title,
            message: contentModel.Message,

            labels: {
                title: labels.Title,
                firstName: labels.FirstName,
                lastName: labels.LastName,
                email: labels.Email,
                phone: labels.Phone,
                address: labels.Address,
                city: labels.City,
                postcode: labels.Postcode,
                country: labels.Country,
            },

            warnings: this.props.basket.warnings,
            countries: this.props.bookingCountries.items,
        };
        return (
            <div>
                {this.isInitialised()
                    && !this.isTrade()
                    && <LeadGuestDetails {...guestDetailsProps} />}
            </div>
        );
    }
}

LeadGuestDetailsContainer.propTypes = {
    context: React.PropTypes.string,
    entity: React.PropTypes.object.isRequired,
    bookingCountries: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    basket: React.PropTypes.object,
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
    const bookingCountries = state.bookingCountries ? state.bookingCountries : {};

    return {
        basket,
        bookingCountries,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BasketActions,
        BookingCountryActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(LeadGuestDetailsContainer);
