(function (angular) {

    angular.module('Conference.services').
        factory('$signalService', signalService);

    signalService.$inject = ['$rootScope'];

    function signalService($rootScope) {
        var self = this;

        var stateConversion = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };
        var usersconnections = [];

        var connection = $.hubConnection();

        var hub = connection.createHubProxy('conferenceHub');

        var confirmation = function () {
            hub.invoke('confirmConnection');
        };

        return {
            connection: connection,
            hub: hub,
            confirmation: confirmation,
            current: {
                getState: function() {
                    return stateConversion[connection.state];
                },
                setConnections: function(connections) {
                    usersconnections = connections;
                },
                getConnections: function () {
                    return usersconnections;
                },
            }
        }
    };


})(window.angular);