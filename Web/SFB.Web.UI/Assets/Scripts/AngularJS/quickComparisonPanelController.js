app.controller('QuickComparisonPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;

                $scope.isMobile = $(window).width() <= 640;
                $scope.id = DfE.Util.QueryString.get('urn') || DfE.Util.QueryString.get('fuid');

                $scope.format = "Charts";
                $scope.activeTab = "Total expenditure";

                $scope.compList = null;
                $http.get('/benchmarkCharts/GetSchoolListFromSimpleCriteria?id=' + $scope.id).then(function (response) {
                    $scope.compList = response.data;
                    self.renderQcChart();
                });

                self.changeQcTab = function(chartName, event) {

                    if (event) {
                        event.preventDefault();
                    }

                    $scope.activeTab = chartName;

                    this.renderQcChart();
                }

                self.switchTo = function (tableOrChart) {
                    $scope.format = tableOrChart;
                    self.renderQcChart();
                }

                self.renderQcChart = function() {

                    let url = "/benchmarkcharts/getQCChart?id=" + $scope.id + "&chartGroup=TotalExpenditure&chartName=" + encodeURI($scope.activeTab);

                    if ($scope.format) {
                        url += "&format=" + $scope.format;
                    }

                    $.ajax({
                        url: url,
                        datatype: 'json',
                        method: 'post',
                        data: { "schools": $scope.compList.schools },
                        beforeSend: () => {
                            DfE.Util.LoadingMessage.display("#benchmarkChartsList", "Loading charts");
                        },
                        success: (data) => {
                            $("#benchmarkChartsList").html(data);

                            DfE.Views.BenchmarkCharts.generateCharts();

                            $("#benchmarkChartsList table.data-table-js").tablesorter({ sortList: [[$("#benchmarkChartsList table.data-table-js").first().find("thead th").length - 1, 1]] });
                        }
                    });
                }

                $(window).resize(function () {
                    $scope.$apply(function () {
                        $scope.isMobile = $(window).width() <= 640;
                    });
                });
            }
        ]);

