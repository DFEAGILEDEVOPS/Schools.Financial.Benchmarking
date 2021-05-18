class FederationDetailsViewModel {
    constructor(chartFormat, unitType, mapApiKey) {
        this.initControls(chartFormat, unitType);
        this.initMaps(mapApiKey);
    }

    initControls(chartFormat, unitType) {
        $.get("/school/GetBenchmarkBasket",
            (data) => {
                $("#benchmarkBasket").replaceWith(data);
            });

        sessionStorage.chartFormat = chartFormat;

        DfE.Views.HistoricalCharts = new HistoricalCharts();
        DfE.Views.HistoricalCharts.generateCharts(unitType);

        GOVUK.Modal.Load();

        new Accordion(document.getElementById('schools-in-federation-accordion'));
        new Accordion(document.getElementById('controls-accordion'));

        $(document).ready(function () {
            setTimeout(function () {
                var tab = DfE.Util.QueryString.get('tab');
                if (tab) {
                    $("a:contains('" + tab + "')").focus();
                }
            }, 500);
        });
    }

    initMaps(mapApiKey) {
        let location = { lat: 52.636, lng: -1.139 }; // no location specified, so use central England.                                    

        var options = {
            elementId: "SchoolLocationMap",
            primaryMarker: {
                geometry: {
                    location: {
                        lat: location.lat,
                        lng: location.lng
                    }
                }
            },
            mapApiKey: mapApiKey,
            fullScreen: true
        };

        this.map = new GOVUK.AzureSchoolLocationsMap(options);
        let fuid = DfE.Util.QueryString.get('fuid');

        $.ajax({
            url: `/federation/getmapdata?fuid=${fuid}`
        }).done((response) => {            
            this.map.renderFederatonSchoolPinsForAzureMap(response);
        }).error(function (error) {
            console.log("Error loading map pins: " + error);
        });
    }

    downloadData(fuid) {
        $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText' role='alert' aria-live='assertive'> Downloading<span aria-hidden='true'>...</span></span>");
        document.getElementById('download_iframe').src = `/federation/download?fuid=${fuid}`;
        setTimeout(() => {
            $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText'> Download data for this federation<span class='visually-hidden'> (CSV)</span></span>");
        }, 2000)
    }

    printPage() {
        $('details').attr('open', 'true');
        let detailses = document.getElementsByTagName("details"),
            details,
            i = -1;
        while (details = detailses[++i]) {
            //DOM API
            details["open"] = true;
        }
        window.print();
    }

    tabChange(urn, tab) {
        let queryString = `?fuid=${urn}&tab=${tab}`;

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

    tabKeydown(e) {
        let keys = {
            left: 37,
            up: 38,
            right: 39,
            down: 40,
            enter: 13,
            space: 32
        };

        focusOnPreviousTab = function () {
            $("ul[role='tablist'] li a:focus").parent().prev().find("a[role='tab']").focus();
        }

        focusOnNextTab = function () {
            $("ul[role='tablist'] li a:focus").parent().next().find("a[role='tab']").focus();
        }

        activateTab = function () {
            $("ul[role='tablist'] li a:focus").attr('aria-selected', 'true');
   
            setTimeout(function () {
                $("ul[role='tablist'] li a:focus").click();
            }, 1000);
        }

        switch (e.keyCode) {
            case keys.left:
            case keys.up:
                focusOnPreviousTab();
                e.preventDefault();
                break
            case keys.right:
            case keys.down:
                focusOnNextTab();
                e.preventDefault();
                break
            case keys.enter:
            case keys.space:
                activateTab();
                e.preventDefault();
                break;
        }
    };

    toggleChartsTables(mode) {
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