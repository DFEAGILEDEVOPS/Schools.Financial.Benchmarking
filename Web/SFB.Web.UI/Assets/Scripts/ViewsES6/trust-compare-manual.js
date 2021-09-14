"use strict";

class TrustCompareManualViewModel {

    constructor() {
        this.bindManualEvents();
        //GOVUK.Accordion.bindElements("SelectTrustAccordion");
        GOVUK.Modal.Load();
    }

    bindManualEvents() {

        this.bindAutosuggest('#NewTrustName', this.getTrustSuggestionHandler);

        $(".remove-trust").click((event) => {
            event.preventDefault();
            this.RemoveTrust($(event.target).data('companyno'));
            $(".error-summary").hide();
        });

        $("#displayNew").click((event) => {
            event.preventDefault();
            this.displayNewTrustElements();
        });


        $("#manualButton").click((event) => {
            if (!this.validate()) {
                event.preventDefault();
            }
        });

        $(".remove-new-trust").click((event) => {
            event.preventDefault();
            $("#NewTrust").hide();
            $("#AddButton").show();
            $(".error-summary").hide();
            $(".error-message").hide();
            document.title = document.title.replace("Error: ", "");
        });

    }

    validate() {
        let count = $(".remove-trust").length;
        if (count === 0 || $("#NewTrustName:visible").length > 0) {
            $(".error-summary").hide();
            $(".error-message").hide();
            if ($("#NewTrustName").val() === "") {
                $(".error-summary.missing").show();
                $(".error-message.missing").show();
            } else {
                $(".error-summary.not-found").show();
                $(".error-message.not-found").show();
            }
            $("#NewTrustName").addClass("form-control-error");
            $(".error-summary-list a").focus();
            document.title = "Error: " + document.title.replace("Error: ","");
            return false;
        } else {
            $(".error-summary").hide();
            $(".error-message").hide();
            $("#NewTrustName").removeClass("form-control-error");
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
                $.get("AddTrust?companyNo=" + suggestion[value] + "&matName=" + suggestion[field],
                    (data) => {
                        $("#TrustsToCompare").html(data);
                        this.bindManualEvents();
                        $("#AddButton a").focus();
                        $(".error-summary-list a").focus();
                    });

            });
    }

    RemoveTrust(companyNo) {
        $.get("RemoveTrust?companyNo=" + companyNo,
            (data) => {
                $("#TrustsToCompare").html(data);
                this.bindManualEvents();
            });
    }

    //RemoveAllTrusts() {
    //    $.get("RemoveAllTrusts",
    //        (data) => {
    //            $("#TrustsToCompare").html(data);
    //            this.bindManualEvents();
    //        });
    //}

    displayNewTrustElements() {
        $("#NewTrust").show();
        $("#NewTrustName").focus();
        $("#NewTrustName").removeClass("form-control-error");
        $("#AddButton").hide();
    }
}



