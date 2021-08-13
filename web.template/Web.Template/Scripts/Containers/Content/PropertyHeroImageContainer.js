import * as EntityActions from '../../actions/content/entityActions';
import * as PropertyActions from '../../actions/content/propertyActions';

import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PropertyHeroImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
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
    componentDidMount() {
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
    getiVectorProperty() {
        const propertyKey = `PropertyFull-${this.getPropertyID()}`;
        const iVectorPropertyResponse
            = this.props.cmsContent[propertyKey] ? this.props.cmsContent[propertyKey] : {};
        return iVectorPropertyResponse.Property;
    }
    getSiteBuilderProperty() {
        const siteBuilderPropertyKey = `Property-${this.getPropertyName()}`;
        const siteBuilderProperty = this.props.entities[siteBuilderPropertyKey]
            ? this.props.entities[siteBuilderPropertyKey].model : {};
        return siteBuilderProperty;
    }
    setupCarouselContentProps() {
        const siteBuilderProperty = this.getSiteBuilderProperty();
        const iVectorProperty = this.getiVectorProperty();
        const contentModel = this.props.entity.model;

        const carouselItems = [
            {
                Image: siteBuilderProperty.HeroImage,
                Caption: {
                    Header: iVectorProperty.Name,
                    SubHeader: iVectorProperty.Country,
                    Rating: parseFloat(iVectorProperty.Rating),
                    Text: iVectorProperty.Summary,
                },
                Configuration: {
                    Caption: {
                        VerticalPosition: 'Middle',
                        HorizontalPosition: 'Left',
                        DesktopTextWidth: 5,
                    },
                },
            },
        ];
        let carouselHeight = 'Extra Large';
        const carouselHeightMobile = 'Extra Small';
        if (contentModel.Configuration.Style === 'Small') {
            carouselHeight = 'Small';
        }

        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 0,
            carouselHeight,
            carouselHeightMobile,
            arrowOffsetY: 0,
            autoScroll: true,
        };
        return carouselContentProps;
    }
    render() {
        const iVectorProperty = this.getiVectorProperty();
        const siteBuilderProperty = this.getSiteBuilderProperty();
        const shouldRender = iVectorProperty && iVectorProperty.hasOwnProperty('Name')
            && siteBuilderProperty && siteBuilderProperty.hasOwnProperty('HeroImage');
        return (
            <div className="widget padding-none carousel">
                {shouldRender
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

PropertyHeroImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    cmsContent: React.PropTypes.object,
    entities: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const cmsContent = state.cmsContent ? state.cmsContent : {};
    const entities = state.entities ? state.entities : {};
    return {
        cmsContent,
        entities,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        PropertyActions,
        EntityActions
    );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PropertyHeroImageContainer);
