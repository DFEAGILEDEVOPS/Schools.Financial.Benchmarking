"use strict";

class HistoricalCharts {

    generateCharts(unitParameter) {
        $(".chart").each(
            (i, el) => {
                let yValues = JSON.parse($('#' + el.id).attr('data-chart'));
                let minBy = _.minBy(yValues, (o) => o.amount );
                let minimum = minBy ? minBy.amount : 0;
                let maxBy = _.maxBy(yValues, o => o.amount);
                let maximum = maxBy ? maxBy.amount : 0;
                if (minimum === 0 && maximum === 0) {
                    this.generateChart(el, null, 0, 0, 0, 0);
                } else if (minimum === maximum) {
                    let middle;
                    if (minimum >= 0) {
                        middle = this.roundedTickRange(0, maximum);
                        this.generateChart(el, unitParameter, 0, middle, middle, middle * 2);
                    } else {
                        middle = this.roundedTickRange(minimum, 0);
                        this.generateChart(el, unitParameter, -2 * middle, -1 * middle, -1 * middle, 0);
                    }
                } else {
                    let range = this.roundedTickRange(minimum, maximum);
                    let newMin = range * Math.floor(minimum / range);
                    let newMax = range * Math.ceil(maximum / range);
                    this.generateChart(el, unitParameter, newMin, newMin + range, newMin + range + range, newMax);
                }
            }
        );
    }

    roundedTickRange(min, max) {
        let range = max - min;
        let tickCount = 3;
        let unroundedTickSize = range / (tickCount - 1);
        let x = Math.ceil(Math.log10(unroundedTickSize) - 1);
        let pow10x = Math.pow(10, x);
        let roundedTickRange = Math.ceil(unroundedTickSize / pow10x) * pow10x;
        return roundedTickRange;
    }

    generateChart(el, showValue, min, mid, mid2, max) {
        showValue = showValue || "AbsoluteMoney";
        let axisLabel = $('#' + el.id).attr('data-axis-label');
        let yAxis;
        let isMobile = $(window).width() <= 640;

        let applyChartStyles = function (el) {
            if (isMobile) {
                let texts = $("#" + el.id + " .c3-axis-x g.tick text tspan");
                jQuery.each(texts, (index, element) => {
                    $(element).text($(element).text().replace('201', '1'));
                    $(element).text($(element).text().replace('202', '2'));
                });                
            }
        };

        switch (showValue) {
            case "PerPupil":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
            case "AbsoluteMoney":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
            case "PerTeacher":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
            case "PercentageOfTotalExpenditure":
            case "PercentageOfTotalIncome":
            case "FTERatioToTotalFTE":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartPercentageFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
            case "NoOfPupilsPerMeasure":
            case "HeadcountPerFTE":
            case "AbsoluteCount":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartDecimalFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
        }

        c3.generate({
            bindto: '#' + el.id,
            size: {
                height: 175,
                width: (window.innerWidth > 768) ? 710 : !isMobile ? 523 : null
            },
            data: {
                json: JSON.parse($('#' + el.id).attr('data-chart')),
                keys: {
                    x: 'year',
                    value: ['amount']
                },
                labels: {
                    format: (v) => { return v === null ? "No data" : ""; }
                }
            },
            axis: {
                y: yAxis,
                x: {
                    type: 'category', // this needed to load string x value
                    tick: {
                        centered: true,
                        culling: {
                            max: isMobile ? 3 : 6
                        }
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
                    }
                }
            },
            legend: {
                show: false
            },
            color: {
                pattern: ['#28A197']
            },
            tooltip: {
                contents: (d, defaultTitleFormat, defaultValueFormat) => {
                    return defaultValueFormat(d[0].value);
                }
            },
            padding: {
                bottom: 10
            },
            onrendered: () => {
                applyChartStyles(el);                
            }
        });
    }

    selectGrouping(grouping, parentGrouping) {
        $("#ChartGroup").val(grouping);
        $("#ChartGroup").change();
        if ($("#financialSummary")[0]) {
            $("#financialSummary")[0].scrollIntoView();
        }
        $("#ChartGroup").focus();
        $(".back-to-main-chart-group-button .js-parent-group").text(parentGrouping);
        $(".back-to-main-chart-group-button").show();
    }

    resetGrouping() {
        $('#ChartGroup').prop('selectedIndex', 0);
        $("#ChartGroup").change();
        $(".back-to-main-chart-group-button").hide();
    }

    setActiveTab() {
        $(".govuk-tabs__list-item").removeClass("govuk-tabs__list-item--selected");
        let tab = DfE.Util.QueryString.get('tab');
        switch (tab) {
            case "Expenditure":
            default:
                $("#ExpenditureTab").addClass("govuk-tabs__list-item--selected");
                break;
            case "Income":
                $("#IncomeTab").addClass("govuk-tabs__list-item--selected");
                break;
            case "Balance":
                $("#BalanceTab").addClass("govuk-tabs__list-item--selected");
                break;
            case "Workforce":
                $("#WorkforceTab").addClass("govuk-tabs__list-item--selected");
                break;
        }
    }

    rebuildCharts(establishment) {
        let codeParameter = DfE.Util.QueryString.get('code');
        let urnParameter = DfE.Util.QueryString.get('urn');
        let companyNoParameter = DfE.Util.QueryString.get('companyNo');
        let fuid = DfE.Util.QueryString.get('fuid');
        let nameParameter = DfE.Util.QueryString.get('name');
        let tabParameter = DfE.Util.QueryString.get('tab') || "Expenditure";
        let unitParameter = $("select[name='ShowValue']").val();
        let chartGroupParameter = $("#ChartGroup").val();
        let financingParameter = $("#Financing").val();
        let formatParameter = sessionStorage.chartFormat;

        let url = "/" +
            establishment +
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
                this.generateCharts(unitParameter);
                this.updateTotals();
                this.updateTrustWarnings();
                GOVUKFrontend.initAll({ scope: $(".historical-charts-list")[0]});
                this.setActiveTab();
            }
        });
    }

    updateTrustWarnings() {
        let isPlaceholder = $("#isPlaceholder").val();
        if (isPlaceholder === "true") {
            $("#placeholderWarning").show();
        } else {
            $("#placeholderWarning").hide();
        }
    }

    updateTotals() {
        let expTotal = $("#expTotal").val();
        let expTotalAbbr = $("#expTotalAbbr").val();
        let incTotal = $("#incTotal").val();
        let incTotalAbbr = $("#incTotalAbbr").val();
        let balTotal = $("#balTotal").val();
        let balTotalAbbr = $("#balTotalAbbr").val();

        $(".exp-total").text(expTotal);
        $("abbr.exp-total").attr("title", expTotalAbbr);
        $(".inc-total").text(incTotal);
        $("abbr.inc-total").attr("title", incTotalAbbr);
        $(".bal-total").text(balTotal);
        $("abbr.bal-total").attr("title", balTotalAbbr);
        if (balTotalAbbr.includes("-")) {
            $("abbr.bal-total").addClass("negative-balance");
            $("span.bal-total").parent().addClass("negative-balance");
        } else {
            $("abbr.bal-total").removeClass("negative-balance");
            $("span.bal-total").parent().removeClass("negative-balance");
        }
    }

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

