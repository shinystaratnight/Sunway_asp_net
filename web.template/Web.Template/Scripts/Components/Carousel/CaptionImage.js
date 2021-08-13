import React from 'react';

export default class CaptionImage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.carouselItem.Image}')`,
            },
        };
        return (
            <div className="caption-image">
               {this.props.carouselItem.URL
                    && <a className="a-container" href={this.props.carouselItem.URL}></a>}

                {this.props.carouselItem.Image
                    && <div className="img-container"><div {...backgroundDiv}></div></div>}

                <p className="h-tertiary">{ this.props.carouselItem.Header }</p>
                <p>{ this.props.carouselItem.Text }</p>
            </div>
        );
    }
}

CaptionImage.propTypes = {
    carouselItem: React.PropTypes.shape({
        Image: React.PropTypes.string.isRequired,
        Header: React.PropTypes.string,
        Text: React.PropTypes.string,
        URL: React.PropTypes.string,
    }).isRequired,
};
