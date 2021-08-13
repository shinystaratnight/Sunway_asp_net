import 'widgets/bookingjourney/_prebookBooking.scss';
import React from 'react';

export default class PreBookBooking extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    validate() {
        this.props.resetWarnings();

        const valid = true;
        if (valid) {
            this.props.onPreBook();
        } else {
            window.scrollTo(0, 0);
        }
    }
    renderButton() {
        return (
            <button type="button"
                className="btn btn-primary btn-lg btn-block-xs"
                onClick={this.validate.bind(this)}>
                {this.props.button && this.props.button.Text}
            </button>
        );
    }
    render() {
        this.props.buttonEnabled = this.props.requireCheckbox
            && this.props.checkboxChecked || !this.props.requireCheckbox;
        let buttonClass = `${this.props.buttonClass} btn btn-primary btn-lg btn-block-xs`;
        if (!this.props.buttonEnabled) {
            buttonClass += ' disable-button';
        }
        const onClick = {
            onClick: (ev) => this.props.handleCheckbox(ev),
        };
        return (
            <div className="panel panel-basic">
                <div className="panel-body">
                    <div className="row">
                        <div className="col-xs-12 col-sm-9">
                            <p className="font-weight-bold btn-text-lg">{this.props.message}</p>
                        </div>
                        <div className="col-xs-12 col-sm-3 mt-2 mt-sm-0 text-right">
                            {!this.props.basket.isLoaded
                                && <i className="loading-icon" aria-hidden="true"></i>}
                            {(this.props.basket.isLoaded && this.props.requireCheckbox)
                                && <input id="acceptance-checkbox"
                                    onClick={onClick.onClick}
                                    checked={this.props.checkboxChecked}
                                    type="checkbox" />}
                            {this.props.basket.isLoaded
                                && <button id="continue-button" type="button"
                                        className={buttonClass}
                                        onClick={this.validate.bind(this)}
                                        disabled={!this.props.buttonEnabled}>
                                        {this.props.button && this.props.button.Text}
                                    </button>}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

PreBookBooking.propTypes = {
    basket: React.PropTypes.object,
    onPreBook: React.PropTypes.func.isRequired,
    onError: React.PropTypes.func.isRequired,
    resetWarnings: React.PropTypes.func.isRequired,
    message: React.PropTypes.string,
    button: React.PropTypes.object,
    requireCheckbox: React.PropTypes.bool,
    checkboxChecked: React.PropTypes.bool,
    handleCheckbox: React.PropTypes.func,
    buttonEnabled: React.PropTypes.bool,
    buttonClass: React.PropTypes.string,
};

PreBookBooking.defaultProps = {
};
