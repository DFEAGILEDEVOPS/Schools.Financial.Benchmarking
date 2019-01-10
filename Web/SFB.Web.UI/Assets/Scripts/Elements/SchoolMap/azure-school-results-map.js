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
        this.defaultZoom = 14;
        this.initialize();
        setTimeout(function () {
            //$("a:contains('Terms of Use')").attr("tabindex", "-1");
            //$("a:contains('Report a map error')").attr("tabindex", "-1");
            $(".gm-style").children().first().attr("aria-label", "An azure map of the schools' locations");
        },
            1500);
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
        }
    };

    GOVUK.AzureSchoolLocationsMap = AzureSchoolLocationsMap;

}(jQuery));