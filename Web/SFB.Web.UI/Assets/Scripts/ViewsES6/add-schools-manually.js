"use strict";

class AddSchoolsManuallyViewModel {

    constructor() {
        GOVUK.Modal.Load();
        this.bindManualEvents();
        $("#manualButton").click((event) => {
            if (!this.validate()) {
                event.preventDefault();
            } else {
                window.open('OverwriteStrategy', '_self');
            }
        });
    }

    bindManualEvents() {

        this.bindAutosuggest('#NewSchoolName', this.getSchoolSuggestionHandler);

        $(".remove-school").click((event) => {
            event.preventDefault();
            this.removeSchool($(event.target).data('urn'));
        });


        $(".remove-new-school").click((event) => {
            event.preventDefault();
            $("#NewSchool").hide();
            $("#AddButton").show();
            $(".govuk-error-summary").hide();
            $(".govuk-error-message").hide();
        });

        $("#displayNew").click((event) => {
            event.preventDefault();
            this.displayNewSchoolElements();            
        });
    }

    getSchoolSuggestionHandler(keywords, callback) {
        let dataSuggestionUrl = $("#NewSchoolName").attr("data-suggestion-url");
        let openOnly = $("#openOnly").val();
        return $.get(encodeURI(dataSuggestionUrl + '?nameId=' + keywords + '&openOnly=' + openOnly),
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
            return '<div><a class="govuk-link" href="javascript:">' + suggestion[field] + '</a></div>';
        };

        $(targetInputElementName).typeahead({
            hint: false,
            highlight: true,
            highlightAliases: [
                ["st. ", "st ", "saint "]
            ],
            minLength: minChars,
            classNames: {
                menu: 'tt-menu govuk-form-control mtm',
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
            (src, suggestion) => {
                $.get("/manualcomparison/AddSchool?urn=" + suggestion[value],
                    (data) => {
                        $("#SchoolsToAdd").html(data);
                        this.bindManualEvents();
                        $("#AddButton a").focus();
                        $(".govuk-error-summary.missing").hide();
                        $(".govuk-error-summary.not-found").hide();
                    });
            });
    }

    removeSchool(urn) {
        $.get("/manualcomparison/RemoveSchool?urn=" + urn,
            (data) => {
                $("#SchoolsToAdd").html(data);
                $(".govuk-error-summary").hide();
                this.bindManualEvents();
            });
    }

    removeAllSchools() {
        $.get("/manualcomparison/RemoveAllSchools",
            (data) => {
                $("#SchoolsToAdd").html(data);
                this.bindManualEvents();
            });
    }

    displayNewSchoolElements() {
        $("#NewSchool").show();
        $("#NewSchoolName").focus();
        $("#NewSchoolName").removeClass("govuk-input--error");
        $("#AddButton").hide();
    }

    validate() {
        let count = $("#schoolCount").val();
        if (count === 0 || $("#NewSchoolName:visible").length > 0) {
            $(".govuk-error-summary").hide();
            $(".govuk-error-message").hide();
            if ($("#NewSchoolName").val() === "") {
                $(".govuk-error-summary.missing").show();
                $(".govuk-error-message.missing").show();
            } else {
                $(".govuk-error-summary.not-found").show();
                $(".govuk-error-message.not-found").show();
            }
            $("#NewSchoolName").addClass("govuk-input--error");
            $(".govuk-error-summary__list a").focus();
            return false;
        } else {
            $(".govuk-error-summary").hide();
            $(".govuk-error-message").hide();
            $("#NewSchoolName").removeClass("govuk-input--error");
            return true;
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
            '<p id="modal-content">Please refine the characteristics entered until there are 20 or fewer matched trusts.</p>';

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        let $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();

    }
}



