"use strict";

class BenchmarkChartManager {

    generateChart(el, showValue, min, mid, max, barCount) {
        let applyChartStyles = function (el) {

            let benchmarkSchoolIndex = $("input[name='benchmarkSchoolIndex']", $(el).closest('.chart-container'))[0].value;

            if (benchmarkSchoolIndex > -1) {
                $(`#${el.id} .c3-shape.c3-shape-${benchmarkSchoolIndex}.c3-bar.c3-bar-${benchmarkSchoolIndex}`).css("fill", "#D53880");
            }

            let incompleteFinanceDataIndex = $("input[name='incompleteFinanceDataIndex']", $(el).closest('.chart-container'))[0].value;
            let incompleteFinanceDataIndexArray = incompleteFinanceDataIndex.split(",");
            if (incompleteFinanceDataIndexArray.length > 0) {
                incompleteFinanceDataIndexArray.forEach(function (index) {
                    $(`#${el.id} .c3-shape.c3-shape-${index}.c3-bar.c3-bar-${index}`).css("fill", "#F47738");
                });
            }

            let texts = $("#" + el.id + " .c3-axis-x g.tick text tspan");
            texts.css('fill', '#1D70B8');

            let svg = $("#" + el.id + " svg");
            svg.css('font-size', '14px');
            svg.css('letter-spacing', 'initial');

            let axisLines = $("#" + el.id + " .ApplicationCore");
            axisLines.css('fill', 'none');
            axisLines.css('stroke', '#000');

            let tickLines = $("#" + el.id + " .tick line");
            tickLines.css('fill', 'none');
            tickLines.css('stroke', '#000');
        };

        let restructureSchoolNames = function (id) {
            let moveLabelLeft = function ($text, margin) {
                $tspan = $text.find('tspan');
                let originalTextX = $text.attr('x');
                let newTextX = originalTextX - margin;
                $text.attr('x', newTextX);
                $tspan.attr('x', newTextX);
                return originalTextX;
            };

            let drawExcIcon = function (originalTextX, $text) {
                let tick = $text.parent()[0];
                let newTextX = originalTextX - 11;
                let isMobile = $(window).width() <= 640;
                let cy = 14;
                let r = 7;
                if (isMobile) {
                    cy += 3;
                }
                let isMAT = $("#Type").val() === "MAT";
                d3.select(tick).append('circle')
                    .classed("ex-icon-circle", 1)
                    .attr("stroke", "#1D70B8")
                    .attr("stroke-width", "4")
                    .attr("fill", "#1D70B8")
                    .attr("r", r)
                    .attr("cy", cy)
                    .attr("cx", newTextX);
                $(tick.lastElementChild).on('click', () => DfE.Views.BenchmarkChartsViewModel.renderMissingFinanceInfoModal(isMAT));
                d3.select(tick).append('line')
                    .classed("ex-icon", 1)
                    .attr("x1", newTextX)
                    .attr("y1", cy - r + 2)
                    .attr("x2", newTextX)
                    .attr("y2", cy - r + 8);
                $(tick.lastElementChild).on('click', () => DfE.Views.BenchmarkChartsViewModel.renderMissingFinanceInfoModal(isMAT));
                d3.select(tick).append('line')
                    .classed("ex-icon", 1)
                    .attr("x1", newTextX)
                    .attr("y1", cy - r + 11)
                    .attr("x2", newTextX)
                    .attr("y2", cy - r + 12);
                $(tick.lastElementChild).on('click', () => DfE.Views.BenchmarkChartsViewModel.renderMissingFinanceInfoModal(isMAT));
            };

            let drawProgressScoreBox = function (originalTextX, $text, progressScoreType, progressScore, p8binding) {
                let tick = $text.parent()[0];
                let newTextX = originalTextX - 42;
                let isMobile = $(window).width() <= 640;
                let height = 18;
                let width = 40;
                if (isMobile) {
                    height += 3;
                }

                let progressColour;
                let fontColour = "#FFFFFF";

                if (progressScore) {
                    if (progressScoreType === "p8") {
                        if (p8binding === 5) {
                            progressColour = "#df3034";
                        }
                        else if (p8binding === 4) {
                            progressColour = "#f47738";
                            fontColour = "#0C0C0C";
                        }
                        else if (p8binding === 3) {
                            progressColour = "#ffdd00";
                            fontColour = "#0C0C0C";
                        }
                        else if (p8binding === 2) {
                            progressColour = "#85994b";
                            fontColour = "#0C0C0C";
                        }
                        else if (p8binding === 1) {
                            progressColour = "#006435";
                        }
                    } else {
                        if (progressScore < -3) {
                            progressColour = "#df3034";
                        }
                        else if (progressScore >= -3 && progressScore < -2) {
                            progressColour = "#f47738";
                            fontColour = "#0C0C0C";
                        }
                        else if (progressScore >= -2 && progressScore <= 2) {
                            progressColour = "#ffdd00";
                            fontColour = "#0C0C0C";
                        }
                        else if (progressScore > 2 && progressScore <= 3) {
                            progressColour = "#85994b";
                            fontColour = "#0C0C0C";
                        }
                        else if (progressScore > 3) {
                            progressColour = "#006435";
                        }
                    }
                } else {
                    progressColour = "#DEE0E2";
                    fontColour = "#0B0C0C";
                }

                d3.select(tick).append('rect')
                    .classed("progress-svg", 1)
                    .attr("stroke", progressColour)
                    .attr("stroke-width", "4")
                    .attr("fill", progressColour)
                    .attr("width", width)
                    .attr("height", height)
                    .attr("x", newTextX)
                    .attr("y", 6);
                d3.select(tick).append("text")
                    .text(progressScore ? progressScore.toFixed(2) : "N/A")
                    .attr("fill", fontColour)
                    .attr("x", newTextX + 7)
                    .attr("y", 20);
            };

            let texts = $(".bm-charts-list #" + id + " .c3-axis-x g.tick text");
            let chartData = $("#" + id).data('chart');

            texts.each(function () {
                let schoolNameParts = $(this).find('tspan');
                if (schoolNameParts.length === 0) {
                    return;
                }
                let schoolName;
                if (schoolNameParts[0]) {
                    schoolName = schoolNameParts[0].textContent;
                }
                if (schoolNameParts[1]) {
                    schoolName += ` ${schoolNameParts[1].textContent}`;
                }
                let schoolData = chartData.find(c => c.school === schoolName);
                if (!schoolData) {
                    schoolData = chartData.find(c => c.school.startsWith(schoolName.replace('...', '')));
                }
                let urn = schoolData.urn;
                let type = $("#Type").val();

                if (type === "MAT") {
                    $(this).on('click', () => window.open("/trust/detail?companyNo=" + urn, '_self'));
                } else {
                    $(this).on('click', () => {
                        dataLayer.push({ 'event': 'bmc_school_link_click' });
                        window.open("/school?urn=" + urn, '_self');
                    });
                }

                if (schoolNameParts.length === 1) {
                    let limit = 42;
                    let text = schoolName.length < limit
                        ? schoolName
                        : urn === $("#HomeSchoolURN").val()
                            ? schoolName.substring(0, limit - 5) + "..."
                            : schoolName.substring(0, limit - 3) + "...";
                    $(this).find('tspan').text(text);
                }

                if (urn === $("#HomeSchoolURN").val()) {
                    $(this).css("font-weight", "bold");
                }

                if (type === "MAT") {
                    if (schoolData.partialyearspresent) {
                        let originalTextX = moveLabelLeft($(this), 25);
                        drawExcIcon(originalTextX, $(this));
                    }
                } else {
                    if (!schoolData.iscompleteyear) {
                        let originalTextX = moveLabelLeft($(this), 25);
                        drawExcIcon(originalTextX, $(this));
                    }
                }

                if ($('#ComparisonType').val() === 'BestInClass') {
                    let originalTextX = moveLabelLeft($(this), 50);
                    drawProgressScoreBox(originalTextX, $(this), schoolData.progressscoretype, schoolData.progressscore, schoolData.p8banding, $("[name='bicCriteria.OverallPhase']").val());
                }
            });
        };

        let insertProgressLabel = function () {
            var left = $("#chart_0")[0].getBoundingClientRect().width - $(".c3-event-rects.c3-event-rects-single")[0].getBoundingClientRect().width - 70;
            $(".chart-scores-header").css("left", left);
            $(".chart-scores-header").show();
        };

        showValue = showValue || "AbsoluteMoney";
        let paddingBottom = min < 0 ? 100 : 0;
        let axisLabel = $('#' + el.id).attr('data-axis-label');
        let yAxis, yFormat;
        let isMobile = $(window).width() <= 640;
        let isBic = $("#ComparisonType").val() === "BestInClass";

        switch (showValue) {
            case "AbsoluteCount":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartIntegerFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => { return window.DfE.Util.Charting.chartDecimalFormat(d); };
                break;
            case "AbsoluteMoney":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); };
                break;
            case "PerPupil":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => {
                    return window.DfE.Util.Charting.chartMoneyFormat(d);
                };
                break;
            case "PerTeacher":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartMoneyFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => {
                    return window.DfE.Util.Charting.chartMoneyFormat(d);
                };
                break;
            case "PercentageOfTotalIncome":
            case "PercentageOfTotalExpenditure":
            case "FTERatioToTotalFTE":
            case "PercentageTeachers":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartPercentageFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom,
                        top: 50
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => { return window.DfE.Util.Charting.chartPercentageFormat(d); };
                break;
            case "NoOfPupilsPerMeasure":
            case "HeadcountPerFTE":
                yAxis = {
                    tick: {
                        format: (d) => { return window.DfE.Util.Charting.chartDecimalFormat(d); },
                        values: () => { return [min, mid, max]; },
                        count: 3
                    },
                    min: min,
                    max: max,
                    padding: {
                        bottom: paddingBottom
                    },
                    label: {
                        text: isMobile ? axisLabel.replace('including', 'inc.').replace('excluding', 'excl.') : axisLabel,
                        position: 'outer-center'
                    }
                };
                yFormat = (d) => { return window.DfE.Util.Charting.chartDecimalFormat(d); };
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
                height: (barCount > 4 ? barCount + 1 : barCount + 2) * 30 * (isMobile ? 1.3 : 1)
            },
            bar: {
                width: 20 * (isMobile ? 1.4 : 1)
            },
            axis: {
                y: yAxis,
                x: {
                    type: 'category', // this needed to load string x value
                    tick: {
                        centered: true,
                        multiline: isMobile,
                        multilineMax: 2
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
                        "<table class='govuk-table bmc-rollover-table' >" +
                        "<tr class='govuk-table__row'><th colspan='2' class='" + highlight + "'>" + name + "</th></tr>" +
                        "<tr class='govuk-table__row'><th scope='row' class='govuk-table__header'>Local authority</th><td class='govuk-table__cell'>" + schoolData.la + "</td></tr>" +
                        "<tr class='govuk-table__row'><th scope='row' class='govuk-table__header'>School type</th><td class='govuk-table__cell'>" + schoolData.type + "</td></tr>" +
                        "<tr class='govuk-table__row'><th scope='row' class='govuk-table__header'>Number of pupils</th><td class='govuk-table__cell'>" + schoolData.pupilCount + "</td></tr>";

                    if ($("#ComparisonType").val() === "Specials") {
                        tableHtml += "<tr class='govuk-table__row'><th style='max-width: 150px' scope='row' class='govuk-table__header'>Highest 3 SEN characteristics</th><td class='govuk-table__cell'>";
                        schoolData.topsen.forEach(
                            topsen => { tableHtml += `${topsen.Key}: ${topsen.Value}%<br/>`; }
                        )
                        tableHtml += "</td></tr>";
                    }

                    if ($("#ComparisonType").val() === "BestInClass") {
                        tableHtml += "<tr class='govuk-table__row'><th class='govuk-table__header'>Key stage progress</th><td class='govuk-table__cell'>" + schoolData.progressscore + "</td></tr>";
                    }

                    tableHtml += "</table>";

                    return tableHtml;
                },

                show: $("#Type").val() !== "MAT",
                position: (data, width, height, element) => {
                    return { top: 0, left: 15 };
                }
            },
            padding: {
                bottom: 10,
                left: isMobile ? (isBic ? 180 : 140) : (isBic ? 360 : 310),
                right: 15,
                top: 20
            },
            onrendered: () => {
                applyChartStyles(el);
                restructureSchoolNames(el.id);
                insertProgressLabel();
            }
        });
    }

    generateCharts(unitParameter) {
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

        $(".bm-charts-list .chart").each(function () {
            let yValues = JSON.parse($('#' + this.id).attr('data-chart'));
            let unit = unitParameter ? unitParameter : yValues[0].unit;
            let minBy = _.minBy(yValues, function (o) { return o.amount; });
            let minimum = minBy ? minBy.amount : 0;
            let maxBy = _.maxBy(yValues, function (o) { return o.amount; });
            let maximum = maxBy ? maxBy.amount : 0;
            if (minimum === 0 && maximum === 0) {
                self.generateChart(this, unit, 0, 0, 0, yValues.length);
            } else if (minimum === maximum) {
                self.generateChart(this, unit, minimum, minimum, minimum, yValues.length);
            } else {
                if (minimum > 0) {
                    minimum = 0;
                }
                let range = RoundedTickRange(minimum, maximum);
                let newMin = (minimum < 0)
                    ? (range * Math.floor(minimum / range))
                    : (range * Math.round(minimum / range));
                let newMax = range * Math.ceil(maximum / range);
                self.generateChart(this, unit, newMin, newMin + range, newMax, yValues.length);
            }
        });

        setTimeout(() => this.generateImageDataURIsOfVisibleCharts(), 2000);
    }

    generateImageDataURIsOfVisibleCharts() {
        let charts = $('.chart-container:visible');
        for (let i = 0; i < charts.length; i++) {
            if (sessionStorage.chartFormat === 'Charts') {
                let svg = $('#chart_' + i).find('svg')[0];
                saveSvgAsPng(svg, name + '.png', { canvg: canvg, backgroundColor: 'white' },
                    (img) => {
                        $('#chart_' + i).attr("data-img", img);
                    });
            }
        }
    }
}