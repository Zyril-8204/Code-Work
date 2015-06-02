var LandingPageController = function($scope) {
    $scope.models = {
        helloAngular:'My Angular Test App!'
    };

    $scope.navbarProperties = {
        isCollapsed: true
    };
}

LandingPageController.$inject = ['$scope'];