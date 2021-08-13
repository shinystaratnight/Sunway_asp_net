import React from 'react';
import TextInput from '../../components/form/textinput';

export default class PromoCode extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    formatCurrency(amount) {
        let formattedAmount = '';
        if (this.props.currency.SymbolPosition === 'Prepend') {
            formattedAmount = `${this.props.currency.Symbol}${amount}`;
        } else {
            formattedAmount = `${amount}${this.props.currency.Symbol}`;
        }
        return formattedAmount;
    }
    validate() {
        if (this.props.workingPromoCode !== '' && this.props.workingPromoCode !== null) {
            this.props.onApply();
        } else {
            this.props.onError('PromoCode', this.props.invalidCodeWarning);
        }
    }
    renderPromoCodeForm() {
        const promoCodeProps = {
            name: 'workingPromoCode',
            label: '',
            type: 'text',
            onChange: this.props.onChange,
            onEnter: this.validate.bind(this),
            value: this.props.workingPromoCode,
            error: this.props.warnings.PromoCode,
        };
        return (
            <div className="apply-promo">
                {this.props.message !== '' && <p className="mb-2">{this.props.message}</p>}
                <div className="row">
                    <div className="col-xs-12 col-sm-6 col-md-3">
                        <TextInput {...promoCodeProps} />
                    </div>
                    <div className="col-xs-12 col-sm-3">
                        <button className="btn btn-sm btn-primary btn-block-xs"
                        onClick={this.validate.bind(this)}
                        type="button">{this.props.applyButton.Text}</button>
                    </div>
                </div>
            </div>
        );
    }
    renderPromoCodeDetails() {
        return (
            <div className="apply-promo">
            {this.props.promoCodeMessage
                && <p className="mb-2">
                        {this.props.promoCodeMessage}
                        <span className="promo-code-discount">
                            {' '}
                            {this.formatCurrency(this.props.currentPromoDiscount)}</span>
                    </p>}
            <button className="btn btn-sm btn-primary"
                onClick={this.props.onRemove}
                type="button">{this.props.removeButton.Text}</button>
            </div>
        );
    }
    render() {
        return (
           <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.currentPromoDiscount === 0
                        && this.renderPromoCodeForm()}
                    {this.props.currentPromoDiscount !== 0
                        && this.renderPromoCodeDetails()}
                </div>
            </div>
        );
    }
}

PromoCode.propTypes = {
    workingPromoCode: React.PropTypes.string.isRequired,
    onApply: React.PropTypes.func.isRequired,
    onRemove: React.PropTypes.func.isRequired,
    onChange: React.PropTypes.func.isRequired,
    currentPromoCode: React.PropTypes.string,
    currentPromoDiscount: React.PropTypes.number,
    currency: React.PropTypes.object,
    onError: React.PropTypes.func.isRequired,
    warnings: React.PropTypes.object,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    invalidCodeWarning: React.PropTypes.string.isRequired,
    promoCodeMessage: React.PropTypes.string.isRequired,
    applyButton: React.PropTypes.object.isRequired,
    removeButton: React.PropTypes.object.isRequired,
};

PromoCode.defaultProps = {
};
