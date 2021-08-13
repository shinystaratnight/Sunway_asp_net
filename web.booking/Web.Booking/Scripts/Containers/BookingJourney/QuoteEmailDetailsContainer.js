import * as QuoteActions from 'actions/bookingjourney/quoteActions';

import ObjectFunctions from '../../library/objectfunctions';
import QuoteEmailDetails from '../../widgets/bookingjourney/quoteemaildetails';
import React from 'react';
import ValidateFunctions from 'library/validateFunctions';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import moment from 'moment';

class QuoteEmailDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.handleSend = this.handleSend.bind(this);
        this.handlePDFSend = this.handlePDFSend.bind(this);
        this.setFormValues = this.setFormValues.bind(this);
        this.handleEmailQuoteHide = this.handleEmailQuoteHide.bind(this);
        this.state = {
            FormValues: {
                From: '',
                FromEmail: '',
                To: '',
                ToEmail: '',
                CCEmail: '',
            },
            warnings: [],
        };
    }
    componentDidMount() {
        const storage = window.localStorage;
        let StoredData = JSON.parse(storage.getItem('QuoteEmailCustomerDetails'));
        if (StoredData !== null) {
            if (moment().isBefore(StoredData.ExpiryDate)) {
                this.state.FormValues.ToEmail = StoredData.ToEmail;
                this.state.FormValues.To = StoredData.ToName;
            } else {
                storage.removeItem('QuoteEmailCustomerDetails');
            }
        }
        StoredData = JSON.parse(storage.getItem('QuoteEmailAgentDetails'));
        if (StoredData !== null) {
            if (moment().isBefore(StoredData.ExpiryDate)) {
                this.state.FormValues.FromEmail = StoredData.FromEmail;
                this.state.FormValues.From = StoredData.FromName;
            } else {
                storage.removeItem('QuoteEmailAgentDetails');
            }
        }
    }
    setFormValues(event) {
        const field = event.target.name;
        const value = event.target.value;
        const FormValues = this.state.FormValues;
        ObjectFunctions.setValueByStringPath(FormValues, field, value);
        this.setState({
            FormValues,
        });
    }
    handleEmailQuoteHide() {
        this.props.actions.hideQuoteEmailPopup();
    }
    generateModel() {
        let FlightToken = '';
        if (this.props.searchResults.Flight) {
            if (this.props.searchResults.Flight.results.length > 0) {
                FlightToken
                    = this.props.searchResults.Flight.results.find(result => result.isSelected)
                        .ComponentToken;
            }
        }
        const PropertyToken = this.props.quote.QuoteEmailPropertyToken;
        const FlightSearchToken = this.props.search.searchResult.ResultTokens.Flight;
        const PropertySearchToken = this.props.search.searchResult.ResultTokens.Hotel;
        const RoomOptions = this.props.quote.QuoteEmailPropertyCheapestResults;
        const ToEmail = this.state.FormValues.ToEmail;
        const ToName = this.state.FormValues.To;
        const FromEmail = this.props.entity.model.DefaultFromEmail === ''
            ? this.state.FormValues.FromEmail : this.props.entity.model.DefaultFromEmail;
        const FromName = this.props.entity.model.DefaultFromName === ''
            ? this.state.FormValues.From : this.props.entity.model.DefaultFromName;
        const CCEmail = this.state.FormValues.CCEmail;
        const model = {
            FlightToken,
            PropertyToken,
            FlightSearchToken,
            PropertySearchToken,
            RoomOptions,
            ToEmail,
            ToName,
            FromEmail,
            FromName,
            CCEmail,
        };
        return model;
    }
    validateForm() {
        let warnings = [];
        warnings.hasError = false;
        const form = this.state.FormValues;
        if (form) {
            Object.keys(form).forEach(k => {
                const value = form[k];
                if (value !== undefined) {
                    if (value.trim() !== '') {
                        if (k.toLowerCase().includes('email')
                            && !ValidateFunctions.isEmail(value)) {
                            warnings = this.addWarning(warnings, k, false);
                        }
                    } else if (k !== 'CCEmail') {
                        warnings = this.addWarning(warnings, k, true);
                    }
                } else if (k !== 'CCEmail') {
                    warnings = this.addWarning(warnings, k, true);
                }
            });
        }
        this.setState({
            warnings,
        });
        return warnings.hasError;
    }
    handleSend() {
        if (this.validateForm() === false) {
            this.createStorageEntries();
            this.props.actions.emailQuote(this.generateModel());
            this.handleEmailQuoteHide();
        }
    }
    handlePDFSend() {
        this.props.actions.pdfQuote(this.generateModel());
        this.handleEmailQuoteHide();
    }
    createStorageEntries() {
        const storage = window.localStorage;
        storage.removeItem('QuoteEmailCustomerDetails');
        const numberOfMinutes = 10;
        let detailsModel = {
            ToEmail: this.state.FormValues.ToEmail,
            ToName: this.state.FormValues.To,
            ExpiryDate: moment().add(numberOfMinutes, 'minutes'),
        };
        window.localStorage.setItem('QuoteEmailCustomerDetails', JSON.stringify(detailsModel));
        storage.removeItem('QuoteEmailAgentDetails');
        const numberOfDays = 30;
        detailsModel = {
            FromEmail: this.props.entity.model.DefaultFromEmail === ''
                ? this.state.FormValues.FromEmail : this.props.entity.model.DefaultFromEmail,
            FromName: this.props.entity.model.DefaultFromName === ''
                ? this.state.FormValues.From : this.props.entity.model.DefaultFromName,
            ExpiryDate: moment().add(numberOfDays, 'days'),
        };
        window.localStorage.setItem('QuoteEmailAgentDetails',
                                       JSON.stringify(detailsModel));
    }
    addWarning(warnings, field, emptyField) {
        const newWarnings = warnings;
        const contentModel = this.props.entity.model;
        switch (field) {
            case 'FromEmail':
                if (this.props.entity.model.DefaultFromEmail === '') {
                    if (emptyField) {
                        newWarnings.FromEmailError = contentModel.Warnings.EmptyField;
                    } else {
                        newWarnings.FromEmailError = contentModel.Warnings.InvalidFromEmail;
                    }
                    newWarnings.hasError = true;
                }
                break;
            case 'From':
                if (this.props.entity.model.DefaultFromName === '') {
                    newWarnings.FromError = contentModel.Warnings.EmptyField;
                    newWarnings.hasError = true;
                }
                break;
            case 'To':
                newWarnings.ToError = contentModel.Warnings.EmptyField;
                newWarnings.hasError = true;
                break;
            case 'ToEmail':
                if (emptyField) {
                    newWarnings.ToEmailError = contentModel.Warnings.EmptyField;
                } else {
                    newWarnings.ToEmailError = contentModel.Warnings.ToEmailError;
                }
                newWarnings.hasError = true;
                break;
            case 'CCEmail':
                newWarnings.CCEmailError = contentModel.Warnings.CCEmailError;
                newWarnings.hasError = true;
                break;
            default:
                return warnings;
        }
        return newWarnings;
    }
    getQuoteEmailDetailsProps() {
        const contentModel = this.props.entity.model;
        const props = {
            Title: contentModel.Title,
            IntroductionText: contentModel.IntroductionText,
            FromLabel: contentModel.FromLabel,
            FromEmailLabel: contentModel.FromEmailLabel,
            ToLabel: contentModel.ToLabel,
            ToEmailLabel: contentModel.ToEmailLabel,
            CCEmailLabel: contentModel.CCEmailLabel,
            SendEmailButtonText: contentModel.SendEmailButtonText,
            onChange: this.setFormValues,
            onSend: this.handleSend,
            onPDFSend: this.handlePDFSend,
            onQuoteEmailHide: this.handleEmailQuoteHide,
            FormValues: this.state.FormValues,
            Warnings: this.state.warnings,
            UsingDefaultFromValues: ((contentModel.DefaultFromName !== ''
                && contentModel.DefaultFromEmail !== '')
                || window.localStorage.getItem('QuoteEmailAgentDetails') !== null),
            ToPlaceholder: contentModel.ToPlaceholder,
            ToNamePlaceholder: contentModel.ToNamePlaceholder,
            FromPlaceholder: contentModel.FromPlaceholder,
            FromNamePlaceholder: contentModel.FromNamePlaceholder,
            CCEmailPlaceholder: contentModel.CCEmailPlaceholder,
        };
        return props;
    }
    render() {
        return (
            <div>
                {this.props.quote.QuoteEmailPopupShown
                    && <QuoteEmailDetails {...this.getQuoteEmailDetailsProps()} />}
            </div>
        );
    }
}
QuoteEmailDetailsContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    basket: React.PropTypes.object.isRequired,
    quote: React.PropTypes.object.isRequired,
    search: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    searchResults: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const basket = state.basket ? state.basket : {};
    const searchResults = state.searchResults ? state.searchResults : {};
    const quote = state.quote ? state.quote : {};
    const search = state.search ? state.search : {};
    return {
        basket,
        searchResults,
        search,
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
        QuoteActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(QuoteEmailDetailsContainer);
