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

    window.DfE.Util.getFormData = function ($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return indexed_array;
    };

    window.DfE.Util.clearDropdowns = function (event) {
        if (event.target.activeElement.id !== "DownloadLink") {
            $('select').prop('selectedIndex', -1);
        }
        //$(".govuk-checkboxes__input").prop("checked", false);
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
        },
        getHashParameter: function (name, url) {
            if (!url) url = window.location.href;
            if (url.includes("#")) {
                return url.substr(url.lastIndexOf("#") + 1);
            }
            return null;
        }
    };

    window.DfE.Util.Features = {
        enabled: function (feature) {
            return $("#Feature-" + feature + "-enabled").val().toLowerCase() === "true";
        }
    };

    window.DfE.Util.LoadingMessage = {
        display : function(location, message) {
            $(location).html('<div style="min-height:300px; margin-top:20px;">' +
                '<img style="vertical-align:bottom" src="../public/assets/images/spinner.gif"></img>' +
                '<span role="alert" aria-live="assertive" aria-label="'+ message +'"></span>' +
                '<span class="govuk-body" style="margin-left: 10px">Loading...</span>'+
                '</div>');
        }
    };

    window.DfE.Sfb.BenchmarkBasket = {
        clearBenchmarkBasket : function () {
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
        chartMoneyFormat: function(amount) {
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
                return "-£" + window.DfE.Util.Charting.numberWithCommas(parseFloat(Math.abs(amount).toFixed(0)).toString());
            else
                return "£" + window.DfE.Util.Charting.numberWithCommas(parseFloat(amount.toFixed(0)).toString());
        },

        chartPercentageFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else {
                if (amount > 0 && amount < 2)
                    return parseFloat(amount.toFixed(2)).toString() + '%';
                else
                    return parseFloat(amount.toFixed(1)).toString() + '%';
            }
        },

        chartDecimalFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else
                return parseFloat(amount.toFixed(2)).toString();
        },

        chartIntegerFormat: function(amount) {
            if (amount === null)
                return "Not applicable";
            else
                return parseFloat(amount.toFixed(0)).toString();
        },

        numberWithCommas: function(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    };

    window.DfE.Util.CookieOverlayRenderer = {
        render: function () {
            var div = document.createElement("div");
            div.className += "cookie-overlay";
            document.getElementById('cookie-overlay-wrapper').appendChild(div);
            window.onscroll = function () { window.scrollTo(0, 0); };
        },

        unRender: function () {
            document.getElementById('cookie-overlay-wrapper').removeChild(document.getElementById('cookie-overlay-wrapper').lastChild);
            window.onscroll = null;
        }
    }

    window.DfE.Util.ModalRenderer = {

        getQueryString: function () {
            return encodeURIComponent(window.location.pathname + window.location.search);
        },

        renderAdditionalGrantModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal govuk-body-s" role="dialog" aria-labelledby="modal-title">' +
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

        },

        renderYourChartsInfoModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
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
        renderBicCriteriaP8Modal: function (event) {

            event.stopPropagation();

            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
                "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='renderP8Info' title='Close'>Close</a>" +
                "<h1 id='modal-title' class='modal-title'>Progress 8 scores</h1><p id='modal-content'>" +
                "Progress 8 score is calculated for each pupil by comparing their Attainment 8 score – with the average Attainment 8 scores of all pupils nationally who had a similar starting point, using assessment results from the end of primary school.</p>" +
                "<h3 class='heading-small'>What do the scores mean</h3>" +
                "<div class='modal__score'><div class='score well-below'>Well below average</div><div>About <span class='bold'>13%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score below'>Below average</div><div>About <span class='bold'>19%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score average'>Average</div><div>About <span class='bold'>37%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score above'>Above average</div><div>About <span class='bold'>17%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='score well-above'>Well above average</div><div>About <span class='bold'>14%</span> of</br> schools in England</div></div>" +
                "<div class='govuk-warning-text mt-2'><span class='govuk-warning-text__icon' aria-hidden='true'>!</span><div class='govuk-body-s govuk-warning-text__text'><span class='govuk-warning-text__assistive'>Warning</span>Due to Covid-19 the Government is not publishing the school educational performance data for 2020, the latest progress data is from 2019.</div></div>" +
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
        renderBicCriteriaKs2Modal: function (event) {
            event.stopPropagation();

            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = "<dialog id='js-modal' class='modal govuk-body-s' role='dialog' aria-labelledby='modal-title'><div role='document'>" +
                "<a href='#' id='js-modal-close' class='modal-close' data-focus-back='renderKs2Info' title='Close'>Close</a>" +
                "<h1 id='modal-title' class='modal-title'>Key stage 2 progress scores</h1><p class='govuk-body-s' id='modal-content'>" +
                "The scores are calculated by comparing the key stage 2 test and assessment results of pupils with the results of pupils in schools across England who started with similar assessment results at the end of the previous key stage 1.</p>" +
                "<h3 class='govuk-heading-s'>What do the scores mean</h3>" +
                "<div class='modal__score'><div class='govuk-body-s score well-below'>Well below average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='govuk-body-s score below'>Below average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='govuk-body-s score average'>Average</div><div>About <span class='bold'>60%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='govuk-body-s score above'>Above average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>" +
                "<div class='modal__score'><div class='govuk-body-s score well-above'>Well above average</div><div>About <span class='bold'>10%</span> of</br> schools in England</div></div>" +
                "<div class='govuk-warning-text mt-2'><span class='govuk-warning-text__icon' aria-hidden='true'>!</span><div class='govuk-body-s govuk-warning-text__text'><span class='govuk-warning-text__assistive'>Warning</span>Due to Covid-19 the Government is not publishing the school educational performance data for 2020, the latest progress data is from 2019.</div></div>" +
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

    window.DfE.Util.ComparisonList = {
        getData: function (cookieName) {
            var decodedCookieData = null;
            var cookieData = GOVUK.cookie(cookieName);
            if (cookieData) decodedCookieData = decodeURIComponent(cookieData);
            return decodedCookieData;
        },
        isInList: function (id) {
            var data = this.getData("sfb_comparison_list");
            var comparisonList = JSON.parse(data);
            if (comparisonList === null) {
                return false;
            }
            var found = _.find(comparisonList.BS, function (bs) { return bs.U === id; });
            return found !== undefined;
        },
        isInManualList: function (id) {
            var data = this.getData("sfb_comparison_list_manual");
            var comparisonList = JSON.parse(data);
            if (comparisonList === null) {
                return false;
            }
            var found = _.find(comparisonList.BS, function (bs) { return bs.U === id; });
            return found !== undefined;
        },
        count: function () {
            var data = this.getData("sfb_comparison_list");
            var comparisonList = JSON.parse(data);
            if (comparisonList === null) {
                return 0;
            }
            return comparisonList.BS.length;
        },
        countManual: function () {
            var data = this.getData("sfb_comparison_list_manual");
            var comparisonList = JSON.parse(data);
            if (comparisonList === null) {
                return 0;
            }
            return comparisonList.BS.length;
        },
        renderFullListWarningModal: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');
        
            var $modal_code = '<dialog id="js-modal" class="modal govuk-body-s" role="dialog" aria-labelledby="modal-title">' +
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
        renderFullListWarningModalManual: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal govuk-body-s" role="dialog" aria-labelledby="modal-title">' +
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
        renderFullListWarningModalMat: function () {
            var $body = $('body');
            var $page = $('#js-modal-page');

            var $modal_code = '<dialog id="js-modal" class="modal govuk-body-s" role="dialog" aria-labelledby="modal-title">' +
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

        }
    };

    window.DfE.Util.Analytics = {
        isAvailable: function () {
            var retVal = window.ga && window.ga.hasOwnProperty('loaded') === true && window.ga.loaded === true;
            return retVal;
        },
        trackClick: function (event) { // Tracks outbound links and element clicks when bound to "a,.js-track"
            if (DfE.Util.Analytics.isAvailable()) {
                var $element = $(this);
                var isHyperlinking = event.currentTarget && event.currentTarget.hostname;
                var isExternalLink = isHyperlinking && location.hostname !== event.currentTarget.hostname;
                var isTargetBlank = $element.attr("target") === "_blank";
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
                            if (parts.length === 3) {
                                gaCategoryName = parts[0];
                                gaEventLabel = parts[1];
                                gaActionName = parts[2];

                                if (gaEventLabel === "[use-path]")
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
                        DfE.Util.Analytics.trackEvent(gaCategoryName, gaEventLabel, gaActionName, gaHitCallback);
                    }
                }
            }
        },
        trackEvent: function (category, label, action, callback) {
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
                    //console.log("trackEvent error: " + error);
                }
            } else {
                //console.log("Unable to track event: cat: " + category + ", action: " + action);
            }
        },
        trackPageView: function (path) {
            if (DfE.Util.Analytics.isAvailable() && path) {
                try {
                    ga('send', 'pageview', path);
                    //console.log("Tracked pageview: " + path);
                } catch (error) {
                    //console.log("trackPageView error: " + error);
                }
            }
        }
    };

    $(function () {

        manageCookies();

        //$(document).on("click", "a,.js-track", window.DfE.Util.Analytics.trackClick);

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

        $(".print-link a").click(function () { window.print(); });

        $(document).on("click", "a.button-view-comparison.zero", function ($e) {
            $e.preventDefault();
            return false;
        });   

        $("a.button, a.js-modal").on("keydown", function (event) {
            // get the target element
            var target = event.target;
            // if the element has a role='button' and the pressed key is a space, we'll simulate a click
            if (target.getAttribute('role') === 'button' && event.keyCode === 32) {
                event.preventDefault();
                // trigger the target's click event
                target.click();
            }
        })
    });

    //$(function () {
    //    DfE.Util.Accessibility.notifyLayoutChanged();
    //});

    //Pollyfill for log10 function
    Math.log10 = Math.log10 || function (x) {
        return Math.log(x) * Math.LOG10E;
    };
}());

(function () {
    "use strict"
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }

    /*
      Cookie methods
      ==============
  
      Usage:
  
        Setting a cookie:
        GOVUK.cookie('hobnob', 'tasty', { days: 30 });
  
        Reading a cookie:
        GOVUK.cookie('hobnob');
  
        Deleting a cookie:
        GOVUK.cookie('hobnob', null);
    */
    GOVUK.cookie = function (name, value, options) {
        if (typeof value !== 'undefined') {
            if (value === false || value === null) {
                return GOVUK.setCookie(name, '', { days: -1 });
            } else {
                return GOVUK.setCookie(name, value, options);
            }
        } else {
            return GOVUK.getCookie(name);
        }
    };
    GOVUK.setDomainCookie = function (name, value, options, domain) {
        if (typeof options === 'undefined') {
            options = {};
        }
        var cookieString = name + "=" + value + "; path=/";
        if (options.days) {
            var date = new Date();
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }
        if (document.location.protocol == 'https:') {
            cookieString = cookieString + "; Secure";
        }
        cookieString = cookieString + "; samesite=lax";
        cookieString = cookieString + "; domain=" + domain;
        document.cookie = cookieString;
    };
    GOVUK.setCookie = function (name, value, options) {
        if (typeof options === 'undefined') {
            options = {};
        }
        var cookieString = name + "=" + value + "; path=/";
        if (options.days) {
            var date = new Date();
            date.setTime(date.getTime() + (options.days * 24 * 60 * 60 * 1000));
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }
        if (document.location.protocol == 'https:') {
            cookieString = cookieString + "; Secure";
        }
        cookieString = cookieString + "; samesite=lax";
        document.cookie = cookieString;
    };
    GOVUK.getCookie = function (name) {
        var nameEQ = name + "=";
        var cookies = document.cookie.split(';');
        for (var i = 0, len = cookies.length; i < len; i++) {
            var cookie = cookies[i];
            while (cookie.charAt(0) == ' ') {
                cookie = cookie.substring(1, cookie.length);
            }
            if (cookie.indexOf(nameEQ) === 0) {
                return decodeURIComponent(cookie.substring(nameEQ.length));
            }
        }
        return null;
    };
}).call(this);
(function () {
    "use strict"
    var root = this;
    if (typeof root.GOVUK === 'undefined') { root.GOVUK = {}; }
}).call(this);
(function () {
    "use strict"

    // add cookie message
    if (window.GOVUK && GOVUK.addCookieMessage) {
        GOVUK.addCookieMessage();
    }

    // header navigation toggle
    if (document.querySelectorAll && document.addEventListener) {
        var els = document.querySelectorAll('.js-header-toggle'),
            i, _i;
        for (i = 0, _i = els.length; i < _i; i++) {
            els[i].addEventListener('click', function (e) {
                e.preventDefault();
                var target = document.getElementById(this.getAttribute('href').substr(1)),
                    targetClass = target.getAttribute('class') || '',
                    sourceClass = this.getAttribute('class') || '';

                if (targetClass.indexOf('js-visible') !== -1) {
                    target.setAttribute('class', targetClass.replace(/(^|\s)js-visible(\s|$)/, ''));
                } else {
                    target.setAttribute('class', targetClass + " js-visible");
                }
                if (sourceClass.indexOf('js-hidden') !== -1) {
                    this.setAttribute('class', sourceClass.replace(/(^|\s)js-hidden(\s|$)/, ''));
                } else {
                    this.setAttribute('class', sourceClass + " js-hidden");
                }
            });
        }
    }
}).call(this);

function manageCookies() {
    var cookiesPolicyCookie = GOVUK.cookie("cookies_policy");
    if (!cookiesPolicyCookie) {
        cookiesPolicyCookie = { "essential": true, "settings": false, "usage": false };
        GOVUK.setDomainCookie("cookies_policy", JSON.stringify(cookiesPolicyCookie), { days: 365 }, $("#cookieDomain").val() );
    } else {
        cookiesPolicyCookie = JSON.parse(cookiesPolicyCookie);
    }

    manageCookiePreferencesCookies();
    manageRecruitmentBannerAndCookie(cookiesPolicyCookie);
    manageGACookies(cookiesPolicyCookie);
    //manageMSCookies(cookiesPolicyCookie);

    $("#acceptAllCookies").click(function () {
        var cookiesPolicyCookie = { "essential": true, "settings": true, "usage": true };
        GOVUK.setDomainCookie("cookies_policy", JSON.stringify(cookiesPolicyCookie), { days: 365 }, $("#cookieDomain").val());
        GOVUK.setDomainCookie("cookies_preferences_set", "true", { days: 365 }, $("#cookieDomain").val());

        $("#govuk-cookie-banner-message").hide();
        $("#govuk-cookie-accept-confirmation").show();           
    });


    $("#rejectAllCookies").click(function () {
        var cookiesPolicyCookie = { "essential": true, "settings": false, "usage": false };
        GOVUK.setDomainCookie("cookies_policy", JSON.stringify(cookiesPolicyCookie), { days: 365 }, $("#cookieDomain").val());
        GOVUK.setDomainCookie("cookies_preferences_set", "true", { days: 365 }, $("#cookieDomain").val());

        $("#govuk-cookie-banner-message").hide();
        $("#govuk-cookie-reject-confirmation").show();     
    });

    $(".cookie-banner-hide-button").click(function () {
        $(".govuk-cookie-banner").attr("hidden", true);
    });
}

function manageCookiePreferencesCookies() {

    if (!GOVUK.cookie("cookies_preferences_set") && !window.location.href.toLowerCase().includes("/help/cookies")) {        
        $(".govuk-cookie-banner").removeAttr("hidden");
        $("#govuk-cookie-banner-message").show();
    } else {
        $(".govuk-cookie-banner").attr("hidden", true);
    }
}

function manageRecruitmentBannerAndCookie(cookiesPolicyCookie) {
    var isInRecruitmentPage = window.location.href.toLowerCase().includes("help/get-involved") || window.location.href.toLowerCase().includes("/help/getinvolvedsubmission");
    if (cookiesPolicyCookie.settings) {
        var suppressRecruitmentBannerCookie = GOVUK.cookie("suppress-recruitment-banner");
        if (suppressRecruitmentBannerCookie === "yes") {
            $(".banner-content__recruitment-banner").hide();
        } else {
            if (!isInRecruitmentPage) {
                $(".banner-content__recruitment-banner").show();
                $(".js-dismiss-recruitment-banner").click(function () {
                    $(".banner-content__recruitment-banner").hide();
                    GOVUK.cookie("suppress-recruitment-banner", 'yes', { days: 180 });
                });
            }
        }
    }
    else {
        GOVUK.cookie("suppress-recruitment-banner", null);
        if (!isInRecruitmentPage) {
            $(".banner-content__recruitment-banner").show();
            $(".js-dismiss-recruitment-banner").click(function () {
                $(".banner-content__recruitment-banner").hide();
                cookiesPolicyCookie = JSON.parse(GOVUK.cookie("cookies_policy"));
                if (cookiesPolicyCookie.settings) {
                    GOVUK.cookie("suppress-recruitment-banner", 'yes', { days: 180 });
                }
            });
        }
    }
}

function manageGACookies(cookiesPolicyCookie) {
    if (!cookiesPolicyCookie.usage)  {
        GOVUK.cookie("_ga", null);
        GOVUK.cookie("_gat", null);
        GOVUK.cookie("_gid", null);
    }
}

//function manageMSCookies(cookiesPolicyCookie) {
//    if (!cookiesPolicyCookie.campaigns) {
//        GOVUK.cookie("MC1", null);
//        GOVUK.cookie("MS0", null);
//    }
//}

