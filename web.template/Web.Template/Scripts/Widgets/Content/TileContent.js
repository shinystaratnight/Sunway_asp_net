import '../../../styles/widgets/content/_tilecontent.scss';
import '../../../styles/widgets/content/_flexColumns.scss';

import * as TileComponents from '../../components/tile';
import ObjectFunctions from '../../library/objectfunctions';
import React from 'react';
import StringFunctions from '../../library/stringfunctions';

export default class TileContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.renderLeadTile = this.renderLeadTile.bind(this);
        this.renderTileRows = this.renderTileRows.bind(this);
        this.renderTileRow = this.renderTileRow.bind(this);
        this.renderTile = this.renderTile.bind(this);
        this.renderFlexGrid = this.renderFlexGrid.bind(this);
        this.renderFlexItem = this.renderFlexItem.bind(this);
        this.renderButton = this.renderButton.bind(this);
    }
    renderLeadTile() {
        const TileComponent = TileComponents[this.props.leadTileType];
        const componentProps = {
            textHeight: this.props.leadTileHeight,
            textItem: this.props.leadTileItem,
        };
        return (
            <div className="row lead-tile">
                <TileComponent {...componentProps} />
            </div>
        );
    }
    renderTileRows() {
        const tileRows = [];
        let tileIndex = 0;
        const tileItems = this.props.tileItems;

        for (let i = 1; i <= this.props.tileRows; i++) {
            const tileRow = {
                key: `${this.props.context}-row-${i}`,
                tiles: [],
            };

            let fullWidthRow = false;
            for (let j = 1; j <= this.props.tileColumns; j++) {
                const tileItem = tileItems[tileIndex];
                if (tileItem) {
                    const tile = {
                        key: `${this.props.context}-tile-${tileIndex}`,
                        tileItem,
                    };
                    tileRow.tiles.push(tile);
                    tileIndex++;

                    if (tileItem.FullWidthTile) {
                        fullWidthRow = true;
                        break;
                    }
                }
            }

            if ((fullWidthRow && tileRow.tiles.length === 1)
                || (!fullWidthRow && tileRow.tiles.length === this.props.tileColumns)) {
                tileRows.push(tileRow);
            }

            if (!tileItems[tileIndex]) {
                break;
            }
        }
        return tileRows.map(this.renderTileRow, this);
    }
    renderTileRow(tileRow) {
        let className = 'row';
        if (this.props.tileType === 'InfoTile') {
            className = `${className} info-tile-row`;
        }
        return (
            <div key={tileRow.key} className={className}>
                {tileRow.tiles.map(this.renderTile, this)}
            </div>
        );
    }
    renderTile(tile, index, array) {
        const totalTitles = array.length;
        const tabletColumns = this.props.tileColumnsTablet;
        const tilesRemain = totalTitles % tabletColumns;
        const TileComponent = TileComponents[this.props.tileType];
        const gridColumns = 12;
        const gridTileColumns = tile.tileItem.FullWidthTile
            ? gridColumns
            : Math.ceil(gridColumns / this.props.tileColumns);
        const gridTileColumnsMobile = tile.tileItem.FullWidthTile
            ? gridColumns
            : Math.ceil(gridColumns / this.props.tileColumnsMobile);
        let columnSmall = '';
        if (tile.tileItem.FullWidthTile) {
            columnSmall = 'col-sm-12';
        } else if (this.props.tileColumnsTablet) {
            const gridTileColumnsTablet = Math.ceil(gridColumns / tabletColumns);
            columnSmall = `col-sm-${gridTileColumnsTablet}`;
        }
        const hideTile = index + 1 > totalTitles - tilesRemain && !tile.tileItem.FullWidthTile
            ? 'hidden-sm' : '';
        const gridClass = `tile col-xs-${gridTileColumnsMobile}
            ${columnSmall}
            col-md-${gridTileColumns}
            ${hideTile}`;
        const componentProps = {
            tileHeight: this.props.tileHeight,
            tileItem: tile.tileItem,
            siteConfiguration: this.props.siteConfiguration,
        };
        return (
            <div key={tile.key} className={gridClass}>
                <TileComponent {...componentProps} />
            </div>
        );
    }
    renderFlexGrid() {
        const containerAttributes = {
            className: 'flex-container',
        };
        const browser = this.props.browser.toLowerCase();
        if (browser === 'internetexplorer' || browser === 'ie') {
            containerAttributes.style = {
                height: '1200px',
            };
        }
        return (
            <div {...containerAttributes}>
                {this.props.tileItems.map(this.renderFlexItem, this)}
            </div>
        );
    }
    renderFlexItem(tileItem, index) {
        const TileComponent = TileComponents[this.props.tileType];
        const key = `${this.props.context}-tile-${index}`;
        const componentProps = {
            tileItem,
        };
        return (
            <TileComponent key={key} {...componentProps} />
        );
    }
    renderButton() {
        const buttonClass = 'btn btn-lg btn-default';
        return (
            <div className="row btn-row">
                {this.props.buttonURL !== ''
                    && <a className={buttonClass} href={this.props.buttonURL}>
                {this.props.buttonText} </a>}
                {this.props.buttonURL === ''
                    && <button className={buttonClass}> {this.props.buttonText}</button>}
            </div>
        );
    }
    render() {
        const tilesContainerAttributes = {
            className: 'tiles',
            style: {},
        };

        if (this.props.tilesPaddingTop && this.props.tilesPaddingTop !== 0) {
            tilesContainerAttributes.style.paddingTop = this.props.tilesPaddingTop;
        }

        if (this.props.tilesPaddingBottom && this.props.tilesPaddingBottom !== 0) {
            tilesContainerAttributes.style.paddingBottom = this.props.tilesPaddingBottom;
        }

        return (
            <div className="tile-content">
                {this.props.title
                    && <h2 className="h-secondary">{this.props.title}</h2>}

                {this.props.leadParagraph !== ''
                    && <p className="preamble">
                    {this.props.leadParagraph}</p>}

                    {this.props.leadTileHeight !== 0
                        && !StringFunctions.isNullOrEmpty(this.props.leadTileType)
                        && !ObjectFunctions.isNullOrEmpty(this.props.leadTileItem)
                        && this.renderLeadTile()}

                <div {...tilesContainerAttributes}>
                    {this.props.gridType === 'Rows'
                        && this.renderTileRows()}

                    {this.props.gridType === 'Flex'
                        && this.renderFlexGrid()}
                </div>

                {this.props.buttonText
                    && this.renderButton()}
            </div>
        );
    }
}

TileContent.propTypes = {
    context: React.PropTypes.string.isRequired,
    siteConfiguration: React.PropTypes.object.isRequired,
    browser: React.PropTypes.string,
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    gridType: React.PropTypes.string.isRequired,
    tileColumns: React.PropTypes.number.isRequired,
    tileRows: React.PropTypes.number.isRequired,
    tileColumnsMobile: React.PropTypes.number.isRequired,
    tileColumnsTablet: React.PropTypes.number.isRequired,
    leadTileHeight: React.PropTypes.number,
    leadTileType: React.PropTypes.string,
    leadTileItem: React.PropTypes.object,
    tileHeight: React.PropTypes.number,
    tilesPaddingTop: React.PropTypes.number,
    tilesPaddingBottom: React.PropTypes.number,
    tileType: React.PropTypes.string.isRequired,
    tileItems: React.PropTypes.array.isRequired,
    buttonText: React.PropTypes.string,
    buttonURL: React.PropTypes.string,
    buttonStyle: React.PropTypes.string,
};
