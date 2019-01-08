"use strict";

function _typeof(obj) { if (typeof Symbol === "function" && typeof Symbol.iterator === "symbol") { _typeof = function _typeof(obj) { return typeof obj; }; } else { _typeof = function _typeof(obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; }; } return _typeof(obj); }

(function (GOVUK, Views) {
  "use strict";

  function CriteriaByAreaViewModel(localAuthorities) {
    this.localAuthorities = localAuthorities;
    this.bindEvents();
  }

  CriteriaByAreaViewModel.prototype = {
    bindEvents: function bindEvents() {
      GOVUK.Accordion.bindElements("SearchTypesAccordion", this.accordionChangeHandler.bind(this));
      this.bindAutosuggest("#FindSchoolByLaName", "#SelectedLocalAuthorityId", {
        data: this.localAuthorities,
        name: "LANAME",
        value: "id"
      });
    },
    accordionChangeHandler: function accordionChangeHandler() {
      $(".error-summary").hide();
    },
    bindAutosuggest: function bindAutosuggest(targetInputElementName, targetResolvedInputElementName, suggestionSource) {
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
        return '<div><a href="javascript:">' + suggestion[field] + "</a></div>";
      };

      $(targetInputElementName).typeahead({
        hint: false,
        highlight: true,
        highlightAliases: [["st. ", "st ", "saint "]],
        minLength: minChars,
        classNames: {
          menu: "tt-menu form-control mtm",
          highlight: "bold-small"
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
      var currentSuggestionName = "";
      $(targetInputElementName).bind("typeahead:select", function (src, suggestion) {
        $(targetResolvedInputElementName).val(suggestion[value]);
        currentSuggestionName = suggestion[field];
        $("#FindSchoolByLaCode").val(suggestion["id"]);
        window.location = "/BenchmarkCriteria/AdvancedCharacteristics?areaType=LACode&lacode=" + suggestion["id"] + "&Urn=" + $("#Urn").val() + "&ComparisonType=" + $("#ComparisonType").val() + "&EstType=" + $("#EstType").val();
      });
      $(targetInputElementName).bind("typeahead:autocomplete", function (src, suggestion) {
        $(targetResolvedInputElementName).val(suggestion[value]);
        currentSuggestionName = suggestion[name];
      });
      $(targetInputElementName).bind("input propertychange", function (event) {
        // When the user changes the value in the search having already selected an item, ensure the selection resets
        var currentValue = $(event.target).val();

        if (currentValue !== currentSuggestionName) {
          $(targetResolvedInputElementName).val("");
        }
      });
    }
  };

  CriteriaByAreaViewModel.Load = function (localAuthorities) {
    new DfE.Views.CriteriaByAreaViewModel(localAuthorities);
  };

  Views.CriteriaByAreaViewModel = CriteriaByAreaViewModel;
})(GOVUK, DfE.Views);