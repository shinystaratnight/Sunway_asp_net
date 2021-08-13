import React from 'react';

export default class VideoPanel extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }

    render() {
        const params = '?showinfo=0&autohide=1&modestbranding=1';
        const videoUrl = `https://www.youtube.com/embed/${this.props.carouselItem.VideoCode}${params}`;

        return (
            <div style={ { position: 'relative',
                            paddingTop: '56.25%',
                            width: '100%',
                            height: '100%' } }>
                <iframe style={ { position: 'absolute',
                    top: 0, left: 0,
                    width: '100%', height: '100%' } }
                    src= {videoUrl}
                         frameBorder="0"
                         allowFullScreen></iframe>
            </div>
        );
    }
}

VideoPanel.propTypes = {
    carouselItemHeight: React.PropTypes.string.isRequired,
    carouselItemHeightMobile: React.PropTypes.string.isRequired,
    carouselItem: React.PropTypes.shape({
        VideoCode: React.PropTypes.string.isRequired,
    }).isRequired,
};
