(function (angular) {
    function conferenceController($scope, $location, $log, $modal, $userService, $conferenceService, $dialogService, $signalService) {
        var self = this;

        this.openDialog = function (dialogId, name, type) {

            var settings = {
                active: true,
                visible: true,
                load: true,
                dialogId: dialogId,
                name: name,
                type: type
            };

            $dialogService.openDialog(settings);


            $location.path('/dialogs');
        };

        this.getConferences = function () {
            return $conferenceService.conferences;
        };

        this.getCurrentUserId = function () {
            return $userService.currentUser ? $userService.currentUser.ID : -1;
        };

        this.getUreadCount = function (conferenceId, type) {
            return $dialogService.getUreadCount(conferenceId, type);
        };

        this.createConference = function() {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'CreateConferenceContent.html',
                controller: 'CreateConferenceController',
                size: 'lg',
                resolve: {
                    model: function () {
                        return { };
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $log.info('Modal succesed:', data);

                $conferenceService.create(data, function (response) {
                    $signalService.hub.invoke('updateConferences', response.ID);
                    $log.info('On before update conference', response);
                });

            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        }

    }

    angular.module("ConferenceApp")
       .controller("ConferenceController", conferenceController);


    conferenceController.$inject = ['$scope', '$location', '$log', '$modal', '$userService', '$conferenceService', '$dialogService', '$signalService'];

})(window.angular);