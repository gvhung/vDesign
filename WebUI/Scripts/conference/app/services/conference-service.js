(function (angular) {
    function conferenceService($log, $http, $dataService) {

        var service = function () {
            var self = this;

            this.conferences = [];

            this.getConference = function (conferenceId) {

                if (self.conferences == null || !self.conferences.length) return null;

                for (var i in self.conferences) {
                    if (self.conferences[i].ID == conferenceId) return self.conferences[i];
                }

                return null;
            };

            this.load = function (callback) {
                $http.get($dataService.conference.url.get)
                    .success(function (response) {
                        self.conferences = response;

                        callback(response);
                    });
            };

            this.create = function (conference, callback) {
                $http.post($dataService.conference.url.create, conference)
                    .success(function (response) {
                        self.conferences.push(response);

                        if (callback)
                            callback(response);
                    });
            };


        };

        return new service();
    };

    conferenceService.$inject = ['$log', '$http', '$dataService'];

    angular.module('Conference.services').
        factory('$conferenceService', conferenceService);

})(window.angular);