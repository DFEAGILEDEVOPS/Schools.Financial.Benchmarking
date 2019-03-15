class BenchmarkChartsViewModel {

    constructor() {

        sessionStorage.chartFormat = 'Charts';

        $(document).ready(() => {
            $("#benchmarkChartsList table.dataTable").tablesorter();
            $("#bestInClassTabSection table.dataTable").tablesorter(
                    { sortList: [[7, 1]] }
            );
            this.GenerateCharts();
            this.RefreshAddRemoveLinks();
            $('.save-as-image').show();
        });

        GOVUK.Modal.Load();
    }


    //This function is accessing to the scope of the AngularJS controller to retrieve the chart selections model and update the buttons accordingly in other tabs.
    RefreshAddRemoveLinks() {

        function showRemoveLink(element) {
            $(element).find(".customRemove").show();
            $(element).find(".customAdd").hide();
        }

        function showAddLink(element) {
            $(element).find(".customAdd").show();
            $(element).find(".customRemove").hide();
        }

        let scope = angular.element($("#listCtrl")).scope();
        if (scope) {
            scope.dataLoaded.then(
                function () {
                    $(".customActions").each(function () {
                        let self = this;
                        let chartName = $(self).attr("data-fn");
                        let showValue = $(self).attr("data-sv");

                        _.forEach(scope.selectionList.HierarchicalCharts,
                            function (group) {
                                let selection = _.find(group.Charts,
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
    }

    //This function is accessing to the scope of the AngularJS controller tab to retrieve and update its chart selections model.
    AddRemoveYourCharts(chartName, showValue, checked, element) {
        let scope = angular.element($("#listCtrl")).scope();
        scope.$apply(() => {
            _.forEach(scope.selectionList.HierarchicalCharts,
                (group) => {
                    let selection = group.Charts.find((c) => {
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
        } else {
            $(element).parents(".your-chart-controls").find(".view-your-charts").hide();
        }

        this.RefreshAddRemoveLinks();
    }

    GenerateChart(el, showValue, min, mid, max, barCount) {
        let applyChartStyles = function (el) {
            let benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']", $(el).closest('.chart-container'))[0]
                .value;
            if (benchmarkSchoolIndex > -1) {
                $("#" +
                    el.id +
                    " .c3-shape.c3-shape-" +
                    benchmarkSchoolIndex +
                    ".c3-bar.c3-bar-" +
                    benchmarkSchoolIndex).css("fill", "#D53880");
            }

            let incompleteFinanceDataIndex = $("input[name='incompleteFinanceDataIndex']", $(el).closest('.chart-container'))[0].value;
            let incompleteFinanceDataIndexArray = incompleteFinanceDataIndex.split(",");
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

            let incompleteWorkforceDataIndex = $("input[name='incompleteWorkforceDataIndex']", $(el).closest('.chart-container'))[0].value;
            let incompleteWorkforceDataIndexArray = incompleteWorkforceDataIndex.split(",");
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

            let texts = $("#" + el.id + " .c3-axis-x g.tick text tspan");
            texts.css('fill', '#005ea5');
                       
            let svg = $("#" + el.id + " svg");
            svg.css('font-size', '14px');

            let axisLines = $("#" + el.id + " .domain");
            axisLines.css('fill', 'none');
            axisLines.css('stroke', '#000');

            let tickLines = $("#" + el.id + " .tick line");
            tickLines.css('fill', 'none');
            tickLines.css('stroke', '#000');
        };

        let restructureSchoolNames = function (id) {
            let texts = $("#" + id + " .c3-axis-x g.tick text tspan");

            texts.each(function () {
                let textParts = $(this).text().split("#");

                let type = $("#Type").val();

                if (type === "MAT") {
                    $(this).on('click',
                        function (e, i) {
                            window.open("/trust/index?companyNo=" + textParts[1] + "&name=" + textParts[0], '_self');
                        });
                } else {
                    $(this).on('click',
                        function (e, i) {
                            dataLayer.push({ 'event': 'bmc_school_link_click' });
                            window.open("/school/detail?urn=" + textParts[1], '_self');
                        });
                }
                let limit = 36;
                let text = textParts[0].length < limit
                    ? textParts[0]
                    : textParts[0].substring(0, limit - 3) + "...";
                $(this).text(text);

                if (textParts[0] === $("#HomeSchoolName").val()) {
                    $(this).css("font-weight", "bold");
                }
            });
        };

        showValue = showValue || "AbsoluteMoney";
        let paddingBottom = min < 0 ? 100 : 0;
        let axisLabel = $('#' + el.id).attr('data-axis-label');
        let yAxis, yFormat;
        switch (showValue) {
            case "AbsoluteCount":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.ChartIntegerFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => { return window.DfE.Util.Charting.ChartDecimalFormat(d); }
                break;
            case "AbsoluteMoney":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => { return window.DfE.Util.Charting.ChartMoneyFormat(d); };
                break;
            case "PerPupil":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => {
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
                        format: (d) => { return window.DfE.Util.Charting.ChartMoneyFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => {
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
                        format: (d) => { return window.DfE.Util.Charting.ChartPercentageFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => { return window.DfE.Util.Charting.ChartPercentageFormat(d); };
                break;
            case "NoOfPupilsPerMeasure":
            case "HeadcountPerFTE":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.ChartDecimalFormat(d); },
                        values: () => { return ($(window).width() <= 640) ? [max] : [min, mid, max] },
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
                yFormat = (d) => { return window.DfE.Util.Charting.ChartDecimalFormat(d); };
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
                    let nameAndUrn = defaultTitleFormat(d[0].index).split('#');
                    let name = nameAndUrn[0];
                    let chartData = JSON.parse($('#' + el.id).attr('data-chart'));
                    let schoolData = chartData[d[0].index];
                    let benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']",
                        $(el).closest('.chart-container'))[0].value;
                    let highlight = benchmarkSchoolIndex === d[0].index.toString() ? "highlighted" : "";
                    let tableHtml =
                        "<table class='bmc-rollover-table' >" +
                        "<tr><th colspan='2' class='" + highlight + "'>" + name + "</th></tr>" +
                        "<tr><td class='bold'>Local authority</td><td>" + schoolData.la + "</td></tr>" +
                        "<tr><td class='bold'>School type</td><td>" + schoolData.type + "</td></tr>" +
                        "<tr><td class='bold'>Number of pupils</td><td>" + schoolData.pupilCount + "</td></tr>";

                    if ($("#ComparisonType").val() === "BestInClass") {
                        tableHtml += "<tr><td class='bold'>Key stage progress</td><td>" + schoolData.progressscore + "</td></tr>";
                    }

                    tableHtml += "</table>";

                    return tableHtml;
                },

                show: $("#Type").val() !== "MAT",
                position: () => {
                    return { top: 0, left: 0 };
                }
            },
            padding: {
                bottom: 10
            },
            onrendered: () => {
                applyChartStyles(el);
                restructureSchoolNames(el.id);
            }
        });
    }

    GenerateCharts(unitParameter) {
        let self = this;
        let RoundedTickRange = function (min, max) {
            let range = max - min;
            let tickCount = 3;
            let unroundedTickSize = range / (tickCount - 1);
            let x = Math.ceil(Math.log10(unroundedTickSize) - 1);
            let pow10x = Math.pow(10, x);
            let roundedTickRange = Math.ceil(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        };

        $(".chart").each(function () {
            let yValues = JSON.parse($('#' + this.id).attr('data-chart'));
            let unit = unitParameter ? unitParameter : yValues[0].unit;
            let minBy = _.minBy(yValues, function (o) { return o.amount; });
            let minimum = minBy ? minBy.amount : 0;
            let maxBy = _.maxBy(yValues, function (o) { return o.amount; });
            let maximum = maxBy ? maxBy.amount : 0;
            if (minimum === 0 && maximum === 0) {
                self.GenerateChart(this, unit, 0, 0, 0, yValues.length);
            } else if (minimum === maximum) {
                self.GenerateChart(this, unit, minimum, minimum, minimum, yValues.length);
            } else {
                if (minimum > 0) {
                    minimum = 0;
                }
                let range = RoundedTickRange(minimum, maximum);
                let newMin = (minimum < 0)
                    ? (range * Math.floor(minimum / range))
                    : (range * Math.round(minimum / range));
                let newMax = range * Math.ceil(maximum / range);
                self.GenerateChart(this, unit, newMin, newMin + range, newMax, yValues.length);
            }
        });

        new Accordion(document.getElementById('bm-charts-accordion'));
    }

    SelectGrouping(grouping, parentGrouping) {
        $("#ChartGroup").val(grouping);
        $("#ChartGroup").change();
        $("#BCHeader")[0].scrollIntoView();
        $("#ChartGroup").focus();
        $(".back-to-main-chart-group-button .js-parent-group").text(parentGrouping);
        $(".back-to-main-chart-group-button").show();
    }

    ResetGrouping() {
        $('#ChartGroup').prop('selectedIndex', 0);
        $("#ChartGroup").change();
        $(".back-to-main-chart-group-button").hide();
    }

    RebuildCharts() {
        let tabParameter = $("#tabSelection").val();
        let chartGroupParameter = $("#ChartGroup").val();
        let unitParameter = $("#ShowValue").val();
        let centralFinancing = $("#CentralFinancing").val();
        let trustCentralFinancing = $("#TrustCentralFinancing").val();
        let formatParameter = sessionStorage.chartFormat;
        let type = $("#Type").val();

        let url = "/benchmarkcharts/getcharts?revgroup=" +
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
            beforeSend: () => {
                DfE.Util.LoadingMessage.display("#benchmarkChartsList", "Updating charts");
            },
            success: (data) => {
                $("#benchmarkChartsList").html(data);
                this.RefreshAddRemoveLinks();
                $('.save-as-image').show();
                this.GenerateCharts(unitParameter);
                $("table.dataTable").tablesorter();
            }
        });
    }

    saveAsImage(name, id) {
        let svg = $('#' + id).find('svg')[0];
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' });
    }

    PrintPage() {

        let accordion_sections = $("#benchmarkChartsList .accordion-section");
        accordion_sections.attr('aria-expanded', true);

        let buttons = $("#benchmarkChartsList .chart-accordion-header");
        buttons.each(function () {
            $(this).attr('aria-label', $(this).attr('aria-label').replace("Show", "Hide"));
        });

        window.print();
    }

    ChangeTab(tab) {
        if (tab === "Custom") {
            $(".tabs li").removeClass("active");
            $(".tabs li a span.bmtab").text("");
            $(".tabs li#" + tab).addClass("active");
            $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
            $("#tabsSection").empty('');
            $("#tabsSection").show();
            $("#customTabSection").show();
            $(".download-links").hide();
        } else if (tab === "BestInClass") {
            $(".tabs li").removeClass("active");
            $(".tabs li a span.bmtab").text("");
            $(".tabs li#" + tab).addClass("active");
            $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
            $("#customTabSection").hide();
            $(".download-links").show();
            $("#bestInClassTabSection").show();
            $("#tabsSection").hide();
        } else {
            let unitParameter = $("#ShowValue").val();
            let financingParameter = $("#CentralFinancing").val();
            let trustFinancingParameter = $("#TrustCentralFinancing").val();
            unitParameter = unitParameter ? unitParameter : "AbsoluteMoney";
            let typeParameter = $("#Type").val();
            let formatParameter = sessionStorage.chartFormat;
            let url = "/benchmarkcharts/tabchange?tab=" + tab +
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
                beforeSend: () => {
                    $("#bestInClassTabSection").hide();
                    $("#customTabSection").hide();
                    $("#tabsSection").show();
                    DfE.Util.LoadingMessage.display("#tabsSection", "Updating charts");
                },
                success: (data) => {
                    $(".tabs li").removeClass("active");
                    $(".tabs li a span.bmtab").text("");
                    $(".tabs li#" + tab).addClass("active");
                    $(".tabs li#" + tab + " a span.bmtab").text(" selected ");
                    $(".download-links").show();
                    $("#tabsSection").html(data);
                    $("table.dataTable").tablesorter();
                    let unitParameter = $("#ShowValue").val();
                    this.RefreshAddRemoveLinks();
                    $('.save-as-image').show();
                    this.GenerateCharts(unitParameter);
                }
            });
        }
    }

    HideShowDetails(element) {
        let $table = $(element).closest('table');
        $table.find('.detail').toggle(200);
    }

    PdfPage() {

        let pdfGenerator = new PdfGenerator();

        pdfGenerator.writeHeadings();

        pdfGenerator.writeWarnings();

        pdfGenerator.writeTabs();

        pdfGenerator.writeLastYearMessage();

        pdfGenerator.writeCharts();

        pdfGenerator.writeCriteria().then(() => {
            pdfGenerator.writeContextData().then(() => {
                pdfGenerator.save();
            });
        });
    }

    SubmitCriteriaForm() {
        $('form#advancedCriteria').submit();
    }
}

class PdfGenerator {

    constructor() {
        this.MARGIN_LEFT = 20;
        this.doc = new jsPDF({ unit: 'px', format: 'a3' });
        this.offset = 60;
    }

    pdfGenerateImage(element) {

        function getCanvas(element) {
            return html2canvas($(element), {
                imageTimeout: 2000,
                removeContainer: true
            });
        }

        return getCanvas(element);
    }

    pdfAddImage(canvas) {
        let img = canvas.toDataURL("image/png");
        this.doc.addImage(img, 'JPEG', this.MARGIN_LEFT, this.offset);
    }

    pdfWriteLine(type, text) {
        this.doc.setFont("helvetica");
        this.doc.setTextColor(0, 0, 0);
        let fontSize;
        switch (type) {
            case 'H1':
                this.doc.setFontType("bold");
                fontSize = 40;
                break;
            case 'H2':
                this.doc.setFontType("bold");
                fontSize = 30;
                break;
            case 'H3':
                this.doc.setFontType("bold");
                fontSize = 20;
                break;
            case 'Warning':
                this.doc.setFontType("italic");
                this.doc.setTextColor(244, 119, 56);
                fontSize = 20;
                break;
            case 'Info':
                this.doc.setFontType("italic");
                fontSize = 15;
                break;
            default:
                this.doc.setFontType("normal");
                fontSize = 15;
        }

        this.doc.setFontSize(fontSize);
        this.doc.text(this.MARGIN_LEFT, this.offset, text);
        this.offset += fontSize + 8;
    }

    pdfAddHorizontalLine() {
        this.doc.line(this.MARGIN_LEFT, this.offset, 620, this.offset);
        this.offset += 18;
    }

    pdfAddNewPage() {
        this.doc.addPage('a3', 'portrait');
        this.offset = 70;
    }

    pdfWriteChart(index) {
        let svg = $('#chart_' + index).find('svg')[0];
        saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' },
            (img) => {
                this.doc.addImage(img, 'JPEG', -50, this.offset);
            });
    }

    pdfWriteTable(id) {
        let headers = $(id + ' th').toArray().map((th) => {
            return th.attributes['data-header'].value;
        });
        let data = $(id + ' tbody tr').toArray().map((tr) => {
            let trObj = {}
            $(tr).children('td').toArray().map((td) => {
                trObj[td.attributes['data-header'].value] = td.textContent.trim();
            });
            return trObj;
        });

        this.doc.table(this.MARGIN_LEFT, this.offset, data, headers, { autoSize: true, fontSize: 9, margins: { bottom: 5, left: 5, top: 5, width: 600 } });
    }

    pdfSave() {
        this.doc.save('sfb-benchmark-charts.pdf');
    }

    writeHeadings() {

        this.pdfWriteLine('H1', 'Schools Financial Benchmarking');

        this.pdfWriteLine('H2', $('#BCHeader').get(0).innerText);

        if ($('#comparing').length > 0) {
            this.pdfWriteLine('H3', $('#comparing').get(0).innerText);
        }
    }

    writeWarnings() {

        let warnings = $('.panel.orange-warning');
        if (warnings.length > 0) {
            warnings.each((index, element) => {
                this.pdfWriteLine('Warning', element.innerText);
            });
        }
    }

    writeTabs() {

        this.offset += 30;

        if ($('.tabs li.active').length > 0) {
            this.pdfWriteLine('H3', $('.tabs li.active').get(0).innerText);
        }

        let filters = $('.chart-filter');
        if (filters.length > 0) {
            filters.each((index, element) => {
                this.pdfWriteLine('Normal', $(element).find('label').get(0).innerText + ': ' + $(element).find('option[selected]').get(0).innerText);
            });
        }
    }

    writeLastYearMessage() {
        this.pdfAddHorizontalLine();
        if ($('.latest-year-message').length > 0) {
            this.pdfWriteLine('Info', $('.latest-year-message').get(0).innerText);
        }

    }

    writeCharts() {
        let charts = $('.chart-container');
        let yValuesCount = JSON.parse($(".chart").first().attr('data-chart')).length;
        let chartPerPage = Math.ceil(12 / yValuesCount);

        charts.each((index, element) => {
            if (index % chartPerPage === 0) {
                this.pdfAddNewPage();
            } else {
                this.offset += (800 / chartPerPage);
            }
            let header = $(element).find('h2').get(0).innerText;
            if (header.length < 60) {
                this.pdfWriteLine('H3', header);
            } else {
                let header1 = header.substring(0, header.lastIndexOf('('));
                let header2 = header.substring(header.lastIndexOf('('));
                this.pdfWriteLine('H3', header1);
                this.pdfWriteLine('H3', header2);
            }
            if (sessionStorage.chartFormat === 'Charts') {
                this.pdfWriteChart(index);
            } else {
                this.pdfWriteTable('#table_for_chart_' + index);
            }
        });
    }

    writeCriteria() {
        return new Promise((resolve, reject) => {
            if ($('#criteriaTable').length > 0 && $('#criteriaTable').is(":visible")) {
                this.pdfAddNewPage();
                this.pdfWriteLine('Normal', $('#criteriaExp').get(0).innerText)
                this.pdfGenerateImage('#criteriaTable').then((canvas) => {
                    this.pdfAddImage(canvas);
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    writeContextData() {
        return new Promise((resolve, reject) => {
            if ($('#contextDataTable').length > 0 && $('#contextDataTable').is(":visible")) {
                this.pdfAddNewPage();
                this.pdfWriteLine('H2', $('#contextExp').get(0).innerText)
                this.pdfGenerateImage('#contextDataTable').then((canvas) => {
                    this.pdfAddImage(canvas);
                    resolve();
                });
            } else {
                resolve();
            }
        });
    }

    save() {
        this.pdfSave();
    }
}