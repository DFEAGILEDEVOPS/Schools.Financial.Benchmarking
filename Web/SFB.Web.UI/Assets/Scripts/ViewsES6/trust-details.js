class TrustDetailsViewModel {
    constructor(chartFormat) {

        sessionStorage.chartFormat = chartFormat;

        DfE.Views.HistoricalCharts = new HistoricalCharts();
        DfE.Views.HistoricalCharts.GenerateCharts();

        GOVUK.Modal.Load();

        new Accordion(document.getElementById('historical-charts-accordion'));

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
            sessionStorage.chartFormat;

        window.location = queryString;
    }
}