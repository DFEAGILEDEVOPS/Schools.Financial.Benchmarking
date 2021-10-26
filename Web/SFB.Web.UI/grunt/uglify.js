module.exports = function (grunt) {
    var pathUtil = require('path');
    return {
        "bundle-elements-dev": {
            options: {
                sourceMap: true,
                beautify: false,
                compress: false,
                mangle: true
            },
            src: [
              'Assets/Scripts/*.js',
              'Assets/Scripts/Behaviours/Collapsible/*.js',
              'Assets/Scripts/Behaviours/Accordion/*.js',
              'Assets/Scripts/Behaviours/PartialRequest/*.js',
              'Assets/Scripts/Behaviours/floatThead/*.js',
              'Assets/Scripts/Elements/Forms/*.js',
              'Assets/Scripts/Elements/Modal/*.js',
              'Assets/Scripts/Elements/SchoolMap/*.js',
              'Assets/Scripts/Views/**/*.js',
              'Public/Scripts/stickyfill/*.js',
              'Public/Scripts/modernizr/*.js'
            ],
            dest: 'public/assets/scripts/application.js'
        }
    };
};