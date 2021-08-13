import React from 'react';

export default class InfoTile extends React.Component {
    render() {
        let containerClass = 'info-tile full-width';
        containerClass += this.props.carouselItem.ContainerClass
            ? ` ${this.props.carouselItem.ContainerClass} ` : '';
        return (
            <div className={containerClass}>
                <div className="info-tile-full-image-container">
                    <img className="info-tile-full-image" src={this.props.carouselItem.Image} />
                </div>
                <div className="info-tile-content-full-image">
                    <h3 className="h-secondary h-override h-alt">
                        {this.props.carouselItem.Title}
                    </h3>
                    <p>
                        {this.props.carouselItem.Description}
                    </p>
                    {this.props.carouselItem.Button && this.props.carouselItem.Button.URL
                    && <a className="btn btn-large btn-default btn-v-margin btn-alt"
                            href={this.props.carouselItem.Button.URL}>
                            {this.props.carouselItem.Button.Text}
                    </a>}
                </div>
            </div>
        );
    }
}

InfoTile.propTypes = {
    carouselItem: React.PropTypes.shape({
        Image: React.PropTypes.string,
        Title: React.PropTypes.string,
        Description: React.PropTypes.string,
        Button: React.PropTypes.object,
        ContainerClass: React.PropTypes.string,
    }).isRequired,
};
