import NumberFunctions from 'library/numberfunctions';
import React from 'react';
import TextInput from './textinput';
import ValidateFunctions from 'library/validateFunctions';

export default class SliderInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isSliding: false,
            input: {
                left: 0,
                width: 0,
            },
            controlWidth: 0,
            minControl: {
                isSliding: false,
                left: 0,
                minBound: 0,
                maxBound: 0,
                value: this.props.startMin > this.props.minValue
                    ? this.props.startMin : this.props.minValue,
                isModified: false,
            },
            maxControl: {
                isSliding: false,
                left: null,
                minBound: 0,
                maxBound: 0,
                value: this.props.startMax < this.props.maxValue
                    ? this.props.startMax : this.props.maxValue,
                isModified: false,
            },
            highlightLeft: 0,
            highlightRight: 0,
            minTextBox: {
                value: this.props.minValue,
                showWarning: false,
                warningMessage: 'This value is not in range',
            },
            maxTextBox: {
                value: this.props.maxValue,
                showWarning: false,
                warningMessage: 'This value is not in range',
            },
        };
        this.textChangeTimeout = 0;
        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.handleMouseMove = this.handleMouseMove.bind(this);
        this.handleMouseUp = this.handleMouseUp.bind(this);
        this.getMinPriceInputProps = this.getMinPriceInputProps.bind(this);
        this.handleTextInputKeyDown = this.handleTextInputKeyDown.bind(this);
        this.handleTextInputKeyUp = this.handleTextInputKeyUp.bind(this);
        this.handleTextInputBlur = this.handleTextInputBlur.bind(this);
        this.handleFilterUpdateFromText = this.handleFilterUpdateFromText.bind(this);
        this.handleTextInputChange = this.handleTextInputChange.bind(this);
        this.handleEnterPress = this.handleEnterPress.bind(this);
        this.getControlPositionFromValue = this.getControlPositionFromValue.bind(this);
    }
    componentDidMount() {
        window.addEventListener('mousemove', this.handleMouseMove);
        window.addEventListener('mouseup', this.handleMouseUp);
    }
    componentWillReceiveProps(nextProps) {
        const input = this.state.input;
        let controlWidth = this.state.controlWidth;
        if (input.width === 0) {
            const sliderId = `${this.props.context}-slider-input`;
            const inputElement = document.getElementById(sliderId);
            input.width = inputElement.getBoundingClientRect().width;
        }
        if (controlWidth === 0) {
            const controlId = `${this.props.context}-slider-control-min`;
            const controlElement = document.getElementById(controlId);
            controlWidth = controlElement.getBoundingClientRect().width;
        }
        const minControl = this.state.minControl;
        const maxControl = this.state.maxControl;
        const maxTextBox = this.state.maxTextBox;
        const minTextBox = this.state.minTextBox;
        let highlightLeft = this.state.highlightLeft;
        let highlightRight = this.state.highlightRight;
        minControl.isModified = (Math.round(minControl.value) !== Math.round(this.props.minValue));
        maxControl.isModified = (Math.round(maxControl.value) !== Math.round(this.props.maxValue));

        if (nextProps.isReset
            || (nextProps.minValue > minControl.value)
            || !minControl.isModified) {
            minControl.value = nextProps.minValue;
            minControl.left = 0;
            highlightLeft = 0;
            minTextBox.value = nextProps.minValue;
        } else {
            const leftPosition
                = this.getControlPositionFromValue(minControl.value, nextProps, 'left');
            minControl.left = leftPosition;
            highlightLeft = leftPosition;
        }

        if (nextProps.isReset
            || (nextProps.maxValue < maxControl.value)
            || !maxControl.isModified) {
            maxControl.value = nextProps.maxValue;
            maxControl.left = this.state.input.width;
            highlightRight = 0;
            maxTextBox.value = nextProps.maxValue;
        } else {
            const rightPosition
                = this.getControlPositionFromValue(maxControl.value, nextProps, 'right');
            maxControl.left = rightPosition ? rightPosition : this.state.input.width;
            highlightRight = rightPosition ? this.state.input.width - rightPosition : 0;
        }
        this.setState({
            minControl,
            maxControl,
            minTextBox,
            maxTextBox,
            highlightLeft,
            highlightRight,
            input,
            controlWidth,
        });
    }
    getMinPriceInputProps() {
        const currency = this.props.currency;
        const symbol = currency.CustomerSymbolOverride
            ? currency.CustomerSymbolOverride
            : currency.Symbol;

        const priceInputProps = {
            name: 'min-price-slider-text-input',
            label: 'From ',
            value: this.state.minTextBox.value,
            onChange: this.handleTextInputChange,
            onBlur: this.handleTextInputBlur,
            onKeyDown: this.handleTextInputKeyDown,
            onKeyUp: this.handleTextInputKeyUp,
            onEnter: this.handleEnterPress,
            required: false,
            type: 'number',
            error: this.state.minTextBox.showWarning
                ? this.state.minTextBox.warningMessage
                : '',
            prependText: symbol,
            containerClass: 'min-price-input col-xs-6 col-sm-3 col-md-2',
        };
        return priceInputProps;
    }
    getMaxPriceInputProps() {
        const currency = this.props.currency;
        const symbol = currency.CustomerSymbolOverride
            ? currency.CustomerSymbolOverride
            : currency.Symbol;
        const priceInputProps = {
            name: 'max-price-slider-text-input',
            label: 'To ',
            value: this.state.maxTextBox.value,
            onChange: this.handleTextInputChange,
            onBlur: this.handleTextInputBlur,
            onKeyDown: this.handleTextInputKeyDown,
            onKeyUp: this.handleTextInputKeyUp,
            onEnter: this.handleEnterPress,
            required: false,
            type: 'number',
            error: this.state.maxTextBox.showWarning
                ? this.state.maxTextBox.warningMessage
                : '',
            prependText: symbol,
            containerClass: 'max-price-input col-xs-6 col-sm-3'
            + ' col-sm-push-6 col-md-2 col-md-push-8',
        };
        return priceInputProps;
    }
    getControlPositionFromValue(value, props, orientation) {
        const log10MaxValue = this.log10(props.maxValue);
        const log10MinValue = this.log10(props.minValue);
        const log10Value = this.log10(value);
        let percentage = log10Value - log10MinValue;
        const maxPercentage = 100;
        percentage = percentage * maxPercentage;
        percentage = percentage / (log10MaxValue - log10MinValue);
        let position = (this.state.input.width / maxPercentage) * percentage;
        const widthDivide = 2;
        const adjustment = this.state.controlWidth / widthDivide;
        position = orientation === 'left'
            ? position + adjustment : position - adjustment;
        return position;
    }
    log10(val) {
        return Math.log(val) / Math.log(10);
    }
    handleTextInputBlur(event) {
        clearTimeout(this.textChangeTimeout);
        this.handleFilterUpdateFromText(event.target);
    }
    handleEnterPress(event) {
        clearTimeout(this.textChangeTimeout);
        this.handleFilterUpdateFromText(event.target);
    }
    handleTextInputKeyDown(event) {
        clearTimeout(this.textChangeTimeout);
        const validKey = ValidateFunctions.isNumericOrEditKey(event.which);
        return (validKey);
    }
    handleTextInputKeyUp(event) {
        const timeOutDelay = 3000;
        const enterKey = 13;
        const target = event.target;
        this.textChangeTimeout = event.which !== enterKey
            ? setTimeout(() => this.handleFilterUpdateFromText(target), timeOutDelay)
            : null;
    }
    handleTextInputChange(event) {
        const isMaxInput = event.target.id.indexOf('max') !== -1;
        const value = event.target.value ? parseInt(event.target.value, 10) : '';
        const showWarning = (value > this.props.maxValue)
            || (value < this.props.minValue)
            || (value === '');
        const textBox = {
            value,
            showWarning,
            warningMessage: 'This value is not in range',
            isModified: true,
        };
        if (isMaxInput) {
            if (value <= this.state.minTextBox.value) {
                textBox.warningMessage = 'The maximum value must be greater than the minimum value';
                textBox.showWarning = true;
            }

            this.setState({
                maxTextBox: textBox,
            });
        } else {
            if (value >= this.state.maxTextBox.value) {
                textBox.warningMessage = 'The minimum value must be less than the maximum value';
                textBox.showWarning = true;
            }
            this.setState({
                minTextBox: textBox,
            });
        }
    }
    handleFilterUpdateFromText(target) {
        const isMaxInput = target.id.indexOf('max') !== -1;
        const input = isMaxInput ? 'max' : 'min';
        let updateFilters = true;
        let value = 0;
        const minTextBox = this.state.minTextBox;
        const maxTextBox = this.state.maxTextBox;
        const minControl = this.state.minControl;
        const maxControl = this.state.maxControl;

        if (isMaxInput) {
            value = this.state.maxTextBox.value > this.props.maxValue
                ? this.props.maxValue
                : this.state.maxTextBox.value;
            value = value < this.state.minTextBox.value
                ? this.state.minTextBox.value
                : value;
            if (value <= this.state.minTextBox.value) {
                updateFilters = false;
            }
            maxTextBox.value = value;
            maxTextBox.showWarning = !updateFilters;
            maxControl.value = value;
        } else {
            value = this.state.minTextBox.value < this.props.minValue
                ? this.props.minValue
                : this.state.minTextBox.value;
            value = value > this.state.maxTextBox.value
                ? this.state.maxTextBox.value
                : value;
            if (value >= this.state.maxTextBox.value) {
                updateFilters = false;
            }
            minTextBox.value = value;
            minTextBox.showWarning = !updateFilters;
            minControl.value = value;
        }
        this.setState({
            maxTextBox,
            minTextBox,
            minControl,
            maxControl,
        });
        if (updateFilters) {
            this.props.onChange(this.props.name, value, input);
        }
        clearTimeout(this.textChangeTimeout);
    }
    handleMouseDown(event) {
        event.preventDefault();

        this.state.isSliding = true;

        const sliderControl = event.target;
        this.state.controlWidth = sliderControl.getBoundingClientRect().width;

        const sliderInput = sliderControl.parentNode;
        const sliderInputRect = sliderInput.getBoundingClientRect();
        this.state.input.left = sliderInputRect.left;
        this.state.input.width = sliderInputRect.width;
        const maxControlLeft
            = this.state.maxControl.left
                ? this.state.maxControl.left : sliderInputRect.width;

        if (sliderControl.className.indexOf('min') !== -1) {
            this.state.minControl.isSliding = true;
            this.state.minControl.minBound = 0;
            this.state.minControl.maxBound = maxControlLeft - this.state.controlWidth;
        } else {
            this.state.maxControl.isSliding = true;
            this.state.maxControl.minBound = this.state.minControl.left + this.state.controlWidth;
            this.state.maxControl.maxBound = sliderInputRect.width;
        }
    }
    handleMouseMove(event) {
        if (this.state.isSliding) {
            const posX = event.pageX - this.state.input.left;
            if (this.state.minControl.isSliding) {
                const minControl = this.updateControl(this.state.minControl, posX);
                const minTextBox = this.state.minTextBox;
                minTextBox.value = minControl.value;
                this.setState({ minTextBox, minControl, highlightLeft: minControl.left });
            }

            if (this.state.maxControl.isSliding) {
                const maxControl = this.updateControl(this.state.maxControl, posX);
                const maxTextBox = this.state.maxTextBox;
                maxTextBox.value = maxControl.value;
                const highlightRight = this.state.input.width - maxControl.left;
                this.setState({ maxTextBox, maxControl, highlightRight });
            }
        }
    }
    handleMouseUp() {
        if (this.state.minControl.isSliding) {
            this.props.onChange(this.props.name, this.state.minControl.value, 'min');
            const minTextBox = this.state.minTextBox;
            minTextBox.value = this.state.minControl.value;
            this.setState({
                minTextBox,
            });
        }

        if (this.state.maxControl.isSliding) {
            this.props.onChange(this.props.name, this.state.maxControl.value, 'max');
            const maxTextBox = this.state.maxTextBox;
            maxTextBox.value = this.state.maxControl.value;
            this.setState({
                maxTextBox,
            });
        }

        this.state.isSliding = false;
        this.state.minControl.isSliding = false;
        this.state.maxControl.isSliding = false;
    }
    updateControl(control, posX) {
        const updatedControl = Object.assign({}, control);

        let updatedPosX = posX < updatedControl.minBound ? updatedControl.minBound : posX;
        updatedPosX = updatedPosX > updatedControl.maxBound ? updatedControl.maxBound : updatedPosX;

        const widthDivide = 2;
        let calculatedPosition = updatedPosX - (this.state.controlWidth / widthDivide);
        if (this.state.maxControl.isSliding) {
            calculatedPosition = updatedPosX + (this.state.controlWidth / widthDivide);
        }

        const maxPercentage = 100;
        const maxRoundThreshold = 99;
        const minRoundThreshold = 1;

        let percent = Math.round((calculatedPosition / this.state.input.width) * maxPercentage);
        percent = percent === maxRoundThreshold || percent > maxPercentage
            ? maxPercentage : percent;
        percent = percent === minRoundThreshold || percent < 0 ? 0 : percent;

        const log10MaxValue = this.log10(this.props.maxValue);
        const log10MinValue = this.log10(this.props.minValue);

        let value = (log10MaxValue - log10MinValue) * percent;
        value = value / maxPercentage;
        value = value + log10MinValue;
        value = Math.round(Math.pow(10, value));

        updatedControl.left = updatedPosX;
        updatedControl.value = value;
        return updatedControl;
    }
    renderPriceRange() {
        const prependText = this.props.prependedRangeText ? this.props.prependedRangeText : '';
        const priceAdjustment = this.props.rangePriceAdjustment
            ? this.props.rangePriceAdjustment : 0;
        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        pricingConfiguration.PriceSliderFormatDisplay = 'Rounded';

        const appendText = this.props.appendedRangeText ? this.props.appendedRangeText : '';

        const rangeMinValue = this.state.minControl.value + priceAdjustment;
        const formattedMinValue = NumberFunctions.formatPriceSliderPrice(rangeMinValue,
            pricingConfiguration, this.props.currency, false);

        const rangeMaxValue = this.state.maxControl.value + priceAdjustment;
        const formattedMaxValue = NumberFunctions.formatPriceSliderPrice(rangeMaxValue,
            pricingConfiguration, this.props.currency, true);

        const minValueText = `${prependText} ${formattedMinValue}`;
        const maxValueText = `${prependText} ${formattedMaxValue}${appendText}`;

        return (
            <p className="slider-range-display">
                <span className="slider-range-min-value">{minValueText}</span>
                <span className="priceSeperator"> - </span>
                <span className="slider-range-max-value">{maxValueText}</span>
            </p>
        );
    }
    render() {
        const sliderControlMin = {
            className: 'slider-control slider-control-min',
            id: `${this.props.context}-slider-control-min`,
            onMouseDown: this.handleMouseDown,
            style: {
                left: this.state.minControl.left,
            },
        };

        let containerClass = 'slider-input-container';
        let sliderClasses = 'slider-input';

        if (this.props.showMaxPriceInput && this.props.showMinPriceInput) {
            containerClass = `${containerClass} row text-inputs-visible`;
            sliderClasses
                = `${sliderClasses} col-xs-12 col-sm-6 col-sm-pull-3 col-md-8 col-md-pull-2`;
        }
        const sliderId = `${this.props.context}-slider-input`;

        const sliderControlMax = {
            className: 'slider-control slider-control-max',
            id: `${this.props.context}-slider-control-max`,
            onMouseDown: this.handleMouseDown,
            style: {},
        };
        if (this.state.maxControl.left) {
            sliderControlMax.style.left = this.state.maxControl.left;
        } else {
            sliderControlMax.style.right = -24;
        }


        const sliderHighlight = {
            className: 'slider-highlight',
            style: {
                left: this.state.highlightLeft,
                right: this.state.highlightRight,
            },
        };

        return (
            <div className={containerClass}>
                {this.props.showMinPriceInput
                    && <TextInput {...this.getMinPriceInputProps() } />}
                {this.props.showMaxPriceInput
                    && <TextInput {...this.getMaxPriceInputProps() } />}
                <div id={sliderId} className={sliderClasses}>
                    {this.props.showPriceRange
                        && !this.props.updatingPrice
                        && this.renderPriceRange()}
                    <div>
                        <div {...sliderHighlight}></div>
                        <div {...sliderControlMin}></div>
                        <div {...sliderControlMax}></div>
                    </div>
                </div>
            </div>
        );
    }
}

SliderInput.propTypes = {
    name: React.PropTypes.string.isRequired,
    minValue: React.PropTypes.number.isRequired,
    maxValue: React.PropTypes.number.isRequired,
    startMin: React.PropTypes.number.isRequired,
    startMax: React.PropTypes.number.isRequired,
    onChange: React.PropTypes.func.isRequired,
    showMinPriceInput: React.PropTypes.bool,
    isReset: React.PropTypes.bool,
    showMaxPriceInput: React.PropTypes.bool,
    showPriceRange: React.PropTypes.bool,
    context: React.PropTypes.string,
    prependedRangeText: React.PropTypes.string,
    appendedRangeText: React.PropTypes.string,
    rangePriceAdjustment: React.PropTypes.number,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    updatingPrice: React.PropTypes.bool,
};
