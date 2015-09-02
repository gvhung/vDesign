(function (angular) {
    function mainController($scope, $rootScope, $location, $element, $interval, $log, $loadingpool, $signalService, $userService, $dialogService, $conferenceService, $routeTableService, $notificationService, uuid2, $rtcService) {

        var self = this;

        this.dataloaded = false;
        this.user = null;

        this.routeTable = {
            '/phones': false,
        }

        $loadingpool.on('users');
        $loadingpool.on('conferences');
        $loadingpool.on('unreaded');

        $dialogService.loadPrivateUnread(function (response) {
            self.dataloaded = $loadingpool.off('unreaded');
        });

        $userService.load(function (response) {
            self.dataloaded = $loadingpool.off('users');
        });

        $conferenceService.load(function (response) {
            self.dataloaded = $loadingpool.off('conferences');
        });

        $signalService.hub.on('Confirmed', function (data) {
            //$log.info(data);
        });

        $signalService.hub.on('OnSignIn', function (connections) {
            //$userService.connected = true;
            $signalService.current.setConnections(connections);

            $userService.updateOnline(connections);
            $scope.$applyAsync();
        });

        $signalService.hub.on('OnSignOut', function (connections) {
            $signalService.current.setConnections(connections);

            $userService.updateOnline(connections);
            $scope.$applyAsync();
        });

        $signalService.hub.on('OnNewConference', function (conference) {
            $log.info('On after update conference', conference);

            $conferenceService.conferences.push(conference);

            $scope.$applyAsync();
        });

        $signalService.hub.on('OnTextMessageSend', function (message, dialogType) {
            if (message == null) return;

            $dialogService.addMessage({
                message: message,
                dialogType: dialogType
            }, function() {
                $scope.$applyAsync();
            });
        });

        //OnSendVideoRequest
        $signalService.hub.on('OnSendVideoRequest', function (dialogId, requestorId, dialogType) {
            $notificationService.videoRequestNotification(dialogId, requestorId, dialogType);
            $scope.$applyAsync();
        });

        //OnCallSuccess
        $signalService.hub.on('OnCallSuccess', function (dialogId, userId, dialogType) {
            $log.info('Success call', dialogId, userId, dialogType);
            $notificationService.videoRequestNotificationStop();

            switch (dialogType) {
                case "PrivateMessage":
                    var user = $userService.getUser(userId);

                    if (user) {

                        $dialogService.openDialog({
                            dialogId: dialogId,
                            name: user.FullName,
                            type: dialogType,
                            active: true,
                            visible: true,
                            load: true,
                        }, function () {
                            //Messages on load
                        });

                        $location.path('/dialogs');

                        var videoKey = uuid2.newuuid();

                        $rtcService.startVideoConference(videoKey, function () {
                            $scope.$applyAsync();
                            //On start callback
                            $signalService.hub.invoke('StartVideoConference', user.ID, videoKey, dialogType);
                        });
                    }
                break;
            }

            $scope.$applyAsync();
        });

        //OnStartVideoConference
        $signalService.hub.on('OnStartVideoConference', function (dialogId, key, dialogType) {
            $log.info('Start conference', dialogId, key, dialogType);

            switch (dialogType) {
                case "PrivateMessage":
                    var user = $userService.getUser(dialogId);

                    if (user) {

                        $dialogService.openDialog({
                            dialogId: dialogId,
                            name: user.FullName,
                            type: dialogType,
                            active: true,
                            visible: true,
                            load: true,
                        }, function () {
                            //Messages on load
                        });

                        $location.path('/dialogs');

                        

                        $rtcService.joinVideoConference(key, function () {
                            //On start callback
                            //$signalService.hub.invoke('StartVideoConference', user.ID, videoKey, dialogType);
                            $scope.$applyAsync();
                        });
                    }
                    break;
            }

            //join video

            $scope.$applyAsync();
        });


        //OnCallCancel
        $signalService.hub.on('OnCallCancel', function (dialogId, userId, dialogType) {
            $log.info('Cancel call', dialogId, userId, dialogType);
            $notificationService.videoRequestNotificationStop();
            $scope.$applyAsync();
        });


        $signalService.connection.start().done(function () {
            $signalService.hub.invoke('SignIn');
            $interval($signalService.confirmation, 45000);
        });

        $scope.$watch(angular.bind(this, function () {
            return self.user;
        }), function (newValue, oldValue) {
            $userService.currentUser = newValue;
        }, true);


        $scope.$watch(angular.bind(this, function () {
            return self.dialogs;
        }), function (newValue, oldValue) {
            if (newValue && newValue !== oldValue) {

            }
        }, true);

        this.isActiveRoute = function (location) {
            return location === $location.path();
        }

        this.onlineCount = function() {
            return $userService.users.filter(function (user) {
                return user.ID !== $userService.currentUser.ID && user.online !== undefined && user.online === true;
            }).length;
        };

        this.unreadCount = function() {
            var count = 0;
            angular.forEach($dialogService.unreaded, function (unread) {
                count += unread.Count;
            });
            return count;
        };

        this.dialogsCount = function () {
            return $dialogService.dialogs.filter(function (dialog) {
                return dialog.visible;
            }).length;
        };
    }

    angular.module("ConferenceApp")
        .controller("MainController", mainController);

    mainController.$inject = ['$scope', '$rootScope', '$location', '$element', '$interval', '$log', '$loadingpool', '$signalService', '$userService', '$dialogService', '$conferenceService', '$routeTableService', '$notificationService', 'uuid2', '$rtcService'];


})(window.angular);