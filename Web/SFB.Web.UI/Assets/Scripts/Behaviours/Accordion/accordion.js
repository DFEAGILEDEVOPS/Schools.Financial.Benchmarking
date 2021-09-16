/*
 <div id="someId" class="js-accordion" aria-expanded="false" aria-controls="collapseMe">
 
 </div>
 <div id="collapseMe" class="js-collapsed">

 </div>
 */
(function ($) {
    "use strict";
    window.GOVUK = window.GOVUK || {};

    // class Accordion
    function Accordion(options) {
        GOVUK.Collapsible.call(this, options);

        if (!options.accordionGroupId)
            throw new ReferenceError("options.accordionGroupId is not present.");
        this.accordionGroupId = options.accordionGroupId;
        this.accordionChangeHandler = options.accordionChangeHandler;

        if (!this.accordionGroupElement)
            throw new ReferenceError("accordionGroupElement did not resolve using the id: " + options.accordionGroupId);
    }

    // class instance methods
    Accordion.prototype = Object.create(GOVUK.Collapsible.prototype, {
        accordionGroupElement: {
            get: function () {
                return document.getElementById(this.accordionGroupId);
            }
        },
        expanderClickHandler: {
            value: function (e) {
                // don't do anything if we're already expanded
                if (!this.isClosed()) return;

                if (this.accordionChangeHandler) this.accordionChangeHandler();

                // collapse any expanded items in the group
                var callingCollapseId = this.collapseId;
                var accordionGroupId = this.accordionGroupId;
                var $accordionElements = $('.js-accordion.selected', this.accordionGroupElement);
                Array.prototype.forEach.call($accordionElements, function (element) {
                    if (element.id == callingCollapseId) return;
                    // ensure any aria-controls are updated correctly using the Collapsible.setExpanded method
                    new GOVUK.Collapsible({ elementId: element.id, accordionGroupId: accordionGroupId, ignoreEvents: true }).setExpanded(false);
                });

                // expand this item
                GOVUK.Collapsible.prototype.setExpanded.call(this, true);

                // set focus to a control underneath expanded element       
                var link = $(this.getCollapsibleElement()).next().find('.focus-first');
                if (link.length > 0) {
                    setTimeout(function () { link.focus(); }, 50);
                } else {
                    var input = $(this.getCollapsibleElement()).next().find('input');
                    setTimeout(function () { input.first().focus(); }, 50);
                }
            }
        }
    });

    Accordion.prototype.replaceHeadWithButton = function () { };

    // class static methods
    Accordion.bindElements = function (accordionGroupId, accordionChangeHandler) {
        var accordionGroupElement = document.getElementById(accordionGroupId);
        var $accordionElements = $(".js-accordion", accordionGroupElement);

        Array.prototype.forEach.call($accordionElements, function (element) {
            new Accordion({ elementId: element.id, accordionGroupId: accordionGroupId, accordionChangeHandler: accordionChangeHandler });
        });
    };

    GOVUK.Accordion = Accordion;

}(jQuery));

