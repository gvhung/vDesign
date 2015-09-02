(function (angular) {
    function createConferenceController($scope, $modalInstance, $userService, $conferenceService, model) {
        var self = this;

        $scope.model = model;
        $scope.choosenusers = [];
        $scope.title = '';

        console.log('Modal:', model);
        console.log('Modal users:', $userService.users);

        $scope.ok = function () {
            if ($scope.title.trim() == '' || !$scope.choosenusers.length) return false;

            var members = [];

            angular.forEach($scope.choosenusers, function(user) {
                members.push({
                    Object: user,
                    ObjectID: user.ID
                });
            });

            $modalInstance.close({
                Title: $scope.title,
                Members: members
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.getUsers = function() {
            return $userService.users.filter(function (user) {
                //if (!$scope.choosenusers.length) return true;

                for (var i in $scope.choosenusers) {
                    if ($scope.choosenusers[i].ID == user.ID || user.ID == $userService.currentUser.ID) return false;
                }

                return user.ID != $userService.currentUser.ID;
            });
        };

        $scope.getChoosen = function() {
            return $scope.choosenusers;
        };

        $scope.checkUser = function(user) {
            $scope.choosenusers.push(user);
        };

        $scope.uncheckUser = function (user) {
            var userIdx = -1;

            angular.forEach($scope.choosenusers, function(u, index) {
                if (user.ID == u.ID)
                    userIdx = index;
            });

            $scope.choosenusers.splice(userIdx, 1);

            //console.log(userIdx);
        };

    }


    angular.module("ConferenceApp")
       .controller("CreateConferenceController", createConferenceController);


    //createConferenceController.$inject = ['$scope', '$modalInstance'];

})(window.angular);