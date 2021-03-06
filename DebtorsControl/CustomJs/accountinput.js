﻿"use strict";
function Vat(amount) {
    var vat = 0.05 * parseFloat(amount);
    document.getElementById("Vat").value = vat.toFixed(4);
}

function Invoiced(amount, vat) {
    var invoiced = parseFloat(amount) + parseFloat(vat);
    document.getElementById("TotalInvoice").value = invoiced.toFixed(4);
}

function Payable(amount, vat) {
    // var fx = parseFloat(document.getElementById("FxRate").value).toFixed(4);
    var payable = parseFloat(amount) - parseFloat(vat);
    document.getElementById("Payable").value = payable.toFixed(4);
}
function Lcd(amount) {
    var fx = parseFloat($("#FxRate").value).toFixed(4);
    var lcd = 0.01 * parseFloat(amount).toFixed(4);
    document.getElementById("LcdCharge").value = lcd;
}
function Outstanding(payable, actual, lcd) {
    var outstanding = payable - actual - lcd;
    if (isNaN(outstanding)) {
        document.getElementById("Outstanding").value = 0;
    } else {
        if (outstanding < 0) {
            var res = outstanding * -1;
            document.getElementById("Outstanding").value = res.toFixed(4);
            var outstandNaira = res * parseFloat($("#FxRate").val());
            document.getElementById("OutNaira").value = outstandNaira;
        } else {
            document.getElementById("Outstanding").value = outstanding.toFixed(4);
            var outstandNaa = outstanding.toFixed(2) * parseFloat($("#FxRate").val());
            document.getElementById("OutNaira").value = outstandNaa;
        }

    }
    var fx = parseFloat($("#FxRate").val());
    if (isNaN(fx)) {
        document.getElementById("LcdChargeNaira").value = lcd;
    } else {
        var lcdinNaira = fx * lcd;
        document.getElementById("LcdChargeNaira").value = lcdinNaira;
    }
}
function WithHoldingTax(amount) {
    var withHold = 0.05 * parseFloat(amount).toFixed(4);
    document.getElementById("WithHoldingTax").value = withHold.toFixed(4);
}
function NairaEqVal(payable, exrate) {
    var nairaeq = payable * exrate;
    if (isNaN(nairaeq)) {
        document.getElementById("NairaEquiv").value = payable;
    }
    if (isNaN(exrate)) {
        document.getElementById("NairaEquiv").value = payable;
    } else {
        document.getElementById("NairaEquiv").value = nairaeq.toFixed(4);
    }

    // alert(nairaeq);
}
$(document).ready(function () {
    $("#Amount").change(function () {
        //$("Amount").empty();
        let amount = $("#Amount").val();
        Vat(amount);
        let vat = $("#Vat").val();
        Invoiced(amount, vat);
        Payable(amount, vat);
        Lcd(amount);
        WithHoldingTax(amount);

    });

    function doAmount() {
        let payable = parseFloat($("#Payable").val());
        let lcdcharge = parseFloat($("#LcdCharge").val());
        let amountpaid = parseFloat($("#AmountPaid").val());
        Outstanding(payable, amountpaid, lcdcharge);
        // let payable = parseFloat($("#Payable").val());
        let exrate = parseFloat($("#FxRate").val());
        NairaEqVal(payable, exrate);
    }
    //().addEventListener("mouseleave", doAmount);
    var elistener = document.getElementById("AmountPaid");
    if (elistener) {
        document.getElementById("AmountPaid").addEventListener("mouseleave", doAmount);
    }


});