import React from "react";

// Track with 
// export default function Component() {
//   //...
// }
// Component.whyDidYouRender = true
//
// https://github.com/welldone-software/why-did-you-render#tracking-components

if (process.env.NODE_ENV === "development") {
  const whyDidYouRender = require("@welldone-software/why-did-you-render");
  whyDidYouRender(React, {
    trackAllPureComponents: true
  });
}