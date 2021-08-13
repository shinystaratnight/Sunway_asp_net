const uglifyJS = require('uglifyjs-webpack-plugin');
const extractTextPlugin = require('extract-text-webpack-plugin');
const optimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const styleLintPlugin = require('stylelint-webpack-plugin');
const precss = require('precss');
const autoprefixer = require('autoprefixer');
const path = require('path');
const merge = require('webpack-merge');
const siteconfig = require('./scripts/siteconfig');
const webpack = require('webpack');
const HappyPack = require('happypack');

HappyPack.SERIALIZABLE_OPTIONS = HappyPack.SERIALIZABLE_OPTIONS.concat(['postcss']);

const WebpackBaseConfig = {
    site: {},
    environment: '',
    common: {
        context: path.resolve(__dirname),
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    enforce: 'pre',
                    include: [
                        path.resolve(__dirname, 'scripts'),
                        path.resolve(__dirname, 'sites')
                    ],
                    exclude: [
                        /node_modules/,
                        /server.js/
                    ],
                    use: 'happypack/loader?id=eslint'
                },
                {
                    test: /\.jsx?$/,
                    include: [
                        path.resolve(__dirname, 'scripts'),
                        path.resolve(__dirname, 'sites')
                    ],
                    exclude: [
                        /node_modules/
                    ],
                    use: 'happypack/loader?id=jsx'
                },
                {
                    test: /\.css$/,
                    use: extractTextPlugin.extract({
                        use: [
                            {
                                loader: 'css-loader',
                                options: {
                                    sourceMap: false,
                                    minimize: true
                                }
                            },
                            {
                                loader: 'postcss-loader'
                            },
                        ]
                    })
                }
            ]
        },
        plugins: [
            new webpack.DllReferencePlugin({
                context: path.resolve(__dirname),
                manifest: require('./assets/vendor-manifest.json')
            }),
            new HappyPack({
                id: 'eslint',
                threads: 2,
                loaders: [
                    {
                        loader: 'eslint-loader',
                        options: {
                            failOnWarning: false,
                            failOnError: false
                        }
                    }
                ]
            }),
            new HappyPack({
                id: 'jsx',
                threads: 4,
                loaders: [
                    {
                        loader: 'babel-loader',
                        options: {
                            presets: ['react', 'es2015']
                        },
                        query: {
                            cacheDirectory: true
                        }
                    }
                ]
            }),
            new styleLintPlugin({
                configFile: '.stylelintrc.json',
                files: './styles/**/*.s?(a|c)ss',
                syntax: 'scss',
                failOnError: false
            })
        ],
        resolve: {
            modules: [
                path.resolve('./scripts'),
                path.resolve('./styles'),
                path.resolve('./node_modules'),
            ],
             alias: {
                moment$: 'moment/moment.js'
            }
        },
        cache: true
    },
    setup: function setup(siteName, environment) {
        this.environment = environment;
        return new Promise((resolve) => {
            siteconfig.setupSite(siteName).then(config => {
                this.site = config;
                resolve(config);
            });
        });
    },
    configureConfig: function configureConfig(entry, plugins) {
        const config =
            merge(
                this.common,
                {
                    entry,
                    output: {
                        filename: `./assets/${this.site.name}/js/[name].js`
                    },
                    module: {
                        rules: [
                            {
                                test: /\.scss$/,
                                include: [
                                    path.resolve(__dirname, 'styles'),
                                    path.resolve(__dirname, `sites/${this.site.name}/styles`)
                                ],
                                use: extractTextPlugin.extract({
                                    use: 'happypack/loader?id=scss'
                                })
                            },
                            {
                                test: /\.(svg|png|jpg)$/,
                                loader: 'file-loader',
                                options: {
                                    name: `./assets/${this.site.name}/images/[name].[ext]`
                                }
                            }
                        ]
                    },
                    plugins: [
                        new uglifyJS({
                            cache: true,
                            parallel: 4,
                            sourceMap: false,
                            uglifyOptions: {
                                mangle: this.environment === 'production',
                                compress: { warnings: false }
                            },
                            extractComments: false
                        }),
                        new HappyPack({
                            id: 'scss',
                            threads: 4,
                            loaders: [
                                {
                                    loader: 'css-loader',
                                    options: {
                                        sourceMap: this.environment === 'development',
                                        minimize: true
                                    }
                                },
                                {
                                    loader: 'postcss-loader',
                                    options: {
                                        sourceMap: this.environment === 'development'
                                    }
                                },
                                {
                                    loader: 'sass-loader',
                                    options: {
                                        sourceMap: this.environment === 'development',
                                        data: this.site.themeVariables
                                    }
                                }
                            ]
                        }),
                        new webpack.DefinePlugin({
                            'process.env': {
                                'NODE_ENV': JSON.stringify(this.environment)
                            }
                        })
                    ],
                    watch: this.environment === 'development',
                    devtool: 'source-map'
                },
                {
                    plugins
                });
        return config;
    },
    configureSiteConfig: function configureSiteConfig() {
        const entry = {
            main: ['babel-polyfill', `./sites/${this.site.name}/scripts/main.js`],
            server: `./sites/${this.site.name}/scripts/server.js`
        };

        if (this.site.exportScript) {
            entry.export = ['babel-polyfill', `./sites/${this.site.name}/scripts/exportMain.js`];
        }

        const plugins = [
            new extractTextPlugin(`./assets/${this.site.name}/css/[name].css`)
        ];

        const config = this.configureConfig(entry, plugins);
        return config;
    },
};

module.exports = WebpackBaseConfig;