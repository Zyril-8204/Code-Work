var AngularTest = angular.module('AngularTest', ['ui.router', 'ui.bootstrap']);

AngularTest.controller('LandingPageController', LandingPageController);
AngularTest.controller('LoginController', LoginController);
AngularTest.controller('RegisterController', RegisterController);

AngularTest.factory('AuthHttpResponseInterceptor', AuthHttpResponseInterceptor);
AngularTest.factory('LoginFactory', LoginFactory);
AngularTest.factory('RegistrationFactory', RegistrationFactory);

var configFunction = function ($stateProvider, $httpProvider, $locationProvider) {

    $locationProvider.hashPrefix('!').html5Mode(true);

    $stateProvider
    .state('stateOne', {
        url: '/stateOne?id',
        views: {
            "containerOne": {
                templateUrl: 'routesTest/one'
            },
            "ContainerTwo": {
                templateUrl: function (params) { return '/routesTest/two?id=' + params.id }
            }
        }
    })
    .state('stateTwo', {
        url: '/stateTwo',
        views: {
            "containerOne": {
                templateUrl: '/routesTest/Two'
            },
            "containerTwo": {
                templateUrl: '/routesTest/three'
            }
        }
    })
    .state('stateThree', {
        url: '/stateThree?id',
        views: {
            "containerOne": {
                templateUrl: function (params) { return '/routesTest/two?id=' + params.id; }
            },
            "containerTwo": {
                templateUrl: '/routesTest/three'
            }
        }
    })
    .state('stateFour', {
        url: '/stateFour',
        views: {
            "containerOne": {
                templateUrl: '/routesTest/four'
            },
            "containerTwo": {
                templateUrl: '/routesTest/three'
            }
        }
    })
    .state('loginRegister', {
        url: '/loginRegister?returnUrl',
        views: {
            "containerOne": {
                templateUrl: '/Account/Login',
                controller: LoginController
            },
            "containerTwo": {
                templateUrl: 'Account/Register',
                controller: RegisterController
            }
        }
    });

    $httpProvider.interceptors.push('AuthHttpResponseInterceptor');
}

configFunction.$inject = ['$stateProvider', '$httpProvider', '$locationProvider'];

AngularTest.config(configFunction);