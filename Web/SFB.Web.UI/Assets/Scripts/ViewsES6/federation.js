"use strict";

class FederationViewModel {
    constructor(chartFormat, unitType, mapApiKey) {
        DfE.Views.BenchmarkCharts = new BenchmarkChartManager();
        DfE.Views.HistoricalCharts = new HistoricalChartManager();

        this.initControls(chartFormat, unitType);

        if ($(window).width() <= 640) {
            $('details#mapDetails').removeAttr('open');
        } else {
            this.initMaps(mapApiKey);
        }

        $("#detailsTab, #mapDetails").on("click", () => this.initMaps(mapApiKey));
    }

    initControls(chartFormat, unitType) {
        $.get("/school/GetBenchmarkBasket",
            (data) => {
                $("#benchmarkBasket").replaceWith(data);
            });

        sessionStorage.chartFormat = chartFormat;

        DfE.Views.HistoricalCharts.generateFinanceCharts();
        DfE.Views.HistoricalCharts.generateWorkforceCharts();

        this.renderQcChart("Total expenditure");

        GOVUK.Modal.Load();

    }

    tabChange(urn, tab) {
        let queryString = `?fuid=${urn}&tab=${tab}`;

        queryString += `&unit=${$("select[name='ShowValue'] option:selected")[0].value}`;

        if ($("select#Financing option:selected").length > 0) {
            queryString += `&financing=${$("select#Financing option:selected")[0].value}`;
        }

        if (sessionStorage.chartFormat) {
            queryString += `&format=${sessionStorage.chartFormat}`;
        }

        queryString += '#finance';

        window.location = queryString;
    }

    changeQcTab(chartName, event, element) {

        if (event) {
            event.preventDefault();
        }

        $("#QCPanel li.app-navigation__list-item").removeClass("app-navigation__list-item--current");
        $(element).parent().addClass("app-navigation__list-item--current");

        this.renderQcChart(chartName);
    }

    renderQcChart(chartName) {

        let url = "/benchmarkcharts/getchart?chartGroup=TotalExpenditure&chartName=" + encodeURI(chartName);

        let formatParameter = "Charts";//or Tables
        if (formatParameter) {
            url += "&format=" + formatParameter;
        }

        $.ajax({
            url: url,
            datatype: 'json',
            beforeSend: () => {
                DfE.Util.LoadingMessage.display("#benchmarkChartsList", "Loading charts");
            },
            success: (data) => {
                $("#benchmarkChartsList").html(data);

                DfE.Views.BenchmarkCharts.generateCharts();

                //window.GOVUKFrontend.initAll({ scope: $("#benchmarkChartsList")[0] });
                //$("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-only-view").first().find("thead th").length - 1, 1]] });
                //$("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
            }
        });
    }

    initMaps(mapApiKey) {
        setTimeout(function () {
            if ($("#SchoolLocationMap").is(":visible")) {
                if (!this.mapLoaded) {
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
                        this.mapLoaded = true;
                    }).error(function (error) {
                        console.log("Error loading map pins: " + error);
                    });
                }
            }
        }, 500);
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
        let detailses = document.getElementsByTagName("details");
        let i = 0;
        let details = detailses[i];
        while (details) {
            details = detailses[i++];
            if (details) {
                details["open"] = true;
            }
        }
        window.print();
    }

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

    rebuildFinanceCharts() {
        let codeParameter = DfE.Util.QueryString.get('code');
        let urnParameter = DfE.Util.QueryString.get('urn');
        let companyNoParameter = DfE.Util.QueryString.get('companyNo');
        let fuid = DfE.Util.QueryString.get('fuid');
        let nameParameter = DfE.Util.QueryString.get('name');
        let tabParameter = DfE.Util.QueryString.get('tab') || "Expenditure";
        let chartGroupParameter = $("#ChartGroup").val();
        let unitParameter = $(".show-value.js-finance-showValue").val();
        let financingParameter = $("#Financing:visible").val() ?? null;
        let formatParameter = sessionStorage.chartFormat;

        let url = "/federation" +
            "/getcharts?urn=" +
            urnParameter +
            "&code=" +
            codeParameter +
            "&companyNo=" +
            companyNoParameter +
            "&fuid=" +
            fuid +
            "&revgroup=" +
            tabParameter +
            "&chartGroup=" +
            chartGroupParameter +
            "&unit=" +
            unitParameter +
            "&name=" +
            nameParameter +
            "&format=" +
            formatParameter;

        if (financingParameter) {
            url += "&financing=" + financingParameter;
        }

        $.ajax({
            url: url,
            datatype: 'json',
            beforeSend: () => {
                DfE.Util.LoadingMessage.display(".historical-charts-list.finance-charts-list", "Updating charts");
            },
            success: (data) => {
                $(".historical-charts-list.finance-charts-list").html(data);
                DfE.Views.HistoricalCharts.generateFinanceCharts();
                GOVUKFrontend.initAll({ scope: $(".historical-charts-list.finance-charts-list")[0] });
            }
        });
    }

    rebuildWorkforceCharts() {
        let codeParameter = DfE.Util.QueryString.get('code');
        let urnParameter = DfE.Util.QueryString.get('urn');
        let companyNoParameter = DfE.Util.QueryString.get('companyNo');
        let fuid = DfE.Util.QueryString.get('fuid');
        let nameParameter = DfE.Util.QueryString.get('name');
        let tabParameter = "workforce";
        let chartGroupParameter = "workforce";
        let unitParameter = $(".show-value.js-wf-showValue").val();
        let financingParameter = $("#Financing:visible").val() ?? null;
        let formatParameter = sessionStorage.chartFormat;

        let url = "/federation" +
            "/getcharts?urn=" +
            urnParameter +
            "&code=" +
            codeParameter +
            "&companyNo=" +
            companyNoParameter +
            "&fuid=" +
            fuid +
            "&revgroup=" +
            tabParameter +
            "&chartGroup=" +
            chartGroupParameter +
            "&unit=" +
            unitParameter +
            "&name=" +
            nameParameter +
            "&format=" +
            formatParameter;

        if (financingParameter) {
            url += "&financing=" + financingParameter;
        }

        $.ajax({
            url: url,
            datatype: 'json',
            beforeSend: () => {
                DfE.Util.LoadingMessage.display(".historical-charts-list.workforce-charts-list", "Updating charts");
            },
            success: (data) => {
                $(".historical-charts-list.workforce-charts-list").html(data);
                DfE.Views.HistoricalCharts.generateWorkforceCharts();
                GOVUKFrontend.initAll({ scope: $(".historical-charts-list.workforce-charts-list")[0] });
            }
        });
    }

    //updateTotals() {
    //    let expTotal = $("#expTotal").val();
    //    let expTotalAbbr = $("#expTotalAbbr").val();
    //    let incTotal = $("#incTotal").val();
    //    let incTotalAbbr = $("#incTotalAbbr").val();
    //    let balTotal = $("#balTotal").val();
    //    let balTotalAbbr = $("#balTotalAbbr").val();

    //    $(".exp-total").text(expTotal);
    //    $("abbr.exp-total").attr("title", expTotalAbbr);
    //    $(".inc-total").text(incTotal);
    //    $("abbr.inc-total").attr("title", incTotalAbbr);
    //    $(".bal-total").text(balTotal);
    //    $("abbr.bal-total").attr("title", balTotalAbbr);
    //    if (balTotalAbbr.includes("-")) {
    //        $("abbr.bal-total").addClass("negative-balance");
    //        $("span.bal-total").parent().addClass("negative-balance");
    //    } else {
    //        $("abbr.bal-total").removeClass("negative-balance");
    //        $("span.bal-total").parent().removeClass("negative-balance");
    //    }
    //}

    updateBenchmarkBasket(urn, withAction) {
        if (withAction === "Add") {
            if (DfE.Util.ComparisonList.count() === 30) {
                DfE.Util.ComparisonList.renderFullListWarningModal();
                return;
            }
        }

        $.get("/school/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
            (data) => {
                $("#benchmarkBasket").replaceWith(data);
                switch (withAction) {
                    case "UnsetDefault":
                        $(".set-unset-default").toggle();
                        break;
                    case "SetDefault":
                        $(".set-unset-default").toggle();
                        $(".addto").hide();
                        $(".removefrom").show();
                        break;
                    case "Add":
                    case "Remove":
                        $(".add-remove-js").toggle();
                        break;
                }
            });
    }
}