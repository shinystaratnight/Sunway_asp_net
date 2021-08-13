import AccordionContent from '../../widgets/content/accordionContent';
import React from 'react';

class AccordionContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = this.props.entity.model;
        const accordionContentProps = {
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            accordionContent: contentModel.AccordionContent,
            button: contentModel.Button,
            configuration: {
                ContainerStyle: contentModel.Configuration.ContainerStyle,
                HighlightedItemStyle: contentModel.Configuration.HighlightedItemStyle,
                AccordionItemPadding: contentModel.Configuration.AccordionItemPadding,
                ItemsToShowAsDefault: contentModel.Configuration.ItemsToShowAsDefault,
            },
        };
        return (
            <AccordionContent {...accordionContentProps} />
        );
    }
}

AccordionContentContainer.propTypes = {
    entity: React.PropTypes.object.isRequired,
};

export default AccordionContentContainer;
