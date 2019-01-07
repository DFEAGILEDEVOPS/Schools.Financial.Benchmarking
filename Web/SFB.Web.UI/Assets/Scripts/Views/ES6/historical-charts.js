(function(GOVUK, Views) {
    'use strict';

    var HistoricalCharts = {
        GenerateCharts: function(unitParameter) {
            var self = this;
            $(".chart").each(
                function () {
                    var yValues = JSON.parse($('#' + this.id).attr('data-chart'));
                    var minBy = _.minBy(yValues, function(o) { return o.amount; });
                    var minimum = minBy ? minBy.amount : 0;
                    var maxBy = _.maxBy(yValues, function(o) { return o.amount; });
                    var maximum = maxBy ? maxBy.amount : 0;
                    if (minimum === 0 && maximum === 0) {
                        self.GenerateChart(this, null, 0, 0, 0, 0);
                    } else if (minimum === maximum) {
                        var middle;
                        if (minimum >= 0) {
                            middle = self.RoundedTickRange(0, maximum);
                            self.GenerateChart(this, unitParameter, 0, middle, middle, middle * 2);
                        } else {
                            middle = self.RoundedTickRange(minimum, 0);
                            self.GenerateChart(this, unitParameter, -2 * middle, -1 * middle, -1 * middle, 0);
                        }
                    } else {
                        var range = self.RoundedTickRange(minimum, maximum);
                        var newMin = range * Math.floor(minimum / range);
                        var newMax = range * Math.ceil(maximum / range);
                        self.GenerateChart(this, unitParameter, newMin, newMin + range, newMin + range + range, newMax);
                    }
                }
            );
        },

        RoundedTickRange: function(min, max) {
            var range = max - min;
            var tickCount = 3;
            var unroundedTickSize = range / (tickCount - 1);
            var x = Math.ceil(Math.log10(unroundedTickSize) - 1);
            var pow10x = Math.pow(10, x);
            var roundedTickRange = Math.ceil(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        },

        GenerateChart: function(el, showValue, min, mid, mid2, max) {
            showValue = showValue || "AbsoluteMoney";
            var axisLabel = $('#' + el.id).attr('data-axis-label');
            var yAxis;
            switch (showValue) {
            case "PerPupil":
                yAxis = {
                    tick: {
                        format: function(d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
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
                        format: function(d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
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
                        format: function(d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: [min, mid, mid2, max],
                        count: 4
                    },
                    min: min,
                    max: max
                };
                break;
            case "PercentageOfTotal":
            case "FTERatioToTotalFTE":
                yAxis = {
                    tick: {
                        format: function(d) { return window.DfE.Util.Charting.ChartPercentageFormat(d); },
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
                        format: function(d) { return window.DfE.Util.Charting.ChartDecimalFormat(d); },
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
                    width: (window.innerWidth > 768) ? 710 : (window.innerWidth > 640)  ?  523 : null
                },
                data: {
                    json: JSON.parse($('#' + el.id).attr('data-chart')),
                    keys: {
                        x: 'year',
                        value: ['amount']
                    },
                    labels: {
                        format: function(v) { return v === null ? "No data" : ""; }
                    }
                },
                axis: {
                    y: yAxis,
                    x: {
                        type: 'category', // this needed to load string x value
                        tick: {
                            centered: true,
                            culling: {
                                max: $(window).width() <= 640 ? 3 : 6
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
                    contents: function(d, defaultTitleFormat, defaultValueFormat) {
                        return defaultValueFormat(d[0].value);
                    }
                },
                padding: {
                    bottom: 10
                }
            });
        },

        SelectGrouping: function (grouping) {
            $("#ChartGroup").val(grouping);
            $("#ChartGroup").change();
            $("#financialSummary")[0].scrollIntoView();
            $("#ChartGroup").focus();
        },

        RebuildCharts: function(establishment) {
            var self = this;
            var codeParameter = DfE.Util.QueryString.get('code');
            var urnParameter = DfE.Util.QueryString.get('urn');
            var matNoParameter = DfE.Util.QueryString.get('matno');
            var nameParameter = DfE.Util.QueryString.get('name');
            var tabParameter = DfE.Util.QueryString.get('tab') || "Expenditure";
            var unitParameter = $("#ShowValue").val();
            var termParameter = $("#Year").val();
            var chartGroupParameter = $("#ChartGroup").val();
            var financingParameter = $("#Financing").val();
            var formatParameter = sessionStorage.chartFormat;

            var url = "/" +
                establishment +
                "/getcharts?urn=" +
                urnParameter +
                "&code=" +
                codeParameter +
                "&matno=" +
                matNoParameter +
                "&revgroup=" +
                tabParameter +
                "&chartGroup=" +
                chartGroupParameter +
                "&unit=" +
                unitParameter +
                "&term=" +
                termParameter +
                "&name=" +
                nameParameter +
                "&format=" +
                formatParameter;;

            if (financingParameter) {
                url += "&financing=" + financingParameter;
            }

            $.ajax({
                url: url,
                datatype: 'json',
                beforeSend: function () {
                    DfE.Util.LoadingMessage.display("#historicalChartsList", "Updating charts");
                },
                success: function(data) {
                    $("#historicalChartsList").html(data);
                    self.GenerateCharts(unitParameter);
                    self.UpdateTotals();
                    self.UpdateTrustWarnings();
                    new Accordion(document.getElementById('historical-charts-accordion'));
                }
            });
        },

        UpdateTrustWarnings: function () {
            var isPlaceholder = $("#isPlaceholder").val();
            if (isPlaceholder == "true")
            {
                $("#placeholderWarning").show();
            } else {
                $("#placeholderWarning").hide();
            }
        },

        UpdateTotals: function() {
            var expTotal = $("#expTotal").val();
            var expTotalAbbr = $("#expTotalAbbr").val();
            var incTotal = $("#incTotal").val();
            var incTotalAbbr = $("#incTotalAbbr").val();
            var balTotal = $("#balTotal").val();
            var balTotalAbbr = $("#balTotalAbbr").val();

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
        },

        UpdateBenchmarkBasket: function(urn, withAction) {
            if (withAction === "Add") {
                if (DfE.Util.ComparisonList.count() === 30) {
                    DfE.Util.ComparisonList.RenderFullListWarningModal();
                    return;
                }
            }

            $.get("/school/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
                function(data) {
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
                        $(".add-remove").toggle();
                        break;
                    }
                });
        }
    };

    Views.HistoricalCharts = HistoricalCharts;

}(GOVUK, DfE.Views));