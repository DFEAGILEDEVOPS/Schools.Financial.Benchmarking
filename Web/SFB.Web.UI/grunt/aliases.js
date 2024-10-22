module.exports = {
  "compile-dev-assets": [
    "compile-dev-js"
  ],
  "compile-dev-js": [    
    "terser:bundle-elements-dev"
  ],
  "compile-dev-css": [
    "clean:elements-css",
    "sass:generate-dev"
  ]
};
