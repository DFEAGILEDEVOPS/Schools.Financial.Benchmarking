(function () {
    "use strict";

    window.GOVUK = window.GOVUK || {};

    function LiveSearch(options) {
        this.state = false;
        this.previousState = false;
        this.resultCache = {};
        this.disabled = false;

        this.onRefresh = options.onRefresh;
        this.onFormChange = options.onFormChange;
        this.onDisplayResults = options.onDisplayResults;
        this.resultsViewModel = options.resultsViewModel || {};

        this.formId = options.formId;
        this.secondaryFormId = options.secondaryFormId;
        this.$form = $("#" + options.formId);
        this.$secondaryForm = $("#" + options.secondaryFormId);
        this.$resultsBlock = options.$results;
        this.action = this.$form.attr('action') + '-js';
        this.$atomAutodiscoveryLink = options.$atomAutodiscoveryLink;
        this.getSummaryContainerBlock = function () { return $('#js-search-results-info div.result-info'); };
        this.getSummaryBlock = function () { return $('#js-search-results-info div.result-info p.summary'); };

        if (GOVUK.support.history()) {
          this.saveState();
          this.bindEvents();
        } else {
            this.$form.find('.js-live-search-fallback').show();
        }
    }

    LiveSearch.prototype.updateSchoolCount = function () {
        $("span.screen-reader-result-count").html("Filtering results");
        setTimeout(function () {
            $("span.screen-reader-result-count").html($('.result-info .summary').html());
        }, 1000);
    };

    LiveSearch.prototype.bindEvents = function bindEvents(state) {
      this.$form = $("#" + this.formId);
      this.$secondaryForm = $("#" + this.secondaryFormId);
      this.$form.on('change', 'input[type=checkbox], input[type=text], input[type=radio]', this.formChange.bind(this));
      this.$form.on('change', 'select', this.distanceChange.bind(this));
      //this.$secondaryForm.on('change', 'select, input[type=checkbox], input[type=text], input[type=radio]', this.formChange.bind(this));
      $('.pagination a').on('click', this.pageChange.bind(this));
      //$(window).on('popstate', this.popState.bind(this));
      if (this.onRefresh) {
        this.onRefresh();
      }
    };

    LiveSearch.prototype.saveState = function saveState(state) {
        if (typeof state === 'undefined') {
            state = this.$form.serializeArray();
        }
        this.previousState = this.state;
        this.state = state;
    };

    LiveSearch.prototype.popState = function popState(event) {
        if (event.originalEvent.state) {
            var tab = event.originalEvent.state.tab;
            if (!tab) tab = "list";
            if (tab !== this.currentTab) {
                if (this.resultsViewModel.changeTab) this.resultsViewModel.changeTab(tab, true);
            } else if (event.originalEvent.state) {
                this.saveState(event.originalEvent.state.payload);
                this.updateResults();
                this.restoreBooleans();
                this.restoreTextInputs();
            }
        } else {
            this.saveState();
            this.updateResults();
            this.restoreBooleans();
            this.restoreTextInputs();
            if (this.resultsViewModel.changeTab) this.resultsViewModel.changeTab(this.resultsViewModel.initialTabName, true);
        }
    };

    LiveSearch.prototype.formChange = function formChange(e) {
        if (this.onFormChange) this.onFormChange(this.$form.serialize());
        var elementIdSelector = e ? '#' + e.currentTarget.id : null;
        var pageUpdated;
        if (this.isNewState()) {
            this.saveState();
            pageUpdated = this.updateResults();
            pageUpdated.done(
              function () {
                  history.pushState({ payload: this.state, tab: this.resultsViewModel.currentTabName }, '', window.location.pathname
                        + "?" + $.param(this.state));
                  if (elementIdSelector) $(elementIdSelector).focus();
              }.bind(this)
            );
        }

        if (e && e.preventDefault) {
            e.preventDefault();
        }
    };

    LiveSearch.prototype.distanceChange = function formChange(e) {
        $('form#SearchFacetsForm input:checked').click();
        this.saveState();
        history.pushState({ payload: this.state, tab: this.resultsViewModel.currentTabName }, '', window.location.pathname + "?" + $.param(this.state) + "&tab=" + this.resultsViewModel.currentTabName);
        location.reload();
    };

    LiveSearch.prototype.pageChange = function pageChange(e) {

        if (e && e.preventDefault) {
            e.preventDefault();
        }

        if (this.onFormChange) this.onFormChange(this.$form.serialize());
        var elementIdSelector = e ? '#' + e.currentTarget.id : null;
        var pageUpdated;
        var state = this.$form.serializeArray();
        state.push({name: 'page', value: getURLParameter(e.target.href, 'page')});
        this.saveState(state);
        pageUpdated = this.updateResults();
        pageUpdated.done(
            function () {
                history.pushState({ payload: this.state, tab: this.resultsViewModel.currentTabName }, '', window.location.pathname
                    + "?" + $.param(this.state));
                if (elementIdSelector) $(elementIdSelector).focus();
            }.bind(this)
        );
    };

    LiveSearch.prototype.tabChange = function (suppressAddHistory) {
        var ct = this.resultsViewModel.currentTabName;
        if (this.currentTab !== ct) {
            this.currentTab = ct;
            if (!suppressAddHistory) history.pushState({ payload: this.state, tab: this.resultsViewModel.currentTabName }, '', window.location.pathname + "?" + $.param(this.state) + "&tab=" + ct);
        }
        
    };

    LiveSearch.prototype.cache = function cache(slug, data) {
        if (typeof data === 'undefined') {
            return this.resultCache[slug];
        } else {
            this.resultCache[slug] = data;
        }
    };

    LiveSearch.prototype.isNewState = function isNewState() {
        return $.param(this.state) !== this.$form.serialize();
    };

    LiveSearch.prototype.updateResults = function updateResults() {
        var searchState = $.param(this.state);
        var cachedResultData = this.cache(searchState);
        var liveSearch = this;
        if (typeof cachedResultData === 'undefined') {
            //this.showLoadingIndicator();
            return $.ajax({
                url: this.action,
                data: this.state,
                searchState: searchState,
                beforeSend: function () {
                    if (!liveSearch.disabled) {
                        DfE.Util.LoadingMessage.display("#schoolResults", "Updating schools");
                    }
                }
            }).done(function (response) {
                liveSearch.cache($.param(liveSearch.state), response);
                liveSearch.displayResults(response, this.searchState);
                liveSearch.toggleSortControl();
                $('.pagination a').on('click', liveSearch.pageChange.bind(liveSearch));
            }).error(function () {
                liveSearch.showErrorIndicator();
            });
        } else {
            this.displayResults(cachedResultData, searchState);
            liveSearch.toggleSortControl();
            $('.pagination a').on('click', liveSearch.pageChange.bind(liveSearch));
            var out = new $.Deferred();
            return out.resolve();
        }
    };

    LiveSearch.prototype.toggleSortControl = function () {
        if ($(".result-info .count-js").text() > 1) {
            $(".filter").show();
        } else {
            $(".filter").hide();
        }

        if ($(".result-info .count-js").text() > 0) {
            $(".result-controllers").show();
        } else {
            $(".result-controllers").hide();
        }
    };

    LiveSearch.prototype.showLoadingIndicator = function showLoadingIndicator() {
        //this.getSummaryBlock().css("display", "none");
        this.getSummaryContainerBlock().append("<p class=\"msg\">Loading...</p>");
    };

    LiveSearch.prototype.showErrorIndicator = function showErrorIndicator() {
        //this.getSummaryBlock().css("display", "none");
        this.getSummaryContainerBlock().append("<p class=\"msg\">Error. Please try modifying your search and trying again.</p>");
    };

    LiveSearch.prototype.displayResults = function displayResults(results, action) {
        if (this.onDisplayResults) this.onDisplayResults(this.state);
        if (this.disabled) return;
        // As search is asynchronous, check that the action associated with these results is
        // still the latest to stop results being overwritten by stale data
        if (action === $.param(this.state)) {
            this.$resultsBlock.empty();
            this.$resultsBlock.html(results);
            DfE.Views.SchoolsResultsViewModel.addAllVisibility();
            DfE.Views.SchoolsResultsViewModel.activateAddRemoveButtons();
            DfE.Views.SchoolsResultsViewModel.initTabs();
            DfE.Views.SchoolsResultsViewModel.initSort();
            this.updateSchoolCount();
        }
    };

    LiveSearch.prototype.restoreBooleans = function restoreBooleans() {
        var that = this;
        this.$form.find('input[type=checkbox], input[type=radio]').each(function (i, el) {
            var $el = $(el);
            $el.prop('checked', that.isBooleanSelected($el.attr('name'), $el.attr('value')));
        });
    };

    LiveSearch.prototype.isBooleanSelected = function isBooleanSelected(name, value) {
        var i, _i;
        for (i = 0, _i = this.state.length; i < _i; i++) {
            if (this.state[i].name === name && this.state[i].value === value) {
                return true;
            }
        }
        return false;
    };

    LiveSearch.prototype.restoreTextInputs = function restoreTextInputs() {
        var that = this;
        this.$form.find('input[type=text]').each(function (i, el) {
            var $el = $(el);
            $el.val(that.getTextInputValue($el.attr('name')));
        });
    };

    LiveSearch.prototype.getTextInputValue = function getTextInputValue(name) {
        var i, _i;
        for (i = 0, _i = this.state.length; i < _i; i++) {
            if (this.state[i].name === name) {
                return this.state[i].value;
            }
        }
        return '';
    };

    GOVUK.LiveSearch = LiveSearch;
}());

function getURLParameter(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}