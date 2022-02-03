var app = angular.module('sadApp', [])
    .controller('SadPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;
                $scope.sad = null;
                var urn = DfE.Util.QueryString.get('urn');
                var fuid = DfE.Util.QueryString.get('fuid');
                $scope.id = urn || fuid;

                self.loadData = function () {
                    $http.get('https://aa-t1dv-sfb.azurewebsites.net/api/selfassessment/' + $scope.id).then(function (response) {
                        $scope.sad = response.data;
                        $scope.teachingStaff = self.getTeachingStaff();
                    });
                };

                self.getTeachingStaff = function () {
                    var ts = self.findAssessmentArea("Teaching staff");
                    var percentage = ((ts.schoolDataLatestTerm / ts.totalForAreaTypeLatestTerm) * 100).toFixed(1);
                    return {
                        percentage: percentage,
                        allBands: ts.allTresholds,
                        matchedBand: self.findMatchingBand(ts.allTresholds, Number(percentage) / 100)
                    };
                }

                self.phase = function () {
                    if ($scope.sad.hasSixthForm && $scope.sad.overallPhase == 'Secondary') {
                        return $scope.sad.overallPhase + ' with sixth form';
                    }
                    return $scope.sad.overallPhase;
                }

                self.findAssessmentArea = function (name) {
                    var selection = _.find($scope.sad.sadAssesmentAreas,
                        function (aa) {
                            return aa.assessmentAreaName === name;
                        });

                    return selection;
                }

                self.findMatchingBand = function (allBands, value) {
                    var selection = _.find(allBands,
                        function (band) {
                            return (band.scoreLow <= value) && (band.scoreHigh >= value);
                        });

                    selection.isMatch = true;
                    return selection;
                }

                //////
                self.loadData();
            }
        ]);

