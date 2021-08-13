import 'widgets/bookingjourney/_quoteEmailDetail.scss';
import React from 'react';
import TextInput from '../../components/form/textinput';
export default class QuoteEmailDetails extends React.Component {
    setupFromProps() {
        const FromProps = {
            name: 'From',
            type: 'text',
            onChange: this.props.onChange,
            label: this.props.FromLabel,
            value: this.props.FormValues.From,
            containerClass: 'QuoteEmailDetailsRow',
            error: this.props.Warnings.FromError,
            placeholder: this.props.FromNamePlaceholder,
        };
        return FromProps;
    }
    setupFromEmailProps() {
        const FromEmailProps = {
            name: 'FromEmail',
            type: 'text',
            onChange: this.props.onChange,
            label: this.props.FromEmailLabel,
            value: this.props.FormValues.FromEmail,
            containerClass: 'QuoteEmailDetailsRow',
            error: this.props.Warnings.FromEmailError,
            placeholder: this.props.FromPlaceholder,
        };
        return FromEmailProps;
    }
    setupToProps() {
        const ToProps = {
            name: 'To',
            type: 'text',
            onChange: this.props.onChange,
            label: this.props.ToLabel,
            value: this.props.FormValues.To,
            containerClass: 'QuoteEmailDetailsRow',
            error: this.props.Warnings.ToError,
            placeholder: this.props.ToNamePlaceholder,
        };
        return ToProps;
    }
    setupToEmailProps() {
        const ToEmailProps = {
            name: 'ToEmail',
            type: 'text',
            onChange: this.props.onChange,
            label: this.props.ToEmailLabel,
            value: this.props.FormValues.ToEmail,
            containerClass: 'QuoteEmailDetailsRow',
            error: this.props.Warnings.ToEmailError,
            placeholder: this.props.ToPlaceholder,
        };
        return ToEmailProps;
    }
    setupCCEmailProps() {
        const CCEmailProps = {
            name: 'CCEmail',
            type: 'text',
            onChange: this.props.onChange,
            label: this.props.CCEmailLabel,
            value: this.props.FormValues.CCEmail,
            containerClass: 'QuoteEmailDetailsRow',
            error: this.props.Warnings.CCEmailError,
            placeholder: this.props.CCEmailPlaceholder,
        };
        return CCEmailProps;
    }

    render() {
        return (<div id="QuoteEmailDetails">
            <div className="HeaderRow">
                <span className="TitleText">{this.props.Title}</span>
                <button className="QuoteCloseButton btn btn-primary" onClick={
                    () => { this.props.onQuoteEmailHide(); }}>
                    Close</button>
                <span className="QuoteEmailIntroduction">{this.props.IntroductionText}</span>
            </div>
            <div className="QuoteEmailDetailsForm">
                {this.props.UsingDefaultFromValues === false
                    && <TextInput {...this.setupFromProps()} />}
                {this.props.UsingDefaultFromValues === false
                    && <TextInput {...this.setupFromEmailProps()} />}
                <TextInput {...this.setupToProps()} />
                <TextInput {...this.setupToEmailProps()} />
                <TextInput {...this.setupCCEmailProps()} />
                <div className="QuoteEmailDetailsRow">
                    <button className="QuoteDownloadButton btn btn-primary"
                        onClick={() => this.props.onPDFSend()}>
                        Download PDF/Print
					</button>
                    <button className="QuoteSendButton btn btn-primary"
                        onClick={() => this.props.onSend()}>
                        {this.props.SendEmailButtonText}
                    </button>
                </div>
            </div>
        </div>);
    }
}

QuoteEmailDetails.propTypes = {
    Title: React.PropTypes.string,
    IntroductionText: React.PropTypes.string,
    FormValues: React.PropTypes.shape({
        From: React.PropTypes.string,
        FromEmail: React.PropTypes.string,
        To: React.PropTypes.string,
        ToEmail: React.PropTypes.string,
        CCEmail: React.PropTypes.string,
    }),
    FromLabel: React.PropTypes.string,
    FromEmailLabel: React.PropTypes.string,
    onChange: React.PropTypes.func.isRequired,
    onSend: React.PropTypes.func.isRequired,
    onPDFSend: React.PropTypes.func.isRequired,
    onQuoteEmailHide: React.PropTypes.func.isRequired,
    ToLabel: React.PropTypes.string,
    ToEmailLabel: React.PropTypes.string,
    CCEmailLabel: React.PropTypes.string,
    SendEmailButtonText: React.PropTypes.string,
    Warnings: React.PropTypes.object,
    UsingDefaultFromValues: React.PropTypes.boolean,
    ToPlaceholder: React.PropTypes.string,
    ToNamePlaceholder: React.PropTypes.string,
    FromPlaceholder: React.PropTypes.string,
    FromNamePlaceholder: React.PropTypes.string,
    CCEmailPlaceholder: React.PropTypes.string,
};
