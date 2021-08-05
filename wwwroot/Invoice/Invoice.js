$(document).ready(function () {
    GetInvoiceSupplierMaster();
    GetInvoiceStoreName();
    dateInvoiceFunction();

});
function SearchInvoiceDrug() {
    $("#txtBrandSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Pharma/Invoice/GetDrugFromDrugMaster",
                type: "GET",
                data: {
                    SearchTearm: request.term
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
            $("#txtBrandSearch").val(ui.item.label);
            $("#hidSeqID").val(ui.item.value)

        },
        select: function (event, ui) {
            // $("#txtDrugSearch").prop('disabled', true);
            $("#txtBrandSearch").val(ui.item.label);
            $("#txtDrugCode").val(ui.item.value);
            $("#txtBatch").focus();
            var DrugCode = parseInt(ui.item.value);

            GetInvoiceDrugByDrugCode(DrugCode);
            return false;
        },
        minLength: 0
    });
}
function GetInvoiceDrugByDrugCode(DrugCode) {
    $.ajax({
        url: "/Pharma/Invoice/GetDrugByDrugCode",
        type: "GET",
        data: {
            DrugCode: DrugCode
        },
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_GST;
                   
                    if (PH_ITEM_DRUG_VAT == "0.00") {
                        $('#modal-AddGst').modal('show');
                        var PH_ITEM_DRUGNAME_BRAND = response[PCHeader].PH_ITEM_DRUGNAME_BRAND;
                        var PH_ITEM_DrugCode = response[PCHeader].PH_ITEM_DrugCode;
                        var PH_ITEM_STRENGTH = response[PCHeader].PH_ITEM_STRENGTH;
                        var drugName = PH_ITEM_DRUGNAME_BRAND + " " + PH_ITEM_STRENGTH
                        $('#lblDrugName').html(drugName);
                        $('#txtPopupDrugCode').val(PH_ITEM_DrugCode);
                        return false;
                    }
                    var Uom = response[PCHeader].PH_ITEM_DRUG_UOM;
                    var PH_ITEM_DRUG_QTY = response[PCHeader].PH_ITEM_DRUG_QTY;
                    $("#txtTax").val(PH_ITEM_DRUG_VAT);
                    $("#lblUom").text("QTY / " + Uom);
                    $("#txtStripNs").val(PH_ITEM_DRUG_QTY);
                }
            }
        }
    });
}
function PackQtyChange() {
    var DrugCode = parseInt($("#hidSeqID").val());
    var pkqty = parseInt($("#txtStripNs").val());
    $.ajax({
        url: "/Pharma/Invoice/UpdatePackQtyINDrugMaster",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            PackQty: pkqty
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function GSTChange() {
    var DrugCode = parseInt($("#hidSeqID").val());
    var GST = parseFloat($("#txtTax").val());
    $.ajax({
        url: "/Pharma/Invoice/UpdateGSTDrugMasterByCode",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            GST: GST
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response = "Save Sucess") {
                StripRateChange();
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function StripQtyChange() {
    var pkqty = parseInt($("#txtStripNs").val());
    var stripQty = parseFloat($("#txtStripQty").val());
    var pkqty = parseInt($("#txtStripNs").val());
    var Qty = parseFloat(stripQty * pkqty);
    $("#txtTotalQty").val(Qty);
}
function StripRateChange() {
    var pkqty = parseInt($("#txtStripNs").val());
    var stripRate = parseFloat($("#txtstripRate").val());
    var stripQty = parseFloat($("#txtStripQty").val());
    var FreeQty = parseInt($("#txtFree").val());
    FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Tax = parseFloat($("#txtTax").val());
    var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);

    var amt = parseFloat(stripRate * stripQty).toFixed(2);
    var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
    var TotalValue = parseFloat(amt) + parseFloat(vatamt);
    var avg = parseFloat(stripRate / pkqty).toFixed(2);
    //$("#txtUnitRate").val(avg);
    $("#txtUnitRate").val(stripRate);
    $("#txtTotalAmt").val(TotalValue.toFixed(2));
    $("#txtTaxAmt").val(vatamt);
}
function FreeQtyChange() {
    var pkqty = parseInt($("#txtStripNs").val());
    var FreeQty = parseInt($("#txtFree").val());
    var stripQty = parseFloat($("#txtStripQty").val());
    FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);


    var TotalAmount = parseFloat($("#txtTotalAmt").val()).toFixed();
    //var UnitRate = parseFloat(TotalAmount / TotalQty).toFixed(2);

    //TotalAmount = parseFloat(UnitRate * TotalQty).toFixed(2);
    ////$("#txtStripQty").val(TotalQty);
    //$("#txtUnitRate").val(UnitRate);
    $("#txtTotalQty").val(TotalQty);
    return false;
}
function NewDiscountChange() {
    var pkqty = parseInt($("#txtStripNs").val());
    var FreeQty = parseInt($("#txtFree").val());
    var stripRate = parseFloat($("#txtstripRate").val());
    var stripQty = parseFloat($("#txtStripQty").val());
    var Tax = parseFloat($("#txtTax").val());
    FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Amount = parseFloat(stripRate * stripQty).toFixed(2);
    var Discount = parseFloat($("#txtDiscount").val()).toFixed(2);
    var disAmt = parseFloat((Amount * Discount) / 100).toFixed(2);
    var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);


    var TotalAmount = parseFloat($("#txtTotalAmt").val()).toFixed(2);
    TotalAmount = Amount - disAmt;
    var UnitRate = parseFloat(TotalAmount / TotalQty).toFixed(2);
    TotalAmount = parseFloat(UnitRate * TotalQty).toFixed(2);
    var vatamt = parseFloat((TotalAmount * Tax) / 100).toFixed(2);
    // $("#txtStripQty").val(TotalQty);
    var NetTotalAmt = parseFloat(TotalAmount) + parseFloat(vatamt);
    //$("#txtUnitRate").val(UnitRate);
    $("#txtUnitRate").val(stripRate);
    $("#txtTotalAmt").val(NetTotalAmt);
    $("#txtTaxAmt").val(vatamt);
    return false;
}
function MRPChange() {

    var stripRate = parseInt($("#txtstripRate").val());
    var MrpRate = parseInt($("#txtStripMRP").val());
    if (MrpRate < stripRate) {
        alert("MRP Should not lesser than Rate");
        $("#txtStripMRP").val('');
        return false;

    }
    var pkqty = parseInt($("#txtStripNs").val());
    var stripQty = parseFloat($("#txtStripQty").val());
    var avg = parseFloat(MrpRate / pkqty).toFixed(2);
    //$("#txtUnitMRP").val(avg);
    $("#txtUnitMRP").val(MrpRate);
}
function GetInvoiceSupplierMaster() {
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

            }
        }
    });
}

function GetStateDeatilsForGst() {
    var supplierId = parseInt($("#ddlSuplier").val());

    $.ajax({
        url: "/Pharma/Invoice/GetStateDeatilsForGst?SupplierId=" + supplierId,
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response > 0) {
                document.getElementById("ddlGstType").value = "GST purchase";
                document.getElementById("ddlGstType").text = "GST purchase";
            }
            else {
                document.getElementById("ddlGstType").value = "IGST purchase";
                document.getElementById("ddlGstType").text = "IGST purchase";
                /*document.getElementById("ddlGstType").value("IGST purchase");*/

            }
        }
    });
}

function GetInvoiceStoreName() {
    $.ajax({
        url: "/Pharma/Invoice/GetStoreName",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var HIS_PH_STOREMASTER = response[PCHeader].HIS_PH_STOREMASTER;
                    var HIS_PH_STORENAME = response[PCHeader].HIS_PH_STORENAME;
                    $('#ddlWherHouse')
                        .append($("<option></option>").val(HIS_PH_STOREMASTER).html(HIS_PH_STORENAME));
                }
            }
        }
    });
}
function OnDirectInvoiceAdd() {
    if (InvoiceValidation()) {
        $("#txtInvoiceNo").prop("disabled", true);
        $("#ddlSuplier").prop("disabled", true);
        $("#txtInvoiceDate").prop("disabled", true);
        $("#ddlWherHouse").prop("disabled", true);
        var SeqID = 0;
        if ($("#hidSeqID").val() === '')
            SeqID = 0;
        else
            SeqID = parseInt($("#hidSeqID").val());
        var Discount = 0;
        if ($("#txtDiscount").val() === '')
            Discount = 0;
        else
            Discount = parseFloat($("#txtDiscount").val());
        var FreeQty = 0;
        if ($("#txtDiscount").val() === '') {
            FreeQty = 0
        }
        else
            FreeQty = parseFloat($("#txtFree").val())
        var sendJsonData = {
            SupplierInvoiceNumber: $("#txtInvoiceNo").val(),
            SupplierID: parseInt($("#ddlSuplier").val()),
            SupplierInvoiceDate: $("#txtInvoiceDate").val(),
            SupplierName: $("#ddlSuplier option:selected").text(),
            InVoiceTaxType: $("#ddlGstType option:selected").text(),
            InvoiceType: $("#ddlInvoiceType option:selected").text(),
            DrugName: $("#txtBrandSearch").val(),
            DrugCode: parseInt($("#txtDrugCode").val()),
            Batch: $("#txtBatch").val(),
            ExpiryDate: $("#txtExpiryDt").val(),
            Rate: parseFloat($("#txtstripRate").val()),
            MRP: parseFloat($("#txtStripMRP").val()),
            //Qty: parseFloat($("#txtTotalQty").val()),
            Qty: parseFloat($("#txtStripQty").val()),
            FreeQty: FreeQty,
            WareHouse: $("#ddlWherHouse option:selected").text(),
            Discount: Discount,
            WareHouse: $("#ddlWherHouse option:selected").text(),
            SeqID: SeqID,
            TaxAmount: parseFloat($("#txtTaxAmt").val()),
            NetAmount: parseFloat($("#txtTotalAmt").val()),
            Tax: parseFloat($("#txtTax").val())
        };
        $.ajax({
            url: "/Pharma/Invoice/DraftSave",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(sendJsonData),
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response.Header.length > 0) {
                    for (PCHeader = 0; PCHeader < response.Header.length; PCHeader++) {
                        $("#ddlSuplier").val(response.Header[PCHeader].SupplierID);
                        $("#ddlWherHouse option:selected").text(response.Header[PCHeader].HIS_PH_STORENAME);
                        $("#ddlWherHouse").val(response.Header[PCHeader].HIS_PH_STOREMASTER);
                        $("#txtInvoiceNo").val(response.Header[PCHeader].SupplierInvoiceNumber);
                        $("#txtInvoiceDate").val(response.Header[PCHeader].SupplierInvoiceDate);
                        $("#ddlGstType").val(response.Header[PCHeader].InVoiceTaxType);
                        $("#ddlInvoiceType").val(response.Header[PCHeader].InvoiceType);
                        if (response.Deatils.length > 0) {
                            var html = "";
                            var Sno = 0;
                            //var table = document.getElementById("tblDrugBind");
                            //var tbodyRowCount = table.tBodies[0].rows.length;
                            $("#tblDrugBind tbody").empty();
                            for (PCDetails = 0; PCDetails < response.Deatils.length; PCDetails++) {
                                Sno = PCDetails + 1;
                                var DtlSeqID = response.Deatils[PCDetails].DtlSeqID;
                                var DrugName = response.Deatils[PCDetails].DrugName;
                                var DrugCode = response.Deatils[PCDetails].DrugCode;
                                var Batch = response.Deatils[PCDetails].Batch;
                                var ExpiryDate = response.Deatils[PCDetails].ExpiryDate;
                                var Rate = response.Deatils[PCDetails].Rate;
                                var MRP = response.Deatils[PCDetails].MRP;
                                var Qty = response.Deatils[PCDetails].Qty;
                                var FreeQty = response.Deatils[PCDetails].FreeQty;
                                var Amount = response.Deatils[PCDetails].Amount;
                                var Tax = response.Deatils[PCDetails].TaxPrecentage;
                                var NetAmount = response.Deatils[PCDetails].NetAmount;
                                var TaxAmount = response.Deatils[PCDetails].Tax;
                                var Discount = response.Deatils[PCDetails].Discount;
                                var PH_ITEM_DRUG_QTY = response.Deatils[PCDetails].PH_ITEM_DRUG_QTY;

                                html += "<tr data-id=\"" + DrugCode + "\"><td>" + Sno + "</td>";//0
                                html += "<td style='display:none;'>" + DrugCode + "</td>";//1
                                html += "<td style='display:none;'>" + DtlSeqID + "</td>";//2
                                html += "<td>" + DrugName + "</td>";//3
                                html += "<td>" + Batch + "</td>";//4
                                html += "<td>" + ExpiryDate + "</td>";//5
                                html += "<td>" + Qty + "</td>";//6
                                html += "<td>" + FreeQty + "</td>";//7
                                html += "<td><input value='" + Rate + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//8
                                html += "<td><input value='" + MRP + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//9
                                html += "<td><input value='" + Amount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//10
                                html += "<td>" + Tax + "</td>";//11
                                html += "<td><input value='" + TaxAmount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//12
                                html += "<td>" + Discount + "</td>";//13
                                html += "<td><input value='" + NetAmount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'/></td>";//14
                                html += "<td style='text-align: center;'>";
                                html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteSaveInvoiceRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";//15
                                html += "<td style='display:none;'>" + PH_ITEM_DRUG_QTY + "</td>";//16
                                html += "</tr>";
                            }
                            $("#tblDrugBind tbody").append(html);
                            document.getElementById("InvoiceDiv").style.display = "block";
                            document.getElementById("DivInvoiceTotal").style.display = "block";
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
                                if (response.Total.length > 0) {
                                    for (PtCount = 0; PtCount < response.Total.length; PtCount++) {
                                        var Amt = response.Total[PtCount].Amount;
                                        var SummaryTax = response.Total[PtCount].Tax;
                                        var SummaryNetAmount = response.Total[PtCount].NetAmount;
                                        $("#txtSummaryAmt").val(Amt);
                                        $("#txtSummaryTax").val(SummaryTax);
                                        $("#txtSummaryNetAmt").val(SummaryNetAmount);
                                        $("#txtNetTotal").val(SummaryNetAmount);
                                        $("#txtRateValue").val("0");
                                        $("#txtDiscountAmt").val("0");
                                    }
                                }
                            }
                        }
                        DraftClearAll();
                    }
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
}
function InvoiceValidation() {
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
    var DrugName = $("#txtBrandSearch").val();
    if (DrugName === '') {
        $("#txtBrandSearch").focus();
        txt += "\n - Please Enter Drug Name";
        var opt = 1;
    }
    var Batch = $("#txtBatch").val();
    if (Batch === '') {
        $("#txtBatch").focus();
        txt += "\n - Please Enter BatchNo";
        var opt = 1;
    }
    var ExpiryDt = $("#txtExpiryDt").val();
    if (ExpiryDt === '') {
        txt += "\n - Please Enter Expiry Date";
        var opt = 1;

    }
    else {
        var DateSplit = ExpiryDt.split('/');
        var day = parseInt(DateSplit[0]);
        var month = parseInt(DateSplit[1]);
        var year = parseInt(DateSplit[2]);
        var today = new Date();
        var consoleddate = month + '/' + day + '/' + year + ' 23:59:59';
        var exdate = new Date(consoleddate);
        var day2 = exdate.getDate();
        var month2 = exdate.getMonth() + 1;
        var year2 = exdate.getFullYear();
        if (day != day2 || month != month2 || year !== year2) {
            txt += "\n - Please Enter Valid Expiry Date";
            opt = 1;
        }
        else if (exdate < today) {
            txt += "\n - Please Enter Expiry Date Greater Than Today's Date.";
            opt = 1;
        }

    }
    var Rate = $("#txtUnitRate").val();
    if (Rate === '' || Rate === '0.00') {
        $("#txtUnitRate").focus();
        $("#txtUnitRate").val('0.00');
        txt += "\n - Please Enter Purchase Rate";
        var opt = 1;

    }
    var MRP = $("#txtStripMRP").val();
    if (MRP === '' || MRP === '0.00') {
        $("#txtStripMRP").focus();
        $("#txtStripMRP").val('0.00');
        txt += "\n - Please Enter MRP";
        var opt = 1;
    }
    var QTY = $("#txtStripQty").val();
    if (QTY === '' || QTY === '0.00') {
        $("#txtStripQty").focus();
        $("#txtStripQty").val('0.00');
        txt += "\n - Please Enter Qty ";
        var opt = 1;
    }
    if (opt == "1") {
        alert(txt);
        $("#txtBrandSearch").focus();
        return false;
    }
    return true;
}
function dateInvoiceFunction() {
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
function OpenInvoiceDraft() {
    GetTop50DraftBill();
    $("#ModelInvoiceDraft").dialog(
        {
            title: "Draft",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelInvoiceDraft").dialog("close");
                }
            }
        });
}
function GetTop50DraftBill() {
    $.ajax({
        url: "/Pharma/Invoice/GetTop50DraftBill",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblInvoiceDraft tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var SeqID = response[PCHeader].SeqID;
                    var SupplierInvoiceNumber = response[PCHeader].SupplierInvoiceNumber;
                    var SupplierInvoiceDate = response[PCHeader].SupplierInvoiceDate;
                    var InVoiceTaxType = response[PCHeader].InVoiceTaxType;
                    var InvoiceType = response[PCHeader].InvoiceType;
                    var WareHouse = response[PCHeader].WareHouse;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;

                    html += "<tr onclick='SelectDraftInvoiceRow(this)'><td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + SeqID + "</td>";
                    html += "<td>" + SupplierInvoiceNumber + "</td>";
                    html += "<td>" + PH_SupplierName + "</td>";
                    html += "<td>" + SupplierInvoiceDate + "</td>";
                    html += "<td>" + WareHouse + "</td>";
                    html += "</tr>";
                }

                $("#tblInvoiceDraft tbody").append(html);
            }
        }
    });
}
function SelectDraftInvoiceRow(selectedrow) {
    var DraftSeqID = parseFloat(selectedrow.cells[1].innerHTML);
    $('#txtDraftHeaderId').val(selectedrow.cells[1].innerHTML);
    $.ajax({
        url: "/Pharma/Invoice/GetSelectedDraftBySeqID",
        type: "GET",
        data: {
            SeqID: DraftSeqID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.Header.length > 0) {
                $("#txtInvoiceNo").prop("disabled", true);
                $("#ddlSuplier").prop("disabled", true);
                $("#txtInvoiceDate").prop("disabled", true);
                $("#ddlWherHouse").prop("disabled", true);
                for (PCHeader = 0; PCHeader < response.Header.length; PCHeader++) {
                    $("#ddlSuplier").val(response.Header[PCHeader].SupplierID);
                    var Warehouseid = parseInt(response.Header[PCHeader].HIS_PH_STOREMASTER);
                    var WarehouseName = response.Header[PCHeader].HIS_PH_STORENAME;
                    $("#ddlWherHouse").val(Warehouseid);
                    $("#ddlWherHouse option:selected").text(HIS_PH_STORENAME);
                    GetPurchaseExpiryDays();
                    //$("#ddlWherHouse").text(WarehouseName);
                    //document.getElementById("ddlWherHouse").text = WarehouseName;
                    $("#txtInvoiceNo").val(response.Header[PCHeader].SupplierInvoiceNumber);
                    $("#txtInvoiceDate").val(response.Header[PCHeader].SupplierInvoiceDate);
                    $("#ddlGstType").val(response.Header[PCHeader].InVoiceTaxType);
                    $("#ddlInvoiceType").val(response.Header[PCHeader].InvoiceType);
                    if (response.Deatils.length > 0) {
                        var html = "";
                        var Sno = 0;
                        var table = document.getElementById("tblDCDrugBind");
                        var tbodyRowCount = table.tBodies[0].rows.length;
                        $("#tblDrugBind tbody").empty();
                        for (PCDetails = 0; PCDetails < response.Deatils.length; PCDetails++) {
                            Sno = PCDetails + 1;
                            //Sno = tbodyRowCount + 1;
                            var DtlSeqID = response.Deatils[PCDetails].DtlSeqID;
                            var DrugName = response.Deatils[PCDetails].DrugName;
                            var DrugCode = response.Deatils[PCDetails].DrugCode;
                            var Batch = response.Deatils[PCDetails].Batch;
                            var ExpiryDate = response.Deatils[PCDetails].ExpiryDate;
                            var Rate = response.Deatils[PCDetails].Rate;
                            var MRP = response.Deatils[PCDetails].MRP;
                            var Qty = response.Deatils[PCDetails].Qty;
                            var FreeQty = response.Deatils[PCDetails].FreeQty;
                            var Amount = response.Deatils[PCDetails].Amount;
                            var Tax = response.Deatils[PCDetails].TaxPrecentage;
                            var NetAmount = response.Deatils[PCDetails].NetAmount;
                            var TaxAmount = response.Deatils[PCDetails].Tax;
                            var Discount = response.Deatils[PCDetails].Discount;
                            var PH_ITEM_DRUG_QTY = response.Deatils[PCDetails].PH_ITEM_DRUG_QTY;

                            html += "<tr data-id=\"" + DrugCode + "\"><td>" + Sno + "</td>";//0
                            html += "<td style='display:none;'>" + DrugCode + "</td>";//1
                            html += "<td style='display:none;'>" + DtlSeqID + "</td>";//2
                            html += "<td>" + DrugName + "</td>";//3
                            html += "<td>" + Batch + "</td>";//4
                            html += "<td>" + ExpiryDate + "</td>";//5
                            html += "<td>" + Qty + "</td>";//6
                            html += "<td>" + FreeQty + "</td>";//7
                            html += "<td><input value='" + Rate + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//8
                            html += "<td><input value='" + MRP + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//9
                            html += "<td><input value='" + Amount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//10
                            html += "<td>" + Tax + "</td>";//11
                            html += "<td><input value='" + TaxAmount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//12
                            html += "<td>" + Discount + "</td>";//13
                            html += "<td><input value='" + NetAmount + "'style='height:28px;width:80px;text-align: right;' type=\"Text\" onchange='QtyValu(this)' onkeypress='return isNumberKey(event)'/></td>";//14
                            html += "<td style='text-align: center;'>";
                            html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteSaveInvoiceRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";//15
                            html += "<td style='display:none;'>" + PH_ITEM_DRUG_QTY + "</td>";//16
                            html += "</tr>";
                        }
                        $("#tblDrugBind tbody").append(html);
                        document.getElementById("InvoiceDiv").style.display = "block";
                        document.getElementById("DivInvoiceTotal").style.display = "block";
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
                                html += "<td><input value='" + Amt + "'style='height:28px;width:60px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberKey(event)'></td>";
                                html += "<td><input value='" + SummaryTax + "'style='height:28px;width:60px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberKey(event)'></td>";
                                html += "<td><input value='" + SummaryNetAmount + "'style='height:28px;width:60px;text-align: right;margin-top:5px;' type=\"Text\" onkeypress='return isNumberKey(event)'></td>";
                                html += "</tr>";
                            }
                            $("#tblInvoiceSummary tbody").append(html);
                            if (response.Total.length > 0) {
                                html = "";
                                $("#tblTotalValue tbody").empty();
                                for (PtCount = 0; PtCount < response.Total.length; PtCount++) {
                                    var Amt = response.Total[PtCount].Amount;
                                    var SummaryTax = response.Total[PtCount].Tax;
                                    var SummaryNetAmount = response.Total[PtCount].NetAmount;
                                    $("#txtSummaryAmt").val(Amt);
                                    $("#txtSummaryTax").val(SummaryTax);
                                    $("#txtSummaryNetAmt").val(SummaryNetAmount);
                                    $("#txtNetTotal").val(SummaryNetAmount);
                                    $("#txtRateValue").val("0");
                                    $("#txtDiscountAmt").val("0");
                                }
                                $("#tblTotalValue tbody").append(html);
                            }
                        }
                    }
                }
                $("#ModelInvoiceDraft").dialog("close");
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    $("#ModelBillHold").dialog("close");
    return false;
}
function QtyValu(SelectedRow) {
    try {
        var row = SelectedRow.parentNode.parentNode;
        var rowIndex = SelectedRow.rowIndex;
        var Qty = row.cells[6].innerHTML;
        if (Qty !== "") {
            var Rate = parseFloat(row.cells[8].getElementsByTagName("input")[0].value).toFixed(2);
            var Cost = 0;
            if (Rate > 0) {
                Cost = Rate;
            }
            var PackQty = parseFloat(row.cells[16].innerHTML).toFixed(2);
            var TotalQty = parseFloat(Qty) * parseFloat(PackQty);
            var Tax = parseFloat(row.cells[11].innerHTML).toFixed(2);
            var amt = parseFloat(Cost * Qty).toFixed(2);
            var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
            var total = parseFloat(amt) + parseFloat(vatamt);
            total = total.toFixed(2);
            row.cells[10].getElementsByTagName("input")[0].value = amt;
            row.cells[12].getElementsByTagName("input")[0].value = vatamt;
            row.cells[14].getElementsByTagName("input")[0].value = total;

            //$("#txtPurchaseBrandSearch").focus();
            //var DrugCode = parseInt(row.cells[0].innerHTML);
            //var pkqty = parseInt(row.cells[7].getElementsByTagName("input")[0].value);

        }
    }
    catch (e) { sconsole.log(e); }
}

function DeleteSaveInvoiceRow(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var myrow = selectedrow.parentNode.parentNode;
    var rowIndex = row.rowIndex;
    var dtlSeqID = parseFloat(row.cells[2].innerHTML);
    document.getElementById("tblDrugBind").deleteRow(myrow.rowIndex);
    var table = document.getElementById("tblDrugBind");
    var rowCount = table.rows.length;
    var i = myrow.rowIndex;
    regroup(i, rowCount, "tblDrugBind");
    DeleteDrug(dtlSeqID);
}
function regroup(i, rc, ti) {
    for (j = (i + 1); j < rc; j++) {
        if (j > 0) {
            document.getElementById(ti).rows[j].cells[0].innerHTML = j;
        }
    }
}
function DeleteDrug(DrugSeqID) {
    $.ajax({
        url: "/Pharma/Invoice/DeleteDraftDtlBySeqID",
        type: "GET",
        data: {
            SeqID: DrugSeqID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
function DraftClearAll() {
    $("#txtBrandSearch").val('');
    $("#txtDrugCode").val('');
    $("#txtBatch").val('');
    $("#txtExpiryDt").val('');
    $("#txtUnitRate").val('');
    $("#txtUnitMRP").val('');
    $("#txtTotalQty").val('');
    $("#txtStripQty").val('');
    $("#txtstripRate").val('');
    $("#txtFree").val('');
    $("#txtDiscount").val('');
    $("#txtTaxAmt").val('');
    $("#txtTotalAmt").val('');
    $("#txtStripMRP").val('');
    $("#txtStripNs").val(0);
    $("#txtTax").val('');
    return false;
}
function InvoiceSave() {
    if (InvoiceSaveValidation()) {
        var DrugInfo = new Array();
        var tblDrugSales = document.getElementById("tblDrugBind");
        var draftHeaderId = $("#txtDraftHeaderId").val();
        if (draftHeaderId == "") {
            draftHeaderId = 0;
        }

        var rowtblDrugSales = tblDrugSales.rows.length;
        for (M = 1; M < rowtblDrugSales; M++) {
            var rowDrug = tblDrugSales.rows[M];
            var ObjectDetails = new Object();
            var Packqty = parseInt(rowDrug.cells[16].innerHTML);
            ObjectDetails.DrugCode = parseFloat(rowDrug.cells[1].innerHTML);
            ObjectDetails.DrugName = rowDrug.cells[3].innerHTML;
            ObjectDetails.Batch = rowDrug.cells[4].innerHTML;
            ObjectDetails.ExpiryDate = rowDrug.cells[5].innerHTML;
            var stripqty = parseInt(rowDrug.cells[6].innerHTML);
            var DrugQty = stripqty * Packqty;
            ObjectDetails.Qty = parseInt(DrugQty);

            var Freestripqty = parseInt(rowDrug.cells[7].innerHTML);
            var FreeDrugQty = Freestripqty * Packqty;
            ObjectDetails.FreeQty = parseInt(FreeDrugQty);
            //ObjectDetails.FreeQty = parseFloat(rowDrug.cells[7].innerHTML);

            var StripRate = parseFloat(rowDrug.cells[8].getElementsByTagName("input")[0].value);
            var DrugRate = parseFloat(StripRate / Packqty).toFixed(2);
            ObjectDetails.Rate = parseFloat(DrugRate);

            var StripMRP = parseFloat(rowDrug.cells[9].getElementsByTagName("input")[0].value);
            var DrugMRP = parseFloat(StripMRP / Packqty).toFixed(2);
            ObjectDetails.MRP = parseFloat(DrugMRP);
            //ObjectDetails.MRP = parseFloat(rowDrug.cells[9].getElementsByTagName("input")[0].value);

            ObjectDetails.Amount = parseFloat(rowDrug.cells[10].getElementsByTagName("input")[0].value);
            ObjectDetails.TaxPrecentage = parseFloat(rowDrug.cells[11].innerHTML);
            ObjectDetails.Tax = parseFloat(rowDrug.cells[12].getElementsByTagName("input")[0].value);
            ObjectDetails.Discount = parseFloat(rowDrug.cells[13].innerHTML);
            ObjectDetails.NetAmount = parseFloat(rowDrug.cells[14].getElementsByTagName("input")[0].value);
            ObjectDetails.DtlSeqID = 0;
            ObjectDetails.Status = "";
            ObjectDetails.OldBatch = "";
            ObjectDetails.OldStock = 0;
            DrugInfo.push(ObjectDetails);
        }
        var sendJsonData = {
            SupplierInvoiceNumber: $("#txtInvoiceNo").val(),
            SupplierID: parseInt($("#ddlSuplier").val()),
            SupplierInvoiceDate: $("#txtInvoiceDate").val(),
            SupplierName: $("#ddlSuplier option:selected").text(),
            InVoiceTaxType: $("#ddlGstType option:selected").text(),
            InvoiceType: $("#ddlInvoiceType option:selected").text(),
            WareHouse: $("#ddlWherHouse option:selected").text(),
            Amount: parseFloat($("#txtSummaryAmt").val()),
            Tax: parseFloat($("#txtSummaryTax").val()),
            NetAmount: parseFloat($("#txtNetTotal").val()),
            Discount: parseFloat($("#txtDiscountAmt").val()),
            DisType: $("#ddlDiscountType").val(),
            DiscountValue: parseFloat($("#txtRateValue").val()),
            DraftHeaderId: parseFloat(draftHeaderId),
            DraftDeatils: DrugInfo
        };
        console.log(sendJsonData);
        $.ajax({
            url: "/Pharma/Invoice/InvoiceSave",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(sendJsonData),
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                if (response === "Supplier Invoice Number Exists") {
                    $("#txtInvoiceNo").prop("disabled", false);
                    $("#ddlSuplier").prop("disabled", false);
                    $("#txtInvoiceDate").prop("disabled", false);
                    $("#ddlWherHouse").prop("disabled", false);
                    alert(response);
                }
                else {
                    $("#ddlSuplier").val(0);
                    $("#ddlWherHouse").val(0);
                    $("#txtInvoiceNo").val('');
                    $("#txtInvoiceDate").val('');
                    $("#ddlGstType").val("GST purchase");
                    $("#ddlInvoiceType").val("Select");
                    $("#tblDrugBind tbody").empty();
                    $("#tblInvoiceSummary tbody").empty();
                    $("#txtSummaryAmt").val('');
                    $("#txtSummaryTax").val('');
                    $("#txtSummaryNetAmt").val('');
                    $("#txtNetTotal").val('');
                    $("#txtDiscountAmt").val('');
                    $("#txtRateValue").val('');
                    $("#txtDraftHeaderId").val('');
                    $("#txtInvoiceNo").prop("disabled", false);
                    $("#ddlSuplier").prop("disabled", false);
                    $("#txtInvoiceDate").prop("disabled", false);
                    $("#ddlWherHouse").prop("disabled", false);
                    alert(response);
                }
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }

}
function InvoiceSaveValidation() {
    try {
        var txt = "Required to fill the following field(s)";
        var opt = 0;
        var Supplire = parseFloat($("#ddlSuplier").val());
        if (Supplire === 0) {
            txt += "\n - Please Select Supplier Name";
            opt = 1;
        }

        var SupplierInvoiceNumber = $("#txtInvoiceNo").val();
        if (SupplierInvoiceNumber === '') {
            txt += "\n - Please Enter Invoice Number";
            opt = 1;
        }

        var InvoiceDt = $("#txtInvoiceDate").val();
        if (InvoiceDt === '') {
            txt += "\n - Please Enter Invoice Date";
            opt = 1;
        }

        var Warehouse = $("#ddlWherHouse").val();
        if (Warehouse == 0 || Warehouse == "0") {
            txt += "\n - Please Select Warehouse";
            opt = 1;
        }
        if (opt > 0) {
            alert(txt);
            return false;
        }

        return true;
    } catch (e) {
        console.log(e);
    }
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 32 && (charCode < 48 || charCode > 57) || (charCode == 32))

        return false;

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
function percentage(percent, total) {
    return ((percent / 100) * total).toFixed(2)
}
function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function TotalInvoiceDiscountCalculation() {
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
function InvoiceClearAll() {
    DraftClearAll();
    $("#ddlSuplier").val(0);
    $("#ddlWherHouse").val(0);
    $("#txtInvoiceNo").val('');
    $("#txtInvoiceDate").val('');
    $("#ddlGstType").val("GST purchase");
    $("#ddlInvoiceType").val("Select");
    $("#tblDrugBind tbody").empty();
    $("#tblInvoiceSummary tbody").empty();
    $("#txtSummaryAmt").val('');
    $("#txtSummaryTax").val('');
    $("#txtSummaryNetAmt").val('');
    $("#txtSummaryNetAmt").val('');
    $("#txtNetTotal").val('');
    $("#txtDiscountAmt").val('');
    $("#txtRateValue").val('');
    $("#txtExpiryDt").prop("disabled", true);
    $("#txtInvoiceNo").prop("disabled", false);
    $("#ddlSuplier").prop("disabled", false);
    $("#txtInvoiceDate").prop("disabled", false);
    $("#ddlWherHouse").prop("disabled", false);
}
function OpenInvoice() {
    GetTop50Invoice();
    $("#ModelInvoice").dialog(
        {
            title: "Draft",
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

//#region Habib
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


function OpenInvoiceReturn() {
    try {

        $("#ModelInvoiceReturn").dialog(
            {
                title: "Invoice Return",
                width: 1103,
                height: 500,
                modal: true,
                buttons: {
                    "Cancel": function () {
                        $("#ModelInvoiceReturn").dialog("close");
                    }
                }
            }
        );

        GetInvoiceListBySuppId();

    }
    catch (e) { console.log(e); }
}

function GetInvoiceListBySuppId() {
    try {
        var SupplierId = $("#ddlSuplier").val();
        if (SupplierId != "0" && SupplierId != 0) {
            $.ajax({
                url: "/Pharma/Invoice/InvoiceBySupplierId",
                type: "GET",
                data: {
                    SeqID: SupplierId
                },
                dataType: "json",
                beforeSend: function () { $("#loading").css("display", "block"); },
                success: function (response) {
                    var html = "";
                    $("#tblInvoiceReturn tbody").empty();
                    if (response.length > 0) {
                        for (var i = 0; i < response.length; i++) {
                            var sno = i + 1;
                            var PH_IN_SEQID = response[i].PH_IN_SEQID;
                            var InvoiceNo = response[i].PH_IN_DOCNO;
                            var SuppName = response[i].PH_SupplierName;
                            var EntryDate = response[i].PH_IN_ENTRYDATE;
                            var WareHouse = response[i].PH_INWareHouse;

                            html += "<tr onclick='OnInvoiceReturnRowClick(this);'>";
                            html += "<td style='display:none;'>" + PH_IN_SEQID + "</td>";
                            html += "<td>" + sno + "</td>";
                            html += "<td>" + InvoiceNo + "</td>";
                            html += "<td>" + EntryDate + "</td>";
                            html += "<td>" + WareHouse + "</td>";
                            html += "</tr>";
                        }

                        $("#tblInvoiceReturn tbody").append(html);
                    }
                },
                complete: function () { $("#loading").css("display", "none"); }
            });
        }

    } catch (e) { console.log(e) }
}
function OnInvoiceReturnRowClick(SelectedRow) {
    try {
        var SeqId = SelectedRow.cells[0].innerHTML;
        $.ajax({
            url: "/Pharma/Invoice/GetInvoiceDetailsById",
            type: "GET",
            data: {
                SeqID: SeqId
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var html = "";
                if (response.lstHdr.length > 0) {
                    for (var hdr = 0; hdr < response.lstHdr.length; hdr++) {
                        $("#ddlSuplier").val(response.lstHdr[hdr].PH_IN_SUPID);
                        $("#ddlWherHouse option:selected").text(response.lstHdr[hdr].PH_INWareHouse);
                        $("#txtInvoiceNo").val(response.lstHdr[hdr].PH_IN_DOCNO);
                        $("#txtInvoiceDate").val(response.lstHdr[hdr].PH_IN_ENTRYDATE);
                        $("#ddlGstType").val(response.lstHdr[hdr].InvoiceGSTType);
                        $("#ddlInvoiceType").val(response.lstHdr[hdr].InvoiceType);
                        $("#ddlWherHouse").val(response.lstHdr[hdr].HIS_PH_STOREMASTER);
                        GetPurchaseExpiryDays();
                    }
                    if (response.lstDtl.length > 0) {
                        $("#tblReturnBind tbody").empty();
                        $("#divReturnBind").prop('hidden', false);
                        html = "";
                        for (var dtl = 0; dtl < response.lstDtl.length; dtl++) {
                            var SNO = dtl + 1;
                            var DtlSeqId = response.lstDtl[dtl].PH_INDTL_SEQID;
                            var DrugName = response.lstDtl[dtl].PH_ITEM_DRUGNAME_BRAND;
                            var DrugCode = response.lstDtl[dtl].PH_INDTL_DRUGCODE;
                            var BatchNo = response.lstDtl[dtl].PH_INDTL_DRUGBATCHNO;
                            var DrugExpiry = response.lstDtl[dtl].PH_INDTL_DRUGEXPIRY;
                            var Recqty = response.lstDtl[dtl].PH_INDTL_RECVDQTY;
                            var BonusQty = response.lstDtl[dtl].PH_INDTL_BONUSQTY;
                            var TotalQty = parseInt(Recqty) + parseInt(BonusQty);
                            var PurchaseCost = response.lstDtl[dtl].PurchaseCost;
                            var BillCost = response.lstDtl[dtl].BillCost;
                            var GST = response.lstDtl[dtl].GST;
                            var PurAmount = response.lstDtl[dtl].PurAmount;
                            var PurNetAmount = response.lstDtl[dtl].PurNetAmount;
                            var PurTaxAmount = response.lstDtl[dtl].PurTaxAmount;

                            var returnType = "<select><option value='Goods Return'>Goods Return</option>" +
                                "<option value='Expiry Return'>Expiry Return</option></select> ";
                            var ReturnQty = "<input type='textbox' style='height:28px;width:50px;text-align: right;' onkeypress='return isNumberKey(event)' onchange='PurchaseReturn(this)' value='" + TotalQty + "' />"
                            var Comments = "<input type='textbox' style='height:28px;width:50px; />";
                            var ItemCatName = response.lstDtl[dtl].PH_ItemCatName;

                            html += "<tr>";
                            html += "<td style='display:none;'>" + DtlSeqId + "</td>"; //0
                            html += "<td style='display:none;'>" + DrugCode + "</td>"; //1 
                            html += "<td>" + SNO + "</td>"; //2
                            html += "<td>" + DrugName + "</td>"; //3
                            html += "<td>" + BatchNo + "</td>"; //4
                            html += "<td>" + DrugExpiry + "</td>"; //5
                            html += "<td>" + Recqty + "</td>"; //6
                            html += "<td>" + BonusQty + "</td>"; //7
                            html += "<td>" + PurchaseCost + "</td>"; //8
                            html += "<td>" + BillCost + "</td>"; //9
                            html += "<td>" + PurAmount + "</td>"; //10
                            html += "<td>" + PurTaxAmount + "</td>"; //11
                            html += "<td>" + GST + "</td>"; //12
                            html += "<td>" + PurNetAmount + "</td>"; //13
                            html += "<td>" + ReturnQty + "</td>"; //14
                            html += "<td style='display:none;'>" + ItemCatName + "</td>"; //15
                            html += "<td style='display:none;'>false</td>"; //16
                            //html += "<td>" + Comments + "</td>"; //10
                            html += "</tr>";

                        }
                        $("#tblReturnBind tbody").append(html);
                    }
                }
                $("#ModelInvoiceReturn").dialog("close");
            },
            complete: function () { $("#loading").css("display", "none"); }
        });

    } catch (e) { }
}
function PurchaseReturn(SelectedRow) {
    // var DrugName = $("#txtSearchDrugType").val();
    var row = SelectedRow.parentNode.parentNode;
    var rowIndex = SelectedRow.rowIndex;
    var RetQty = parseInt(row.cells[6].innerHTML);
    var CurrentQty = parseInt(row.cells[14].getElementsByTagName("input")[0].value);
    if (CurrentQty <= RetQty) {
        var Cost = parseFloat(row.cells[8].innerHTML).toFixed(2);
        var Tax = parseFloat(row.cells[12].innerHTML).toFixed(2);
        var amt = parseFloat(Cost * CurrentQty).toFixed(2);
        var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        var total = parseFloat(amt) + parseFloat(vatamt);
        total = total.toFixed(2);
        row.cells[10].innerHTML = amt;
        row.cells[11].innerHTML = vatamt;
        row.cells[13].innerHTML = total;
        if (CurrentQty === RetQty)
            row.cells[16].innerHTML = false;
        else
            row.cells[16].innerHTML = true;
    }
    else {
        alert('Please Enter value less than Purchase Qty');
    }
}
function SaveReturnInvoice() {
    try {
        var ReturnInvoice = new Array();
        var tblReturnBind = document.getElementById("tblReturnBind");
        var rowtblReturnBind = tblReturnBind.rows.length;
        for (M = 1; M < rowtblReturnBind; M++) {
            var rowReturn = tblReturnBind.rows[M];
            var ObjectDetails = new Object();
            var Qty = parseFloat(rowReturn.cells[6].innerHTML);
            var FreeQty = parseFloat(rowReturn.cells[7].innerHTML);
            var Ph_Ret_Qty = parseFloat(rowReturn.cells[14].getElementsByTagName("input")[0].value);
            var Ph_Ret_ReminQty = (Qty + FreeQty) - Ph_Ret_Qty;
            ObjectDetails.Ph_Ret_DtlSeqID = parseFloat(rowReturn.cells[0].innerHTML);
            ObjectDetails.Ph_Ret_DrugCode = parseInt(rowReturn.cells[1].innerHTML);
            ObjectDetails.Ph_Ret_BrandName = rowReturn.cells[3].innerHTML;
            ObjectDetails.Ph_Ret_Batch = rowReturn.cells[4].innerHTML;
            ObjectDetails.Ph_Ret_ExpiryDate = rowReturn.cells[5].innerHTML;
            ObjectDetails.Ph_Ret_Qty = parseFloat(rowReturn.cells[14].getElementsByTagName("input")[0].value);
            ObjectDetails.Ph_Ret_ReminQty = Ph_Ret_ReminQty;
            ObjectDetails.Ph_Ret_EachRate = parseFloat(rowReturn.cells[8].innerHTML);
            ObjectDetails.Ph_Ret_BillCost = parseFloat(rowReturn.cells[9].innerHTML);
            ObjectDetails.Ph_Ret_RowAmt = parseFloat(rowReturn.cells[10].innerHTML);
            ObjectDetails.Ph_Ret_RowTax = parseFloat(rowReturn.cells[11].innerHTML);
            ObjectDetails.Ph_Ret_GST = parseFloat(rowReturn.cells[12].innerHTML);
            ObjectDetails.Ph_Ret_RowNetAmt = parseFloat(rowReturn.cells[13].innerHTML);
            ObjectDetails.Ph_Ret_Cat_Name = rowReturn.cells[15].innerHTML;
            ObjectDetails.IsChanged = rowReturn.cells[16].innerHTML;
            ReturnInvoice.push(ObjectDetails);
        }
        var sendJsonData = {
            Ph_Ret_InvoiceNo: $("#txtInvoiceNo").val(),
            Ph_Ret_SupplierID: parseInt($("#ddlSuplier").val()),
            SupplierInvoiceDate: $("#txtInvoiceDate").val(),
            SupplierName: $("#ddlSuplier option:selected").text(),
            InvoiceGSTType: $("#ddlGstType option:selected").text(),
            InvoiceType: $("#ddlInvoiceType option:selected").text(),
            Ph_Ret_StoreName: $("#ddlWherHouse option:selected").text(),
            Ph_Stock_ReturnType: $("#drpReturnType option:selected").text(),
            Ph_Ret_Comments: $("#txtInvRetComments").val(),
            //Amount: parseFloat($("#txtSummaryAmt").val()),
            //Tax: parseFloat($("#txtSummaryTax").val()),
            //NetAmount: parseFloat($("#txtNetTotal").val()),
            //Discount: parseFloat($("#txtDiscountAmt").val()),
            //DisType: $("#ddlDiscountType").val(),
            //DiscountValue: parseFloat($("#txtRateValue").val()),
            InvoiceReturnDetails: ReturnInvoice
        };
        $.ajax({
            url: "/Pharma/Invoice/InvoiceReturnSave",
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

                    alert(response);
                }
                $("#ddlSuplier").val(0);
                $("#ddlWherHouse").val(0);
                $("#txtInvoiceNo").val('');
                $("#txtInvoiceDate").val('');
                $("#ddlGstType").val("GST purchase");
                $("#ddlInvoiceType").val("Select");
                $("#tblDrugBind tbody").empty();
                $("#tblInvoiceSummary tbody").empty();
                $("#tblReturnBind tbody").empty();
                $("#divReturnBind").prop('hidden', true);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    catch (e) { console.log(e); }
}
function Refresh() {
    try {
        window.location.href = "/Pharmacy/DirectInvoice/DirectInvoice";
    }
    catch (e) { console.log(e) }
}
function GotoLogin() {
    try {
        window.location.href = "/Login/Login";
    }
    catch (e) {
    }
}
//$(function () {
//    $('#txtExpiryDt').datepicker({

//        minDate: new Date(2021, 07 - 1, 10),
//    });
//});
function GetPurchaseExpiryDays() {
    var ExpiryDate = $("#txtExpiryDt").val();
    var wareHouse = $("#ddlWherHouse").val();
    if (wareHouse == "0") {
        $('#txtExpiryDt').prop("disabled", true);
    }
    else {
        $.ajax({
            url: "/Pharma/Invoice/GetPurchaseExpiryDays",
            type: "GET",
            data: {
                wareHouse: wareHouse,
            },
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var res = response;
                var arr2 = res.split('-');
                var year = arr2[0];
                var month = arr2[1];
                var day = arr2[2];
                $("#txtExpiryDt").datepicker({
                    // yearRange: '1900:' + Presentyear,
                    changeMonth: true,
                    changeYear: true,
                    // dateFormat: "mm/dd/yy"
                    dateFormat: "dd/mm/yy",
                    //minDate: '-0D',
                    minDate: new Date(year, month - 1, day),
                    maxDate: '+100M',
                });
                $('#txtExpiryDt').prop("disabled", false);

            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
      
}
function UpdateGSTChange() {
    var DrugCode = parseInt($("#txtPopupDrugCode").val());
    var GST = parseFloat($("#txtPopUpGst").val());
    $.ajax({
        url: "/Pharma/Invoice/UpdateGSTDrugMasterByCode",
        type: "GET",
        data: {
            DrugCode: DrugCode,
            GST: GST
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response =="Save Success" ) {
                GetInvoiceDrugByDrugCode(DrugCode);
                $("#txtPopUpGst").val('');
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}
//#endregion