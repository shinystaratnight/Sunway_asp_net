import ClimateGrid from '../../widgets/content/climategrid';
import React from 'react';

class ClimateGridContainer extends React.Component {
    getClimateGridProps() {
        const contentModel = this.props.entity.model;
        const climate = contentModel.Region ? contentModel.Region.Climate : {};
        const props = {
            title: climate.Title ? climate.Title : '',
            leadParagraph: climate.Summary ? climate.Summary : '',
            measures: climate.Measures ? climate.Measures : [],
            monthNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May',
                'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'],
        };
        return props;
    }
    render() {
        return (
            <ClimateGrid {...this.getClimateGridProps()} />
        );
    }
}

ClimateGridContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default ClimateGridContainer;
