//This script handles the dropdownlist change that render the dropdownlist of invoice associated with the client name

"use strict";
$(document).ready(function() {
    $("#GetClients").change(function () {
        $("#invoicenumber").empty();
        $.ajax({
            method: 'GET',
            url: '../api/InvoiceNumber',
            dataType: 'json',
            data: { clientName: $("#GetClients").val() },
            success: function(data) {
                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        $("#invoicenumber").append('<option value="' +
                            data[key].InvoiceNumber +
                          
                            '">' +
                            data[key].InvoiceNumber +
                            '</option>');
                        console.log(data[key].InvoiceNumbe);
                    }
                }
            },
            error: function(ex) {
                console.log("An error occurred in completing your request " + ex);
            }
        });
        return false;
    });

    $("#invoicenumber").change(function () {
        $("#servicenumber").empty();
        $.ajax({
            method: 'GET',
            url: '../api/ServiceEntry',
            dataType: 'json',
            data: { clientName: $("#GetClients").val(), invoicenumber: $("#invoicenumber").val() },
            success: function (data) {
                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        $("#servicenumber").append('<option value="' +
                            data[key].ServiceNumber +

                            '">' +
                            data[key].ServiceNumber +
                            '</option>');
                        console.log(data[key].ServiceNumber);
                    }
                }
            },
            error: function (ex) {
                console.log("An error occurred in completing your request " + ex);
            }
        });
        return false;
    });
});
