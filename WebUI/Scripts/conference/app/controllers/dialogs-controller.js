(function (angular) {
    function dialogsController($scope, $log, $interval, $dateParser, $element, $sce, $filter, $loadingpool, $dialogService, $userService, $conferenceService, uuid2) {
        var self = this;

        this.loading = false;

        $scope.$on('dialog.newtextmessage', function (event, data) {
            $dialogService.addMessage(data);
        });

        this.getDialogs = function() {
            return $dialogService.dialogs;
        };

        //this.videoStarted = function() {
        //    return $dialogService.videostarted;
        //};

        //this.getPlayer = function () {
        //    return $sce.trustAsHtml($dialogService.player.outerHTML);
        //};

        this.hideDialog = function(dialogId, dialogType) {
            var dialog = $dialogService.getDialog(dialogId, dialogType);

            dialog.visible = false;
            dialog.active = false;
        };

        this.getUserImage = function (userId) {
            var user = $userService.getUser(userId);
            return user && user.Image ? user.Image.FileID : null;
        };

        this.getUserName = function(userId) {
            var user = $userService.getUser(userId);
            return user && user.FullName ? user.FullName.trim() : '';
        };

        this.stringToDate = function (stringDate) {
            var rDate = $filter('relativeDate');
            return rDate($dateParser(stringDate, 'dd.MM.yyyy HH:mm:ss'));
        };

        this.currentUser = function () {
            return $userService.currentUser;
        };

        this.startPresentation = function(messageId) {
            $log.info("Play:", messageId);
        }

        this.canPlayPresentation = function(userId) {
            return userId == $userService.currentUser.ID;
        };

        this.hasDialog = function() {
            var dialog = $dialogService.getDialog();
            return dialog != null;
        };
    }

    angular.module("ConferenceApp")
       .controller("DialogsController", dialogsController);


    dialogsController.$inject = ['$scope', '$log', '$interval', '$dateParser', '$element', '$sce', '$filter', '$loadingpool', '$dialogService', '$userService', '$conferenceService', 'uuid2'];

})(window.angular);