(function (GOVUK, Views) {
    "use strict";

    function CriteriaByAreaViewModel(localAuthorities) {
        this.localAuthorities = localAuthorities;
        this.bindEvents();
    }

    CriteriaByAreaViewModel.prototype = {

        bindEvents: function () {
            GOVUK.Accordion.bindElements("SearchTypesAccordion", this.accordionChangeHandler.bind(this));
            $("#FindCurrentPosition").click(this.findCurrentLocationHandler.bind(this));
            this.bindAutosuggest("#FindSchoolByLaName", "#SelectedLocalAuthorityId", { data: this.localAuthorities, name: "LANAME", value: "id" });
        },

        findCurrentLocationHandler: function (evt) {
            evt.preventDefault();
            if (navigator.geolocation) {
                $("#FindSchoolByPostcode").attr("placeholder", "Locating...");
                navigator.geolocation.getCurrentPosition(this.getCurrentPositionSuccessHandler, this.getCurrentPositionErrorHandler);
            } else this.getCurrentPositionErrorHandler(-1);
        },

        accordionChangeHandler: function () {
            $(".error-summary").hide();
        },

        getCurrentPositionErrorHandler: function (err) {

            DfE.Util.Analytics.TrackEvent("gps", "ERR_" + err.code, "auto-geolocation");

            var msg;
            switch (err.code) {
                case err.UNKNOWN_ERROR:
                    msg = "Unable to find your location";
                    break;
                case err.PERMISSION_DENIED:
                    msg = "Permission denied in finding your location";
                    break;
                case err.POSITION_UNAVAILABLE:
                    msg = "Your location is currently unknown";
                    break;
                case err.TIMEOUT:
                    msg = "Attempt to find location took too long";
                    break;
                default:
                    msg = "Location detection not supported in browser";
            }
            $("#FindSchoolByTown").attr("placeholder", msg);
        },

        getCurrentPositionSuccessHandler: function (position) {
            var coords = position.coords || position.coordinate || position;

            $("#LocationCoordinates").val(coords.latitude + "," + coords.longitude);
            $('#SearchByTownFieldset button[type="submit"]').removeAttr("disabled");

            $.getJSON("https://api.postcodes.io/postcodes?lat=" + coords.latitude + "&lon=" + coords.longitude + "&widesearch=true", function (data) {
                if (data.result) {
                    $("#FindSchoolByTown").val(data.result[0].postcode);
                    $("#FindSchoolByTown").attr("placeholder", "");
                }
            });

            DfE.Util.Analytics.TrackEvent("gps", "SUCCESS", "auto-geolocation");
        },

        bindAutosuggest: function (targetInputElementName, targetResolvedInputElementName, suggestionSource) {

            var field = "Text";
            var value = "Id";
            var source = null;
            var minChars = 0;

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

            var templateHandler = function (suggestion) { return '<div><a href="javascript:">' + suggestion[field] + "</a></div>"; };

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
                if (currentValue != currentSuggestionName) {
                    $(targetResolvedInputElementName).val("");
                }
            });
        }
    };

    CriteriaByAreaViewModel.Load = function (localAuthorities) {
        new DfE.Views.CriteriaByAreaViewModel(localAuthorities);
    };

    Views.CriteriaByAreaViewModel = CriteriaByAreaViewModel;

}(GOVUK, DfE.Views));