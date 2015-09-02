(function (angular) {
    function notificationController($scope, $location, $log, $dialogService, $signalService, $notificationService) {
        var self = this;

        this.showNotification = function() {
            return $notificationService.shownotification;
        };

        this.videoRequestExists = function() {
            return $notificationService.videoRequest != null;
        };


        this.getVideoRequest = function() {
            return $notificationService.videoRequest;
        };

        //this.getUserImage = function(userId) {
        //    var user = $userService.getUser(userId);
        //    return user && user.Image ? user.Image.FileID : null;
        //};

        this.callSuccess = function () {
            $log.info('Success');
            $signalService.hub.invoke('SuccessCall', $notificationService.videoRequest.dialogId, $notificationService.videoRequest.from.ID, $notificationService.videoRequest.dialogType);
            $notificationService.videoRequestNotificationStop();
        };

        this.callCancel = function() {
            $log.info('Cancel');
            $signalService.hub.invoke('CancelCall', $notificationService.videoRequest.dialogId, $notificationService.videoRequest.from.ID, $notificationService.videoRequest.dialogType);
            $notificationService.videoRequestNotificationStop();
        };
    }

    angular.module("ConferenceApp")
       .controller("NotificationController", notificationController);


    notificationController.$inject = ['$scope', '$location', '$log', '$dialogService', '$signalService', '$notificationService'];

})(window.angular);

//Переосмыслить, это все неудобно и негибко, сделано на скороту, бред бред