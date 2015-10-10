//inject angular file upload directives and services.
//var app = angular.module('fileUplad', ['ngFileUpload']);
//app.controller('MyCtrl', ['$scope', 'Upload', '$timeout'], function ($scope, Upload, $timeout) {
//    $scope.$watch('files', function () {
//        $scope.upload($scope.files);

//    });
//    $scope.$watch('file', function () { if ($scope.file != null) { $scope.files = [$scope.file]; } });
//    $scope.log = '';
//    $scope.upload = function (files)
//    {
//        if (files && files.length)
//        {

//            for (var i = 0; i < files.length; i++)
//            {
//                var file=files[i]
//                if (!file.$error)
//                {
//                    Upload.upload({
//                        url: 'http://wwww.',
//                        data: {
//                            username: $scope.username,
//                            file:file

//                        }

//                    }).progress(function (evt) {

//                        var progress = parseInt(100 * evt.loaded / evt.total);
//                        $scope.log ='Progress : '+ progress + ' %' + config.data.filename + '\n' + $scope.log;
//                    }).success(function (data,status,headers,config) {
//                        $timeout(function () {
//                            $scope.log = 'file upload: ' + config.data.file.name + ' ' + JSON.stringify(data) + $scope.log;

//                        })
//                    })
//                }

//            }
//        }

//    }

//});
var app = angular.module('fileUpload', ['ngFileUpload']);

app.controller('MyCtrl', ['$scope', 'Upload', '$timeout', function ($scope, Upload, $timeout) {
    $scope.$watch('files', function () {
        $scope.upload($scope.files);
    });
    $scope.$watch('file', function () {
        if ($scope.file != null) {
            $scope.files = [$scope.file];
        }
    });
    $scope.log = '';
    $scope.stream_source = "/api/files/mp4/BodyPart_6871b7bc-9e00-48fa-a0a1-d79a8a321fec";
    $scope.upload = function (files) {
        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                if (!file.$error) {
                    Upload.upload({
                        url: 'http://localhost:58879/api/files',
                        data: {
                            username: $scope.username,
                            file: file
                        }
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        $scope.log = 'progress: ' + progressPercentage + '% ' +
                                    evt.config.data.file.name + '\n' + $scope.log;
                    }).success(function (data, status, headers, config) {
                       
                        $timeout(function () {
                            $scope.log = 'file: ' + config.data.file.name + ', Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                            
                        });
                    });
                }
            }
        }
    };
}]);