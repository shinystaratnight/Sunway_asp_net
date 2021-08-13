import CheckboxInput from '../../components/form/checkboxinput';
import React from 'react';

export default class TermsAndConditions extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    showTermsPopup(event, termsurl) {
        event.preventDefault();
        window.open(termsurl,
            'Terms and Conditions',
            'scrollbars=yes,resizable=yes,'
            + 'toolbar=yes,location=0,menubar=no,'
            + 'copyhistory=no,status=no,directories=no');
    }
    render() {
        let terms = '';
        let termsURL = '';

        this.props.components.forEach(
                component => {
                    if (component.TermsAndConditions !== null) {
                        terms = component.TermsAndConditions;
                    }
                    if (component.TermsAndConditionsUrl !== null) {
                        termsURL = component.TermsAndConditionsUrl;
                    }
                }
        );

        const termsCheckProps = {
            name: 'Terms Check',
            label: ` ${this.props.message}`,
            onChange: this.props.onChange,
            value: this.props.accepted,
            error: this.props.warnings.TermsAndConditions,
            containerClass: 'm-0',
        };
        const footerclass = terms !== '' ? 'panel-footer' : 'panel-body';

        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                {terms !== ''
                    && <div className="panel-body">
                    {terms !== ''
                        && <p>{terms}</p>
                    }
                    {termsURL !== ''
                        && <a href={termsURL}
                            onClick={(event) => this.showTermsPopup(event, termsURL)}>
                            Terms and Conditions
                            </a>
                    }
                    </div>
                }
                <footer className={footerclass}>
                    <CheckboxInput {...termsCheckProps} />
                </footer>
            </div>
        );
    }
}

TermsAndConditions.propTypes = {
    onChange: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    accepted: React.PropTypes.bool,
    components: React.PropTypes.array,
    warnings: React.PropTypes.object,
};

TermsAndConditions.defaultProps = {
};
