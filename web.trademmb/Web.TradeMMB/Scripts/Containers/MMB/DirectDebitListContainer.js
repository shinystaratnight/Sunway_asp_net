import '../../../styles/widgets/mmb/directDebitList.scss';

import * as BrandActions from '../../actions/lookups/brandActions';
import * as DirectDebitActions from '../../actions/mmb/directDebitActions';
import * as EntityActions from '../../actions/content/entityActions';

import DirectDebitList from '../../widgets/mmb/directdebitlist';

import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

import moment from 'moment';

class DirectDebitListContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            expandList: true,
        };
        this.toggleExpansion = this.toggleExpansion.bind(this);
    }
    componentDidMount() {
        const tradeId = this.props.session.UserSession.TradeSession.TradeId;
        this.props.actions.loadAllDirectDebits(tradeId);
        this.props.actions.loadBrands();
    }
    toggleExpansion() {
        if (this.props.directDebit.directDebits.length > 0) {
            this.setState({
                expandList: !this.state.expandList,
            });
        }
    }
    getListProps() {
        const props = {
            brands: this.props.brands,
            directDebits: this.props.directDebit.directDebits,
            pricingConfiguration: this.props.pricingConfiguration,
            selectCurrency: this.props.selectCurrency,
        };
        return props;
    }
    renderHeader(renderContent) {
        const contentModel = this.props.entity.model;
        const date = this.props.directDebit.PaymentDueBeforeDate;
        const formattedDate = moment(date).format('DD MMM \'YY');
        const title = renderContent ? contentModel.SectionHeader.replace('{Date}', formattedDate)
            : contentModel.NoResultsSectionHeader;
        const baseClass = 'expand-list link fa float-right';
        const toggleClass = this.state.expandList
            ? `${baseClass} fa-minus` : `${baseClass} fa-plus`;
        const iconProps = {
            className: toggleClass,
        };
        const headerClass = this.state.expandList
            ? 'title h-tertiary float-left mb-2' : 'title h-tertiary float-left';
        const containerClass = renderContent ? 'col-xs-12 link' : 'col-xs-12';
        return (
            <div className="row">
                <div className={containerClass} onClick ={() => this.toggleExpansion()}>
                    <h2 className={headerClass}>
                        {title}
                    </h2>
                    {renderContent
                        && <span {...iconProps} /> }
                </div>
            </div>
        );
    }
    render() {
        const siteConfiguration = this.props.site.SiteConfiguration;
        const displayDirectDebits = siteConfiguration.TradeConfiguration.DisplayDirectDebits
            && this.props.directDebit.isLoaded
            && this.props.directDebit.AllowAgentReadOnlyAccess;
        const renderContent = displayDirectDebits
            && this.props.directDebit.directDebits.length > 0;
        const renderList = this.state.expandList;

        let containerClass = 'container';
        containerClass += displayDirectDebits ? ' displayed' : '';

        return (
            <div className={containerClass}>
                {displayDirectDebits
                    && this.renderHeader(renderContent)}
                {renderContent
                    && renderList
                    && <DirectDebitList {...this.getListProps()} />}
            </div>
        );
    }
}

DirectDebitListContainer.propTypes = {
    brands: React.PropTypes.array.isRequired,
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    directDebit: React.PropTypes.object.isRequired,
    pricingConfiguration: React.PropTypes.object.isRequired,
    selectCurrency: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const directDebit = state.directDebit.hasOwnProperty('directDebits')
        ? state.directDebit : { directDebits: [] };
    const pricingConfiguration = state.site.SiteConfiguration.PricingConfiguration
        ? state.site.SiteConfiguration.PricingConfiguration : {};
    const selectCurrency = state.session.UserSession.SelectCurrency
        ? state.session.UserSession.SelectCurrency : {};
    const brands = state.brands ? state.brands : {};
    return {
        brands,
        directDebit,
        pricingConfiguration,
        selectCurrency,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        BrandActions,
        DirectDebitActions,
        EntityActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(DirectDebitListContainer);
