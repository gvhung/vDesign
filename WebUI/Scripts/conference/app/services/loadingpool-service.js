(function (angular) {
    function loadingService($log) {

        var service = function () {
            var self = this;
            
            this.pool = [];

            this.on = function(name) {
                var item = this.get(name);
                if (item)
                    item.value = true;
                else
                    this.pool.push({
                        name: name,
                        value: true
                    });

                $log.info('pool:', this.pool);
                return this.isComplete();
            };

            this.off = function(name) {
                var item = this.get(name);

                if (item)
                    item.value = false;
                else
                    this.pool.push({
                        name: name,
                        value: false
                    });

                return this.isComplete();
            };

            this.switch = function(name) {
                var item = this.get(name);

                if (item)
                    item.value = !item.value;
                else
                    this.pool.push({
                        name: name,
                        value: true
                    });

                return this.isComplete();
            };

            this.get = function(name) {
                for (var i in this.pool) {
                    if (this.pool[i].name === name) {
                        return this.pool[i];
                    }
                };

                return false;
            };

            this.isComplete = function() {
                var complete = true;

                for (var i in this.pool) {
                    if (this.pool[i].value) {
                        complete = false;
                        break;
                    }
                };

                $log.info('complete:', complete);
                return complete;
            };

        };

        return new service();
    };

    loadingService.$inject = ['$log'];

    angular.module('Conference.services').
        factory('$loadingpool', loadingService);

})(window.angular);