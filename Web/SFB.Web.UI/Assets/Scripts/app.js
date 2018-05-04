(function () {
    'use strict';

    if (typeof window.GOVUK === 'undefined') { window.GOVUK = {}; }
    if (typeof window.GOVUK.support === 'undefined') { window.GOVUK.support = {}; }

    window.GOVUK.support.history = function () {
        return window.history && window.history.pushState && window.history.replaceState;
    };

    window.GOVUK.support.detailsElement = function () {
        var retVal = "open" in document.createElement("details");
        return retVal;
    };

    // Declares the main application library
    if (typeof window.DfE === 'undefined') {
        window.DfE = {
            Views: {},
            Elements: {},
            Util: { Analytics: {} },
            Sfb: { BenchmarkBasket: {} }
        };
    }

    window.DfE.Util.randomNumber = function () { return Math.floor((Math.random() * 10000000) + 1); };

    window.DfE.Util.QueryString = {
        get: function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }
    };

    window.DfE.Util.LoadingMessage = {
        display : function(location, message) {
            $(location).html('<div style="min-height:300px; margin-top:20px;">' +
                '<img style="vertical-align:bottom" src="../public/assets/images/spinner.gif"></img>' +
                '<span role="alert" aria-live="assertive" aria-label="'+ message +'"></span>' +
                '<span class="font-medium" style="margin-left: 10px">Loading...</span>'+
                '</div>');
        }
    };

    window.DfE.Sfb.BenchmarkBasket = {
        ClearBenchmarkBasket : function () {
            $.get("/school/UpdateBenchmarkBasket?withAction=clear",
                function (data) {
                    $("#benchmarkBasket").replaceWith(data);
                    if ($(".search-results").length > 0) {
                        location.reload();
                    }
                    if ($(".benchmarking-charts").length > 0) {
                        location.replace("/BenchmarkCharts");
                    }
                });
        }
    };

    window.DfE.Util.Charting = {
        ChartMoneyFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else if (amount >= 1000000 || amount <= -1000000)
                return "£" + parseFloat((amount / 1000000).toFixed(2)).toString() + 'm';
            else if (amount >= 10000 || amount <= -10000)
                return "£" + parseFloat((amount / 1000).toFixed(1)).toString() + 'k';
            else
                return "£" + window.DfE.Util.Charting.NumberWithCommas(parseFloat(amount.toFixed(0)).toString());
        },

        ChartPercentageFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else
                return parseFloat(amount.toFixed(1)).toString() + '%';
        },

        ChartDecimalFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else
                return parseFloat(amount.toFixed(2)).toString();
        },

        ChartIntegerFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else
                return parseFloat(amount.toFixed(0)).toString();
        },

        NumberWithCommas: function(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        },

        ToggleChartsTables: function (mode) {
            var $charts = $('.chart-wrapper');
            var $tables = $('.chart-table-wrapper');
            var $showChartsButton = $('a.view-charts-tables.charts');
            var $showTablesButton = $('a.view-charts-tables.tables');
            if (mode === 'Charts') {
                $showChartsButton.hide();
                $showTablesButton.show();
                $tables.hide();
                $charts.show();
                sessionStorage.chartFormat = 'Charts';
            } else if (mode === 'Tables') {
                $showTablesButton.hide();
                $showChartsButton.show();
                $charts.hide();
                $tables.show();
                sessionStorage.chartFormat = 'Tables';
            }
        }

    };

    window.DfE.Util.ComparisonList = {
        getData: function () {
            var decodedCookieData = null;
            var cookieData = GOVUK.cookie("sfb_comparison_list");
            if (cookieData) decodedCookieData = decodeURIComponent(cookieData);
            return decodedCookieData;
        },
        isInList: function (id) {
            var data = this.getData();
            var comparisonList = JSON.parse(data);
            if (comparisonList == null) {
                return false;
            }
            var found = _.find(comparisonList.BS, function (bs) { return bs.U === id; });
            return found !== undefined;
        },
        count: function () {
            var data = this.getData();
            var comparisonList = JSON.parse(data);
            if (comparisonList == null) {
                return 0;
            }
            return comparisonList.BS.length;
        },
        RenderFullListWarningModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');
        
            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title"><div role="document">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Not enough space in basket</h1><p id="modal-content"><br/>' +
                'You can only benchmark up to 30 schools. You can view and remove schools from the <a href=\'/benchmarklist\'>edit basket</a> page.</p>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },
        RenderFullListWarningModalMat: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title"><div role="document">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Trust basket is full</h1><p id="modal-content"><br/>' +
                'You can only benchmark up to 10 trusts.</p>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        }
    };

    window.DfE.Util.Analytics = {
        isAvailable: function () {
            var retVal = window.ga && window.ga.hasOwnProperty('loaded') === true && window.ga.loaded === true;
            return retVal;
        },
        TrackClick: function (event) { // Tracks outbound links and element clicks when bound to "a,.js-track"
            if (DfE.Util.Analytics.isAvailable()) {
                var $element = $(this);
                var isHyperlinking = event.currentTarget && event.currentTarget.hostname;
                var isExternalLink = isHyperlinking && location.hostname != event.currentTarget.hostname;
                var isTargetBlank = $element.attr("target") == "_blank";
                var targetHref = isHyperlinking ? event.currentTarget.href : null;
                var navigateHitCallback = function () { document.location = targetHref; };
                var hasJsTrackCssClass = $(this).hasClass("js-track");

                var trackAction = isExternalLink || hasJsTrackCssClass;
                if (trackAction) {
                    var gaActionName = null, gaCategoryName = null, gaEventLabel = null;
                    if (isExternalLink && !hasJsTrackCssClass) {
                        gaActionName = "click";
                        gaCategoryName = "outbound";
                        gaEventLabel = targetHref;
                    }
                    else {
                        var trackData = $element.data("track"); // 'category|label|action'
                        if (trackData) {
                            var parts = trackData.split("|");
                            if (parts.length == 3) {
                                gaCategoryName = parts[0];
                                gaEventLabel = parts[1];
                                gaActionName = parts[2];

                                if (gaEventLabel == "[use-path]")
                                    gaEventLabel = event.currentTarget.pathname + event.currentTarget.search;
                            }
                        }
                    }

                    if (gaCategoryName && gaActionName) {
                        var gaHitCallback = null;
                        if (isHyperlinking && !isTargetBlank && !navigator.sendBeacon) {
                            gaHitCallback = navigateHitCallback;
                            event.preventDefault();
                        }
                        DfE.Util.Analytics.TrackEvent(gaCategoryName, gaEventLabel, gaActionName, gaHitCallback);
                    }
                }
            }
        },
        TrackEvent: function (category, label, action, callback) {
            if (DfE.Util.Analytics.isAvailable() && category && action) {
                category = category.trim();
                label = label ? label.trim() : null;
                action = action.trim();

                var isCallbackRequired = typeof callback === "function";
                var hasCallbackBeenCalled = false;
                var safeCallback = isCallbackRequired ? function () {
                    if (hasCallbackBeenCalled === true) return;
                    hasCallbackBeenCalled = true;
                    callback();
                } : null;

                try {
                    var payload = {
                        eventCategory: category,
                        eventAction: action,
                        eventLabel: label
                    };

                    if (isCallbackRequired) {
                        payload.transport = "beacon";
                        payload.hitCallback = safeCallback;
                        window.setTimeout(function () { // allow GA 1 second to call the callback, otherwise we do it ourselves
                            //console.log("Hit callback not invoked within 1 second; invoking failsafe");
                            safeCallback();
                        }, 1000);
                    }

                    ga('send', 'event', payload);
                    //console.log("Tracked event: cat: " + category + ", action: " + action + ", label: " + label);
                }
                catch (error) {
                    //console.log("TrackEvent error: " + error);
                }
            } else {
                //console.log("Unable to track event: cat: " + category + ", action: " + action);
            }
        },
        TrackPageView: function (path) {
            if (DfE.Util.Analytics.isAvailable() && path) {
                try {
                    ga('send', 'pageview', path);
                    //console.log("Tracked pageview: " + path);
                } catch (error) {
                    //console.log("TrackPageView error: " + error);
                }
            }
        }
    };

    $(function () {
        $(document).on("click", "a,.js-track", window.DfE.Util.Analytics.TrackClick);
        $(document).on("click", ".expander > span,.label > a", function () {
            var $ele = $(this).closest(".expander");
            if ($ele.hasClass("open")) {
                $ele.removeClass("open").addClass("closed");
                $ele.trigger("contracted");
            } else {
                $ele.addClass("open").removeClass("closed");
                $ele.trigger("expanded");
            }

        });
        var suppressCookie = GOVUK.cookie("suppress-dynamic-header");
        if (suppressCookie === "yes") {
            $(".dynamic-header").hide();
        } else {
            $(".dynamic-header").show();
        }
        $(".js-dismiss-dynamic-header").click(function () {
            $(".dynamic-header").hide();
            GOVUK.cookie("suppress-dynamic-header", 'yes', { days: 7 });
        });
        $(".print-link a").click(function () { window.print(); });
        $(document).on("click", "a.button-view-comparison.zero", function ($e) {
            $e.preventDefault();
            return false;
        });
    });

    //$(function () {
    //    DfE.Util.Accessibility.notifyLayoutChanged();
    //});

    //Pollyfill for log10 function
    Math.log10 = Math.log10 || function (x) {
        return Math.log(x) * Math.LOG10E;
    };
}());