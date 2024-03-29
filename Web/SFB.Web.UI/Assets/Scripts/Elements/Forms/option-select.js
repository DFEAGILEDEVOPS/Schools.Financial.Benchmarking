(function ($) {
  "use strict";
  window.GOVUK = window.GOVUK || {};

  function OptionSelect(options){
    /* This JavaScript provides two functional enhancements to option-select components:
     1) A count that shows how many results have been checked in the option-container
     2) Open/closing of the list of checkboxes - this is not provided for ie6 and 7 as the performance is too janky.
     */

    this.$optionSelect = options.$el;
    this.$options = this.$optionSelect.find("input[type='checkbox']");
    this.$labels = this.$optionSelect.find("label");
    this.$optionsContainer = this.$optionSelect.find('.options-container');
    this.$optionList = this.$optionsContainer.children('.js-auto-height-inner');

    this.attachCheckedCounter();

    // Performance in ie 6/7 is not good enough to support animating the opening/closing
    // so do not allow option-selects to be collapsible in this case
    var allowCollapsible = (typeof ieVersion == "undefined" || ieVersion > 7) ? true : false;
    if(allowCollapsible){

      // Attach listener to update checked count
      this.$options.on('click', this.updateCheckedCount.bind(this));

      // Replace div.container-head with a button **COMMENTED OUT: Because the focus gets on the button instead of the whole div and div does not get highlighted #8955
      //this.replaceHeadWithButton();

      // Add js-collapsible class to parent for CSS
      this.$optionSelect.addClass('js-collapsible');

      // Add open/close listeners
      this.$optionSelect.find('.js-container-head').on('click', this.toggleOptionSelect.bind(this));

      this.$optionSelect.keypress(this.checkForSpecialKeys.bind(this));

      // Add a listener to the checkboxes so if you navigate to them with the keyboard you can definitely see them
      this.$options.on('focus', this.open.bind(this));

      if (this.$optionSelect.data('closed-on-load') == true) {
        this.close();
      }
    }
  }

  OptionSelect.prototype.replaceHeadWithButton = function replaceHeadWithButton(){
    /* Replace the div at the head with a button element. This is based on feedback from Leonie Watson.
     * The button has all of the accessibility hooks that are used by screen readers and etc.
     * We do this in the JavaScript because if the JavaScript is not active then the button shouldn't
     * be there as there is no JS to handle the click event.
     */
    var $containerHead = this.$optionSelect.find('.js-container-head');
    var jsContainerHeadHTML = $containerHead.html();

    // Create button and replace the preexisting html with the button.
    var $button = $('<button>');
    $button.addClass('js-container-head');
    $button.attr('aria-expanded', this.isClosed());
    $button.attr('aria-controls', this.$optionSelect.find('.options-container').attr('id'));
    $button.html(jsContainerHeadHTML);
    $containerHead.replaceWith($button);

  };

  OptionSelect.prototype.attachCheckedCounter = function attachCheckedCounter(){
      if (this.$options.filter(":checked").length > 0) {
          this.updateCheckedCount();
      }
  };

  OptionSelect.prototype.updateCheckedCount = function updateCheckedCount(){
    //this.$optionSelect.find('.js-selected-counter').text(this.checkedString());
  };

  OptionSelect.prototype.checkedString = function checkedString(){
    var count = this.$options.filter(":checked").length;
    var checkedString = "";
    if (count > 0){
      checkedString = count+" selected";
    }

    return checkedString;
  };


  OptionSelect.prototype.toggleOptionSelect = function toggleOptionSelect(e) {
    this.$optionSelect.focus();
    if (this.isClosed()) {
      this.open();
    } else {
      this.close();
    }
  };

  OptionSelect.prototype.open = function open(){
      if (this.isClosed()) {
          this.$optionSelect.attr('aria-expanded', true);
      this.$optionSelect.removeClass('js-closed');
      if (!this.$optionsContainer.prop('style').height) {
        this.setupHeight();
      }
    }
  };

  OptionSelect.prototype.close = function close(){
    this.$optionSelect.addClass('js-closed');
    this.$optionSelect.attr('aria-expanded', false);
  };

  OptionSelect.prototype.isClosed = function isClosed(){
    return this.$optionSelect.hasClass('js-closed');
  };

  OptionSelect.prototype.setContainerHeight = function setContainerHeight(height) {
    this.$optionsContainer.css({
      'max-height': 'none', // Have to clear the 'max-height' set by the CSS in order for 'height' to be applied
      'height': height
    });
  };

  OptionSelect.prototype.isLabelVisible = function isLabelVisible(index, option){
    var $label = $(option);
    var initialOptionContainerHeight = this.$optionsContainer.height();
    var optionListOffsetTop = this.$optionList.offset().top;
    var distanceFromTopOfContainer = $label.offset().top - optionListOffsetTop;
    return distanceFromTopOfContainer < initialOptionContainerHeight;
  };

  OptionSelect.prototype.getVisibleLabels = function getVisibleLabels(){
    return this.$labels.filter(this.isLabelVisible.bind(this));
  };

  OptionSelect.prototype.setupHeight = function setupHeight(){
    var height = this.$optionList.height();
    this.setContainerHeight(height + 5);

  };

    OptionSelect.prototype.checkForSpecialKeys = function checkForSpecialKeys(e) {
        if (e.target.nodeName == "DIV") {
            if (e.keyCode == 13 || e.keyCode == 32) {
                this.toggleOptionSelect();
                e.preventDefault();
            }
        }
    };

  GOVUK.OptionSelect = OptionSelect;
})(jQuery);