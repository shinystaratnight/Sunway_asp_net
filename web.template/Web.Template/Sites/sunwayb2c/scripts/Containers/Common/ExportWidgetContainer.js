import * as Containers from 'containers';
import * as EntityActions from 'actions/content/entityActions';
import React from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

class ExportWidgetContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            entity: {},
            lastModifiedDate: new Date(),
            entityInit: false,
        };
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
        if (this.props.context !== '' && !this.state.entityInit) {
            this.loadEntityIfNeeded(this.props.context);
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
        const key = `${this.props.entityName}-${this.props.context}`;
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
        return this.props.session.isLoaded
            && this.props.site.isLoaded
            && (this.state.entity.isLoaded || this.props.context === '');
    }
    containerProps() {
        const props = {
            entity: this.state.entity,
            context: this.props.context,
            session: this.props.session,
            site: this.props.site,
        };
        return props;
    }
    render() {
        let Container = {};
        if (this.props.customerContainers
            && this.props.customerContainers.hasOwnProperty(`${this.props.entityName}Container`)) {
            Container = this.props.customerContainers[`${this.props.entityName}Container`];
        } else {
            Container = Containers[`${this.props.entityName}Container`];
        }

        const renderContainer
            = this.isInitialised()
            && this.state.entity.status !== 'no content';

        const containerAttributes = renderContainer
            ? this.setupContainerAttributes()
            : {
                className: 'widget padding-none',
            };

        return (
            <div {...containerAttributes}>
                {renderContainer
                    && <Container {...this.containerProps()} />}
            </div>
        );
    }
}

ExportWidgetContainer.propTypes = {
    actions: React.PropTypes.object.isRequired,
    entityName: React.PropTypes.string.isRequired,
    context: React.PropTypes.string.isRequired,
    shared: React.PropTypes.bool,
    site: React.PropTypes.object.isRequired,
    session: React.PropTypes.object.isRequired,
    entities: React.PropTypes.object.isRequired,
    customerContainers: React.PropTypes.object,
};

/**
 * map state to props
 * @param {object} state - The current state.
 * @return {object} the props mapped to state.
 */
function mapStateToProps(state) {
    const session = state.session ? state.session : {};
    const entities = state.entities ? state.entities : {};
    const site = state.site ? state.site : {};

    return {
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

export default connect(mapStateToProps, mapDispatchToProps)(ExportWidgetContainer);
