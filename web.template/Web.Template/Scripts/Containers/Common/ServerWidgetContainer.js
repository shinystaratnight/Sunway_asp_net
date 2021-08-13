import * as Containers from '../../containers';
import React from 'react';

class ServerWidgetContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    setupContainerAttributes() {
        const containerAttributes = {
            className: `widget widget-container widget-${this.props.entityName.toLowerCase()}`,
            style: {},
        };

        if (this.state.entity.isLoaded) {
            const contentModel = JSON.parse(this.props.contentJSON);
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
    containerProps() {
        const entity = {
            name: this.props.entityName,
            context: this.props.context,
            model: JSON.parse(this.props.contentJSON),
            isLoaded: true,
            isFetching: false,
        };

        const session = {
            UserSession: this.props.user,
            Success: true,
            Warnings: [],
        };

        const props = {
            entity,
            context: this.props.context,
            session,
            page: this.props.page,
            entitiesCollection: this.props.entitiesCollection,
            specificEntitiesCollection: this.props.specificEntitiesCollection,
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

        const containerAttributes = this.setupContainerAttributes();
        return (
            <div {...containerAttributes}>
                <Container {...this.containerProps()} />}
            </div>
        );
    }
}

ServerWidgetContainer.propTypes = {
    entityName: React.PropTypes.string.isRequired,
    context: React.PropTypes.string.isRequired,
    contentJSON: React.PropTypes.string.isRequired,
    user: React.PropTypes.object,
    page: React.PropTypes.object,
    site: React.PropTypes.object,
    entitiesCollection: React.PropTypes.object,
    specificEntitiesCollection: React.PropTypes.object,
    customerContainers: React.PropTypes.object,
};

export default ServerWidgetContainer;
