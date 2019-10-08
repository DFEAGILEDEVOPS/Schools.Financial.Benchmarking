class SchoolsResultsViewModel {
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

        GOVUK.Modal.Load();

        $(function () {
            if ($(window).width() <= 640)
                $('details').removeAttr('open');
        });
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
    }

    getTabName() {
        return !this.currentTabName ? "list" : this.currentTabName;
    }

    load() {
        this.initTabs();
        this.initSort();
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

    initSort() {
        $("#OrderByControl").change((e) => {
            this.liveSearch.formChange();            
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
            if (this.currentTabName === "map") {
                $(".filter.school").hide();
                $(".pagination-container").hide();
            }
            else {
                $(".filter.school").show();
                $(".pagination-container").show();
            }

            this.bindAzureMap(this.mapApiKey);
            this.liveSearch.disabled = (tabName === "map");
            if (tabName === "list") {
                this.liveSearch.updateResults.bind(this.liveSearch).call(null);
                this.mapLoaded = false;
            } 
            this.liveSearch.tabChange(suppressAddHistory);

            this.addAllVisibility();
        }
    }

    onRefresh() {
        this.bindEditSearchButton();
        this.bindFilterCollapseButtons();
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
                var searchController = $("#SearchMethod").val() === "Manual" ? "ManualComparison"
                    : $("#SearchMethod").val() === "MAT" ? "TrustSearch" : "SchoolSearch";
                return $.ajax({
                    url: `/${searchController}/search-json`,
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
            $("span.screen-reader-result-count").html($('#result-list .summary').html());
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

    updateManualBasket(urn, withAction) {
        if (withAction === "Add") {
            if (DfE.Util.ComparisonList.countManual() === 30) {
                DfE.Util.ComparisonList.RenderFullListWarningModalManual();
                return;
            }
        }

        $.get("/manualComparison/UpdateManualBasket?urn=" + urn + "&withAction=" + withAction,
            (data) => {
                if (data > 1) {
                    $(".manual-button").show();
                    $(".hidden-goto-basket").attr('aria-label',`Continue to benchmark charts with ${data} schools selected`);
                    $(".hidden-goto-basket").show();
                } else {
                    $(".manual-button").hide();
                    $(".hidden-goto-basket").hide();
                }
                $("#manualCount").text(data); 
                $("div[data-urn='" + urn + "']>.add-remove").toggle();                                
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
            $(".add-all-wrapper").hide();
        } else {
            $(".add-all-wrapper").show();
        }
    }

    activateAddRemoveButtons() {
        let $addRemoveButtons = $(".add-remove:visible");

        $addRemoveButtons.each(function () {
            var urn = $(this).parent().attr("data-urn");
            var inList;
            if ($("#SearchMethod").val() === "Manual") {
                inList = DfE.Util.ComparisonList.isInManualList(urn);
            } else {
                inList = DfE.Util.ComparisonList.isInList(urn);
            }
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

    switchPageFromPagingWidget(pageNo) {
        let pagingUrl = window.location.href.replace('Search?', 'Search-js?').replace('search?', 'search-js?');
        pagingUrl += '&page=' + pageNo;
        $.ajax({
            url: pagingUrl,
            datatype: 'json',
            beforeSend: () => {
                DfE.Util.LoadingMessage.display("#schoolResults", "Updating schools");
            },
            success: (data) => {
                $("#schoolResults").html(data);
                this.initTabs();
                this.initSort();
                this.liveSearch.updateSchoolCount();
            }
        });
    }

}//class

