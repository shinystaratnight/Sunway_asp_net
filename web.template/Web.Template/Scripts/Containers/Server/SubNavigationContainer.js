import React from 'react';
import ServerContainerFunctions from '../../library/servercontainerfunctions';
import SubNavigation from '../../widgets/common/subnavigation';

export default class SubNavigationContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const page = this.props.page;
        const contentModel = JSON.parse(this.props.contentJSON);
        const containerAttributes
            = ServerContainerFunctions.setupContainerAttributes(this.props.context, contentModel);
        containerAttributes.className += ' widget-sub-navigation';
        const showSocialMedia
            = contentModel.ShowSocialMedia !== undefined
            ? contentModel.ShowSocialMedia
            : true;

        const subNavigationProps = {
            userLoggedIn: false,
            socialMedia: contentModel.SocialMedia,
            showSocialMedia,
            twitterHandle: this.props.site.SiteConfiguration.TwitterConfiguration.TwitterHandle,
            page,
        };

        return (
            <div {...containerAttributes}>
                <SubNavigation {...subNavigationProps}/>
            </div>
        );
    }
}

SubNavigationContainer.propTypes = {
    context: React.PropTypes.string,
    contentJSON: React.PropTypes.string,
    page: React.PropTypes.object,
    site: React.PropTypes.object,
};
