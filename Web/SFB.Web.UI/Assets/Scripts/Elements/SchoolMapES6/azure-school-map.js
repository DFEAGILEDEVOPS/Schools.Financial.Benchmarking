﻿(function ($) {
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
        this.fullScreen = options.fullScreen;
        this.initialize();
    }

    AzureLocationMap.prototype = {

        get mapElement() {
            return document.getElementById(this.mapElementId);
        },

        initialize: function () {
            if (this.hasMap) {
                this.centreLatLng = [this.primaryMarker.geometry.location.lat, this.primaryMarker.geometry.location.lng];
                this.setMapOptions();
                this.addMarker();
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

            this.azureMap = L.map(this.mapElementId, { attributionControl: false }).setView(this.centreLatLng, this.defaultZoom);

            this.azureMap.addControl(L.control.attribution({
                prefix: ''
            }));

            if (this.fullScreen) {
                this.azureMap.addControl(new L.Control.Fullscreen());
            }

            L.tileLayer(
                'https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key=' + this.mapApiKey,
                mapOptions)
                .addTo(this.azureMap);
        },

        addMarker: function () {
          const blackIcon = L.icon({
            iconUrl: '/public/assets/images/icons/icon-location.png',
            iconSize: [20, 32]
          });
            L.marker(this.centreLatLng, { icon: blackIcon }).addTo(this.azureMap);
        }
    };

    GOVUK.AzureLocationMap = AzureLocationMap;

}(jQuery));