

"use strict";
//$(document).ready(function () {
    var myRequest = function (url, callbackF) {
        var xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState === 4 && this.status === 200) {
                var data = JSON.parse(this.responseText);              
                callbackF(data);
            }
        };
        xhttp.open("GET", url, true);
        xhttp.setRequestHeader("Content-type", "application/json; chartset=utf-8");
        xhttp.send();
    };


    function plotChart(objects) {
        var year = [];
        var actual = [];
        var month = [];
        var unpaid =[];
        var paid =[];
              let key;
        for (key in objects) {
            if (objects.hasOwnProperty(key)) {
             
month.push(objects[key].Month);
               unpaid.push(objects[key].Unpaid);
paid.push(objects[key].Paid);               
               
            }          
        }
       console.log(paid);
       console.log(unpaid);
        var ctx = document.getElementById('myChart').getContext('2d');
        var chart = new Chart(ctx, {
            // The type of chart we want to create
            type: 'bar',
            
            // The data for our dataset
             // The data for our dataset
             data: {
                labels: month,
                datasets: [{
                    label: "Unpaid",
                    backgroundColor: ["red"],
                    data: unpaid
                },{
                    label: "Paid",
                    backgroundColor: ["green"],
                    data: paid
                }]
            },

            // Configuration options go here
            options: {
                title: {
                    display: true,
                    text: 'Dollar unpaid vs paid chart'
                }
            }
        });

    }
    function plotLineChart(objects) {
        var year = [];
        var actual = [];
        var month = [];
        var unpaid =[];
        var paid =[];
        // var colorArray = ['Yellow', 'Red'];
        
        let key;
        for (key in objects) {
            if (objects.hasOwnProperty(key)) {
              //  year.push(objects[key].Dtype);
//actual.push(objects[key].Flow);
month.push(objects[key].Month);
               unpaid.push(objects[key].Unpaid);
paid.push(objects[key].Paid);               
               
            }          
        }
//console.log(objects);
        //var ctx = document.getElementById('myChartBar').getContext('2d');
        
        //var chart = new Chart(ctx, {
        //    // The type of chart we want to create
        //    type: 'bar',

        //    // The data for our dataset
        //    data: {
        //        labels: month,
        //        datasets:[{
        //            label: "Unpaid",
        //            backgroundColor: 'rgba(0, 99, 132, 0.6)',
        //            data: unpaid,
        //            fillColor:["red"]
        //        },{
        //            label: "Paid",
        //            backgroundColor: 'rgba(99, 132, 0, 0.6)',
        //            data: paid,
        //            fillColor:["green"]
        //        }]
        //    },

        //    // Configuration options go here
        //    options: {
        //        title: {
        //            display: true,
        //            text: 'Naira unpaid vs paid chart'
        //        }
        //    }
        //});
        var densityCanvas = document.getElementById("myChartBar");

        Chart.defaults.global.defaultFontFamily = "Lato";
        Chart.defaults.global.defaultFontSize = 18;

        var densityData = {
            label: 'Unpaid debts',
            data: unpaid,
            backgroundColor: 'rgba(0, 99, 132, 0.6)',
            borderWidth: 0,
            yAxisID: "y-axis-debts"
        };

        var gravityData = {
            label: 'paid debts',
            data: paid,
            backgroundColor: 'rgba(99, 132, 0, 0.6)',
            borderWidth: 0,
            yAxisID: "y-axis-credits"
        };

        var planetData = {
            labels: month,
            datasets: [densityData, gravityData]
        };

        var chartOptions = {
            scales: {
                //xAxes: [{
                //    barPercentage: 1,
                //    categoryPercentage: 0.6
                //}],
                yAxes: [{
                    id: "y-axis-density"
                }, {
                    id: "y-axis-gravity"
                }]
            }
        };

        var barChart = new Chart(densityCanvas, {
            type: 'bar',
            data: planetData,
            options: chartOptions
        });
    }
    function plotLChart(objects) {
        var year = [];
        var actual = [];
        var month = [];
        var unpaid =[];
        var paid =[];
              let key;
        for (key in objects) {
            if (objects.hasOwnProperty(key)) {
             
month.push(objects[key].Month);
               unpaid.push(objects[key].Unpaid);
paid.push(objects[key].Paid);               
               
            }          
        }
       console.log(paid);
       console.log(unpaid);
       var ctx = document.getElementById('myChartBar').getContext('2d');
       var chartz = new Chart(ctx, {
           // The type of chart we want to create
           type: 'bar',

           // The data for our dataset
           data: {
               labels: month,
               datasets:[{
                   label: "Unpaid",
                   backgroundColor: ["yellow"],
                   data: unpaid                  
               },{
                   label: "Paid",
                   backgroundColor: ["green"],
                   data: paid
               }]
           },

            // Configuration options go here
            options: {
                title: {
                    display: true,
                    text: 'Naira unpaid vs paid chart'
                }
            }
        });
        chartz.update();

    }
var currentTime = new Date();
var year = currentTime.getFullYear();
myRequest("../api/DollarDebt", plotChart);
myRequest("../api/NairaDebt", plotLChart);

function MediaType(objects, chartid, charttitle) {
    var year = [];
    var actual = [];
    var month = [];
    var unpaid = [];
    var paid = [];
    let key;
    for (key in objects) {
        if (objects.hasOwnProperty(key)) {

            month.push(objects[key].Month);
            unpaid.push(objects[key].Unpaid);
            paid.push(objects[key].Paid);

        }
    }
    console.log(paid);
    console.log(unpaid);
    var ctx = document.getElementById(chartid).getContext('2d');
    var chartz = new Chart(ctx, {
        // The type of chart we want to create
        type: 'bar',

        // The data for our dataset
        data: {
            labels: month,
            datasets: [{
                label: "Unpaid",
                backgroundColor: ["yellow"],
                data: unpaid
            }, {
                label: "Paid",
                backgroundColor: ["green"],
                data: paid
            }]
        },

        // Configuration options go here
        options: {
            title: {
                display: true,
                text: charttitle
            }
        }
    });
    chartz.update();
}
//binding the plot to client name changed
$("#YearList").change(function () {
    var chartid = "NairaChartBar";
    var charttitle = "Naira Unpaid vs Paid for " + $("#GetClients").val();
    var chartid2 = "DollarChart";
    var charttitle2 = "Dollar Unpaid vs Paid for " + $("#GetClients").val();
    $.ajax({
        method: 'GET',
        url: '../api/DebtNairaClient',
        dataType: 'json',
        data: { yr: $("#YearList").val(), client: $("#GetClients").val() },
        success: function (data) {
            MediaType(data, chartid, charttitle);
        },
        error: function (ex) {
            console.log("An error occurred in completing your request " + ex);
        }
    });
    $.ajax({
        method: 'GET',
        url: '../api/DebtDollarClient',
        dataType: 'json',
        data: { yr: $("#YearList").val(), client: $("#GetClients").val() },
        success: function (data) {
            MediaType(data, chartid2, charttitle2);
        },
        error: function (ex) {
            console.log("An error occurred in completing your request " + ex);
        }
    });
    return false;
});