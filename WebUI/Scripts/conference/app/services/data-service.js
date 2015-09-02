(function (angular) {
    function dataService() {

        var service = function () {

            this.user = {
                url: {
                    get: '/Conference/GetUsers'
                }
            };

            this.conference = {
                url: {
                    get: '/Conference/GetConferences',
                    create: '/Conference/CreateConference'
                }
            };

            this.dialogs = {
                url: {
                    getPrivateMessages: '/Conference/GetMessages',
                    getPublicMessages: '/Conference/GetConferenceMessages',
                    getUnread: '/Conference/UnreadMessages',
                    postReaded: '/Conference/ReadMessages'
                }
            };

        };

        return new service();
    };

    angular.module('Conference.services').
        factory('$dataService', dataService);

})(window.angular);