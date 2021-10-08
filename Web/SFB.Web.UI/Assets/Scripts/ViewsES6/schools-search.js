"use strict";

class SchoolsSearchViewModel {

    constructor(localAuthorities, azureMapsAPIKey) {
        this.localAuthorities = localAuthorities;

        atlas.setSubscriptionKey(azureMapsAPIKey);
        this.azureMapsClient = new atlas.service.Client(atlas.getSubscriptionKey());

        this.bindEvents();

        this.setTab();
    }

    setTab() {
        let searchType = DfE.Util.QueryString.get('searchtype');
        switch (searchType) {
            case "search-by-trust-location":
            case "search-by-trust-name-id":
            case "search-by-trust-la-code-name":
                $("#TrustTab a")[0].click();
                break;
            case "compare-without-default-school":
                $("#NoDefaultTab a")[0].click();
                break;
        }
    }

    bindEvents() {
        $('#FindCurrentPosition').click(this.findCurrentLocationHandler.bind(this));
        $('#openOnlyName').change(() => $('#FindByNameId').typeahead('val', ''));
        this.bindAutosuggest('#FindByNameId', '#FindByNameIdSuggestionId', this.getSchoolsSuggestionHandler);
        this.bindAutosuggest('#FindByTrustName', '#FindByTrustNameSuggestionId', this.getTrustSuggestionHandler);
        this.bindAutosuggest('#FindSchoolByTown', '#LocationCoordinates', this.getLocationResultsHandler.bind(this));
        this.bindAutosuggest('#FindTrustByTown', '#LocationCoordinatesForTrust', this.getLocationResultsHandler.bind(this));
        this.bindAutosuggest('#FindSchoolByLaCodeName', '#SelectedLocalAuthorityId', { data: this.localAuthorities, name: "LANAME", value: "id" });
        this.bindAutosuggest('#FindTrustByLaCodeName', '#SelectedLocalAuthorityIdTrust', { data: this.localAuthorities, name: "LANAME", value: "id" });
        this.bindAutosuggest('#FindSchoolManuallyByTown', '#LocationCoordinates', this.getLocationResultsHandler.bind(this));
        this.bindAutosuggest('#FindSchoolManuallyByLaCodeName', '#SelectedLocalAuthorityId', { data: this.localAuthorities, name: "LANAME", value: "id" });
        this.bindEnterKeysToButtons();
    }

    bindEnterKeysToButtons() {
        let inputs = $("#finderSection input[type ='text']");
        inputs.keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                $(e.target).closest(".govuk-form-group").find("button").click();
            }
        });
    }

    findCurrentLocationHandler(evt) {
        evt.preventDefault();
        if (navigator.geolocation) {
            $('#FindSchoolByPostcode').attr("placeholder", "Locating...");
            navigator.geolocation.getCurrentPosition(this.getCurrentPositionSuccessHandler, this.getCurrentPositionErrorHandler);
        } else this.getCurrentPositionErrorHandler(-1);
    }

    getSchoolsSuggestionHandler(keywords, callback) {
        let dataSuggestionUrl = $("#FindByNameId").attr("data-suggestion-url");
        let openSchoolsOnly = $("#FindByNameId").parents('.govuk-form-group').first().find("input:checkbox[name='openOnly']").prop('checked');
        return $.get(encodeURI(`${dataSuggestionUrl}?nameId=${keywords}&openonly=${openSchoolsOnly}`), function (response) {
            return callback(response.Matches);
        });
    }

    getTrustSuggestionHandler(keywords, callback) {
        let dataSuggestionUrl = $("#FindByTrustName").attr("data-suggestion-url");
        return $.get(encodeURI(`${dataSuggestionUrl}?name=${keywords}`), function (response) {
            return callback(response.Matches);
        });
    }

    getCurrentPositionErrorHandler(err) {
        let msg;
        switch (err.code) {
            case err.UNKNOWN_ERROR:
                msg = "Unable to find your location";
                break;
            case err.PERMISSION_DENIED:
                msg = "Your location is blocked by your browser, enter your location manually";
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
        var html = `<div class="error-summary" role="alert" aria-labelledby="ErrorSummaryHeading">
                <h1 id = "ErrorSummaryHeading" class="heading-medium error-summary-heading">
                There is a problem
                </h1>
                <ul class="error-summary-list">
                    <li>
                        <a id="error-msg" href="#finderSection">${msg}</a>
                    </li>
                </ul>
                </div>`;
        $("#error-summary-placeholder").empty();
        $("#error-summary-placeholder").append(html);
        $(".location-error-message-placeholder").empty();
        $(".location-error-message-placeholder").append(`<span class="error-message">${msg}</span>`);
    }

    getCurrentPositionSuccessHandler(position) {
        let coords = position.coords || position.coordinate || position;

        $("#error-summary-placeholder").empty();
        $(".location-error-message-placeholder").empty();
        $('#LocationCoordinates').val(coords.latitude + ',' + coords.longitude);
        $('#SearchByTownFieldset button[type="submit"]').removeAttr('disabled');

        $.getJSON(`https://api.postcodes.io/postcodes?lat=${coords.latitude}&lon=${coords.longitude}&widesearch=true`, function (data) {
            if (data.result) {
                $('#FindSchoolByTown').val(data.result[0].postcode);
                $('#FindSchoolManuallyByTown').val(data.result[0].postcode);
                $('#FindSchoolByTown').attr("placeholder", "");
                $('#FindSchoolManuallyByTown').attr("placeholder", "");
            }
        });
    }

    getLocationResultsHandler(keywords, callback) {
        this.azureMapsClient.search.getSearchAddress(keywords, {
            typeahead: true,
            countrySet: ['GB'],
            limit: 10
        }).then(function (response) {
            let suggestions = [];
            if (response && response.results !== null && response.results.length > 0) {
                var geojsonResponse = new atlas.service.geojson.GeoJsonSearchResponse(response);
                var results = geojsonResponse.getGeoJsonResults();

                suggestions = results.features.filter(function (r) { return r.properties.address.countrySubdivision === "ENG"; }).map(function (r) {
                    var obj = {};
                    var output = "";

                    if (r.properties.type !== "Street" && !r.properties.address.postalCode && r.properties.address.municipalitySubdivision) {
                        output = r.properties.address.municipalitySubdivision + ', ' + r.properties.address.municipality;
                    }
                    else {
                        output = r.properties.address.freeformAddress;
                    }

                    output += ', ' + r.properties.address.countrySecondarySubdivision;

                    obj.Text = output;
                    obj.Location = r.properties.position.lat + ',' + r.properties.position.lon;
                    return obj;
                });
            }

            return callback(suggestions);
        });
    }

    bindAutosuggest(targetInputElementName, targetResolvedInputElementName, suggestionSource) {

        let field = "Text";
        let value = "Id";
        let source = null;
        let minChars = 0;

        if (typeof suggestionSource === "function") { // remote source
            minChars = 3;
            source = function (query, syncResultsFn, asyncResultsFn) {
                return suggestionSource.call(self, query, asyncResultsFn);
            };
        } else if (typeof suggestionSource === "object") { // local data source

            if (!suggestionSource.data) { console.log("suggestionSource.data is null"); return; }
            if (!suggestionSource.name) { console.log("suggestionSource.name is null"); return; }
            if (!suggestionSource.value) { console.log("suggestionSource.value is null"); return; }

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

        var templateHandler = function (suggestion) { return '<div><a href="javascript:">' + suggestion[field] + '</a></div>'; };

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
        }, {
            display: field,
            limit: 10,
            source: source,
            templates: {
                suggestion: templateHandler
            }
        });

        let currentSuggestionName = "";

        $(targetInputElementName).bind("typeahead:select", function (src, suggestion) {
            $(targetResolvedInputElementName).val(suggestion[value]);
            currentSuggestionName = suggestion[field];
            var openSchoolsOnly = $(this).parents('.govuk-form-group').first().find("input:checkbox[name='openOnly']").prop('checked');
            var textBoxId = $(this).attr('id');
            let url = '';
            switch (textBoxId) {
                case 'FindSchoolByTown':
                    $('#LocationCoordinates').val(suggestion['Location']);
                    break;
                case 'FindTrustByTown':
                    $('#LocationCoordinatesForTrust').val(suggestion['Location']);
                    break;
                case 'FindSchoolManuallyByTown':
                    $('#LocationCoordinates').val(suggestion['Location']);
                    break;
            }
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
}





