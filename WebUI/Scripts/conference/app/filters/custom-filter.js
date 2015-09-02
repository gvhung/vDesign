(function (angular) {

    function notUser() {
        return function (items, id, online) {

            //if (online == undefined) online = false;

            //console.log("Online filter:", online);
            //var users = {};

            //angular.forEach(items, function (user, index) {
            //    if (user.ID != id && user.online != undefined && user.online)
            //        users[index] = user;
            //});

            return items.filter(function (user) {
                return user.ID != id && (online == undefined || user.online == online);
            });

            //return users;
        };

    };

    angular.module('Conference.filters', []).
        filter('notUser', notUser);

})(window.angular);
