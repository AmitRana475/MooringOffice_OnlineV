var app = angular.module('MyApp', ['angularjs-dropdown-multiselect']);
app.controller('multiselecteddropdown', ['$scope', '$http', function ($scope, $http) {
    //define object
    $scope.Skillsets = [];
    $scope.Categories = [];
    $scope.dropdownSetting = {
        scrollable: true,
        scrollableHeight: '200px'
    }

    //fetch data from database
    $http.get('/setting/user/Autocompletedropdown').then(function (data) {
        angular.forEach(data.data, function (value, index) {
            $scope.Categories.push({ id: value.VesselID, label: value.VesselName });
        });
    })

   
    //$scope.changedValue = function () {
    //    $scope.Skillsets.Id;
    //}

}]);



var app2 = angular.module('test', ['angularjs-dropdown-multiselect']);
app2.controller('multiselectedrank', ['$scope', '$http', function ($scope, $http) {
    //define object
    $scope.Ranks = [];
    $scope.Categories1 = [];
    $scope.dropdownSetting = {
        scrollable: true,
        scrollableHeight: '200px'
    }

    //fetch data from database
    $http.get('/analysis/analysis/Autocompleterank').then(function (data) {
        angular.forEach(data.data, function (value, index) {
            $scope.Categories1.push({ id: value.Ranks, label: value.Ranks });

        });
    })

}]);

var app3 = angular.module('MyApp1', ['angularjs-dropdown-multiselect']);
app3.controller('multiselecteddropdown1', ['$scope', '$http', function ($scope, $http) {
    //define object
    $scope.Skillsets = [];
    $scope.Categories = [];
    $scope.dropdownSetting = {
        scrollable: true,
        scrollableHeight: '200px'
    }

    //fetch data from database
    $http.get('/setting/user/Autocompletedropdown').then(function (data) {
        angular.forEach(data.data, function (value, index) {
            $scope.Categories.push({ id: value.VesselName, label: value.VesselName });
        });
    })

}]);

var app4 = angular.module('MyApp4', ['angularjs-dropdown-multiselect']);
app4.controller('multiselecteddropdown4', ['$scope', '$http', function ($scope, $http) {
    angular.element(document).ready(function () {
        $scope.SkillsetsFleetName = [];
        $scope.CategoriesF = [];
        $scope.dropdownSetting = {
            scrollable: true,
            scrollableHeight: '200px'
        }

        //fetch data from database
        $http.get('/home/AutoCompletedfname').then(function (data) {
            angular.forEach(data.data, function (value, index) {
                $scope.CategoriesF.push({ id: value.FleetName, label: value.FleetName });
            });
        })

    })
}]);

var app5 = angular.module('MyApp5', ['angularjs-dropdown-multiselect']);
app5.controller('multiselecteddropdown5', ['$scope', '$http', function ($scope, $http) {
    angular.element(document).ready(function () {
        //define object
        $scope.SkillsetsFleetType = [];
        $scope.CategoriesT = [];
        $scope.dropdownSetting = {
            scrollable: true,
            scrollableHeight: '200px'
        }

        //fetch data from database
        $http.get('/home/AutoCompletedtname').then(function (data) {
            angular.forEach(data.data, function (value, index) {
                $scope.CategoriesT.push({ id: value.FleetType, label: value.FleetType });
            });
        })
    })
}]);

var app6 = angular.module('MyApp6', ['angularjs-dropdown-multiselect']);
app6.controller('multiselecteddropdown6', ['$scope', '$http', function ($scope, $http) {
    //define object
    angular.element(document).ready(function () {
  
        $scope.Skillsets = [];
        $scope.Categories = [];
        $scope.dropdownSetting = {
            scrollable: true,
            scrollableHeight: '200px'
        }

        //fetch data from database
        $http.get('/home/AutoCompletevessel').then(function (data) {
            angular.forEach(data.data, function (value, index) {
                $scope.Categories.push({ id: value.VesselName, label: value.VesselName });
            });
        })
    })
}]);
angular.module("CombineModule", ["test", "MyApp","MyApp1","MyApp4","MyApp5","MyApp6"]);