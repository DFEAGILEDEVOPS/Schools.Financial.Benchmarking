(function(GOVUK, Views) {
    'use strict';

    function BenchmarkListViewModel() {}

    BenchmarkListViewModel.UpdateBenchmarkBasket = function (urn, withAction) {
        $.get("/benchmarklist/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
            function (data) {
                switch (withAction) {
                    case "makedefaultbenchmark":
                        $(".add:hidden").show();
                        $(".remove:visible").hide();
                        $("div[data-urn='" + urn + "']>.add-remove").toggle();
                        $(".name").removeClass("highlight");
                        $("#doc_" + urn + " .name").addClass("highlight");
                        break;
                    case "removedefaultbenchmark":
                        $("div[data-urn='" + urn + "']>.add-remove").toggle();
                        $("#doc_" + urn + " .name").removeClass("highlight");
                        break;
                    case "removefromcompare":
                        $("#doc_" + urn).hide();
                        $("#benchmarkBasketControlsPlaceHolder").html(data);
                        break;
                    case "clear":
                        $(".document").hide();
                        $("#benchmarkBasketControlsPlaceHolder").html(data);
                        break;
                }
                if ($(".document:visible").length < 2) {
                    $(".view-button-bottom").hide();
                }
            });
    };

    BenchmarkListViewModel.Load = function () {
        new DfE.Views.BenchmarkListViewModel();
    };

    Views.BenchmarkListViewModel = BenchmarkListViewModel;
}(GOVUK, DfE.Views));