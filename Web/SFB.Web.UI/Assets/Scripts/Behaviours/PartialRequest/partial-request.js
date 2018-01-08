/*
 <div id="someId" class="js-collapsible" aria-expanded="true" aria-controls="collapseMe">
 
 </div>
 <div id="collapseMe" class="js-collapsed">

 </div>
 */
(function ($) {
  "use strict";
  window.GOVUK = window.GOVUK || {};

  function PartialRequest(options) {
    if (!options.elementId || options.elementId == "") {
      throw new ReferenceError("options.elementId is not present.");
    }

    this.renderElementId = options.elementId;
    if (!this.renderElement) {
      throw new ReferenceError("PartialRequest did not resolve renderElement using the id: " + options.elementId);
    }

    // loading indicator
    this.indicatorElementId = options.indicatorElementId;

    // set the refresh function
    this.onRefresh = options.onRefresh;

    // caches results per page request
    this.cache = {};
  }

  PartialRequest.prototype = {

    get renderElement() {
      return document.getElementById(this.renderElementId);
    },

    get indicatorElement() {
      return document.getElementById(this.indicatorElementId);
    },

    request: function (partialUrl) {
      if (this.cache.hasOwnProperty(partialUrl)) {
        this.displayResults(this.cache[partialUrl]);
      } else {
        var self = this;
        this.showLoadingIndicator();
        $.ajax({
          url: partialUrl
        }).done(function (responseHTML) {
          self.cache[partialUrl] = responseHTML;
          self.displayResults(responseHTML);
          self.clearLoadingIndicator();
        }).error(function (e) {
          self.showErrorIndicator(e);
        });
      }
    },
    displayResults: function (contentHTML) {
      this.renderElement.outerHTML = contentHTML;
      if (this.onRefresh) {
        this.onRefresh();
      }
    },
    showLoadingIndicator: function () {
      $(this.indicatorElement).html("Loading...");
    },
    clearLoadingIndicator: function () {
      $(this.indicatorElement).html("&nbsp;");
    },
    showErrorIndicator: function (e) {
      $(this.indicatorElement).html("An error has occured whilst trying to load the requested content.");
      throw e;
    }
  };

  GOVUK.PartialRequest = PartialRequest;

} (jQuery));