(function (GOVUK, Views) {
    'use strict';
    var azureMapsClient;

    function SchoolsSearchViewModel(localAuthorities, azureMapsAPIKey) {
        atlas.setSubscriptionKey(azureMapsAPIKey);
        azureMapsClient = new atlas.service.Client(atlas.getSubscriptionKey());

        this.localAuthorities = localAuthorities;
        this.bindEvents();
    }

    SchoolsSearchViewModel.prototype = {

        bindEvents: function () {
            GOVUK.Accordion.bindElements("SearchTypesAccordion", this.accordionChangeHandler.bind(this));
            $('#FindCurrentPosition').click(this.findCurrentLocationHandler.bind(this));
            this.bindAutosuggest('#FindByNameId', '#FindByNameIdSuggestionId', this.getSchoolsSuggestionHandler);
            this.bindAutosuggest('#FindByTrustName', '#FindByTrustNameSuggestionId', this.getTrustSuggestionHandler);
            this.bindAutosuggest('#FindSchoolByTown', '#LocationCoordinates', this.getLocationResultsHandler);
            this.bindAutosuggest('#FindSchoolByLaCodeName', '#SelectedLocalAuthorityId', { data: this.localAuthorities, name: "LANAME", value: "id" });
            this.bindEnterKeysToButtons();
            this.bindAccordionHeaderClick();
        },

        bindAccordionHeaderClick: function () {
            var inputs = $("#SearchTypesAccordion .js-accordion");
            inputs.click(function (event) {
                $("input:checkbox[name='openOnly']").prop('disabled', true);
                $(event.currentTarget).next().find("input:checkbox[name='openOnly']").prop('disabled', false);
            });
        },

        bindEnterKeysToButtons: function () {
            var inputs = $("#finderSection input[type ='text']");
            inputs.keypress(function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $(e.target).closest(".form-group").find("button").click();
                }
            });
        },

        findCurrentLocationHandler: function (evt) {
            evt.preventDefault();
            if (navigator.geolocation) {
                $('#FindSchoolByPostcode').attr("placeholder", "Locating...");
                navigator.geolocation.getCurrentPosition(this.getCurrentPositionSuccessHandler, this.getCurrentPositionErrorHandler);
            } else this.getCurrentPositionErrorHandler(-1);
        },

        accordionChangeHandler: function () {
            $(".error-summary").hide();
        },

        getSchoolsSuggestionHandler: function (keywords, callback) {
            var dataSuggestionUrl = $("#FindByNameId").attr("data-suggestion-url");
            return $.get(encodeURI(dataSuggestionUrl + '?nameId=' + keywords), function (response) {
                return callback(response.Matches);
            });
        },

        getTrustSuggestionHandler: function (keywords, callback) {
            var dataSuggestionUrl = $("#FindByTrustName").attr("data-suggestion-url");
            return $.get(encodeURI(dataSuggestionUrl + '?name=' + keywords), function (response) {
                return callback(response.Matches);
            });
        },

        getCurrentPositionErrorHandler: function (err) {

            DfE.Util.Analytics.TrackEvent('gps', "ERR_" + err.code, 'auto-geolocation');

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
            $('#FindSchoolByTown').attr("placeholder", msg);
        },

        getCurrentPositionSuccessHandler: function (position) {
            var coords = position.coords || position.coordinate || position;

            $('#LocationCoordinates').val(coords.latitude + ',' + coords.longitude);
            $('#SearchByTownFieldset button[type="submit"]').removeAttr('disabled');

            $.getJSON('https://api.postcodes.io/postcodes?lat=' + coords.latitude + '&lon=' + coords.longitude + '&widesearch=true', function (data) {
                if (data.result) {
                    $('#FindSchoolByTown').val(data.result[0].postcode);
                    $('#FindSchoolByTown').attr("placeholder", "");
                }
            });

            DfE.Util.Analytics.TrackEvent('gps', "SUCCESS", 'auto-geolocation');
        },

        getLocationResultsHandler: function (keywords, callback) {
            azureMapsClient.search.getSearchAddress(keywords, {
                typeahead: true,
                countrySet: ['GB'],
                limit: 10
            }).then(function (response) {
                var suggestions = [];
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
        },

        bindAutosuggest: function (targetInputElementName, targetResolvedInputElementName, suggestionSource) {

            var field = "Text";
            var value = "Id";
            var source = null;
            var minChars = 0;

            if (typeof suggestionSource === "function") { // remote source
                minChars = 3;
                source = function (query, syncResultsFn, asyncResultsFn) {
                    return suggestionSource.call(self, query, asyncResultsFn);
                };
            } else if (typeof suggestionSource === "object") { // local data source

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

            var currentSuggestionName = "";

            $(targetInputElementName).bind("typeahead:select", function (src, suggestion) {
                $(targetResolvedInputElementName).val(suggestion[value]);
                currentSuggestionName = suggestion[field];
                var openSchoolsOnly = $(this).parent().parent().find("input:checkbox[name='openOnly']").prop('checked');
                var textBoxId = $(this).attr('id');
                if (textBoxId === 'FindByNameId') {
                    window.location = '/school/detail?urn=' + suggestion['Id'];
                }
                else if (textBoxId === 'FindSchoolByLaCodeName') {
                    // convert it to an la code search, which is the same as if they'd submitted.
                    var url = '/schoolsearch/search?searchType=search-by-la-code-name&laCodeName=' + suggestion['id'];
                    if (openSchoolsOnly) {
                        url += '&openOnly=true';
                    }
                    window.location = url;
                }
                else if (textBoxId === 'FindByTrustName') {
                    window.location = '/trust/index?matno=' + suggestion['Id'] + '&name=' + suggestion['Text'];
                } else if (textBoxId === 'FindSchoolByTown') {
                    $('#LocationCoordinates').val(suggestion['Location']);                    
                    url = '/SchoolSearch/Search?searchtype=search-by-location&LocationCoordinates=' + suggestion['Location'] + '&locationorpostcode=' + suggestion['Text'];
                    if (openSchoolsOnly) {
                        url += '&openOnly=true';
                    }
                    window.location = url;
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
    };

    SchoolsSearchViewModel.Load = function (localAuthorities, azureMapsAPIKey) {
        new DfE.Views.SchoolsSearchViewModel(localAuthorities, azureMapsAPIKey);
    };

    Views.SchoolsSearchViewModel = SchoolsSearchViewModel;

}(GOVUK, DfE.Views));