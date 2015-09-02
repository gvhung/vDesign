(function (angular) {
    function rtcService($log, $userService, $signalService) {

        var service = function () {
            var self = this;

            var videoTypes = {
                0: 'capture',
                1: 'conference'
            }

            this.rtc = null;

            this.started = false;
            this.player = '';
            this.videoType = videoTypes[0];

            this.config = {
                channel: null,
                userid: null,
                sessionid: null,
                streamid: null,
                session: {
                    audio: true,
                    video: true,
                    oneway: false
                },
                sessionScreen: {
                    screen: true,
                    oneway: true
                },
                dontTransmit: true,
                extra: {},
                stream: null,
                direction: 'many-to-many',
                iceServers: { iceServers: [] },
                keepStreamsOpened: false,
                started: false,
                onstart: null,
                onstop: null,
                onconnect: null,
                ondisconnect: null,
                onpartofscreen: function (event) {
                }
            };


            this.startRecording = function (callback) {
                self.rtc = new RTCMultiConnection();

                self.rtc.onstream = function (data) {
                    data.mediaElement.className = "demo-video";
                    self.player = data.mediaElement;
                    self.started = true;

                    self.rtc.streams[data.streamid].startRecording({ audio: true, video: true });

                    if (callback) {
                        callback(data);
                    }
                };


                self.rtc.captureUserMedia(function (stream) {
                    $log.info("RTC:", self.rtc);
                }, {
                    audio: true,
                    video: true
                });

            };

            this.stopRecording = function(callback) {
                var data = self.rtc.streams.selectFirst({ local: true });

                if (data) {
                    self.rtc.streams[data.streamid].stopRecording(function (blob) {
                        //wav
                        var audioBlob = new Blob([blob.audio], {
                            type: 'audio/wav'
                        });

                        //webm
                        var videoBlob = new Blob([blob.video], {
                            type: 'video/webm'
                        });

                        var formData = new FormData();

                        formData.append('video', videoBlob, 'temp.webm');
                        formData.append('audio', audioBlob, 'temp.wav');

                        self.xhr('/FileData/SaveFiles', formData, function (response) {
                            if (callback) {
                                callback(audioBlob, response);
                            }
                        });

                        self.stopRTC();
                    });
                };
            };

            this.stopRTC = function() {
                self.rtc.streams.stop();
                self.rtc = null;
                self.player = '';
                self.started = false;
            };

            var init = function (key, user, onstart, onstop, onconnect, ondisconnect) {
                self.config.channel = 'channel_' + key;
                self.config.sessionid = 'session_' + key;
                self.config.userid = user.ID;
                self.config.onstart = onstart;
                self.config.onstop = onstop;
                self.config.onconnect = onconnect;
                self.config.ondisconnect = ondisconnect;

                var onMessageCallbacks = {};

                $signalService.hub.on('onMessageReceived', function (message) {
                    message = JSON.parse(message);

                    if (onMessageCallbacks[message.channel]) {
                        onMessageCallbacks[message.channel](message.message);
                    }
                });


                self.rtc = new RTCMultiConnection(self.config.channel);

                self.rtc.sdpConstraints.mandatory = {
                    OfferToReceiveAudio: true,
                    OfferToReceiveVideo: true
                };

                self.rtc.mediaConstraints.mandatory = {
                    minWidth: 1280,
                    maxWidth: 1280,
                    minHeight: 720,
                    maxHeight: 720,
                    minFrameRate: 30
                };

                self.rtc.openSignalingChannel = function (config) {
                    var channel = config.channel || this.channel;
                    onMessageCallbacks[channel] = config.onmessage;

                    if (config.onopen) setTimeout(config.onopen, 1000);

                    return {
                        send: function (message) {
                            message = JSON.stringify({
                                message: message,
                                channel: channel
                            });

                            $signalService.hub.invoke('WebRtcSend', message);
                        }
                    };
                };



            };


            this.startVideoConference = function (key, onstart, onstop, onconnect, ondisconnect) {
                $log.info('RTC console:', 'start', key);

                init(key, $userService.currentUser, onstart, onstop, onconnect, ondisconnect);


                self.started = true;

                if (onstart)
                    onstart();
            };

            this.joinVideoConference = function (key, onstart, onstop, onconnect, ondisconnect) {
                $log.info('RTC console:', 'join', key);

                init(key, $userService.currentUser, onstart, onstop, onconnect, ondisconnect);

                self.started = true;

                if (onstart)
                    onstart();
            };


            //Перенести в Data
            this.xhr = function(url, data, callback) {
                var request = new XMLHttpRequest();
                request.onreadystatechange = function () {
                    if (request.readyState == 4 && request.status == 200) {
                        callback(JSON.parse(request.responseText));
                    }
                };
                request.open('POST', url);
                request.send(data);
            }


        };

        return new service();
    };

    rtcService.$inject = ['$log', '$userService', '$signalService'];

    angular.module('Conference.services').
        factory('$rtcService', rtcService);

})(window.angular);