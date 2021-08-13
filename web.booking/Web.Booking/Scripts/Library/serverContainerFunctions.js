class ServerContainerFunctions {
    /**
     * Function to get query string values from a url
     * @param {array} entityTypes - The entities to obtain
     * @param {object} entities - The data source
     * @return {Object} returnObject - The query key values
    */
    static getSpecificEntities(entityTypes, entities) {
        const returnObject = {};
        entityTypes.forEach(entityType => {
            const key = `${entityType.type}${entityType.source}`;
            const entity
                = entities[key]
                    && entities[key].Success
                    && entities[key].ContentJSON
                    ? JSON.parse(entities[key].ContentJSON)
                    : false;
            returnObject[entityType.name] = entity;
        });
        return returnObject;
    }

    /**
     * Function to get query string values from a url
     * @param {string} context - The widget context
     * @param {object} contentModel - The data source
     * @return {Object} containerAttributes - The containerAttributes for the widget
    */
    static setupContainerAttributes(context, contentModel) {
        const containerAttributes = {
            className: `widget widget-container widget-${context.toLowerCase()}`,
            style: {},
        };
        const configuration = contentModel.Configuration;

        if (configuration) {
            switch (configuration.ContainerStyle) {
                case 'Highlight':
                    containerAttributes.className = `${containerAttributes.className} highlight`;
                    break;
                case 'Alternate':
                    containerAttributes.className = `${containerAttributes.className} alt`;
                    break;
                default:
            }
            switch (configuration.ContainerPadding) {
                case 'None':
                    containerAttributes.className = `${containerAttributes.className} padding-none`;
                    break;
                case 'Small':
                    containerAttributes.className = `${containerAttributes.className} padding-sm`;
                    break;
                case 'Medium':
                    containerAttributes.className = `${containerAttributes.className} padding-md`;
                    break;
                case 'Large':
                    containerAttributes.className = `${containerAttributes.className} padding-lg`;
                    break;
                default:
            }
        }

        if (contentModel.BackgroundImage) {
            containerAttributes.style.backgroundImage = `url(${contentModel.BackgroundImage})`;
        }
        return containerAttributes;
    }

}

export default ServerContainerFunctions;
