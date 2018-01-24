(function (GOVUK, Views) {
    'use strict';

    var jqxhr;
    var questionCheckBoxSelector = ".multiple-choice.question > input";

    function AdvancedCharacteristicsViewModel() {

        new Accordion(document.getElementById('characteristics-accordion'));

        this.validateForm();

        this.bindEvents();

        this.updateResultCount();
        this.updateAllCounters();
    }

    AdvancedCharacteristicsViewModel.prototype = {

        validateForm: function () {
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
        },

        updateResultCount: function () {
            if (jqxhr) {
                jqxhr.abort();
            }
            jqxhr = $.post("GenerateCountFromManualCriteria", $('#criteriaForm').serialize())
                .done(function (count) {
                    $("#schoolCount").text(count);
                    $("#liveCountBar").show();
                    if (count > 0) {
                        $("button.submit").show();
                    } else {
                        $("button.submit").hide();
                    }
                    $('.sticky-div').Stickyfill();
                });
        },

        updateCounter: function (element) {
            var $counterElement = $(element).parents(".accordion-section").find(".selection-counter");
            var count = $counterElement.text();
            if ($(element).is(":checked")) {
                $counterElement.text(++count);
            } else {
                $counterElement.text(--count);
            }
        },

        updateAllCounters: function () {
            var $counterElements = $(".accordion-section-header .selection-counter");
            $counterElements.each(function () {
                var count = $(this).parents(".accordion-section").find("div.multiple-choice.question input:checked").length;
                $(this).text(count);
            }
            );
        },

        checkResultCount: function () {
            var self = this;
            var count = $("#schoolCount").text();
            if (count <= 30) {
                $("#criteriaForm").submit();
            } else {
                self.renderWarningModal(count);
            }
        },

        clear: function () {
            $(questionCheckBoxSelector + ":checked").click();
        },

        renderWarningModal: function (resultCount) {
            var $body = $('body');
            var $page = $('#js-modal-page');

            // insert code at the end
            var $modal_code =
                '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title"><div role="document">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">' +
                resultCount +
                ' matches found</h1><p id="modal-content"><br/>' +
                'Please refine the characteristics entered until there are 30 or fewer matched schools.</p>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },

        bindEvents: function () {

            var self = this;

            $(questionCheckBoxSelector).change(event,
                function () {
                    var $panel = $(this).parent().next();
                    $panel.toggle();
                    $panel.find("input").prop('disabled', function (i, v) { return !v; });
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
                    self.updateResultCount();
                });

            $("input.criteria-input").keyup(function () {
                if ($(this).valid()) {
                    self.updateResultCount();
                }
            });

            $("input.criteria-checkbox").change(function () {
                if ($(this).valid()) {
                    self.updateResultCount();
                }
            });

            $("input.criteria-radio").change(function () {
                if ($(this).valid()) {
                    self.updateResultCount();
                }
            });

            $("button.submit").click(function (event) {
                event.preventDefault();
                self.checkResultCount();
            });

            $(".clear-criteria").click(function (event) {
                self.clear();
            });
            
        }
    };

    AdvancedCharacteristicsViewModel.Load = function () {
        new DfE.Views.AdvancedCharacteristicsViewModel();
    };

    Views.AdvancedCharacteristicsViewModel = AdvancedCharacteristicsViewModel;

}(GOVUK, DfE.Views));