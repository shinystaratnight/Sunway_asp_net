import KeyValueItems from '../../widgets/content/keyvalueitems';
import React from 'react';

export default class KeyValueItemsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    render() {
        const page = this.props.page;
        const containerAttributes = {
            className: 'widget widget-key-value-items',
        };
        const contentModel = JSON.parse(this.props.contentJSON);

        const keyValueItemsProps = {
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            keyValueItems: contentModel.KeyValueItems,
            button: contentModel.Button,
            configuration: contentModel.Configuration,
            page,
        };

        switch (keyValueItemsProps.configuration.ContainerStyle) {
            case 'Highlight':
                containerAttributes.className = `${containerAttributes.className} highlight`;
                break;
            case 'Alternate':
                containerAttributes.className = `${containerAttributes.className} alt`;
                break;
            default:
                containerAttributes.className = `${containerAttributes.className} default`;
        }

        switch (keyValueItemsProps.configuration.ContainerPadding) {
            case 'Small':
                containerAttributes.className = `${containerAttributes.className} padding-sm`;
                break;
            case 'Medium':
                containerAttributes.className = `${containerAttributes.className} padding-md`;
                break;
            case 'Large':
                containerAttributes.className = `${containerAttributes.className} padding-lg`;
                break;
            default:
                containerAttributes.className = `${containerAttributes.className} padding-none`;
        }

        return (
            <div {...containerAttributes}>
                <KeyValueItems {...keyValueItemsProps}/>
            </div>
        );
    }
}

KeyValueItemsContainer.propTypes = {
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
};
