app.controller('QuickComparisonPanelController',
        [
            '$scope', '$http',
            function ($scope, $http) {
                var self = this;
                debugger;
                $scope.isMobile = $(window).width() <= 640;
                $scope.id = DfE.Util.QueryString.get('urn') || DfE.Util.QueryString.get('fuid');

                $scope.compList = null;
                $http.get('BenchmarkCharts/GetSchoolListFromSimpleCriteria?id=' + $scope.id).then(function (response) {
                    $scope.compList = response.data;
                });

                $(window).resize(function () {
                    $scope.$apply(function () {
                         $scope.isMobile = $(window).width() <= 640;
                    });
                });

            }
        ]);

