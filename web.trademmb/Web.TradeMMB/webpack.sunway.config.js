const webpackBaseConfig = require('./webpackBaseConfig');


/**
 * Exported method to build up webpack configurations for each site.
 * @return {object} The configurations to build.
 */
module.exports = function () {
    return webpackBaseConfig.setup('Sunway', 'development')
        .then(() => {
            const config = webpackBaseConfig.configureSiteConfig();
            return config;
        });
}
