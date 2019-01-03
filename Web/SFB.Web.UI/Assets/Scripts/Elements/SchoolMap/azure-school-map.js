(function ($) {
    "use strict";
    window.GOVUK = window.GOVUK || {};

    function AzureLocationMap(options) {
        if (!options.elementId || options.elementId === '')
            throw new ReferenceError("options.elementId is not present.");

        this.mapApiKey = options.mapApiKey;
        this.mapElementId = options.elementId;
        if (!this.mapElement)
            throw new ReferenceError("mapElement did not resolve using the id: " + options.elementId);

        if (!options.primaryMarker)
            throw new ReferenceError("options.primaryMarker is not present.");
        this.primaryMarker = options.primaryMarker;
        this.hasMap = options.hasMap;
        this.defaultZoom = 14;
        this.initialize();
        setTimeout(function () {
            //$("a:contains('Terms of Use')").attr("tabindex", "-1");
            //$("a:contains('Report a map error')").attr("tabindex", "-1");
            $(".gm-style").children().first().attr("aria-label", "An azure map of the school's location");
        },
            1500);
    }

    AzureLocationMap.prototype = {

        get mapElement() {
            return document.getElementById(this.mapElementId);
        },

        initialize: function () {
            if (this.hasMap) {
                this.centreLatLng = [this.primaryMarker.geometry.location.lat, this.primaryMarker.geometry.location.lng];
                this.setMapOptions();
                this.addMarker(this.primaryMarker);
            }
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

            this.azureMap = L.map(this.mapElementId).setView(this.centreLatLng, this.defaultZoom);
            L.tileLayer(
                'https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key='+this.mapApiKey,
                mapOptions)
                .addTo(this.azureMap);
        },

        addMarker: function () {
            L.marker(this.centreLatLng).addTo(this.azureMap);
        }

    };

    GOVUK.AzureLocationMap = AzureLocationMap;

}(jQuery));