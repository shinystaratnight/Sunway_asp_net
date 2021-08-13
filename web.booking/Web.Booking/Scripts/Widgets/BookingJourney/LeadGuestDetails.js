import React from 'react';
import SelectInput from '../../components/form/selectinput';
import TextInput from '../../components/form/textinput';

export default class LeadGuestDetails extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getTitleProps() {
        const titleProps = {
            name: 'LeadGuest.Title',
            label: this.props.labels.title,
            selectClass: 'destination',
            onChange: this.props.onChange,
            value: this.props.leadGuest.Title ? this.props.leadGuest.Title : '',
            options: [' ', 'Mr', 'Mrs', 'Miss', 'Ms'],
            required: true,
            error: this.props.warnings['LeadGuest.Title'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            selectContainerClass: 'col-xs-12 col-md-3',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };
        return titleProps;
    }
    getFirstNameProps() {
        const firstNameProps = {
            name: 'LeadGuest.FirstName',
            label: this.props.labels.firstName,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.FirstName ? this.props.leadGuest.FirstName : '',
            error: this.props.warnings['LeadGuest.FirstName'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };
        return firstNameProps;
    }
    getLastNameProps() {
        const lastNameProps = {
            name: 'LeadGuest.LastName',
            label: this.props.labels.lastName,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.LastName ? this.props.leadGuest.LastName : '',
            error: this.props.warnings['LeadGuest.LastName'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };
        return lastNameProps;
    }
    getEmailProps() {
        const emailInputProps = {
            name: 'LeadGuest.Email',
            label: this.props.labels.email,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.Email ? this.props.leadGuest.Email : '',
            error: this.props.warnings['LeadGuest.Email'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };

        return emailInputProps;
    }
    getPhoneProps() {
        const phoneInputProps = {
            name: 'LeadGuest.Phone',
            label: this.props.labels.phone,
            type: 'text',
            required: true,
            onKeyDown: this.props.handlePhoneKeyDown,
            onChange: this.props.onChange,
            value: this.props.leadGuest.Phone ? this.props.leadGuest.Phone : '',
            error: this.props.warnings['LeadGuest.Phone'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            dataAttributes: [
                {
                    key: 'type',
                    value: 'integer',
                }],
        };

        return phoneInputProps;
    }
    getAddressProps() {
        const addressProps = {
            name: 'LeadGuest.AddressLine1',
            label: this.props.labels.address,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.AddressLine1 ? this.props.leadGuest.AddressLine1 : '',
            error: this.props.warnings['LeadGuest.AddressLine1'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
            dataAttributes: [
                {
                    key: 'type',
                    value: 'string',
                }],
        };

        return addressProps;
    }
    getCityProps() {
        const cityProps = {
            name: 'LeadGuest.TownCity',
            label: this.props.labels.city,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.TownCity ? this.props.leadGuest.TownCity : '',
            error: this.props.warnings['LeadGuest.TownCity'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-9',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };

        return cityProps;
    }
    getPostcodeProps() {
        const postCodeProps = {
            name: 'LeadGuest.Postcode',
            label: this.props.labels.postcode,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.Postcode ? this.props.leadGuest.Postcode : '',
            error: this.props.warnings['LeadGuest.Postcode'],
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            inputContainerClass: 'col-xs-12 col-md-3',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };

        return postCodeProps;
    }
    getCountryProps() {
        let options = [{ Id: 0, Name: '' }];
        options = options.concat(this.props.countries);
        const countryProps = {
            name: 'LeadGuest.BookingCountryID',
            label: this.props.labels.country,
            type: 'text',
            required: true,
            onChange: this.props.onChange,
            value: this.props.leadGuest.BookingCountryID
                ? this.props.leadGuest.BookingCountryID : 0,
            error: this.props.warnings['LeadGuest.Country'],
            options,
            containerClass: 'row',
            labelClass: 'col-form-label col-xs-12 col-md-3',
            selectContainerClass: 'col-xs-12 col-md-6',
            warningTextClass: 'col-xs-12 col-md-9 col-md-push-3',
        };

        return countryProps;
    }
    render() {
        return (
            <div className="panel panel-basic">
                 <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                    <p>* Required fields</p>
                </header>
                <div className="panel-body">
                    {this.props.message
                        && <p>{this.props.message}</p>}
                   <div className="lead-guest" >
                        <SelectInput {...this.getTitleProps()}/>
                        <TextInput {...this.getFirstNameProps()} />
                        <TextInput {...this.getLastNameProps()} />
                        <TextInput {...this.getEmailProps()} />
                        <TextInput {...this.getPhoneProps()} />
                        <TextInput {...this.getAddressProps()} />
                        <TextInput {...this.getCityProps()} />
                        <TextInput {...this.getPostcodeProps()} />
                        <SelectInput {...this.getCountryProps()} />
                    </div>
                 </div>
            </div>
        );
    }
}

LeadGuestDetails.propTypes = {
    leadGuest: React.PropTypes.object.isRequired,
    onChange: React.PropTypes.func.isRequired,
    handlePhoneKeyDown: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,

    labels: React.PropTypes.shape({
        title: React.PropTypes.string.isRequired,
        firstName: React.PropTypes.string.isRequired,
        lastName: React.PropTypes.string.isRequired,
        email: React.PropTypes.string.isRequired,
        phone: React.PropTypes.string.isRequired,
        address: React.PropTypes.string.isRequired,
        city: React.PropTypes.string.isRequired,
        postcode: React.PropTypes.string.isRequired,
        country: React.PropTypes.string.isRequired,
    }),

    warnings: React.PropTypes.object,
    countries: React.PropTypes.array,
};

LeadGuestDetails.defaultProps = {
    rooms: [],
    showDateOfBirth: false,
};
