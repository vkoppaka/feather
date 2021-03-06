﻿(function () {
    //angular controller responsible for the Advanced mode logic
    var advancedDesignerModule = angular.module('advancedDesignerModule', ['breadCrumbModule', 'controlPropertyServices']);

    //basic controller for the advanced designer view
    advancedDesignerModule.controller('AdvancedDesignerModuleCtrl', ['$scope','PropertyDataService',
            function ($scope, PropertyDataService) {

                var onGetPropertiesSuccess = function (data) {
                    if (data.Items)
                        $scope.Items = data.Items;
                    $scope.ShowLoadingIndicator = false;
                };

                var onGetError = function (data, status, headers, config) {
                    $scope.ShowError = true;
                    if (data)
                        $scope.ErrorMessage = data.Detail;

                    $scope.ShowLoadingIndicator = false;
                }

                $scope.$on('saveButtonPressed', function (event, e) {
                    PropertyDataService.setProperties($scope.Items);
                });

                $scope.$on("$destroy", function () {
                    PropertyDataService.setProperties($scope.Items);
                });

                PropertyDataService.getProperties(onGetPropertiesSuccess, onGetError);

                $scope.ShowLoadingIndicator = true;

                $scope.ShowSimpleButton();

                $scope.DrillDownPropertyHierarchy = function (propertyPath, propertyName) {
                    $scope.propertyPath = propertyPath;
                    $scope.propertyName = propertyName;
                };

                if (typeof ($telerik) != "undefined") {
                    $telerik.$(document).one("controlPropertiesLoaded", function (e, params) {
                        if (params.Items) {
                            $scope.Items = params.Items;
                            $scope.$apply();
                        }
                    });
                }

            }]);

    //filters property hierarchy
    advancedDesignerModule.filter('propertyHierarchy', function () {

        return function (inputArray, propertyName, propertyPath) {
            var currentLevel;
            if (propertyPath)
                currentLevel = propertyPath.split('/').length - 1;

            var levelFilter = function (property) {
                var level = property.PropertyPath.split('/').length - 2;
                if (propertyName == null || propertyName.length == 0 || propertyName == 'Home') {
                    if (level > 0) {
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    if (property.PropertyPath.indexOf(propertyPath) >= 0) {
                        return currentLevel == level;
                    } else {
                        return false;
                    }
                }
            };

            var proxyFilter = function (property) {
                return !property.IsProxy;
            }

            var result = inputArray;
            if (inputArray && inputArray[0].IsProxy)
                result = inputArray.filter(proxyFilter);
            else if (inputArray)
                result = inputArray.filter(levelFilter);

            return result;
        };

    });

})();