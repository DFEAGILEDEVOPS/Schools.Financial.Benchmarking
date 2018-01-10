var app = angular.module('reportApp', [])
    .controller('ChartListController',
        [
            '$scope', '$location', '$http', '$q',
            function ($scope, $location, $http, $q) {
                var self = this;
                self.location = $location;

                self.loadData = function(resolve) {
                    if (localStorage.CustomCharts) {
                        $scope.selectionList = JSON.parse(localStorage.CustomCharts);
                        resolve();
                    } else {
                        $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function (response) {
                            $scope.selectionList = response.data;
                            resolve();
                        });
                    }                    
                }

                self.persist = function ()
                {
                    localStorage.CustomCharts = JSON.stringify($scope.selectionList);
                }

                self.anySelected = function() {
                    return self.totalSelectCount() > 0;
                };

                self.clear = function() {
                    $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function(response) {
                        $scope.selectionList = response.data;
                        self.persist();
                        self.query = "";
                        $.getScript("/public/scripts/termi/element.details.ajax.js");//required for accordions in IE
                    });
                };

                self.openDetails = function () {
                    if (self.query) {
                        $("details").attr("open", "open");
                    }
                }

                self.displayCustomReport = function() {
                      $.post("/benchmarkcharts/CustomReport", { "json": localStorage.CustomCharts })
                        .done(function(data) {
                            $('#CustomReportContentPlaceHolder').html(data);
                            $("#BCHeader").text("Custom benchmarking report");
                            $("#PrintLinkText").text(" Print report");
                            $("#benchmarkBasket").hide();
                            $("#searchSchoolsLink").hide();
                            $("#downloadLinkContainer").hide();
                            $("#BackToBMCharts").show();
                            GenerateCharts();
                            $("table").tablesorter();

                        });
                    self.location.url("report");;
                };

                self.groupSelectCount = function(group) {
                    var count = _.reduce(group.Charts,
                        function(sum, ch) {
                            return sum +
                                (ch.PerPupilSelected ? 1 : 0) +
                                (ch.PerTeacherSelected ? 1 : 0) +
                                (ch.PercentageSelected ? 1 : 0) +
                                (ch.AbsoluteMoneySelected ? 1 : 0) +
                                (ch.AbsoluteCountSelected ? 1 : 0) +
                                (ch.HeadCountPerFTESelected ? 1 : 0) +
                                (ch.PercentageOfWorkforceSelected ? 1 : 0) +
                                (ch.NumberOfPupilsPerMeasureSelected ? 1 : 0);
                        },
                        0);

                    return count;
                };

                self.totalSelectCount = function() {
                    var count = 0;
                    if ($scope.selectionList) {
                        count = _.reduce($scope.selectionList.HierarchicalCharts,
                            function(sum, group) {
                                return sum + self.groupSelectCount(group);
                            },
                            0);
                    }
                    return count;
                };

                $scope.dataLoaded = $q(function(resolve) {
                    self.loadData(resolve);
                });
            }
        ]);

app.run(function($rootScope, $location) {
    $rootScope.$on('$locationChangeStart',
        function() {
            if ($location.$$urlUpdatedByLocation) {
                window.location.reload();
            }
        });
});