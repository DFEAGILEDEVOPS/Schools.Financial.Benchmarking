class SchoolDetailsViewModel {

    constructor(modelId, modelLat, modelLng, modelHasCoordinates, chartFormat, unitType, mapApiKey) {
        this.initControls(modelId, chartFormat, unitType);
        this.initMaps(modelLat, modelLng, modelHasCoordinates, mapApiKey);
    }

    initControls(modelId, chartFormat, unitType) {
        $.get("/school/GetBenchmarkBasket",
            (data) => {
                $("#benchmarkBasket").replaceWith(data);
            });

        if ($("#benchmarkControlsPlaceHolder").length > 0) {
            $.get("/school/GetBenchmarkControls?urn=" + modelId,
                (data) => {
                    $("#benchmarkControlsPlaceHolder").html(data);
                    GOVUK.Modal.Load();
                });
        }

        var sptUrl = `https://www.compare-school-performance.service.gov.uk/estab-details/${modelId}`;
        $.ajax({
            type: 'HEAD',
            url: sptUrl,
            success: function () { $(".spt_link_js").show(); },
            error: function (xhr, e) {
                $(".spt_link_js").hide();
            }
        });

        sessionStorage.chartFormat = chartFormat;

        DfE.Views.HistoricalCharts = new HistoricalCharts();
        DfE.Views.HistoricalCharts.GenerateCharts(unitType);

        new Accordion(document.getElementById('historical-charts-accordion'));
    }

    initMaps(modelLat, modelLng, modelHasCoordinates, mapApiKey) {
        let school = {
            lat: modelLat,
            lng: modelLng,
            hasMap: modelHasCoordinates ? "true" : "false"
        };

        let options = {
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
            scrollWheel: false,
            mapApiKey: mapApiKey,
            fullScreen: true
        };

        setTimeout(function () {
            if ($(window).width() <= 640) {
                $('details').removeAttr('open');
            }

            if (!this.mapLoaded) {
                this.map = new GOVUK.AzureLocationMap(options);
                this.mapLoaded = true;
            }

            let tab = DfE.Util.QueryString.get('tab');
            if (tab) {
                $("a:contains('" + tab + "')").focus();
            }
        }, 500);
    }

    PrintPage() {
        $('details').attr('open', 'true');
        let detailses = document.getElementsByTagName("details");
        let details;
        let i = -1;
        while (details = detailses[++i]) {
            //DOM API
            details["open"] = true;
        }

        window.print();
    }

    TabChange(urn, tab) {
        let queryString = `?urn=${urn}&tab=${tab}`;

        if (DfE.Util.QueryString.get('tab') !== "Workforce") {
            queryString += `&unit=${$("select#ShowValue option:selected")[0].value}`;
        }

        if ($("select#Financing option:selected").length > 0) {
            queryString += `&financing=${$("select#Financing option:selected")[0].value}`;
        }

        if (sessionStorage.chartFormat) {
            queryString += `&format=${sessionStorage.chartFormat}`;
        }

        queryString += '#financialSummary';

        window.location = queryString;
    }

    ToggleChartsTables(mode) {
        let $charts = $('.chart-wrapper');
        let $tables = $('.chart-table-wrapper');
        let $mobile_balance_wrappers = $('.balance-wrapper-mobile');
        let $showChartsButton = $('.view-charts-tables.charts');
        let $showTablesButton = $('.view-charts-tables.tables');
        let $saveAsImagesButtons = $('.save-as-image');
        let $viewMoreLabels = $("a span.view-more");
        if (mode === 'Charts') {
            $showChartsButton.hide();
            $showTablesButton.show();
            $tables.hide();
            $mobile_balance_wrappers.removeClass('balance-wrapper-mobile--inactive');
            $mobile_balance_wrappers.addClass('balance-wrapper-mobile--active');
            $charts.css('display', 'block');
            $saveAsImagesButtons.show();
            $viewMoreLabels.text("View more charts");
            sessionStorage.chartFormat = 'Charts';
        } else if (mode === 'Tables') {
            $showTablesButton.hide();
            $showChartsButton.show();
            $charts.hide();
            $tables.show();
            $mobile_balance_wrappers.removeClass('balance-wrapper-mobile--active');
            $mobile_balance_wrappers.addClass('balance-wrapper-mobile--inactive');
            $saveAsImagesButtons.hide();
            $viewMoreLabels.text("View more tables");
            sessionStorage.chartFormat = 'Tables';
        }
    }
}

