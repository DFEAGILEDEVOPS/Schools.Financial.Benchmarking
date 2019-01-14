(function (GOVUK, Views) {
    'use strict';

    function SchoolsResultsViewModel(activeTab, mapApiKey) {
        this.cache = {};
        this.bindEvents();
        this.location;
        this.currentTabName = activeTab;
        this.mapApiKey = mapApiKey;

        if (activeTab === "map") {            
            this.bindAzureMap(this.mapApiKey);
            this.liveSearch.disabled = true;
        }
    }

    SchoolsResultsViewModel.prototype = {

        bindEvents: function () {
            // Live Search Form (aka filter form)
            var $results = $('#schoolResults'),
                $atomAutodiscoveryLink = $("link[type='application/atom+xml']").eq('0');

            this.liveSearch = new GOVUK.LiveSearch({
                formId: "SearchFacetsForm",
                secondaryFormId: "SearchFacetsForm2",
                $results: $results,
                $atomAutodiscoveryLink: $atomAutodiscoveryLink,
                onRefresh: this.onRefresh.bind(this),
                onDisplayResults: this.onDisplayResults.bind(this),
                resultsViewModel: this 
            });

            window.addEventListener("load", this.load.bind(this));
            this.bindTracking();
        },
        getTabName: function () {
            return !this.currentTabName ? "list" : this.currentTabName;
        },
        load: function () {
            this.trackRadiusSearch();
            this.initTabs();
            SchoolsResultsViewModel.AddAllVisibility();
        },
        onDisplayResults: function (state) {
            var serialisedState = $.param(state);
            this.getMapData(serialisedState);
        },
        initTabs: function () {
            this.currentTabName = this.initialTabName = $("nav.navigation-links .olist .litem.active").data("tab") === "map" ? "list" : "map";
            var self = this;
            $(".navigation-link").click(function (e) {
                self.changeTab($(this).data("tab"));
                e.preventDefault();
                return false;
            });
        },
        trackRadiusSearch: function () {
            var item = $("#DistanceRadius option:selected");
            if (item.length > 0) this.trackFilter("Radius: " + item.text().trim());
        },
        trackFilter: function (label) {
            DfE.Util.Analytics.TrackEvent('search-results', label.trim(), 'filter');
        },
        bindTracking: function () {
            var self = this;
            $("#DistanceRadius").change(this.trackRadiusSearch.bind(this));
            $(".js-live-search-results-block input[type=checkbox]").change(function () {
                if ($(this).is(":checked")) {
                    var facetLabel = $(this).parent().text();
                    var facetGroupName = $(this).closest("div.govuk-option-select").find("div.option-select-label")
                        .text();
                    self.trackFilter(facetGroupName.trim() + "/" + facetLabel.trim());
                }
            });
        },

        bindEditSearchButton: function () {
            GOVUK.Collapsible.bindElements("#EditSearchCollapsible.js-collapsible");
        },
        bindFilterCollapseButtons: function () {
            // Instantiate an option select for each one found on the page
            var filters = $('.govuk-option-select').map(function () {
                return new GOVUK.OptionSelect({ $el: $(this) });
            });
        },
        changeTab: function (tabName, suppressAddHistory) {
            if ($(".navigation-links .litem.active").data("tab") === tabName) {

                $("nav.navigation-links .olist .litem,  div.tabs>div").removeClass("active");
                $("nav.navigation-links .olist .litem." + tabName + ", div.tabs>div." + tabName).addClass("active");
                $("nav.navigation-links .olist .litem.active a").focus();
                this.currentTabName = tabName;
                this.bindAzureMap(this.mapApiKey);
                this.liveSearch.disabled = (tabName === "map");
                if (tabName === "list") {
                    this.liveSearch.updateResults.bind(this.liveSearch).call(null);
                }
                this.liveSearch.tabChange(suppressAddHistory);

                SchoolsResultsViewModel.AddAllVisibility();
            }
        },
        onRefresh: function () {
            this.bindEditSearchButton();
            this.bindFilterCollapseButtons();
            this.bindTracking();
            this.mapLoaded = false;
            this.initTabs();
        },
        bindAzureMap: function (mapApiKey) {
            if (!this.mapLoaded && this.currentTabName === "map") {
                                
                this.location = { lat: 52.636, lng: -1.139 }; // no location specified, so use central England.                                    

                var options = {
                    elementId: "azuremap",
                    primaryMarker: {
                        geometry: {
                            location: {
                                lat: this.location.lat,
                                lng: this.location.lng
                            }
                        }
                    },
                    mapApiKey: mapApiKey,
                    fullScreen : true
                };

                this.map = new GOVUK.AzureSchoolLocationsMap(options);

                this.mapLoaded = true;
            }

            this.getMapData(this.liveSearch.$form.serialize());
        },
        getMapData: function (serialisedState) {
            if (this.currentTabName === "map") {
                if (this.cache[serialisedState]) {
                    this.updateLiveCount(this.cache[serialisedState].count);
                    this.map.renderMapPinsForAzureMap(this.cache[serialisedState]);
                }
                else {
                    var self = this;
                    return $.ajax({
                        url: "/SchoolSearch/search-json",
                        data: serialisedState
                    }).done(function (response) {
                        self.cache[serialisedState] = response;
                        self.updateLiveCount(response.count);
                        self.map.renderMapPinsForAzureMap(response);
                    }).error(function (error) {
                        console.log("Error loading map pins: " + error);
                    });
                }
            }
        },

        updateLiveCount: function (count) {
            $("span.result-count").html(count);
            $("span.screen-reader-result-count").html("Filtering results");
            setTimeout(function () {
                $("span.screen-reader-result-count").html(count + " schools found");
            }, 1000);

            this.liveSearch.getSummaryBlock().css("visibility", "visible");
            this.liveSearch.getSummaryContainerBlock().find("p.msg").remove();
        }      

    };

    SchoolsResultsViewModel.UpdateBenchmarkBasket = function (urn, withAction) {
        if (withAction === "Add") {
            if (DfE.Util.ComparisonList.count() === 30) {
                DfE.Util.ComparisonList.RenderFullListWarningModal();
                return;
            }
        }

        $.get("/school/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
            function (data) {
                $("#benchmarkBasket").replaceWith(data);
                $("div[data-urn='" + urn + "']>.add-remove").toggle();
                SchoolsResultsViewModel.AddAllVisibility();
            });
    };

    SchoolsResultsViewModel.AddAll = function () {
        var self = this;
        self.$addButtons = $(".addto:visible");
        var schoolsToAddCount = self.$addButtons.length;
        var comparisonListCount = DfE.Util.ComparisonList.count();
        if (comparisonListCount + schoolsToAddCount > 30) {
            DfE.Util.ComparisonList.RenderFullListWarningModal();
        } else {
            var urns = [];
            self.$addButtons.each(function () {
                var urn = $(this).attr("data-urn");
                urns.push(urn);
            });
            $.post("/school/UpdateBenchmarkBasketAddMultiple",
                { 'urns[]': urns },
                function (data) {
                    $("#benchmarkBasket").replaceWith(data);
                    self.$addButtons.parent().find(">.add-remove").toggle();
                    SchoolsResultsViewModel.AddAllVisibility();
                });
        }
    };

    SchoolsResultsViewModel.AddAllVisibility = function () {
        var self = this;
        self.$addButtons = $(".addto:visible");
        var schoolsToAddCount = self.$addButtons.length;
        var comparisonListCount = DfE.Util.ComparisonList.count();
        if (schoolsToAddCount <= 1 || comparisonListCount + schoolsToAddCount > 30) {
            $(".addall").hide();
        } else {
            $(".addall").show();
        }
    };

    SchoolsResultsViewModel.ActivateAddRemoveButtons = function () {
        var self = this;
        self.$addRemoveButtons = $(".add-remove:visible");

        self.$addRemoveButtons.each(function () {
            var urn = $(this).parent().attr("data-urn");
            var inList = DfE.Util.ComparisonList.isInList(urn);
            if (inList) {
                $(this).parent().find('.addto').hide();
                $(this).parent().find('.removefrom').show();
            } else {
                $(this).parent().find('.addto').show();
                $(this).parent().find('.removefrom').hide();
            }
        });
    };

    // entry point when the schools results view page is loaded
    SchoolsResultsViewModel.Load = function (loc, tab, mapApiKey) {
        new DfE.Views.SchoolsResultsViewModel(loc, tab, mapApiKey);
    };

    Views.SchoolsResultsViewModel = SchoolsResultsViewModel;

}(GOVUK, DfE.Views));

function updateMainFilterForm(form, field, value) {
    $('#' + form).find('#' + field).val(value);
}