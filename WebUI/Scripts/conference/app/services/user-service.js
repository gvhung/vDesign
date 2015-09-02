(function (angular) {
    function userService($log, $http, $dataService, $signalService) {

        var service = function () {
            var self = this;

            this.users = [];
            this.currentUser = null;
            //this.connected = false;

            this.getUser = function(userId) {
                if (self.users == null || !self.users.length) return null;

                for (var i in self.users) {
                    if (self.users[i].ID == userId) return self.users[i];
                }

                return null;
            };

            this.updateOnline = function (connections) {

                $log.info('Update online:', connections);

                angular.forEach(self.users, function (user) {
                    user.online = connections[user.ID] != undefined;
                });
            };

            this.load = function (callback) {
                $http.get($dataService.user.url.get)
                    .success(function (response) {
                        self.users = response;

                        self.updateOnline($signalService.current.getConnections());

                        callback(response);
                    });
            };

        };

        return new service();
    };

    userService.$inject = ['$log', '$http', '$dataService', '$signalService'];

    angular.module('Conference.services').
        factory('$userService', userService);

})(window.angular);