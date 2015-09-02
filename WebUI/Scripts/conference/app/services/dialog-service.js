(function (angular) {
    function dialogService($log, $http, $dataService, $userService, $conferenceService, uuid2) {

        var service = function () {
            var self = this;

            this.dialogs = [];

            this.unreaded = {};

            //this.videostarted = false;
            //this.player = '';

            //this.startOfflineVideo = function(htmlPlayer) {
            //    self.player = htmlPlayer;
            //    self.videostarted = true;
            //};

            //this.stopOfflineVideo = function () {
            //    self.player = '';
            //    self.videostarted = false;
            //};

            this.getUreadCount = function (dialogId, type) {

                type = type == undefined ? 'PrivateMessage' : type;

                return self.unreaded[dialogId + '_' + type] != undefined ? self.unreaded[dialogId + '_' + type].Count : 0;
            };

            this.getDialog = function(dialogId, type) {
                var dialog = null;

                type = type == undefined ? 'PrivateMessage' : type;

                angular.forEach(self.dialogs, function (d) {
                    if ((dialogId == undefined && d.active) || (d.dialogId == dialogId && d.dialogType == type)) {
                        dialog = d;
                    }
                });

                return dialog;
            };

            this.openDialog = function(settings, callback) {
                if (settings.active == undefined) settings.active = true;
                if (settings.visible == undefined) settings.visible = settings.active;
                if (settings.load == undefined) settings.load = false;

                var dialog = self.getDialog(settings.dialogId, settings.type);

                console.log("This is get dialog:", dialog);


                if (!dialog) {
                    self.addDialog({
                        title: settings.name,
                        messages: [],
                        dialogId: settings.dialogId,
                        dialogType: settings.type,
                        active: settings.active,
                        visible: settings.visible
                    });

                    if (settings.load)
                        self.loadMessages(settings.dialogId, settings.type, function (messages) {
                            if (callback)
                                callback();
                        });

                } else {
                    dialog.visible = settings.visible;
                    dialog.active = settings.active;

                    setTimeout(function () {
                        self.readMessages(dialog.messages, dialog.dialogType);
                    }, 7000);
                }
            };

            this.addDialog = function (dialog) {
                angular.extend(dialog, { version: uuid2.newuuid() });
                self.dialogs.push(dialog);
            };

            this.addMessage = function (data, callback) {
                data.message = JSON.parse(data.message);

                $log.info('Message:', data);

                //Check on dialog type
                var dialogId = data.dialogType == 'PrivateMessage' ? (data.message.FromId == $userService.currentUser.ID ? data.message.ToUserId : data.message.FromId) :
                    data.message.ToConferenceId;

                var dialog = self.getDialog(dialogId, data.dialogType);

                $log.info('Dialog:', dialog);

                if (!dialog) {
                    var dialogTitle = '';

                    if (data.dialogType == 'PrivateMessage') {
                        var user = $userService.getUser(dialogId);
                        dialogTitle = user.FullName;
                    } else {
                        var conf = $conferenceService.getConference(dialogId);
                        dialogTitle = conf.Title;
                    }

                    self.openDialog({
                        dialogId: dialogId,
                        name: dialogTitle,
                        type: data.dialogType,
                        load: true,
                        active: false,
                        visible: false
                    });

                } else {
                    if (data.message.FromId != $userService.currentUser.ID) {
                        if (dialog.active) {
                            self.readMessages([data.message], data.dialogType, false);
                        } else {
                            if (self.unreaded[dialogId + '_' + data.dialogType] == undefined)
                                self.unreaded[dialogId + '_' + data.dialogType] = {
                                    Count: 0,
                                    Type: data.dialogType
                                }
                            self.unreaded[dialogId + '_' + data.dialogType].Count += 1;
                        }
                    }

                    dialog.messages.push(data.message);

                    if (callback)
                        callback();

                    //$scope.$applyAsync();

                    //Костыль
                    if (dialog.active) {
                        setTimeout(function () {
                            //console.log('Scroll dialog:', '#dialog-' + data.dialogType + '-' + dialogId);
                            var $el = $('#dialog-' + data.dialogType + '-' + dialogId);
                            $el.stop().animate({ scrollTop: $el[0].scrollHeight }, "slow");
                        }, 0);
                    }
                }

            };

            this.loadMessages = function (dialogId, dialogType, callback) {
                var url = '';

                switch (dialogType) {
                    case 'PublicMessage':
                        url = $dataService.dialogs.url.getPublicMessages;
                        break;
                    case 'PrivateMessage':
                    default:
                        url = $dataService.dialogs.url.getPrivateMessages;
                        break;
                }

                $http.get(url + '/' + dialogId)
                    .success(function (response) {
                        var dialog = self.getDialog(dialogId, dialogType);
                        var messages = response;

                        console.log('Dialog ID:', dialogId, 'Dialog type:', dialogType);
                        console.log('On load dialog:', dialog, 'On load messages:', messages);

                        if (dialog) {
                            var unreaded = {};

                            angular.forEach(messages, function (message) {
                                var unreadedkey = dialogType == 'PrivateMessage' ? message.FromId + "_PrivateMessage" : message.ToConferenceId + "_PublicMessage";

                                if (message.FromId != $userService.currentUser.ID && message.IsNew) {
                                    if (unreaded[unreadedkey] == undefined)
                                        unreaded[unreadedkey] = {
                                            Count: 0,
                                            Type: dialogType
                                        };
                                    unreaded[unreadedkey].Count += 1;
                                }

                                dialog.messages.push(message);
                            });

                            //dialog.messages = messages;
                            if (dialog.active && dialog.visible) {
                                setTimeout(function () {
                                    self.readMessages(messages, dialogType);
                                }, 7000);
                            } else {
                                angular.forEach(unreaded, function (unread, index) {
                                    self.unreaded[index] = unread;
                                });
                            }

                        }

                        callback(response);
                    });
            };

            this.readMessages = function (messages, type, calc, callback) {
                console.log('Read messages:', type);

                type = type == undefined ? 'PrivateMessage' : type;
                if (calc == undefined) calc = true;

                $log.info('Reading start..');

                var messageIDs = [];

                angular.forEach(messages, function (message) {
                    if (message.FromId != $userService.currentUser.ID && message.IsNew) {
                        message.IsNew = false;

                        var unreadedkey = type == 'PrivateMessage' ? message.FromId + '_PrivateMessage' : message.ToConferenceId + '_PublicMessage';
                        console.log('Read message:', unreadedkey);

                        if (self.unreaded[unreadedkey] != undefined && self.unreaded[unreadedkey].Count > 0 && calc) {
                            self.unreaded[unreadedkey].Count -= 1;
                        }

                        messageIDs.push(message.ID);
                    }
                });

                if (messageIDs.length) {
                    $http.post($dataService.dialogs.url.postReaded, messageIDs)
                       .success(function (response) {
                           $log.info('Reading stop:', response);
                           if (callback)
                                callback(response);
                   });
                } else {
                    $log.info('Reading stop, nothing new...');
                }
            };

            this.loadPrivateUnread = function (callback) {
                $http.get($dataService.dialogs.url.getUnread)
                    .success(function (response) {
                        self.unreaded = response;
                        callback(response);
                    });
            };

        };

        return new service();
    };

    dialogService.$inject = ['$log', '$http', '$dataService', '$userService', '$conferenceService', 'uuid2'];

    angular.module('Conference.services').
        factory('$dialogService', dialogService);

})(window.angular);