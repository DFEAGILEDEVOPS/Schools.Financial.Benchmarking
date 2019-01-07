const presets = [
  [
    "@babel/env",
    {
      targets: {
        chrome: "67"
      },
      useBuiltIns: "entry",
    },
  ],
];

module.exports = { presets };