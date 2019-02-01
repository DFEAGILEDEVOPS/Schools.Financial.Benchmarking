class AdvancedCharacteristicsViewModel {

    constructor() {

        this.jqxhr = null;
        this.questionCheckBoxSelector = ".multiple-choice.question > input";

        new Accordion(document.getElementById('characteristics-accordion'));

        this.validateForm();

        this.bindEvents();

        this.updateResultCount();
        this.updateAllCounters();

        GOVUK.Modal.Load();
    }


    validateForm() {
        $('#criteriaForm').
            validate({
                errorPlacement: (error, element) => {
                    error.appendTo(element.closest(".question").find(".error-message"));
                },
                highlight: (element, errorClass, validClass) => {
                    $(element).addClass(errorClass).removeClass(validClass);
                    $(element).closest(".panel").addClass("error");
                },
                unhighlight: (element, errorClass, validClass) => {
                    $(element).removeClass(errorClass).addClass(validClass);
                    if ($(element).closest(".panel").find("input.error").length === 0) {
                        $(element).closest(".panel").removeClass("error");
                    }
                }
            });
    }

    updateResultCount() {
        if (this.jqxhr) {
            this.jqxhr.abort();
        }
        this.jqxhr = $.post("GenerateCountFromManualCriteria", $('#criteriaForm').serialize())
            .done((count) => {
                $("#schoolCount").text("Searching");
                setTimeout(() => {
                    $("#schoolCount").text(count + " schools found");
                }, 500);
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
    }


    updateCounter(element) {
        var $counterElement = $(element).parents(".accordion-section").find(".selection-counter");
        var count = $counterElement.text();
        if ($(element).is(":checked")) {
            $counterElement.text(++count);
        } else {
            $counterElement.text(--count);
        }
    }

    updateAllCounters() {
        var $counterElements = $(".accordion-section-header .selection-counter");
        $counterElements.each(function () {
            var count = $(this).parents(".accordion-section").find("div.multiple-choice.question input:checked").length;
            $(this).text(count);
        });
    }

    checkResultCount() {
        let count = $("#schoolCount").text().substring(0, $("#schoolCount").text().indexOf(' '));
        if (count <= 30) {
            $("#criteriaForm").submit();
        } else {
            this.renderWarningModal(count);
        }
    }

    clear() {
        $(this.questionCheckBoxSelector + ":checked").click();
    }


    renderWarningModal(resultCount) {
        let $body = $('body');
        let $page = $('#js-modal-page');

        // insert code at the end
        let $modal_code =
            '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title" aria-describedby="modal-content"><div role="document">' +
            '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
            '<h1 id="modal-title" class="modal-title">' + resultCount + ' matches found</h1>' +
            '<p id="modal-content"><br/>Please refine the characteristics entered until there are 30 or fewer matched schools.</p>';

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        let $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();

    }

    bindEvents() {
        $(this.questionCheckBoxSelector).change(
            (event) => {
                let $panel = $(event.target).parent().next();
                $panel.toggle();
                $panel.find("input").prop('disabled', (i, v) => { return !v; });
                if (!event.target.checked) {
                    $panel.removeClass("error");
                    $panel.find("input.error").removeClass("error");
                    $panel.find("label.error").css("display", "none");
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

                this.updateCounter(event.target);
                if (!event.target.checked) {
                    this.updateResultCount();
                }
            });

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

        $(".clear-criteria").click(() => {
            this.clear();
        });

    }
}


