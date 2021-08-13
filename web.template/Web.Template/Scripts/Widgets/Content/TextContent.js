import '../../../../styles/widgets/content/_textcontent.scss';

import AccordionContent from '../../../../scripts/widgets/content/accordionContent';
import MarkdownText from '../../../../scripts/components/common/MarkdownText';
import React from 'react';

export default class TextContent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
        this.numberOfColumns = 12;
        this.renderTitleAndLeadParagraph = this.renderTitleAndLeadParagraph.bind(this);
        this.getContent = this.getContent.bind(this);
        this.shouldUseOverride = this.shouldUseOverride.bind(this);
    }
    getWidgetStyling(style) {
        let styleClass = '';
        switch (style) {
            case 'Default':
                styleClass = 'style-default';
                break;
            case 'Alternate':
                styleClass = 'style-alt';
                break;
            case 'Highlight':
                styleClass = 'style-highlight';
                break;
            default:
        }
        return (
            styleClass
        );
    }
    getWidgetPadding(style) {
        let styleClass = 'padding-none';
        switch (style) {
            case 'Small':
                styleClass = 'padding-sm';
                break;
            case 'Medium':
                styleClass = 'padding-md';
                break;
            case 'Large':
                styleClass = 'padding-lg';
                break;
            default :
        }
        return (
            styleClass
        );
    }
    getImageClassFromAspectRatio(aspectRatio) {
        let imageClass = 'img-aspect-';
        if (aspectRatio) {
            imageClass += aspectRatio.replace(':', '-');
        } else {
            imageClass += '4-3';
        }
        return imageClass;
    }
    getImageWidth(item) {
        const defaultWidth = 6;
        let imageWidth = 0;
        if (this.hasSideImages(item)) {
            imageWidth = this.shouldUseOverride(item.Configuration.SideImage.Width)
                ? item.Configuration.SideImage.Width
                : this.props.sideImageWidth;
            imageWidth = imageWidth ? imageWidth : defaultWidth;
        }
        return imageWidth;
    }
    hasSideImages(item) {
        return item.SideImage
            || (item.SideImages && item.SideImages.filter(img => img.Image).length > 0);
    }
    shouldUseOverride(configuration) {
        const useOverrride = configuration && configuration !== 'Use default';
        return useOverrride;
    }
    renderSideImage(content, imgLocation) {
        const imageWidth = this.getImageWidth(content);

        let push = '';
        if (this.props.sideImageMobileLocation === 'Bottom'
                && imgLocation === 'left') {
            push = `col-md-pull-${this.numberOfColumns - imageWidth}`;
        } else if (this.props.sideImageMobileLocation === 'Top'
            && imgLocation === 'right') {
            push = `col-md-push-${this.numberOfColumns - imageWidth}`;
        }

        const containerClass
            = `col-xs-12 col-md-${imageWidth} img-side ${push}`;

        let imageContainerClass = 'img-container';
        const aspectRatio = this.shouldUseOverride(content.Configuration.SideImage.AspectRatio)
            ? content.Configuration.SideImage.AspectRatio
            : this.props.sideImageAspectRatio;
        imageContainerClass += ` ${this.getImageClassFromAspectRatio(aspectRatio)}`;

        const backgroundDiv = {
            className: 'img-background ',
            style: {
                backgroundImage: `url('${content.SideImage}')`,
            },
        };

        return (
            <div className={containerClass}>
                <div className={imageContainerClass}>
                    <div {...backgroundDiv}></div>
                </div>
            </div>
        );
    }
    renderSideImages(sideImages, imgLocation, content) {
        const noImages = 0;
        const oneImage = 1;
        const twoImages = 2;
        const threeImages = 3;
        const fourImages = 4;
        const fiveImages = 5;
        const imageWidth = this.getImageWidth(content);
        let push = '';
        if (this.props.sideImageMobileLocation === 'Bottom'
                && imgLocation === 'left') {
            push = `col-md-pull-${this.numberOfColumns - imageWidth}`;
        } else if (this.props.sideImageMobileLocation === 'Top'
            && imgLocation === 'right') {
            push = `col-md-push-${this.numberOfColumns - imageWidth}`;
        }
        const containerClass = `article-image-container col-xs-12 col-md-6 ${push}`;
        return (
            <div className={containerClass}>
                {sideImages.length > noImages
                    && <div className="row">
                        {sideImages.length > noImages
                            && <img className="article-grid-image col-xs-4 col-md-4"
                            src={sideImages[noImages].Image}/>}
                        {sideImages.length > oneImage
                            && <img className="article-grid-image col-xs-4 col-md-4"
                            src={sideImages[oneImage].Image}/>}
                        {sideImages.length > twoImages
                        && <img className="article-grid-image col-xs-4 col-md-4"
                            src={sideImages[twoImages].Image}/>}
                        </div>}
                        {sideImages.length > threeImages
                        && <div className="row">
                            {sideImages.length > threeImages
                                && <img className="article-grid-image col-xs-4 col-md-4"
                                src={sideImages[threeImages].Image}/>}
                        {sideImages.length > fourImages
                            && <img className="article-grid-image col-xs-4 col-md-4"
                            src={sideImages[fourImages].Image}/>}
                        {sideImages.length > fiveImages
                        && <img className="article-grid-image col-xs-4 col-md-4"
                        src={sideImages[fiveImages].Image}/>}
                    </div>
                }
            </div>
        );
    }
    renderButton(button) {
        let btnClass = '';

        switch (button.Style) {
            case 'Arrow':
                btnClass = `${button.Style.toLowerCase()}-link`;
                break;
            default:
                btnClass = 'btn btn-large btn-default';
        }

        return (
            <div>
                <a className={btnClass} href={button.URL}>
                    {button.Text}
                </a>
            </div>
        );
    }
    renderTitleAndLeadParagraph() {
        return (
            <div className="col-xs-12 center-block">
                {this.props.title !== ''
                    && <h2 className="h-secondary h-secondary-standout">{this.props.title}</h2>}

                {this.props.leadParagraph !== ''
                    && <p className="preamble">
                        {this.props.leadParagraph}</p>}
            </div>
        );
    }
    sortTextContentItems(textContentItemA, textContentItemB) {
        return (
            (textContentItemA.Sequence - textContentItemB.Sequence)
        );
    }
    getAlignmentClasses(item, mobile, tablet, desktop, isAlternate) {
        let alignImage = '';
        const txtAlignMobile = mobile
            ? mobile.toLowerCase() : '';
        const txtAlignTablet = tablet
            ? tablet.toLowerCase() : '';
        const txtAlignDesktop = desktop
            ? desktop.toLowerCase() : '';
        const txtDesktopWidth = this.props.desktopTextWidth
            ? this.props.desktopTextWidth : this.numberOfColumns;
        let divClasses = (txtDesktopWidth === this.numberOfColumns)
            ? 'col-xs-12' : `col-xs-12 col-md-${txtDesktopWidth} center-block`;

        if ((item.SideImage)
            || (item.SideImages && item.SideImages.length > 0
            && item.SideImages[0].Image)) {
            const defaultWidth = 6;
            const sideImageWidth = this.getImageWidth(item);
            let txtColumnSize = this.numberOfColumns - sideImageWidth;
            txtColumnSize = txtColumnSize ? txtColumnSize : defaultWidth;

            divClasses = `${divClasses} col-md-${txtColumnSize}`;

            if (this.shouldUseOverride(item.Configuration.SideImage.Location)) {
                alignImage = `${item.Configuration.SideImage.Location}`.toLowerCase();
            } else if (isAlternate) {
                alignImage = `${this.props.sideImageAlternatedLocation}`.toLowerCase();
            } else {
                alignImage = `${this.props.sideImageLocation}`.toLowerCase();
            }

            const alignText = alignImage === 'left' ? 'right' : 'left';
            divClasses += ` txt-${alignText}`;

            if (this.props.sideImageMobileLocation === 'Top'
                    && alignImage === 'right') {
                divClasses += ` col-md-pull-${sideImageWidth}`;
            } else if (this.props.sideImageMobileLocation === 'Bottom'
                && alignImage === 'left') {
                divClasses += ` col-md-push-${sideImageWidth}`;
            }
        }

        divClasses = `${divClasses} text-align-xs-${txtAlignMobile}`;
        if (txtAlignMobile !== txtAlignTablet) {
            divClasses = `${divClasses} text-align-md-${txtAlignTablet}`;
        }
        if (txtAlignTablet !== txtAlignDesktop) {
            divClasses = `${divClasses} text-align-md-${txtAlignDesktop}`;
        }
        const returnClasses = {
            divClasses,
            alignImage,
        };
        return (
            returnClasses
        );
    }
    renderContentSection(content, index) {
        const key = `content-section-${index}`;
        const numRowsRepeated = 2;
        const isAlternatedContent = ((index % numRowsRepeated) === 1);

        let mobile = '';
        let tablet = '';
        let desktop = '';

        if (isAlternatedContent) {
            mobile = this.props.alternatedTextAlignment.Mobile;
            tablet = this.props.alternatedTextAlignment.Tablet;
            desktop = this.props.alternatedTextAlignment.Desktop;
        } else {
            mobile = this.props.textAlignment.Mobile;
            tablet = this.props.textAlignment.Tablet;
            desktop = this.props.textAlignment.Desktop;
        }

        mobile = this.shouldUseOverride(content.Configuration.TextAlignment.Mobile)
            ? content.Configuration.TextAlignment.Mobile
            : mobile;

        tablet = this.shouldUseOverride(content.Configuration.TextAlignment.Tablet)
            ? content.Configuration.TextAlignment.Tablet
            : tablet;

        desktop = this.shouldUseOverride(content.Configuration.TextAlignment.Desktop)
            ? content.Configuration.TextAlignment.Desktop
            : desktop;

        const textAlignment = {
            mobile,
            tablet,
            desktop,
        };

        const classes = this.getAlignmentClasses(content,
                                                 textAlignment.mobile,
                                                 textAlignment.tablet,
                                                 textAlignment.desktop,
                                                 isAlternatedContent,
                                                 content.Configuration.SideImage);

        const divClasses = classes.divClasses;
        const alignImage = classes.alignImage;
        const widgetPadding = this.getWidgetPadding(this.props.widgetPadding);
        const sectionClasses = `text-content-section ${widgetPadding}`;

        const textColumns = this.shouldUseOverride(content.Configuration.TextColumns)
            ? content.Configuration.TextColumns
            : this.props.textColumns;
        const markdownProps = {
            markdown: content.MarkdownText,
            textColumns,
        };

        const showAccordion
            = content.Accordion
            ? content.Accordion.AccordionContent.filter(item => item.Hidden === false).length > 0
            : false;
        let accordionProps = {};
        if (showAccordion) {
            const accordionConfiguration = content.Accordion.Configuration;
            accordionConfiguration.ContainerStyle = this.props.widgetStyle;
            accordionProps = {
                accordionContent: content.Accordion.AccordionContent,
                button: content.Accordion.ShowAllButton,
                configuration: accordionConfiguration,
            };
        }
        const sideImages = content.SideImages ? content.SideImages.filter(img => img.Image) : [];
        return (
            <div key={key} className={sectionClasses}>

                {this.props.sideImageMobileLocation === 'Top'
                    && content.SideImage
                    && this.renderSideImage(content, alignImage)}

                {this.props.sideImageMobileLocation === 'Top'
                    && sideImages.length > 0
                    && this.renderSideImages(sideImages, alignImage, content)}

                <div className={divClasses}>
                    {content.MarkdownText
                        && <MarkdownText {...markdownProps} />}

                    {showAccordion
                    && <div className="widget-accordioncontent">
                            <AccordionContent {...accordionProps} />
                        </div>}

                    {content.Button
                        && content.Button.Text
                        && content.Button.URL
                        && this.renderButton(content.Button)}
                </div>

                {this.props.sideImageMobileLocation === 'Bottom'
                    && content.SideImage
                    && this.renderSideImage(content, alignImage)}

                {this.props.sideImageMobileLocation === 'Bottom'
                    && sideImages.length > 0
                    && this.renderSideImages(sideImages, alignImage, content)}
            </div>
        );
    }
    shouldDisplayContentItem(item) {
        let display = false;
        if (item.Accordion) {
            display = ((item.Accordion.AccordionContent.filter(content =>
                    content.Hidden === false).length > 0)
                || (item.SideImages.length > 0)
                || (item.MarkdownText)
                || (item.SideImage)
                || (item.Button));
        } else {
            display = ((item.SideImages && item.SideImages.length > 0)
                || (item.MarkdownText)
                || (item.SideImage)
                || (item.Button));
        }
        return display;
    }
    getContent() {
        let content = this.props.content.filter(item => item.Hidden === false);
        content = content.filter(item => (this.shouldDisplayContentItem(item)));
        content = content.sort(this.sortTextContentItems);
        return (
            content
        );
    }
    render() {
        let content = [];
        if (Array.isArray(this.props.content)) {
            content = this.getContent();
        }
        const indicateMultipleContent = content.length > 1 ? 'alternated-content' : '';
        const txtAlignVertical = this.props.textAlignment.Vertical
            ? this.props.textAlignment.Vertical.toLowerCase() : '';
        const widgetStyling = this.getWidgetStyling(this.props.widgetStyle);
        const contentClasses = `markdown-content row ${txtAlignVertical} ${widgetStyling}
                                  ${indicateMultipleContent}`;
        return (
            <div className={contentClasses}>
                {(this.props.title !== '' || this.props.leadParagraph !== '')
                && this.renderTitleAndLeadParagraph()}

                {content.map(this.renderContentSection, this)}
            </div>
        );
    }
}

TextContent.propTypes = {
    title: React.PropTypes.string,
    leadParagraph: React.PropTypes.string,
    content: React.PropTypes.arrayOf(
        React.PropTypes.shape({
            MarkdownText: React.PropTypes.string.isRequired,
            SideImage: React.PropTypes.string,
            SideImages: React.PropTypes.array,
            Button: React.PropTypes.shape({
                Text: React.PropTypes.string,
                URL: React.PropTypes.string,
                Style: React.PropTypes.string,
            }),
            Sequence: React.PropTypes.number,
            Hidden: React.PropTypes.boolean,
            Configuration: React.PropTypes.shape({
                TextColumns: React.PropTypes.oneOfType([
                    React.PropTypes.string,
                    React.PropTypes.number,
                ]),
                TextAlignment: React.PropTypes.shape({
                    Desktop: React.PropTypes.string,
                    Tablet: React.PropTypes.string,
                    Mobile: React.PropTypes.string,
                }),
                SideImage: React.PropTypes.shape({
                    AspectRatio: React.PropTypes.string,
                    Width: React.PropTypes.oneOfType([
                        React.PropTypes.string,
                        React.PropTypes.number,
                    ]),
                    Location: React.PropTypes.string,
                }),
            }),
        })
    ),
    textColumns: React.PropTypes.number,
    textAlignment: React.PropTypes.shape({
        Desktop: React.PropTypes.string,
        Tablet: React.PropTypes.string,
        Mobile: React.PropTypes.string,
        Vertical: React.PropTypes.string,
    }),
    alternatedTextAlignment: React.PropTypes.shape({
        Desktop: React.PropTypes.string,
        Tablet: React.PropTypes.string,
        Mobile: React.PropTypes.string,
    }),
    sideImageLocation: React.PropTypes.string,
    sideImageMobileLocation: React.PropTypes.string,
    sideImageAlternatedLocation: React.PropTypes.string,
    sideImageAspectRatio: React.PropTypes.string,
    sideImageWidth: React.PropTypes.number,
    desktopTextWidth: React.PropTypes.number,
    widgetStyle: React.PropTypes.string,
    widgetPadding: React.PropTypes.string,
};

TextContent.defaultProps = {
    sideImageMobileLocation: 'Top',
};
