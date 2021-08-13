import AwardsPanel from 'widgets/content/awardspanel';
import React from 'react';
import ServerContainerFunctions from 'library/servercontainerfunctions';


export default class AwardsPanelContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getAwardsPanelProps() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const props = {
            title: contentModel.Title,
            awards: contentModel.Awards,
        };
        return props;
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const containerAttributes
            = ServerContainerFunctions.setupContainerAttributes('awardspanel', contentModel);
        return (
            <div {...containerAttributes}>
                <AwardsPanel {...this.getAwardsPanelProps()} />
            </div>
        );
    }
}

AwardsPanelContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
