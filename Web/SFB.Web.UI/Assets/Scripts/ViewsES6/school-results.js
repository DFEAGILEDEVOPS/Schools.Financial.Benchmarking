﻿class SchoolsResultsViewModel {
    constructor(activeTab, mapApiKey) {
        this.cache = {};
        this.location;
        this.currentTabName = activeTab;
        this.mapApiKey = mapApiKey;

        this.bindEvents();

        if (activeTab === "map") {
            this.bindAzureMap(this.mapApiKey);
            this.liveSearch.disabled = true;
        }
    }

    bindEvents() {
        // Live Search Form (aka filter form)
        let $results = $('#schoolResults');
        let $atomAutodiscoveryLink = $("link[type='application/atom+xml']").eq('0');

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
    }

    getTabName() {
        return !this.currentTabName ? "list" : this.currentTabName;
    }

    load() {
        this.trackRadiusSearch();
        this.initTabs();
        this.addAllVisibility();
    }

    onDisplayResults(state) {
        let serialisedState = $.param(state);
        this.getMapData(serialisedState);
    }

    initTabs() {
        this.currentTabName = this.initialTabName = $("nav.navigation-links .olist .litem.active").data("tab") === "map" ? "list" : "map";
        $(".navigation-link").click((e) => {
            this.changeTab($(e.currentTarget).data("tab"));
            e.preventDefault();
            return false;
        });
    }

    trackRadiusSearch() {
        var item = $("#DistanceRadius option:selected");
        if (item.length > 0) this.trackFilter("Radius: " + item.text().trim());
    }

    trackFilter(label) {
        DfE.Util.Analytics.TrackEvent('search-results', label.trim(), 'filter');
    }

    bindTracking() {
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
    }

    bindEditSearchButton() {
        GOVUK.Collapsible.bindElements("#EditSearchCollapsible.js-collapsible");
    }

    bindFilterCollapseButtons() {
        // Instantiate an option select for each one found on the page
        $('.govuk-option-select').map(function () {
            return new GOVUK.OptionSelect({ $el: $(this) });
        });
    }

    changeTab(tabName, suppressAddHistory) {
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

            this.addAllVisibility();
        }
    }

    onRefresh() {
        this.bindEditSearchButton();
        this.bindFilterCollapseButtons();
        this.bindTracking();
        this.mapLoaded = false;
        this.initTabs();
    }

    bindAzureMap(mapApiKey) {
        if (!this.mapLoaded && this.currentTabName === "map") {

            let location = { lat: 52.636, lng: -1.139 }; // no location specified, so use central England.                                    

            var options = {
                elementId: "azuremap",
                primaryMarker: {
                    geometry: {
                        location: {
                            lat: location.lat,
                            lng: location.lng
                        }
                    }
                },
                mapApiKey: mapApiKey,
                fullScreen: true
            };

            this.map = new GOVUK.AzureSchoolLocationsMap(options);

            this.mapLoaded = true;
        }

        this.getMapData(this.liveSearch.$form.serialize());
    }

    getMapData(serialisedState) {
        if (this.currentTabName === "map") {
            if (this.cache[serialisedState]) {
                this.updateLiveCount(this.cache[serialisedState].count);
                this.map.renderMapPinsForAzureMap(this.cache[serialisedState]);
            }
            else {
                return $.ajax({
                    url: "/SchoolSearch/search-json",
                    data: serialisedState
                }).done((response) => {
                    this.cache[serialisedState] = response;
                    this.updateLiveCount(response.count);
                    this.map.renderMapPinsForAzureMap(response);
                }).error(function (error) {
                    console.log("Error loading map pins: " + error);
                });
            }
        }
    }

    updateLiveCount(count) {
        $("span.result-count").html(count);
        $("span.screen-reader-result-count").html("Filtering results");
        setTimeout(function () {
            $("span.screen-reader-result-count").html(count + " schools found");
        }, 1000);

        this.liveSearch.getSummaryBlock().css("visibility", "visible");
        this.liveSearch.getSummaryContainerBlock().find("p.msg").remove();
    }

    updateBenchmarkBasket(urn, withAction) {
        if (withAction === "Add") {
            if (DfE.Util.ComparisonList.count() === 30) {
                DfE.Util.ComparisonList.RenderFullListWarningModal();
                return;
            }
        }

        $.get("/school/UpdateBenchmarkBasket?urn=" + urn + "&withAction=" + withAction,
            (data) => {
                $("#benchmarkBasket").replaceWith(data);
                $("div[data-urn='" + urn + "']>.add-remove").toggle();
                this.addAllVisibility();
            });
    }

    addAll() {
        let $addButtons = $(".addto:visible");
        let schoolsToAddCount = $addButtons.length;
        let comparisonListCount = DfE.Util.ComparisonList.count();
        if (comparisonListCount + schoolsToAddCount > 30) {
            DfE.Util.ComparisonList.RenderFullListWarningModal();
        } else {
            var urns = [];
            $addButtons.each(function () {
                var urn = $(this).attr("data-urn");
                urns.push(urn);
            });
            $.post("/school/UpdateBenchmarkBasketAddMultiple",
                { 'urns[]': urns },
                (data) => {
                    $("#benchmarkBasket").replaceWith(data);
                    $addButtons.parent().find(">.add-remove").toggle();
                    this.addAllVisibility();
                });
        }
    }

    addAllVisibility() {
        let $addButtons = $(".addto:visible");
        let schoolsToAddCount = $addButtons.length;
        let comparisonListCount = DfE.Util.ComparisonList.count();
        if (schoolsToAddCount <= 1 || comparisonListCount + schoolsToAddCount > 30) {
            $(".addall").hide();
        } else {
            $(".addall").show();
        }
    }

    activateAddRemoveButtons() {
        let $addRemoveButtons = $(".add-remove:visible");

        $addRemoveButtons.each(function () {
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
    }

    updateMainFilterForm(form, field, value) {
        $('#' + form).find('#' + field).val(value);
    }

}//class
