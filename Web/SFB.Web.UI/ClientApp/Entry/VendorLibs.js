// legacy js extensively relies on these libs.
// moved as a temp fix so we can stop publishing node_modules 
// in the build pipeline

import $ from 'jquery';
import _ from 'lodash';
import {jsPDF} from 'jspdf';
// legacy scripts require GOVUKFrontend as a global
const GOVUKFrontend = require('govuk-frontend');
// export for others scripts to use
window.$ = $;
window.jQuery = $;
window._ = _;
window.jsPDF = jsPDF;
window.GOVUKFrontend = GOVUKFrontend;
GOVUKFrontend.initAll();