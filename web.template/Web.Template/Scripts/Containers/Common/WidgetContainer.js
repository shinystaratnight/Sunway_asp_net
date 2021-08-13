import * as Containers from '../../containers';
import * as EntityActions from '../../actions/content/entityActions';
import React from 'react';
import WidgetStatus from '../../components/common/widgetstatus';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class WidgetContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            entity: {},
            lastModifiedDate: new Date(),
            context: this.props.context,
            entityInit: false,
        };
        this.handleEdit = this.handleEdit.bind(this);
        this.handlePublish = this.handlePublish.bind(this);

        const entity = this.getEntity(this.props.entities);
        this.state.entity = entity;
    }
    componentDidMount() {
        this.entityInit();
    }
    componentWillUpdate() {
        this.entityInit();
    }
    componentWillReceiveProps(nextProps) {
        const entity = this.getEntity(nextProps.entities);
        if (entity.isLoaded
            && entity.hasOwnProperty('lastModifiedDate')
            && this.state.lastModifiedDate.toString()
                !== entity.lastModifiedDate.toString()) {
            this.setState({
                lastModifiedDate: entity.lastModifiedDate,
                entity: Object.assign({}, entity),
            });
        }
    }
    entityInit() {
        if (!this.state.entityInit) {
            if (this.props.entitySpecific) {
                this.setEntitySpecificContext();
            }
            const entity = this.getEntity(this.props.entities);
            if (entity.isLoaded) {
                this.setState({
                    entity: Object.assign({}, entity),
                    lastModifiedDate: entity.lastModifiedDate,
                    entityInit: true,
                });
            }
        }
        if (!this.props.entitySpecific && this.props.context !== '' && !this.state.entityInit) {
            this.loadEntityIfNeeded(this.props.context);
        }
        if (this.props.entitySpecific && !this.state.entityInit && this.props.page.isLoaded) {
            const context = this.setEntitySpecificContext();
            this.loadEntityIfNeeded(context);
        }
    }
    loadEntityIfNeeded(context) {
        const mode = this.getContentMode();
        const site = this.props.shared ? 'Shared' : this.props.site.Name;
        this.state.entityInit = true;
        this.props.actions.loadEntityIfNeeded(site, this.props.entityName, context, mode);
    }
    getEntity(entities) {
        let entity = {};
        const key = `${this.props.entityName}-${this.state.context}`;
        if (entities[key]) {
            entity = entities[key];
        }
        return entity;
    }
    getContentMode() {
        let mode = 'live';
        if (this.props.session.isLoaded && this.props.session.UserSession.AdminMode) {
            mode = 'draft';
        }
        return mode;
    }
    setEntitySpecificContext() {
        const page = this.props.page;
        let context = this.props.context;
        if (page.EntityType && page.EntityInformations) {
            let entityValue = '';
            page.EntityInformations.forEach(entityInformation => {
                if (!entityInformation.Hide) {
                    entityValue += `${entityInformation.UrlSafeValue}-`;
                }
            });
            entityValue = entityValue.substring(0, entityValue.length - 1);
            context = `${context}_${entityValue}`;
            this.state.context = context;
        }
        return context;
    }
    handleEdit() {
        const site = this.props.shared ? 'Shared' : this.props.site.Name;
        this.props.actions.editEntity(site, this.props.entityName, this.state.context);
    }
    handlePublish() {
        const site = this.props.shared ? 'Shared' : this.props.site.Name;
        this.props.actions.publishEntity(site, this.props.entityName,
            this.state.context, this.state.entity);
    }
    setupWidgetStatusProps() {
        const widgetStatusProps = {
            name: this.props.entityName,
            context: this.props.context,
            status: this.state.entity.status ? this.state.entity.status : '',
            lastModifiedDate: this.state.lastModifiedDate,
            lastModifiedUser: this.state.entity.lastModifiedUser
                ? this.state.entity.lastModifiedUser : '',
            onEdit: this.handleEdit,
            onPublish: this.handlePublish,
        };
        return widgetStatusProps;
    }
    renderWidgetStatus() {
        const widgetStatusProps = this.setupWidgetStatusProps();
        return (
            <div className="container widget-status-container">
                <WidgetStatus {...widgetStatusProps} />
            </div>
        );
    }
    setupContainerAttributes() {
        const containerAttributes = {
            className: `widget widget-container widget-${this.props.entityName.toLowerCase()}`,
            style: {},
        };

        if (this.state.entity.isLoaded) {
            const contentModel = this.state.entity.model;
            const configuration = contentModel.Configuration;

            if (configuration) {
                switch (configuration.ContainerStyle) {
                    case 'Highlight':
                        containerAttributes.className
                            = `${containerAttributes.className} highlight`;
                        break;
                    case 'Alternate':
                        containerAttributes.className = `${containerAttributes.className} alt`;
                        break;
                    default:
                }
                switch (configuration.ContainerPadding) {
                    case 'None':
                        containerAttributes.className
                            = `${containerAttributes.className} padding-none`;
                        break;
                    case 'Small':
                        containerAttributes.className
                            = `${containerAttributes.className} padding-sm`;
                        break;
                    case 'Medium':
                        containerAttributes.className
                            = `${containerAttributes.className} padding-md`;
                        break;
                    case 'Large':
                        containerAttributes.className
                            = `${containerAttributes.className} padding-lg`;
                        break;
                    default:
                }
            }

            if (contentModel.BackgroundImage) {
                containerAttributes.style.backgroundImage = `url(${contentModel.BackgroundImage})`;
            }
        }

        return containerAttributes;
    }
    isInitialised() {
        return this.props.page.isLoaded
            && this.props.session.isLoaded
            && this.props.site.isLoaded
            && (this.state.entity.isLoaded || this.props.context === '');
    }
    containerProps() {
        const props = {
            entity: this.state.entity,
            context: this.props.context,
            session: this.props.session,
            page: this.props.page,
            site: this.props.site,
            customerWidgets: this.props.customerWidgets ? this.props.customerWidgets : {},
            customerComponents: this.props.customerComponents ? this.props.customerComponents : {},
        };
        return props;
    }
    renderBalsamiqImage() {
        const imagePathXs = `/images/balsamiq/${this.props.entityName}Xs.png`;
        const imagePathSm = `/images/balsamiq/${this.props.entityName}Sm.png`;
        const imagePathMd = `/images/balsamiq/${this.props.entityName}Md.png`;
        const imagePath = `/images/balsamiq/${this.props.entityName}.png`;
        return (
            <div className="container-fluid">
                <img className="balsamiq-image hidden-xs hidden-sm hidden-md" src={imagePath}/>
                <img className="balsamiq-image hidden-xs hidden-sm hidden-lg" src={imagePathMd}/>
                <img className="balsamiq-image hidden-xs hidden-md hidden-lg" src={imagePathSm}/>
                <img className="balsamiq-image hidden-lg hidden-sm hidden-md" src={imagePathXs}/>
            </div>
        );
    }
    shouldRenderWidget() {
        let renderWidget = true;
        const contentModel = this.state.entity.model;
        const configuration = contentModel ? contentModel.Configuration : null;
        if (configuration
            && typeof configuration === 'object'
            && configuration.hasOwnProperty('DisplayWidget')
            && !configuration.DisplayWidget) {
            renderWidget = false;
        }
        return renderWidget;
    }
    render() {
        let Container = {};
        if (this.props.customerContainers
            && this.props.customerContainers.hasOwnProperty(`${this.props.entityName}Container`)) {
            Container = this.props.customerContainers[`${this.props.entityName}Container`];
        } else {
            Container = Containers[`${this.props.entityName}Container`];
        }
        const renderWidget = this.isInitialised() && this.shouldRenderWidget();
        const renderContainer
            = this.isInitialised()
            && this.state.entity.status !== 'no content'
            && renderWidget;
        const renderBalsamiq = this.isInitialised() && !renderContainer
            && this.props.session.UserSession && this.props.session.UserSession.AdminMode;
        const containerAttributes = renderContainer
            ? this.setupContainerAttributes()
            : {
                className: 'widget padding-none',
            };
        if (!renderWidget && this.props.session.UserSession
            && !this.props.session.UserSession.AdminMode) {
            containerAttributes.className += ' hidden';
        }
        return (
            <div {...containerAttributes}>

                {this.isInitialised()
                    && this.props.context !== ''
                    && this.props.session.UserSession.AdminMode
                    && this.props.editable
                    && this.renderWidgetStatus()}

                {renderContainer
                    && <Container {...this.containerProps()} />}

                {renderBalsamiq
                    && this.renderBalsamiqImage()}
            </div>
        );
    }
}

WidgetContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entityName: React.PropTypes.string.isRequired,
    context: React.PropTypes.string.isRequired,
    editable: React.PropTypes.bool,
    entitySpecific: React.PropTypes.bool,
    shared: React.PropTypes.bool,
    site: React.PropTypes.object.isRequired,
    page: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    customerContainers: React.PropTypes.object,
    customerWidgets: React.PropTypes.object,
    customerComponents: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const page = state.page ? state.page : {};
    const session = state.session ? state.session : {};
    const entities = state.entities ? state.entities : {};
    const site = state.site ? state.site : {};

    return {
        page,
        session,
        entities,
        site,
    };
}

/**
 * map dispatch to props
 * @param {object} dispatch - redux dispatch
 * @return {object} entity actions
 */
function mapDispatchToProps(dispatch) {
    return {
        actions: bindActionCreators(EntityActions, dispatch),
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(WidgetContainer);
