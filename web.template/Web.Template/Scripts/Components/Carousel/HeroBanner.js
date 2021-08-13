import Rating from 'components/common/rating';
import React from 'react';
import StringFunctions from '../../library/stringfunctions';
import moment from 'moment';

export default class HeroBanner extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    renderTag(tag, index) {
        let tagContent = tag;
        if (this.props.carouselItem.Caption.MetaData.RenderLinks) {
            const link = StringFunctions.safeUrl(tag);
            let linkUrl = `?category=${link}`;
            linkUrl = this.props.carouselItem.Caption.MetaData.BaseLink
                ? `${this.props.carouselItem.Caption.MetaData.BaseLink}${linkUrl}`
                : linkUrl;
            tagContent = <a href={linkUrl}>{tag}</a>;
        }
        return (
            <li key={index} className="banner-tag">
                {tagContent}
            </li>
        );
    }
    renderMetaData() {
        const metaData = this.props.carouselItem.Caption.MetaData;
        let dateString = '';
        if (metaData.Date) {
            const momentDate = moment(metaData.Date, 'YYYY-MM-DD');
            dateString = momentDate.format('dddd[,] D MMMM YYYY');
        }
        return (
            <div className="banner-meta center-block clear">
                {metaData.Date
                    && dateString
                    && <time dateTime={metaData.Date} className="banner-date">
                           {dateString}
                       </time>}
                {metaData.Tags
                    && metaData.Tags.length > 0
                    && <ul className="banner-tags">
                           {metaData.Tags.map(this.renderTag, this)}
                       </ul>}
            </div>
        );
    }
    renderCaption() {
        const caption = this.props.carouselItem.Caption;
        const configuration = this.props.carouselItem.Configuration;
        let url = '';
        if (caption.hasOwnProperty('MetaData')) {
            url = `${caption.MetaData.BaseLink}/${this.props.carouselItem.URL}`;
        } else if (this.props.carouselItem.hasOwnProperty('URL')) {
            url = this.props.carouselItem.URL;
        }
        const renderButton = (this.props.carouselItem.hasOwnProperty('ButtonText')
                || caption.hasOwnProperty('ButtonText'))
            && (this.props.carouselItem.ButtonText || caption.ButtonText)
            && url;
        const verticalPosition = configuration.Caption.VerticalPosition.toLowerCase();
        const horizontalPosition = configuration.Caption.HorizontalPosition.toLowerCase();

        const defaultWidth = 5;
        let txtColumnSize = defaultWidth;
        if (configuration.Caption.DesktopTextWidth) {
            txtColumnSize = configuration.Caption.DesktopTextWidth;
        }
        const desktopTextWidth = `col-xs-12 col-md-${txtColumnSize}`;

        const captionContainerClass = `carousel-caption ${verticalPosition} 
                                        ${horizontalPosition} ${desktopTextWidth}`;
        const renderMetaData
            = caption.MetaData
            && (caption.MetaData.Date
                || (caption.MetaData.Tags
                    && (caption.MetaData.Tags.length > 0)));
        const minimumRating = 2.5;
        let renderRating = false;
        if (caption.Rating) {
            renderRating = parseFloat(caption.Rating) > minimumRating;
        }
        const buttonText = this.props.carouselItem.ButtonText
            ? this.props.carouselItem.ButtonText : caption.ButtonText;
        const buttonClass = horizontalPosition === 'center'
            ? `${horizontalPosition}-block` : `${horizontalPosition} block`;
        return (
            <div className={captionContainerClass}>
                <div className="carousel-caption-content">
                    {renderMetaData
                        && this.renderMetaData()}
                    {renderRating
                        && this.renderRating()}
                    {caption.Header
                        && caption.Header !== ''
                        && <h2 className="h-primary">{caption.Header}</h2>}
                    {caption.SubHeader
                        && caption.SubHeader !== ''
                        && <p className="h-secondary">{caption.SubHeader}</p>}
                    {caption.Text
                        && caption.Text !== ''
                        && <p className="h-tertiary">{caption.Text}</p>}
                    {renderButton
                        && <div className={buttonClass}><a className="btn btn-alt"
                            href={url}>{buttonText}</a></div>}
                </div>
            </div>
        );
    }
    renderRating() {
        const caption = this.props.carouselItem.Caption;
        const starRatingConfiguration = this.props.siteConfiguration.StarRatingConfiguration;
        const ratingProps = {
            rating: caption.Rating,
            appendText: starRatingConfiguration.AppendText,
            displayHalfRatings: starRatingConfiguration.DisplayHalfRatings,
            containerClass: 'white',
        };
        return (
            <Rating {...ratingProps} />
        );
    }
    render() {
        const backgroundDiv = {
            className: 'img-background',
            style: {
                backgroundImage: `url('${this.props.carouselItem.Image}')`,
            },
        };
        const heightClass = StringFunctions.heightClassFromEnum(this.props.carouselItemHeight);
        const mobileHeightClass
            = StringFunctions.heightClassFromEnum(this.props.carouselItemHeightMobile);
        const containerAttributes = {
            className: `container ${heightClass} mobile-${mobileHeightClass}`,
        };
        const caption = this.props.carouselItem.Caption;
        let url = '';
        if (caption.hasOwnProperty('MetaData')) {
            url = `${caption.MetaData.BaseLink}/${this.props.carouselItem.URL}`;
        } else if (this.props.carouselItem.hasOwnProperty('URL')) {
            url = this.props.carouselItem.URL;
        }
        return (
            <div className="hero-banner overlay">
                {url
                    && url.length > 0
                    && <a className="a-container" href={url}/>}

                <div {...backgroundDiv}></div>
                <div {...containerAttributes}>
                  {this.props.carouselItem.Caption
                    && this.renderCaption()}
                </div>
            </div>
        );
    }
}

HeroBanner.propTypes = {
    carouselItemHeight: React.PropTypes.string.isRequired,
    carouselItemHeightMobile: React.PropTypes.string.isRequired,
    carouselItem: React.PropTypes.shape({
        Image: React.PropTypes.string.isRequired,
        URL: React.PropTypes.string,
        ButtonText: React.PropTypes.string,
        Caption: React.PropTypes.shape({
            Header: React.PropTypes.string,
            SubHeader: React.PropTypes.string,
            Text: React.PropTypes.string,
            Rating: React.PropTypes.number,
            ButtonText: React.PropTypes.string,
            MetaData: React.PropTypes.shape({
                Date: React.PropTypes.string,
                Tags: React.PropTypes.arrayOf(
                    React.PropTypes.string
                ),
                RenderLinks: React.PropTypes.bool,
                BaseLink: React.PropTypes.string,
            }),
        }),
        Configuration: React.PropTypes.shape({
            Caption: React.PropTypes.shape({
                VerticalPosition: React.PropTypes.oneOf(['Top', 'Middle', 'Bottom']),
                HorizontalPosition: React.PropTypes.oneOf(['Left', 'Center', 'Right']),
                DesktopTextWidth: React.PropTypes.number,
            }),
        }),
    }).isRequired,
    siteConfiguration: React.PropTypes.object,
};
