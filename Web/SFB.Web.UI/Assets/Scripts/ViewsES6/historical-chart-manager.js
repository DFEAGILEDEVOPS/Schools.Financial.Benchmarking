"use strict";

class HistoricalChartManager {
    generateFinanceCharts() {
        let unitParameter = $("#ShowValue.js-finance-showValue").val();
        $(".finance-charts-list .chart").each(
            (i, el) => this.generateCharts(el, unitParameter)
        );
    }

    generateWorkforceCharts() {
        let unitParameter = $("#ShowValue.js-wf-showValue").val();
        $(".workforce-charts-list .chart").each(
            (i, el) => this.generateCharts(el, unitParameter)
        );
    }

    generateCharts(el, unitParameter) {
        let yValues = JSON.parse($('#' + el.id).attr('data-chart'));
        let minBy = _.minBy(yValues, (o) => o.amount);
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

    //setActiveTab() {
    //    debugger;
    //    $(".govuk-tabs__list-item").removeClass("govuk-tabs__list-item--selected");
    //    let tab = DfE.Util.QueryString.get('tab');
    //    switch (tab) {
    //        case "Expenditure":
    //        default:
    //            $("#ExpenditureTab").addClass("govuk-tabs__list-item--selected");
    //            break;
    //        case "Income":
    //            $("#IncomeTab").addClass("govuk-tabs__list-item--selected");
    //            break;
    //        case "Balance":
    //            $("#BalanceTab").addClass("govuk-tabs__list-item--selected");
    //            break;
    //        case "Workforce":
    //            $("#WorkforceTab").addClass("govuk-tabs__list-item--selected");
    //            break;
    //    }
    //}



}

