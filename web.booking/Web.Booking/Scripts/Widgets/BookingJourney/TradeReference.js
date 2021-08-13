import React from 'react';
import TextInput from '../../components/form/textinput';


export default class TradeReference extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    render() {
        const referenceProps = {
            name: 'TradeReference',
            label: '',
            type: 'text',
            onChange: this.props.onChange,
            value: this.props.reference,
            error: this.props.warnings.TradeReference,
        };
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.bodytext
                        && <p>{this.props.bodytext}</p>}
                    <h4 className="reference-subtitle h3-teritary">Reference Number:</h4>
                    <TextInput {...referenceProps} />
                </div>
            </div>
        );
    }
}

TradeReference.propTypes = {
    onChange: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    bodytext: React.PropTypes.string,
    reference: React.PropTypes.string.isRequired,
    warnings: React.PropTypes.object,
};

TradeReference.defaultProps = {
};
