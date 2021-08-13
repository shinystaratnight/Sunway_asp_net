import CheckboxInput from '../../components/form/checkboxinput';

import React from 'react';

export default class Errata extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    renderErratum(errata, index) {
        return (
            <p className="info-block" key={`Erratum-${index}`}>{errata.Description}</p>);
    }

    render() {
        const errataCheckProps = {
            name: 'errataCheck',
            label: this.props.acceptanceCheckboxLabel,
            onChange: this.props.onChange,
            value: this.props.accepted,
            error: this.props.warnings.Errata,
            containerClass: 'm-0',
        };
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    <p className="mb-2">{this.props.message}</p>
                    {this.props.errata.map(this.renderErratum, this)}
                </div>
                <footer className="panel-footer">
                    <CheckboxInput {...errataCheckProps} />
                </footer>
            </div>
        );
    }
}

Errata.propTypes = {
    onChange: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    acceptanceCheckboxLabel: React.PropTypes.string.isRequired,
    errata: React.PropTypes.array,
    accepted: React.PropTypes.bool,
    components: React.PropTypes.array,
    warnings: React.PropTypes.object,
};

Errata.defaultProps = {
};
