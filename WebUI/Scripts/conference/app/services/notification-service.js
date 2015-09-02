(function (angular) {
    function notificationService($log, $http, $userService, $conferenceService) {

        var service = function () {
            var self = this;

            this.shownotification = false;
            this.videoRequest = null;

            this.videoRequestNotification = function (dialogId, requestorId, dialogType) {
                var from = null, to = null, conference = null;

                from = $userService.getUser(requestorId);

                switch (dialogType) {
                    case "PrivateMessage":
                        to = $userService.getUser(dialogId);

                        self.videoRequest = {
                            dialogId: dialogId,
                            dialogType: dialogType,
                            from: from,
                            to: to,
                            conference: null,
                            selfRequest: requestorId === $userService.currentUser.ID
                        };
                        break;

                    case "PublicMessage":
                        if (requestorId !== $userService.currentUser.ID)
                            to = $userService.getUser($userService.currentUser.ID);

                        conference = $conferenceService.getConference(dialogId);

                        self.videoRequest = {
                            dialogId: dialogId,
                            dialogType: dialogType,
                            from: from,
                            to: to,
                            conference: conference,
                            selfRequest: requestorId === $userService.currentUser.ID
                        };

                        break;

                }

                self.shownotification = true;

                $log.info('Video request:', self.videoRequest);
            };

            this.videoRequestNotificationStop = function() {
                self.shownotification = false;
                self.videoRequest = null;
            };
        };

        return new service();
    };

    notificationService.$inject = ['$log', '$http', '$userService', '$conferenceService'];

    angular.module('Conference.services').
        factory('$notificationService', notificationService);

})(window.angular);