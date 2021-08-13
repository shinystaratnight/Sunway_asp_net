import * as EntityActions from '../../actions/content/entityActions';

import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BlogHeroImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupCarouselContentProps() {
        const contentModel = this.props.entity.model;
        const configuration = contentModel.Configuration;
        const blogEntity = this.props.blog;
        const tags = blogEntity.Categories ? blogEntity.Categories : [];
        const carouselItems = [
            {
                Image: blogEntity.HeroImage,
                Caption: {
                    Header: blogEntity.Title,
                    Text: blogEntity.LeadParagraph,
                    MetaData: {
                        Date: blogEntity.Date,
                        Tags: tags,
                        RenderLinks: true,
                        BaseLink: '/blog',
                    },
                },
                URL: blogEntity.context,
                ButtonText: 'Read More',
                Configuration: {
                    Caption: {
                        VerticalPosition: 'Middle',
                        HorizontalPosition: 'Center',
                        DesktopTextWidth: 10,
                    },
                },
            },
        ];
        const carouselContentProps = {
            context: this.props.context,
            siteConfiguration: this.props.site.SiteConfiguration,
            carouselItemType: 'HeroBanner',
            carouselItems,
            carouselTiles: 0,
            carouselHeight: configuration.CarouselHeight,
            carouselHeightMobile: configuration.CarouselHeightMobile,
            arrowOffsetY: 0,
            autoScroll: true,
        };
        return carouselContentProps;
    }
    render() {
        const blogArticleProps = this.props.blog ? this.props.blog : {};
        const renderContent = blogArticleProps
                            && blogArticleProps.MarkdownText;
        return (
            <div>
                {renderContent
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
            </div>
        );
    }
}

BlogHeroImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    blog: React.PropTypes.object,
    page: React.PropTypes.object,
    site: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const blogItem = state.blogList && state.blogList.items ? state.blogList.items[0] : {};
    return {
        blog: blogItem,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    const actions = Object.assign({},
        EntityActions
        );
    return {
        actions: bindActionCreators(actions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(BlogHeroImageContainer);
