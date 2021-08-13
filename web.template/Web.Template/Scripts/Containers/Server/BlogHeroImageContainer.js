import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';
import StringFunctions from '../../library/stringfunctions';

export default class BlogHeroImageContainer extends React.Component {
    getCarouselItemsClass() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const config = contentModel.Configuration;
        const heightClass = StringFunctions.heightClassFromEnum(config.CarouselHeight);
        const mobileHeightClass
                = StringFunctions.heightClassFromEnum(config.CarouselHeightMobile);
        const className = `carousel-items ${heightClass} mobile-${mobileHeightClass}`;
        return className;
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const containerAttributes
            = ServerContainerFunctions.setupContainerAttributes('blogheroimage', contentModel);
        return (
            <div {...containerAttributes}>
                <div className={this.getCarouselItemsClass()}></div>
            </div>
        );
    }
}

BlogHeroImageContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
};
