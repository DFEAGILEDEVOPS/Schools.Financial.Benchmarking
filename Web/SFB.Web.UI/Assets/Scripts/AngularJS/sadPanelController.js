app.controller('SadPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;
                $scope.isMobile = $(window).width() <= 640;
                $scope.id = DfE.Util.QueryString.get('urn') || DfE.Util.QueryString.get('fuid');

                $scope.sad = null;
                $http.get('https://' + $("#SfbApiUrl").val() +'/api/selfassessment/' + $scope.id).then(function (response) {
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
                    self.setActiveTab("Teaching staff");
                });

                self.setActiveTab = function (tabName, event) {
                    if (event) {
                        event.preventDefault();
                    }

                    $scope.activeTab = tabName;
                }

                self.getAAbyName = function (name) {
                    var aa = findAssessmentArea(name);
                    var percentage = ((aa.schoolDataLatestTerm / aa.totalForAreaTypeLatestTerm) * 100).toFixed(1);
                    return {
                        name: aa.assessmentAreaName.replace("Administrative", "Admin."),
                        percentage: percentage,
                        allBands: aa.allTresholds,
                        matchedBand: findMatchingBand(aa.allTresholds, (Number(percentage) / 100).toFixed(3))
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
                        if (typeof selection !== 'undefined') {
                          selection.isMatch = true
                          return selection;
                        }
                    }
                }

                self.getPhase = function () {
                    if ($scope.sad.hasSixthForm && $scope.sad.overallPhase == 'Secondary') {
                        return $scope.sad.overallPhase + ' with sixth form';
                    }
                    return $scope.sad.overallPhase;
                }

                $(window).resize(function () {
                    $scope.$apply(function () {
                         $scope.isMobile = $(window).width() <= 640;
                    });
                });

            }
        ]);

