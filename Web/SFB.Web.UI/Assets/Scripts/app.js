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

    window.DfE.Util.focusTo = function (elementId) {
        location.href = '#' + elementId;
    };

    window.DfE.Util.QueryString = {
        get: function (name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)", 'i'),
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
            $.get("/school/UpdateBenchmarkBasket?withAction=RemoveAll",
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
            else if (amount >= 1000000)
                return "£" + parseFloat((amount / 1000000).toFixed(2)).toString() + 'm';
            else if (amount <= -1000000)
                return "-£" + parseFloat(Math.abs(amount / 1000000).toFixed(2)).toString() + 'm';
            else if (amount >= 10000)
                return "£" + parseFloat((amount / 1000).toFixed(1)).toString() + 'k';
            else if (amount <= -10000)
                return "-£" + parseFloat(Math.abs(amount / 1000).toFixed(1)).toString() + 'k';
            else if (amount < 0)
                return "-£" + window.DfE.Util.Charting.NumberWithCommas(parseFloat(Math.abs(amount).toFixed(0)).toString());
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
        }
    };

    window.DfE.Util.ModalRenderer = {
        RenderAdditionalGrantModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="additionalGrantModal" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Additional grant for schools</h1><p id="modal-content">' +
                'This includes: primary PE and sports grants, universal infant free school meal funding, and additional grant funding for secondary schools to release PE teachers to work in primary schools.</p>' +
                '<a href="#" id="js-modal-close-bottom" class="modal-close white-font" data-focus-back="additionalGrantModal" title="Close">Close</a></dialog>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        }
    }

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
        
            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="label_modal_1" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Not enough space in basket</h1><p id="modal-content">' +
                'You can only benchmark up to 30 schools. You can view and remove schools from the <a href=\'/benchmarklist\'>edit basket</a> page.</p>' +
                '<a href="#" id="js-modal-close-bottom" class="modal-close white-font" data-focus-back="label_modal_1" title="Close">Close</a></dialog>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },
        RenderFullListWarningModalManual: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="manualButton" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Not enough space in basket</h1><p id="modal-content">' +
                'You can only benchmark up to 30 schools.</p>' +
                '<a href="#" id="js-modal-close-bottom" class="modal-close white-font" data-focus-back="manualButton" title="Close">Close</a></dialog>';

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

            var $modal_code = '<dialog id="js-modal" class="modal" role="dialog" aria-labelledby="modal-title">' +
                '<a href="#" id="js-modal-close" class="modal-close" data-focus-back="manualButton" title="Close">Close</a>' +
                '<h1 id="modal-title" class="modal-title">Trust basket is full</h1><p id="modal-content">' +
                'You can only benchmark up to 20 trusts.</p>' +
                '<a href="#" id="js-modal-close-bottom" class="modal-close white-font" data-focus-back="manualButton" title="Close">Close</a></dialog>';

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },
        RenderYourChartsInfoModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
                "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='renderYourChartsInfo' title='Close'>Close</a>" +
                "<h1 id='modal-title' class='modal-title'>Your charts tab</h1><p id='modal-content'>" +
                "This tab shows which charts you have chosen to include in your customised report. You can add charts to this area by selecting the ‘Add to your charts’ function beside each chart. You can also add charts within the tab by selecting the relevant checkboxes.</p>" +
                "</div><a href='#' id='js-modal-close-bottom' class='modal-close white-font' data-focus-back='renderYourChartsInfo' title='Close'>Close</a></dialog>";

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },
        RenderBicCriteriaP8Modal: function (event) {

            event.stopPropagation();

            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
                "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='renderP8Info' title='Close'>Close</a>" +
                "<h1 id='modal-title' class='modal-title'>Progress 8 scores</h1><p id='modal-content'>" +
                "Progress 8 score is calculated for each pupil by comparing their Attainment 8 score – with the average Attainment 8 scores of all pupils nationally who had a similar starting point, using assessment results from the end of primary school.</p>" +                
                "<h3 class='heading-small'>What do the scores mean</h3>" +
                "<div class='modal__score'><div class='score well-below'>Well below average</div><div>About <span class='bold'>13%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score below'>Below average</div><div>About <span class='bold'>19%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score average'>Average</div><div>About <span class='bold'>37%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score above'>Above average</div><div>About <span class='bold'>17%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score well-above'>Well above average</div><div>About <span class='bold'>14%</span> of</br> schools in England</div></div>" +
                "</div><a href='#' id='js-modal-close-bottom' class='modal-close white-font' data-focus-back='renderP8Info' title='Close'>Close</a></dialog>";

            $($modal_code).insertAfter($page);
            $body.addClass('no-scroll');

            $page.attr('aria-hidden', 'true');

            // add overlay
            var $modal_overlay =
                '<span id="js-modal-overlay" class="modal-overlay" title="Close" data-background-click="enabled"><span class="invisible">Close modal</span></span>';

            $($modal_overlay).insertAfter($('#js-modal'));

            $('#js-modal-close').focus();

        },
        RenderBicCriteriaKs2Modal: function (event) {

            event.stopPropagation();

            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
                "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='renderKs2Info' title='Close'>Close</a>" +
                "<h1 id='modal-title' class='modal-title'>Key stage 2 progress scores</h1><p id='modal-content'>" +
                "The scores are calculated by comparing the key stage 2 test and assessment results of pupils with the results of pupils in schools across England who started with similar assessment results at the end of the previous key stage 1.</p>" +
                "<h3 class='heading-small'>What do the scores mean</h3>" +
                "<div class='modal__score'><div class='score well-below'>Well below average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>"+
                "<div class='modal__score'><div class='score below'>Below average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>"+
                "<div class='modal__score'><div class='score average'>Average</div><div>About <span class='bold'>60%</span> of</br> schools in England</div></div>"+
                "<div class='modal__score'><div class='score above'>Above average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>"+
                "<div class='modal__score'><div class='score well-above'>Well above average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>"+
                "</div><a href='#' id='js-modal-close-bottom' class='modal-close white-font' data-focus-back='renderKs2Info' title='Close'>Close</a></dialog>";

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
            $(".header-content__dynamic-header ").hide();
        } else {
            $(".header-content__dynamic-header ").show();
        }
        $(".js-dismiss-dynamic-header").click(function () {
            $(".header-content__dynamic-header ").hide();
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