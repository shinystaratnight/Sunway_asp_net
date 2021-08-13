import AwardsPanel from 'widgets/content/awardspanel';
import React from 'react';

class AwardsPanelContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getAwardsPanelProps() {
        const contentModel = this.props.entity.model;
        const props = {
            title: contentModel.Title,
            awards: contentModel.Awards,
        };
        return props;
    }
    render() {
        return (
            <AwardsPanel {...this.getAwardsPanelProps()} />
        );
    }
}

AwardsPanelContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default AwardsPanelContainer;
