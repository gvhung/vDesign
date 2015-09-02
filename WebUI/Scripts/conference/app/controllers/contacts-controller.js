(function (angular) {
    function contactsController($scope, $location, $log, $userService, $dialogService) {
        var self = this;

        this.openDialog = function (dialogId, name, type) {
            $dialogService.openDialog({
                dialogId: dialogId,
                name: name,
                type: type,
                active: true,
                visible: true,
                load: true,
            });
            $location.path('/dialogs');
        };

        this.getUsers = function() {
            return $userService.users;
        };

        this.getCurrentUserId = function() {
            return $userService.currentUser ? $userService.currentUser.ID : -1;
        };

        this.getUreadCount = function(userId, type) {
            return $dialogService.getUreadCount(userId, type);
        };

        this.getImage = function(user) {
            return user.Image != null ? user.Image.FileID : null;
        };

    }

    angular.module("ConferenceApp")
       .controller("ContactsController", contactsController);


    contactsController.$inject = ['$scope', '$location', '$log', '$userService', '$dialogService'];

})(window.angular);