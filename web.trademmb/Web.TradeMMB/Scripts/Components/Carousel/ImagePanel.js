import React from 'react';

export default class ImagePanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    render() {
        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.carouselItem.ImageUrl}')`,
            },
        };

        return (
            <div className="image-panel">
               {this.props.carouselItem.LinkUrl
                    && <a className="a-container" href={this.props.carouselItem.LinkUrl}></a>}
                <div {...backgroundDiv}></div>
            </div>
        );
    }
}

ImagePanel.propTypes = {
    carouselItemHeight: React.PropTypes.string.isRequired,
    carouselItemHeightMobile: React.PropTypes.string.isRequired,
    carouselItem: React.PropTypes.shape({
        ImageUrl: React.PropTypes.string.isRequired,
        LinkUrl: React.PropTypes.string,
    }).isRequired,
};
