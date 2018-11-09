(function(GOVUK, Views) {
    'use strict';

    function BenchmarkChartsViewModel() {
        sessionStorage.chartFormat = 'Charts';
        $(document).ready(function () {
            $("table.dataTable").tablesorter();
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
                        var chartName = $(self).attr("data-fn");
                        var showValue = $(self).attr("data-sv");

                        _.forEach(scope.selectionList.HierarchicalCharts,
                            function (group) {
                                var selection = _.find(group.Charts,
                                    function (c) {
                                        return c.Name === chartName;
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
    BenchmarkChartsViewModel.AddRemoveYourCharts = function (chartName, showValue, checked, element) {
        var self = this;
        var scope = angular.element($("#listCtrl")).scope();
        scope.$apply(function () {
            _.forEach(scope.selectionList.HierarchicalCharts,
                function (group) {
                    var selection = _.find(group.Charts,
                        function (c) {
                            return c.Name === chartName;
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

        if (checked) {
            $(element).parents(".your-chart-controls").find(".view-your-charts").show();
            $(".custom").addClass("bold");
        } else
        {
            $(element).parents(".your-chart-controls").find(".view-your-charts").hide();
        }


        self.RefreshAddRemoveLinks();
    };

    BenchmarkChartsViewModel.GenerateChart = function (el, showValue, min, mid, max, barCount) {
        var applyChartStyles = function (el) {
            var benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']", $(el).closest('.chartContainer'))[0]
                .value;
            if (benchmarkSchoolIndex > -1) {
                $("#" +
                    el.id +
                    " .c3-shape.c3-shape-" +
                    benchmarkSchoolIndex +
                    ".c3-bar.c3-bar-" +
                    benchmarkSchoolIndex).css("fill", "#D53880");
            }

            var incompleteFinanceDataIndex = $("input[name='incompleteFinanceDataIndex']", $(el).closest('.chartContainer'))[0].value;
            var incompleteFinanceDataIndexArray = incompleteFinanceDataIndex.split(",");
            if (incompleteFinanceDataIndexArray.length > 0) {
                incompleteFinanceDataIndexArray.forEach(function (index) {
                    $("#" +
                        el.id +
                        " .c3-shape.c3-shape-" +
                        index +
                        ".c3-bar.c3-bar-" +
                        index).css("fill", "#F47738");
                });
            }

            var incompleteWorkforceDataIndex = $("input[name='incompleteWorkforceDataIndex']", $(el).closest('.chartContainer'))[0].value;
            var incompleteWorkforceDataIndexArray = incompleteWorkforceDataIndex.split(",");
            if (incompleteWorkforceDataIndexArray.length > 0) {
                incompleteWorkforceDataIndexArray.forEach(function (index) {
                    $("#" +
                        el.id +
                        " .c3-shape.c3-shape-" +
                        index +
                        ".c3-bar.c3-bar-" +
                        index).css("fill", "#F47738");
                });
            }
        };

        var restructureSchoolNames = function (id) {
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
                            dataLayer.push({ 'event': 'bmc_school_link_click' });
                            window.open("/school/detail?urn=" + textParts[1], '_self');
                        });
                }
                var limit = 36;
                var text = textParts[0].length < limit
                    ? textParts[0]
                    : textParts[0].substring(0, limit - 3) + "...";
                $(this).text(text);

                if (textParts[0] === $("#HomeSchoolName").val())
                {
                    $(this).css("font-weight", "bold");
                }
            });
        };

        showValue = showValue || "AbsoluteMoney";
        var paddingBottom = min < 0 ? 100 : 0;
        var axisLabel = $('#' + el.id).attr('data-axis-label');
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                    },
                    label: {
                        text: axisLabel,
                        position: 'outer-center'
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
                contents: function (d, defaultTitleFormat) {
                    var nameAndUrn = defaultTitleFormat(d[0].index).split('#');
                    var name = nameAndUrn[0];
                    var chartData = JSON.parse($('#' + el.id).attr('data-chart'));
                    var schoolData = chartData[d[0].index];
                    var benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']",
                        $(el).closest('.chartContainer'))[0].value;
                    var highlight = benchmarkSchoolIndex === d[0].index.toString() ? "highlighted" : "";
                    var tableHtml =
                        "<table class='bmc-rollover-table' >" +
                        "<tr><th colspan='2' class='" + highlight + "'>" + name + "</th></tr>" +
                        "<tr><td class='bold'>Local authority</td><td>" + schoolData.la + "</td></tr>" +
                        "<tr><td class='bold'>School type</td><td>" + schoolData.type + "</td></tr>" +
                        "<tr><td class='bold'>Number of pupils</td><td>" + schoolData.pupilCount + "</td></tr>";

                    if ($("#ComparisonType").val() == "BestInBreed"){
                        tableHtml += "<tr><td class='bold'>Efficiency metric rank</td><td>" + schoolData.efficiencyRank + "</td></tr>";
                    }

                   tableHtml += "</table>";

                    return tableHtml;
                },
                
                show: $("#Type").val() !== "MAT",
                position: function (data, width, height, element) {
                    return { top: 0, left: 0 };
                }
            },
            onrendered: function () {
                applyChartStyles(el);
                restructureSchoolNames(el.id);
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

    BenchmarkChartsViewModel.SelectGrouping = function (grouping) {
        $("#ChartGroup").val(grouping);
        $("#ChartGroup").change();
        $("#BCHeader")[0].scrollIntoView();
        $("#ChartGroup").focus();
    };

    BenchmarkChartsViewModel.RebuildCharts = function () {
        var self = this;
        var tabParameter = $("#tabSelection").val();
        var chartGroupParameter = $("#ChartGroup").val();
        var unitParameter = $("#ShowValue").val();
        var centralFinancing = $("#CentralFinancing").val();
        var trustCentralFinancing = $("#TrustCentralFinancing").val();
        var formatParameter = sessionStorage.chartFormat;
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

        if (formatParameter) {
            url += "&format=" + formatParameter;
        }

        $.ajax({
            url: url,
            datatype: 'json',
            beforeSend: function() {
                DfE.Util.LoadingMessage.display("#benchmarkChartsList", "Updating charts");
            },
            success: function(data) {
                $("#benchmarkChartsList").html(data);
                self.RefreshAddRemoveLinks();
                $('.save-as-image').show();
                self.GenerateCharts(unitParameter);
                $("table.dataTable").tablesorter();
            }
        });
    };

    BenchmarkChartsViewModel.saveAsImage = function(name, id) {
        var svg = $('#' + id).find('svg')[0];
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' });
    };

    BenchmarkChartsViewModel.PrintPage = function() {

        var accordion_sections = $("#benchmarkChartsList .accordion-section");
        accordion_sections.attr('aria-expanded', true)

        var buttons = $("#benchmarkChartsList .chart-accordion-header")
        buttons.each(function() {
            $(this).attr('aria-label', $(this).attr('aria-label').replace("Show", "Hide"))
        });

        window.print();
    };
    
    BenchmarkChartsViewModel.ChangeTab = function (tab) {
        var self = this;
        if (tab === "Custom") {
            $(".tabs li").removeClass("active");
            $(".tabs li#" + tab).addClass("active");
            $("#tabsSection").empty('');
            $("#tabsSection").show();
            $("#customTabSection").show();
            $(".download-links").hide();
        } else if (tab === "BestInClass") {
            $(".tabs li").removeClass("active");
            $(".tabs li#" + tab).addClass("active");
            $("#customTabSection").hide();
            $(".download-links").show();
            $("#bestInClassTabSection").show();
            $("#tabsSection").hide();
        }else {
            var unitParameter = $("#ShowValue").val();
            var financingParameter = $("#CentralFinancing").val();
            var trustFinancingParameter = $("#TrustCentralFinancing").val();
            unitParameter = unitParameter ? unitParameter : "AbsoluteMoney";
            var typeParameter = $("#Type").val();
            var formatParameter = sessionStorage.chartFormat;
            var url = "/benchmarkcharts/tabchange?tab=" + tab +
                "&type=" + typeParameter +
                "&showValue=" + unitParameter;
            if (financingParameter) {
                url += "&financing=" + financingParameter;
            }
            if (trustFinancingParameter) {
                url += "&trustFinancing=" + trustFinancingParameter;
            }
            if (formatParameter) {
                url += "&format=" + formatParameter;
            }
            $.ajax({
                url: url,
                datatype: 'json',
                beforeSend: function () {
                    $("#bestInClassTabSection").hide();
                    $("#customTabSection").hide();
                    $("#tabsSection").show();
                    DfE.Util.LoadingMessage.display("#tabsSection", "Updating charts");
                },
                success: function (data) {
                    $(".tabs li").removeClass("active");
                    $(".tabs li#" + tab).addClass("active");
                    $(".download-links").show();
                    $("#tabsSection").html(data);                    
                    $("table.dataTable").tablesorter();
                    var unitParameter = $("#ShowValue").val();
                    self.RefreshAddRemoveLinks();
                    $('.save-as-image').show();
                    self.GenerateCharts(unitParameter);
                }
            });
        }
    };

    BenchmarkChartsViewModel.HideShowDetails = function (element) {
        var $table = $(element).closest('table');
        $table.find('.detail').toggle(200);
    }

    BenchmarkChartsViewModel.Load = function () {
        new DfE.Views.BenchmarkChartsViewModel();
    };

    BenchmarkChartsViewModel.PdfGenerator = PdfGenerator();

    BenchmarkChartsViewModel.PdfPage = function () {        

        BenchmarkChartsViewModel.PdfGenerator.init();

        BenchmarkChartsViewModel.PdfGenerator.writeHeadings();

        BenchmarkChartsViewModel.PdfGenerator.writeWarnings();

        BenchmarkChartsViewModel.PdfGenerator.writeTabs();

        BenchmarkChartsViewModel.PdfGenerator.writeLastYearMessage();
        
        BenchmarkChartsViewModel.PdfGenerator.writeCharts();

        BenchmarkChartsViewModel.PdfGenerator.writeCriteria().then(function () {
            BenchmarkChartsViewModel.PdfGenerator.writeContextData().then(function () {
                BenchmarkChartsViewModel.PdfGenerator.save();
            });
        });
    };


    Views.BenchmarkChartsViewModel = BenchmarkChartsViewModel;
}(GOVUK, DfE.Views));

function PdfGenerator() {

    var MARGIN_LEFT = 20;
    var doc, offset;
    
    function pdfGenerateImage(element) {

        function getCanvas(element) {
            return html2canvas($(element), {
                imageTimeout: 2000,
                removeContainer: true
            });
        }

        return getCanvas(element);
    }

    function pdfAddImage(canvas) {
        var img = canvas.toDataURL("image/png");
        doc.addImage(img, 'JPEG', MARGIN_LEFT, offset);
    }
    
    function pdfWriteLine(type, text) {
        doc.setFont("helvetica");
        doc.setTextColor(0, 0, 0);
        var fontSize;
        switch (type) {
            case 'H1':
                doc.setFontType("bold");
                fontSize = 40;
                break;
            case 'H2':
                doc.setFontType("bold");
                fontSize = 30;
                break;
            case 'H3':
                doc.setFontType("bold");
                fontSize = 20;
                break;
            case 'Warning':
                doc.setFontType("italic");
                doc.setTextColor(244, 119, 56);
                fontSize = 20;
                break;
            case 'Info':
                doc.setFontType("italic");
                fontSize = 15;
                break;
            default:
                doc.setFontType("normal");
                fontSize = 15;
        }

        doc.setFontSize(fontSize);
        doc.text(MARGIN_LEFT, offset, text);
        offset += fontSize + 8;
    }

    function pdfAddHorizontalLine() {
        doc.line(MARGIN_LEFT, offset, 620, offset);
        offset += 18;
    }

    function pdfAddNewPage() {
        doc.addPage('a3', 'portrait');
        offset = 70;
    }

    function pdfWriteChart(index) {
        var svg = $('#chart_' + index).find('svg')[0];
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' },
            function (img) {
                doc.addImage(img, 'JPEG', -50, offset);
            });
    }

    function pdfWriteTable(id) {    
        var headers = $(id + ' th').toArray().map(function (th) { return th.attributes['data-header'].value });
        var data = $(id + ' tbody tr').toArray().map(function (tr) {
            var trObj = {}
            $(tr).children('td').toArray().map(function (td) {
                trObj[td.attributes['data-header'].value] = td.textContent.trim();
            });
            return trObj;
        });

        doc.table(MARGIN_LEFT, offset, data, headers, { autoSize: true, fontSize: 9, margins: {bottom: 5, left: 5, top: 5, width: 600} });
    }

    function pdfSave() {
        doc.save('sfb-benchmark-charts.pdf');
    }

    return {

        init: function () {
            doc = new jsPDF({ unit: 'px', format: 'a3' });
            offset = 60;           
        },

        writeHeadings: function () {

            pdfWriteLine('H1', 'Schools Financial Benchmarking');

            pdfWriteLine('H2', $('#BCHeader').get(0).innerText);

            if ($('#comparing').length > 0) {
                pdfWriteLine('H3', $('#comparing').get(0).innerText);
            }
        },

        writeWarnings: function () {

            var warnings = $('.panel.orange-warning');
            if (warnings.length > 0) {
                warnings.each(function (index, element) {
                    pdfWriteLine('Warning', element.innerText);
                });
            }
        },

        writeTabs: function () {

            offset += 30;

            if ($('.tabs li.active').length > 0) {
                pdfWriteLine('H3', $('.tabs li.active').get(0).innerText);
            }

            var filters = $('.chart-filter');
            if (filters.length > 0) {
                filters.each(function (index, element) {
                    pdfWriteLine('Normal', $(element).find('label').get(0).innerText + ': ' + $(element).find('option[selected]').get(0).innerText);
                });
            }
        },

        writeLastYearMessage: function () {
            pdfAddHorizontalLine();
            if ($('.latest-year-message').length > 0) {
                pdfWriteLine('Info', $('.latest-year-message').get(0).innerText);
            }

        },

        writeCharts: function () {
            var charts = $('.chartContainer');
            var yValuesCount = JSON.parse($(".chart").first().attr('data-chart')).length;
            var chartPerPage = Math.ceil(12 / yValuesCount);

            charts.each(function (index, element) {
                if (index % chartPerPage == 0) {
                    pdfAddNewPage();
                } else
                {
                    offset += (800 / chartPerPage);
                }
                var header = $(element).find('h2').get(0).innerText;                
                if (header.length < 60) {
                    pdfWriteLine('H3', header);
                } else {
                    var header1 = header.substring(0, header.lastIndexOf('('));
                    var header2 = header.substring(header.lastIndexOf('('));
                    pdfWriteLine('H3', header1);
                    pdfWriteLine('H3', header2);
                }
                if (sessionStorage.chartFormat === 'Charts') {
                    pdfWriteChart(index);
                } else {
                    pdfWriteTable('#table_for_chart_' + index);
                }
            });
        },

        writeCriteria: function () {
            return new Promise(function(resolve, reject) {
                if ($('#criteriaTable').length > 0 && $('#criteriaTable').is(":visible")) {
                    pdfAddNewPage();
                    pdfWriteLine('Normal', $('#criteriaExp').get(0).innerText)
                    pdfGenerateImage('#criteriaTable').then(function (canvas) {
                        pdfAddImage(canvas);
                        resolve();
                    });
                } else {
                    resolve();
                }
            });
        },

        writeContextData: function () {
            return new Promise(function (resolve, reject) {
                if ($('#contextDataTable').length > 0 && $('#contextDataTable').is(":visible")) {
                    pdfAddNewPage();
                    pdfWriteLine('H2', $('#contextExp').get(0).innerText)
                    pdfGenerateImage('#contextDataTable').then(function (canvas) {
                        pdfAddImage(canvas);
                        resolve();
                    });
                } else {
                    resolve();
                }
            });
        },

        save: function () {
            pdfSave();
        }

    };
}