var app = angular.module('sadApp', [])
    .controller('SadPanelController',
        [
            '$scope', '$http', '$q',
            function ($scope, $http, $q) {
                var self = this;
                $scope.sad = null;

                self.loadData = function (resolve) {

                    $http.get('https://aa-t1dv-sfb.azurewebsites.net/api/selfassessment/125271').then(function (response) {
                        $scope.sad = response.data;
                        resolve();
                    });

                };

                //self.onSelectionChange = function () {
                //    self.persist();
                //    setTimeout(function () {
                //        self.displayCustomReport();
                //    }, 600);
                //};

                //self.totalSelectCount = function () {
                //    var count = 0;
                //    if ($scope.selectionList) {
                //        count = _.reduce($scope.selectionList.HierarchicalCharts,
                //            function (sum, group) {
                //                return sum + self.groupSelectCount(group);
                //            },
                //            0);
                //    }
                //    return count;
                //};

                $scope.dataLoaded = $q(function (resolve) {
                    self.loadData(resolve);
                });

                $(document).ready(function () {
                    //$scope.schoolChartData = JSON.parse(($('.chart.c3').first()).attr('data-chart'));
                    //$scope.homeSchoolName = $('#HomeSchoolName').val();
                });

                self.phase = function () {
                    if ($scope.sad.hasSixthForm && $scope.sad.overallPhase == 'Secondary') {
                        return $scope.sad.overallPhase + ' with sixth form';
                        }
                    return $scope.sad.overallPhase;
                }
            }
        ]);

