import * as EntityActions from '../../actions/content/entityActions';

import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class PropertyListImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    componentDidMount() {
        const countryInformation
        = this.props.page.EntityInformations
            && this.props.page.EntityInformations.filter(i => i.Name === 'Country')
            ? this.props.page.EntityInformations.filter(i => i.Name === 'Country')[0]
            : {};
        this.props.actions.loadEntity(this.props.site.Name,
            'Country', countryInformation.Value, 'live');
    }
    getMostSpecifcGeographyName() {
        const countryInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Country')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Country')[0]
                : false;
        const regionInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Region')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Region')[0]
                : false;
        const resortInformation
            = this.props.page.EntityInformations
                && this.props.page.EntityInformations.filter(i => i.Name === 'Resort')
                ? this.props.page.EntityInformations.filter(i => i.Name === 'Resort')[0]
                : false;

        let geographyInformation = regionInformation ? regionInformation : countryInformation;
        geographyInformation = resortInformation ? resortInformation : geographyInformation;
        return geographyInformation.Value;
    }
    setupCarouselContentProps() {
        const siteBuilderCountry = this.props.country.model;
        const contentModel = this.props.entity.model;
        const carouselItems = [
            {
                Image: siteBuilderCountry.HeroImage,
                Caption: {
                    Header: this.getMostSpecifcGeographyName(),
                    SubHeader: '',
                    Text: contentModel.HeroImageText,
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
        const renderContent = this.props.country && this.props.country.isLoaded;
        return (
            <div className=" widget padding-none carousel">
                {renderContent
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

PropertyListImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    country: React.PropTypes.object,
    page: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const countryInformation
        = state.page.EntityInformations
            && state.page.EntityInformations.filter(i => i.Name === 'Country')
            ? state.page.EntityInformations.filter(i => i.Name === 'Country')[0]
            : {};
    const countryEntityKey = `Country-${countryInformation.Value}`;
    const country
        = state.entities && state.entities[countryEntityKey]
        ? state.entities[countryEntityKey] : {};
    return {
        country,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions
        );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(PropertyListImageContainer);
