class TrustCompareAdvancedViewModel {

    constructor() {
        this.questionCheckBoxSelector = ".multiple-choice.question > input";
        this.questionCheckBoxSelectorRadio = ".multiple-choice.questionRadio > input";
        this.questionNumberInputSelector = ".panel input.criteria-input";

        this.bindCriteriaEvents();
        this.validateForm();
        GOVUK.Accordion.bindElements("SelectTrustAccordion");
        GOVUK.Modal.Load();
        this.updateResultCount();
    }

    validateForm() {
        jQuery.extend(jQuery.validator.messages, {
            max: jQuery.validator.format("Enter a value less than or equal to {0}"),
            min: jQuery.validator.format("Enter a value greater than or equal to {0}")
        });

        $.validator.methods.maxGreaterThanMin = (value, element) => {
            let minValue = $(element.parentNode.parentNode).find(".min-js").val();
            return minValue === "" || parseFloat(value) >= parseFloat(minValue);
        };

        $.validator.methods.minLowerThanMax = (value, element) => {
            let maxValue = $(element.parentNode.parentNode).find(".max-js").val();
            return maxValue === "" || parseFloat(value) <= parseFloat(maxValue);
        };

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
                },
                errorElement: "span"
            });

        $(".max-js").each((index, element) => $(element).rules("add", {
            maxGreaterThanMin: ""
        }));

        $(".min-js").each((index, element) => $(element).rules("add", {
            minLowerThanMax: ""
        }));
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
                    let $innerPanel = $panel.find(".panel");
                    $innerPanel.hide();
                } else {
                    $panel.find("input").prop('disabled', false);
                }
                $panel.find("input[type='number']:disabled").val(null);
                $panel.find("input[type='checkbox']:disabled").prop('checked', false);
                $panel.find("input[type='radio']:disabled").prop('checked', false);

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

                this.updateResultCount();

            });

        $(this.questionNumberInputSelector).keyup((e) => {
            let code = e.keyCode || e.which;
            if (code !== 9) {
                this.updateResultCount();
            }
        });

    }

    onClear(event) {
        $(this.questionCheckBoxSelector + ":checked").click();
        event.preventDefault();
    }

    onSubmit(event) {

        let count = $("#schoolCount").text().substring(0, $("#schoolCount").text().indexOf(' '));
        if (count > 0 && count <= 20) {
            $("#criteriaForm").submit();
        } else {
            this.renderWarningModal(count);
            event.preventDefault();
        }
    }

    renderWarningModal(resultCount) {
        let $body = $('body');
        let $page = $('#js-modal-page');

        // insert code at the end
        let $modal_code =
            '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title" aria-describedby="modal-content"><div role="document">' +
            '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="sbmt_button" title="Close">Close</a>' +
            '<h1 id="modal-title" class="modal-title">' + resultCount + ' matches found</h1>' +
            '<p id="modal-content">Refine the characteristics entered until there are between 1 and 20 matched trusts.</p>';

        $($modal_code).insertAfter($page);
        $body.addClass('no-scroll');

        $page.attr('aria-hidden', 'true');

        // add overlay
        let $modal_overlay =
            '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

        $($modal_overlay).insertAfter($('#js-modal'));

        $('#js-modal-close').focus();

    }

    updateResultCount() {
        if (this.jqxhr) {
            this.jqxhr.abort();
        }
        $("#schoolCount").html(`<img style="vertical-align:bottom; height: 25px" src="../public/assets/images/spinner.gif" alt="Loading" /><span style="margin-left: 10px; color: black">Searching</span>`);
        this.jqxhr = $.post("GenerateCountFromAdvancedCriteria", $('#criteriaForm').serialize())
            .done(function (count) {
                setTimeout(function () {
                    $("#schoolCount").html(`<span class="bold-small">${count}</span><span> trusts found</span >`);                    
                }, 500);
                    
                    $("button.view-benchmark-charts").attr("aria-label", "View " + count + " trusts in a benchmark chart");
                    $('.sticky-div').Stickyfill();
            });
    }      

}



