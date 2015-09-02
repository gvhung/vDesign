(function (angular) {
    function routeTableService() {

        var service = function () {

            var self = this;

            this.routes = {
                '': true,
                '/contacts': false,
                '/dialogs': false,
            };

            this.setActive = function (r) {
                //console.log('current:', r);
                angular.forEach(self.routes, function (value, route) {
                    //value = r === route;
                    self.routes[route] = r === route;
                    //console.log(route, value);
                });
            };

        };

        return new service();
    };

    angular.module('Conference.services').
        factory('$routeTableService', routeTableService);

})(window.angular);