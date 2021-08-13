import ArrayFunctions from 'library/arrayfunctions';
import CheckboxInput from 'components/form/checkboxinput';
import Price from 'components/common/price';
import React from 'react';
import moment from 'moment';

export default class CancellationCharges extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            errors: {},
            valid: false,
        };
    }
    renderCancellationRow(cancellation, arrivalDate) {
        const elements = [];
        const dateRange = this.renderDayMessage(cancellation.StartDate,
            cancellation.EndDate, arrivalDate);
        const dt = <dt className="col-xs-9 col-md-6 date">{dateRange}</dt>;
        elements.push(dt);

        const pricingConfiguration = Object.assign({}, this.props.pricingConfiguration);
        pricingConfiguration.PerPersonPricing = false;
        pricingConfiguration.PriceFormatDisplay = 'TwoDP';

        const priceProps = {
            amount: cancellation.Amount,
            currency: this.props.currency,
            pricingConfiguration,
        };
        const dd = <dd className="col-xs-3 col-md-6 text-right"><Price {...priceProps} /></dd>;
        elements.push(dd);

        return elements;
    }
    renderDayMessage(startDate, endDate, arrivalDate) {
        const startMoment = moment(startDate);
        const endMoment = moment(endDate);
        const arrivalMoment = moment(arrivalDate);
        const nowMoment = moment();
        let message = '';

        if ((startMoment.isSameOrBefore(nowMoment))) {
            message += `${nowMoment.format('ll')}`;
        } else {
            message += `${startMoment.format('ll')}`;
        }

        if ((endMoment.isAfter(arrivalMoment))) {
            message += ' onwards';
        } else if (!startMoment.isSame(endMoment)) {
            message += ` - ${endMoment.format('ll')}`;
        }
        return message;
    }
    renderComponentCancellation(component, index) {
        if (!component.CancellationCharges || component.CancellationCharges.length === 0) {
            return '';
        }
        return (
            <div key={`CancellationComponent-${index}`} className="row">
                <div className="col-xs-12 col-md-6 mb-2">
                    <h3>{component.ComponentType}</h3>
                    {component.Name
                        && <h4>{component.Name}</h4>}
                    <dl className="data-list row my-0">
                        {component.CancellationCharges.map((cancellationCharge, componentIndex) =>
                            this.renderCancellationRow(cancellationCharge,
                                component.ArrivalDate, componentIndex))}
                    </dl>
                </div>
            </div>
        );
    }
    renderPackageCancellationCosts() {
        let cancellationCosts = [];
        let earliestArrivalDate = null;
        this.props.components.forEach(c => {
            if (c.CancellationCharges) {
                c.CancellationCharges.forEach(cc => {
                    cancellationCosts.push(cc);
                });
            }
            if (!earliestArrivalDate || moment(c.ArrivalDate).isBefore(earliestArrivalDate)) {
                earliestArrivalDate = c.ArrivalDate;
            }
        });
        cancellationCosts = ArrayFunctions.sortByPropertyAscending(cancellationCosts, 'EndDate');

        const dateRanges = [];
        let lastEndDate = null;
        cancellationCosts.forEach(c => {
            const index = dateRanges.findIndex(d => d.EndDate.isSame(c.EndDate));
            if (index === -1) {
                const startMoment = lastEndDate
                    ? moment(lastEndDate).add(1, 'd') : moment(c.StartDate);
                const endMoment = moment(c.EndDate);
                const dateRange = {
                    StartDate: startMoment,
                    EndDate: endMoment,
                    Amount: 0,
                };
                dateRanges.push(dateRange);
                lastEndDate = c.EndDate;
            }
        });

        for (let i = 0; i < dateRanges.length; i++) {
            const dateRange = dateRanges[i];
            cancellationCosts.forEach(c => {
                if (moment(c.StartDate).isSameOrAfter(dateRange.StartDate)
                        && moment(c.StartDate).isSameOrBefore(dateRange.EndDate)
                    || (moment(c.EndDate).isSameOrAfter(dateRange.StartDate)
                        && moment(dateRange.StartDate).isSameOrAfter(c.StartDate))) {
                    dateRange.Amount += c.Amount;
                }
            });
        }

        return (
            <div className="row">
                <div className="col-xs-12 col-md-12 mb-2">
                    <dl className="data-list row my-0">
                        {dateRanges.map((dateRange) =>
                            this.renderCancellationRow(dateRange, earliestArrivalDate), this)}
                    </dl>
                </div>
            </div>
        );
    }
    render() {
        const cancellationCheckProps = {
            name: 'Cancellation Check',
            label: ` ${this.props.acceptanceCheckboxLabel}`,
            onChange: this.props.onChange,
            value: this.props.accepted,
            error: this.props.warnings.Cancellation,
            containerClass: 'm-0',
        };
        const packagePricing = this.props.pricingConfiguration.PackagePrice
            && this.props.search.searchDetails.SearchMode === 'FlightPlusHotel';
        const cancellationCosts = packagePricing ? this.renderPackageCancellationCosts()
            : this.props.components.map(this.renderComponentCancellation, this);
        return (
            <div className="panel panel-basic">
                <header className="panel-header">
                    <h2 className="h-tertiary">{this.props.title}</h2>
                </header>
                <div className="panel-body">
                    {this.props.message !== ''
                        && <p className="mb-2">{this.props.message}</p>}
                    {cancellationCosts}
                </div>
                <footer className="panel-footer">
                    <CheckboxInput {...cancellationCheckProps} />
                </footer>
            </div>
        );
    }
}

CancellationCharges.propTypes = {
    onChange: React.PropTypes.func.isRequired,
    title: React.PropTypes.string.isRequired,
    message: React.PropTypes.string.isRequired,
    acceptanceCheckboxLabel: React.PropTypes.string.isRequired,
    accepted: React.PropTypes.bool,
    components: React.PropTypes.array,
    currency: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    warnings: React.PropTypes.object,
    search: React.PropTypes.object,
};

CancellationCharges.defaultProps = {
};
