import React from 'react';
import SimpleTextContent from '../../widgets/content/simpletextcontent';

class SimpleTextContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getSimpleTextContentProps() {
        const contentModel = this.props.entity.model;
        const props = {
            backgroundImage: contentModel.BackgroundImage,
            markdown: contentModel.Markdown,
            textAlignment: contentModel.Configuration.TextAlignment,
            button: contentModel.Button,
        };
        return props;
    }
    render() {
        return (
            <SimpleTextContent {...this.getSimpleTextContentProps()} />
        );
    }
}

SimpleTextContentContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default SimpleTextContentContainer;
