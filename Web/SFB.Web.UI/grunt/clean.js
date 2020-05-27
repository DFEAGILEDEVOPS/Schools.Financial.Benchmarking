module.exports = function () {
  var pathUtil = require('path');
  return {
    "elements-css": {
      force: true,
      src: pathUtil.join(process.cwd(), './public/assets/stylesheets')
    }
  };
};