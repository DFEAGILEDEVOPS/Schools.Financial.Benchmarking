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
        this.fullScreen = options.fullScreen;
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

            if (this.fullScreen) {
                this.azureMap.addControl(new L.Control.Fullscreen());
            }

            L.tileLayer(
                'https://atlas.microsoft.com/map/tile/png?api-version=1&layer=basic&style=main&zoom={z}&x={x}&y={y}&subscription-key='+this.mapApiKey,
                mapOptions)
                .addTo(this.azureMap);
        },

        renderMapPinsForAzureMap : function (response) {
            var data = response.results;

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

                var blackIcon = L.icon({
                    iconUrl: '/public/assets/images/icons/icon-location.png',
                    iconSize: [20, 32]
                });

                var marker = L.marker([data[i].Latitude, data[i].Longitude], { icon: blackIcon });
                markers.addLayer(marker);
                latLangs.push([data[i].Latitude, data[i].Longitude]);
                var info = data[i];

                var html = `<div class="infowindow-school-summary">                    
                        <a class="govuk-link" href ="/school/detail?urn=${info.Id}">${info.Name}</a>
                        <p>${info.Address}</p>
                        <p>${info.EducationPhases}</p>
                        <p>${info.NFType}</p>
                        <div id ="${info.Id}" data-urn="${info.Id}">`;

                if (info.CompanyNumber !== "0") {
                    html += `<div class="mt-1" style="font-style: italic">Part of the </div>
                    <div class="mb-1"><a class="govuk-link" href="/trust/index?companyNo=${info.CompanyNumber}">${info.SponsorName}</a></div>`;
                }  

                if ($("#SearchMethod").val() === "Manual") {
                    if (DfE.Util.ComparisonList.isInManualList(info.Id)) {
                        html += `<button class="govuk-button govuk-!-margin-bottom-0 add add-remove" data-module="govuk-button" style="display: none" onclick="DfE.Views.SchoolsResultsViewModel.updateManualBasket('${info.Id}','Add')">Add</button>
                            <button class="govuk-button govuk-!-margin-bottom-0 govuk-button--secondary remove add-remove" data-module="govuk-button" onclick="DfE.Views.SchoolsResultsViewModel.updateManualBasket('${info.Id}','Remove')">Remove</button>`;
                    } else {
                        html += `<button class="govuk-button govuk-!-margin-bottom-0 add add-remove" data-module="govuk-button" onclick="DfE.Views.SchoolsResultsViewModel.updateManualBasket('${info.Id}','Add')">Add</button>
                            <button class="govuk-button govuk-!-margin-bottom-0 govuk-button--secondary remove add-remove" data-module="govuk-button" style="display: none" onclick="DfE.Views.SchoolsResultsViewModel.updateManualBasket('${info.Id}','Remove')">Remove</button>`;
                    }
                } else if ($("#SearchMethod").val() === "School"){
                    if (DfE.Util.ComparisonList.isInList(info.Id)) {
                        html += `<button class="govuk-button govuk-!-margin-bottom-0 add add-remove" data-module="govuk-button" style="display: none" onclick="DfE.Views.SchoolsResultsViewModel.updateBenchmarkBasket('${info.Id}','Add')">Add</button>
                            <button class="govuk-button govuk-!-margin-bottom-0 govuk-button--secondary remove add-remove" data-module="govuk-button" onclick="DfE.Views.SchoolsResultsViewModel.updateBenchmarkBasket('${info.Id}','Remove')">Remove</div>`;
                    } else {
                        html += `<button class="govuk-button govuk-!-margin-bottom-0 add add-remove" data-module="govuk-button" onclick="DfE.Views.SchoolsResultsViewModel.updateBenchmarkBasket('${info.Id}','Add')">Add</button>
                            <button class="govuk-button govuk-!-margin-bottom-0 govuk-button--secondary remove add-remove" data-module="govuk-button" style="display: none" onclick="DfE.Views.SchoolsResultsViewModel.updateBenchmarkBasket('${info.Id}','Remove')">Remove</button>`;
                    }
                }
                

                marker.bindPopup(html);
                marker.on('click', function (ev) {
                    ev.target.options.icon.options.iconUrl = "/public/assets/images/icons/icon-location-pink.png";
                    ev.target.refreshIconOptions();

                    var urn = $(ev.target.getPopup().getContent()).find('.add').parent().data('urn');
                    if ($("#SearchMethod").val() === "Manual") {
                        if (DfE.Util.ComparisonList.isInManualList(urn.toString())) {
                            $('.infowindow-school-summary').find('.add').hide();
                            $('.infowindow-school-summary').find('.remove').show();
                        } else {
                            $('.infowindow-school-summary').find('.add').show();
                            $('.infowindow-school-summary').find('.remove').hide();
                        }
                    } else {
                        if (DfE.Util.ComparisonList.isInList(urn.toString())) {
                            $('.infowindow-school-summary').find('.add').hide();
                            $('.infowindow-school-summary').find('.remove').show();
                        } else {
                            $('.infowindow-school-summary').find('.add').show();
                            $('.infowindow-school-summary').find('.remove').hide();
                        }
                    }
                });
                marker.on('popupclose', function (ev) {                    
                    ev.target.options.icon.options.iconUrl = "/public/assets/images/icons/icon-location.png";
                    ev.target.refreshIconOptions();
                });
            }

            this.topLayer = markers;

            this.azureMap.addLayer(markers);
 
            this.azureMap.fitBounds(L.latLngBounds(latLangs));
        },

        renderFederatonSchoolPinsForAzureMap: function (response) {
            var data = response.results;

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

                var blackIcon = L.icon({
                    iconUrl: '/public/assets/images/icons/icon-location.png',
                    iconSize: [20, 32]
                });

                var marker = L.marker([data[i].Latitude, data[i].Longitude], { icon: blackIcon });
                markers.addLayer(marker);
                latLangs.push([data[i].Latitude, data[i].Longitude]);
                var info = data[i];
                var html = `<div class="infowindow-school-summary">                    
                        <a class="govuk-link" href ="/school/detail?urn=${info.Id}">${info.Name}</a>
                        <p>${info.Address}</p>
                        <p>${info.OverallPhase}</p>
                        <p>${info.NFType}</p>`;

                marker.bindPopup(html);
                marker.on('click', function (ev) {
                    ev.target.options.icon.options.iconUrl = "/public/assets/images/icons/icon-location-pink.png";
                    ev.target.refreshIconOptions();
                });
                marker.on('popupclose', function (ev) {
                    ev.target.options.icon.options.iconUrl = "/public/assets/images/icons/icon-location.png";
                    ev.target.refreshIconOptions();
                });
            }

            this.topLayer = markers;

            this.azureMap.addLayer(markers);

            let opt = { padding: [50, 50] };
            this.azureMap.fitBounds(L.latLngBounds(latLangs), opt);
        }
    };

    GOVUK.AzureSchoolLocationsMap = AzureSchoolLocationsMap;

}(jQuery));
