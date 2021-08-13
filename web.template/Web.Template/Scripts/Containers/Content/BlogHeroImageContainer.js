import * as EntityActions from '../../actions/content/entityActions';

import CarouselContent from '../../widgets/content/carouselcontent';
import React from 'react';
import StringFunctions from '../../library/stringfunctions';

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class BlogHeroImageContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            blogInit: false,
        };
    }
    componentDidMount() {
        if (!this.state.blogInit) {
            const blogName = this.getBlogName();
            if (blogName) {
                this.props.actions.loadEntity(this.props.site.Name, 'BlogItem', blogName, 'live');
                this.setState({
                    blogInit: true,
                });
            }
        }
    }
    getBlogName() {
        const blogItems = this.props.page.EntityInformations.filter(entityInfo =>
            entityInfo.Name === 'blogitem');
        const blogName = blogItems[0] ? blogItems[0].Value : '';
        return blogName;
    }
    getCarouselItemsClass() {
        const contentModel = this.props.entity.model;
        const config = contentModel.Configuration;
        const heightClass = StringFunctions.heightClassFromEnum(config.CarouselHeight);
        const mobileHeightClass
                = StringFunctions.heightClassFromEnum(config.CarouselHeightMobile);
        const className = `carousel-items ${heightClass} mobile-${mobileHeightClass}`;
        return className;
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
                URL: this.props.context,
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
        const renderContent = this.state.blogInit
                            && blogArticleProps
                            && blogArticleProps.HeroImage;
        return (
            <div>
                {renderContent
                    && <CarouselContent {...this.setupCarouselContentProps()} />}
                {!renderContent
                    && <div className={this.getCarouselItemsClass()}></div>}
            </div>
        );
    }
}

BlogHeroImageContainer.propTypes = {
    context: React.PropTypes.string.isRequired,
    entity: React.PropTypes.object.isRequired,
    site: React.PropTypes.object.isRequired,
    actions: React.PropTypes.object,
    blog: React.PropTypes.object,
    page: React.PropTypes.object,
};


/**
 * map state to props
 * @param {object} state - The current state.
 * @param {object} ownProps - The current props.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const blogItems = state.page.EntityInformations.filter(entityInfo =>
        entityInfo.Name === 'blogitem');
    const blogName = blogItems[0] ? blogItems[0].Value : '';
    const blogKey = `BlogItem-${blogName}`;
    const blogEntity = state.entities[blogKey] ? state.entities[blogKey] : {};
    return {
        blog: blogEntity.model,
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
