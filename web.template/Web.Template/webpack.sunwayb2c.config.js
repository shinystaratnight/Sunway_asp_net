const webpackBaseConfig = require('./webpackBaseConfig');


/**
 * Exported method to build up webpack configurations for each site.
 * @return {object} The configurations to build.
 */
module.exports = function () {
    return webpackBaseConfig.setup('SunwayB2C', 'development')
        .then(() => {
            const siteConfig = webpackBaseConfig.configureSiteConfig();
            return siteConfig;
        });
}
