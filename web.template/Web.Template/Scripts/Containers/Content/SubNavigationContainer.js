import React from 'react';
import SubNavigation from '../../widgets/common/subnavigation';

export default class SubNavigationContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    render() {
        const page = this.props.page;
        const containerAttributes = {
            className: 'widget-sub-navigation',
        };
        const contentModel = this.props.entity.model;
        const subNavigationProps = {
            userLoggedIn: false,
            socialMedia: contentModel.SocialMedia,
            showSocialMedia: contentModel.ShowSocialMedia,
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
    entity: React.PropTypes.object,
    page: React.PropTypes.object,
    site: React.PropTypes.object,
};
