import Header from '../../widgets/header';
import React from 'react';

export default class HeaderContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const contentModel = JSON.parse(this.props.contentJSON);
        const headerProps = {
            user: this.props.user,
            logo: contentModel.Logo,
            logoUrl: contentModel.LogoUrl,
            bannerImage: contentModel.BannerImage,
            offer: contentModel.Offer,
            contact: contentModel.Contact,
            navigationLinks: contentModel.NavigationLinks,
            socialMedia: contentModel.SocialMedia,

        };
        return (
            <div className="widget widget-header">
                <Header {...headerProps} />
            </div>
        );
    }
}

HeaderContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    user: React.PropTypes.string,
};
