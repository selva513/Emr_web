$(document).ready(function () {
    var SeqIDList = sessionStorage.getItem("ListSeqIDList");
    if (SeqIDList !== '') {
        GetDrugsBySeqIDList(SeqIDList);
    }
    var SupplierID = parseInt(sessionStorage.getItem("SupplierID"));
    GetSupplierMaster();
    dateFunction();
    GetDCStateDeatilsForGst();
    $("#ddlSuplier").val(SupplierID);
});
function GetSupplierMaster() {
    $.ajax({
        url: "/Pharma/Invoice/GetAllSupplierMaster",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var PH_SupplierID = response[PCHeader].PH_SupplierID;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;
                    $('#ddlSuplier')
                        .append($("<option></option>").val(PH_SupplierID).html(PH_SupplierName));
                }
                var SupplierID = parseInt(sessionStorage.getItem("SupplierID"));
                $("#ddlSuplier").val(SupplierID);
            }
        }
    });
}
function dateFunction() {
    var objDate = new Date();
    var Presentyear = objDate.getFullYear();
    $("#txtInvoiceDate").datepicker({
        // yearRange: '1900:' + Presentyear,
        changeMonth: true,
        changeYear: true,
        // dateFormat: "mm/dd/yy"
        dateFormat: "dd/mm/yy",
        maxDate: '0d'
    });
}
function OnSupplierChanges() {
    var SupplierID = parseInt($("#ddlSuplier").val());
    SelectSupplierID(SupplierID);
}
function SelectSupplierID(SupplierID) {
    var arg = parseInt(SupplierID);
    $.ajax({
        url: "/Pharma/DcInvoice/GetDCDrugDeatilsBySupplier/?SupplierID=" + arg + "",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var html = "";
            var Sno = 0;
            $("#tblDrugBind tbody").empty();
            for (SymCount = 0; SymCount < response.Deatils.length; SymCount++) {
                Sno = SymCount + 1;
                html += "<tr onclick='javascript:return CheckSelect(this)'>";
                html += "<td>" + Sno + "</td>";//00
                html += "<td>" + response.Deatils[SymCount].PH_ITEM_DRUGNAME_BRAND + "</td>";//1
                html += "<td>" + response.Deatils[SymCount].PH_DCDTL_DRUGBATCHNO + "</td>";//2
                html += "<td>" + response.Deatils[SymCount].PH_DCDTL_DRUGEXPIRY + "</td>";//3
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RECVDQTY + "</td>";//4
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_BONUSQTY + "</td>";//5
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RATEEACH + "</td>";//6
                html += "<td style='display:none;'>" + response.Deatils[SymCount].MRP + "</td>";//7
                html += "<td style='display:none;'>" + response.Deatils[SymCount].GST + "</td>";//8
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_AMOUNT + "</td>";//9
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_DRUGCODE + "</td>";//10
                html += "<td style='display:none;'>" + response.Deatils[SymCount].DcNumber + "</td>";//11
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_SEQID + "</td>";//12
                html += "<td style='display:none;'>" + response.Deatils[SymCount].DiscountPrec + "</td>";//13
                html += "</tr>";
            }
            $("#tblDrugBind tbody").append(html);
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
var SummaryArray = new Array();
function CheckSelect(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var DrugCode = SelectedRow.cells[10].innerHTML;
    var BrandName = SelectedRow.cells[1].innerHTML;
    var Batch = SelectedRow.cells[2].innerHTML;
    var Expiry = SelectedRow.cells[3].innerHTML;
    var Qty = SelectedRow.cells[4].innerHTML;
    var FreeQty = SelectedRow.cells[5].innerHTML;
    var Cost = parseFloat(SelectedRow.cells[6].innerHTML);
    var MRP = parseFloat(SelectedRow.cells[7].innerHTML);
    var Tax = parseFloat(SelectedRow.cells[8].innerHTML);
    var amt = parseFloat(Cost * Qty).toFixed(2);
    var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
    var total = parseFloat(amt) + parseFloat(vatamt);
    total = total.toFixed(2);
    var DtlSeqID = SelectedRow.cells[12].innerHTML;
    var DicountPre = parseFloat(SelectedRow.cells[13].innerHTML);
    var disAmt = parseFloat((amt * DicountPre) / 100).toFixed(2);
    amt = parseFloat(amt) - parseFloat(disAmt);
    vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
    total = parseFloat(amt) + parseFloat(vatamt);
    total = total.toFixed(2);
    var html = "";
    var $tr = $('#tblSelectDrugBind tr[data-id="' + DrugCode + '"]');
    var $batch = $('#tblSelectDrugBind td[class="' + Batch + '"]');
    if ($tr.length === 0 || $batch.length === 0) {
        html += "<tr data-id=\"" + DrugCode + "\">";
        html += "<td>" + BrandName + "</td>";//0
        html += "<td class='" + Batch + "'>" + Batch + "</td>";//1
        html += "<td>" + Expiry + "</td>";//2
        html += "<td>" + Qty + "</td>";//3
        html += "<td>" + FreeQty + "</td>";//4
        html += "<td>" + Cost + "</td>";//5
        html += "<td>" + MRP + "</td>";//6
        html += "<td class='AmountCell'>" + amt + "</td>";//7
        html += "<td>" + Tax + "</td>";//8
        html += "<td class='TaxCell'>" + vatamt + "</td>";//9
        html += "<td class='TotalAmountCell'>" + total + "</td>";//10
        html += "<td style='display:none;'>" + DrugCode + "</td>";//11
        html += "<td style='display:none;'>" + DtlSeqID + "</td>";//12
        html += "<td><input value='" + DicountPre + "'style='height:28px;width:50px;text-align: right;margin-top:5px;' type=\"Text\" onchange='DCtoInvoiceDiscount(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//13
        html += "<td style='text-align: center;'>";
        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";//14
        html += "</tr>";
        $("#tblSelectDrugBind tbody").append(html);
        var ObjectDetails = new Object();
        ObjectDetails.DrugCode = parseInt(DrugCode);
        ObjectDetails.Amount = parseFloat(amt);
        ObjectDetails.Tax =parseFloat(vatamt);
        ObjectDetails.TaxPrecentage = parseFloat(Tax);
        ObjectDetails.NetAmount = parseFloat(total);
        SummaryArray.push(ObjectDetails);
        
        $.ajax({
            url: "/Pharma/DcInvoice/SummaryCalculation",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(SummaryArray),
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.Summary.length > 0) {
                    html = "";
                    $("#tblInvoiceSummary tbody").empty();
                    for (PsCount = 0; PsCount < response.Summary.length; PsCount++) {
                        var TaxPre = response.Summary[PsCount].TaxPrecentage;
                        var Amt = response.Summary[PsCount].Amount;
                        var SummaryTax = response.Summary[PsCount].Tax;
                        var SummaryNetAmount = response.Summary[PsCount].NetAmount;
                        html += "<tr>";
                        html += "<td>" + TaxPre + "</td>";
                        html += "<td><input value='" + Amt + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                        html += "<td><input value='" + SummaryTax + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                        html += "<td><input value='" + SummaryNetAmount + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                        html += "</tr>";
                    }
                    $("#tblInvoiceSummary tbody").append(html);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
        TotalCalculation();
    }
    else {
        alert("Drug And Batch Already Selected");
    }
    
    //var className = row.cells[0].getElementsByTagName("input")[0].className;
    return false;
}
function groupBy(objectArray, property) {
    return objectArray.reduce((acc, obj) => {
        const key = obj[property];
        if (!acc[key]) {
            acc[key] = [];
        }
        // Add object to list for given key's value
        acc[key].push(obj);
        return acc;
    }, {});
}
function GetDrugsBySeqIDList(SeqIDList) {
    $.ajax({
        url: "/Pharma/DcInvoice/GetDCDrugDeatilsByDcSeqIDList/?SeqIDList=" + SeqIDList + "",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var html = "";
            var Sno = 0;
            for (SymCount = 0; SymCount < response.Deatils.length; SymCount++) {
                Sno = SymCount + 1;
                html += "<tr onclick='javascript:return CheckSelect(this)'>";
                html += "<td>" + Sno + "</td>";//00
                html += "<td>" + response.Deatils[SymCount].PH_ITEM_DRUGNAME_BRAND + "</td>";//1
                html += "<td>" + response.Deatils[SymCount].PH_DCDTL_DRUGBATCHNO + "</td>";//2
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_DRUGEXPIRY + "</td>";//3
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RECVDQTY + "</td>";//4
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_BONUSQTY + "</td>";//5
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RATEEACH + "</td>";//6
                html += "<td style='display:none;'>" + response.Deatils[SymCount].MRP + "</td>";//7
                html += "<td style='display:none;'>" + response.Deatils[SymCount].GST + "</td>";//8
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_AMOUNT + "</td>";//9
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_DRUGCODE + "</td>";//10
                html += "<td>" + response.Deatils[SymCount].DcNumber + "</td>";//11
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_SEQID + "</td>";//12
                html += "<td style='display:none;'>" + response.Deatils[SymCount].DiscountPrec + "</td>";//13
                html += "</tr>";
            }
            $("#tblDrugBind tbody").append(html);
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function TotalCalculation() {
    var AmountCells = document.getElementsByClassName("AmountCell"); //returns a list with all the elements that have class 'priceCell'
    var Amount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < AmountCells.length; i++) {
        var thisPrice = parseFloat(AmountCells[i].innerHTML); //get inner text of this cell in number format
        Amount = Amount + thisPrice;
    };
    Amount = Amount.toFixed(2);
    $("#txtSummaryAmt").val(Amount);

    var TaxCells = document.getElementsByClassName("TaxCell"); //returns a list with all the elements that have class 'priceCell'
    var Tax = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < TaxCells.length; i++) {
        var thisPrice = parseFloat(TaxCells[i].innerHTML); //get inner text of this cell in number format
        Tax = Tax + thisPrice;
    };
    Tax = Tax.toFixed(2);
    $("#txtSummaryTax").val(Tax);

    var TotalAmountCells = document.getElementsByClassName("TotalAmountCell"); //returns a list with all the elements that have class 'priceCell'
    var TotalAmount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < TotalAmountCells.length; i++) {
        var thisPrice = parseFloat(TotalAmountCells[i].innerHTML); //get inner text of this cell in number format
        TotalAmount = TotalAmount + thisPrice;
    };
    TotalAmount = TotalAmount.toFixed(2);
    $("#txtSummaryNetAmt").val(TotalAmount);
    $("#txtNetTotal").val(TotalAmount);
}
function DeleteOrdersRow(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var myrow = selectedrow.parentNode.parentNode;
    var DrugCode = parseFloat(row.cells[11].innerHTML);
   
    var TaxExists = SummaryArray.findIndex(user => user.DrugCode === DrugCode);
    if (TaxExists !== -1) SummaryArray.splice(TaxExists, 1);
    $.ajax({
        url: "/Pharma/DcInvoice/SummaryCalculation",
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(SummaryArray),
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.Summary.length > 0) {
                html = "";
                $("#tblInvoiceSummary tbody").empty();
                for (PsCount = 0; PsCount < response.Summary.length; PsCount++) {
                    var TaxPre = response.Summary[PsCount].TaxPrecentage;
                    var Amt = response.Summary[PsCount].Amount;
                    var SummaryTax = response.Summary[PsCount].Tax;
                    var SummaryNetAmount = response.Summary[PsCount].NetAmount;
                    html += "<tr>";
                    html += "<td>" + TaxPre + "</td>";
                    html += "<td><input value='" + Amt + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "<td><input value='" + SummaryTax + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "<td><input value='" + SummaryNetAmount + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "</tr>";
                }
                $("#tblInvoiceSummary tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    TotalCalculation();
    document.getElementById("tblSelectDrugBind").deleteRow(myrow.rowIndex);
    var table = document.getElementById("tblSelectDrugBind");
    var rowCount = table.rows.length;
    if (rowCount == 1) {
        $("#tblInvoiceSummary tbody").empty();
        $('#txtSummaryAmt').val('');
        $('#txtSummaryTax').val('');
        $('#txtSummaryNetAmt').val('');
        $('#txtRateValue').val('');
        $('#txtDiscountAmt').val('');
        $('#txtNetTotal').val('');
    }
    else {
        var i = myrow.rowIndex;
        TotalCalculation();
    }
}
function InvoiceSave() {
    if (Validation()) {
        var DrugInfo = new Array();
        var tblDrugSales = document.getElementById("tblSelectDrugBind");
        var rowtblDrugSales = tblDrugSales.rows.length;
        for (M = 1; M < rowtblDrugSales; M++) {
            var rowDrug = tblDrugSales.rows[M];
            var ObjectDetails = new Object();
            ObjectDetails.DrugCode = parseFloat(rowDrug.cells[11].innerHTML);
            ObjectDetails.DrugName = rowDrug.cells[0].innerHTML;
            ObjectDetails.Batch = rowDrug.cells[1].innerHTML;
            ObjectDetails.ExpiryDate = rowDrug.cells[2].innerHTML;
            ObjectDetails.Qty = parseFloat(rowDrug.cells[3].innerHTML);
            ObjectDetails.FreeQty = parseFloat(rowDrug.cells[4].innerHTML);
            ObjectDetails.Rate = parseFloat(rowDrug.cells[5].innerHTML);
            ObjectDetails.MRP = parseFloat(rowDrug.cells[6].innerHTML);
            ObjectDetails.Amount = parseFloat(rowDrug.cells[7].innerHTML);
            ObjectDetails.TaxPrecentage = parseFloat(rowDrug.cells[8].innerHTML);
            ObjectDetails.Tax = parseFloat(rowDrug.cells[9].innerHTML);
            ObjectDetails.Discount = 0;
            ObjectDetails.NetAmount = parseFloat(rowDrug.cells[10].innerHTML);
            ObjectDetails.DtlSeqID = parseFloat(rowDrug.cells[12].innerHTML);
            DrugInfo.push(ObjectDetails);
        }
        var sendJsonData = {
            SupplierInvoiceNumber: $("#txtInvoiceNo").val(),
            SupplierID: parseInt($("#ddlSuplier").val()),
            SupplierInvoiceDate: $("#txtInvoiceDate").val(),
            SupplierName: $("#ddlSuplier option:selected").text(),
            InVoiceTaxType: $("#ddlDcGstType option:selected").text(),
            InvoiceType: $("#ddlInvoiceType option:selected").text(),
            WareHouse: $("#ddlWherHouse option:selected").text(),
            Amount: parseFloat($("#txtSummaryAmt").val()),
            Tax: parseFloat($("#txtSummaryTax").val()),
            NetAmount: parseFloat($("#txtNetTotal").val()),
            Discount: parseFloat($("#txtDiscountAmt").val()),
            DisType: $("#ddlDiscountType").val(),
            DiscountValue: parseFloat($("#txtRateValue").val()),
            DraftDeatils: DrugInfo
        };
        $.ajax({
            url: "/Pharma/DcInvoice/InvoiceSave",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(sendJsonData),
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response === "Supplier Invoice Number Exists") {
                    alert(response);
                }
                else {
                    $("#ddlSuplier").val(0);
                    $("#ddlWherHouse").val(0);
                    $("#txtInvoiceNo").val('');
                    $("#txtInvoiceDate").val('');
                    $("#ddlGstType").val("GST purchase");
                    $("#ddlInvoiceType").val("Select");
                    $("#tblSelectDrugBind tbody").empty();
                    $("#tblInvoiceSummary tbody").empty();
                    $("#tblDrugBind tbody").empty();
                    $("#txtSummaryAmt").val('');
                    $("#txtSummaryTax").val('');
                    $("#txtSummaryNetAmt").val('');
                    $("#txtNetTotal").val('');
                    $("#txtDiscountAmt").val('');
                    $("#txtRateValue").val('');
                    SummaryArray.length = 0;
                    alert(response);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
}
function Validation() {
    var txt = "Required to fill the following field(s)";
    var opt = 0;
    var Supplire = parseFloat($("#ddlSuplier").val());
    if (Supplire === 0) {
        txt += "\n - Please Select Supplier Name";
        var opt = 1;
    }
    var SupplierInvoiceNumber = $("#txtInvoiceNo").val();
    if (SupplierInvoiceNumber === '') {
        txt += "\n - Please Enter Invoice Number";
        var opt = 1;
    }
    var InvoiceDt = $("#txtInvoiceDate").val();
    if (InvoiceDt === '') {
        txt += "\n - Please Enter Invoice Date";
        var opt = 1;
    }
    if (opt == "1") {
        alert(txt);
        $("#txtBrandSearch").focus();
        return false;
    }
    return true;
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
function OnKeyPressBrandSearch() {
    var SupplierID = parseInt($("#ddlSuplier").val());
    var Search = $("#txtBrandSearch").val();
    SelectSupplierIDandSearch(SupplierID, Search);
}
function SelectSupplierIDandSearch(SupplierID,Search) {
    SupplierID = parseInt(SupplierID);
    $.ajax({
        url: "/Pharma/DcInvoice/GetDCDrugDeatilsBySupplierAndDrug",
        type: 'Get',
        data: {
            SupplierID: SupplierID,
            FreeSearch: Search
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblDrugBind tbody").empty();
            var html = "";
            var Sno = 0;
            for (SymCount = 0; SymCount < response.Deatils.length; SymCount++) {
                Sno = SymCount + 1;
                html += "<tr onclick='javascript:return CheckSelect(this)'>";
                html += "<td>" + Sno + "</td>";//00
                html += "<td>" + response.Deatils[SymCount].PH_ITEM_DRUGNAME_BRAND + "</td>";//1
                html += "<td>" + response.Deatils[SymCount].PH_DCDTL_DRUGBATCHNO + "</td>";//2
                html += "<td>" + response.Deatils[SymCount].PH_DCDTL_DRUGEXPIRY + "</td>";//3
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RECVDQTY + "</td>";//4
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_BONUSQTY + "</td>";//5
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_RATEEACH + "</td>";//6
                html += "<td style='display:none;'>" + response.Deatils[SymCount].MRP + "</td>";//7
                html += "<td style='display:none;'>" + response.Deatils[SymCount].GST + "</td>";//8
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_AMOUNT + "</td>";//9
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_DRUGCODE + "</td>";//10
                html += "<td style='display:none;'>" + response.Deatils[SymCount].DcNumber + "</td>";//11
                html += "<td style='display:none;'>" + response.Deatils[SymCount].PH_DCDTL_SEQID + "</td>";//12
                html += "</tr>";
            }
            $("#tblDrugBind tbody").append(html);
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function OpenInvoice() {
    GetTop50Invoice();
    $("#ModelInvoice").dialog(
        {
            title: "Invoice",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelInvoice").dialog("close");
                }
            }
        });
}
function GetTop50Invoice() {
    $.ajax({
        url: "/Pharma/Invoice/GetTop50Invoice",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblInvoice tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_IN_SEQID = response[PCHeader].PH_IN_SEQID;
                    var PH_IN_SUPP_INVNO = response[PCHeader].PH_IN_SUPP_INVNO;
                    var PH_IN_ENTRYDATE = response[PCHeader].PH_IN_ENTRYDATE;
                    var InvoiceType = response[PCHeader].InvoiceType;
                    var InvoiceGSTType = response[PCHeader].InvoiceGSTType;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;
                    var Amount = response[PCHeader].Amount;
                    var Tax = response[PCHeader].Tax;
                    var NetAmount = response[PCHeader].NetAmount;
                    var DiscountAmt = response[PCHeader].DiscountAmt;
                    var PH_INWareHouse = response[PCHeader].PH_INWareHouse;

                    html += "<tr>";
                    html += "<td><input type='image' style='width:21px;height:21px;' src='" + rootUrl + "/Images/details_open.png' onclick='javascript:return CheckSelectOpen(this)'></td>";
                    html += "<td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + PH_IN_SEQID + "</td>";
                    html += "<td>" + PH_SupplierName + "</td>";
                    html += "<td>" + PH_IN_SUPP_INVNO + "</td>";
                    html += "<td>" + PH_IN_ENTRYDATE + "</td>";
                    html += "<td>" + InvoiceType + "</td>";
                    html += "<td>" + InvoiceGSTType + "</td>";
                    html += "<td>" + Amount + "</td>";
                    html += "<td>" + Tax + "</td>";
                    html += "<td>" + DiscountAmt + "</td>";
                    html += "<td>" + NetAmount + "</td>";
                    html += "<td style='text-align: center;display:none;'>";
                    html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteInvoiceRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                    html += "<td style='display:none;'>" + PH_INWareHouse + "</td>";
                    html += "<td><input type='button' value='Print' class='btn btn-primary' onclick='PrintInvoiceBySeqID(this);'/></td>";
                    html += "</tr>";
                }
                $("#tblInvoice tbody").append(html);
            }
        }
    });
}
function CheckSelectOpen(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var myrow = SelectedRow.parentNode.parentNode;
    var rowIndex = row.rowIndex - 1;
    var PSeqID = row.cells[2].innerHTML;

    var className = myrow.cells[0].getElementsByTagName("input")[0].className;
    if (className === "shown") {
        var imgUrl = rootUrl + "/Images/details_open.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'abc');
        $('#tblInvoice > tbody > tr').eq(rowIndex).next().remove();
    }
    else {
        var imgUrl = rootUrl + "/Images/details_close.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'shown');

        var arg = parseFloat(PSeqID);
        $.ajax({
            url: "/Pharma/Invoice/GetInvoiceDrugDeatils/?SeqID=" + arg + "",
            type: 'Get',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var html = "<table id='myTable1' class='table table-striped table-bordered table-hover table-full-width dataTable no-footer'>";
                html += "<tr>";
                html += "<thead><th>Brand Name</th><th>Batch</th><th>Expiry DT</th>";
                html += "<th>Qty</th><th>Free Qty</th>";
                html += "<th>Purchase Cost</th><th>MRP</th><th>GST</th>";
                html += "</thead>";
                html += "</tr>";
                for (SymCount = 0; SymCount < response.Deatils.length; SymCount++) {
                    html += "<tr>";
                    html += "<td>" + response.Deatils[SymCount].PH_ITEM_DRUGNAME_BRAND + "</td>";
                    html += "<td>" + response.Deatils[SymCount].PH_INDTL_DRUGBATCHNO + "</td>";
                    html += "<td>" + response.Deatils[SymCount].PH_INDTL_DRUGEXPIRY + "</td>";
                    html += "<td>" + response.Deatils[SymCount].PH_INDTL_RECVDQTY + "</td>";
                    html += "<td>" + response.Deatils[SymCount].PH_INDTL_BONUSQTY + "</td>";
                    html += "<td>" + response.Deatils[SymCount].PurchaseCost + "</td>";
                    html += "<td>" + response.Deatils[SymCount].BillCost + "</td>";
                    html += "<td>" + response.Deatils[SymCount].GST + "</td>";
                    html += "</tr>";
                }
                html += "</table></br>";
                html += "<table id='myTable1' class='table table-striped table-bordered table-hover table-full-width dataTable no-footer'>";
                html += "<tr style='text-align: center;'>";
                html += "<thead><th>GST %</th><th>Amt</th><th>Tax</th>";
                html += "<th>Total Amt</th>";
                html += "</thead>";
                html += "</tr>";
                for (PsCount = 0; PsCount < response.Summary.length; PsCount++) {
                    var TaxPre = response.Summary[PsCount].GST;
                    var Amt = response.Summary[PsCount].PurAmount;
                    var SummaryTax = response.Summary[PsCount].PurTaxAmount;
                    var SummaryNetAmount = response.Summary[PsCount].PurNetAmount;
                    html += "<tr>";
                    html += "<td style='text-align: center;'>" + TaxPre + "</td>";
                    html += "<td style='text-align: center;'>" + Amt + "</td>";
                    html += "<td style='text-align: center;'>" + SummaryTax + "</td>";
                    html += "<td style='text-align: center;'>" + SummaryNetAmount + "</td>";
                    html += "</tr>";
                }
                html += "</table></br>";
                var newRow = $('<tr><td></td><td colspan="11">' + html + '</td></tr>');
                $('#tblInvoice > tbody > tr').eq(rowIndex).after(newRow);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    return false;
}
function DeleteInvoiceRow(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var myrow = selectedrow.parentNode.parentNode;
    var rowIndex = row.rowIndex;
    var dtlSeqID = parseFloat(row.cells[2].innerHTML);
    var SuuplierInvoice = row.cells[4].innerHTML;
    var WareHosue = row.cells[13].innerHTML;
    document.getElementById("tblInvoice").deleteRow(myrow.rowIndex);
    var table = document.getElementById("tblInvoice");
    var rowCount = table.rows.length;
    var i = myrow.rowIndex;
    regroup(i, rowCount, "tblInvoice");
    DeleteInvoice(dtlSeqID, SuuplierInvoice, WareHosue);
}
function DeleteInvoice(dtlSeqID, SuuplierInvoice, WareHosue) {
    $.ajax({
        url: "/Pharma/Invoice/DeleteInvoice",
        type: "GET",
        data: {
            SeqID: dtlSeqID,
            StoreName: WareHosue,
            SupplierInvoiceNo: SuuplierInvoice
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function PrintInvoiceBySeqID(SelectedRow) {
    try {
        var row = SelectedRow.parentNode.parentNode;
        var SeqID = row.cells[2].innerHTML;

        $.ajax({
            url: "/Pharma/Invoice/GetInvoiceDetailsById?SeqID=" + SeqID,
            type: "GET",
            dataType: "json",
            success: function (response) {
                var html = "";

                if (response.lstHdr.length > 0 && response.lstDtl.length > 0 && response.DtlSummary.length > 0) {
                    var address = response.lstHdr[0].PH_Supplier_Address1 + " " + response.lstHdr[0].PH_Supplier_Address2;
                    var TotalPurAmount = 0;
                    var TotalPurTaxAmount = 0;
                    var TotalPurNetAmount = 0;
                    html += "<table style='font-family:sans-serif;font-size:12px;font-weight:500;'>";
                    html += "<tbody>";
                    html += "<tr>";
                    html += "<td style='width: 196px!important;' >Invoice Number</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + response.lstHdr[0].PH_IN_SUPP_INVNO + "</td>";
                    html += "<td style='width: 196px!important;' >Invoice Date</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + response.lstHdr[0].PH_IN_ENTRYDATE + "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td style='width: 196px!important;' >Supplier Name</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + response.lstHdr[0].PH_SupplierName + "</td>";
                    html += "<td style='width: 196px!important;' >Address</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + address + "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td style='width: 196px!important;' >GST</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + response.lstHdr[0].PH_Supplier_GSTRegNo + "</td>";
                    html += "<td style='width: 196px!important;' >EmailId</td>";
                    html += "<td>:</td>";
                    html += "<td style='width: 341px!important;'>" + response.lstHdr[0].PH_Supplier_EmailId + "</td>";
                    html += "</tr>";
                    html += "</tbody>";
                    html += "</table>";
                    html += "<br/>";



                    html += "<table border='1' style='font-family:sans-serif;font-size:12px;font-weight:500;border-collapse:collapse;width:100%;'>";
                    html += "<thead style='font-weight:bold;'>";
                    html += "<tr>";
                    html += "<td>S.No</td>";
                    html += "<td>Brand Name</td>";
                    html += "<td>Batch</td>";
                    html += "<td>Expiry DT</td>";
                    html += "<td>Qty</td>";
                    html += "<td>Free Qty</td>";
                    html += "<td>Purchase Cost</td>";
                    html += "<td>MRP</td>";
                    html += "<td>GST %</td>";
                    html += "<td>Amount</td>";
                    html += "<td>Tax Amount</td>";
                    html += "<td>Total Amount</td>";
                    html += "</tr>";
                    html += "</thead>";
                    html += "<tbody>";
                    for (var dtl = 0; dtl < response.lstDtl.length; dtl++) {
                        var sno = dtl + 1;
                        var pamount = parseFloat(response.lstDtl[dtl].PurAmount);
                        TotalPurAmount = TotalPurAmount + parseFloat(response.lstDtl[dtl].PurAmount);
                        TotalPurTaxAmount = TotalPurTaxAmount + parseFloat(response.lstDtl[dtl].PurTaxAmount);
                        TotalPurNetAmount = TotalPurNetAmount + parseFloat(response.lstDtl[dtl].PurNetAmount);

                        html += "<tr>";
                        html += "<td>" + sno + "</td>";
                        html += "<td>" + response.lstDtl[dtl].PH_ITEM_DRUGNAME_BRAND + "</td>";
                        html += "<td>" + response.lstDtl[dtl].PH_INDTL_DRUGBATCHNO + "</td>";
                        html += "<td>" + response.lstDtl[dtl].PH_INDTL_DRUGEXPIRY + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PH_INDTL_RECVDQTY + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PH_INDTL_BONUSQTY + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PurchaseCost + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].BillCost + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].GST + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PurAmount + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PurTaxAmount + "</td>";
                        html += "<td style='text-align: right;'>" + response.lstDtl[dtl].PurNetAmount + "</td>";
                        html += "</tr>";
                    }

                    html += "</tbody>";
                    html += "</table>";

                    html += "<table style='font-family:sans-serif;font-weight: 500;font-size: 12px;width:100%;'>";
                    html += "<tbody>";
                    html += "<tr>";
                    html += "<td style='width: 706px !important;'></td>";
                    html += "<td style='width: 90px !important;' >Total</td>";
                    html += "<td>:</td>";
                    html += "<td align='right' style='width:80px !important;'>" + TotalPurAmount.toFixed(3) + "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td></td>";
                    html += "<td style='width: 90px !important;' >GST</td>";
                    html += "<td>:</td>";
                    html += "<td align='right' style='width:80px !important;'>" + TotalPurTaxAmount.toFixed(3) + "</td>";
                    html += "</tr>";
                    html += "<tr style='font-size:16px !important;' >";
                    html += "<td></td>";
                    html += "<td style='width: 90px !important;' >Net Total</td>";
                    html += "<td>:</td>";
                    html += "<td align='right' style='width:80px !important;'>" + TotalPurNetAmount.toFixed(3) + "</td>";
                    html += "</tr>";
                    html += "</tbody>";
                    html += "</table>";

                    html += "<br/>";
                    html += "<table border='1' style='font-family:sans-serif;font-size:12px;font-weight:500;border-collapse:collapse;float:left;'>";
                    html += "<thead style='font-weight:bold;'>";
                    html += "<tr>";
                    html += "<td>S.No</td>";
                    html += "<td>GST %</td>";
                    html += "<td>Amount</td>";
                    html += "<td>Tax Amount</td>";
                    html += "<td>Total Amount</td>";
                    html += "</tr>";
                    html += "</thead>";
                    html += "<tbody>";
                    for (var sum = 0; sum < response.DtlSummary.length; sum++) {
                        var sno = sum + 1;
                        var TaxPre = response.DtlSummary[sum].GST;
                        var Amt = response.DtlSummary[sum].PurAmount;
                        var SummaryTax = response.DtlSummary[sum].PurTaxAmount;
                        var SummaryNetAmount = response.DtlSummary[sum].PurNetAmount;
                        html += "<tr>";
                        html += "<td>" + sno + "</td>";
                        html += "<td>" + TaxPre + "</td>";
                        html += "<td style='text-align: right;'>" + Amt + "</td>";
                        html += "<td style='text-align: right;'>" + SummaryTax + "</td>";
                        html += "<td style='text-align: right;'>" + SummaryNetAmount + "</td>";
                        html += "</tr>";
                    }

                    html += "</tbody>";
                    html += "</table>";
                }

                sessionStorage.setItem("PrintDetails", html);
                openRequestedPopup();
            },
            error: function (response) { console.log(response); }
        });
    }
    catch (e) { }
}
function openRequestedPopup() {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    var url = rootUrl + '/Pharmacy/Dispense/print';
    window.open(url, '_blank');
    sessionStorage.setItem("PrintDetails", "");
}
function GotoLogin() {
    try {
        window.location.href = "/Login/Login";
    }
    catch (e) {
    }
}
function TotalDiscountCalculation() {
    var CalculationType = $("#ddlDiscountType").val();
    var Amount = $("#txtSummaryNetAmt").val();
    var PrecentageValue = $("#txtRateValue").val();
    if (CalculationType === "PER") {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= 100) {
                    var PrecResult = percentage(PrecentageValue, Amount);
                    $("#txtDiscountAmt").val(PrecResult);
                    var NetAmount = (Amount - PrecResult);
                    $("#txtNetTotal").val(NetAmount);
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtSummaryNetAmt").val();
            if (SubTot.length > 0) {
                var NetTotal = parseFloat(SubTot);
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal);
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                var NetTotal = parseFloat(SubTot);
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal);
            }
        }
    }
    else {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= parseFloat(Amount)) {
                    var PrecResult = (Amount - PrecentageValue);
                    $("#txtDiscountAmt").val(PrecentageValue);
                    $("#txtNetTotal").val(PrecResult.toFixed(2));
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtSummaryNetAmt").val();
            if (SubTot.length > 0) {
                var NetTotal = parseFloat(SubTot);
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal);
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                $("#txtDiscountAmt").val(0);
                $("#txtNetTotal").val(NetTotal);
            }
        }
    }
}
function CleanAll() {
    $("#tblDrugBind tbody").empty();
    $("#tblSelectDrugBind tbody").empty();
    $("#tblInvoiceSummary tbody").empty();
    $('#ddlSuplier').val('');
    $('#txtInvoiceNo').val('');
    $('#txtSummaryAmt').val('');
    $('#txtSummaryTax').val('');
    $('#txtSummaryNetAmt').val('');
    $('#txtRateValue').val('');
    $('#txtDiscountAmt').val('');
    $('#txtNetTotal').val('');
}
function percentage(percent, total) {
    return ((percent / 100) * total).toFixed(2)
}
function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function DCtoInvoiceDiscount(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var DrugCode = parseInt(row.cells[11].innerHTML);
    var Discount = parseInt(row.cells[13].getElementsByTagName("input")[0].value);
    var Qty = parseFloat(row.cells[3].innerHTML);
    var Cost = parseFloat(row.cells[5].innerHTML);
    var Tax = parseFloat(row.cells[8].innerHTML);
    var amt = parseFloat(Cost * Qty).toFixed(2);
    var disAmt = parseFloat((amt * Discount) / 100).toFixed(2);
    amt = parseFloat(amt) - parseFloat(disAmt);
    var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
    var total = parseFloat(amt) + parseFloat(vatamt);
    
    row.cells[7].innerHTML = amt;
    row.cells[9].innerHTML = vatamt;
    row.cells[10].innerHTML = total.toFixed(2);
    var ObjectDetails = new Object();
    ObjectDetails.DrugCode = parseInt(DrugCode);
    ObjectDetails.Amount = parseFloat(amt);
    ObjectDetails.Tax = parseFloat(vatamt);
    ObjectDetails.TaxPrecentage = parseFloat(Tax);
    ObjectDetails.NetAmount = parseFloat(total);
    SummaryArray.push(ObjectDetails);

    $.ajax({
        url: "/Pharma/DcInvoice/SummaryCalculation",
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(SummaryArray),
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.Summary.length > 0) {
                html = "";
                $("#tblInvoiceSummary tbody").empty();
                for (PsCount = 0; PsCount < response.Summary.length; PsCount++) {
                    var TaxPre = response.Summary[PsCount].TaxPrecentage;
                    var Amt = response.Summary[PsCount].Amount;
                    var SummaryTax = response.Summary[PsCount].Tax;
                    var SummaryNetAmount = response.Summary[PsCount].NetAmount;
                    html += "<tr>";
                    html += "<td>" + TaxPre + "</td>";
                    html += "<td><input value='" + Amt + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "<td><input value='" + SummaryTax + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "<td><input value='" + SummaryNetAmount + "'style='height:28px;width:80px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberAndDecimal(this,event)'></td>";
                    html += "</tr>";
                }
                $("#tblInvoiceSummary tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    TotalCalculation();
}
function GetDCStateDeatilsForGst() {
    var supplierId = parseInt(sessionStorage.getItem("SupplierID"));

    $.ajax({
        url: "/Pharma/Invoice/GetStateDeatilsForGst?SupplierId=" + supplierId,
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response > 0) {
                document.getElementById("ddlDcGstType").value = "GST purchase";
                document.getElementById("ddlDcGstType").text = "GST purchase";
            }
            else {
                document.getElementById("ddlDcGstType").value = "IGST purchase";
                document.getElementById("ddlDcGstType").text = "IGST purchase";
                /*document.getElementById("ddlGstType").value("IGST purchase");*/

            }
        }
    });
}