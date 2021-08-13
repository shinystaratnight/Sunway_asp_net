import * as AirportActions from '../../actions/lookups/airportActions';
import * as EntityActions from '../../actions/content/entityActions';
import * as PageActions from '../../actions/content/pageActions';
import * as SpecialOfferActions from '../../actions/custom/specialOfferActions';

import React from 'react';
import SpecialOfferDetails from '../../widgets/content/specialofferdetails';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class SpecialOfferDetailsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            entity: {
                isLoaded: false,
                model: {
                    Configuration: {},
                },
            },
            specialOffer: {
                isLoaded: false,
            },
            lastModifiedDate: new Date(),
            lastModifiedUser: '',
        };
    }
    getSpecialOfferID() {
        let id = 0;
        for (let i = 0; i < this.props.page.EntityInformations.length; i++) {
            const entityInfo = this.props.page.EntityInformations[i];
            if (entityInfo.Name === 'Special Offer') {
                id = entityInfo.Id;
                break;
            }
        }
        return id;
    }
    getPropertyID() {
        let id = 0;
        for (let i = 0; i < this.props.page.EntityInformations.length; i++) {
            const entityInfo = this.props.page.EntityInformations[i];
            if (entityInfo.Name === 'Property') {
                id = entityInfo.Id;
                break;
            }
        }
        return id;
    }
    getPropertyName() {
        let name = '';
        for (let i = 0; i < this.props.page.EntityInformations.length; i++) {
            const entityInfo = this.props.page.EntityInformations[i];
            if (entityInfo.Name === 'Property') {
                name = entityInfo.Value;
                break;
            }
        }

        return name;
    }
    buildSpecialOfferRequest(propertyId) {
        const key = 'specialOfferDetailsList';
        const offerRequest = {
            tabName: 'specialOfferDetailsList',
            key,
            geographyLevel1: 0,
            geographyLevel2: 0,
            numberOfOffers: 10,
            productAtttribute: '',
            orderBy: 'Newest',
            propertyReference: propertyId,
            includeLeadIns: 'false',
            highlightedPropertiesOnly: 'false',
        };
        return offerRequest;
    }
    componentWillReceiveProps(nextProps) {
        if (!this.state.offersInit) {
            const propertyId = this.getPropertyID();
            if (propertyId > 0) {
                const offerRequest
                        = this.buildSpecialOfferRequest(propertyId);
                const siteConfiguration = this.props.site.SiteConfiguration;
                this.props.actions.getSpecialOffers(offerRequest, siteConfiguration);
                this.setState({
                    offersInit: true,
                });
            }
        }
        if (!this.state.propertyInit && nextProps.specialOffer.isLoaded) {
            const propertyName = nextProps.specialOffer.offer.HotelName;
            if (propertyName) {
                this.props.actions.loadEntity(this.props.site.Name,
                    'Property', propertyName, 'live');
                this.setState({
                    propertyInit: true,
                });
            }
        }

        if (!this.state.cmsPropertyLoaded) {
            const propertyId = this.getPropertyID();
            const propertyKey = `PropertyFull-${propertyId}`;
            if (nextProps.cmsContent[propertyKey]) {
                const property = nextProps.cmsContent[propertyKey];
                if (property && property.isLoaded) {
                    this.setState({
                        cmsPropertyLoaded: true,
                    });
                    this.props.actions.addPageContent('propertyFull', property.Property);
                }
            }
        }

        if (this.state.propertyInit && !this.state.propertyLoaded) {
            let propertyName;
            if (nextProps.specialOffer.offer) {
                propertyName = nextProps.specialOffer.offer.HotelName;
            } else {
                propertyName = this.getPropertyName();
            }
            if (propertyName) {
                const propertyKey = `Property-${propertyName}`;
                if (nextProps.entities[propertyKey]) {
                    const property = nextProps.entities[propertyKey];
                    if (property && property.isLoaded) {
                        this.setState({
                            propertyLoaded: true,
                        });
                        this.props.actions.addPageContent('property', property.model);
                    }
                }
            }
        }
    }
    componentDidMount() {
        const specialOfferId = this.getSpecialOfferID();
        if (specialOfferId > 0) {
            this.props.actions.getSpecialOffer(specialOfferId, 'specialOfferDetails');
        }
        this.props.actions.loadAirportsIfNeeded();
        const propertyName = this.getPropertyName();
        if (propertyName) {
            this.props.actions.loadEntity(this.props.site.Name, 'Property', propertyName, 'live');
            const propertyId = this.getPropertyID();
            this.props.actions.loadCMSModel('PropertyFull', propertyId);
            this.setState({
                propertyInit: true,
            });
        }
    }
    isInitialised() {
        return this.props.airports.isLoaded
            && ((this.props.specialOffer && this.props.specialOffer.isLoaded)
            || (this.props.specialOfferList && this.props.specialOfferList.isLoaded)
            );
    }
    getMode() {
        let mode = 'noOffer';
        if (this.props.specialOffer.offer
            && this.props.specialOffer.offer.AdditionalInformation) {
            mode = 'singleOffer';
        } else if (this.props.specialOfferList.isLoaded
            && this.props.specialOfferList.items.length > 0) {
            mode = 'offerList';
        }
        return mode;
    }
    toggleLoginMenu() {
        const loginWindowButton = document.querySelector('.header-account.header-top-nav-text');

        if (!document.querySelector('.user-menu.login')) {
            loginWindowButton.click();
        }
    }
    scrollTo(scrollToPosition) {
        const scrollY = window.scrollY || window.pageYOffset;
        let scrollTotal = scrollToPosition - scrollY;
        const timeToScroll = 150;
        const scrollStep = Math.round(scrollTotal / timeToScroll);
        let finalStep = false;
        const timePerStep = 1;
        const scrollInterval = setInterval(() => {
            if (finalStep === false) {
                window.scrollBy(0, scrollStep);
            } else {
                clearInterval(scrollInterval);
            }
            if (Math.abs(scrollTotal) > Math.abs(scrollStep)) {
                scrollTotal = scrollTotal - scrollStep;
            } else {
                window.scrollTo(0, scrollToPosition + 1);
                finalStep = true;
            }
        }, timePerStep);
    }

    render() {
        const contentModel = this.props.entity.model;
        const detailProps = {
            userLoggedIn: this.props.session.UserSession.LoggedIn,
            currency: this.props.session.UserSession.SelectCurrency,
        };
        if (this.isInitialised()) {
            const offer = this.props.specialOffer.offer;
            if (offer && offer.AdditionalInformation) {
                const airportPrices = Array.isArray(offer.AirportPrices)
                    ? offer.AirportPrices : [offer.AirportPrices];
                const price = Math.min.apply(null,
                    airportPrices.map(item => item.PerPersonPrice));
                detailProps.offerID = this.getSpecialOfferID();
                detailProps.offerName = offer.AdditionalInformation.Name;
                detailProps.durationOfStay = offer.AdditionalInformation.DurationOfStay;
                detailProps.priceIncludes = offer.AdditionalInformation.PriceIncludes;
                detailProps.smallPrint = offer.AdditionalInformation.Smallprint;
                detailProps.smallPrintATOL = offer.AdditionalInformation.SmallPrintATOL;
                detailProps.price = price;
                detailProps.propertyURL = offer.AdditionalInformation.PropertyURL;
                detailProps.OfferSaving = offer.AdditionalInformation.OfferSaving;
                detailProps.offerPageText = offer.AdditionalInformation.OfferPageText;
                detailProps.airportPrices = airportPrices;
                detailProps.airports = this.props.airports.items ? this.props.airports.items : [];
                detailProps.getOfferPoster = this.props.actions.getSpecialOfferPoster;
            }
            detailProps.loggedOutText = contentModel.LoggedOutText;
            detailProps.iVectorProperty = this.props.propertyFull;
            detailProps.offerMessage = contentModel.OfferMessage;
            detailProps.cmsWebsiteName
                = this.props.session.UserSession.SelectedCmsWebsite.CountryCode;

            detailProps.mode = this.getMode();
            if (this.props.property) {
                detailProps.property = this.props.property;
            }
            detailProps.offerFooter = contentModel.OfferFooter;
            detailProps.offers = this.props.specialOfferList;
            detailProps.toggleLoginMenu = this.toggleLoginMenu.bind(this);
            detailProps.scrollTo = this.scrollTo.bind(this);
        }
        return (
            <div>
                {this.isInitialised()
                    && <SpecialOfferDetails {...detailProps}/>}
            </div>
        );
    }
}

SpecialOfferDetailsContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    cmsContent: React.PropTypes.object.isRequired,
    airports: React.PropTypes.object.isRequired,
    property: React.PropTypes.object.isRequired,
    propertyFull: React.PropTypes.object.isRequired,
    specialOffer: React.PropTypes.object.isRequired,
    specialOfferList: React.PropTypes.object.isRequired,
    userSession: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const airports = state.airports ? state.airports : {};
    const entities = state.entities ? state.entities : {};

    const cmsContent = state.cmsContent ? state.cmsContent : {};

    const property = state.page && state.page.content && state.page.content.property
        ? state.page.content.property : {};

    const propertyFull = state.page && state.page.content && state.page.content.propertyFull
        ? state.page.content.propertyFull : {};

    const specialOffer = state.specialOffers.specialOfferDetails
        ? state.specialOffers.specialOfferDetails : {};

    const specialOfferList = state.specialOffers.specialOfferDetailsList
        ? state.specialOffers.specialOfferDetailsList : {};

    const userSession
        = state.session && state.session.UserSession ? state.session.UserSession : {};

    return {
        airports,
        entities,
        cmsContent,
        property,
        propertyFull,
        specialOffer,
        specialOfferList,
        userSession,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        AirportActions,
        EntityActions,
        SpecialOfferActions,
        PageActions);
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(SpecialOfferDetailsContainer);
