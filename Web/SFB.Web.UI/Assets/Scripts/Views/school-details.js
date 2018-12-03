(function (GOVUK, Views) {
    'use strict';

    function SchoolDetailsViewModel(modelId, modelLat, modelLng, modelHasCoordinates, chartFormat, unitType) {
        this.initControls(modelId, chartFormat, unitType);
        this.initMaps(modelId, modelLat, modelLng, modelHasCoordinates);
    }

    SchoolDetailsViewModel.prototype = {

        initControls: function (modelId, chartFormat, unitType) {
            $.get("/school/GetBenchmarkBasket",
                function (data) {
                    $("#benchmarkBasket").replaceWith(data);
                });

            if ($("#benchmarkControlsPlaceHolder").length > 0) {
                $.get("/school/GetBenchmarkControls?urn=" + modelId,
                    function (data) {
                        $("#benchmarkControlsPlaceHolder").html(data);
                        GOVUK.Modal.Load();
                        $('.sticky-div').Stickyfill();
                    });
            }

            sessionStorage.chartFormat = chartFormat;
            DfE.Views.HistoricalCharts.GenerateCharts(unitType);

            new Accordion(document.getElementById('historical-charts-accordion'));
        },

        initMaps: function (modelId, modelLat, modelLng, modelHasCoordinates) {
            var school = {
                id: modelId,
                lat: modelLat,
                lng: modelLng,
                hasMap: modelHasCoordinates ? "true" : "false"
            };

            var options = {
                elementId: "SchoolLocationMap",
                hasMap: school.hasMap,
                primaryMarker: {
                    geometry: {
                        location: {
                            lat: school.lat,
                            lng: school.lng
                        }
                    }
                },
                scrollWheel: false
            };

            setTimeout(function () {
                if ($(window).width() <= 640) {
                    $('details').removeAttr('open');
                }

                if (!this.mapLoaded) {
                    this.map = new GOVUK.LocationMap(options);
                    this.mapLoaded = true;
                }

                var tab = DfE.Util.QueryString.get('tab');
                if (tab) {
                    $("a:contains('" + tab + "')").focus();
                }
            },
                500);
        }
    };

    SchoolDetailsViewModel.Load = function (modelId, modelLat, modelLng, modelHasCoordinates, chartFormat, unitType) {
        new DfE.Views.SchoolDetailsViewModel(modelId, modelLat, modelLng, modelHasCoordinates, chartFormat, unitType);
    };

    SchoolDetailsViewModel.PrintPage = function () {
        $('details').attr('open', 'true');
        var detailses = document.getElementsByTagName("details"),
            details,
            i = -1;
        while (details = detailses[++i]) {
            //DOM API
            details["open"] = true;
        }

        window.print();
    };

    SchoolDetailsViewModel.TabChange = function (urn, tab) {
        var queryString = "?urn=" +
            urn +
            "&tab=" +
            tab;

        if (DfE.Util.QueryString.get('tab') !== "Workforce") {
            queryString += "&unit=" + $("select#ShowValue option:selected")[0].value;
        }

        if ($("select#Financing option:selected").length > 0) {
            queryString += "&financing=" + $("select#Financing option:selected")[0].value;
        }

        if (sessionStorage.chartFormat) {
            queryString += "&format=" + sessionStorage.chartFormat;
        }

        window.location = queryString;
    };

    Views.SchoolDetailsViewModel = SchoolDetailsViewModel;

}(GOVUK, DfE.Views));