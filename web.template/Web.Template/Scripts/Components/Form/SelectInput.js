import ArrayFunctions from '../../library/arrayfunctions';
import LookupApi from '../../api/lookupAPI';
import React from 'react';

export default class SelectInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            options: Object.assign([], this.props.options),
        };
        this.lookupApi = new LookupApi();
    }
    componentWillMount() {
        if (this.props.optionsRangeMin !== undefined) {
            const min = this.props.optionsRangeMin;
            const max = this.props.optionsRangeMax;
            const options = this.generateOptionsFromRange(min, max);
            this.setState({ options });
        }
    }
    componentDidMount() {
        if (this.props.lookup) {
            this.setOptions();
        }
    }
    componentWillReceiveProps(nextProps) {
        if (this.state.options !== nextProps.options
                && typeof this.props.optionsRangeMin === 'undefined'
                && typeof nextProps.options !== 'undefined') {
            this.setState({ options: nextProps.options });
        }
        if ((this.props.optionsRangeMin !== nextProps.optionsRangeMin)
            || (this.props.optionsRangeMax !== nextProps.optionsRangeMax)) {
            const min = nextProps.optionsRangeMin;
            const max = nextProps.optionsRangeMax;
            const options = this.generateOptionsFromRange(min, max);
            this.setState({ options });
        }
    }
    setOptions() {
        this.lookupApi.getLookup(this.props.lookup)
            .then(lookup => {
                let options = Object.assign([], lookup);

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
    generateOptionsFromRange(min, max) {
        const options = [];
        for (let i = min; i <= max; i++) {
            options.push(i);
        }
        return options;
    }
    getWrapperClass() {
        let wrapperClass = 'form-group';

        const minErrorLength = 0;
        if (this.props.error && this.props.error.length > minErrorLength) {
            wrapperClass += ' has-error';
        }

        if (this.props.containerClass) {
            wrapperClass += ` ${this.props.containerClass}`;
        }

        return wrapperClass;
    }
    getInputProps() {
        const inputProps = {
            name: this.props.name,
            ref: this.props.name,
            className: this.props.selectClass
                ? `form-control ${this.props.selectClass}` : 'form-control',
            onChange: this.props.onChange,
        };

        const dataAttributes = this.props.dataAttributes ? this.props.dataAttributes : [];
        for (let i = 0; i < dataAttributes.length; i++) {
            const dataAttribute = dataAttributes[i];
            inputProps[`data-${dataAttribute.key}`] = dataAttribute.value;
        }

        if (this.props.value || this.props.value === 0) {
            inputProps.value = this.props.value;
        } else if (this.props.placeholder) {
            inputProps.defaultValue = this.props.placeholder;
            inputProps.className += ' has-placeholder';
        }

        if (this.props.disabled) {
            inputProps.disabled = true;
        }

        return inputProps;
    }
    renderOptionGroup(option) {
        return (
            <optgroup key={option.Id} label={option.Name}>
                {option.Options.map(this.renderOption, this)}
            </optgroup>
        );
    }
    renderOption(option) {
        let result = '';
        if (option && typeof option === 'object') {
            const key = `${this.props.name}_${option.Id}`;
            result = <option key={key} value={option.Id} className="option">
                        {option[this.props.fieldValue]}
                    </option>;
        } else {
            result = <option key={option} className="option">{option}</option>;
        }
        return result;
    }
    renderError() {
        let textClass = 'text-danger';
        if (this.props.warningTextClass) {
            textClass += ` ${this.props.warningTextClass}`;
        }
        return (
            <p className={textClass}>{this.props.error}</p>
        );
    }
    render() {
        const wrapperClass = this.getWrapperClass();

        const label = this.props.required ? `${this.props.label} *` : this.props.label;
        const labelClass = this.props.labelClass ? this.props.labelClass : '';

        let selectContainerClass = '';
        if (this.props.selectContainerClass) {
            selectContainerClass += ` ${this.props.selectContainerClass}`;
        }

        return (
            <div className={wrapperClass}>
                {this.props.label
                    && <label className={labelClass} htmlFor={this.props.name}>{label}</label>}
                <div className={selectContainerClass}>
                    <div className="custom-select">
                        <select {...this.getInputProps()}>
                            {this.props.placeholder
                                && <option className="placeholder" disabled>
                                    {this.props.placeholder}</option>}
                            {this.props.priorityOptions && !this.props.renderOptionGroups
                                && this.props.priorityOptions.map(this.renderOption, this)}

                            {this.props.priorityOptions && !this.props.renderOptionGroups
                                && <option className="divider" disabled>––––––––––––</option>}

                            {this.state.options && !this.props.renderOptionGroups
                                && this.state.options.map(this.renderOption, this)}

                            {this.state.options && this.props.renderOptionGroups
                                && this.state.options.map(this.renderOptionGroup, this)}
                        </select>
                    </div>
                </div>
                {this.props.description
                    && <p className="help-block">{this.props.description}</p>}
                {this.props.error
                    && this.renderError()}
            </div>
        );
    }
}

SelectInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    priorityOptions: React.PropTypes.array,
    options: React.PropTypes.array,
    optionsRangeMin: React.PropTypes.number,
    optionsRangeMax: React.PropTypes.number,
    renderOptionGroups: React.PropTypes.bool,
    value: React.PropTypes.oneOfType([
        React.PropTypes.string,
        React.PropTypes.number,
    ]),
    label: React.PropTypes.string,
    onChange: React.PropTypes.func,
    error: React.PropTypes.string,
    required: React.PropTypes.bool,
    placeholder: React.PropTypes.string,
    lookup: React.PropTypes.string,
    description: React.PropTypes.string,
    containerClass: React.PropTypes.string,
    labelClass: React.PropTypes.string,
    fieldValue: React.PropTypes.string,
    selectContainerClass: React.PropTypes.string,
    selectClass: React.PropTypes.string,
    warningTextClass: React.PropTypes.string,
    dataAttributes: React.PropTypes.array,
    disabled: React.PropTypes.bool,
};

SelectInput.defaultProps = {
    renderOptionGroups: false,
    dataAttributes: [],
    disabled: false,
    fieldValue: 'Name',
};
