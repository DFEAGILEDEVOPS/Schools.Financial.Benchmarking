module.exports = function (grunt) {
    var pathUtil = require('path');
    return {
        "bundle-elements-dev": {
            options: {
                sourceMap: true,
                beautify: true,
                compress: false,
                mangle: false
            },
            src: [
              // couldn't use /**/*.js as govuk/analytics/error-tracking.js causes an error with GOVUK undefined
              'node_modules/govuk_frontend_toolkit/javascripts/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
              'Assets/Scripts/*.js',
              'Assets/Scripts/Behaviours/Collapsible/*.js',
              'Assets/Scripts/Behaviours/Accordion/*.js',
              'Assets/Scripts/Behaviours/PartialRequest/*.js',
              'Assets/Scripts/Behaviours/floatThead/*.js',
              'Assets/Scripts/Elements/**/*.js',
              'Assets/Scripts/Views/*.js',
              'Public/Scripts/stickyfill/*.js',
              'Public/Scripts/modernizr/*.js'
            ],
            dest: 'public/assets/scripts/application.js'
        }
    };
};