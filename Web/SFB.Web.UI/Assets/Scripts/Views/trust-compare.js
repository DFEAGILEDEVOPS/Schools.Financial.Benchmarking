(function(GOVUK, Views) {
    'use strict';

    function TrustCompareViewModel() {
        this.bindEvents();
    }

    TrustCompareViewModel.prototype = {
        bindEvents: function() {
            this.bindAutosuggest('#NewTrustName', this.getTrustSuggestionHandler);

            var questionCheckBoxSelector = ".multiple-choice.question > input";
            $(questionCheckBoxSelector).change(
                function (event) {
                    var $panel = $(this).parent().next();
                    $panel.toggle();
                    $panel.find("input").prop('disabled', function (i, v) { return !v; });
                    if (!this.checked) {
                        $panel.removeClass("error");
                        $panel.find("input.error").removeClass("error");
                        $panel.find("label.error").css("display", "none");
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

                    self.updateCounter(this);
                    if (!event.target.checked) {
                        self.updateResultCount();
                    }
                });

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

            $(".clear-criteria").click(function (event) {
                self.clear();
            });

        },

        checkResultCount: function () {
            var self = this;
            var count = $("#schoolCount").text().substring(0, $("#schoolCount").text().indexOf(' '));
            if (count <= 30) {
                $("#criteriaForm").submit();
            } else {
                self.renderWarningModal(count);
            }
        },

        renderWarningModal: function (resultCount) {
            var $body = $('body');
            var $page = $('#js-modal-page');

            // insert code at the end
            var $modal_code =
                '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title" aria-describedby="modal-content"><div role="document">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">' + resultCount + ' matches found</h1>' +
                '<p id="modal-content"><br/>Please refine the characteristics entered until there are 30 or fewer matched schools.</p>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },

        updateResultCount: function () {
            if (jqxhr) {
                jqxhr.abort();
            }
            jqxhr = $.post("GenerateCountFromManualCriteria", $('#criteriaForm').serialize())
                .done(function (count) {
                    $("#schoolCount").text("Searching");
                    setTimeout(function () { $("#schoolCount").text(count + " schools found"); }, 500);
                    $("button.view-benchmark-charts").attr("aria-label", "View " + count + " schools in a benchmark chart");
                    $("#liveCountBar").show();
                    if (count > 0) {
                        $("button.submit").show();
                        $("button.submit").removeAttr("disabled");
                        //if (count <= 30) {
                        //    $('button.submit.view-benchmark-charts').focus();
                        //}
                    } else {
                        $("button.submit").hide();
                        $("button.submit").attr("disabled", "disabled");
                    }
                    $('.sticky-div').Stickyfill();
                });
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