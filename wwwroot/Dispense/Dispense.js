$(document).ready(function () {
    GetStoreName();
    GetClientGstDetails();
    $("#tblBillHold tbody").on('click', 'td', function () {
        var indexx = $(this).index();
        if (indexx != 11) {
            SelectHoldBillRow($(this));
        }
    });

    //$('#tblBillHold tbody tr').find('td:gt(0):lt(5)').filter(function () {
    //    alert('vdfdff');
    //}).on('click', function () {
    //    alert("hello");
    //});

    //$('#table tr').find('td:gt(0):lt(5)').on("click", function () {
    //    if ($(this).closest('tr').find('td:eq(5)').text() != "true") {
    //        alert("hello");
    //    }
    //});
});
function PendingAmt() {
    $('#modal-Pay').modal('show');
}
function GetCurretntStockByDrugCode(DrugCode, StoreName, gstType) {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    $.ajax({
        url: "/Pharma/Dispense/GetCurrentStockByDrugCode",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            StoreName: StoreName,
            GstType: gstType
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblDrugSales");
                var tbodyRowCount = table.tBodies[0].rows.length;
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var DISPENSE_EXPIRYDT = response[PCHeader].DispenseExpiry;
                        //Date checked in Store Procedure -Habib 07/07/21
                        //if (PH_CUR_STOCK_EXPIRYDT != "") {
                        //    var getpurchase = GetDispnesePurchaseExpiry(DISPENSE_EXPIRYDT);
                        //    if (getpurchase == false) {
                        //        alert("Cannot Dispense Expired Drug");
                        //        $('#txtDrugSearch').val('');
                        //        return false;
                        //    }
                        //}
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;

                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        var ExpiryDt = response[PCHeader].ExpiryDt;
                        var Sno = tbodyRowCount + 1;
                        var $tr = $('#tblDrugSales tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                        if ($tr.length === 0) {
                            html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                            html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                            html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                            html += "<td><input value='' style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/> " + PH_CUR_STOCK + "</td>";
                            html += "<td  class='AmountCell'></td>";
                            html += "<td class='TaxCell'></td>";
                            html += "<td class='TotalAmountCell'></td>";
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";
                            html += "<td style='text-align: center;'>";
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                            html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                            html += "<td style='display:none;'>" + ExpiryDt + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                            html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";

                            $("#tblDrugSales tbody").append(html);
                            $("#tblDrugSales").find("input[type='text']").last().focus();
                            $("#txtDrugSearch").val('');
                            html = "";
                        }
                    }
                }
                else {
                    $("#tblSelectBatch tbody").empty();
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;

                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        // var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        //var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        // var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        html += "<tr  onclick='SelectedBatchChange(this)' data-id=\"" + PH_CUR_DRUGCODE + "\">";
                        html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                        html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                        html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                        html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                        html += "<td>" + PH_CUR_STOCK + "</td>";
                        html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td></tr>";
                        $("#tblSelectBatch tbody").append(html);
                        html = "";
                    }
                    $("#ModalAdd").dialog(
                        {
                            title: "Select Batch",
                            width: 806,
                            height: 300,
                            modal: true,
                            buttons: {
                                "Cancel": function () {
                                    $("#ModalAdd").dialog("close");
                                }
                            }
                        });
                }
            }
            else {
                alert("Cannot Dispense Expired Drug");
                $('#txtDrugSearch').val('');
                return false;
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetCurrentStockByDrugCodeAndBatch(DrugCode, StoreName, Btach, Qty, GstType) {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    $.ajax({
        url: "/Pharma/Dispense/GetCurrentStockByDrugCodeAndBatch",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            StoreName: StoreName,
            Batch: Btach,
            GstType: GstType
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var gst = $('#lblGst').html();
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblDrugSales");
                var tbodyRowCount = table.tBodies[0].rows.length;
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;
                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        var ExpiryDt = response[PCHeader].ExpiryDt;
                        var DISPENSE_EXPIRYDT = response[PCHeader].DispenseExpiry;
                        if (gst == "Registered") {
                            var amount = parseFloat(Qty * PH_CUR_STOCK_BILLINGPRICE).toFixed(2);
                            var Tax = parseFloat(PH_ITEM_DRUG_VAT).toFixed(2);
                            var vatamt = parseFloat((amount * Tax) / 100).toFixed(2);
                            var total = parseFloat(amount) + parseFloat(vatamt);
                            total = total.toFixed(2);
                        }

                        else {
                            var amount = parseFloat(Qty * PH_CUR_STOCK_BILLINGPRICE).toFixed(2);
                            var Tax = "";
                            var vatamt = 0;
                            var total = parseFloat(amount);
                            total = total.toFixed(2);
                        }
                        //Date checked in Store Procedure -Habib 07/07/21
                        //if (PH_CUR_STOCK_EXPIRYDT != "") {
                        //    var getpurchase = GetDispnesePurchaseExpiry(DISPENSE_EXPIRYDT);
                        //    if (getpurchase == false) {
                        //        alert("Cannot Dispense Expired Drug");
                        //        $('#txtDrugSearch').val('');
                        //        return false;
                        //    }
                        //}

                        var Sno = tbodyRowCount + 1;
                        var $tr = $('#tblDrugSales tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                        if ($tr.length === 0) {
                            html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                            html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                            html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                            html += "<td><input style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' value='" + Qty + "' onkeypress='return isNumberKey(event)'> " + PH_CUR_STOCK + "</td></td>";
                            html += "<td  class='AmountCell'>" + amount + "</td>";
                            html += "<td class='TaxCell'>" + vatamt + "</td>";
                            html += "<td class='TotalAmountCell'>" + total + "</td>";
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";
                            html += "<td style='text-align: center;'>";
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                            html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                            html += "<td style='display:none;'>" + ExpiryDt + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                            html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";
                            $("#tblDrugSales tbody").append(html);
                            $("#tblDrugSales").find("input[type='text']").last().focus();
                            $("#txtDrugSearch").val('');
                            html = "";

                        }
                        else {
                            var $tr = $('#tblDrugSales tr[data-Batch="' + PH_CUR_STOCK_BATCHNO + '"]');
                            if ($tr.length === 0) {
                                html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                                html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                                html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                                html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                                html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                                html += "<td><input style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' value='" + Qty + "' onkeypress='return isNumberKey(event)'> " + PH_CUR_STOCK + "</td></td>";
                                html += "<td  class='AmountCell'>" + amount + "</td>";
                                html += "<td class='TaxCell'>" + vatamt + "</td>";
                                html += "<td class='TotalAmountCell'>" + total + "</td>";
                                html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";
                                html += "<td style='text-align: center;'>";
                                html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                                html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                                html += "<td style='display:none;'>" + ExpiryDt + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                                html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";
                                $("#tblDrugSales tbody").append(html);
                                $("#tblDrugSales").find("input[type='text']").last().focus();
                                $("#txtDrugSearch").val('');
                                html = "";
                            }
                            else {
                                alert("Already Drug and Batch Added");
                            }
                        }
                        TotalCalculation();
                        TotalDiscountCalculation();
                        //$("#txtDrugSearch").focus();
                    }
                }
            }
            else {
                alert("Cannot Dispense Expired Drug");
                $('#txtDrugSearch').val('');
                return false;
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function SelectedBatchChange(selectedrow) {
    var myrow = selectedrow.rowIndex;
    var DrugCode = selectedrow.cells[0].innerHTML;
    var Batch = selectedrow.cells[2].innerHTML;
    var StoreName = $('#ddlStoreName :selected').text();
    var gstType = $('#lblGst').html();
    GetCurrentStockBatchByDrugCodeAndBatch(DrugCode, StoreName, Batch, 0, gstType);
    //  $('#modal-default').modal().hide();
    $("#ModalAdd").dialog("close");
}
function CalculateTableRowAmount() {
    try {
        var tblDrugSales = document.getElementById("tblDrugSales");
        //$("#tblDrugSales");
    }
    catch (e) { console.log(e); }
}
function SearchDrug() {
    var StoreName = $('#ddlStoreName :selected').text();

    $("#txtDrugSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Pharma/Dispense/GetDrugSearchByFreeText",
                type: "GET",
                data: {
                    SearchTearm: request.term,
                    StoreName: StoreName
                },
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.PH_CUR_DRUGBRANDNAME,
                            value: el.PH_CUR_DRUGCODE
                        };
                    }));
                }
            });
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#txtDrugSearch").val(ui.item.label);
        },
        select: function (event, ui) {
            // $("#txtDrugSearch").prop('disabled', true);
            $("#txtDrugSearch").val(ui.item.label);
            var DrugCode = parseInt(ui.item.value);
            var gstType = $('#lblGst').html();
            /*GetDispnesePurchaseExpiry(StoreName);*/
            GetCurretntStockByDrugCode(DrugCode, StoreName, gstType);
            return false;
        },
        minLength: 0
    });
}
function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 32 && (charCode < 48 || charCode > 57) || (charCode == 32))

        return false;

    return true;
}
function QtyValu(SelectedRow) {
    // var DrugName = $("#txtSearchDrugType").val();
    var row = SelectedRow.parentNode.parentNode;
    var rowIndex = SelectedRow.rowIndex;
    var Qty = parseInt(row.cells[6].getElementsByTagName("input")[0].value);
    var CurrentQty = parseInt(row.cells[12].innerHTML);
    var gstTypes = $('#lblGst').html();
    if (Qty <= CurrentQty) {
        if (gstTypes == "Registered") {

            var sno = row.cells[0].innerHTML;
            var Itemname = row.cells[1].innerHTML;
            var Batchno = row.cells[3].innerHTML;
            var ExpiryDate = row.cells[4].innerHTML;
            var Cost = parseFloat(row.cells[5].innerHTML).toFixed(2);
            var Tax = parseFloat(row.cells[10].innerHTML).toFixed(2);
            var amt = parseFloat(Cost * Qty).toFixed(2);
            var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
            var total = parseFloat(amt) + parseFloat(vatamt);
            total = total.toFixed(2);
            row.cells[7].innerHTML = amt;
            row.cells[8].innerHTML = vatamt;
            row.cells[9].innerHTML = total;
            TotalCalculation();
            TotalDiscountCalculation();
            $("#txtDrugSearch").focus();
        }
        else if (gstTypes == "UnRegistered") {

            var sno = row.cells[0].innerHTML;
            var Itemname = row.cells[1].innerHTML;
            var Batchno = row.cells[3].innerHTML;
            var ExpiryDate = row.cells[4].innerHTML;
            var Cost = parseFloat(row.cells[5].innerHTML).toFixed(2);
            /* var Tax = parseFloat(row.cells[10].innerHTML).toFixed(2);*/
            var amt = parseFloat(Cost * Qty).toFixed(2);
            /*var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);*/
            var total = parseFloat(amt);
            total = total.toFixed(2);
            row.cells[7].innerHTML = amt;
            row.cells[8].innerHTML = 0;
            row.cells[9].innerHTML = total;
            TotalCalculation();
            TotalDiscountCalculation();
            $("#txtDrugSearch").focus();
        }


    }
    else {

        alert("Stock Limit Exceeded");
        row.cells[6].getElementsByTagName("input")[0].value = 0;
        row.cells[7].innerHTML = "0.00";
        row.cells[8].innerHTML = "0.00";
        row.cells[9].innerHTML = "0.00";
        TotalCalculation();
        TotalDiscountCalculation();
        row.cells[6].getElementsByTagName("input")[0].focus();
    }
}
function TotalCalculation() {
    var AmountCells = document.getElementsByClassName("AmountCell"); //returns a list with all the elements that have class 'priceCell'
    var Amount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < AmountCells.length; i++) {
        if (AmountCells[i].innerHTML != "") {
            var thisPrice = parseFloat(AmountCells[i].innerHTML); //get inner text of this cell in number format
            Amount = Amount + thisPrice;
        }
    };
    Amount = Amount.toFixed(2);
    $("#txtTotalamt").val(Amount);

    var TaxCells = document.getElementsByClassName("TaxCell"); //returns a list with all the elements that have class 'priceCell'
    var Tax = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < TaxCells.length; i++) {
        if (TaxCells[i].innerHTML != "") {
            var thisPrice = parseFloat(TaxCells[i].innerHTML); //get inner text of this cell in number format
            Tax = Tax + thisPrice;
        }
    };
    Tax = Tax.toFixed(2);
    $("#txtTotalVatmat").val(Tax);

    var TotalAmountCells = document.getElementsByClassName("TotalAmountCell"); //returns a list with all the elements that have class 'priceCell'
    var TotalAmount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < TotalAmountCells.length; i++) {
        if (TotalAmountCells[i].innerHTML != "") {
            var thisPrice = parseFloat(TotalAmountCells[i].innerHTML); //get inner text of this cell in number format
            TotalAmount = TotalAmount + thisPrice;
        }
    };
    TotalAmount = TotalAmount.toFixed(2);
    $("#txtGrandTotal").val(TotalAmount);
}
function DeleteOrdersRow(selectedrow) {
    var myrow = selectedrow.parentNode.parentNode;
    var rowIndex = myrow.rowIndex;
    document.getElementById("tblDrugSales").deleteRow(myrow.rowIndex);
    var table = document.getElementById("tblDrugSales");
    var rowCount = table.rows.length;
    var i = myrow.rowIndex;
    regroup(i, rowCount, "tblDrugSales");
    $("#txtRateValue").val('');
    $("#txtDiscountAmt").val('');
    $("#txtCurrentDue").val('');
    $("#txtCashAmt").val('');
    $("#txtBankAmt").val();
    $("#txtBankRefNum").val('');
    $("#txtChequeAmt").val('');
    $("#txtChequeNumber").val('');
    TotalCalculation();
    TotalDiscountCalculation();
}
function regroup(i, rc, ti) {
    for (j = (i + 1); j < rc; j++) {
        if (j > 0) {
            document.getElementById(ti).rows[j].cells[0].innerHTML = j;
        }
    }
}
function percentage(percent, total) {
    return ((percent / 100) * total).toFixed(2)
}
function TotalDiscountCalculation() {
    var CurrentDue = $("#txtCurrentDue").val();
    if (CurrentDue == "" || CurrentDue == '' || CurrentDue == null)
        CurrentDue = 0;
    else
        CurrentDue = parseFloat(CurrentDue);
    var CalculationType = $("#ddlDiscountType").val();
    var Amount = $("#txtTotalamt").val();
    var PrecentageValue = $("#txtRateValue").val();
    var Tax = parseFloat($("#txtTotalVatmat").val());
    if (CalculationType === "PER") {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= 100) {
                    var PrecResult = percentage(PrecentageValue, Amount);
                    $("#txtDiscountAmt").val(PrecResult);
                    var NetAmount = (Amount - PrecResult) + CurrentDue + Tax;
                    NetAmount = Math.round(NetAmount);
                    var Roundoff = (Amount - PrecResult) + CurrentDue + Tax;
                    Roundoff = NetAmount - Roundoff;
                    $("#txtRoundoff").val(Roundoff.toFixed(2));
                    $("#txtNetTotal").val(NetAmount.toFixed(2));
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtTotalamt").val();
            var DueTot = $("#txtCurrentDue").val();
            if (SubTot.length > 0 && DueTot.length > 0) {
                var NetTotal = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                NetTotal = Math.round(NetTotal);
                var Roundoff = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                Roundoff = NetTotal - Roundoff;
                $("#txtRoundoff").val(Roundoff.toFixed(2));
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal.toFixed(2));
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                if (DueTot == "" || DueTot == null || DueTot == '')
                    DueTot = 0;
                var NetTotal = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                NetTotal = Math.round(NetTotal);
                var Roundoff = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                Roundoff = NetTotal - Roundoff;
                $("#txtRoundoff").val(Roundoff.toFixed(2));
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal.toFixed(2));
            }
        }
    }
    else {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= parseFloat(Amount)) {
                    var PrecResult = (Amount - PrecentageValue) + CurrentDue + Tax;
                    PrecResult = Math.round(PrecResult);
                    var Roundoff = (Amount - PrecResult) + CurrentDue + Tax;
                    Roundoff = PrecResult - Roundoff;
                    $("#txtRoundoff").val(Roundoff.toFixed(2));
                    $("#txtDiscountAmt").val(PrecentageValue);
                    $("#txtNetTotal").val(PrecResult.toFixed(2));
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtTotalamt").val();
            var DueTot = $("#txtCurrentDue").val();
            if (SubTot.length > 0 && DueTot.length > 0) {
                var NetTotal = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                NetTotal = Math.round(NetTotal);
                var Roundoff = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                Roundoff = NetTotal - Roundoff;
                $("#txtRoundoff").val(Roundoff.toFixed(2));
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal.toFixed(2));
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                if (DueTot == "" || DueTot == null || DueTot == '')
                    DueTot = 0;
                var NetTotal = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                NetTotal = Math.round(NetTotal);
                var Roundoff = parseFloat(SubTot) + parseFloat(DueTot) + Tax;
                Roundoff = NetTotal - Roundoff;
                $("#txtRoundoff").val(Roundoff.toFixed(2));
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal.toFixed(2));
            }
        }
    }
}
function TotalCalculationChange(ControlValue) {
    var Currentpendingamount = 0;
    var RECIEVEDAMT = 0;
    var OP_Current_Refund = 0;
    var OP_Old_Refund = parseFloat($("#lblRefundAmount").text());
    var OP_Old_Due = parseFloat($("#lblCurrentAmount").text());

    var PENDINGTOPAY = 0;
    var Currentpendingamount = 0;


    var OP_Current_Refund = 0;

    var CashReceivedAmt = 0;
    var DebitCardAmt = 0;
    var CreditCardAmt = 0;
    var ThroughBankAmt = 0;
    var ChequeAmt = 0;

    if ($("#txtCashAmt").val().length > 0)
        CashReceivedAmt = $("#txtCashAmt").val();
    if ($("#txtDebitCardAmt").val().length > 0)
        DebitCardAmt = $("#txtDebitCardAmt").val();
    if ($("#txtCreditCardAmt").val().length > 0)
        CreditCardAmt = $("#txtCreditCardAmt").val();
    if ($("#txtBankAmt").val().length > 0)
        ThroughBankAmt = $("#txtBankAmt").val();
    if ($("#txtChequeAmt").val().length > 0)
        ChequeAmt = $("#txtChequeAmt").val();

    var Ord_nettotal = $("#txtNetTotal").val();
    var RECIEVEDAMT = parseFloat(CashReceivedAmt) + parseFloat(DebitCardAmt) + parseFloat(ThroughBankAmt) + parseFloat(ChequeAmt) + parseFloat(CreditCardAmt);
    if (RECIEVEDAMT > Ord_nettotal) {
        //OP_Current_Refund = RECIEVEDAMT - Ord_nettotal;
        ////OP_Current_Refund = OP_Current_Refund.toFixed(2);
        //OP_Current_Refund = parseFloat(OP_Current_Refund);
        //if (OP_Old_Refund != 0) {
        //    OP_Old_Refund = OP_Old_Refund + OP_Current_Refund;
        //}
        //else
        //    OP_Old_Refund = OP_Current_Refund;
        //OP_Old_Refund = OP_Old_Refund.toFixed(2);
        //$("#txtCurrentRefund").val(OP_Old_Refund);
        //$("#txtCurrentDue").val(0.00);
        //$("#lblCurrentAmount").text(0);
        //OP_Current_Refund = ''+OP_Current_Refund;
        //$("#lblRefundAmount").text(OP_Old_Refund);
        var id = $(ControlValue).attr("id");
        document.getElementById(id).value = 0.00;
        alert('Recieved Amount  shall not be more than Net Total Payable');
    }
    else {
        Currentpendingamount = Ord_nettotal - RECIEVEDAMT;
        //Currentpendingamount = Currentpendingamount.toFixed(2);
        Currentpendingamount = parseFloat(Currentpendingamount).toFixed(2);
        //if (OP_Old_Due != 0)
        //    OP_Old_Due = OP_Old_Due + Currentpendingamount;
        //else
        //    OP_Old_Due = Currentpendingamount;
        //OP_Old_Due = OP_Old_Due.toFixed(2);
        $("#txtCurrentDue").val(Currentpendingamount);
        //  $("#txtCurrentRefund").val(0.00);
        $("#lblRefundAmount").text(0);
        OP_Old_Due = '' + OP_Old_Due;
        // $("#lblCurrentAmount").text(OP_Old_Due);
    }
}
function isNumberAndDecimal(el, evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    var number = el.value.split('.');
    if (charCode != 46 && charCode > 32 && (charCode < 48 || charCode > 57) || (charCode == 32))

        return false;

    if (number.length > 1 && charCode == 46) {
        return false;
    }

    return true;
}
// Patinet Search
function SearchPatient() {
    $("#txtSearchPatID").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Pharma/Dispense/GetPatientIDByFreeSearch",
                type: "GET",
                data: {
                    FreeSearch: request.term
                },
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el
                        };
                    }));
                }
            });
        },
        focus: function (event, ui) {
            event.preventDefault();
            $("#txtSearchPatID").val(ui.item.label);
        },
        select: function (event, ui) {
            // $("#txtDrugSearch").prop('disabled', true);
            $("#txtSearchPatID").val(ui.item.label);
            GetPatDeatilsByPatID();
            var PatinetID = $("#txtSearchPatID").val();
            GetTreatmentHeaderByPatientID(PatinetID);
            $("#txtDrugSearch").focus();
            return false;
        },
        minLength: 0
    });
}
function GetPatDeatilsByPatID() {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    var PatinetID = $("#txtSearchPatID").val();
    $.ajax({
        url: "/Pharma/Dispense/GetPatDeatilsByPatID",
        type: "GET",
        data: {
            PatientID: PatinetID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var Lis_PatFirstName = response[PCHeader].Lis_PatFirstName;
                        var Lis_Sex = response[PCHeader].Lis_Sex;
                        var Lis_AgeDays = response[PCHeader].Lis_AgeDays;
                        var Lis_AgeMonth = response[PCHeader].Lis_AgeMonth;
                        var Lis_AgeYear = response[PCHeader].Lis_AgeYear;
                        var HIS_RELATION = response[PCHeader].HIS_RELATION;
                        var HIS_RELATIONNAME = response[PCHeader].HIS_RELATIONNAME;
                        var HIS_PHONE = response[PCHeader].HIS_PHONE;
                        var LisAddress = response[PCHeader].LisAddress;
                        var LIS_RefDRNAME = response[PCHeader].LIS_RefDRNAME;
                        var LIS_Pattype = response[PCHeader].LIS_Pattype;
                        var HIS_CompanyName = response[PCHeader].HIS_CompanyName;
                        var HIS_StaffID = response[PCHeader].HIS_StaffID;
                        var HIS_StaffDepartment = response[PCHeader].HIS_StaffDepartment;
                        var HIS_IsPatientResponsible = response[PCHeader].HIS_IsPatientResponsible;
                        var MobileNo = 0;
                        if (LIS_RefDRNAME === "")
                            LIS_RefDRNAME = "-";
                        if (HIS_PHONE === "") {
                            HIS_PHONE = "-";
                        }
                        else {
                            HIS_PHONE = HIS_PHONE;
                        }
                        if (HIS_RELATIONNAME === "")
                            HIS_RELATIONNAME = "-";
                        var LIS_Pattype_text = "";
                            $("#ddlPaymentMode").empty();


                        if (LIS_Pattype == "0") {
                            LIS_Pattype_text = "OP";
                            $("#tblDispCashPayment").prop('hidden', false);
                            $("#ddlPaymentMode").append($("<option></option>").val('Self Payment').html('Self Payment'));
                            //if (HIS_CompanyName != "") {
                            //    $("#ddlPaymentMode").append($("<option></option>").val('Company Payment').html('Company Payment'));
                            //}

                        }
                        else if (LIS_Pattype == "1") {
                            LIS_Pattype_text = "IP";
                            $("#tblDispCashPayment").prop('hidden', true);

                            if (HIS_CompanyName != "") {
                                $("#ddlPaymentMode").append($("<option></option>").val('Company Payment').html('Company Payment'));
                            }

                            $("#ddlPaymentMode").append($("<option></option>").val('IP Payment').html('IP Payment'));
                            $("#ddlPaymentMode").append($("<option></option>").val('Self Payment').html('Self Payment'));
                        }


                        //if (LIS_Pattype === "0") {
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "OP PHRAMACY";
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].value = "OPPHARMACY";
                        //}
                        //else if (LIS_Pattype === "1") {
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "IP PHRAMACY";
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "IPPHARMACY";

                        //}
                        //else {
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "OP PHRAMACY";
                        //    document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].value = "OPPHARMACY";
                        //}
                        $("#txtPatName").val(Lis_PatFirstName);
                        $("#txtSex").val(Lis_Sex);
                        $("#txtYear").val(Lis_AgeDays);
                        $("#txtMonth").val(Lis_AgeMonth);
                        $("#txtDay").val(Lis_AgeYear);
                        $("#txtReleation").val(HIS_RELATION + ' ' + HIS_RELATION);
                        $("#txtPhoneNumber").val(HIS_PHONE);
                        $("#txtRefDoctor").val(LIS_RefDRNAME);

                        $("#lblPatientID").text(PatinetID);
                        $("#lblPatinetName").text(Lis_PatFirstName);
                        $("#lblGender").text(Lis_Sex + "/" + Lis_AgeYear + "Y " + Lis_AgeMonth + "M " + Lis_AgeDays+"D");
                        $("#lblMobileNo").text(HIS_PHONE);
                        $("#lblRelation").text(HIS_RELATIONNAME);
                        $("#lblDoctor").text(LIS_RefDRNAME);

                        $("#lblPatType").text(LIS_Pattype_text);
                        $("#lblPatCompany").text(HIS_CompanyName);
                        $("#lblStaffCode").text(HIS_StaffID);
                        $("#lblStaffDept").text(HIS_StaffDepartment);
                        $("#lblIsResponse").text(HIS_IsPatientResponsible);
                        $("#hidSex").val(Lis_Sex);
                        $("#hidAge").val(Lis_AgeDays);
                    }
                }
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
var loading = false;
function BillSave() {
    if (loading) {
        return false;
    }
    loading = true;
    if (validate()) {
        var DrugInfo = new Array();
        var tblDrugSales = document.getElementById("tblDrugSales");
        var rowtblDrugSales = tblDrugSales.rows.length;
        var ErrorMsg = "";
        DrugInfo.length = 0;
        for (M = 1; M < rowtblDrugSales; M++) {
            var rowDrug = tblDrugSales.rows[M];
            var ObjectDetails = new Object();
            ObjectDetails.PH_CUR_DRUGBRANDNAME = rowDrug.cells[2].innerHTML;
            ObjectDetails.PH_CUR_STOCK_EXPIRYDT = rowDrug.cells[4].innerHTML;
            ObjectDetails.PH_CUR_STOCK_BILLINGPRICE = parseFloat(rowDrug.cells[5].innerHTML);
            ObjectDetails.PH_CUR_STOCK_BATCHNO = rowDrug.cells[3].innerHTML;
            var Qty = rowDrug.cells[6].getElementsByTagName("input")[0].value;
            if (Qty !== "") {
                Qty = parseInt(rowDrug.cells[6].getElementsByTagName("input")[0].value);
                if (Qty > 0)
                    ObjectDetails.Qty = parseInt(rowDrug.cells[6].getElementsByTagName("input")[0].value);
                else
                    ErrorMsg += "\n - Please Enter Qty for:" + ObjectDetails.PH_CUR_DRUGBRANDNAME + "";
            }
            else {
                ErrorMsg += "\n - Please Enter Qty for:" + ObjectDetails.PH_CUR_DRUGBRANDNAME + "";
            }
            ObjectDetails.PH_CUR_STOCK = parseInt(rowDrug.cells[12].innerHTML);
            ObjectDetails.PH_CUR_DRUGCODE = parseInt(rowDrug.cells[1].innerHTML);
            ObjectDetails.PH_ITEM_DRUG_VAT = parseFloat(rowDrug.cells[10].innerHTML);
            ObjectDetails.PH_CUR_STOCK_PURCHCOST = parseFloat(rowDrug.cells[13].innerHTML);
            ObjectDetails.Amount = parseFloat(rowDrug.cells[7].innerHTML);
            ObjectDetails.Tax = parseFloat(rowDrug.cells[8].innerHTML);
            ObjectDetails.TotalAmount = parseFloat(rowDrug.cells[9].innerHTML);
            ObjectDetails.ExpiryDt = rowDrug.cells[14].innerHTML;
            DrugInfo.push(ObjectDetails);
        }


        var Amount = $("#txtTotalamt").val();
        if (Amount == "" || Amount == null || Amount == '')
            Amount = 0;
        else
            Amount = parseFloat(Amount);

        var Tax = $("#txtTotalVatmat").val();
        if (Tax == "" || Tax == null || Tax == '')
            Tax = 0;
        else
            Tax = parseFloat(Tax);

        var TotalAmount = $("#txtGrandTotal").val();
        if (TotalAmount == "" || TotalAmount == null || TotalAmount == '')
            TotalAmount = 0;
        else
            TotalAmount = parseFloat(TotalAmount);

        var DiscountType = $("#ddlDiscountType").val();

        var DiscountRate = $("#txtDiscountAmt").val();
        if (DiscountRate == "" || DiscountRate == null || DiscountRate == '')
            TotalAmount = 0;
        else
            DiscountRate = parseFloat(DiscountRate);

        var DueCollected = $("#txtCurrentDue").val();
        if (DueCollected == "" || DueCollected == null || DueCollected == '')
            DueCollected = 0;
        else
            DueCollected = parseFloat(DueCollected);

        var NetCollected = $("#txtNetTotal").val();
        if (NetCollected == "" || NetCollected == null || NetCollected == '')
            NetCollected = 0;
        else
            NetCollected = parseFloat(NetCollected);

        var Roundoff = $("#txtRoundoff").val();
        if (Roundoff == "" || Roundoff == null || Roundoff == '')
            Roundoff = 0;
        else
            Roundoff = parseFloat(Roundoff);

        var Consession = $("#txtDiscountAmt").val();
        if (Consession == "" || Consession == null || Consession == '')
            Consession = 0;
        else
            Consession = parseFloat(Consession);
        var MobileNo = $("#txtPhoneNumber").val();
        if (MobileNo !== "") {
            if (isNumeric(MobileNo)) {
                MobileNo = parseFloat($("#txtPhoneNumber").val());
            }
            else {
                MobileNo = 0;
            }
        }
        else
            MobileNo = 0;
        var CashReceivedAmt = 0;
        var DebitCardAmt = 0;
        var CreditCardAmt = 0;
        var ThroughBankAmt = 0;
        var ChequeAmt = 0;

        if ($("#txtCashAmt").val().length > 0)
            CashReceivedAmt = parseFloat($("#txtCashAmt").val());
        if ($("#txtDebitCardAmt").val().length > 0)
            DebitCardAmt = parseFloat($("#txtDebitCardAmt").val());
        if ($("#txtCreditCardAmt").val().length > 0)
            CreditCardAmt = parseFloat($("#txtCreditCardAmt").val());
        if ($("#txtBankAmt").val().length > 0)
            ThroughBankAmt = parseFloat($("#txtBankAmt").val());
        if ($("#txtChequeAmt").val().length > 0)
            ChequeAmt = parseFloat($("#txtChequeAmt").val());

        var CreditCardNumber = $("#txtCreditCardNum").val();
        var DebitCardNumber = $("#txtDebitCardNumber").val();
        var ChequeNo = $("#txtChequeNumber").val();
        var BankRefNo = $("#txtBankRefNum").val();
        var HoldBillId = $("#hidHoldBillId").val();
        if (HoldBillId != null && HoldBillId != "") {
            HoldBillId = parseInt(HoldBillId);
        }
        else {
            HoldBillId = 0;
        }

        var IsPatientResponse = $("#lblIsResponse").text();
        var boolIsResponse = false;
        if (IsPatientResponse == "true") {
            boolIsResponse = true;
        }
        var patType = $('#txtPatType').val();

        if (patType === "") {

            var patTypes = "OTC";
        }
        else {
            patTypes = $("#lblPatType").text();
        }


        if (ErrorMsg === "") {
            var sendJsonData = {
                PatinetID: $("#lblPatientID").text(),
                Lis_PatFirstName: $("#lblPatinetName").text(),
                Lis_Sex: $("#hidSex").val(),
                Lis_AgeDays: $("#txtDay").val(),
                Lis_AgeMonth: $("#txtMonth").val(),
                Lis_AgeYear: $("#hidAge").val(),
                HIS_RELATION: $("#txtReleation").val(),
                HIS_PHONE: $("#txtPhoneNumber").val(),
                LisAddress: '',
                LIS_RefDRNAME: $("#txtRefDoctor").val(),
                DrugInfos: DrugInfo,
                Amount: Amount,
                Tax: Tax,
                TotalAmount: TotalAmount,
                Roundoff: Roundoff,
                Consession: Consession,
                DiscountType: DiscountType,
                DiscountRate: DiscountRate,
                PendingtoPay: DueCollected,
                NetTotlaAmount: NetCollected,
                CashReceivedAmt: CashReceivedAmt,
                CreditCardAmt: CreditCardAmt,
                DebitCardAmt: DebitCardAmt,
                ThroughBankAmt: ThroughBankAmt,
                ChequeAmt: ChequeAmt,
                CreditCardNumber: CreditCardNumber,
                DebitCardNumber: DebitCardNumber,
                ChequeNo: ChequeNo,
                BankRefNo: BankRefNo,
                StoreName: $('#ddlStoreName :selected').text(),
                Mobile: MobileNo,
                HoldBillId: HoldBillId,
                PaymentMode: $("#ddlPaymentMode option:selected").text(),
                PH_CSH_Company: $("#lblPatCompany").text(),
                PH_CSH_StaffCode: $("#lblStaffCode").text(),
                PH_CSH_Department: $("#lblStaffDept").text(),
                PH_CSH_IsResponse: boolIsResponse,
                PatType: patTypes,
                Lis_Pattype: $("#lblPatType").text()
            };
            $.ajax({
                url: "/Pharma/Dispense/BillSave",
                type: 'post',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(sendJsonData),
                dataType: "json",
                beforeSend: function () { $("#loading").css("display", "block"); },
                success: function (data) {
                    loading = false;
                    ClearAll();
                    BillPrint(data);
                },
                complete: function () { $("#loading").css("display", "none"); },
                error: function () {
                    loading = false;
                }
            });
        }
        else {
            loading = false;
            alert(ErrorMsg);
        }
    }
    loading = false;
    return false;
}
function BillPrint(response) {
    if (response.PrintHeader.length > 0) {
        var html = "";
        for (PCHeader = 0; PCHeader < response.PrintHeader.length; PCHeader++) {
            var PH_CSH_BILLNO = response.PrintHeader[PCHeader].PH_CSH_BILLNO;
            var PH_CSH_BILLDT = response.PrintHeader[PCHeader].PH_CSH_BILLDT;
            var PH_CSH_PATNAME = response.PrintHeader[PCHeader].PH_CSH_PATNAME;
            var PH_CSH_Relation = response.PrintHeader[PCHeader].PH_CSH_Relation;
            var PH_CSH_PATSEX = response.PrintHeader[PCHeader].PH_CSH_PATSEX;
            var PH_CSH_TOTAMOUNT = response.PrintHeader[PCHeader].PH_CSH_TOTAMOUNT;
            var PH_CSH_TAXAMOUNT = parseFloat(response.PrintHeader[PCHeader].PH_CSH_TAXAMOUNT);
            var TotalTaxDived = parseFloat(PH_CSH_TAXAMOUNT).toFixed(2) / 2;
            TotalTaxDived = TotalTaxDived.toFixed(2);
            var PH_CSH_CASHRECIVEDAMT = response.PrintHeader[PCHeader].PH_CSH_CASHRECIVEDAMT.toFixed(2);
            var PH_CSH_NETTAMOUNT = response.PrintHeader[PCHeader].PH_CSH_NETTAMOUNT.toFixed(2);
            var PH_CSH_ROUNDOFF = response.PrintHeader[PCHeader].PH_CSH_ROUNDOFF.toFixed(2);
            var Warehouse = document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text;
            var PH_CSH_PATTYPE = response.PrintHeader[PCHeader].PH_CSH_PATTYPE;
            var PH_CSH_PENDINGTOPAY = response.PrintHeader[PCHeader].PH_CSH_PENDINGTOPAY;
            var PatinetAge = response.PrintHeader[PCHeader].PatinetAge;
            var PH_CSH_PHONE = response.PrintHeader[PCHeader].PH_CSH_PHONE;
            if (response.ClientDetails.length > 0) {
                //Client Deatils
                var ClientName = response.ClientDetails[PCHeader].ClientName;
                var Address1 = response.ClientDetails[PCHeader].Address1;
                var Address2 = response.ClientDetails[PCHeader].Address2;
                var Email = response.ClientDetails[PCHeader].Email;
                var PhomeNumber = response.ClientDetails[PCHeader].PhomeNumber;
                var GstNo = response.ClientDetails[PCHeader].Gst;
                var ARNNo = response.ClientDetails[PCHeader].ARNNo;
                var CityName = response.ClientDetails[PCHeader].CityName;
                var StateName = response.ClientDetails[PCHeader].StateName;
                var CountryName = response.ClientDetails[PCHeader].CountryName;
                var gstTypes = $('#lblGst').html();

                html += "<hr>";
                html += "<div style='text-align: Center;'>" + ClientName + "</div><br/>";
                html += "<div style='text-align: Center;'>" + Address1 + "</div><br/>";
                html += "<div style='text-align: Center;'>" + Address2 + "</div><br/>";
                if (gstTypes == "Registered") {
                    html += "<div style='margin-right:10px;float:right;'>GST No:" + GstNo + "</div><br/>";
                }
                else {
                    html += "<div style='margin-right:10px;float:right;'></div><br/>";
                }

                html += "<hr>";
            }
            html = "";
            //Header
            html += "<table style='font-family:sans-serif;font-size: 12px;font-weight: 500;'>";
            html += "<tr><td style='width:196px !important;'>Invoice number</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_BILLNO + "</td>";
            html += "<td style='width:196px !important;'>Date</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_BILLDT + "</td></tr>";
            html += "<tr><td>Patient name</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_PATNAME + "</td>";
            html += "<td>Relation:</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_Relation + "</td></tr>";
            html += "<tr><td>Sex/Age</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_PATSEX + " / "+PatinetAge+"</td>";
            html += "<td>Patient Type</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_PATTYPE + "</td></tr>";
            html += "<tr><td>Phone Number</td><td>:</td><td style='width:341px !important;'>" + PH_CSH_PHONE + "</td><td>Outlet</td><td>:</td><td style='width:341px !important;'>" + Warehouse + "</td></tr>";
            html += "</table>";
            //Deatils
            html += "<table border='1' style='font-family:sans-serif;font-size: 12px;font-weight: 500;border-collapse:collapse;width: 100%;'><tr>";
            html += "<td style='width: 53px !important;'>S.no</td>";
            html += "<td style='width: 109px !important;'>Hsn Code</td>";
            html += "<td style='width: 275px !important;'>Particular</td>";
            html += "<td style='width: 140px !important;'>Batch No</td>";
            html += "<td style='width: 53px !important;'>ExpiryDt</td>";
            html += " <td align='right' style='width: 53px !important;'>Qty</td>";
            html += "<td align='right' style='width: 53px !important;'>Rate</td>";
            if (gstTypes == "Registered") {
                html += "<td align='right' style='width: 63px !important;'>Amount</td>";
                html += "<td align='right' style='width: 63px !important;'>GST</td>";
                html += "<td align='right'>SGST</td><td align='right'>CGST</td>";
            }
            html += "<td align='right' style='width: 63px !important;'>Total</td></tr>";
            var Sno = 0;
            if (response.PrintDeatils.length > 0) {
                for (PCDetails = 0; PCDetails < response.PrintDeatils.length; PCDetails++) {
                    Sno = Sno + 1;
                    var PH_CUR_DRUGBRANDNAME = response.PrintDeatils[PCDetails].PH_CUR_DRUGBRANDNAME;
                    var PH_CSHDTL_DRUGBATCHNO = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUGBATCHNO;
                    var PH_CSHDTL_DRUGEXPIRYDT = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUGEXPIRYDT;
                    var PH_CSHDTL_DRUG_QTY = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUG_QTY;
                    var PH_CSHDTL_DRUG_AMTEACH = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUG_AMTEACH;
                    var PH_CSHDTL_DRUG_ROWTOTALAMT = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUG_ROWTOTALAMT;
                    var PHCSHDTL_DRUG_TAXVALUE = response.PrintDeatils[PCDetails].PHCSHDTL_DRUG_TAXVALUE;
                    var PH_CSHDTL_DRUG_NETTAMT = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUG_NETTAMT;
                    var Uom = response.PrintDeatils[PCDetails].Uom;
                    if (gstTypes == "Registered") {
                        var PH_CSHDTL_DRUG_TAXPERCENT = response.PrintDeatils[PCDetails].PH_CSHDTL_DRUG_TAXPERCENT;
                    }
                    else {
                        var PH_CSHDTL_DRUG_TAXPERCENT = "";
                    }

                    var PH_ITEM_HSNCODE = response.PrintDeatils[PCDetails].PH_ITEM_HSNCODE;
                    var TaxDivied = parseFloat(PHCSHDTL_DRUG_TAXVALUE).toFixed(2) / 2;
                    TaxDivied = TaxDivied.toFixed(2);
                    html += "<tr><td style='width: 53px !important;'>" + Sno + "</td>";
                    html += "<td style='width: 109px !important;'>" + PH_ITEM_HSNCODE + "</td>";
                    html += "<td style='width: 275px !important;'>" + PH_CUR_DRUGBRANDNAME + "</td>";
                    html += "<td style='width: 140px !important;'>" + PH_CSHDTL_DRUGBATCHNO + "</td>";
                    html += "<td style='width: 53px !important;'>" + PH_CSHDTL_DRUGEXPIRYDT + "</td>";
                    html += "<td align='right' style='width: 53px !important;'>" + PH_CSHDTL_DRUG_QTY + "</td>";
                    html += "<td align='right' style='width: 53px !important;'>" + PH_CSHDTL_DRUG_AMTEACH + "</td>";
                    html += "<td align='right' style='width: 63px !important;'>" + PH_CSHDTL_DRUG_ROWTOTALAMT + "</td>";
                    if (gstTypes == "Registered") {
                        html += "<td align='right' style='width: 63px !important;'>" + PH_CSHDTL_DRUG_TAXPERCENT + "</td>";
                        html += "<td align='right' style='width: 63px !important;'>" + TaxDivied + "</td>";
                        html += "<td align='right' style='width: 63px !important;'>" + TaxDivied + "</td>";
                        html += "<td align='right' style='width: 63px !important;'>" + PH_CSHDTL_DRUG_NETTAMT + "</td>";
                    }
                    html += "</tr>";
                }
                html += "<tr><td colspan='12' style='height:8px;'></td></tr>";
                if (gstTypes == "Registered") {
                    html += "<tr style='font-size: 13px;font-weight: 700;'><td colspan='7'>Total</td><td align='right' style='width: 63px !important;'>" + PH_CSH_TOTAMOUNT.toFixed(2) + "</td>";
                }
                else {
                    html += "<tr style='font-size: 13px;font-weight: 700;'><td colspan='6'>Total</td><td align='right' style='width: 63px !important;'>" + PH_CSH_TOTAMOUNT.toFixed(2) + "</td>";
                }
                if (gstTypes == "Registered") {
                    html += "<td align='right' style='width: 63px !important;'></td>";
                    html += "<td align='right' style='width: 63px !important;'>" + TotalTaxDived + "</td>";
                    html += "<td align='right' style='width: 63px !important;'>" + TotalTaxDived + "</td>";
                }
                html += "<td align='right' style='width: 63px !important;'>" + PH_CSH_NETTAMOUNT + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            //Amount Details
            html += "<table style='font-family:sans-serif;font-weight: 700;font-size: 16px;'><tr>";
            html += "<td style='width: 53px !important;'></td><td style='width: 109px !important;'></td><td style='width: 275px !important;'></td><td style='width: 100px !important;'></td>";
            html += "<td style='width: 53px !important;'></td><td style='width: 53px !important;'></td><td style='width: 63px !important;'></td>";
            html += "<td style='width: 53px !important;'></td><td>Round off:</td><td align='right' style='width: 53px !important;'>" + PH_CSH_ROUNDOFF + "</td></tr>";
            html += "<tr style='font-size:16px !important;'><td colspan='8'></td><td style='width: 188px !important;'>Net Total:</td><td align='right' style='width: 53px !important;'>" + PH_CSH_NETTAMOUNT + "</td></tr>";
            html += "<tr style='font-size:16px !important;'><td colspan='8'></td><td style='width: 188px !important;'>Paid:</td><td align='right' style='width: 53px !important;'>" + PH_CSH_CASHRECIVEDAMT + "</td></tr>";
            html += "<tr style='font-size:16px !important;'><td colspan='8'></td><td style='width: 188px !important;'>Due:</td><td align='right' style='width: 53px !important;'>" + PH_CSH_PENDINGTOPAY.toFixed(2) + "</td></tr>";
           
            html += "<tr></tr><tr></tr><br/><br/><tr><td colspan='6'></td><td colspan='3'>PHARMACIST SIGNATURE</td></tr></table>";
        }
        sessionStorage.setItem("PrintDetails", html);
        openRequestedPopup();
    }
}
function openRequestedPopup() {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    var url = rootUrl + '/Pharmacy/Dispense/print';
    window.open(url, '_blank');
    sessionStorage.setItem("PrintDetails", "");
}
function OpenReprint() {
    GetLast100BillHeader();
    $("#ModelReprint").dialog(
        {
            title: "Re-Print",
            width: 806,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelReprint").dialog("close");
                }
            }
        });
}
function GetLast100BillHeader() {
    var StoreName = $("#ddlStoreName option:selected").text();
    $.ajax({
        url: "/Pharma/Dispense/GetLast100BillHeader",
        type: "GET",
        dataType: "json",
        data: { StoreName: StoreName },
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblBillReprint tbody").empty();
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                    var BillDt = response[PCHeader].BillDt;
                    var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                    var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                    var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                    var PH_CSH_PATID = response[PCHeader].PH_CSH_PATID;
                    html += "<tr onclick='SelectedReprint(this)'><td>" + Sno + "</td>";
                    html += "<td>" + PH_CSH_BILLNO + "</td>";
                    html += "<td>" + PH_CSH_PATID + "</td>";
                    html += "<td>" + PH_CSH_PATNAME + "</td>";
                    html += "<td>" + BillDt + "</td>";
                    html += "<td>" + PH_CSH_PATSEX + "</td>";
                    html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                }
                $("#tblBillReprint tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function BindLast100BillHeader(response) {
    $("#tblBillReprint tbody").empty();
    if (response.length > 0) {
        var html = "";
        var Sno = 0;
        for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
            Sno = PCHeader + 1;
            var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
            var BillDt = response[PCHeader].BillDt;
            var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
            var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
            var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
            var PH_CSH_PATID = response[PCHeader].PH_CSH_PATID;
            html += "<tr onclick='SelectedReprint(this)'><td>" + Sno + "</td>";
            html += "<td>" + PH_CSH_BILLNO + "</td>";
            html += "<td>" + PH_CSH_PATID + "</td>";
            html += "<td>" + PH_CSH_PATNAME + "</td>";
            html += "<td>" + BillDt + "</td>";
            html += "<td>" + PH_CSH_PATSEX + "</td>";
            html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
        }
        $("#tblBillReprint tbody").append(html);
    }
}
function SelectedReprint(selectedrow) {
    var myrow = selectedrow.rowIndex;
    var BillNo = parseFloat(selectedrow.cells[1].innerHTML);
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    $.ajax({
        url: "/Pharma/Dispense/GetBillReprint",
        type: "GET",
        data: {
            BillNo: BillNo
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            BillPrint(response);
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
    //  $('#modal-default').modal().hide();
    $("#ModelReprint").dialog("close");
}
function OpenBillCancel() {
    GetLast100BillCancelHeader();
    $("#ModelBillCancel").dialog(
        {
            title: "Bill Cancel",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelBillCancel").dialog("close");
                }
            }
        });
}
function GetLast100BillCancelHeader() {
    var StoreName = $("#ddlStoreName option:selected").text();
    $.ajax({
        url: "/Pharma/Dispense/GetLast100BillHeader",
        type: "GET",
        dataType: "json",
        data: {
            StoreName: StoreName
        },
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblBillCancel tbody").empty();
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                    var BillDt = response[PCHeader].BillDt;
                    var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                    var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                    var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                    var PH_CSH_NETTAMOUNT = response[PCHeader].PH_CSH_NETTAMOUNT;
                    var PH_CSH_ROUNDOFF = response[PCHeader].PH_CSH_ROUNDOFF;
                    var PH_CSH_TAXAMOUNT = response[PCHeader].PH_CSH_TAXAMOUNT;
                    var PH_CSH_CONCESSION = response[PCHeader].PH_CSH_CONCESSION;
                    var PH_CSH_TOTAMOUNT = response[PCHeader].PH_CSH_TOTAMOUNT;
                    var PH_CSH_CASHRECIVEDAMT = response[PCHeader].PH_CSH_CASHRECIVEDAMT;

                    html += "<tr ><td>" + Sno + "</td>";
                    html += "<td>" + PH_CSH_BILLNO + "</td>";
                    html += "<td>" + PH_CSH_PATNAME + "</td>";
                    html += "<td>" + BillDt + "</td>";
                    html += "<td>" + PH_CSH_PATSEX + "</td>";
                    html += "<td>" + PH_CSH_TOTAMOUNT + "</td>";
                    html += "<td>" + PH_CSH_TAXAMOUNT + "</td>";
                    html += "<td>" + PH_CSH_ROUNDOFF + "</td>";
                    html += "<td>" + PH_CSH_CASHRECIVEDAMT + "</td>";
                    html += "<td style='text-align: center;'>";
                    html += "<button class='btn btn - primary' onclick='SelectBillCancel(this)'>Bill Cancel</button></td>";
                    html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                }
                $("#tblBillCancel tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function SelectBillCancel(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var BillNo = parseFloat(row.cells[1].innerHTML);
    if (confirm('Are you sure you want to Cancel this Bill ?')) {
        $.ajax({
            url: "/Pharma/SalesReturn/GetHeaderSelectedReturnBill",
            type: "GET",
            data: {
                BillNo: BillNo
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.length > 0) {
                    alert("Cannot Cancel bill the Already Returned Bill")
                }
                else {

                    $.ajax({
                        url: "/Pharma/Dispense/GetBillCancel",
                        type: "GET",
                        data: {
                            BillNo: BillNo
                        },
                        dataType: "json",
                        beforeSend: function () { $("#loading").css("display", "block"); },
                        success: function (response) {
                            GetLast100BillCancelHeader();
                        },
                        complete: function () { $("#loading").css("display", "none"); }
                    });
                }

            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    else {
        return false;
    }
}
function OpenCasPatientEntry() {
    $("#ModelCashBill").dialog(
        {
            title: "OTC",
            width: 900,
            height: 500,
            modal: true,
            buttons: {
                "Save": function () {
                    DisplayPatientInfo();
                    $("#ModelCashBill").dialog("close");
                },
                "Cancel": function () {
                    $("#ModelCashBill").dialog("close");
                }
            }
        });
}
function StoreNameChange() {
    var StoreName = document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text;
    if (StoreName === 'CASH') {
        OpenCasPatientEntry()
    }
    $("#txtTotalamt").val('');
    $("#txtTotalVatmat").val('');
    $("#txtGrandTotal").val('');
    $("#txtRateValue").val('');
    $("#txtDiscountAmt").val('');
    $("#txtCurrentDue").val('');
    $("#txtRoundoff").val('');
    $("#txtNetTotal").val('');

    $("#txtCashAmt").val('');
    $("#txtBankAmt").val();
    $("#txtBankRefNum").val('');
    $("#txtChequeAmt").val('');
    $("#txtChequeNumber").val('');

    $("#txtDebitCardAmt").val('');
    $("#txtDebitCardNumber").val('');
    $("#txtBankRefNum").val('');
    $("#txtChequeAmt").val('');
    $("#txtChequeNumber").val('');
    $("#tblDrugSales tbody").empty();

}
function DisplayPatientInfo() {
    //$("#txtPatName").val('');
    //$("#txtSex").val('');
    //$("#txtYear").val('');
    //$("#txtMonth").val('');
    //$("#txtDay").val('');
    //$("#txtReleation").val('');
    //$("#txtPhoneNumber").val('');
    //$("#txtRefDoctor").val('');
    var errors = "";
    if ($("#txtPatName").val() === "") {
        errors += "* Please Fill Patient Name.\n";
    }
    if ($("#txtSex").val() === "0") {
        errors += "* Please Select Gender. \n";
    }

    if (errors === "") {
        $("#lblPatinetName").text($("#txtPatName").val());
        $("#lblGender").text($("#txtSex").val() + "/" + $("#txtYear").val() + "Y");
        $("#lblMobileNo").text($("#txtPhoneNumber").val());
        $("#lblRelation").text($("#txtReleation").val());
        $("#lblDoctor").text($("#txtRefDoctor").val());
        $("#txtPatType").val("OTC");
        $("#lblPatType").text("OTC");
        $("#hidSex").val($("#txtSex").val());
        $("#hidAge").val($("#txtYear").val());
    }
    else {
        alert('One or more errors occurred:\n\n' + errors);
        OpenCasPatientEntry();
    }

}
function validate() {
    var PatientName = $("#lblPatinetName").text();
    var RECIEVEDAMT = "";
    var CashReceivedAmt = "";
    var DebitCardAmt = "";
    var CreditCardAmt = "";
    var ThroughBankAmt = "";
    var ChequeAmt = "";

    var amt = $("#txtCashAmt").val();
    var debitCard = $("#txtDebitCardAmt").val();
    var creditamt = $("#txtCreditCardAmt").val();
    var Bankamt = $("#txtBankAmt").val();
    var CheqAmt = $("#txtChequeAmt").val();

    if (amt > "0")
        CashReceivedAmt = parseFloat($("#txtCashAmt").val());

    if (debitCard > "0")
        DebitCardAmt = parseFloat($("#txtDebitCardAmt").val());
    if (creditamt > "0")
        CreditCardAmt = parseFloat($("#txtCreditCardAmt").val());
    if (Bankamt > "0")
        ThroughBankAmt = parseFloat($("#txtBankAmt").val());
    if (CheqAmt > "0") {
        ChequeAmt = parseFloat($("#txtChequeAmt").val());
    }
    if (amt == "" && debitCard == "" && creditamt == "" && Bankamt == "" && CheqAmt == "") {
        alert("Please Enter the Minimum 0 Value ");
        return false;
    }

    if (RECIEVEDAMT == "") {
        RECIEVEDAMT = 0;
    }
    if (CashReceivedAmt == "") {
        CashReceivedAmt = 0;
    }
    if (DebitCardAmt == "") {
        DebitCardAmt = 0;
    }
    if (CreditCardAmt == "") {
        CreditCardAmt = 0;
    }
    if (ThroughBankAmt == "") {
        ThroughBankAmt = 0;
    }
    if (ChequeAmt == "") {
        ChequeAmt = 0;
    }




    var errors = "";
    if (PatientName == "") {
        errors += "* Please Fill Patient Name.\n";
    }
    var Ord_nettotal = $("#txtNetTotal").val();
    var RECIEVEDAMT = parseFloat(CashReceivedAmt) + parseFloat(DebitCardAmt) + parseFloat(ThroughBankAmt) + parseFloat(ChequeAmt) + parseFloat(CreditCardAmt);
    var PayMode = $("#ddlPaymentMode option:selected").text();
    if (PayMode == "Self Payment") {
        if (RECIEVEDAMT < Ord_nettotal) {
            DueAmount = $("#txtCurrentDue").val();
            //errors += "* Please do not enter Less amount.\n";*/
            //PendingAmt();
            //return false;
            if (confirm('Pending to Pay Rs-' + DueAmount + ',Still you want Continue? ')) {
                return true;

            } else {
                return false;
            }
        }
        if (RECIEVEDAMT > Ord_nettotal) {
            errors += "* Please do not enter greater amount.\n";
        }
    }


    var table = document.getElementById("tblDrugSales");
    var tbodyRowCount = table.tBodies[0].rows.length;
    if (tbodyRowCount <= 0) {
        errors += "* Please Select Drugs.\n";
    }
    if (errors.length > 0) {
        alert('One or more errors occurred:\n\n' + errors);
        return false;
    }
    else {
        return true;
    }
}


//function SavePendindtoPay() {
//    //var change = document.getElementById("btnsave");
//    //if (change.value == "false" || change.value == "") {
//    //    /* BillSave();*/
//    //    return false;

//    //} else {
//    //    change.value = "true";
//    //    return true;

//    //}
//    var change = document.getElementById("btnsave");
//    if (change.value == "false" ) {
//        return false;
//        //document.test.submit();
//    } else {
//        change.value = "true";
//        return true;
//    }

//}

function ClearAll() {

    $("#txtSearchPatID").val('');
    $("#txtPatName").val('');
    $("#txtSex").val('');
    $("#txtYear").val('');
    $("#txtMonth").val('');
    $("#txtDay").val('');
    $("#txtReleation").val('');
    $("#txtPhoneNumber").val('');
    $("#txtRefDoctor").val('');
    $("#txtDrugSearch").val('');

    $("#lblPatientID").text('');
    $("#lblPatinetName").text('');
    $("#lblGender").text('');
    $("#lblMobileNo").text('');
    $("#lblRelation").text('');
    $("#lblDoctor").text('');

    $("#txtTotalamt").val('');
    $("#txtTotalVatmat").val('');
    $("#txtGrandTotal").val('');
    $("#txtRateValue").val('');
    $("#txtDiscountAmt").val('');
    $("#txtCurrentDue").val('');
    $("#txtRoundoff").val('');
    $("#txtNetTotal").val('');

    $("#txtCashAmt").val('');
    $("#txtBankAmt").val();
    $("#txtBankRefNum").val('');
    $("#txtChequeAmt").val('');
    $("#txtChequeNumber").val('');

    $("#txtCreditCardAmt").val('');
    $("#txtCreditCardNum").val('');
    $("#txtDebitCardAmt").val('');
    $("#txtDebitCardNumber").val('');
    $("#txtBankAmt").val('');
    $("#txtBankRefNum").val('');
    $("#txtChequeAmt").val('');
    $("#txtChequeNumber").val('');
    $("#tblDrugSales tbody").empty();

    $("#hidHoldBillId").val(0);
    $("#ddlPaymentMode").empty();
    $("#ddlPaymentMode").append($("<option></option>").val('Self Payment').html('Self Payment'));
    $("#tblDispCashPayment").prop('hidden', false);
}
function GetCashBillHeaderByBillNo() {
    if ($("#txtBillNo").val().length > 0) {
        var BillNo = parseFloat($("#txtBillNo").val());
        $.ajax({
            url: "/Pharma/Dispense/GetDispenseBillBySearch",
            type: "GET",
            data: {
                BillNo: BillNo
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                BindLast100BillHeader(response);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}
function GetBillCancelByBillNo() {
    if ($("#txtBillNoCancel").val().length > 0) {
        var BillNo = parseFloat($("#txtBillNoCancel").val());
        $.ajax({
            url: "/Pharma/Dispense/GetDispenseBillBySearch",
            type: "GET",
            data: {
                BillNo: BillNo
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.length > 0) {
                    var html = "";
                    var Sno = 0;
                    $("#tblBillCancel tbody").empty();
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        Sno = PCHeader + 1;
                        var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                        var BillDt = response[PCHeader].BillDt;
                        var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                        var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                        var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                        var PH_CSH_NETTAMOUNT = response[PCHeader].PH_CSH_NETTAMOUNT;
                        var PH_CSH_ROUNDOFF = response[PCHeader].PH_CSH_ROUNDOFF;
                        var PH_CSH_TAXAMOUNT = response[PCHeader].PH_CSH_TAXAMOUNT;
                        var PH_CSH_CONCESSION = response[PCHeader].PH_CSH_CONCESSION;
                        var PH_CSH_TOTAMOUNT = response[PCHeader].PH_CSH_TOTAMOUNT;
                        var PH_CSH_CASHRECIVEDAMT = response[PCHeader].PH_CSH_CASHRECIVEDAMT;

                        html += "<tr ><td>" + Sno + "</td>";
                        html += "<td>" + PH_CSH_BILLNO + "</td>";
                        html += "<td>" + PH_CSH_PATNAME + "</td>";
                        html += "<td>" + BillDt + "</td>";
                        html += "<td>" + PH_CSH_PATSEX + "</td>";
                        html += "<td>" + PH_CSH_TOTAMOUNT + "</td>";
                        html += "<td>" + PH_CSH_TAXAMOUNT + "</td>";
                        html += "<td>" + PH_CSH_ROUNDOFF + "</td>";
                        html += "<td>" + PH_CSH_CASHRECIVEDAMT + "</td>";
                        html += "<td style='text-align: center;'>";
                        html += "<button class='btn btn - primary' onclick='SelectBillCancel(this)'>Bill Cancel</button></td>";
                        html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                    }
                    $("#tblBillCancel tbody").append(html);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}

function GetCashBillBySearch() {
    if ($("#txtBillSearch").val().length > 0) {
        var Search = $("#txtBillSearch").val();
        $.ajax({
            url: "/Pharma/Dispense/GetCashBillBySearch",
            type: "GET",
            data: {
                Search: Search
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.length > 0) {
                    var html = "";
                    var Sno = 0;
                    $("#tblBillReprint tbody").empty();
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        Sno = PCHeader + 1;
                        var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                        var BillDt = response[PCHeader].BillDt;
                        var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                        var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                        var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                        var PH_CSH_PATID = response[PCHeader].PH_CSH_PATID;
                        html += "<tr onclick='SelectedReprint(this)'><td>" + Sno + "</td>";
                        html += "<td>" + PH_CSH_BILLNO + "</td>";
                        html += "<td>" + PH_CSH_PATID + "</td>";
                        html += "<td>" + PH_CSH_PATNAME + "</td>";
                        html += "<td>" + BillDt + "</td>";
                        html += "<td>" + PH_CSH_PATSEX + "</td>";
                        html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                    }
                    $("#tblBillReprint tbody").append(html);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}
function GetCancelCashBillBySearch() {
    if ($("#txtCancelBillSearch").val().length > 0) {
        var Search = $("#txtCancelBillSearch").val();
        $.ajax({
            url: "/Pharma/Dispense/GetCashBillBySearch",
            type: "GET",
            data: {
                Search: Search
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var html = "";
                var Sno = 0;
                $("#tblBillCancel tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                    var BillDt = response[PCHeader].BillDt;
                    var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                    var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                    var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                    var PH_CSH_NETTAMOUNT = response[PCHeader].PH_CSH_NETTAMOUNT;
                    var PH_CSH_ROUNDOFF = response[PCHeader].PH_CSH_ROUNDOFF;
                    var PH_CSH_TAXAMOUNT = response[PCHeader].PH_CSH_TAXAMOUNT;
                    var PH_CSH_CONCESSION = response[PCHeader].PH_CSH_CONCESSION;
                    var PH_CSH_TOTAMOUNT = response[PCHeader].PH_CSH_TOTAMOUNT;
                    var PH_CSH_CASHRECIVEDAMT = response[PCHeader].PH_CSH_CASHRECIVEDAMT;

                    html += "<tr ><td>" + Sno + "</td>";
                    html += "<td>" + PH_CSH_BILLNO + "</td>";
                    html += "<td>" + PH_CSH_PATNAME + "</td>";
                    html += "<td>" + BillDt + "</td>";
                    html += "<td>" + PH_CSH_PATSEX + "</td>";
                    html += "<td>" + PH_CSH_TOTAMOUNT + "</td>";
                    html += "<td>" + PH_CSH_TAXAMOUNT + "</td>";
                    html += "<td>" + PH_CSH_ROUNDOFF + "</td>";
                    html += "<td>" + PH_CSH_CASHRECIVEDAMT + "</td>";
                    html += "<td style='text-align: center;'>";
                    html += "<button class='btn btn - primary' onclick='SelectBillCancel(this)'>Bill Cancel</button></td>";
                    html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                }
                $("#tblBillCancel tbody").append(html);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}
function GetSearchPatDeatils() {
    $.ajax({
        url: "/Pharma/Dispense/GetSearchPatDeatils",
        type: "GET",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblHISPatient tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var Lis_Patient_Id = response[PCHeader].Lis_Patient_Id;
                    var Lis_PatFirstName = response[PCHeader].Lis_PatFirstName;
                    var Lis_AgeYear = response[PCHeader].Lis_AgeYear;
                    var Lis_AgeMonth = response[PCHeader].Lis_AgeMonth;
                    var Lis_AgeDays = response[PCHeader].Lis_AgeDays;
                    var Lis_Sex = response[PCHeader].Lis_Sex;
                    var HIS_PHONE = response[PCHeader].HIS_PHONE;
                    var LisAddress = response[PCHeader].LisAddress;
                    var LIS_RefDRNAME = response[PCHeader].LIS_RefDRNAME;
                    var LIS_Pattype = response[PCHeader].LIS_Pattype;
                    if (LIS_Pattype == "0") {
                        LIS_Pattype = "OP"
                    }
                    else if (LIS_Pattype == "1"){
                        LIS_Pattype = "IP"
                    }
                        
                    var LIS_VisitDate = response[PCHeader].LIS_VisitDate;
                    var HIS_IsPatientResponsible = response[PCHeader].HIS_IsPatientResponsible;

                    html += "<tr onclick='SelectPatRow(this)'><td>" + Sno + "</td>";
                    html += "<td>" + Lis_Patient_Id + "</td>";
                    html += "<td>" + Lis_PatFirstName + "</td>";
                    html += "<td>" + Lis_Sex + "</td>";
                    html += "<td>" + Lis_AgeYear + 'Y ' + Lis_AgeMonth + 'M ' + Lis_AgeDays+"D </td>";
                    html += "<td>" + LIS_RefDRNAME + "</td>";
                    html += "<td>" + HIS_PHONE + "</td>";
                    html += "<td>" + LisAddress + "</td>";
                    html += "<td>" + LIS_VisitDate + "</td>";
                    html += "<td>" + LIS_Pattype + "</td>";
                    html += "<td style='display:none;'>" + HIS_IsPatientResponsible + "</td>";
                    html += "</tr>";
                }
                $("#tblHISPatient tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetSearchPatDeatilsBySearchTearm() {
    if ($("#txtPatientSearch").val().length > 0) {
        var Search = $("#txtPatientSearch").val();
        $.ajax({
            url: "/Pharma/Dispense/GetSearchPatDeatilsBySearchTearm",
            type: "GET",
            data: {
                Search: Search
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.length > 0) {
                    var html = "";
                    var Sno = 0;
                    $("#tblHISPatient tbody").empty();
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        Sno = PCHeader + 1;
                        var Lis_Patient_Id = response[PCHeader].Lis_Patient_Id;
                        var Lis_PatFirstName = response[PCHeader].Lis_PatFirstName;
                        var Lis_AgeYear = response[PCHeader].Lis_AgeYear;
                        var Lis_Sex = response[PCHeader].Lis_Sex;
                        var HIS_PHONE = response[PCHeader].HIS_PHONE;
                        var LisAddress = response[PCHeader].LisAddress;
                        var LIS_RefDRNAME = response[PCHeader].LIS_RefDRNAME;
                        var LIS_Pattype = response[PCHeader].LIS_Pattype;
                        var LIS_Pattype = response[PCHeader].LIS_Pattype;
                        if (LIS_Pattype == "0") {
                            LIS_Pattype = "OP"
                        }
                        else if (LIS_Pattype == "1") {
                            LIS_Pattype = "IP"
                        }

                        var LIS_VisitDate = response[PCHeader].LIS_VisitDate;
                        var HIS_IsPatientResponsible = response[PCHeader].HIS_IsPatientResponsible;

                        html += "<tr onclick='SelectPatRow(this)'><td>" + Sno + "</td>";
                        html += "<td>" + Lis_Patient_Id + "</td>";
                        html += "<td>" + Lis_PatFirstName + "</td>";
                        html += "<td>" + Lis_Sex + "</td>";
                        html += "<td>" + Lis_AgeYear + "</td>";
                        html += "<td>" + LIS_RefDRNAME + "</td>";
                        html += "<td>" + HIS_PHONE + "</td>";
                        html += "<td>" + LisAddress + "</td>";
                        html += "<td>" + LIS_VisitDate + "</td>";
                        html += "<td>" + LIS_Pattype + "</td>";
                        html += "<td style='display:none;'>" + HIS_IsPatientResponsible + "</td>";
                        html += "</tr>";
                    }
                    $("#tblHISPatient tbody").append(html);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }

}
function OpenPatient() {
    GetSearchPatDeatils();
    $("#ModelPatient").dialog(
        {
            title: "Patient Deatils",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelPatient").dialog("close");
                }
            }
        });
}
function SelectPatRow(selectedrow) {
    var myrow = selectedrow.rowIndex;
    var PatientID = selectedrow.cells[1].innerHTML;
    GetTreatmentHeaderByPatientID(PatientID);
    $.ajax({
        url: "/Pharma/Dispense/GetPatDeatilsByPatID",
        type: "GET",
        data: {
            PatientID: PatientID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var Lis_PatFirstName = response[PCHeader].Lis_PatFirstName;
                        var Lis_Sex = response[PCHeader].Lis_Sex;
                        var Lis_AgeDays = response[PCHeader].Lis_AgeDays;
                        var Lis_AgeMonth = response[PCHeader].Lis_AgeMonth;
                        var Lis_AgeYear = response[PCHeader].Lis_AgeYear;
                        var HIS_RELATION = response[PCHeader].HIS_RELATION;
                        var HIS_RELATIONNAME = response[PCHeader].HIS_RELATIONNAME;
                        var HIS_PHONE = response[PCHeader].HIS_PHONE;
                        var LisAddress = response[PCHeader].LisAddress;
                        var LIS_RefDRNAME = response[PCHeader].LIS_RefDRNAME;
                        var LIS_Pattype = response[PCHeader].LIS_Pattype;
                        var HIS_CompanyName = response[PCHeader].HIS_CompanyName;
                        var HIS_StaffID = response[PCHeader].HIS_StaffID;
                        var HIS_StaffDepartment = response[PCHeader].HIS_StaffDepartment;
                        var HIS_IsPatientResponsible = response[PCHeader].HIS_IsPatientResponsible;
                        if (LIS_RefDRNAME === "")
                            LIS_RefDRNAME = "-";
                        if (HIS_PHONE === "")
                            HIS_PHONE = "-";
                        if (HIS_RELATIONNAME === "")
                            HIS_RELATIONNAME = "-";
                        var LIS_Pattype_text = "";



                        if (LIS_Pattype === "0") {
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].value = "OPPHARMACY";
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "OP PHRAMACY";
                            LIS_Pattype_text = "OP";
                            $("#ddlPaymentMode").empty();
                            $("#tblDispCashPayment").prop('hidden', false);
                            $("#ddlPaymentMode").append($("<option></option>").val('Self Payment').html('Self Payment'));
                            //$("#ddlStoreName").val("OPPHARMACY");
                            //Claim not done for OP Patient in HIS
                            //if (HIS_CompanyName != "") {
                            //    $("#ddlPaymentMode").append($("<option></option>").val('Company Payment').html('Company Payment'));
                            //}
                        }
                        else if (LIS_Pattype === "1") {
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].value = "IPPHARMACY";
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "IP PHRAMACY";
                            LIS_Pattype_text = "IP";
                            //$("#ddlStoreName").val("IPPHARMACY");
                            $("#ddlPaymentMode").empty();
                            $("#tblDispCashPayment").prop('hidden', true);

                            if (HIS_CompanyName != "") {
                                $("#ddlPaymentMode").append($("<option></option>").val('Company Payment').html('Company Payment'));
                            }

                            $("#ddlPaymentMode").append($("<option></option>").val('IP Payment').html('IP Payment'));
                            $("#ddlPaymentMode").append($("<option></option>").val('Self Payment').html('Self Payment'));
                        }
                        else {
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].value = "OPPHARMACY";
                            //document.getElementById("ddlStoreName").options[document.getElementById('ddlStoreName').selectedIndex].text = "OP PHRAMACY";
                            //$("#ddlStoreName").val("OPPHARMACY");
                        }

                        $("#txtPatName").val(Lis_PatFirstName);
                        $("#txtSex").val(Lis_Sex);
                        $("#txtYear").val(Lis_AgeDays);
                        $("#txtMonth").val(Lis_AgeMonth);
                        $("#txtDay").val(Lis_AgeYear);
                        $("#txtReleation").val(HIS_RELATION + ' ' + HIS_RELATION);
                        $("#txtPhoneNumber").val(HIS_PHONE);
                        $("#txtRefDoctor").val(LIS_RefDRNAME);
                        $("#txtSearchPatID").val(PatientID);

                        $("#lblPatientID").text(PatientID);
                        $("#lblPatinetName").text(Lis_PatFirstName);
                        $("#lblGender").text(Lis_Sex + "/" + Lis_AgeYear + "Y " + Lis_AgeMonth + "M " + Lis_AgeDays + "D");
                        $("#lblMobileNo").text(HIS_PHONE);
                        $("#lblRelation").text(HIS_RELATIONNAME);
                        $("#lblDoctor").text(LIS_RefDRNAME);

                        $("#lblPatType").text(LIS_Pattype_text);
                        $("#lblPatCompany").text(HIS_CompanyName);
                        $("#lblStaffCode").text(HIS_StaffID);
                        $("#lblStaffDept").text(HIS_StaffDepartment);
                        $("#lblIsResponse").text(HIS_IsPatientResponsible);
                        $("#hidSex").val(Lis_Sex);
                        $("#hidAge").val(Lis_AgeYear);

                    }
                }
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    $("#ModelPatient").dialog("close");
    return false;
}
function OnPaymentModeChange() {
    try {
        var PayMode = $("#ddlPaymentMode option:selected").text();
        if (PayMode !== "Self Payment") {
            $("#tblDispCashPayment").prop('hidden', true);
        }
        else {
            $("#tblDispCashPayment").prop('hidden', false);
        }
    }
    catch (e) { console.log(e); }
}
function BillHoldSave() {
    if (HoldValidation()) {
        var DrugInfo = new Array();
        var tblDrugSales = document.getElementById("tblDrugSales");
        var rowtblDrugSales = tblDrugSales.rows.length;
        for (M = 1; M < rowtblDrugSales; M++) {
            var rowDrug = tblDrugSales.rows[M];
            var ObjectDetails = new Object();
            ObjectDetails.PH_CUR_DRUGBRANDNAME = rowDrug.cells[2].innerHTML;
            ObjectDetails.PH_CUR_STOCK_EXPIRYDT = rowDrug.cells[4].innerHTML;
            ObjectDetails.PH_CUR_STOCK_BILLINGPRICE = parseFloat(rowDrug.cells[5].innerHTML);
            ObjectDetails.PH_CUR_STOCK_BATCHNO = rowDrug.cells[3].innerHTML;
            var QtyVlaue = rowDrug.cells[6].getElementsByTagName("input")[0].value;
            if (QtyVlaue !== "")
                ObjectDetails.Qty = parseInt(rowDrug.cells[6].getElementsByTagName("input")[0].value);
            else
                ObjectDetails.Qty = 0;
            ObjectDetails.PH_CUR_STOCK = parseInt(rowDrug.cells[12].innerHTML);
            ObjectDetails.PH_CUR_DRUGCODE = parseInt(rowDrug.cells[1].innerHTML);
            ObjectDetails.PH_ITEM_DRUG_VAT = parseFloat(rowDrug.cells[10].innerHTML);
            ObjectDetails.PH_CUR_STOCK_PURCHCOST = parseFloat(rowDrug.cells[13].innerHTML);
            var Amt = rowDrug.cells[7].innerHTML;
            if (Amt !== "")
                ObjectDetails.Amount = parseFloat(rowDrug.cells[7].innerHTML);
            else
                ObjectDetails.Amount = 0;
            var TaxValue = rowDrug.cells[8].innerHTML;
            if (TaxValue !== "")
                ObjectDetails.Tax = parseFloat(rowDrug.cells[8].innerHTML);
            else
                ObjectDetails.Tax = 0;
            var TotalAmtValue = rowDrug.cells[9].innerHTML;
            if (TotalAmtValue !== "")
                ObjectDetails.TotalAmount = parseFloat(rowDrug.cells[9].innerHTML);
            else
                ObjectDetails.TotalAmount = 0;
            ObjectDetails.ExpiryDt = rowDrug.cells[14].innerHTML;
            DrugInfo.push(ObjectDetails);
        }
        var Amount = $("#txtTotalamt").val();
        if (Amount == "" || Amount == null || Amount == '')
            Amount = 0;
        else
            Amount = parseFloat(Amount);

        var Tax = $("#txtTotalVatmat").val();
        if (Tax == "" || Tax == null || Tax == '')
            Tax = 0;
        else
            Tax = parseFloat(Tax);

        var TotalAmount = $("#txtGrandTotal").val();
        if (TotalAmount == "" || TotalAmount == null || TotalAmount == '')
            TotalAmount = 0;
        else
            TotalAmount = parseFloat(TotalAmount);

        var DiscountType = $("#ddlDiscountType").val();

        var DiscountRate = $("#txtDiscountAmt").val();
        if (DiscountRate == "" || DiscountRate == null || DiscountRate == '')
            DiscountRate = 0;
        else
            DiscountRate = parseFloat(DiscountRate);

        var DueCollected = $("#txtCurrentDue").val();
        if (DueCollected == "" || DueCollected == null || DueCollected == '')
            DueCollected = 0;
        else
            DueCollected = parseFloat(DueCollected);

        var NetCollected = $("#txtNetTotal").val();
        if (NetCollected == "" || NetCollected == null || NetCollected == '')
            NetCollected = 0;
        else
            NetCollected = parseFloat(NetCollected);

        var Roundoff = $("#txtRoundoff").val();
        if (Roundoff == "" || Roundoff == null || Roundoff == '')
            Roundoff = 0;
        else
            Roundoff = parseFloat(Roundoff);

        var Consession = $("#txtDiscountAmt").val();
        if (Consession == "" || Consession == null || Consession == '')
            Consession = 0;
        else
            Consession = parseFloat(Consession);

        var CashReceivedAmt = 0;
        var DebitCardAmt = 0;
        var CreditCardAmt = 0;
        var ThroughBankAmt = 0;
        var ChequeAmt = 0;

        if ($("#txtCashAmt").val().length > 0)
            CashReceivedAmt = parseFloat($("#txtCashAmt").val());
        if ($("#txtDebitCardAmt").val().length > 0)
            DebitCardAmt = parseFloat($("#txtDebitCardAmt").val());
        if ($("#txtCreditCardAmt").val().length > 0)
            CreditCardAmt = parseFloat($("#txtCreditCardAmt").val());
        if ($("#txtBankAmt").val().length > 0)
            ThroughBankAmt = parseFloat($("#txtBankAmt").val());
        if ($("#txtChequeAmt").val().length > 0)
            ChequeAmt = parseFloat($("#txtChequeAmt").val());

        var CreditCardNumber = $("#txtCreditCardNum").val();
        var DebitCardNumber = $("#txtDebitCardNumber").val();
        var ChequeNo = $("#txtChequeNumber").val();
        var BankRefNo = $("#txtBankRefNum").val();

        var sendJsonData = {
            PatinetID: $("#txtSearchPatID").val(),
            Lis_PatFirstName: $("#txtPatName").val(),
            Lis_Sex: $("#txtSex").val(),
            Lis_AgeDays: $("#txtDay").val(),
            Lis_AgeMonth: $("#txtMonth").val(),
            Lis_AgeYear: $("#txtYear").val(),
            HIS_RELATION: $("#txtReleation").val(),
            HIS_PHONE: $("#txtPhoneNumber").val(),
            LisAddress: '',
            LIS_RefDRNAME: $("#txtRefDoctor").val(),
            DrugInfos: DrugInfo,
            Amount: Amount,
            Tax: Tax,
            TotalAmount: TotalAmount,
            Roundoff: Roundoff,
            Consession: Consession,
            DiscountType: DiscountType,
            DiscountRate: DiscountRate,
            PendingtoPay: DueCollected,
            NetTotlaAmount: NetCollected,
            CashReceivedAmt: CashReceivedAmt,
            CreditCardAmt: CreditCardAmt,
            DebitCardAmt: DebitCardAmt,
            ThroughBankAmt: ThroughBankAmt,
            ChequeAmt: ChequeAmt,
            CreditCardNumber: CreditCardNumber,
            DebitCardNumber: DebitCardNumber,
            ChequeNo: ChequeNo,
            BankRefNo: BankRefNo,
            StoreName: $('#ddlStoreName :selected').text()
        };
        $.ajax({
            url: "/Pharma/Dispense/BillHoldSave",
            type: 'post',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(sendJsonData),
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (data) {
                ClearAll();
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
}
function HoldValidation() {
    var PatientName = document.getElementById('txtPatName').value;
    var errors = "";
    if (PatientName == "") {
        errors += "* Please Fill Patient Name.\n";
    }
    var table = document.getElementById("tblDrugSales");
    var tbodyRowCount = table.tBodies[0].rows.length;
    if (tbodyRowCount <= 0) {
        errors += "* Please Select Drugs.\n";
    }
    if (errors.length > 0) {
        alert('One or more errors occurred:\n\n' + errors);
        return false;
    }
    else {
        return true;
    }
}
function GetHoldBillHeader() {
    var StoreName = $("#ddlStoreName option:selected").text();
    $.ajax({
        url: "/Pharma/Dispense/GetHoldBillHeader",
        type: "GET",
        dataType: "json",
        data: {
            StoreName: StoreName
        },
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblBillHold tbody").empty();
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var SeqID = response[PCHeader].SeqID;
                    var HoldNumber = response[PCHeader].HoldNumber;
                    var PatientID = response[PCHeader].PatientID;
                    var Name = response[PCHeader].Name;
                    var Gender = response[PCHeader].Gender;
                    var Age = response[PCHeader].Age;
                    var MobileNo = response[PCHeader].MobileNo;
                    var Doctor = response[PCHeader].Doctor;
                    var Realtion = response[PCHeader].Realtion;
                    var CreateDatetime = response[PCHeader].CreateDatetime;
                    var StoreName = response[PCHeader].StoreName;

                    html += "<tr ><td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + SeqID + "</td>";
                    html += "<td>" + PatientID + "</td>";
                    html += "<td>" + Name + "</td>";
                    html += "<td>" + Gender + "</td>";
                    html += "<td>" + Age + "</td>";
                    html += "<td>" + Doctor + "</td>";
                    html += "<td>" + MobileNo + "</td>";
                    html += "<td>" + CreateDatetime + "</td>";
                    html += "<td style='display:none;'>" + Realtion + "</td>";
                    html += "<td style='display:none;'>" + StoreName + "</td>";
                    html += "<td style='text-align:center;' ><img src='/images/Delete.png' style='height:20px;width:20px;' title='Delete' onclick='DeleteDispenseHoldBill(this);'/></td>";
                    html += "</tr>";
                }
                $("#tblBillHold tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function DeleteDispenseHoldBill(selectedtd) {
    var row = selectedtd.parentNode.parentNode;
    var SeqID = row.cells[1].innerHTML;
    try {
        $.ajax({
            url: "/Pharma/Dispense/DeleteHoldBillHeader",
            type: 'GET',
            dataType: "json",
            data: {
                SeqID: SeqID
            },
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                GetHoldBillHeader();
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    catch (e) { console.log(e); }
}
function OpenHoldBills() {
    GetHoldBillHeader();
    $("#ModelBillHold").dialog(
        {
            title: "Hold Bills",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelBillHold").dialog("close");
                }
            }
        });
}
function SelectHoldBillRow(selectedtd) {
    var selectedrow = selectedtd.context.parentNode;
    var HoldBill = parseFloat(selectedrow.cells[1].innerHTML);

    //$("#txtPatName").val(selectedrow.cells[3].innerHTML);
    //$("#txtSex").val(selectedrow.cells[4].innerHTML);
    //$("#txtYear").val(selectedrow.cells[5].innerHTML);
    //$("#txtMonth").val('');
    //$("#txtDay").val('');
    //$("#txtReleation").val(selectedrow.cells[9].innerHTML);
    //$("#txtPhoneNumber").val(selectedrow.cells[7].innerHTML);
    //$("#txtRefDoctor").val(selectedrow.cells[6].innerHTML);
    //$("#txtSearchPatID").val(selectedrow.cells[2].innerHTML);

    var StoreName = selectedrow.cells[10].innerHTML;

    $("#lblPatientID").text(selectedrow.cells[2].innerHTML);
    $("#lblPatinetName").text(selectedrow.cells[3].innerHTML);
    $("#lblGender").text(selectedrow.cells[4].innerHTML + "/" + selectedrow.cells[5].innerHTML);
    $("#hidSex").val(selectedrow.cells[4].innerHTML);
    $("#lblMobileNo").text(selectedrow.cells[7].innerHTML);
    $("#lblRelation").text(selectedrow.cells[9].innerHTML);
    $("#lblDoctor").text(selectedrow.cells[6].innerHTML);
    $("#txtSearchPatID").val(selectedrow.cells[2].innerHTML);
    GetPatDeatilsByPatID();
    $.ajax({
        url: "/Pharma/Dispense/GetHoldBillDetailsBySeqID",
        type: "GET",
        data: {
            SeqID: HoldBill
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                $("#hidHoldBillId").val(HoldBill);
                $("#tblDrugSales tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var HoldSeqID = response[PCHeader].HoldSeqID;
                    var DrugCode = response[PCHeader].DrugCode;
                    var Batch = response[PCHeader].Batch;
                    var Qty = response[PCHeader].Qty;
                    var StoreName = $('#ddlStoreName :selected').text();
                    var gstType = $('#lblGst').html();
                    GetCurrentStockByDrugCodeAndBatch(DrugCode, StoreName, Batch, Qty, gstType);
                }
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    $("#ModelBillHold").dialog("close");
    return false;
}

function onclickUserPin() {
    var StoreName = $('#ddlStoreName :selected').text();

    var StoreCode = $('#ddlStoreName').val();


    //if (StoreName = 'OPPHARMACY') {
    //    StoreCode = 1;
    //}
    //else if (StoreName = 'CASH') {
    //    StoreCode = 2;
    //}
    //else if (StoreName = 'IPPHARMACY') {
    //    StoreCode = 3;
    //}



    $.ajax({
        url: "/Pharma/Dispense/CreateStorePinDeatils?StoreName=" + StoreName + "&StoreCode=" + StoreCode,
        type: 'post',
        contentType: "application/json; charset=utf-8",
        /*data: JSON.stringify(UserPin),*/
        dataType: "json",
        success: function (data) {
            alert('Store Name Pinned Successfully!!');
        },
    });
}
function GetStoreName() {
    $.ajax({
        url: "/Pharma/Invoice/GetStoreName",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var HIS_PH_STOREMASTER = response[PCHeader].HIS_PH_STOREMASTER;
                    var HIS_PH_STORENAME = response[PCHeader].HIS_PH_STORENAME;
                    $('#ddlStoreName')
                        .append($("<option></option>").val(HIS_PH_STOREMASTER).text(HIS_PH_STORENAME));
                }
                GetDeatilsByUserId();
            }
        }
    });
}
function GetDeatilsByUserId() {
    $.ajax({
        url: "/Pharma/Dispense/GetStorePinDeatilsByUserId",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        /*data: JSON.stringify(UserPin),*/
        dataType: "json",
        success: function (response) {
            /*  var storeName = response[0].HIS_PH_STOREMASTER*/
            if (response.length > 0) {
                var StoreName = response[0].StoreName;
                var StoreCode = response[0].StoreCode;
                document.getElementById('ddlStoreName').value = StoreCode;
            }
            //document.getElementById('ddlStoreName').text = StoreName;

        },
    });
}
function OnChangeSearchPhone() {
    var Phone = parseFloat($("#txtSearchPhone").val());
    GetPatDeatilsByMobileNo(Phone);
}
function GetPatDeatilsByMobileNo(MobileNo) {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    $.ajax({
        url: "/Pharma/Dispense/GetPatDeatilsByMobileNo",
        type: "GET",
        data: {
            MobileNo: MobileNo
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                    var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                    var PH_CSH_PHONE = response[PCHeader].PH_CSH_PHONE;
                    var PH_CSH_Relation = response[PCHeader].PH_CSH_Relation;
                    var PH_REF_DOCTOR = response[PCHeader].PH_REF_DOCTOR;
                    $("#txtPatName").val(PH_CSH_PATNAME);
                    $("#txtSex").val(PH_CSH_PATSEX);
                    $("#txtYear").val('');
                    $("#txtReleation").val(PH_CSH_Relation);
                    $("#txtPhoneNumber").val(PH_CSH_PHONE);
                    $("#txtRefDoctor").val(PH_REF_DOCTOR);
                }
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GotoLogin() {
    try {
        window.location.href = "/Login/Login";
    }
    catch (e) {
    }
}
function GetTreatmentHeaderByPatientID(PatientID) {

    $.ajax({
        url: "/Pharma/Dispense/GetTreatmentHeaderByPatientID",
        type: "GET",
        data: {
            PatientID: PatientID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                $("#tblTreatment tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var SeqID = response[PCHeader].SeqID;
                    var PatientID = response[PCHeader].PatientID;
                    var CreateDate = response[PCHeader].CreateDate;
                    var DoctorName = response[PCHeader].DoctorName;
                    html += "<tr>";
                    html += "<td><input type='image' style='width:21px;height:21px;' src='" + rootUrl + "/Images/details_open.png' onclick='javascript:return CheckTreatmentClick(this)'></td>";
                    html += "<td style='display:none;'>" + SeqID + "</td>";
                    html += "<td>" + PatientID + "</td>";
                    html += "<td>" + CreateDate + "</td>";
                    html += "<td>" + DoctorName + "</td>";
                    html += "<td><button type='button' onclick='return OnSelectPrescription(this)'>Select</button></td>";
                    $("#tblTreatment tbody").append(html);
                    html = "";
                }
                $("#ModalTreatment").dialog(
                    {
                        title: "Prescription",
                        width: 806,
                        height: 500,
                        modal: true,
                        buttons: {
                            "Cancel": function () {
                                $("#ModalTreatment").dialog("close");
                            }
                        }
                    });
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function CheckTreatmentClick(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var myrow = SelectedRow.parentNode.parentNode;
    var rowIndex = row.rowIndex - 1;
    var PSeqID = row.cells[1].innerHTML;

    var className = myrow.cells[0].getElementsByTagName("input")[0].className;
    if (className === "shown") {
        var imgUrl = rootUrl + "/Images/details_open.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'abc');
        $('#tblTreatment > tbody > tr').eq(rowIndex).next().remove();
    }
    else {
        var imgUrl = rootUrl + "/Images/details_close.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'shown');

        var arg = parseFloat(PSeqID);
        $.ajax({
            url: "/Pharma/Dispense/GetPatientDrugDeatilsBySeqID/?PHSeqID=" + arg + "",
            type: 'Get',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var html = "<table id='myTable1' class='table table-striped table-bordered table-hover table-full-width dataTable no-footer'>";
                html += "<tr>";
                html += "<thead><th>Brand Name</th><th>Frequency</th><th>Duration</th><th>Remarks</th>";
                html += "</thead>";
                html += "</tr>";
                for (SymCount = 0; SymCount < response.length; SymCount++) {
                    var DrugCode = response[SymCount].DrugCode;
                    var DrugName = response[SymCount].DrugName;
                    var Frequency = response[SymCount].Frequency;
                    var Duration = response[SymCount].Duration;
                    var Remarks = response[SymCount].Remarks;
                    html += "<tr>";
                    html += "<td>" + DrugName + "</td>";
                    html += "<td>" + Frequency + "</td>";
                    html += "<td>" + Duration + "</td>";
                    html += "<td>" + Remarks + "</td>";
                    html += "</tr>";
                }
                html += "</table></br>";
                var newRow = $('<tr><td></td><td colspan="5">' + html + '</td></tr>');
                $('#tblTreatment > tbody > tr').eq(rowIndex).after(newRow);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    return false;
}
function OnSelectPrescription(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var dtlSeqID = parseFloat(row.cells[1].innerHTML);
    var arg = parseFloat(dtlSeqID);
    $.ajax({
        url: "/Pharma/Dispense/GetPatientDrugDeatilsBySeqID/?PHSeqID=" + arg + "",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            for (SymCount = 0; SymCount < response.length; SymCount++) {
                var DrugCode = parseInt(response[SymCount].DrugCode);
                var StoreName = $('#ddlStoreName :selected').text();
                var gst = $('#lblGst').html();
                GetTreatmentDrugCode(DrugCode, StoreName, gst);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function GetTreatmentDrugCode(DrugCode, StoreName,gst) {
    $.ajax({
        url: "/Pharma/Dispense/GetCurrentStockByDrugCode",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            StoreName: StoreName,
            GstType: gst

        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblDrugSales");
                var tbodyRowCount = table.tBodies[0].rows.length;
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;

                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        var ExpiryDt = response[PCHeader].ExpiryDt;
                        var Sno = tbodyRowCount + 1;
                        var $tr = $('#tblDrugSales tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                        if ($tr.length === 0) {
                            html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";//0
                            html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//1
                            html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";//2
                            html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";//3
                            html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";//4
                            html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";//5
                            html += "<td><input value='' style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/> " + PH_CUR_STOCK + "</td>";//6
                            html += "<td  class='AmountCell'></td>";//7
                            html += "<td class='TaxCell'></td>";//8
                            html += "<td class='TotalAmountCell'></td>";//9
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";//10
                            html += "<td style='text-align: center;'>";//11
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                            html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";//12
                            html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";//13
                            html += "<td style='display:none;'>" + ExpiryDt + "</td>";//14
                            html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";//15
                            html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";//16

                            $("#tblDrugSales tbody").append(html);
                            html = "";
                        }
                    }
                }
                else {
                    var BatchNoDropdown = "";
                    ($.map(response, function (item) {
                        BatchNoDropdown += ("<option value=" + item.PH_CUR_STOCK_BATCHNO + "> Batch-" + item.PH_CUR_STOCK_BATCHNO + "- EXP-" + item.PH_CUR_STOCK_EXPIRYDT + " -QTY-" + item.PH_CUR_STOCK + "</option>");//.val(item.Batchno).html(item.Batchno)
                    }));
                    var table = document.getElementById("tblDrugSales");
                    var tbodyRowCount = table.tBodies[0].rows.length;
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var Sno = tbodyRowCount + 1;
                        if (PCHeader == 0) {
                            var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                            var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                            var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                            html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\"><td>" + Sno + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//0
                            html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";//1
                            html += "<td><select id='ddlbatch" + Sno + "' class='m-wrap large' multiple='multiple' style='font-size:14px;width: 250px !important'  onchange='return getBatchValue(this)'>" + BatchNoDropdown + "</select></td>";//2
                            html += "<td></td>";//3
                            html += "<td></td>";//4
                            html += "<td><input value='' style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//5
                            html += "<td  class='AmountCell'></td>";//6
                            html += "<td class='TaxCell'></td>";//7
                            html += "<td class='TotalAmountCell'></td>";//7
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";//8
                            html += "<td style='text-align: center;'>";//9
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                            html += "<td style='display:none;'></td>";//10
                            html += "<td style='display:none;'></td>";//11
                            html += "<td style='display:none;'></td>";//12
                            html += "<td style='display:none;'></td>";//13
                            html += "<td style='display:none;'></td>";//14
                            html += "</tr>";
                            $("#tblDrugSales tbody").append(html);
                            html = "";
                        }
                    }
                }
                $("#ModalTreatment").dialog("close");
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function getBatchValue(selectedrow) {
    var myrow = selectedrow.parentNode.parentNode;
    var DrugCode = parseInt(myrow.cells[1].innerHTML);
    var batchno = myrow.cells[3].getElementsByTagName("select")[0].value;
    var StoreName = $('#ddlStoreName :selected').text();
    var gstType = $('#lblGst').html();

    $.ajax({
        url: "/Pharma/Dispense/GetCurrentStockByDrugCodeAndBatch",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            StoreName: StoreName,
            Batch: batchno,
            GstType: gstType,
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblDrugSales");
                var tbodyRowCount = table.tBodies[0].rows.length;
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;
                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        var ExpiryDt = response[PCHeader].ExpiryDt;
                        var DISPENSE_EXPIRYDT = response[PCHeader].DispenseExpiry;
                        //Date checked in Store Procedure -Habib 07/07/21
                        //if (PH_CUR_STOCK_EXPIRYDT != "") {
                        //    var getpurchase = GetDispnesePurchaseExpiry(DISPENSE_EXPIRYDT);
                        //    if (getpurchase == false) {
                        //        alert("Cannot Dispense Expired Drug");
                        //        $('#txtDrugSearch').val('');
                        //        return false;
                        //    }
                        //}
                        var Sno = tbodyRowCount + 1;
                        myrow.cells[3].innerHTML = "";
                        myrow.cells[3].innerHTML = "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                        myrow.cells[4].innerHTML = "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                        myrow.cells[5].innerHTML = "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                        myrow.cells[6].innerHTML = "";
                        myrow.cells[6].innerHTML = "<td><input value='' style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/> " + PH_CUR_STOCK + "</td>";
                        myrow.cells[12].innerHTML = "<td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                        myrow.cells[13].innerHTML = "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                        myrow.cells[14].innerHTML = "<td style='display:none;'>" + ExpiryDt + "</td>";
                        myrow.cells[15].innerHTML = "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                        myrow.cells[16].innerHTML = "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td>";
                    }
                }
            }
            else {
                alert("Cannot Dispense Expired Drug");
                $('#txtDrugSearch').val('');
                return false;
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetClientGstDetails() {
    $.ajax({
        url: rootUrl + "/api/ClientMaster/GetClientDetailsByHospitalId",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var gst = response[0].GstRequired;
                $('#lblGst').html(gst);
            }
        }
    })
}
function GetCurrentStockBatchByDrugCodeAndBatch(DrugCode, StoreName, Btach, Qty, GstType) {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    $.ajax({
        url: "/Pharma/Dispense/GetCurrentStockByDrugCodeAndBatch",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            StoreName: StoreName,
            Batch: Btach,
            GstType: GstType
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var gst = $('#lblGst').html();
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblDrugSales");
                var tbodyRowCount = table.tBodies[0].rows.length;
                if (response.length <= 1) {
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                        var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                        var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                        var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                        var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                        var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;
                        var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                        var PH_CUR_STOCKUOM = response[PCHeader].PH_CUR_STOCKUOM;
                        var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                        var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                        var ExpiryDt = response[PCHeader].ExpiryDt;
                        var DISPENSE_EXPIRYDT = response[PCHeader].DispenseExpiry;
                        //Date checked in Store Procedure - Habib 07/07/21
                        //if (PH_CUR_STOCK_EXPIRYDT != "") {
                        //    var getpurchase = GetDispnesePurchaseExpiry(DISPENSE_EXPIRYDT);
                        //    if (getpurchase == false) {
                        //        alert("Cannot Dispense Expired Drug");
                        //        $('#txtDrugSearch').val('');
                        //        return false;
                        //    }
                        //}
                        var Qty = "";
                        //if (gst == "Registered") {
                        //    var amount = parseFloat(Qty * PH_CUR_STOCK_BILLINGPRICE).toFixed(2);
                        //    var Tax = parseFloat(PH_ITEM_DRUG_VAT).toFixed(2);
                        //    var vatamt = parseFloat((amount * Tax) / 100).toFixed(2);
                        //    var total = parseFloat(amount) + parseFloat(vatamt);
                        //    total = total.toFixed(2);
                        //}

                        //else {
                        //    var amount = parseFloat(Qty * PH_CUR_STOCK_BILLINGPRICE).toFixed(2);
                        //    var Tax = "";
                        //    var vatamt = 0;
                        //    var total = parseFloat(amount);
                        //    total = total.toFixed(2);
                        //}

                        var Sno = tbodyRowCount + 1;
                        var $tr = $('#tblDrugSales tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                        if ($tr.length === 0) {
                            html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                            html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                            html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                            html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                            html += "<td><input style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' value='" + Qty + "' onkeypress='return isNumberKey(event)'> " + PH_CUR_STOCK + "</td></td>";
                            html += "<td  class='AmountCell'></td>";
                            html += "<td class='TaxCell'></td>";
                            html += "<td class='TotalAmountCell'></td>";
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";
                            html += "<td style='text-align: center;'>";
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                            html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                            html += "<td style='display:none;'>" + ExpiryDt + "</td>";
                            html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                            html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";
                            $("#tblDrugSales tbody").append(html);
                            $("#tblDrugSales").find("input[type='text']").last().focus();
                            $("#txtDrugSearch").val('');
                            html = "";

                        }
                        else {
                            var $tr = $('#tblDrugSales tr[data-Batch="' + PH_CUR_STOCK_BATCHNO + '"]');
                            if ($tr.length === 0) {
                                html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\" data-Batch=\"" + PH_CUR_STOCK_BATCHNO + "\"><td>" + Sno + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                                html += "<td>" + PH_CUR_DRUGBRANDNAME + "</td>";
                                html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                                html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                                html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                                html += "<td><input style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='QtyValu(this)' value='" + Qty + "' onkeypress='return isNumberKey(event)'> " + PH_CUR_STOCK + "</td></td>";
                                html += "<td  class='AmountCell'></td>";
                                html += "<td class='TaxCell'></td>";
                                html += "<td class='TotalAmountCell'></td>";
                                html += "<td style='display:none;'>" + PH_ITEM_DRUG_VAT + "</td>";
                                html += "<td style='text-align: center;'>";
                                html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";
                                html += "</td><td style='display:none;'>" + PH_CUR_STOCK + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                                html += "<td style='display:none;'>" + ExpiryDt + "</td>";
                                html += "<td style='display:none;'>" + PH_CUR_STOCKUOM + "</td>";
                                html += "<td style='display:none;'>" + PH_ITEM_HSNCODE + "</td></tr>";
                                $("#tblDrugSales tbody").append(html);
                                $("#tblDrugSales").find("input[type='text']").last().focus();
                                $("#txtDrugSearch").val('');
                                html = "";
                            }
                            else {
                                alert("Already Drug and Batch Added");
                            }
                        }
                        TotalCalculation();
                        TotalDiscountCalculation();
                        //$("#txtDrugSearch").focus();
                    }
                }
            }
            else {
                alert("Cannot Dispense Expired Drug");
                $('#txtDrugSearch').val('');
                return false;
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetDispnesePurchaseExpiry(DISPENSE_EXPIRYDT) {
    var wareHouse = parseFloat($('#ddlStoreName :selected').val());
    var returnbool;
    $.ajax({
        url: "/Pharma/Dispense/GetDispnesePurchaseExpiry",
        type: "GET",
        data: {
            wareHouse: wareHouse
        },
        dataType: "json",
        async: false,
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var res = response;
            var arr2 = res.split('-');
            var Disyear = arr2[0];
            var Dismonth = arr2[1];
            var Disday = arr2[2];
            var dispenseExpiry = new Date(Disyear, Dismonth - 1, Disday);
            var expiryDate = DISPENSE_EXPIRYDT;
            var d = expiryDate.split(' ')[0];
            var drugEx = d.split('/');
            var Dday = drugEx[0];
            var Dmonth = drugEx[1];
            var Dyear = drugEx[2];
            var drugExpiry = new Date(Dyear, Dmonth - 1, Dday);
            if (drugExpiry > dispenseExpiry) {
                returnbool = true
            }
            else {
                returnbool = false
            }
        }

    });
    return returnbool;
}
//selvendiran
function GetDispenseBillBySearchAllCondtion() {
    if ($("#txtDisReprintBillSearch").val().length > 0) {
        var Search = $("#txtDisReprintBillSearch").val();
        $.ajax({
            url: "/Pharma/Dispense/GetDispenseBillBySearchAllCondtion",
            type: "GET",
            data: {
                SearchTearm: Search
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                BindLast100BillHeader(response);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}
function GetDispenseBillBySearchAllBillCancel() {
    if ($("#txtDisCancelBillSearch").val().length > 0) {
        var Search = $("#txtDisCancelBillSearch").val();
        $.ajax({
            url: "/Pharma/Dispense/GetDispenseBillBySearchAllCondtion",
            type: "GET",
            data: {
                SearchTearm: Search
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.length > 0) {
                    var html = "";
                    var Sno = 0;
                    $("#tblBillCancel tbody").empty();
                    for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                        Sno = PCHeader + 1;
                        var PH_CSH_BILLNO = response[PCHeader].PH_CSH_BILLNO;
                        var BillDt = response[PCHeader].BillDt;
                        var PH_CSH_PATNAME = response[PCHeader].PH_CSH_PATNAME;
                        var PH_CSH_PATSEX = response[PCHeader].PH_CSH_PATSEX;
                        var PH_CSH_PROCESSKEY = response[PCHeader].PH_CSH_PROCESSKEY;
                        var PH_CSH_NETTAMOUNT = response[PCHeader].PH_CSH_NETTAMOUNT;
                        var PH_CSH_ROUNDOFF = response[PCHeader].PH_CSH_ROUNDOFF;
                        var PH_CSH_TAXAMOUNT = response[PCHeader].PH_CSH_TAXAMOUNT;
                        var PH_CSH_CONCESSION = response[PCHeader].PH_CSH_CONCESSION;
                        var PH_CSH_TOTAMOUNT = response[PCHeader].PH_CSH_TOTAMOUNT;
                        var PH_CSH_CASHRECIVEDAMT = response[PCHeader].PH_CSH_CASHRECIVEDAMT;

                        html += "<tr ><td>" + Sno + "</td>";
                        html += "<td>" + PH_CSH_BILLNO + "</td>";
                        html += "<td>" + PH_CSH_PATNAME + "</td>";
                        html += "<td>" + BillDt + "</td>";
                        html += "<td>" + PH_CSH_PATSEX + "</td>";
                        html += "<td>" + PH_CSH_TOTAMOUNT + "</td>";
                        html += "<td>" + PH_CSH_TAXAMOUNT + "</td>";
                        html += "<td>" + PH_CSH_ROUNDOFF + "</td>";
                        html += "<td>" + PH_CSH_CASHRECIVEDAMT + "</td>";
                        html += "<td style='text-align: center;'>";
                        html += "<button class='btn btn - primary' onclick='SelectBillCancel(this)'>Bill Cancel</button></td>";
                        html += "<td style='display:none;'>" + PH_CSH_PROCESSKEY + "</td></tr>";
                    }
                    $("#tblBillCancel tbody").append(html);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        return false;
    }
}
function GetHoldBillHeaderSearch() {
    var StoreName = $("#ddlStoreName option:selected").text();
    var Search = $("#txtBillHoldSearch").val();
    $.ajax({
        url: "/Pharma/Dispense/GetHoldBillHeaderSearch",
        type: "GET",
        dataType: "json",
        data: {
            StoreName: StoreName,
            SearchTearm:Search
        },
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblBillHold tbody").empty();
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var SeqID = response[PCHeader].SeqID;
                    var HoldNumber = response[PCHeader].HoldNumber;
                    var PatientID = response[PCHeader].PatientID;
                    var Name = response[PCHeader].Name;
                    var Gender = response[PCHeader].Gender;
                    var Age = response[PCHeader].Age;
                    var MobileNo = response[PCHeader].MobileNo;
                    var Doctor = response[PCHeader].Doctor;
                    var Realtion = response[PCHeader].Realtion;
                    var CreateDatetime = response[PCHeader].CreateDatetime;
                    var StoreName = response[PCHeader].StoreName;

                    html += "<tr ><td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + SeqID + "</td>";
                    html += "<td>" + PatientID + "</td>";
                    html += "<td>" + Name + "</td>";
                    html += "<td>" + Gender + "</td>";
                    html += "<td>" + Age + "</td>";
                    html += "<td>" + Doctor + "</td>";
                    html += "<td>" + MobileNo + "</td>";
                    html += "<td>" + CreateDatetime + "</td>";
                    html += "<td style='display:none;'>" + Realtion + "</td>";
                    html += "<td style='display:none;'>" + StoreName + "</td>";
                    html += "<td style='text-align:center;' ><img src='/images/Delete.png' style='height:20px;width:20px;' title='Delete' onclick='DeleteDispenseHoldBill(this);'/></td>";
                    html += "</tr>";
                }
                $("#tblBillHold tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetExpiredDrugByStore() {
    var StoreName = $("#ddlStoreName option:selected").text();
    $.ajax({
        url: "/Pharma/Dispense/GetExpiredDrugByStore",
        type: "GET",
        dataType: "json",
        data: {
            StoreName: StoreName
        },
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblExpiredReprot tbody").empty();
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                    var PH_ITEM_DRUGNAME_BRAND = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                    var PH_CUR_STOCK_EXPIRYDT = response[PCHeader].PH_CUR_STOCK_EXPIRYDT;
                    var PH_CUR_STOCK_BATCHNO = response[PCHeader].PH_CUR_STOCK_BATCHNO;
                    var PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                    var PH_CUR_STOCK_BILLINGPRICE = response[PCHeader].PH_CUR_STOCK_BILLINGPRICE;
                    var PH_CUR_STOCK = response[PCHeader].PH_CUR_STOCK;
                    var PH_CUR_STOCK_STORENAME = response[PCHeader].PH_CUR_STOCK_STORENAME;

                    html += "<tr ><td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";
                    html += "<td>" + PH_ITEM_DRUGNAME_BRAND + "</td>";
                    html += "<td>" + PH_CUR_STOCK_BATCHNO + "</td>";
                    html += "<td>" + PH_CUR_STOCK_EXPIRYDT + "</td>";
                    html += "<td>" + PH_CUR_STOCK + "</td>";
                    html += "<td>" + PH_CUR_STOCK_PURCHCOST + "</td>";
                    html += "<td>" + PH_CUR_STOCK_BILLINGPRICE + "</td>";
                    html += "<td>" + PH_CUR_STOCK_STORENAME + "</td>";
                    html += "</tr>";
                }
                $("#tblExpiredReprot tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function OpenExpiredReport() {
    GetExpiredDrugByStore();
    $("#ModalExpiredReprot").dialog(
        {
            title: "Expired Report",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModalExpiredReprot").dialog("close");
                }
            }
        });
}
