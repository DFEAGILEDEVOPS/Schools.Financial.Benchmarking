class SchoolViewModel {

    constructor(modelId, modelLat, modelLng, modelHasCoordinates, chartFormat, unitType, mapApiKey) {

        this.initControls(modelId, chartFormat, unitType);

        if ($(window).width() <= 640) {
            $('details#mapDetails').removeAttr('open');
        } else {
            this.initMaps(modelLat, modelLng, modelHasCoordinates, mapApiKey);
        }

        $("#detailsTab, #mapDetails").on("click", () => this.initMaps(modelLat, modelLng, modelHasCoordinates, mapApiKey));        
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

        DfE.Views.HistoricalCharts = new HistoricalChartManager();
        DfE.Views.HistoricalCharts.generateCharts(unitType);

        if ($(window).width() <= 640) {
            $('#school-website').text('website');
        }
    }

    initMaps(modelLat, modelLng, modelHasCoordinates, mapApiKey) {
        setTimeout(function () {
            if ($("#SchoolLocationMap").is(":visible")) {
                if (!this.mapLoaded) {
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
                    this.map = new GOVUK.AzureLocationMap(options);
                    this.mapLoaded = true;
                }
            }
        }, 500);
    }

    downloadData(urn) {
        $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText' role='alert' aria-live='assertive'> Downloading<span aria-hidden='true'>...</span></span>");
        document.getElementById('download_iframe').src = '/school/download?urn=' + urn;
        setTimeout(() => {
            $("#DownloadLinkTextWrapper").html("<span id='DownloadLinkText'> Download data for this school<span class='visually-hidden'> (CSV)</span></span>");
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

    tabChange(urn, tab) {
        debugger;
        let queryString = `?urn=${urn}&tab=${tab}`;

        queryString += `&unit=${$("select#ShowValue option:selected")[0].value}`;

        if ($("select#Financing option:selected").length > 0) {
            queryString += `&financing=${$("select#Financing option:selected")[0].value}`;
        }

        if (sessionStorage.chartFormat) {
            queryString += `&format=${sessionStorage.chartFormat}`;
        }

        if (tab.toLowerCase() === "workforce") {
            queryString += '#workforce';
        } else {
            queryString += '#finance';
        }        

        window.location = queryString;
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

    rebuildCharts() {
        debugger;
        let codeParameter = DfE.Util.QueryString.get('code');
        let urnParameter = DfE.Util.QueryString.get('urn');
        let companyNoParameter = DfE.Util.QueryString.get('companyNo');
        let fuid = DfE.Util.QueryString.get('fuid');
        let nameParameter = DfE.Util.QueryString.get('name');
        let tabParameter = DfE.Util.QueryString.get('tab') || "Expenditure";
        let unitParameter = $("#ShowValue").val();
        let chartGroupParameter = $("#ChartGroup").val();
        let financingParameter = $("#Financing").val();
        let formatParameter = sessionStorage.chartFormat;

        let url = "/school" +            
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
                DfE.Util.LoadingMessage.display(".historical-charts-list", "Updating charts");
            },
            success: (data) => {
                $(".historical-charts-list").html(data);
                DfE.Views.HistoricalCharts.generateCharts(unitParameter);
                //this.updateTotals();
                GOVUKFrontend.initAll({ scope: $(".historical-charts-list")[0] });
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

