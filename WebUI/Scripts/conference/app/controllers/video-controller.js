(function (angular) {
    function videoController($scope, $location, $log, $sce, $rtcService, $dialogService, $signalService) {
        var self = this;

        this.videoStarted = function() {
            return $rtcService.started;
        };

        this.getPlayer = function () {
            setTimeout(function () {
                if ($rtcService.player.outerHTML !== undefined && $rtcService.rtc) {
                    var stream = $rtcService.rtc.streams.selectFirst({ local: true });
                    if (stream) {
                        var player = angular.element('#' + stream.streamid);
                        player[0].removeAttribute("controls");
                        player[0].play();
                        //$log.info('Element:', player[0]);
                    }
                }
            }, 300);

            return $sce.trustAsHtml($rtcService.player.outerHTML);
        };


        this.stopRecording = function() {
            $rtcService.stopRecording(function (blob, response) {
                var active = $dialogService.getDialog();

                //$dialogService.stopOfflineVideo();

                var file = response ? response[0] : null;

                if (active && file) {

                    var f = {
                        //ChangeDate: file.ChangeDate,
                        //CreationDate: file.CreationDate,
                        FileID: file.FileID,
                        FileName: file.FileName,
                        Hidden: file.Hidden,
                        ID: file.ID,
                        Key: file.Key,
                        RowVersion: file.RowVersion,
                        Size: file.Size,
                        SortOrder: file.SortOrder
                    };

                    $signalService.hub.invoke('SendFileMessage', active.dialogId, f, active.dialogType);
                }
            });
        };

    }

    angular.module("ConferenceApp")
       .controller("VideoController", videoController);


    videoController.$inject = ['$scope', '$location', '$log', '$sce', '$rtcService', '$dialogService', '$signalService'];

})(window.angular);