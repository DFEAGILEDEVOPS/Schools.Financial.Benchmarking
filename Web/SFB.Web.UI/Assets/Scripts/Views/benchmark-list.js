(function(GOVUK, Views) {
    'use strict';

    function BenchmarkListViewModel() {}

    BenchmarkListViewModel.UpdateBenchmarkBasket = function (urn, withAction) {
        $.get("/benchmarklist/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
            function (data) {
                switch (withAction) {
                    case "SetDefault":
                        $(".add:hidden").show();
                        $(".remove:visible").hide();
                        $("div[data-urn='" + urn + "']>.add-remove").toggle();
                        $(".name").removeClass("highlight");
                        $("#doc_" + urn + " .name").addClass("highlight");
                        break;
                    case "UnsetDefault":
                        $("div[data-urn='" + urn + "']>.add-remove").toggle();
                        $("#doc_" + urn + " .name").removeClass("highlight");
                        break;
                    case "Remove":
                        $("#doc_" + urn).hide();
                        $("#benchmarkBasketControlsPlaceHolder").html(data);
                        break;
                    case "RemoveAll":
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