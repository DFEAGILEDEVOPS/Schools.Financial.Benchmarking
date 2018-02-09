(function(GOVUK, Views) {
    'use strict';

    function TrustCompareViewModel() {
        this.bindEvents();
    }

    TrustCompareViewModel.prototype = {
        bindEvents: function() {
            this.bindAutosuggest('#NewTrustName', this.getTrustSuggestionHandler);
        },

        getTrustSuggestionHandler: function(keywords, callback) {
            var dataSuggestionUrl = $("#NewTrustName").attr("data-suggestion-url");
            return $.get(encodeURI(dataSuggestionUrl + '?name=' + keywords),
                function(response) {
                    return callback(response.Matches);
                });
        },

        bindAutosuggest: function(targetInputElementName, suggestionSource) {

            var field = "Text";
            var value = "Id";
            var source = null;
            var minChars = 0;

            if (typeof suggestionSource === "function") { // remote source
                minChars = 3;
                source = function(query, syncResultsFn, asyncResultsFn) {
                    return suggestionSource.call(self, query, asyncResultsFn);
                };
            } else if (typeof suggestionSource === "object") { // local data source

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
                    datumTokenizer: function(d) { return Bloodhound.tokenizers.whitespace(d[field]); },
                    queryTokenizer: Bloodhound.tokenizers.whitespace,
                    local: suggestionSource.data
                });
                source.initialize();
            } else {
                console.log("Incompatible suggestionSource");
                return;
            }

            var templateHandler = function(suggestion) {
                return '<div><a href="javascript:">' + suggestion[field] + '</a></div>';
            };

            $(targetInputElementName).typeahead({
                    hint: false,
                    highlight: true,
                    highlightAliases: [
                        ["st. ", "st ", "saint "]
                    ],
                    minLength: minChars,
                    classNames: {
                        menu: 'tt-menu form-control mtm',
                        highlight: 'bold-small'
                    },
                    ariaOwnsId: "arialist_" + DfE.Util.randomNumber()
                },
                {
                    display: field,
                    limit: 10,
                    source: source,
                    templates: {
                        suggestion: templateHandler
                    }
                });

            $(targetInputElementName).bind("typeahead:select",
                function (src, suggestion) {
                    $.get("trustcomparison/AddTrust?matNo=" + suggestion[value] + "&matName=" + suggestion[field],
                        function (data) {
                            $("#TrustsToCompare").html(data);
                            DfE.Views.TrustCompareViewModel.Load();
                            $("#AddButton a").focus();
                            $(".error-summary-list a").focus();
                        });

                });
        }

    };

    TrustCompareViewModel.Load = function() {
        new DfE.Views.TrustCompareViewModel();
    };
    
    TrustCompareViewModel.RemoveTrust = function (matNo) {
        $.get("trustcomparison/RemoveTrust?matNo=" + matNo,
            function (data) {
                $("#TrustsToCompare").html(data);
                DfE.Views.TrustCompareViewModel.Load();
            });
    }

    TrustCompareViewModel.RemoveAllTrusts = function () {
        $.get("trustcomparison/RemoveAllTrusts",
            function (data) {
                $("#TrustsToCompare").html(data);
                DfE.Views.TrustCompareViewModel.Load();
            });
    }

    TrustCompareViewModel.DisplayNewTrustElements = function () {
        $("#NewTrust").show();
        $("#NewTrustName").focus();
        $("#AddButton").hide();
    }

    Views.TrustCompareViewModel = TrustCompareViewModel;

}(GOVUK, DfE.Views));