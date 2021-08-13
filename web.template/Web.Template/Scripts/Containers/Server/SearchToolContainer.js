import React from 'react';
import ServerContainerFunctions from 'library/servercontainerfunctions';

export default class SearchToolContainer extends React.Component {
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const configuration = contentModel.configuration;

        const containerAttributes = configuration.DisplayWidget
            ? ServerContainerFunctions.setupContainerAttributes('searchtool', contentModel)
            : 'widget padding-none';
        return (
            <div {...containerAttributes}>
                <div className="widget-search-tool"></div>
            </div>
        );
    }
}

SearchToolContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
