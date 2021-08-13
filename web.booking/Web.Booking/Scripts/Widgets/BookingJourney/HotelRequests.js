import React from 'react';
import TextInput from '../../components/form/textinput';

export default class HotelRequests extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    render() {
        const firstNameProps = {
            name: 'HotelRequests ',
            type: 'textarea',
            onChange: this.props.onChange,
            value: this.props.hotelRequest,
            placeholder: this.props.placeholder,
            containerClass: 'm-0',
        };

        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    <p className="mb-2">{this.props.message}</p>
                    <h4 className="requests h-teriary">Your Requests:</h4>
                    <TextInput {...firstNameProps} />
                </div>
            </div>
        );
    }
}

HotelRequests.propTypes = {
    onChange: React.PropTypes.func.isRequired,
    hotelRequest: React.PropTypes.string,
    title: React.PropTypes.string,
    message: React.PropTypes.string,
    placeholder: React.PropTypes.string,
};

HotelRequests.defaultProps = {
};
