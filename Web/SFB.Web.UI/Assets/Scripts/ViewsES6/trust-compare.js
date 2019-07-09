class TrustCompareViewModel {

    constructor() {
        this.questionCheckBoxSelector = ".multiple-choice.question > input";
        this.questionCheckBoxSelectorRadio = ".multiple-choice.questionRadio > input";

        this.bindManualEvents();
        this.bindCriteriaEvents();
        this.validateForm();
        GOVUK.Accordion.bindElements("SelectTrustAccordion");
        GOVUK.Modal.Load();
    }

    validateForm() {
        $('#criteriaForm').
            validate({
                errorPlacement: function (error, element) {
                    error.appendTo(element.closest(".question").find(".error-message"));
                },
                highlight: function (element, errorClass, validClass) {
                    $(element).addClass(errorClass).removeClass(validClass);
                    $(element).closest(".panel").addClass("error");
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).removeClass(errorClass).addClass(validClass);
                    if ($(element).closest(".panel").find("input.error").length === 0) {
                        $(element).closest(".panel").removeClass("error");
                    }
                }
            });
    }

    bindCriteriaEvents() {

        $(this.questionCheckBoxSelector).change(
            (event) => {
                let $panel = $(event.target).parent().next(".panel");
                $panel.toggle();
                if (!event.target.checked) {
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
                if ($(this.questionCheckBoxSelector + ":checked").length > 0) {
                    $("#liveCountBar").show();
                    $("#comparisonListInfoPanelResults").show();
                    $("#comparisonListInfoPanelResultsEmpty").hide();
                } else {
                    $("#liveCountBar").show();
                    $("#comparisonListInfoPanelResultsEmpty").show();
                    $("#comparisonListInfoPanelResults").hide();
                }

                if (!event.target.checked) {
                    this.updateResultCount();
                }
            });

        $(this.questionCheckBoxSelectorRadio).change(
            (event) => {
                let $allpanels = $(this).parent().parent().children(".panel");
                if ($allpanels.length > 0) {
                    $allpanels.hide();
                    $allpanels.find("input").prop('disabled', true);
                    $allpanels.find(".panel").hide();
                    $allpanels.find("input[type='number']:disabled").val(null);
                    $allpanels.find("input[type='checkbox']:disabled").prop('checked', false);
                    $allpanels.find("input[type='radio']:disabled").prop('checked', false);
                }

                var $mypanel = $(event.target).parent().next(".panel");
                if ($mypanel.length > 0) {
                    $mypanel.show();
                    $mypanel.find("input").prop('disabled', false);
                }

                $("#liveCountBar").show();
                $("#comparisonListInfoPanelResults").show();
                $("#comparisonListInfoPanelResultsEmpty").hide();

                this.updateResultCount();

            });


        $("#removeAllTrusts").click(() => {
            this.RemoveAllTrusts();
        });
    }

    bindManualEvents() {

        this.bindAutosuggest('#NewTrustName', this.getTrustSuggestionHandler);

        $("input.criteria-input").keyup((e) => {
            let code = e.keyCode || e.which;
            if (code !== 9) {
                this.updateResultCount();
            }
        });

        $("input.criteria-checkbox").change(() => {
            this.updateResultCount();
        });

        $("input.criteria-radio").change(() => {
            this.updateResultCount();
        });

        $("button.submit").click((event) => {
            event.preventDefault();
            this.checkResultCount();
        });

        $(".remove-trust").click((event) => {
            event.preventDefault();
            this.RemoveTrust($(event.target).data('companyno'));
        });

        $("#displayNew").click((event) => {
            event.preventDefault();
            this.DisplayNewTrustElements();
        });

        $(".js-clear-criteria").click(() => {
            this.clear();
        });

        $("#EnterManually").click(() => {
            $("#liveCountBar").hide();
            $("button.submit").hide();
            $("#manualButton").show();
        });

        $("#EnterChars").click(() => {
            $("#liveCountBar").show();
            $("button.submit").show();
            $("#manualButton").hide();
        });

    }

    checkResultCount() {
        let count = $("#schoolCount").text().substring(0, $("#schoolCount").text().indexOf(' '));
        if (count <= 20) {
            $("#criteriaForm").submit();
        } else {
            this.renderWarningModal(count);
        }
    }

    renderWarningModal(resultCount) {
        let $body = $('body');
        let $page = $('#js-modal-page');

        // insert code at the end
        let $modal_code =
            '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title" aria-describedby="modal-content"><div role="document">' +
            '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
            '<h1 id="modal-title" class="modal-title">' + resultCount + ' matches found</h1>' +
            '<p id="modal-content"><br/>Please refine the characteristics entered until there are 20 or fewer matched trusts.</p>';

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        let $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();

    }

    clear() {
        $(this.questionCheckBoxSelector + ":checked").click();
    }

    updateResultCount() {
        if (this.jqxhr) {
            this.jqxhr.abort();
        }
        this.jqxhr = $.post("TrustComparison/GenerateCountFromAdvancedCriteria", $('#criteriaForm').serialize())
            .done(function (count) {
                $("#schoolCount").text("Searching");
                setTimeout(function () { $("#schoolCount").text(count + " trusts found"); }, 500);
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
    }
    
    getTrustSuggestionHandler(keywords, callback) {
        let dataSuggestionUrl = $("#NewTrustName").attr("data-suggestion-url");
        return $.get(encodeURI(dataSuggestionUrl + '?name=' + keywords),
            function (response) {
                return callback(response.Matches);
            });
    }
    
    bindAutosuggest(targetInputElementName, suggestionSource) {
        let field = "Text";
        let value = "Id";
        let source = null;
        let minChars = 0;

        if (typeof suggestionSource === "function") { // remote source
            minChars = 3;
            source = (query, syncResultsFn, asyncResultsFn) => {
                return suggestionSource.call(this, query, asyncResultsFn);
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
            (src, suggestion)  => {
                $.get("trustcomparison/AddTrust?companyNo=" + suggestion[value] + "&matName=" + suggestion[field],
                    (data) => {
                        $("#TrustsToCompare").html(data);
                        this.bindManualEvents();
                        $("#AddButton a").focus();
                        $(".error-summary-list a").focus();
                    });

            });
    }

    RemoveTrust(companyNo) {
        $.get("trustcomparison/RemoveTrust?companyNo=" + companyNo,
            (data) => {
                $("#TrustsToCompare").html(data);
                this.bindManualEvents();
            });
    }

    RemoveAllTrusts() {
        $.get("trustcomparison/RemoveAllTrusts",
            (data) => {
                $("#TrustsToCompare").html(data);
                this.bindManualEvents();
            });
    }

    DisplayNewTrustElements() {
        $("#NewTrust").show();
        $("#NewTrustName").focus();
        $("#AddButton").hide();
    }
}



