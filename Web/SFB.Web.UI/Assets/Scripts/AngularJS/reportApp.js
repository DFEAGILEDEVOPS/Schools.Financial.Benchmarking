var app = angular.module('reportApp', [])
    .controller('ChartListController',
        [
            '$scope', '$location', '$http', '$q',
            function($scope, $location, $http, $q) {
                var self = this;
                self.location = $location;
                self.showContextTable = true;
                self.inReportView = false;
                self.format = "Charts";

                self.loadData = function(resolve) {
                    if (localStorage.CustomCharts) {
                        $scope.selectionList = JSON.parse(localStorage.CustomCharts);
                        resolve();
                    } else {
                        $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function(response) {
                            $scope.selectionList = response.data;
                            resolve();
                        });
                    }
                }

                self.persist = function () {
                    setTimeout(function () {
                        localStorage.CustomCharts = JSON.stringify($scope.selectionList);
                    }, 500);                    
                }

                self.anySelected = function() {
                    return self.totalSelectCount() > 0;
                };

                self.clear = function() {
                    $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function(response) {
                        $scope.selectionList = response.data;
                        self.persist();
                        self.query = "";
                        setTimeout(function() { new Accordion(document.getElementById('custom-report-accordion')); },
                            500);
                    });
                };

                self.openDetails = function() {
                    $("#customTabSection button.accordion-expand-all:contains('Open')").click();

                }

                self.displayCustomReport = function () {
                    $.ajax({
                        type: "POST",
                        url: "/benchmarkcharts/CustomReport",
                        datatype: "json",
                        data: { "json": localStorage.CustomCharts, "format": self.format },
                        beforeSend: function() {
                            DfE.Util.LoadingMessage.display("#CustomReportContentPlaceHolder", "Generating custom benchmarking report");
                        },
                        success: function (data) {
                            setTimeout(function() {
                                $('#CustomReportContentPlaceHolder').html(data);
                                $("#BCHeader").text("Custom benchmarking report");
                                $("#PrintLinkText").text(" Print report");
                                $("#benchmarkBasket").hide();
                                $(".download-links").show();
                                $("#searchSchoolsLink").hide();
                                $("#downloadLinkContainer").hide();
                                $("#BackToBMCharts").show();
                                DfE.Views.BenchmarkChartsViewModel.GenerateCharts();
                                $("table.dataTable").tablesorter();
                            }, 500);
                        }
                    });
                    self.location.url("report");
                    self.inReportView = true;
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

                angular.element(document).ready(function () {
                    new Accordion(document.getElementById('custom-report-accordion'));
                    $("#custom-report-accordion .accordion-section-header").first().click();
                });

                $(document).ready(function() {
                    $scope.schoolChartData = JSON.parse(($('.chart.c3').first()).attr('data-chart'));
                });
            }
        ]);

//app.run(function($rootScope, $location) {
//    $rootScope.$on('$locationChangeStart',
//        function() {
//            if ($location.$$urlUpdatedByLocation) {
//                window.location.reload();
//            }
//        });
//});