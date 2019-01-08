"use strict";

function _typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

(function (GOVUK, Views) {
  'use strict';

  var jqxhr;
  var questionCheckBoxSelector = ".multiple-choice.question > input";
  var questionCheckBoxSelectorRadio = ".multiple-choice.questionRadio > input";

  function TrustCompareViewModel() {
    this.bindManualEvents();
    this.bindCriteriaEvents();
    this.validateForm();
    GOVUK.Accordion.bindElements("SelectTrustAccordion");
    GOVUK.Modal.Load();
  }

  TrustCompareViewModel.prototype = {
    validateForm: function validateForm() {
      $('#criteriaForm').validate({
        errorPlacement: function errorPlacement(error, element) {
          error.appendTo(element.closest(".question").find(".error-message"));
        },
        highlight: function highlight(element, errorClass, validClass) {
          $(element).addClass(errorClass).removeClass(validClass);
          $(element).closest(".panel").addClass("error");
        },
        unhighlight: function unhighlight(element, errorClass, validClass) {
          $(element).removeClass(errorClass).addClass(validClass);

          if ($(element).closest(".panel").find("input.error").length === 0) {
            $(element).closest(".panel").removeClass("error");
          }
        }
      });
    },
    bindCriteriaEvents: function bindCriteriaEvents() {
      var self = this;
      $(questionCheckBoxSelector).change(function (event) {
        var $panel = $(this).parent().next(".panel");
        $panel.toggle();

        if (!this.checked) {
          $panel.find("input").prop('disabled', true);
          $panel.removeClass("error");
          $panel.find("input.error").removeClass("error");
          $panel.find("label.error").css("display", "none");
        } else {
          $panel.find("input").prop('disabled', false);
        }

        $panel.find("input[type='number']:disabled").val(null);
        $panel.find("input[type='checkbox']:disabled").prop('checked', false);
        $panel.find("input[type='radio']:disabled").prop('checked', false);

        if ($(questionCheckBoxSelector + ":checked").length > 0) {
          $("#liveCountBar").show();
          $("#comparisonListInfoPanelResults").show();
          $("#comparisonListInfoPanelResultsEmpty").hide();
        } else {
          $("#liveCountBar").show();
          $("#comparisonListInfoPanelResultsEmpty").show();
          $("#comparisonListInfoPanelResults").hide();
        }

        if (!event.target.checked) {
          self.updateResultCount();
        }
      });
      $(questionCheckBoxSelectorRadio).change(function (event) {
        var $allpanels = $(this).parent().parent().children(".panel");

        if ($allpanels.length > 0) {
          $allpanels.hide();
          $allpanels.find("input").prop('disabled', true);
          $allpanels.find(".panel").hide();
          $allpanels.find("input[type='number']:disabled").val(null);
          $allpanels.find("input[type='checkbox']:disabled").prop('checked', false);
          $allpanels.find("input[type='radio']:disabled").prop('checked', false);
        }

        var $mypanel = $(this).parent().next(".panel");

        if ($mypanel.length > 0) {
          $mypanel.show();
          $mypanel.find("input").prop('disabled', false);
        }

        $("#liveCountBar").show();
        $("#comparisonListInfoPanelResults").show();
        $("#comparisonListInfoPanelResultsEmpty").hide();
        self.updateResultCount();
      });
      $("a#removeAllTrusts").click(function (event) {
        event.preventDefault();
        self.RemoveAllTrusts();
      });
    },
    bindManualEvents: function bindManualEvents() {
      var self = this;
      self.bindAutosuggest('#NewTrustName', this.getTrustSuggestionHandler);
      $("input.criteria-input").keyup(function (e) {
        var code = e.keyCode || e.which;

        if (code !== 9) {
          self.updateResultCount();
        }
      });
      $("input.criteria-checkbox").change(function () {
        self.updateResultCount();
      });
      $("input.criteria-radio").change(function () {
        self.updateResultCount();
      });
      $("button.submit").click(function (event) {
        event.preventDefault();
        self.checkResultCount();
      });
      $("a.remove-trust").click(function (event) {
        event.preventDefault();
        self.RemoveTrust($(event.target).data('matno'));
      });
      $("a#displayNew").click(function (event) {
        event.preventDefault();
        self.DisplayNewTrustElements();
      });
      $(".clear-criteria").click(function (event) {
        self.clear();
      });
      $("#EnterManually").click(function (event) {
        $("#liveCountBar").hide();
        $("button.submit").hide();
        $("#manualButton").show();
      });
      $("#EnterChars").click(function (event) {
        $("#liveCountBar").show();
        $("button.submit").show();
        $("#manualButton").hide();
      });
    },
    checkResultCount: function checkResultCount() {
      var self = this;
      var count = $("#schoolCount").text().substring(0, $("#schoolCount").text().indexOf(' '));

      if (count <= 20) {
        $("#criteriaForm").submit();
      } else {
        self.renderWarningModal(count);
      }
    },
    renderWarningModal: function renderWarningModal(resultCount) {
      var $body = $('body');
      var $page = $('#js-modal-page'); // insert code at the end

      var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title" aria-describedby="modal-content"><div role="document">' + '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' + '<h1 id="modal-title" class="modal-title">' + resultCount + ' matches found</h1>' + '<p id="modal-content"><br/>Please refine the characteristics entered until there are 20 or fewer matched trusts.</p>';
      $($modal_code).insertAfter($page);
      $body.addClass('no-scroll');
      $page.attr('aria-hidden', 'true'); // add overlay

      var $modal_overlay = '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';
      $($modal_overlay).insertAfter($('#js-modal'));
      $('#js-modal-close').focus();
    },
    clear: function clear() {
      $(questionCheckBoxSelector + ":checked").click();
    },
    updateResultCount: function updateResultCount() {
      if (jqxhr) {
        jqxhr.abort();
      }

      jqxhr = $.post("TrustComparison/GenerateCountFromManualCriteria", $('#criteriaForm').serialize()).done(function (count) {
        $("#schoolCount").text("Searching");
        setTimeout(function () {
          $("#schoolCount").text(count + " trusts found");
        }, 500);
        $("button.view-benchmark-charts").attr("aria-label", "View " + count + " trusts in a benchmark chart");
        $("#liveCountBar").show();

        if (count > 0) {
          $("button.submit").show();
          $("button.submit").removeAttr("disabled");
        } else {
          $("button.submit").hide();
          $("button.submit").attr("disabled", "disabled");
        }

        $('.sticky-div').Stickyfill();
      });
    },
    getTrustSuggestionHandler: function getTrustSuggestionHandler(keywords, callback) {
      var dataSuggestionUrl = $("#NewTrustName").attr("data-suggestion-url");
      return $.get(encodeURI(dataSuggestionUrl + '?name=' + keywords), function (response) {
        return callback(response.Matches);
      });
    },
    bindAutosuggest: function bindAutosuggest(targetInputElementName, suggestionSource) {
      var self = this;
      var field = "Text";
      var value = "Id";
      var source = null;
      var minChars = 0;

      if (typeof suggestionSource === "function") {
        // remote source
        minChars = 3;

        source = function source(query, syncResultsFn, asyncResultsFn) {
          return suggestionSource.call(self, query, asyncResultsFn);
        };
      } else if (_typeof(suggestionSource) === "object") {
        // local data source
        if (!suggestionSource.data) {
          console.log("suggestionSource.data is null");
          return;
        }

        if (!suggestionSource.name) {
          console.log("suggestionSource.name is null");
          return;
        }

        if (!suggestionSource.value) {
          console.log("suggestionSource.value is null");
          return;
        }

        console.log("suggestionSource.data has " + suggestionSource.data.length + " items");
        minChars = 2;
        field = suggestionSource.name;
        value = suggestionSource.value;
        source = new Bloodhound({
          datumTokenizer: function datumTokenizer(d) {
            return Bloodhound.tokenizers.whitespace(d[field]);
          },
          queryTokenizer: Bloodhound.tokenizers.whitespace,
          local: suggestionSource.data
        });
        source.initialize();
      } else {
        console.log("Incompatible suggestionSource");
        return;
      }

      var templateHandler = function templateHandler(suggestion) {
        return '<div><a href="javascript:">' + suggestion[field] + '</a></div>';
      };

      $(targetInputElementName).typeahead({
        hint: false,
        highlight: true,
        highlightAliases: [["st. ", "st ", "saint "]],
        minLength: minChars,
        classNames: {
          menu: 'tt-menu form-control mtm',
          highlight: 'bold-small'
        },
        ariaOwnsId: "arialist_" + DfE.Util.randomNumber()
      }, {
        display: field,
        limit: 10,
        source: source,
        templates: {
          suggestion: templateHandler
        }
      });
      $(targetInputElementName).bind("typeahead:select", function (src, suggestion) {
        $.get("trustcomparison/AddTrust?matNo=" + suggestion[value] + "&matName=" + suggestion[field], function (data) {
          $("#TrustsToCompare").html(data);
          self.bindManualEvents();
          $("#AddButton a").focus();
          $(".error-summary-list a").focus();
        });
      });
    },
    RemoveTrust: function RemoveTrust(matNo) {
      var self = this;
      $.get("trustcomparison/RemoveTrust?matNo=" + matNo, function (data) {
        $("#TrustsToCompare").html(data);
        self.bindManualEvents();
      });
    },
    RemoveAllTrusts: function RemoveAllTrusts() {
      var self = this;
      $.get("trustcomparison/RemoveAllTrusts", function (data) {
        $("#TrustsToCompare").html(data);
        self.bindManualEvents();
      });
    },
    DisplayNewTrustElements: function DisplayNewTrustElements() {
      $("#NewTrust").show();
      $("#NewTrustName").focus();
      $("#AddButton").hide();
    }
  };

  TrustCompareViewModel.Load = function () {
    new DfE.Views.TrustCompareViewModel();
  };

  Views.TrustCompareViewModel = TrustCompareViewModel;
})(GOVUK, DfE.Views);