module.exports = function () {
  return {
    'generate-dev': {
      files: {
        'public/assets/stylesheets/main.css': 'Assets/Sass/main.scss',
        'public/assets/stylesheets/main-ie6.css': 'Assets/Sass/main-ie6.scss',
        'public/assets/stylesheets/main-ie7.css': 'Assets/Sass/main-ie7.scss',
        'public/assets/stylesheets/main-ie8.css': 'Assets/Sass/main-ie8.scss',
		'public/assets/stylesheets/elements-page.css': 'Assets/Sass/elements-page.scss',
        'public/assets/stylesheets/elements-page-ie6.css': 'Assets/Sass/elements-page-ie6.scss',
        'public/assets/stylesheets/elements-page-ie7.css': 'Assets/Sass/elements-page-ie7.scss',
        'public/assets/stylesheets/elements-page-ie8.css': 'Assets/Sass/elements-page-ie8.scss'
      },
      options: {
        includePaths: ['node_modules/govuk_frontend_toolkit/stylesheets', 'node_modules/govuk_frontend_toolkit/stylesheets/design-patterns'],
        outputStyle: 'expanded',
        imagePath: '../images',
        sourceMap: true
      }
    }
  };
};
