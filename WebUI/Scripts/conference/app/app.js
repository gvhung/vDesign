var conference = conference || {};

conference.app = angular.module("ConferenceApp", ['ngMessages', 'ui.router', 'ngRoute', 'ngCookies', 'perfect_scrollbar', 'dateParser', 'relativeDate', 'ui.bootstrap', 'Conference.services', 'Conference.filters', 'angularUUID2', 'angularFileUpload', 'ngFileLink'])
    .config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.otherwise("/home");

        $stateProvider
            .state('home', {
                url: "/home",
                templateUrl: "home.html"
            }).state('dialogs', {
                url: "/dialogs",
                templateUrl: "dialogs.html",
                controller: 'DialogsController',
                controllerAs: 'dial'
            }).state('contacts', {
                url: "/contacts",
                templateUrl: "contacts.html"
            });

        //$routeProvider.
        //   when('/dialogs', {
        //       templateUrl: 'dialogs.html',
        //   }).
        //    when('/contacts', {
        //       templateUrl: 'contacts.html',
        //       //controller: 'ContactsController',
        //       //controllerAs: 'contacts2'
        //   });


    }]).
  run(['$log', '$rootScope', '$location', '$routeTableService', function ($log, $rootScope, $location, $routeTableService) {
      //$rootScope.$on("$locationChangeStart", function (event, next, current) {
      //    //$log.info('Route:', $location, event, next, current);
      //    //$routeTableService.setActive($location.$$url);
      //});
      //$rootScope.$on('$routeChangeSuccess', function (event, next, current) {
      //    $log.info('Route change:', event, next, current);
      //});
  }]);



