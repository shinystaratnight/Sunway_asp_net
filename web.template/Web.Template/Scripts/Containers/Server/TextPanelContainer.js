import React from 'react';
import TextPanel from '../../widgets/content/textpanel';

export default class TextPanelContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getContainerClass() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const configuration = contentModel.Configuration;

        let containerClass = 'widget widget-container widget-textpanel';
        switch (configuration.ContainerPadding) {
            case 'None':
                containerClass += ' padding-none';
                break;
            case 'Small':
                containerClass += ' padding-sm';
                break;
            case 'Medium':
                containerClass += ' padding-md';
                break;
            case 'Large':
                containerClass += ' padding-lg';
                break;
            default:
        }

        return containerClass;
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const textPanelProps = {
            title: contentModel.Title,
            markdown: contentModel.Markdown,
        };

        return (
            <div className={this.getContainerClass()}>
                <TextPanel {...textPanelProps} />
            </div>
        );
    }
}

TextPanelContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
