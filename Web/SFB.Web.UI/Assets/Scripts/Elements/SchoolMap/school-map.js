(function ($) {
    "use strict";
    window.GOVUK = window.GOVUK || {};

    function LocationMap(options) {
        if (!options.elementId || options.elementId == "")
            throw new ReferenceError("options.elementId is not present.");

        this.mapElementId = options.elementId;
        if (!this.mapElement)
            throw new ReferenceError("mapElement did not resolve using the id: " + options.elementId);

        if (!options.primaryMarker)
            throw new ReferenceError("options.primaryMarker is not present.");
        this.primaryMarker = options.primaryMarker;
        this.hasMap = options.hasMap;
        this.defaultZoom = 15;
        this.initialize();
        //setTimeout(function () {
        //        $("a:contains('Terms of Use')").attr("tabindex", "-1");
        //        $("a:contains('Report a map error')").attr("tabindex", "-1");
        //    },
        //    1500);
    }

    // static constants
    LocationMap.EarthRadii = {
        miles: 3963.1676,
        km: 6378.1
    };

    LocationMap.prototype = {

        get mapElement() {
            return document.getElementById(this.mapElementId)
        },

        initialize: function () {
            if (this.hasMap) {
                this.centreLatLng = new google.maps.LatLng(this.primaryMarker.geometry.location.lat, this.primaryMarker.geometry.location.lng);
                this.setMapOptions();
                this.addMarker(this.primaryMarker);
            }
        },

        setMapOptions: function () {
            var mapOptions = {
                center: this.centreLatLng,
                zoom: this.defaultZoom,
                streetViewControl: false,
                scrollwheel: false,
                disableDefaultUI: true,
                mapTypeControl: true,
                mapTypeControlOptions: {
                    position: google.maps.ControlPosition.TOP_RIGHT
                },
                fullscreenControl: true,
                fullscreenControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM
                },
                zoomControl: true,
                zoomControlOptions: {
                    position: google.maps.ControlPosition.TOP_LEFT
                }
            };
            this.map = new google.maps.Map(
              this.mapElement,
              mapOptions
            );
        },

        triggerResize: function () {
            google.maps.event.trigger(this.map, 'resize');
        },

        addMarker: function (markerDetail) {
            var markerPosition = new google.maps.LatLng(markerDetail.geometry.location.lat, markerDetail.geometry.location.lng);
            var markerInfo = {
                title: markerDetail.title,
                position: markerPosition,
                status: 'active',
                map: this.map
                //icon: this.markerImageInfo
                //shape: shape
            };
            var marker = new google.maps.Marker(markerInfo);
        }

    };

    GOVUK.LocationMap = LocationMap;

}(jQuery));