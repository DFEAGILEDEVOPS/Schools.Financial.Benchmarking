var app = angular.module('reportApp', [])
    .controller('ChartListController',
        [
            '$scope', '$http', '$q',
            function($scope, $http, $q) {
                var self = this;
                self.format = "Charts";
                self.accordionInitialised = false;

                self.loadData = function (resolve) {
                    localStorage.removeItem('CustomCharts');
                    localStorage.removeItem('MyCharts');
                    if (localStorage.YourCharts) {
                        $scope.selectionList = JSON.parse(localStorage.YourCharts);
                        resolve();
                    } else {
                        $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function (response) {
                            $scope.selectionList = response.data;
                            localStorage.YourCharts = JSON.stringify($scope.selectionList);
                            resolve();
                        });
                    }
                };

                self.onSelectionChange = function () {
                    self.persist();
                    setTimeout(function () {
                        self.displayCustomReport();
                    }, 600);
                };

                self.persist = function () {
                    setTimeout(function () {
                        localStorage.YourCharts = JSON.stringify($scope.selectionList);
                    }, 500);
                };

                self.anySelected = function() {
                    return self.totalSelectCount() > 0;
                };

                self.clear = function () {
                    $http.get('/Assets/Scripts/AngularJS/allChartSelections.json').then(function(response) {
                        $scope.selectionList = response.data;
                        self.query = "";
                        self.onSelectionChange();
                    });
                };

                self.openDetails = function () {
                    $("#customTabSection button.govuk-accordion__open-all:contains('Open')").click();
                };

                self.displayCustomReport = function () {
                    sessionStorage.chartFormat = self.format;
                    $.ajax({
                        type: "POST",
                        url: "/benchmarkcharts/CustomReport",
                        datatype: "json",
                        data: { "json": localStorage.YourCharts, "format": self.format },
                        beforeSend: function () {
                            $('#CustomReportContentPlaceHolder').html(' ');
                            $('#spinner-place-holder').show();
                        },
                        success: function (data) {
                            setTimeout(function () {
                                $('#spinner-place-holder').hide();
                                $('#CustomReportContentPlaceHolder').html(data); 
                                DfE.Views.BenchmarkCharts.generateCharts();
                                $("table.data-table-js.chart-table--mobile-above-view").tablesorter({ sortList: [[$("table.data-table-js.chart-table--mobile-above-view").first().find("thead th").length - 1, 1]] });
                                $("table.data-table-js.chart-table--mobile-only-view.chart-table--summary-view").tablesorter({ sortList: [[$("table.data-table-js.chart-table--mobile-only-view.chart-table--summary-view").first().find("thead th").length - 1, 1]] });
                                $("table.data-table-js.includes-table").tablesorter({ sortList: [[1, 1]] });
                                if (!self.accordionInitialised) {
                                    window.GOVUKFrontend.initAll({ scope: $("#customTabSection .selections")[0] });
                                    self.accordionInitialised = true;
                                }
                            }, 500);
                        }
                    });
                };

                self.groupSelectCount = function (group) {
                    var count = _.reduce(group.Charts,
                        function(sum, ch) {
                            return sum +
                                (ch.PerPupilSelected ? 1 : 0) +
                                (ch.PerTeacherSelected ? 1 : 0) +
                                (ch.PercentageExpenditureSelected ? 1 : 0) +
                                (ch.PercentageIncomeSelected ? 1 : 0) +
                                (ch.AbsoluteMoneySelected ? 1 : 0) +
                                (ch.AbsoluteCountSelected ? 1 : 0) +
                                (ch.HeadCountPerFTESelected ? 1 : 0) +
                                (ch.PercentageOfWorkforceSelected ? 1 : 0) +
                                (ch.NumberOfPupilsPerMeasureSelected ? 1 : 0)+
                                (ch.PercentageTeachersSelected ? 1 : 0);
                        },
                        0);
                    return count;
                };

                self.totalSelectCount = function () {
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

                $scope.dataLoaded = $q(function (resolve) {
                    self.loadData(resolve);
                });

                $(document).ready(function() {
                    $scope.schoolChartData = JSON.parse(($('.chart.c3').first()).attr('data-chart'));
                    $scope.homeSchoolName = $('#HomeSchoolName').val();
                });
            }
        ]);

