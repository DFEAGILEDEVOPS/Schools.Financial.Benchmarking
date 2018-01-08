(function ($) {
    "use strict";
    window.GOVUK = window.GOVUK || {};

    // class Modal
    function Modal() {
        this.init();
        this.bindEvents();
    }

    // class instance methods
    Modal.prototype = {
        bindEvents: function () {
            //$("body").on("click", ".js-modal", this.renderAccessibleModal.bind(this));
            $("body").on("click", "#js-modal-close", this.closeAccessibleModal.bind(this));
            $("body").on("click", "#js-modal-overlay", this.updateClickFocus.bind(this));
            $("body").on("keydown", "#js-modal-overlay", this.updateKeydownFocus.bind(this));
            $("body").on("keydown", "#js-modal", this.keyDownCloseAccessibleModal.bind(this));
            $("body").on("focus", "#js-modal-tabindex", this.focusTabIndex.bind(this));
        },
        closeAccessibleModal: function (event) {
            var $this = $(this),
             $focus_back = '#' + $this.attr('data-focus-back'),
             $js_modal = $('#js-modal'),
             $js_modal_overlay = $('#js-modal-overlay'),
             $body = $('body'),
             $page = $('#js-modal-page');
             
            $page.removeAttr('aria-hidden');
            $body.removeClass('no-scroll');
            $js_modal.remove();
            $js_modal_overlay.remove();
            $($focus_back).focus();
            event.preventDefault();
        },
        focusTabIndex : function(event) {
            $('#js-modal-close').focus();
        },
        init: function () {
            //if ($(".js-modal").length) { // if there are at least one :)
                $(".js-modal").each(function (index_to_expand) {
                    var $this = $(this),
                        index_lisible = index_to_expand + 1;

                    $this.attr({
                        'id': 'label_modal_' + index_lisible
                    });
                });

                if ($("#js-modal-page").length === 0) {
                    $("body").wrapInner('<div id="js-modal-page"></div>');
                }
            //}
        },
        keyDownCloseAccessibleModal: function(event) {
            var $this = $(this);

            if (event.keyCode === 9) { // tab
                event.preventDefault();
                return;
            } 

            if (event.keyCode === 27) { // esc
                var $focus_back = '#' + $('#js-modal-close').attr('data-focus-back'),
                    $js_modal = $('#js-modal'),
                    $body = $('body'),
                    $js_modal_overlay = $('#js-modal-overlay'),
                    $page = $('#js-modal-page');

                $page.removeAttr('aria-hidden');
                $body.removeClass('no-scroll');
                $js_modal.remove();
                $js_modal_overlay.remove();
                $($focus_back).focus();
            }
            if (event.keyCode == 9) { // tab or maj+tab

                // get list of all children elements in given object
                var children = $this.find('*');

                // get list of focusable items
                this.focusableElementsString = "a[href], area[href], input:not([disabled]), button:not([disabled])";
                var focusableItems = children.filter(focusableElementsString).filter(':visible');

                // get currently focused item
                var focusedItem = $(document.activeElement);

                // get the number of focusable items
                var numberOfFocusableItems = focusableItems.length;

                var focusedItemIndex = focusableItems.index(focusedItem);

                if (!event.shiftKey && (focusedItemIndex == numberOfFocusableItems - 1)) {
                    focusableItems.get(0).focus();
                    event.preventDefault();
                }
                if (event.shiftKey && focusedItemIndex == 0) {
                    focusableItems.get(numberOfFocusableItems - 1).focus();
                    event.preventDefault();
                }
            }
        },
        renderAccessibleModal: function (event) {
            // Re-initialise as we may have AJAXed in a new page
            this.init();
            var $this = $(this);
            var options = $this.data();
            var $modal_starter_id = $(event.target).parent().attr('id');
            var $modal_prefix_classes = typeof options.modalPrefixClass !== 'undefined'
                ? options.modalPrefixClass + '-'
                : '';
            var $modal_text = $(event.target).parent().data('modal-text');
            var $modal_content_id = typeof options.modalContentId !== 'undefined' ? '#' + options.modalContentId : '';
            var $modal_title = $(event.target).parent().data('modal-title');
            var $modal_close_text = $(event.target).parent().data('modal-close-text');
            var $modal_close_title = $(event.target).parent().data('modal-close-title');
            var $help_text_key = $(event.target).parent().data('help-text-key');
            var $modal_background_click = options.modalBackgroundClick || '';
            var $modal_code;
            var $modal_overlay;
            var $body = $('body');
            var $page = $('#js-modal-page');

            // insert code at the end
            $modal_code = '<dialog id="js-modal" class="' + $modal_prefix_classes + 'modal" role="dialog" aria-labelledby="modal-title"><div role="document">';
            $modal_code += '<a href="#" id="js-modal-close" class="' + $modal_prefix_classes + 'modal-close" data-focus-back="' + $modal_starter_id +
                '" title="' + $modal_close_title + '">' + $modal_close_text + "</a><br/>";
            if ($modal_title !== '') {
                $modal_code += '<h1 id="modal-title" class="' + $modal_prefix_classes + 'modal-title">' + $modal_title + '</h1>';
            }
            if ($modal_text !== '') {
                $modal_text = $modal_text.replace(/[\n]/g, '<br />');
                $modal_code += '<p id="modal-content">' + $modal_text + '</p>';
            }
            else {
                if ($modal_content_id !== '' && $($modal_content_id).length) {
                    $modal_code += $($modal_content_id).html();
                }
            }

            $modal_code += '</div></dialog>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            if ($modal_background_click != 'disabled') {
                $modal_overlay = '<span id="js-modal-overlay" class="' + $modal_prefix_classes + 'modal-overlay" title="' + $modal_close_title +
                    '" data-background-click="enabled"><span class="invisible">Close modal</span></span>';
            }
            else { $modal_overlay = '<span id="js-modal-overlay" class="' + $modal_prefix_classes + 'modal-overlay" data-background-click="disabled"></span>'; }

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

            event.preventDefault();

        },
        updateClickFocus: function(event) {
            var $focus_back = '#' + $('#js-modal-close').attr('data-focus-back'),
             $js_modal = $('#js-modal'),
             $js_modal_overlay = $('#js-modal-overlay'),
             $modal_background_click = $js_modal_overlay.attr('data-background-click'),
             $body = $('body'),
             $page = $('#js-modal-page');

            if ($modal_background_click == 'enabled') {
                $page.removeAttr('aria-hidden');
                $body.removeClass('no-scroll');
                $js_modal.remove();
                $js_modal_overlay.remove();
                $($focus_back).focus();
            }
        },
        updateKeydownFocus: function (event) {
            // Space or Enter
            if (event.keyCode == 13 || event.keyCode == 32) {

                var $focus_back = '#' + $('#js-modal-close').attr('data-focus-back'),
                    $js_modal = $('#js-modal'),
                    $body = $('body'),
                    $js_modal_overlay = $('#js-modal-overlay'),
                    $modal_background_click = $js_modal_overlay.attr('data-background-click'),
                    $page = $('#js-modal-page');

                if ($modal_background_click == 'enabled') {
                    $page.removeAttr('aria-hidden');
                    $body.removeClass('no-scroll');
                    $js_modal.remove();
                    $js_modal_overlay.remove();
                    $($focus_back).focus();
                }
            }
        }
    };

    Modal.Load = function () {
        new GOVUK.Modal();
    };

    GOVUK.Modal = Modal;

}(jQuery));
