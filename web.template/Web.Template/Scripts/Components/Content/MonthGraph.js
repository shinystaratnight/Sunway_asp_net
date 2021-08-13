import React from 'react';

export default class MonthGraph extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    renderMonthLabel(label, index) {
        const key = `month-label-${label}-${index}`;
        return (
            <span key={key} className="month-label col-xs-1">{label}</span>
        );
    }
    renderAxisX() {
        const months = ['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D'];
        return (
            <div className="row">
                    <div className="col-xs-2 col-sm-1 col-md-2"> </div>
                    <div className="x-axis col-xs-10 col-sm-11 col-md-10">
                    <div className="x-axis-labels row">
                        {months.map(this.renderMonthLabel, this)}
                    </div>
                    <div className="row">
                        <div className="col-xs-12 x-axis-label">
                            <label>Months</label>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    renderYValueLabel(label, index) {
        const key = `y-value-label-${label.toString}-${index}`;
        return (
            <label key={key} className="y-value-label">
                {label.toString()}
            </label>
        );
    }
    renderYAxisLabel() {
        switch (this.props.dataType) {
            case 'Temperature':
                return (
                    <label>Temperature <sup>o</sup>C</label>
                );
            default:
        }
        return (
            <label></label>
        );
    }
    renderAxisY() {
        const minValue = this.props.yRangeMinimum;
        const maxValue = this.props.yRangeMaximum;
        const range = maxValue - minValue;
        const numberOfLabels = 6;
        const interval = range / (numberOfLabels - 1);
        const values = [];
        for (let i = (numberOfLabels - 1); i >= 0; i--) {
            const value = minValue + (i * interval);
            values.push(parseInt(value, 10));
        }
        return (
            <div className="col-xs-2 col-sm-1 col-md-2">
                <div className="row">
                    <div className="y-axis-label">
                        {this.renderYAxisLabel()}
                    </div>
                    <div className="y-value-labels row">
                        <div className="col-xs-6"></div>
                        <div className="col-xs-6">
                            {values.map(this.renderYValueLabel, this)}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
    renderBackgroundBar(barClass, index) {
        const key = `background-bar-${index}`;
        return (
            <div key={key} className={barClass}> </div>
        );
    }
    renderBackgroundBars() {
        const backgroundBars = [];
        const numberBackgroundBars = 9;
        for (let i = 0; i < numberBackgroundBars; i++) {
            const even = (i % 2) === 0;
            let barClass = 'background-bar-';
            barClass = even ? `${barClass}small` : `${barClass}large`;
            backgroundBars.push(barClass);
        }
        return (
            <div className="background-bars">
                {backgroundBars.map(this.renderBackgroundBar, this)}
            </div>
        );
    }
    renderMonthBar(bar, index) {
        const key = `month-bar-${index}`;
        const barClasses
            = (index % 2) === 0 ? 'month-value-bar' : 'month-value-bar secondary';
        const barAttributes = {
            className: barClasses,
            style: {
                height: `${bar}%`,
            },
        };
        return (
            <div key={key} className="month-bar col-xs-1">
                <div {...barAttributes}></div>
            </div>
        );
    }
    renderMonthBars(bars) {
        const minValue = this.props.yRangeMinimum;
        const maxValue = this.props.yRangeMaximum;
        const range = maxValue - minValue;
        const barPercentages = [];
        bars.forEach(bar => {
            const fraction = (bar - minValue) / (range);
            const totalPercentagePoints = 100;
            const percentage = totalPercentagePoints * fraction;
            barPercentages.push(parseInt(percentage, 10).toString());
        });
        return (
            <div className="month-bars col-xs-10 col-sm-11 col-md-10">
                {this.renderBackgroundBars()}
                <div className="month-bar-container row">
                    {barPercentages.map(this.renderMonthBar, this)}
                </div>
            </div>
        );
    }
    render() {
        const barsString = this.props.values.split(',');
        const bars = [];
        barsString.forEach(bar => {
            bars.push(parseFloat(bar));
        });
        return (
            <div className="col-xs-12">
                <div className="row">
                    {this.renderAxisY()}
                    {this.renderMonthBars(bars)}
                </div>
                {this.renderAxisX()}
            </div>
        );
    }
}

MonthGraph.propTypes = {
    values: React.PropTypes.string,
    yRangeMinimum: React.PropTypes.number,
    yRangeMaximum: React.PropTypes.number,
    dataType: React.PropTypes.oneOf(['Temperature']),
};
