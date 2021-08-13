import 'widgets/content/_specialoffers.scss';

import * as SpecialOfferActions from 'actions/custom/specialOfferActions';
import CarouselContent from 'widgets/content/carouselcontent';

import React from 'react';
import StringFunctions from 'library/stringfunctions';
import Tabs from 'components/common/tabs';
import TileContent from 'widgets/content/tilecontent';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SpecialOffersContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedTab: 'unset',
            selectedTabIndex: 0,
            offersInit: false,
        };
        this.renderOfferTiles = this.renderOfferTiles.bind(this);
        this.renderFeaturedHotels = this.renderFeaturedHotels.bind(this);
        this.renderTabs = this.renderTabs.bind(this);
        this.renderContent = this.renderContent.bind(this);
        this.buildSpecialOfferRequest = this.buildSpecialOfferRequest.bind(this);
    }

    sortFilterSettings(filtersA, filtersB) {
        return (filtersA.Sequence - filtersB.Sequence);
    }
    componentWillReceiveProps() {
        const shouldDisplay = this.shouldDisplay();
        if (!shouldDisplay) {
            const offers = this.props.specialOffers;
            this.props.entity.model.SpecialOfferFilters.forEach(sof => {
                const thisKey = `${this.props.context}-${sof.TabDisplayName}`;
                const shouldDisplayThisTab = offers[thisKey]
                    && offers[thisKey].items.length >= this.getMinimumItems();
                if (shouldDisplayThisTab && sof.TabDisplayName !== this.state.selectedTab) {
                    this.handleTabChange(sof.TabDisplayName);
                }
            });
        }
    }
    componentDidMount() {
        const entity = this.props.entity;
        if (entity.model
            && entity.model.SpecialOfferFilters) {
            const sortedOfferFilters
                    = entity.model.SpecialOfferFilters.sort(this.sortFilterSettings);
            const defaultSelectedTab = sortedOfferFilters[0].TabDisplayName;
            const offerFilters = entity.model.SpecialOfferFilters;
            const siteConfiguration = this.props.site.SiteConfiguration;
            offerFilters.forEach(of => {
                const request = this.buildSpecialOfferRequest(of);
                this.props.actions.getSpecialOffers(request, siteConfiguration);
            });
            this.setState({
                selectedTab: defaultSelectedTab,
                offersInit: true,
            });
        }
    }
    handleTabChange(newTabName, index) {
        this.setState({
            selectedTab: newTabName,
            selectedTabIndex: index,
        });
    }
    buildSpecialOfferRequest(filterSettings) {
        const key = `${this.props.context}-${filterSettings.TabDisplayName}`;
        const offerRequest = {
            tabName: filterSettings.TabDisplayName,
            key,
            geographyLevel1: filterSettings.Country,
            geographyLevel2: filterSettings.Region,
            numberOfOffers: filterSettings.NumberOfOffers,
            productAtttribute: filterSettings.ProductAttribute,
            orderBy: filterSettings.OrderBy,
            includeLeadIns: !filterSettings.HighlightedPropertiesOnly,
            highlightedPropertiesOnly: filterSettings.HighlightedPropertiesOnly,
        };

        if (this.props.page.EntityInformations) {
            this.props.page.EntityInformations.forEach(entityInformation => {
                if (entityInformation.Name.toLowerCase()
                    === 'productattribute') {
                    if (offerRequest.productAtttribute !== '') {
                        offerRequest.productAtttribute += ',';
                    }
                    offerRequest.productAtttribute += entityInformation.Id;
                }
            });
        }
        return offerRequest;
    }
    renderTabs() {
        const contentModel = this.props.entity.model;
        const specialOfferFilters = contentModel.SpecialOfferFilters;
        const tabs = [];

        for (let i = 0; i < specialOfferFilters.length; i++) {
            const specialOfferFilter = specialOfferFilters[i];
            const key = `${this.props.context}-${specialOfferFilter.TabDisplayName}`;
            const hasLoadedOffers
                = this.props.specialOffers[key].items.length >= this.getMinimumItems();
            if (hasLoadedOffers) {
                const tab = {
                    name: specialOfferFilter.TabDisplayName,
                    value: i.toString(),
                    onClick: this.handleTabChange.bind(this, specialOfferFilter.TabDisplayName, i),
                };
                tabs.push(tab);
            }
        }
        const tabsProps = {
            selectedTab: this.state.selectedTabIndex.toString(),
            tabs,
            tabContainerStyle: contentModel.Configuration.TabsContainerStyle,
            selectedTabStyle: contentModel.Configuration.ContainerStyle,
            enableMobileTabs: true,
        };
        return (
            <div>
                <Tabs {...tabsProps} />
            </div>
        );
    }
    renderOfferTiles() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const specialOfferKey = `${this.props.context}-${this.state.selectedTab}`;
        const specialOffers = this.props.specialOffers[specialOfferKey].items;
        const filterOptions = contentModel.SpecialOfferFilters
            .filter(f => f.TabDisplayName === this.state.selectedTab)[0];
        const tileItems = [];
        for (let i = 0; i < specialOffers.length; i++) {
            const specialOffer = specialOffers[i];
            const information = specialOffer.AdditionalInformation;
            const source = !filterOptions.HighlightedPropertiesOnly
                && specialOffer.ChosenByUs === '1' ? 'Chosen By Us' : specialOffer.Source;
            const property = {
                Name: specialOffer.HotelName,
                Resort: specialOffer.HotelResort,
                Country: specialOffer.HotelRegion,
                Rating: specialOffer.HotelRating,
                Source: source,
                Duration: specialOffer.AdditionalInformation.Duration,
                Price: specialOffer.Price,
                Currency: this.props.session.UserSession.SelectCurrency,
                Image: {
                    Source: specialOffer.MainImage,
                },
            };
            const tileItem = {
                Property: property,
                URL: `/${specialOffer.AdditionalInformation.Url}`,
            };
            if (information) {
                tileItem.OfferText = '';
                tileItem.OfferPricing = information.Price;
                tileItems.push(tileItem);
            }
        }
        const filter = contentModel.SpecialOfferFilters
            .filter(f => f.TabDisplayName === this.state.selectedTab)[0];
        const haveOverrideButton = filter.hasOwnProperty('Button')
            ? filter.Button.hasOwnProperty('Text') && filter.Button.Text !== ''
                && filter.Button.hasOwnProperty('URL') && filter.Button.URL !== ''
            : false;
        const button = haveOverrideButton ? filter.Button : contentModel.Button;

        const numColumns = 2;
        const numRows = Math.floor(tileItems.length / numColumns);
        const offerTilesProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: '',
            leadParagraph: '',
            gridType: 'Rows',
            tileColumns: numColumns,
            tileRows: numRows,
            tileColumnsTablet: 1,
            tileColumnsMobile: 1,
            tileHeight: 220,
            tilesPaddingTop: 0,
            tilesPaddingBottom: 0,
            tileType: 'OfferTile',
            tileItems,
            buttonText: button.Text,
            buttonURL: button.URL,
            buttonStyle: 'Default',
        };

        let containerClass = 'tile-container';

        switch (configuration.ContainerStyle) {
            case 'Highlight':
                containerClass = `${containerClass} highlight`;
                break;
            case 'Alternate':
                containerClass = `${containerClass} alt`;
                break;
            default:
                break;
        }

        return (
            <div className={containerClass}>
                <TileContent {...offerTilesProps} />
            </div>
        );
    }
    renderFeaturedHotels(mode) {
        const contentModel = this.props.entity.model;
        const specialOfferKey = `${this.props.context}-${this.state.selectedTab}`;
        const specialOffers = this.props.specialOffers[specialOfferKey].items;
        const carouselItems = [];
        for (let i = 0; i < specialOffers.length; i++) {
            const specialOffer = specialOffers[i];
            const information = specialOffer.AdditionalInformation;
            const filterOptions = contentModel.SpecialOfferFilters
                .filter(f => f.TabDisplayName === this.state.selectedTab)[0];
            const source = !filterOptions.HighlightedPropertiesOnly
                && specialOffer.ChosenByUs === '1' ? 'Chosen By Us' : specialOffer.Source;
            const property = {
                Name: specialOffer.HotelName,
                Resort: specialOffer.HotelResort,
                Country: specialOffer.HotelRegion,
                Rating: specialOffer.HotelRating,
                Source: source,
                Duration: information.Duration,
                Mode: mode,
                Price: specialOffer.Price ? specialOffer.Price : 0,
                Currency: this.props.session.UserSession.SelectCurrency,
                Image: {
                    Source: specialOffer.MainImage,
                },
            };

            if (mode === 'standard') {
                property.Summary = specialOffer.Summary;
            }

            const carouselItem = {
                Property: property,
                URL: `/${information.Url}`,
            };

            if (information) {
                carouselItem.OfferText = information.OffersPageText;
                carouselItem.OfferPricing = information.Price;
            } else {
                carouselItem.OfferText = '';
                carouselItem.OfferPricing = '';
            }
            carouselItems.push(carouselItem);
        }
        const carouselProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            title: '',
            leadParagraph: '',
            carouselItemType: 'FeaturedHotel',
            carouselTiles: 3,
            carouselHeight: mode === 'standard' ? 'Large' : 'Small-Plus',
            carouselHeightMobile: 'Extra Extra Large',
            carouselItems,
            autoScroll: false,
            pauseOnHover: false,
            delay: 1000,
            displayArrows: false,
            arrowOffsetY: 0,
            indicatorType: 'Circles',
            indicatorPosition: 'Center',
            transitionType: 'Scroll',
            transitionDuration: 1000,
            transitionTiming: 'Ease',
            sidePreview: false,
            buttonText: contentModel.Button.Text,
            buttonURL: contentModel.Button.URL,
        };

        return (
            <div className="carousel-content">
                <CarouselContent {...carouselProps} />
            </div>
        );
    }
    renderContent() {
        const contentModel = this.props.entity.model;
        switch (contentModel.TileType) {
            case 'OfferTile':
                return this.renderOfferTiles();
            case 'FeaturedHotel':
                return this.renderFeaturedHotels('standard');
            case 'FeaturedHotelCollapsed':
                return this.renderFeaturedHotels('collapsed');
            default:
                return (<div></div>);
        }
    }
    renderTitleAndLeadParagraph(contentModel) {
        return (
            <div className="col-xs-12 center-block">
                {contentModel.Title !== ''
                && <h2 className="h-secondary">{contentModel.Title}</h2>}

                {contentModel.LeadParagraph !== ''
                    && <p className="preamble">
                        {contentModel.LeadParagraph}</p>}
            </div>
        );
    }
    getContainerStyle(incomingClasses, containerStyle, containerPadding) {
        let classes = incomingClasses;
        switch (containerStyle) {
            case 'Highlight':
                classes = `${classes} highlight`;
                break;
            case 'Alternate':
                classes = `${classes} alt`;
                break;
            default:
                classes = `${classes} default`;
                break;
        }
        switch (containerPadding) {
            case 'None':
                classes = `${classes} padding-none`;
                break;
            default:
        }
        return (
            classes
        );
    }
    getMinimumItems() {
        const offerTileMinimum = 2;
        const featuredHotelTileMinimum = 3;
        const numTilesMinimum = this.props.entity.model.TileType === 'OfferTile'
            ? offerTileMinimum : featuredHotelTileMinimum;
        return numTilesMinimum;
    }
    shouldDisplay() {
        const offers = this.props.specialOffers;
        const specialOfferKey = `${this.props.context}-${this.state.selectedTab}`;
        const shouldDisplay = offers[specialOfferKey]
            && offers[specialOfferKey].items.length >= this.getMinimumItems();
        return shouldDisplay;
    }
    renderAllContent() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const contentClasses = this.getContainerStyle(
                            'widget-bottom',
                            configuration.ContainerStyle,
                            configuration.ContainerPadding);

        const tabAreaClasses = this.getContainerStyle(
                               'widget-top',
                               configuration.TabsContainerStyle,
                               '');

        const specialOfferKey = `${this.props.context}-${this.state.selectedTab}`;
        return (
            <div>
                <div className={tabAreaClasses}>
                    <div className="container">
                        {(!StringFunctions.isNullOrEmpty(contentModel.Title)
                        || !StringFunctions.isNullOrEmpty(contentModel.LeadParagraph))
                            && this.renderTitleAndLeadParagraph(contentModel)}
                        {contentModel.SpecialOfferFilters
                            && contentModel.SpecialOfferFilters.length > 1
                            && this.renderTabs()}
                    </div>
                </div>
                <div className={contentClasses}>
                    <div className="container">
                    {this.props.specialOffers[specialOfferKey]
                            && this.props.specialOffers[specialOfferKey].items.length > 0
                            && this.renderContent()}
                    </div>
                        </div>
                    </div>
        );
    }
    render() {
        const shouldDisplay = this.shouldDisplay();
        const containerClass = shouldDisplay ? 'special-offers-display' : 'special-offers-hide';
        return (
            <div className={containerClass}>
                {shouldDisplay
                    && this.renderAllContent()}
            </div>
        );
    }
}

SpecialOffersContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    specialOffers: React.PropTypes.object.isRequired,
    site: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The props from the container.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const specialOffers = state.specialOffers ? state.specialOffers : {};

    return {
        specialOffers,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        SpecialOfferActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SpecialOffersContainer);
