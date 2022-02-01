var app = angular.module('sadApp', [])
    .controller('SadPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;
                $scope.sad = null;
                var urn = DfE.Util.QueryString.get('urn');
                var fuid = DfE.Util.QueryString.get('fuid');
                var id = urn || fuid;

                self.loadData = function () {
                    $http.get('https://aa-t1dv-sfb.azurewebsites.net/api/selfassessment/' + id).then(function (response) {
                        $scope.sad = response.data;                        
                    });
                };

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

                self.phase = function () {
                    if ($scope.sad.hasSixthForm && $scope.sad.overallPhase == 'Secondary') {
                        return $scope.sad.overallPhase + ' with sixth form';
                        }
                    return $scope.sad.overallPhase;
                }

                self.loadData();
            }
        ]);

