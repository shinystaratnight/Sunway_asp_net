import React from 'react';
import TileContent from '../../widgets/content/tilecontent';

export default class TileContentContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    sortBySequence(a, b) {
        return a.Sequence - b.Sequence;
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const configuration = contentModel.Configuration;

        const user = this.props.user;

        const containerAttributes = {
            className: `widget tile-container ${contentModel.TileType}`,
        };
        switch (configuration.ContainerStyle) {
            case 'Highlight':
                containerAttributes.className = `${containerAttributes.className} highlight`;
                break;
            case 'Alternate':
                containerAttributes.className = `${containerAttributes.className} alt`;
                break;
            default:
        }
        switch (configuration.ContainerPadding) {
            case 'None':
                containerAttributes.className = `${containerAttributes.className} padding-none`;
                break;
            default:
        }
        const tileContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            browser: user ? user.Browser : '',
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
        if (tileContentProps.tileItems) {
            tileContentProps.tileItems.sort(this.sortBySequence);
        }
        if (contentModel.Button) {
            tileContentProps.buttonText = contentModel.Button.Text;
            tileContentProps.buttonURL = contentModel.Button.URL;
            tileContentProps.buttonStyle = contentModel.Button.Style;
        }
        return (
            <div {...containerAttributes}>
                <div className="container">
                    {contentModel.TileItems.length > 0
                        && <TileContent {...tileContentProps} />}
                </div>
            </div>
        );
    }
}

TileContentContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    user: React.PropTypes.object,
    site: React.PropTypes.object,
};
