
// this is to chain items to the datatype dropdown list

$("#DoRemittance").change(function () {
    $("#InvoiceNumber").empty();
    $.ajax({
        method: 'GET',
        url: '../api/InvoiceNumber',
        dataType: 'json',
        data: { clientName: $("#DoRemittance").val() },
        success: function (data) {
            for (let key in data) {
                if (data.hasOwnProperty(key)) {
                    $("#InvoiceNumber").append('<option value="' + data[key].InvoiceNumber + '">' + data[key].InvoiceNumber + '</option>');
                }
            }
        },
        error: function (ex) {
            console.log("An error occurred in completing your request " + ex);
        }
    });
    return false;
});
$("#InvoiceTypes").change(function() {
    if ($("#InvoiceTypes").val() === "Naira") {
        //   $("#fxrate").hidden = false;
        document.getElementById("fxrate").setAttribute('type', 'text');
    } else {
        document.getElementById("fxrate").setAttribute('type', 'hidden');
    }
});
if ($("#InvoiceTypes").val() === "Naira") {
    //   $("#fxrate").hidden = false;
    document.getElementById("fxrate").setAttribute('type', 'text');
} else {
    document.getElementById("fxrate").setAttribute('type', 'hidden');
}
$("#InvoiceNumber").change(function () {
    $("#ServiceNumber").empty();
    $.ajax({
        method: 'GET',
        url: '../api/ServiceEntries',
        dataType: 'json',
        data: { clientName: $("#DoRemittance").val(), invoicenum: $("#InvoiceNumber").val() },
        success: function (data) {
            for (let key in data) {
                if (data.hasOwnProperty(key)) {
                    $("#ServiceNumber").append('<option value="' + data[key].SeNumber + '">' + data[key].SeNumber + '</option>');
                }
            }
        },
        error: function (ex) {
            console.log("An error occurred in completing your request " + ex);
        }
    });
    return false;
});