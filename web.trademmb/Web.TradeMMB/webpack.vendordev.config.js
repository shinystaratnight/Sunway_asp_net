var uglifyJS = require('uglifyjs-webpack-plugin');
var webpack = require('webpack');
var path = require('path');

module.exports = {
    entry: {
        vendor: [
          'babel-polyfill',
          'react',
          'react-redux',
          'react-dom',
          'moment',
          'isomorphic-fetch',
          'commonmark',
          'commonmark-react-renderer',
          'react-datepicker'
        ]
    },
    output: {
        filename: '[name].bundle.js',
        path: path.resolve(__dirname, 'assets/js'),

        // The name of the global variable which the library's
        // require() function will be assigned to
        library: '[name]_lib'
    },
    plugins: [
        new webpack.DllPlugin({
            // The path to the manifest file which maps between
            // modules included in a bundle and the internal IDs
            // within that bundle
            path: path.resolve(__dirname, 'assets/[name]-manifest.json'),
            // The name of the global variable which the library's
            // require function has been assigned to. This must match the
            // output.library option above
            name: '[name]_lib'
        }),
        new uglifyJS({
            cache: true,
            parallel: 4,
            sourceMap: false,
            uglifyOptions: {
                mangle: false,
                compress: { warnings: false }
            },
            extractComments: false
        })
    ],
    resolve: {
        alias: {
            moment$: 'moment/moment.js'
        }
    }
}
