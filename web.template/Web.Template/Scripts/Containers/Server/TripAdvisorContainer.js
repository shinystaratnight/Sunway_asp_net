import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';
import TripAdvisor from '../../widgets/content/tripadvisor';

export default class TripAdvisorContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getTripAdvisorProps() {
        const entityTypes = [
            {
                name: 'siteBuilderProperty',
                type: 'Property',
                source: 'SiteBuilder',
            },
            {
                name: 'iVectorProperty',
                type: 'Property',
                source: 'iVector',
            },
        ];
        const specificEntities
            = ServerContainerFunctions.getSpecificEntities(entityTypes,
                this.props.specificEntitiesCollection);
        const siteBuilderProperty = specificEntities.siteBuilderProperty;
        const iVectorProperty = specificEntities.iVectorProperty.Property;
        const props = {
            tripAdvisorScript: siteBuilderProperty.TripAdvisorScript,
            rating: iVectorProperty.Rating,
            iVectorProperty,
            siteConfiguration: this.props.site.SiteConfiguration,
        };
        return props;
    }
    render() {
        return (
            <div className="widget padding-sm highlight">
                <TripAdvisor {...this.getTripAdvisorProps()} />
            </div>
        );
    }
}

TripAdvisorContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
    site: React.PropTypes.object,
};
