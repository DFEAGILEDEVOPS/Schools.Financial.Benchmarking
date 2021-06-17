"use strict";

class CriteriaByAreaViewModel {

    constructor(localAuthorities) {
        this.localAuthorities = localAuthorities;
        this.bindEvents();
    }

    bindEvents() {
        GOVUK.Accordion.bindElements("SearchTypesAccordion", this.accordionChangeHandler.bind(this));
        this.bindAutosuggest("#FindSchoolByLaCode", "#SelectedLocalAuthorityId", { data: this.localAuthorities, name: "LANAME", value: "id" });
    }

    accordionChangeHandler() {
        $(".error-summary").hide();
    }

    bindAutosuggest(targetInputElementName, targetResolvedInputElementName, suggestionSource) {

        let field = "Text";
        let value = "Id";
        let source = null;
        let minChars = 0;

        if (typeof (suggestionSource) === "function") { // remote source
            minChars = 3;
            source = function (query, syncResultsFn, asyncResultsFn) {
                return suggestionSource.call(self, query, asyncResultsFn);
            };
        } else if (typeof (suggestionSource) === "object") { // local data source

            if (!suggestionSource.data) { console.log("suggestionSource.data is null"); return; }
            if (!suggestionSource.name) { console.log("suggestionSource.name is null"); return; }
            if (!suggestionSource.value) { console.log("suggestionSource.value is null"); return; }
            console.log("suggestionSource.data has " + suggestionSource.data.length + " items");

            minChars = 2;
            field = suggestionSource.name;
            value = suggestionSource.value;

            source = new Bloodhound({
                datumTokenizer: function (d) { return Bloodhound.tokenizers.whitespace(d[field]); },
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: suggestionSource.data
            });
            source.initialize();
        } else {
            console.log("Incompatible suggestionSource");
            return;
        }

        let templateHandler = function (suggestion) {
            return '<div><a href="javascript:">' + suggestion[field] + "</a></div>";
        };

        $(targetInputElementName).typeahead({
            hint: false,
            highlight: true,
            highlightAliases: [
                ["st. ", "st ", "saint "]
            ],
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

        let currentSuggestionName = "";

        $(targetInputElementName).bind("typeahead:select", (src, suggestion) => {
            $(targetResolvedInputElementName).val(suggestion[value]);
            currentSuggestionName = suggestion[field];
            $("#FindSchoolByLaCode").val(suggestion["id"]);
            window.location = `/BenchmarkCriteria/AdvancedCharacteristics?areaType=LaCodeName&lacodename=${suggestion["id"]}&Urn=${$("#Urn").val()}`
                + `&ComparisonType=${$("#ComparisonType").val()}&EstType=${$("#EstType").val()}&ExcludePartial=${$("#ExcludePartial").val()}`;
        });

        $(targetInputElementName).bind("typeahead:autocomplete", function (src, suggestion) {
            $(targetResolvedInputElementName).val(suggestion[value]);
            currentSuggestionName = suggestion[name];
        });

        $(targetInputElementName).bind("input propertychange", function (event) {
            // When the user changes the value in the search having already selected an item, ensure the selection resets
            let currentValue = $(event.target).val();
            if (currentValue !== currentSuggestionName) {
                $(targetResolvedInputElementName).val("");
            }
        });
    }
}

