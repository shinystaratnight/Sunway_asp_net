import React from 'react';
import TileContent from '../../widgets/content/tilecontent';

class TileContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    sortBySequence(a, b) {
        return a.Sequence - b.Sequence;
    }
    tileContentProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;

        const props = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            browser: this.props.session.UserSession.Browser
                ? this.props.session.UserSession.Browser : '',
            title: contentModel.Title,
            leadParagraph: contentModel.LeadParagraph,
            leadTileHeight: configuration.LeadTileHeight,
            leadTileType: contentModel.LeadTileType,
            leadTileItem: contentModel.LeadTileItem ? contentModel.LeadTileItem : {},
            gridType: configuration.GridType,
            tileRows: configuration.TileRows,
            tileColumns: configuration.TileColumns,
            tileColumnsMobile: configuration.TileColumnsMobile
                ? configuration.TileColumnsMobile : 1,
            tileColumnsTablet: configuration.TileColumnsTablet
                ? configuration.TileColumnsTablet : 1,
            tileHeight: configuration.TileHeight,
            tilesPaddingTop: configuration.TilesPaddingTop,
            tilesPaddingBottom: configuration.TilesPaddingBottom,
            tileType: contentModel.TileType,
            tileItems: contentModel.TileItems ? contentModel.TileItems : [],
        };
        if (props.tileItems) {
            props.tileItems.sort(this.sortBySequence);
        }
        if (contentModel.Button) {
            props.buttonText = contentModel.Button.Text;
            props.buttonURL = contentModel.Button.URL;
            props.buttonStyle = contentModel.Button.Style;
        }

        return props;
    }
    getContainerAttributes() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;

        const containerAttributes = {
            className: `container tile-container ${this.props.entity.model.TileType}`,
        };

        switch (configuration.ContainerStyle) {
            case 'Highlight':
                containerAttributes.className = `${containerAttributes.className} highlight`;
                break;
            case 'Alternate':
                containerAttributes.className = `${containerAttributes.className} alt`;
                break;
            default:
                break;
        }
        return containerAttributes;
    }
    render() {
        return (
            <div {...this.getContainerAttributes()}>
                <TileContent {...this.tileContentProps()} />
            </div>
        );
    }
}

TileContentContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
};

export default TileContentContainer;
