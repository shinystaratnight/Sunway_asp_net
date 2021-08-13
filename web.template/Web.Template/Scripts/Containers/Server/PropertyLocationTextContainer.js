import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';
import SimpleTextContent from '../../widgets/content/simpletextcontent';

export default class PropertyLocationTextContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    getMarkdownText(iVectorProperty) {
        const destination = iVectorProperty.Destination.toUpperCase();
        const name = iVectorProperty.Name;
        const resort = iVectorProperty.Resort;
        const country = iVectorProperty.Country;
        const location = `${resort}, ${country}`;
        const markdown = `${destination} \n # ${name} \n ### ${location}`;
        return (
            markdown
        );
    }
    setupSimpleTextContentProps(siteBuilderProperty, iVectorProperty) {
        const props = {
            backgroundImage: siteBuilderProperty.BannerImage,
            markdown: this.getMarkdownText(iVectorProperty),
            textAlignment: 'Center',
        };
        return props;
    }
    renderContent(siteBuilderProperty, iVectorProperty) {
        const containerAttributes = {
            className: 'widget padding-md overlay',
            style: {
                backgroundImage: `url(${siteBuilderProperty.BannerImage})`,
            },
        };

        const textProps = this.setupSimpleTextContentProps(siteBuilderProperty, iVectorProperty);
        return (
            <div {...containerAttributes}>
                <SimpleTextContent {...textProps} />
            </div>
        );
    }

    render() {
        const entityTypes = [
            {
                name: 'siteBuilderProperty',
                type: 'Property',
                source: 'SiteBuilder',
            },
            {
                name: 'iVectorProperty',
                type: 'Property',
                source: 'iVector',
            },
        ];

        const specificEntities = ServerContainerFunctions.getSpecificEntities(entityTypes,
            this.props.specificEntitiesCollection);
        const siteBuilderProperty = specificEntities.siteBuilderProperty;
        const iVectorProperty = specificEntities.iVectorProperty.Property;

        const canRender = siteBuilderProperty.BannerImage !== '';

        const content = canRender
            ? this.renderContent(siteBuilderProperty, iVectorProperty) : '';
        return (
            <div>{content}</div>
        );
    }
}

PropertyLocationTextContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
};
