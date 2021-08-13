import * as PropertyActions from '../../actions/content/propertyActions';

import ArrayFunctions from '../../library/arrayfunctions';
import Paging from '../../components/common/paging';
import PropertyListContent from '../../widgets/content/propertylistcontent';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PropertyListContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            propertiesTop: 0,
            scrollTimeout: null,
        };
        this.handlePageChange = this.handlePageChange.bind(this);
        this.scrollToTop = this.scrollToTop.bind(this);
    }
    isInitialised() {
        const initialised
            = this.props.properties.isLoaded
            && (this.props.properties.items.length > 0);
        return initialised;
    }
    componentDidMount() {
        const key = this.props.context;
        if (!this.props.properties.isLoaded
                && !this.props.properties.isFetching) {
            const propertiesRequest = this.getPropertiesRequest(key);
            this.props.actions.getProperties(propertiesRequest);
        }
    }
    componentDidUpdate(prevProps) {
        if (this.props.properties.isLoaded
            && this.props.properties.currentPage
                !== prevProps.properties.currentPage) {
            const propertyContainerRect
                = document.getElementById('property-list-container').getBoundingClientRect();
            this.state.propertiesTop = propertyContainerRect.top + window.scrollY;
            this.scrollToTop();
        }
    }
    scrollToTop() {
        if (window.scrollY > this.state.propertiesTop) {
            const scrollBy = 50;
            const diff = window.scrollY - this.state.propertiesTop;
            const nextScrollBy = diff > scrollBy ? scrollBy : diff;
            window.scrollBy(0, -nextScrollBy);
            this.scrollTimeout = setTimeout(this.scrollToTop, 10);
        } else {
            clearTimeout(this.scrollTimeout);
        }
    }
    handlePageChange(page) {
        const key = this.props.context;
        this.props.actions.propertiesUpdatePage(page, key);
    }
    getGeographies() {
        const geography = this.props.relevantGeography;
        const geographies = {
            country: 0,
            region: 0,
            resort: 0,
        };
        switch (geography.Name.toLowerCase()) {
            case 'country':
                geographies.country = geography.Id;
                break;
            case 'region':
                geographies.region = geography.Id;
                break;
            case 'resort':
                geographies.resort = geography.Id;
                break;
            default:
        }
        return geographies;
    }
    getPropertiesRequest(key) {
        const contentModel = this.props.entity.model;
        const resultsPerPage = contentModel.PropertiesPerPage;
        const geographies = this.getGeographies();
        const geographyLevel1 = geographies.country.toString();
        const geographyLevel2 = geographies.region.toString();
        const geographyLevel3 = geographies.resort.toString();
        const currencyId = this.props.session.UserSession.SelectCurrency.Id.toString();
        const propertiesRequest = {
            key,
            params: [geographyLevel1, geographyLevel2, geographyLevel3, currencyId],
            resultsPerPage,
        };
        return (
            propertiesRequest
        );
    }
    getPropertiesProps() {
        const contentModel = this.props.entity.model;
        const fallbackPriceText
            = contentModel.FallbackPriceText
            ? contentModel.FallbackPriceText
            : 'Call for price';
        const properties = this.props.properties;
        const propertyStartIndex = (properties.currentPage - 1) * properties.resultsPerPage;
        const propertyEndIndex = propertyStartIndex + properties.resultsPerPage;
        const items = this.props.properties.items.slice(propertyStartIndex, propertyEndIndex);
        const onSortChange = (selectedSort) => this.props.actions.sortProperties(
            this.props.properties.key, this.props.properties.items, selectedSort);
        const props = {
            properties: items,
            destination: this.props.geographyGroup.UrlSafeValue,
            geographyLevel: this.props.relevantGeography.Name.toLowerCase(),
            session: this.props.session ? this.props.session : {},
            fallbackPriceText,
            siteConfiguration: this.props.site.SiteConfiguration,
            sortOptions: this.props.properties.sortOptions,
            selectedSort: this.props.properties.selectedSort,
            onSortChange,
        };
        return props;
    }
    getPagingProps() {
        const pagingProps = {
            totalPages: this.props.properties.totalPages,
            currentPage: this.props.properties.currentPage,
            pageLinks: 5,
            onPageClick: this.handlePageChange,
        };
        return pagingProps;
    }
    renderOffersList() {
        return (
            <div className="container" id="property-list-container">
                <PropertyListContent {...this.getPropertiesProps()} />
                {this.props.properties.totalPages > 1
                    && <Paging {...this.getPagingProps()} />}
            </div>
        );
    }
    render() {
        return (
            <div>
                {this.isInitialised()
                     && this.renderOffersList()}
            </div>
        );
    }
}

PropertyListContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    properties: React.PropTypes.object.isRequired,
    geographyGroup: React.PropTypes.object.isRequired,
    relevantGeography: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The props from the container.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state, ownProps) {
    const properties = state.properties && state.properties[ownProps.context]
        ? state.properties[ownProps.context] : {};
    const geographyGroup
        = state.page.EntityInformations
        && state.page.EntityInformations.filter(i => i.Name === 'GeographyGroup')
        ? state.page.EntityInformations.filter(i => i.Name === 'GeographyGroup')[0]
        : {};

    const geoEntityInformation = state.page.EntityInformations
        .filter(i => (i.Name === 'Country') || (i.Name === 'Region') || (i.Name === 'Resort'));

    const sortedGeoEntityInformation
        = ArrayFunctions.sortByPropertyDescending(geoEntityInformation, 'Name');

    const relevantGeography = geoEntityInformation ? sortedGeoEntityInformation[0] : {};
    return {
        properties,
        geographyGroup,
        relevantGeography,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        PropertyActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PropertyListContainer);
