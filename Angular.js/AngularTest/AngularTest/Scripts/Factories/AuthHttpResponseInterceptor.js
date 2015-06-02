var AuthHttpResponseInterceptor = function ($q, $location, $injector) {
    return {
        response: function (response) {
            if (response.status === 401) {
                console.log('Response 401');
            }
            return response || $q.when(response);
        },
        responseError: function (rejction) {
            if (rejction.status === 401) {
                $injector.get('$state').go('loginRegister', { returnUrl: $location.path() });
            }
            return $q.reject(rejction);
        }
    }
}

AuthHttpResponseInterceptor.$inject = ['$q', '$location', '$injector'];