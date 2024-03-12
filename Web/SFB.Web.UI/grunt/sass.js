const sass = require('sass');
module.exports = function () {
  return {
    'generate-dev': {
      files: {
        'public/assets/stylesheets/main.css': 'Assets/Sass/main.scss',
      },
      options: {
        outputStyle: 'compressed',
        imagePath: '../images',
        sourceMap: true,
        implementation: sass
      }
     }
  };
};
