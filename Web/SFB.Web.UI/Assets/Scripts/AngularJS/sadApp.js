var app = angular.module('sadApp', [])
    .controller('SadPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;

                $scope.sad = null;
                $scope.id = DfE.Util.QueryString.get('urn') || DfE.Util.QueryString.get('fuid');

                self.loadData = function () {
                    $http.get('https://aa-t1dv-sfb.azurewebsites.net/api/selfassessment/' + $scope.id).then(function (response) {
                        $scope.sad = response.data;
                        $scope.assessmentAreas = [
                            self.getAAbyName("Teaching staff"),
                            self.getAAbyName("Supply staff"),
                            self.getAAbyName("Education support staff"),
                            self.getAAbyName("Administrative and clerical staff"),
                            self.getAAbyName("Other staff costs"),
                            self.getAAbyName("Premises costs"),
                            self.getAAbyName("Educational supplies"),
                            self.getAAbyName("Energy")

                        ];
                    });
                };

                self.getAAbyName = function (name) {
                    var aa = findAssessmentArea(name);
                    var percentage = ((aa.schoolDataLatestTerm / aa.totalForAreaTypeLatestTerm) * 100).toFixed(1);
                    return {
                        name: aa.assessmentAreaName,
                        percentage: percentage,
                        allBands: aa.allTresholds,
                        matchedBand: findMatchingBand(aa.allTresholds, Number(percentage) / 100)
                    };

                    function findAssessmentArea(name) {
                        var selection = $scope.sad.sadAssesmentAreas.find(
                            function (aa) {
                                return aa.assessmentAreaName === name;
                            });
                        return selection;
                    }

                    function findMatchingBand (allBands, value) {
                        var selection = allBands.find(
                            function (band) {
                                return (band.scoreLow <= value) && (band.scoreHigh >= value);
                            });

                        selection.isMatch = true;
                        return selection;
                    }
                }

                self.getPhase = function () {
                    if ($scope.sad.hasSixthForm && $scope.sad.overallPhase == 'Secondary') {
                        return $scope.sad.overallPhase + ' with sixth form';
                    }
                    return $scope.sad.overallPhase;
                }

                //////
                self.loadData();
            }
        ]);

