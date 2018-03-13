(function (GOVUK, Views) {
    'use strict';

    function SchoolsResultsViewModel(location, activeTab) {
        this.cache = {};
        this.bindEvents();
        this.location = location;
        this.currentTabName = activeTab;

        if (activeTab == "map") {
            this.bindMap();
            this.liveSearch.disabled = true;
        }
    }

    SchoolsResultsViewModel.prototype = {

        bindEvents: function() {
            // Live Search Form (aka filter form)
            var $results = $('#schoolResults'),
                $atomAutodiscoveryLink = $("link[type='application/atom+xml']").eq('0');

            this.liveSearch = new GOVUK.LiveSearch({
                formId: "SearchFacetsForm",
                secondaryFormId : "SearchFacetsForm2",
                $results: $results,
                $atomAutodiscoveryLink: $atomAutodiscoveryLink,
                onRefresh: this.onRefresh.bind(this),
                onDisplayResults: this.onDisplayResults.bind(this),
                resultsViewModel: this // really, this component and LiveSearch should be merged together
            });

            window.addEventListener("load", this.load.bind(this));
            this.bindTracking();
        },
        getTabName: function () {
            return !this.currentTabName ? "list" : this.currentTabName;
        },
        load: function() {
            this.trackRadiusSearch();
            this.initTabs();
            SchoolsResultsViewModel.AddAllVisibility();
        },
        onDisplayResults: function (state) {
            var serialisedState = $.param(state);
            this.getMapData(serialisedState);
        },
        initTabs: function () {
            this.currentTabName = this.initialTabName = $("nav.navigation-links ol li.active").data("tab") === "map" ? "list" : "map";
            var self = this;
            $(".navigation-link").click(function (e) {
                self.changeTab($(this).data("tab"));
                e.preventDefault();
                return false;
            });
        },
        trackRadiusSearch: function() {
            var item = $("#DistanceRadius option:selected");
            if (item.length > 0) this.trackFilter("Radius: " + item.text().trim());
        },
        trackFilter: function(label) {
            DfE.Util.Analytics.TrackEvent('search-results', label.trim(), 'filter');
        },
        bindTracking: function() {
            var self = this;
            $("#DistanceRadius").change(this.trackRadiusSearch.bind(this));
            $(".js-live-search-results-block input[type=checkbox]").change(function() {
                if ($(this).is(":checked")) {
                    var facetLabel = $(this).parent().text();
                    var facetGroupName = $(this).closest("div.govuk-option-select").find("div.option-select-label")
                        .text();
                    self.trackFilter(facetGroupName.trim() + "/" + facetLabel.trim());
                }
            });
        },

        bindEditSearchButton: function() {
            GOVUK.Collapsible.bindElements("#EditSearchCollapsible.js-collapsible");
        },
        bindFilterCollapseButtons: function() {
            // Instantiate an option select for each one found on the page
            var filters = $('.govuk-option-select').map(function() {
                return new GOVUK.OptionSelect({ $el: $(this) });
            });
        },
        changeTab: function (tabName, suppressAddHistory) {
            if ($(".navigation-links li.active").data("tab") === tabName) {

                $("nav.navigation-links ol li,  div.tabs>div").removeClass("active");
                $("nav.navigation-links ol li." + tabName + ", div.tabs>div." + tabName).addClass("active");
                $("nav.navigation-links ol li.active a").focus();
                this.currentTabName = tabName;
                this.bindMap();
                this.liveSearch.disabled = (tabName == "map");
                if (tabName == "list") {
                    this.liveSearch.updateResults.bind(this.liveSearch).call(null);
                }
                this.liveSearch.tabChange(suppressAddHistory);
                if (tabName == "map" && this.count > 1000) {
                    $(".map-view-qualifier").show();
                } else {
                    $(".map-view-qualifier").hide();
                }
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
        bindMap: function () {
            if (!this.mapLoaded && this.currentTabName == "map") {
                var zoomLevel = 12;
                if (this.location == null) {
                    this.location = { lat: 52.636, lng: -1.139 }; // no location specified, so use central England.
                    zoomLevel = 6;
                }

                this.centrePoint = new google.maps.LatLng(this.location.lat, this.location.lng);
                this.map = new google.maps.Map(document.getElementById("gmap"),
                    {
                        center: this.centrePoint,
                        zoom: zoomLevel,
                        streetViewControl: false,
                        scrollwheel: false,
                        disableDefaultUI: true,
                        mapTypeControl: true,
                        mapTypeControlOptions: {
                            position: google.maps.ControlPosition.TOP_RIGHT
                        },
                        fullscreenControl: true,
                        fullscreenControlOptions: {
                            position: google.maps.ControlPosition.RIGHT_BOTTOM
                        },
                        zoomControl: true,
                        zoomControlOptions: {
                            position: google.maps.ControlPosition.TOP_LEFT
                        }
                    });
                setTimeout(function () {
                        $(".gm-style").children().first().attr("aria-label", "A google map of the school locations");
                    },
                    1500);
                this.mapLoaded = true;
                this.map.addListener("click", function () {
                    if (this.infoWindow) this.infoWindow.close();
                    if (this.activeMarker) this.activeMarker.setIcon(this.iconBlack);
                    this.activeMarker = null;
                }.bind(this));
            }

            if (this.currentTabName == "map") {
                //this.liveSearch.showLoadingIndicator();
                this.getMapData(this.liveSearch.$form.serialize());
            }

            if (!this.iconBlack) {
                this.iconBlack = { url: "/public/assets/images/icons/icon-location.png", scaledSize: new google.maps.Size(20, 32) };
                this.iconPink = { url: "/public/assets/images/icons/icon-location-pink.png", scaledSize: new google.maps.Size(20, 32) };
            }
        },
        getMapData: function (serialisedState) {
            if (this.currentTabName != "map") return;

            if (this.cache[serialisedState]) this.renderMapPins(this.cache[serialisedState], serialisedState);
            else {
                var self = this;
                return $.ajax({
                    url: "/SchoolSearch/search-json",
                    data: serialisedState
                }).done(function (response) {
                    self.cache[serialisedState] = response;
                    self.renderMapPins(response, serialisedState);
                }).error(function (error) {
                    console.log("Error loading map pins: " + error);
                });
            }
        },
        renderMapPins: function (response, serialisedState) {
            var self = this;
            var data = response.results;
            var count = response.count;
            self.count = count;
            console.log("renderMapPins: " + self.count);
            this.rawMapData = data;
            this.infoWindow = new google.maps.InfoWindow();
            this.infoWindow.addListener("closeclick", function () {
                if (self.activeMarker) self.activeMarker.setIcon(self.iconBlack);
                self.activeMarker = null;
            });

            $("span.result-count").html(count);
            this.liveSearch.getSummaryBlock().css("visibility", "visible");
            this.liveSearch.getSummaryContainerBlock().find("p.msg").remove();

            if (this.markerCluster) this.markerCluster.clearMarkers();

            var markers = [];
            var coords = [];
            var hashtable = {};
            var genKey = function (lat, lng) { return lat + "#" + lng; };

            for (var i = 0; i < data.length; i++) {

                // This is where we scatter any pins that have the exact same co-ordinates
                var adjustment = 0.00005; // put the school pin about 6 metres away from it's equivalent.
                var lat = new Number(data[i].Latitude), lng = new Number(data[i].Longitude);
                var key = genKey(lat, lng);
                if (!hashtable[key]) hashtable[key] = key;
                else {
                    lng += adjustment;
                    key = genKey(lat, lng);
                    hashtable[key] = key;
                }

                var latLng = new google.maps.LatLng(lat, lng);
                var marker = new google.maps.Marker({ position: latLng, icon: this.iconBlack });

                markers.push(marker);
                coords.push(latLng);

                window.google.maps.event.addListener(marker, "click", (function (m, info, infoWindow) {
                    return function (evt) {
                        var html = "<div class=\"infowindow-school-summary\">";
                        html += "<a href=\"/school/detail?urn=" + info.Id + "\">" + info.Name + "</a>";
                        html += "<p>" + info.Address + "</p>";
                        html += "<p>" + info.EducationPhases + "</p>";
                        html += "<p>" + info.NFType + "</p>";
                        html += "<div id=\"" + info.Id + "\" data-urn=\"" + info.Id + "\">";
                        if (DfE.Util.ComparisonList.isInList(info.Id)) {
                            html += "<div class=\"button add add-remove\"" + "style=\"display: none\"" + "onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket(" + info.Id + ",'addtocompare')\">Add</div>";
                            html += "<div class=\"button remove add-remove\"" + "onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket(" + info.Id + ",'removefromcompare')\">Remove</div>";
                        } else {
                            html += "<div class=\"button add add-remove\"" + "onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket(" + info.Id + ",'addtocompare')\">Add</div>";
                            html += "<div class=\"button remove add-remove\"" + "style=\"display: none\"" + "onclick=\"DfE.Views.SchoolsResultsViewModel.UpdateBenchmarkBasket(" + info.Id + ",'removefromcompare')\">Remove</div>";
                        }
                        html += "</div></div>";
                        infoWindow.setContent(html);
                        infoWindow.open(this.map, m);
                        m.setIcon(self.iconPink);
                        if (self.activeMarker) self.activeMarker.setIcon(self.iconBlack);
                        self.activeMarker = m;
                    }
                })(marker, data[i], this.infoWindow));

                window.google.maps.event.addListener(marker, "mouseover", (function (m) {
                    return function (evt) {
                        if (!self.activeMarker) m.setIcon(self.iconPink);
                    }
                })(marker));

                window.google.maps.event.addListener(marker, "mouseout", (function (m) {
                    return function (evt) {
                        if (!self.activeMarker) m.setIcon(self.iconBlack);
                    }
                })(marker));
            }

            this.markerCluster = new MarkerClusterer(this.map, markers, { imagePath: "/public/js-marker-clusterer/images/m", minimumClusterSize: 5 });

            var bounds = new google.maps.LatLngBounds();
            for (var i = 0; i < coords.length; i++) bounds.extend(coords[i]);
            if (coords.length > 0) {
                this.map.fitBounds(bounds);
            } else {
                this.map.setCenter(this.centrePoint);
                this.map.setZoom(7);
            }

            if (self.count > 1000) $(".map-view-qualifier").show();
            else $(".map-view-qualifier").hide();
        }
    };
    
    SchoolsResultsViewModel.UpdateBenchmarkBasket = function (urn, withAction) {
        if (withAction === "addtocompare") {
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
            self.$addButtons.each(function() {
                var urn = $(this).attr("data-urn");
                urns.push(urn);
            });
            $.post("/school/UpdateBenchmarkBasketAddMultiple",
                { 'urns[]': urns },
                function(data) {
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
    SchoolsResultsViewModel.Load = function (loc, tab) {
        new DfE.Views.SchoolsResultsViewModel(loc, tab);
    };

    Views.SchoolsResultsViewModel = SchoolsResultsViewModel;

}(GOVUK, DfE.Views));

function updateMainFilterForm(form, field, value) {
    $('#' + form).find('#' + field).val(value);
}