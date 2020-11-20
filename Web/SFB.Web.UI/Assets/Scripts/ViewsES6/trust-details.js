class TrustDetailsViewModel {
    constructor(chartFormat) {

        sessionStorage.chartFormat = chartFormat;

        DfE.Views.HistoricalCharts = new HistoricalCharts();
        DfE.Views.HistoricalCharts.GenerateCharts();

        GOVUK.Modal.Load();

        new Accordion(document.getElementById('historical-charts-accordion'));
        new Accordion(document.getElementById('controls-accordion'));
        new Accordion(document.getElementById('schools-in-trust-accordion'));

        $(document).ready(function () {
            var tab = DfE.Util.QueryString.get('tab');
            if (tab) {
                $("a:contains('" + tab + "')").focus();
            }
        });
    }

    PrintPage() {
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

    TabChange(code, companyNo, name, tab) {
        let queryString = "?code=" +
            code +
            "&companyNo=" +
            companyNo +
            "&name=" +
            name +
            "&tab=" +
            tab +
            "&unit=" +
            $("select#ShowValue option:selected")[0].value +
            "&financing=" +
            $("select#Financing option:selected")[0].value +
            "&format=" +
            sessionStorage.chartFormat +
            '#financialSummary';

        window.location = queryString;
    }

    ToggleChartsTables(mode) {
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