import React from 'react';
import StringFunctions from '../../library/stringfunctions';

export default class SubNavigation extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        this.renderSocialMediaLink = this.renderSocialMediaLink.bind(this);
        this.renderBreadCrumb = this.renderBreadCrumb.bind(this);
    }

    renderSocialMediaLink(socialMedia) {
        const key = socialMedia.Icon;
        const socialMediaClass = `icon-${socialMedia.Icon.toLowerCase()}`;
        const canonicalUrl = this.props.page.MetaInformation.CanonicalUrl
            ? this.props.page.MetaInformation.CanonicalUrl : '';
        const pageTitle = this.props.page.MetaInformation.SocialMediaTitle
            ? this.props.page.MetaInformation.SocialMediaTitle : '';
        const image = this.props.page.MetaInformation.PageImage
            ? this.props.page.MetaInformation.PageImage : '';
        const content = this.props.page.MetaInformation.SocialMediaDescription
            ? this.props.page.MetaInformation.SocialMediaDescription : '';
        const userHandle = this.props.twitterHandle
            ? this.props.twitterHandle : '';
        let url = socialMedia.URL.replace('[url]', canonicalUrl).replace('[URL]', canonicalUrl);
        url = url.replace('[title]', pageTitle).replace('[TITLE]', pageTitle);
        url = url.replace('[img]', image).replace('[IMG]', image);
        url = url.replace('[content]', content).replace('[CONTENT]', content);
        url = url.replace('[handle]', userHandle).replace('[HANDLE]', userHandle);
        return (
            <li key={key} role="presentation">
                <a href={url} target="_blank">
                    <span className={socialMediaClass}></span>
                    {socialMedia.Icon}
                </a>
            </li>
        );
    }

    renderBreadCrumb(entityInformation, index, array) {
        const key = index;
        const last = index === array.length - 1;
        let breadcrumbClass = 'breadcrumb-item';

        if (last) {
            breadcrumbClass = `${breadcrumbClass} active`;
        }

        let url = '';

        for (let i = 0; i <= index; i++) {
            url = `${url}/${StringFunctions.safeUrl(array[i].Value)}`;
        }

        if (entityInformation.Hide && !last) {
            return false;
        }

        return (
            <li key={key} role="presentation" className={breadcrumbClass}>
                {last
                    && <span>{entityInformation.Value}</span>
                }
                {!last
                    && <a href={url}>{entityInformation.Value}</a>
            }
            </li>
        );
    }
    renderSocialMedia() {
        return (
            <ul className="nav social-media">
                <li role="presentation">Share: </li>
                {this.props.socialMedia.map(this.renderSocialMediaLink, this)}
            </ul>
        );
    }
    renderBreadcrumbs() {
        return (
            <ul className="breadcrumb">
                <li role="presentation" className="breadcrumb-item"><a href="/">Home</a></li>
                {this.props.page.EntityInformations
                    && this.props.page.EntityInformations.length > 0
                    && this.props.page.EntityInformations.map(this.renderBreadCrumb)}
                {this.props.page
                    && (this.props.page.EntityInformations === null
                    || this.props.page.EntityInformations.length === 0)
                    && <li role="presentation" className="breadcrumb-item active">
                           <span>{this.props.page.PageName}</span>
                </li>}
            </ul>
        );
    }
    render() {
        const socialMedia = this.props.showSocialMedia ? this.renderSocialMedia() : '';
        const breadcrumbs = this.renderBreadcrumbs();
        return (
            <div>
                <div className="container hidden-xs">
                    {socialMedia}
                    {breadcrumbs}
                </div>
                <div className="hidden-sm-up">
                    <div className="social-media-container container">{socialMedia}</div>
                    <div className="breadcrumbs-container container">{breadcrumbs}</div>
                </div>
            </div>
        );
    }
}

SubNavigation.propTypes = {
    userLoggedIn: React.PropTypes.bool,
    showSocialMedia: React.PropTypes.bool,
    socialMedia: React.PropTypes.arrayOf(
        React.PropTypes.shape({
            URL: React.PropTypes.string.isRequired,
            Icon: React.PropTypes.string.isRequired,
        })
    ),
    page: React.PropTypes.shape({
        PageName: React.PropTypes.string,
        PageURL: React.PropTypes.string,
        EntityInformations: React.PropTypes.arrayOf(
            React.PropTypes.shape({
                Id: React.PropTypes.number,
                Name: React.PropTypes.string,
                Value: React.PropTypes.string,
            })),
        MetaInformation: React.PropTypes.shape({
            CanonicalUrl: React.PropTypes.string,
            SocialMediaTitle: React.PropTypes.string,
            SocialMediaDescription: React.PropTypes.string,
            PageImage: React.PropTypes.string,
        }),
    }),
    twitterHandle: React.PropTypes.string,
};
