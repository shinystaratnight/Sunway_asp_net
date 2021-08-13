import React from 'react';

export default class InfoTile extends React.Component {
    renderTitle() {
        const h3TitleClass = 'h-secondary tile-header';
        const h2TitleClass = 'h-tertiary';
        return (
            <div>
                {this.props.tileItem.Image
                    ? <h3 className={h3TitleClass}>{this.props.tileItem.Title}</h3>
                    : <h2 className={h2TitleClass}>{this.props.tileItem.Title}</h2>}
            </div>
        );
    }
    renderButton() {
        let btnClass = '';

        switch (this.props.tileItem.Button.Style) {
            case 'Arrow':
                btnClass = `${this.props.tileItem.Button.Style.toLowerCase()}-link`;
                break;
            default:
                btnClass = 'btn btn-large btn-default';
        }

        return (
            <a className={btnClass} href={this.props.tileItem.Button.URL}>
                {this.props.tileItem.Button.Text}
            </a>
        );
    }

    render() {
        let imgOuterClass = '';
        let imgInnerClass = {
            style: {},
        };
        const containerProps = {
            className: 'info-tile',
            style: {},
        };
        let tileText = {};
        const subTitleProps = {
            className: 'subtitle',
        };
        const mainContentProps = {
            className: 'main-content',
        };
        if (this.props.tileItem.Image) {
            imgOuterClass = `col-sm-3 col-md-${this.props.tileItem.ImageColumns} hidden-xs`;
            const gridColumns = 12;
            const gridTextColumn = Math.ceil(gridColumns - this.props.tileItem.ImageColumns);

            containerProps.className = `${containerProps.className} text-align-center-xs row`;

            tileText = {
                className: `col-xs-12 col-sm-9 col-md-${gridTextColumn}`,
            };
            if (this.props.tileHeight && this.props.tileHeight > 0) {
                if (!imgInnerClass.style) {
                    imgInnerClass.style = {};
                }

                imgInnerClass.style.height = `${this.props.tileItem.ImageHeight}px`;
            }
            if (this.props.tileItem.ImageStyle !== 'Default') {
                containerProps.className = `${containerProps.className} shape-img`;
                imgInnerClass = {
                    className: `image img-${this.props.tileItem.ImageStyle.toLowerCase()}`,
                    style: {},
                };
            }

            if (this.props.tileHeight && this.props.tileHeight > 0) {
                imgInnerClass.style.height = `${this.props.tileItem.ImageHeight}px`;
            }
        }
        if (this.props.tileHeight > 0) {
            containerProps.style.height = `${this.props.tileHeight}px`;
        }
        return (
            <div {...containerProps}>
                {this.props.tileItem.Image && <div className={imgOuterClass}>
                    {<img {...imgInnerClass} src={this.props.tileItem.Image} />}
                </div>}
                <div {...tileText}>
                    {this.props.tileItem.Title
                        && this.renderTitle()}
                    {this.props.tileItem.LeadParagraph
                        && <p {...subTitleProps}>{this.props.tileItem.LeadParagraph}</p>}
                    {this.props.tileItem.BodyText
                        && <p {...mainContentProps}>{this.props.tileItem.BodyText}</p>}

                    {this.props.tileItem.Button.Text
                        && this.renderButton()}
                </div>
            </div>
        );
    }
}
InfoTile.propTypes = {
    tileHeight: React.PropTypes.number.isRequired,
    tileItem: React.PropTypes.shape({
        Image: React.PropTypes.string,
        ImageStyle: React.PropTypes.string,
        ImageHeight: React.PropTypes.number,
        ImageColumns: React.PropTypes.number,
        Title: React.PropTypes.string.isRequired,
        LeadParagraph: React.PropTypes.string,
        BodyText: React.PropTypes.string,
        Button: React.PropTypes.shape({
            Text: React.PropTypes.string,
            URL: React.PropTypes.string,
            Style: React.PropTypes.string,
        }),
    }).isRequired,
};
