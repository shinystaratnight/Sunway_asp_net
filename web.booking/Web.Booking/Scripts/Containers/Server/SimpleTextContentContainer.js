import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';
import SimpleTextContent from '../../widgets/content/simpletextcontent';

export default class SimpleTextContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getSimpleTextContentProps() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const props = {
            backgroundImage: contentModel.BackgroundImage,
            markdown: contentModel.Markdown,
            textAlignment: contentModel.Configuration.TextAlignment,
            button: contentModel.Button,
        };
        return props;
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const containerAttributes
            = ServerContainerFunctions.setupContainerAttributes('simpletextcontent', contentModel);
        return (
            <div {...containerAttributes}>
                <SimpleTextContent {...this.getSimpleTextContentProps()} />
            </div>
        );
    }
}

SimpleTextContentContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
};
