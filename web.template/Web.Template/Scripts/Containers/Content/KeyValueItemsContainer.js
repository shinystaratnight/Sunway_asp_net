import KeyValueItems from '../../widgets/content/keyvalueitems';
import React from 'react';

class KeyValueItemsContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;

        const keyValueItemsProps = {
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            keyValueItems: contentModel.KeyValueItems,
            button: contentModel.Button,
            configuration,
        };

        return (
            <KeyValueItems {...keyValueItemsProps} />
        );
    }
}

KeyValueItemsContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default KeyValueItemsContainer;
