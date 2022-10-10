'use strict';
const webpack = require('webpack');
const path = require('path');
const glob = require('glob');
const buildDir = path.resolve('./public/build/');
const entryDirPath = './ClientApp/Entry/**/*';
const scssEntryPath = './ClientApp/styles/*.scss';
const entryFiles = path.join(entryDirPath, '**/*');
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const {CleanWebpackPlugin} = require('clean-webpack-plugin');
const StyleLintPlugin = require('stylelint-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");

const config = {
  entry: () => {
    const jsFiles = glob.sync(entryDirPath)
      .reduce((acc, filePath) => {
        const file = path.parse(filePath);
        acc[file.name] = path.resolve(process.cwd(), filePath);
        return acc;
      }, {});

    const cssFiles = glob
      .sync(scssEntryPath)
      .reduce((acc, filePath) => {
        const file = path.parse(filePath);
        acc[file.name] = path.resolve(process.cwd(), filePath);
        return acc;
      }, {});

    return Object.assign(jsFiles, cssFiles);
  },
  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin({
      extractComments: false,
      terserOptions: {
        format: {
          comments: false,
        },
      },
    })],
  },
  output: {
    filename: '[name].[contenthash].js',
    path: buildDir
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js', '.json']
  },
  module: {
    rules: [
      {
        test: /\.(tsx|ts)?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [
              ['@babel/preset-env', {
                useBuiltIns: 'usage',
                corejs: 3
              }]
            ]
          }
        },
      },
      {
        test: /\.(scss|css)$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
            options: {
              publicPath: '../../',
            },
          },
          {
            loader: 'css-loader',
            options: {
              url: false,
            }
          },
          {
            loader: 'postcss-loader',
            // options: {
            //     postcssOptions: {
            //         plugins: [
            //             base64({
            //                 excludeAtFontFace: false,
            //                 replaceValues: true,
            //                 extensions: ['.woff2', '.woff']
            //             }),
            //         ]
            //     }
            // }
          },
          {loader: 'sass-loader'}
        ],
      },

    ]
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: './[name].[contenthash].css',
    }),


    new CleanWebpackPlugin(),

    /*new webpack.ProvidePlugin({
        $: "jquery",
        jQuery: "jquery"
    }),*/

  ]

};

module.exports = (env, argv) => {
  if (argv.mode === 'development') {
    config.devtool = 'source-map';
    config.optimization.minimize = false;
  }
  if (argv.mode === 'production') {
    config.devtool = 'source-map';
  }
  if (!argv.env.ci) {
    config.devtool = 'source-map';
    config.plugins.push(
      // new StyleLintPlugin({
      //     configFile: '.stylelintrc',
      //     configBaseDir: 'node_modules',
      //     context: "./Assets/Sass"
      // }),

      new CleanWebpackPlugin(),

      // new webpack.SourceMapDevToolPlugin({
      //     filename: '[file].map[query]',
      //     columns: false,
      //     exclude: /node_modules/,
      //     test: /\.css?|\.js?$/,
      // }),

    );
  } else {
    delete config.devtool;
  }

  return config;
};

