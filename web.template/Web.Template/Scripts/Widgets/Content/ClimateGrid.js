import '../../../styles/widgets/content/_climategrid.scss';

import React from 'react';

export default class ClimateGrid extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.measureTypes = {
            Sun: {
                icon: 'icon-sun',
            },
            Rain: {
                icon: 'icon-rain',
            },
            Temperature: {
                icon: 'icon-temp',
            },
        };
    }
    renderMeasure(measure, index) {
        const key = `measure-${index}`;
        const values = measure.Values ? measure.Values.split(',') : [];
        return (
            <div key={key} className="climate-grid-measure">
                <div className="container">
                    <div className="row">
                        <div className="measure-title col-xs-12 col-md-3">
                            <h3 className="h-tertiary">{measure.Title}</h3>
                        </div>
                        <div className="measure-values col-xs-12 col-md-9">
                            <div className="row">
                                {values.map((value, valueIndex) =>
                                    this.renderMeasureValue(measure.Type,
                                        value, valueIndex), this)}
                             </div>
                         </div>
                     </div>
                 </div>
            </div>
        );
    }
    renderMeasureValue(type, value, index) {
        const key = `measure-value-${index}`;
        const icon = `icon ${this.measureTypes[type].icon}`;
        return (
            <div key={key} className="measure-value col-xs-12 col-md-1">
                <p><span className={icon}></span>{value}</p>
            </div>
        );
    }
    renderMonthName(month) {
        return (
            <div key={month} className="month col-xs-12 col-md-1">{month}</div>
        );
    }
    render() {
        return (
            <div className="climate-grid">
                <div className="container">
                    <div className="row">
                     <div className="col-xs-12 center-block">
                        {this.props.title !== ''
                            && <h2 className="h-secondary">{this.props.title}</h2>}
                        {this.props.leadParagraph !== ''
                            && <p className="preamble">
                                {this.props.leadParagraph}</p>}
                        </div>
                    </div>
                 </div>
                <div className="climate-grid-measures">
                    {this.props.measures.map(this.renderMeasure, this)}
                </div>
                <div className="climate-grid-months">
                    <div className="container">
                        <div className="row">
                            <div className="months-title col-xs-12 col-md-3"></div>
                            <div className="months col-xs-12 col-md-9">
                                <div className="row">
                                       {this.props.monthNames.map(this.renderMonthName, this)}
                                 </div>
                             </div>
                         </div>
                     </div>
                </div>
            </div>
        );
    }
}

ClimateGrid.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    measures: React.PropTypes.arrayOf(
            React.PropTypes.shape({
                Title: React.PropTypes.string,
                Values: React.PropTypes.string,
                Type: React.PropTypes.string,
            })
        ).isRequired,
    monthNames: React.PropTypes.array,
};
