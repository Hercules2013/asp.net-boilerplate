﻿//define(['jquery', 'abp/abp'], function ($, abp) {

//    var getTasks = function () {
//        return abp.ajax({
//            url: '/api/services/Task/GetMyTasks',
//            type: 'GET'
//        });
//    };

//    var createTask = function(task) {
//        return abp.ajax({
//            url: '/api/services/Task/Create',
//            type: 'POST',
//            data: JSON.stringify(task)
//        });
//    };

//    var deleteTask = function (taskId) {
//        return abp.ajax({
//            url: '/api/services/Task/Delete',
//            type: 'GET',
//            data: {
//                taskId: taskId
//            }
//        });
//    };
    
//    return {
//        getTasks: getTasks,
//        createTask: createTask,
//        deleteTask: deleteTask
//    };
//});
define(['jquery', 'abp/abp'], function ($, abp) {

    var getMyTasks = function() {
        return abp.ajax({
            url: '/api/services/task/getMyTasks',
            type: 'POST'
        });
    };

    var create = function(task) {
        return abp.ajax({
            url: '/api/services/task/create',
            type: 'POST',
            data: JSON.stringify(task)
        });
    };

    var deleteTask = function(taskId) {
        return abp.ajax({
            url: '/api/services/task/delete',
            type: 'GET',
            data: {
                 taskId: taskId
            }
        });
    };

    return {
        getMyTasks: getMyTasks,
        create: create,
        deleteTask: deleteTask
    };

});