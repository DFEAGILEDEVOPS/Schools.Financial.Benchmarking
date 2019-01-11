(function ($) {
    "use strict";
    window.GOVUK = window.GOVUK || {};

    function AzureSchoolLocationsMap(options) {
        if (!options.elementId || options.elementId === '')
            throw new ReferenceError("options.elementId is not present.");

        this.mapApiKey = options.mapApiKey;
        this.mapElementId = options.elementId;
        if (!this.mapElement)
            throw new ReferenceError("mapElement did not resolve using the id: " + options.elementId);

        if (!options.primaryMarker)
            throw new ReferenceError("options.primaryMarker is not present.");
        this.primaryMarker = options.primaryMarker;
        this.defaultZoom = 6;
        this.topLayer = null;
        this.initialize();
    }

    AzureSchoolLocationsMap.prototype = {

        get mapElement() {
            return document.getElementById(this.mapElementId);
        },

        initialize: function () {
                this.centreLatLng = [this.primaryMarker.geometry.location.lat, this.primaryMarker.geometry.location.lng];
                this.setMapOptions();
        },

        setMapOptions: function () {
            var mapOptions = {
                attribution: '© ' + new Date().getFullYear() + ' Microsoft, © 1992 - ' + new Date().getFullYear() + ' TomTom',
                maxZoom: 18,
                minZoom: 4,
                id: 'azuremaps.road',
                crossOrigin: true,
                subscriptionKey: this.mapApiKey
            };

            this.azureMap = L.map(this.mapElementId, { attributionControl: false }).setView(this.centreLatLng, this.defaultZoom);

            this.azureMap.addControl(L.control.attribution({
                prefix: ''
            }));

            L.tileLayer(
                'https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key='+this.mapApiKey,
                mapOptions)
                .addTo(this.azureMap);
        },

        renderMapPinsForAzureMap : function (response) {
            var data = response.results;

            //this.infoWindow = new google.maps.InfoWindow();
            //this.infoWindow.addListener("closeclick", function () {
            //    if (self.activeMarker) self.activeMarker.setIcon(self.iconBlack);
            //    self.activeMarker = null;
            //});

            var hashtable = {};

            var genKey = function genKey(lat, lng) {
                return lat + "#" + lng;
            };

            if (this.topLayer) {
                this.topLayer.clearLayers();
            }

            var latLangs = [];
            var markers = L.markerClusterGroup();

            for (var i = 0; i < data.length; i++) {
                // This is where we scatter any pins that have the exact same co-ordinates
                var adjustment = 0.00005; // put the school pin about 6 metres away from it's equivalent.

                var lat = new Number(data[i].Latitude);
                var lng = new Number(data[i].Longitude);
                var key = genKey(lat, lng);

                if (!hashtable[key]) {
                    hashtable[key] = key;
                } else {
                    lng += adjustment;
                    key = genKey(lat, lng);
                    hashtable[key] = key;
                }

                var marker = L.marker([data[i].Latitude, data[i].Longitude]);
                markers.addLayer(marker);
                latLangs.push([data[i].Latitude, data[i].Longitude]);
                var info = data[i];
                var html = "<div class=\"infowindow-school-summary\">\n                    <a href=\"/school/detail?urn=".concat(info.Id, "\">").concat(info.Name, "</a>\n                    <p>").concat(info.Address, "</p>\n                    <p>").concat(info.EducationPhases, "</p>\n                    <p>").concat(info.NFType, "</p>\n                    <div id=\"").concat(info.Id, "\" data-urn=\"").concat(info.Id, "\">");

                if (DfE.Util.ComparisonList.isInList(info.Id)) {
                    html += "<div class=\"button add add-remove\" style=\"display: none\" onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket('".concat(info.Id, "','Add')\">Add</div>\n                        <div class=\"button remove add-remove\" onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket('").concat(info.Id, "','Remove')\">Remove</div>");
                } else {
                    html += "<div class=\"button add add-remove\" onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket('".concat(info.Id, "','Add')\">Add</div>\n                        <div class=\"button remove add-remove\" style=\"display: none\" onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket('").concat(info.Id, "','Remove')\">Remove</div>");
                }

                marker.bindPopup(html);

                // window.google.maps.event.addListener(marker, "mouseover", (function (m) {
                //    return function (evt) {
                //        if (!self.activeMarker) m.setIcon(self.iconPink);
                //    }
                //})(marker));
                //window.google.maps.event.addListener(marker, "mouseout", (function (m) {
                //    return function (evt) {
                //        if (!self.activeMarker) m.setIcon(self.iconBlack);
                //    }
                //})(marker));
            }

            this.topLayer = markers;

            this.azureMap.addLayer(markers);

            this.azureMap.fitBounds(L.latLngBounds(latLangs));
        }
    };

    GOVUK.AzureSchoolLocationsMap = AzureSchoolLocationsMap;

}(jQuery));