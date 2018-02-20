(function(GOVUK, Views) {
    'use strict';

    function BenchmarkChartsViewModel() {
        $(document).ready(function () {
            $("table").tablesorter();
            DfE.Views.BenchmarkChartsViewModel.GenerateCharts();
            DfE.Views.BenchmarkChartsViewModel.RefreshAddRemoveLinks();
            $('.save-as-image').show();
        });
    }

    //This function is accessing to the scope of the AngularJS controller to retrieve the chart selections model and update the buttons accordingly in other tabs.
    BenchmarkChartsViewModel.RefreshAddRemoveLinks = function() {
        var showRemoveLink = function (element) {
            $(element).find("a.customRemove").show();
            $(element).find("a.customAdd").hide();
        }

        var showAddLink = function (element) {
            $(element).find("a.customAdd").show();
            $(element).find("a.customRemove").hide();
        }

        var scope = angular.element($("#listCtrl")).scope();
        if (scope) {
            scope.dataLoaded.then(
                function () {
                    $(".customActions").each(function () {
                        var self = this;
                        var fieldName = $(self).attr("data-fn");
                        var showValue = $(self).attr("data-sv");

                        _.forEach(scope.selectionList.HierarchicalCharts,
                            function (group) {
                                var selection = _.find(group.Charts,
                                    function (c) {
                                        return c.FieldName === fieldName;
                                    });

                                if (selection) {
                                    switch (showValue) {
                                        case 'PerPupil':
                                            selection.PerPupilSelected ? showRemoveLink(self) : showAddLink(self);
                                            break;
                                        case 'PerTeacher':
                                            selection.PerTeacherSelected ? showRemoveLink(self) : showAddLink(self);
                                            break;
                                        case 'PercentageOfTotal':
                                            selection.PercentageSelected ? showRemoveLink(self) : showAddLink(self);
                                            break;
                                        case 'AbsoluteMoney':
                                            selection.AbsoluteMoneySelected ? showRemoveLink(self) : showAddLink(self);
                                            break;
                                        case 'AbsoluteCount':
                                            selection.AbsoluteCountSelected ? showRemoveLink(self) : showAddLink(self);
                                            break;
                                        case 'HeadcountPerFTE':
                                            selection.HeadCountPerFTESelected
                                                ? showRemoveLink(self)
                                                : showAddLink(self);
                                            break;
                                        case 'FTERatioToTotalFTE':
                                            selection.PercentageOfWorkforceSelected
                                                ? showRemoveLink(self)
                                                : showAddLink(self);
                                            break;
                                        case 'NoOfPupilsPerMeasure':
                                            selection.NumberOfPupilsPerMeasureSelected
                                                ? showRemoveLink(self)
                                                : showAddLink(self);
                                            break;
                                    }
                                }
                            });
                    });
                });
        }
    };

    //This function is accessing to the scope of the AngularJS controller tab to retrieve and update its chart selections model.
    BenchmarkChartsViewModel.AddRemoveYourCharts = function (fieldName, showValue, checked) {
        var self = this;
        var scope = angular.element($("#listCtrl")).scope();
        scope.$apply(function () {
            _.forEach(scope.selectionList.HierarchicalCharts,
                function (group) {
                    var selection = _.find(group.Charts,
                        function (c) {
                            return c.FieldName === fieldName;
                        });

                    if (selection) {
                        switch (showValue) {
                            case 'PerPupil':
                                selection.PerPupilSelected = checked;
                                break;
                            case 'PerTeacher':
                                selection.PerTeacherSelected = checked;
                                break;
                            case 'PercentageOfTotal':
                                selection.PercentageSelected = checked;
                                break;
                            case 'AbsoluteMoney':
                                selection.AbsoluteMoneySelected = checked;
                                break;
                            case 'AbsoluteCount':
                                selection.AbsoluteCountSelected = checked;
                                break;
                            case 'HeadcountPerFTE':
                                selection.HeadCountPerFTESelected = checked;
                                break;
                            case 'FTERatioToTotalFTE':
                                selection.PercentageOfWorkforceSelected = checked;
                                break;
                            case 'NoOfPupilsPerMeasure':
                                selection.NumberOfPupilsPerMeasureSelected = checked;
                                break;
                        }
                    }
                });
            scope.ctrl.persist();
        });

        self.RefreshAddRemoveLinks();
    };

    BenchmarkChartsViewModel.GenerateChart = function (el, showValue, min, mid, max, barCount) {
        var applyChartStyles = function (el) {
            var benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']", el.parentElement.parentElement.parentElement)[0]
                .value;
            if (benchmarkSchoolIndex > -1) {
                $("#" +
                    el.id +
                    " .c3-shape.c3-shape-" +
                    benchmarkSchoolIndex +
                    ".c3-bar.c3-bar-" +
                    benchmarkSchoolIndex).css("fill", "#D53880");
            }

            var incompleteDataIndex = $("input[name='incompleteDataIndex']", el.parentElement.parentElement.parentElement)[0].value;
            var incompleteDataIndexArray = incompleteDataIndex.split(",");
            if (incompleteDataIndexArray.length > 0) {
                incompleteDataIndexArray.forEach(function (index) {
                    $("#" +
                        el.id +
                        " .c3-shape.c3-shape-" +
                        index +
                        ".c3-bar.c3-bar-" +
                        index).css("fill", "#F47738");
                });
            }
        };

        var addSchoolLinks = function (id) {
            var texts = $("#" + id + " .c3-axis-x g.tick text tspan");

            texts.each(function () {
                var textParts = $(this).text().split("#");

                var type = $("#Type").val();

                if (type === "MAT") {
                    $(this).on('click',
                        function (e, i) {
                            window.open("/trust/index?matno=" + textParts[1] + "&name=" + textParts[0], '_self');
                        });
                } else {
                    $(this).on('click',
                        function (e, i) {
                            window.open("/school/detail?urn=" + textParts[1], '_self');
                        });
                }
                var limit = 36;
                var text = textParts[0].length < limit
                    ? textParts[0]
                    : textParts[0].substring(0, limit - 3) + "...";
                $(this).text(text);

            });
        };

        showValue = showValue || "AbsoluteMoney";
        var paddingBottom = 0;
        var yAxis, yFormat;
        switch (showValue) {
            case "AbsoluteCount":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartIntegerFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    }
                };
                yFormat = function (d) { return window.DfE.Util.Charting.ChartIntegerFormat(d); }
                break;
            case "AbsoluteMoney":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    }
                };
                yFormat = function (d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); };
                break;
            case "PerPupil":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    }
                };
                yFormat = function (d) {
                    if (d === null) {
                        return "No pupil data";
                    } else {
                        return window.DfE.Util.Charting.ChartMoneyFormat(d);
                    }
                };
                break;
            case "PerTeacher":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    }
                };
                yFormat = function (d) {
                    if (d === null) {
                        return "No teacher data";
                    } else {
                        return window.DfE.Util.Charting.ChartMoneyFormat(d);
                    }
                };
                break;
            case "PercentageOfTotal":
            case "FTERatioToTotalFTE":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartPercentageFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom,
                        top: 50
                    }
                };
                yFormat = function (d) { return window.DfE.Util.Charting.ChartPercentageFormat(d); };
                break;
            case "NoOfPupilsPerMeasure":
            case "HeadcountPerFTE":
                yAxis = {
                    tick: {
                        format: function (d) { return window.DfE.Util.Charting.ChartDecimalFormat(d); },
                        values: function () { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    }
                };
                yFormat = function (d) { return window.DfE.Util.Charting.ChartDecimalFormat(d); };
                break;
        }

        c3.generate({
            bindto: '#' + el.id,
            data: {
                json: JSON.parse($('#' + el.id).attr('data-chart')),
                keys: {
                    x: 'school',
                    value: ['amount']
                },
                type: 'bar',
                labels: {
                    format: yFormat
                }
            },
            size: {
                height: (barCount + 1) * 30
            },
            bar: {
                width: 20
            },
            axis: {
                y: yAxis,
                x: {
                    type: 'category', // this needed to load string x value
                    tick: {
                        centered: true,
                        multiline: false
                    }
                },
                rotated: true
            },
            legend: {
                show: false
            },
            color: {
                pattern: ['#097F96']
            },
            tooltip: {
                show: false
            },
            onrendered: function () {
                applyChartStyles(el);
                addSchoolLinks(el.id);
            }
        });
    };

    BenchmarkChartsViewModel.GenerateCharts = function (unitParameter) {
        var self = this;
        var RoundedTickRange = function (min, max) {
            var range = max - min;
            var tickCount = 3;
            var unroundedTickSize = range / (tickCount - 1);
            var x = Math.ceil(Math.log10(unroundedTickSize) - 1);
            var pow10x = Math.pow(10, x);
            var roundedTickRange = Math.ceil(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        };

        $(".chart").each(function () {
            var yValues = JSON.parse($('#' + this.id).attr('data-chart'));
            var unit = unitParameter ? unitParameter : yValues[0].unit;
            var minBy = _.minBy(yValues, function (o) { return o.amount; });
            var minimum = minBy ? minBy.amount : 0;
            var maxBy = _.maxBy(yValues, function (o) { return o.amount; });
            var maximum = maxBy ? maxBy.amount : 0;
            if (minimum === 0 && maximum === 0) {
                self.GenerateChart(this, unit, 0, 0, 0, yValues.length);
            } else if (minimum === maximum) {
                self.GenerateChart(this, unit, minimum, minimum, minimum, yValues.length);
            } else {
                if (minimum > 0) {
                    minimum = 0;
                }
                var range = RoundedTickRange(minimum, maximum);
                var newMin = (minimum < 0)
                    ? (range * Math.floor(minimum / range))
                    : (range * Math.round(minimum / range));
                var newMax = range * Math.ceil(maximum / range);
                self.GenerateChart(this, unit, newMin, newMin + range, newMax, yValues.length);
            }
        });

        new Accordion(document.getElementById('bm-charts-accordion'));
    };

    BenchmarkChartsViewModel.RebuildCharts = function () {
        var self = this;
        var tabParameter = $("#tabSelection").val();
        var chartGroupParameter = $("#ChartGroup").val();
        var unitParameter = $("#ShowValue").val();
        var centralFinancing = $("#CentralFinancing").val();
        var trustCentralFinancing = $("#TrustCentralFinancing").val();
        var type = $("#Type").val();

        var url = "/benchmarkcharts/getcharts?revgroup=" +
            tabParameter +
            "&showValue=" +
            unitParameter +
            "&chartGroup=" +
            chartGroupParameter;

        if (centralFinancing) {
            url += "&centralFinancing=" + centralFinancing;
        }

        if (trustCentralFinancing) {
            url += "&trustCentralFinancing=" + trustCentralFinancing;
        }

        if (type) {
            url += "&type=" + type;
        }

        $.get(url,
            function(data) {
                $("#benchmarkChartsList").html(data);
                self.RefreshAddRemoveLinks();
                $('.save-as-image').show();
                self.GenerateCharts(unitParameter);
                $("table").tablesorter();
            });
    };

    BenchmarkChartsViewModel.saveAsImage = function(name, id) {
        var svg = $('#' + id).find('svg')[0];
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' });
    };

    BenchmarkChartsViewModel.PrintPage = function() {
        $('details').attr('open', 'true');
        window.print();
    };

    BenchmarkChartsViewModel.ChangeTab = function (tab) {
        var self = this;
        if (tab === "Custom") {
            $(".tabs li").removeClass("active");
            $(".tabs li#" + tab).addClass("active");
            $("#tabsSection").empty('');
            $("#customTabSection").show();
        } else {
            var unitParameter = $("#ShowValue").val();
            var financingParameter = $("#CentralFinancing").val();
            unitParameter = unitParameter ? unitParameter : "AbsoluteMoney";
            var typeParameter = $("#Type").val();
            $.get("/benchmarkcharts/tabchange?tab=" + tab +
                "&type=" + typeParameter +
                "&showValue=" + unitParameter +
                "&financing=" + financingParameter,
                function (data) {
                    $(".tabs li").removeClass("active");
                    $(".tabs li#" + tab).addClass("active");
                    $("#customTabSection").hide();
                    $("#tabsSection").html(data);
                    $("table").tablesorter();
                    var unitParameter = $("#ShowValue").val();
                    self.RefreshAddRemoveLinks();
                    $('.save-as-image').show();
                    self.GenerateCharts(unitParameter);
                });
        }
    };

    BenchmarkChartsViewModel.Load = function () {
        new DfE.Views.BenchmarkChartsViewModel();
    };

    Views.BenchmarkChartsViewModel = BenchmarkChartsViewModel;
}(GOVUK, DfE.Views));