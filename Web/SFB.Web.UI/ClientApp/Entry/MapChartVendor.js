// temp fix so we can stop publishing node_modules
// in the build pipeline
import  'leaflet/dist/leaflet';
window.L = L;
import 'leaflet-fullscreen';
import 'leaflet.markercluster/dist/leaflet.markercluster';

// todo : update imports to grab only what is needed from these libs
import * as d3 from 'd3';
import c3 from 'c3';

window.d3 = d3;
window.c3 = c3;