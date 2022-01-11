class ChartWrapper {
  selectGrouping(grouping, parentGrouping) {
    $("#ChartGroup").val(grouping);
    $("#ChartGroup").change();
    if ($("#financialSummary")[0]) {
      $("#financialSummary")[0].scrollIntoView();
    }
    $("#ChartGroup").focus();
    $(".back-to-main-chart-group-button .js-parent-group").text(parentGrouping);
    $(".back-to-main-chart-group-button").show();
  }

  resetGrouping() {
    $('#ChartGroup').prop('selectedIndex', 0);
    $("#ChartGroup").change();
    $(".back-to-main-chart-group-button").hide();
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

  updateTrustWarnings() {
    let isPlaceholder = $("#isPlaceholder").val();
    if (isPlaceholder === "true") {
      $("#placeholderWarning").show();
    } else {
      $("#placeholderWarning").hide();
    }
  }

  updateTotals() {
    let expTotal = $("#expTotal").val();
    let expTotalAbbr = $("#expTotalAbbr").val();
    let incTotal = $("#incTotal").val();
    let incTotalAbbr = $("#incTotalAbbr").val();
    let balTotal = $("#balTotal").val();
    let balTotalAbbr = $("#balTotalAbbr").val();

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
  }

  updateBenchmarkBasket(urn, withAction) {
    if (withAction === "Add") {
      if (DfE.Util.ComparisonList.count() === 30) {
        DfE.Util.ComparisonList.renderFullListWarningModal();
        return;
      }
    }

    $.get("/school/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
      (data) => {
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
            $(".add-remove-js").toggle();
            break;
        }
      });
  }
}