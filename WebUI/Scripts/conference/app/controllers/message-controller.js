(function (angular) {
    function messageController($scope, $log, $signalService, $dialogService, fileUploader, $rtcService, $element) {
        var self = this;

        this.message = '';

        this.uploader = new fileUploader({
            url: '/FileData/SaveFiles',
            autoUpload: true,
            onSuccessItem: function (item, response, status, headers) {
                $log.info(item, response, status, headers);

                var file = response ? response[0] : null;

                var active = $dialogService.getDialog();

                if (active && file) {

                    //Костыль с сериализатором DateTime

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

                    //$log.info('New file:', f);

                    $signalService.hub.invoke('SendFileMessage', active.dialogId, f, active.dialogType);
                }
            }
        });

        this.sendMessage = function () {
            if (self.message.trim().length == 0) return;

            var active = $dialogService.getDialog();
            if (active) {
                $signalService.hub.invoke('SendTextMessage', active.dialogId, self.message, active.dialogType);
            }
            self.message = '';
        };

        this.sendVideo = function() {
            $rtcService.startRecording(function (data) {
                //$log.info('On stream:', data);
                $scope.$applyAsync();
            });
        };

        this.videoRequest = function() {
            var active = $dialogService.getDialog();

            if (active) {
                $signalService.hub.invoke('SendVideoRequest', active.dialogId, active.dialogType);
            }
        };

        this.hasActiveDialog = function () {
            var active = $dialogService.getDialog();
            return active != null;
        };
    }

    angular.module("ConferenceApp")
       .controller("MessageController", messageController);


    messageController.$inject = ['$scope', '$log', '$signalService', '$dialogService', 'FileUploader', '$rtcService', '$element'];

})(window.angular);