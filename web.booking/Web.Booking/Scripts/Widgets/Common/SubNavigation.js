import React from 'react';

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
        let url = socialMedia.URL.replace('[url]', canonicalUrl);
        url = url.replace('[title]', pageTitle);
        url = url.replace('[img]', image);
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
            url = `${url}/${array[i].Value.replace(/ /g, '-').toLowerCase()}`;
        }

        if (entityInformation.Hide) {
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
    render() {
        return (
            <div className="container">
                {this.props.showSocialMedia
                    && <ul className="nav social-media">
                        <li role="presentation">Share: </li>
                    {this.renderSocialMediaLinks}
                    {this.props.socialMedia.map(this.renderSocialMediaLink, this)}
                </ul>}
                <ul className="breadcrumbs">
                    <li role="presentation" className="breadcrumb-item"><a href="/">Home</a></li>
                    {this.props.page.EntityInformations
                        && this.props.page.EntityInformations.length > 0
                        && this.props.page.EntityInformations.map(this.renderBreadCrumb)}
                    {this.props.page
                        && (this.props.page.EntityInformations === null
                        || this.props.page.EntityInformations.length === 0)
                        && <li role="presentation" className="breadcrumb-item active">
                                <span>{this.props.page.PageName}</span>
                            </li>
                    }
                </ul>
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
            PageImage: React.PropTypes.string,
        }),
    }),
};
