import React from 'react';

export default class ImageTile extends React.Component {
    render() {
        const containerProps = {
            className: 'img-container image-tile alt',
            style: {
                height: `${this.props.tileHeight}px`,
            },
        };

        if (this.props.tileItem.BackgroundImage && this.props.tileItem.URL) {
            containerProps.className
                = `${containerProps.className} img-zoom-hover gradient-overlay`;
        }

        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.tileItem.BackgroundImage}')`,
            },
        };

        return (
            <div {...containerProps}>
                {this.props.tileItem.URL
                    && <a className="a-container" href={this.props.tileItem.URL}></a>}

                {this.props.tileItem.Image
                    && <img className="image" src={this.props.tileItem.Image} />}

                {this.props.tileItem.BackgroundImage
                    && <div {...backgroundDiv}></div>}

                {this.props.tileItem.Title
                    && <span className="text h-secondary">
                            {this.props.tileItem.Title}</span>}

                {this.props.tileItem.ArrowLink
                    && <span className="arrow-link">{this.props.tileItem.ArrowLink}</span>}
            </div>
        );
    }
}

ImageTile.propTypes = {
    tileHeight: React.PropTypes.number.isRequired,
    tileItem: React.PropTypes.shape({
        Image: React.PropTypes.string,
        BackgroundImage: React.PropTypes.string,
        Title: React.PropTypes.string,
        ArrowLink: React.PropTypes.string,
        URL: React.PropTypes.string,
    }).isRequired,
};
