const webpackBaseConfig = require('./webpackBaseConfig');
const sites = ['Sunway'];

/**
 * Exported method to build up webpack configurations for each site.
 * @return {object} The configurations to build.
 */
module.exports = function () {
    const configs = [];

    const promises = [];
    sites.forEach(site => {
        const promise = new Promise((resolve) => {
            webpackBaseConfig.setup(site, 'development')
                .then(() => {
                    const siteConfig = webpackBaseConfig.configureSiteConfig();
                    configs.push(siteConfig);
                    resolve(siteConfig);
                });
        });
        promises.push(promise);
    });

    return Promise.all(promises).then(() => configs);
}
