import React from 'react';
import TextPanel from '../../widgets/content/textpanel';

export class TextPanelContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = this.props.entity.model;
        const textPanelProps = {
            title: contentModel.Title,
            markdown: contentModel.Markdown,
        };
        return (
            <TextPanel {...textPanelProps} />
        );
    }
}

TextPanelContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default TextPanelContainer;
