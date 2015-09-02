(function (angular) {
    function fileLink($rootScope, $log) {

        var getFileType = function(filename) {
            var extension = /[^.]+$/.exec(filename);
            return (extension && extension.length) ? extension[0] : '';
        };

        var getClassByType = function(extension) {
            switch (extension) {
                case 'rtf':
                    return 'file-icon filetype filetype-rtf';
                case 'txt':
                    return 'file-icon filetype filetype-txt';
                case 'doc':
                    return 'file-icon filetype filetype-doc';
                case 'log':
                    return 'file-icon filetype filetype-log';
                case 'tex':
                    return 'file-icon filetype filetype-tex';
                case 'msg':
                    return 'file-icon filetype filetype-msg';
                case 'text':
                    return 'file-icon filetype filetype-text';
                case 'wpd':
                    return 'file-icon filetype filetype-wpd';
                case 'wps':
                    return 'file-icon filetype filetype-wps';
                case 'docx':
                    return 'file-icon filetype filetype-docx';
                case 'page':
                    return 'file-icon filetype filetype-page';
                case 'csv':
                    return 'file-icon filetype filetype-csv';
                case 'dat':
                    return 'file-icon filetype filetype-dat';
                case 'tar':
                    return 'file-icon filetype filetype-tar';
                case 'xml':
                    return 'file-icon filetype filetype-xml';
                case 'vcf':
                    return 'file-icon filetype filetype-vcf';
                case 'pps':
                    return 'file-icon filetype filetype-pps';
                case 'key':
                    return 'file-icon filetype filetype-key';
                case 'ppt':
                    return 'file-icon filetype filetype-ppt';
                case 'pptx':
                    return 'file-icon filetype filetype-pptx';
                case 'sdf':
                    return 'file-icon filetype filetype-sdf';
                case 'gbr':
                    return 'file-icon filetype filetype-gbr';
                case 'ged':
                    return 'file-icon filetype filetype-ged';
                case 'mp3':
                    return 'file-icon filetype filetype-mp3';
                case 'm4a':
                    return 'file-icon filetype filetype-m4a';
                case 'waw':
                    return 'file-icon filetype filetype-waw';
                case 'wma':
                    return 'file-icon filetype filetype-wma';
                case 'mpa':
                    return 'file-icon filetype filetype-mpa';
                case 'iff':
                    return 'file-icon filetype filetype-iff';
                case 'aif':
                    return 'file-icon filetype filetype-aif';
                case 'ra':
                    return 'file-icon filetype filetype-ra';
                case 'mid':
                    return 'file-icon filetype filetype-mid';
                case 'm3v':
                    return 'file-icon filetype filetype-m3v';
                case '3gp':
                    return 'file-icon filetype filetype-e-3gp';
                case 'shf':
                    return 'file-icon filetype filetype-shf';
                case 'avi':
                    return 'file-icon filetype filetype-avi';
                case 'asx':
                    return 'file-icon filetype filetype-asx';
                case 'mp4':
                    return 'file-icon filetype filetype-mp4';
                case 'mpg':
                    return 'file-icon filetype filetype-mpg';
                case 'asf':
                    return 'file-icon filetype filetype-asf';
                case 'vob':
                    return 'file-icon filetype filetype-vob';
                case 'wmv':
                    return 'file-icon filetype filetype-wmv';
                case 'mov':
                    return 'file-icon filetype filetype-mov';
                case 'srt':
                    return 'file-icon filetype filetype-srt';
                case 'flv':
                    return 'file-icon filetype filetype-flv';
                case 'png':
                    return 'file-icon filetype filetype-png';
                case 'psd':
                    return 'file-icon filetype filetype-psd';
                case 'jpg':
                    return 'file-icon filetype filetype-jpg';
                case 'tif':
                    return 'file-icon filetype filetype-tif';
                case 'tiff':
                    return 'file-icon filetype filetype-tiff';
                case 'gif':
                    return 'file-icon filetype filetype-gif';
                case 'bmp':
                    return 'file-icon filetype filetype-bmp';
                case 'ai':
                    return 'file-icon filetype filetype-ai';
                case 'svg':
                    return 'file-icon filetype filetype-svg';
                case 'pdf':
                    return 'file-icon filetype filetype-pdf';
                case 'xlr':
                    return 'file-icon filetype filetype-xlr';
                case 'xls':
                    return 'file-icon filetype filetype-xls';
                case 'xlsx':
                    return 'file-icon filetype filetype-xlsx';
                case 'db':
                    return 'file-icon filetype filetype-db';
                case 'dbf':
                    return 'file-icon filetype filetype-dbf';
                case 'mdb':
                    return 'file-icon filetype filetype-mdb';
                case 'pdb':
                    return 'file-icon filetype filetype-pdb';
                case 'sql':
                    return 'file-icon filetype filetype-sql';
                case 'exe':
                    return 'file-icon filetype filetype-exe';
                case 'com':
                    return 'file-icon filetype filetype-com';
                case 'bat':
                    return 'file-icon filetype filetype-bat';
                case 'jar':
                    return 'file-icon filetype filetype-jar';
                case 'css':
                    return 'file-icon filetype filetype-css';
                case 'js':
                    return 'file-icon filetype filetype-js';
                case 'php':
                    return 'file-icon filetype filetype-php';
                case 'xhtml':
                    return 'file-icon filetype filetype-xhtml';
                case 'htm':
                    return 'file-icon filetype filetype-htm';
                case 'html':
                    return 'file-icon filetype filetype-html';
                case 'aspx':
                    return 'file-icon filetype filetype-aspx';
                case 'rss':
                    return 'file-icon filetype filetype-rss';
                case 'zip':
                    return 'file-icon filetype filetype-zip';
                case 'zipx':
                    return 'file-icon filetype filetype-zipx';
                case 'rar':
                    return 'file-icon filetype filetype-rar';
                case '7z':
                    return 'file-icon filetype filetype-e-7z';
                case 'iso':
                    return 'file-icon filetype filetype-iso';
                case 'ini':
                    return 'file-icon filetype filetype-ini';
            }
            return 'file-icon filetype filetype-txt';
        };

        return {
            restrict: 'E',
            require: 'ngModel',
            scope: {
                model: '=ngModel',
                onplay: '=ngPlayCallback',
                showplay: '=ngPresentationIf'
            },
            template: '<button ng-if="presentationfile && showplay" class="msg-presentation-play" ng-click="onplay(model.FileID)"><span class="glyphicon glyphicon-play"></span></button><a class="msg-file-link" ng-href="/Files/GetFile?fileid={{model.FileID}}"><i ng-class="class"></i><p class="msg-file-name">{{model.FileName}}</p><p class="msg-file-date">{{model.CreationDate | date:"dd.MM.yyyy HH:mm:ss"}}</p></a>',
            link: function ($scope, $element, $attrs) {
                $scope.extension = getFileType($scope.model.FileName);
                $scope.presentationfile = $scope.extension == 'pptx';
                $scope.class = getClassByType($scope.extension);
            }
        };
    }

    angular.module("ngFileLink", [])
        .directive('ngFileLink', fileLink);

    fileLink.$inject = ['$rootScope', '$log'];

})(window.angular);