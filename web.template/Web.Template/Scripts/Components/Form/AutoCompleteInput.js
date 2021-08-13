import ArrayFunctions from '../../library/arrayfunctions';
import LookupApi from '../../api/lookupAPI';
import React from 'react';
import { Typeahead } from 'react-bootstrap-typeahead';

export default class AutoCompleteInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            options: [],
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.lookupApi = new LookupApi();
    }
    componentDidMount() {
        this.setOptions();
    }
    handleChange(selection) {
        let value = this.props.allowZeroValue ? 0 : '';
        if (this.props.multiple) {
            const selectedValues = [];
            selection.forEach(selectionItem => {
                if (selectionItem.Id) {
                    selectedValues.push(selectionItem.Id);
                } else if (selectionItem.Name) {
                    selectedValues.push(selectionItem.Name);
                } else {
                    selectedValues.push(selectionItem);
                }
            });
            value = selectedValues;
        } else if (selection.length > 0) {
            const selectionItem = selection[0];
            if (selectionItem.Id) {
                value = selectionItem.Id;
            } else if (selectionItem.Name) {
                value = selectionItem.Name;
            } else {
                value = selectionItem;
            }
        }
        this.props.onChange(this.props.name, value);
    }
    handleInputChange() {
        this.setOptions();
    }
    setOptions() {
        this.lookupApi.getLookup(this.props.lookup, this.props.siteName)
            .then(lookup => {
                let options = Object.assign([], lookup);

                // if selecting multiple remove already selected values
                if (this.props.multiple && Array.isArray(this.props.value)) {
                    this.props.value.forEach(valueItem => {
                        options = ArrayFunctions.removeItem(lookup, 'Name', valueItem);
                    });
                }

                // sort by name if available otherwise by value
                if (options[0].Name) {
                    options = ArrayFunctions.sortByPropertyAscending(options, 'Name');
                } else {
                    options.sort();
                }

                this.setState({
                    options,
                });
            });
    }
    typeaheadProps() {
        const typeaheadProps = {
            align: 'justify',
            labelKey: 'Name',
            minLength: 1,
            multiple: this.props.multiple,
            onChange: this.handleChange,
            onInputChange: this.handleInputChange,
            options: this.state.options,
            placeholder: this.props.placeholder,
        };

        if (this.props.value) {
            let selectedValue = this.props.value;

            if (!isNaN(this.props.value) && this.state.options[0].Id) {
                const selectedItem = this.state.options.find(option =>
                    option.Id === this.props.value);
                selectedValue = selectedItem.Name;
            }

            typeaheadProps.selected = Array.isArray(this.props.value)
                ? selectedValue : [selectedValue];
        }

        return typeaheadProps;
    }
    render() {
        let wrapperClass = 'form-group form-autocomplete';
        if (this.props.containerClass) {
            wrapperClass += ` ${this.props.containerClass}`;
        }
        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }
        const label = this.props.required ? `${this.props.label} *` : this.props.label;

        return (
            <div className={wrapperClass}>
                <label htmlFor={this.props.name}>{label}</label>
                {this.state.options.length > 0
                    && <Typeahead {...this.typeaheadProps()} />}
                {this.props.description
                    && <p className="help-block">{this.props.description}</p>}
                {this.props.error
                    && <p className="text-danger">{this.props.error}</p>}
            </div>
        );
    }
}

AutoCompleteInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    label: React.PropTypes.string,
    lookup: React.PropTypes.string.isRequired,
    onChange: React.PropTypes.func,
    placeholder: React.PropTypes.string,
    value: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
        React.PropTypes.array,
    ]),
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    description: React.PropTypes.string,
    siteName: React.PropTypes.string,
    multiple: React.PropTypes.bool,
    containerClass: React.PropTypes.string,
    allowZeroValue: React.PropTypes.bool,
};

AutoCompleteInput.defaultProps = {
    allowZeroValue: false,
};
