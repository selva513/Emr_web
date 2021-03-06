$(document).ready(function () {
    GetSupplierMaster();
    GetStoreName();
    dateFunction();
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
                    $('#ddlDCSuplier')
                        .append($("<option></option>").val(PH_SupplierID).html(PH_SupplierName));
                }
            }
        }
    });
}
function SearchDrug() {
    $("#txtDCBrandSearch").autocomplete({
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
            $("#txtDCBrandSearch").val(ui.item.label);
        },
        select: function (event, ui) {
            // $("#txtDrugSearch").prop('disabled', true);
            $("#txtDCBrandSearch").val(ui.item.label);
            $("#txtDCDrugCode").val(ui.item.value);
            $("#txtDCBatch").focus();
            var DrugCode = parseInt(ui.item.value);

            GetDrugByDrugCode(DrugCode);
            return false;
        },
        minLength: 0
    });
}
function GetDrugByDrugCode(DrugCode) {
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
                    var Uom = response[PCHeader].PH_ITEM_DRUG_UOM;
                    var PH_ITEM_DRUG_QTY = response[PCHeader].PH_ITEM_DRUG_QTY;
                    $("#txtDCTax").val(PH_ITEM_DRUG_VAT);
                    //$("#lblDCUom").text("QTY / " + Uom);
                    $("#lblDCUom").text(Uom);
                    $("#txtDCStripNs").val(PH_ITEM_DRUG_QTY);
                }
            }
        }
    });
}
function DCPackQtyChange() {
    var DrugCode = parseInt($("#txtDCDrugCode").val());
    var pkqty = parseInt($("#txtDCStripNs").val());
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
function StripDCQtyChange() {
    var pkqty = parseInt($("#txtDCStripNs").val());
    var stripQty = parseFloat($("#txtDCStripQty").val());
    var Qty = parseFloat(stripQty * pkqty);
    $("#txtDCTotalQty").val(Qty);
}
function StripDCRateChange() {
    var pkqty = parseInt($("#txtDCStripNs").val());
    var stripRate = parseFloat($("#txtDCstripRate").val());
    var stripQty = parseFloat($("#txtDCStripQty").val());
    var FreeQty = parseInt($("#txtDCFree").val());
    FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Tax = parseFloat($("#txtDCTax").val());
    var Qty = stripQty.toFixed(2);
    //var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);

    var amt = parseFloat(stripRate * stripQty).toFixed(2);
    var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
    var TotalValue = parseFloat(amt) + parseFloat(vatamt);
    var avg = parseFloat(stripRate / pkqty).toFixed(2);
    $("#txtDCUnitRate").val(stripRate);
    $("#txtDCTotalAmt").val(TotalValue.toFixed(2));
    $("#txtDCTaxAmt").val(vatamt);
}
function FreeDCQtyChange() {
    var pkqty = parseInt($("#txtDCStripNs").val());
    var FreeQty = parseInt($("#txtDCFree").val());
    var stripQty = parseFloat($("#txtDCStripQty").val());
    FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    //var Qty = stripQty.toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);


    var TotalAmount = parseFloat($("#txtDCTotalAmt").val()).toFixed();
    //var UnitRate = parseFloat(TotalAmount / TotalQty).toFixed(2);

    //TotalAmount = parseFloat(UnitRate * TotalQty).toFixed(2);
    ////$("#txtStripQty").val(TotalQty);
    //$("#txtUnitRate").val(UnitRate);
    $("#txtDCTotalQty").val(TotalQty);
    return false;
}
function NewDCDiscountChange() {
    var pkqty = parseInt($("#txtDCStripNs").val());
    var stripRate = parseFloat($("#txtDCstripRate").val());
    var stripQty = parseFloat($("#txtDCStripQty").val());
    var FreeQty = parseInt($("#txtDCFree").val());
    //FreeQty = parseFloat(pkqty * FreeQty).toFixed(2);
    var Tax = parseFloat($("#txtDCTax").val());
    var Amount = parseFloat(stripRate * stripQty).toFixed(2);
    var Discount = parseFloat($("#txtDCDiscount").val()).toFixed(2);
    var disAmt = parseFloat((Amount * Discount) / 100).toFixed(2);
    //var Qty = parseFloat(stripQty * pkqty).toFixed(2);
    var Qty = stripQty.toFixed(2);
    var TotalQty = parseFloat(Qty) + parseFloat(FreeQty);


    var TotalAmount = parseFloat($("#txtDCTotalAmt").val()).toFixed(2);
    Amount = parseFloat(Amount) - parseFloat(disAmt);
    //var UnitRate = parseFloat(TotalAmount / TotalQty).toFixed(2);
    //TotalAmount = parseFloat(UnitRate * TotalQty).toFixed(2);
    var vatamt = parseFloat((Amount * Tax) / 100).toFixed(2);
    // $("#txtStripQty").val(TotalQty);
    var NetTotalAmt = parseFloat(Amount) + parseFloat(vatamt);
    //$("#txtDCUnitRate").val(UnitRate);
    $("#txtDCTotalAmt").val(NetTotalAmt);
    $("#txtDCTaxAmt").val(vatamt);
    return false;
}
function MRPDCChange() {
    var stripRate = parseFloat($("#txtDCstripRate").val());
    var MrpRate = parseInt($("#txtDCStripMRP").val());
    if (MrpRate < stripRate) {
        alert("MRP Should not lesser than Rate");
        $("#txtDCStripMRP").val('');
        return false;

    }
    var pkqty = parseInt($("#txtDCStripNs").val());
    var stripQty = parseFloat($("#txtDCStripQty").val());

    var avg = parseFloat(MrpRate / pkqty).toFixed(2);
    $("#txtDCUnitMRP").val(MrpRate);
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
                    $('#ddlDCWherHouse')
                        .append($("<option></option>").val(HIS_PH_STOREMASTER).html(HIS_PH_STORENAME));
                }
            }
        }
    });
}
function dateFunction() {
    var objDate = new Date();
    var Presentyear = objDate.getFullYear();
    $("#txtSupplierDCDate").datepicker({
        // yearRange: '1900:' + Presentyear,
        changeMonth: true,
        changeYear: true,
        // dateFormat: "mm/dd/yy"
        dateFormat: "dd/mm/yy",
        maxDate: '0d'
    });
    //$("#txtDCExpiryDt").datepicker({
    //    // yearRange: '1900:' + Presentyear,
    //    changeMonth: true,
    //    changeYear: true,
    //    // dateFormat: "mm/dd/yy"
    //    dateFormat: "dd/mm/yy",
    //    minDate: '-0D',
    //    maxDate: '+48M',
    //});
}
function RateValue(SelectedRow) {
    // var DrugName = $("#txtSearchDrugType").val();
    var row = SelectedRow.parentNode.parentNode;
    var rowIndex = SelectedRow.rowIndex;
    var Qty = parseInt(row.cells[5].getElementsByTagName("input")[0].value);
    if (Qty > 0) {
        var Cost = parseFloat(row.cells[7].getElementsByTagName("input")[0].value).toFixed(2);
        var Tax = parseFloat(row.cells[10].innerHTML).toFixed(2);
        var amt = parseFloat(Cost * Qty).toFixed(2);
        var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        var total = parseFloat(amt) + parseFloat(vatamt);
        total = total.toFixed(2);
        row.cells[9].innerHTML = amt;
        row.cells[11].innerHTML = vatamt;
        row.cells[13].innerHTML = total;
        TotalCalculation();
    }
    else {
        alert('Please Enter Qty');
    }
}
function TotalCalculation() {
    var DCAmountCells = document.getElementsByClassName("DCAmountCell"); //returns a list with all the elements that have class 'priceCell'
    var Amount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < DCAmountCells.length; i++) {
        if (DCAmountCells[i].innerHTML !== "") {
            var thisPrice = parseFloat(DCAmountCells[i].innerHTML); //get inner text of this cell in number format
            Amount = Amount + thisPrice;
        }
    };
    Amount = Amount.toFixed(2);
    $("#txtDCSummaryAmt").val(Amount);

    var DCTaxCells = document.getElementsByClassName("DCTaxCell"); //returns a list with all the elements that have class 'priceCell'
    var Tax = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < DCTaxCells.length; i++) {
        if (DCTaxCells[i].innerHTML !== "") {
            var thisPrice = parseFloat(DCTaxCells[i].innerHTML); //get inner text of this cell in number format
            Tax = Tax + thisPrice;
        }
    };
    Tax = Tax.toFixed(2);
    $("#txtDCSummaryTax").val(Tax);

    var TotalDCAmountCells = document.getElementsByClassName("TotalDCAmountCell"); //returns a list with all the elements that have class 'priceCell'
    var TotalAmount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < TotalDCAmountCells.length; i++) {
        if (TotalDCAmountCells[i].innerHTML != "") {
            var thisPrice = parseFloat(TotalDCAmountCells[i].innerHTML); //get inner text of this cell in number format
            TotalAmount = TotalAmount + thisPrice;
        }
    };
    TotalAmount = TotalAmount.toFixed(2);
    $("#txtDCSummaryNetAmt").val(TotalAmount);
    $("#txtDCNetTotal").val(TotalAmount);
}
function DiscountValue(SelectedRow) {
    // var DrugName = $("#txtSearchDrugType").val();
    var row = SelectedRow.parentNode.parentNode;
    var rowIndex = SelectedRow.rowIndex;
    var Qty = parseInt(row.cells[5].getElementsByTagName("input")[0].value);
    var DiscountValue = row.cells[12].getElementsByTagName("input")[0].value;
    var Discount = 0;
    if (DiscountValue !== "") {
        Discount = parseInt(row.cells[12].getElementsByTagName("input")[0].value);
        var Cost = parseFloat(row.cells[7].getElementsByTagName("input")[0].value).toFixed(2);
        var Tax = parseFloat(row.cells[10].innerHTML).toFixed(2);
        var amt = parseFloat(Cost * Qty).toFixed(2);
        var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        var total = parseFloat(amt) + parseFloat(vatamt);

        var disAmt = parseFloat((amt * Discount) / 100).toFixed(2);
        amt = parseFloat(amt) - parseFloat(disAmt);
        vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        total = parseFloat(amt) + parseFloat(vatamt);

        total = total.toFixed(2);
        row.cells[9].innerHTML = amt;
        row.cells[11].innerHTML = vatamt;
        row.cells[13].innerHTML = total;
        TotalCalculation();
    }
    else {
        Discount = 0;
        var Cost = parseFloat(row.cells[7].getElementsByTagName("input")[0].value).toFixed(2);
        var Tax = parseFloat(row.cells[10].innerHTML).toFixed(2);
        var amt = parseFloat(Cost * Qty).toFixed(2);
        var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        var total = parseFloat(amt) + parseFloat(vatamt);

        var disAmt = parseFloat((amt * Discount) / 100).toFixed(2);
        amt = parseFloat(amt) - parseFloat(disAmt);
        vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        total = parseFloat(amt) + parseFloat(vatamt);

        total = total.toFixed(2);
        row.cells[9].innerHTML = amt;
        row.cells[11].innerHTML = vatamt;
        row.cells[13].innerHTML = total;
        TotalCalculation();
    }

}
function percentage(percent, total) {
    return ((percent / 100) * total).toFixed(2)
}
function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function TotalDiscountCalculation() {
    var CalculationType = $("#ddlDCDiscountType").val();
    var Amount = $("#txtDCSummaryNetAmt").val();
    var PrecentageValue = $("#txtDCRateValue").val();
    if (CalculationType === "PER") {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= 100) {
                    var PrecResult = percentage(PrecentageValue, Amount);
                    $("#txtDCDiscountAmt").val(PrecResult);
                    var NetAmount = (Amount - PrecResult);
                    $("#txtDCNetTotal").val(NetAmount.toFixed(2));
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtDCSummaryNetAmt").val();
            if (SubTot.length > 0) {
                var NetTotal = parseFloat(SubTot);
                $("#txtDCDiscountAmt").val(0);
                $("#txtDCNetTotal").val(NetTotal.toFixed(2));
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                var NetTotal = parseFloat(SubTot);
                $("#txtDCDiscountAmt").val(0);
                $("#txtDCNetTotal").val(NetTotal.toFixed(2));
            }
        }
    }
    else {
        if (Amount.length > 0 && PrecentageValue.length > 0) {
            if (isNumeric(Amount) && isNumeric(PrecentageValue)) {
                if (parseFloat(PrecentageValue) <= parseFloat(Amount)) {
                    var PrecResult = (Amount - PrecentageValue);
                    $("#txtDCDiscountAmt").val(PrecentageValue);
                    $("#txtDCNetTotal").val(PrecResult.toFixed(2));
                }
                else {
                    alert('Discount % shall not be more than 100% Or the total value of the Service');
                }
            }
        }
        else {
            var SubTot = $("#txtDCSummaryNetAmt").val();
            if (SubTot.length > 0) {
                var NetTotal = parseFloat(SubTot);
                $("#txtDCDiscountAmt").val(0);
                $("#txtDCNetTotal").val(NetTotal.toFixed(2));
            }
            else {
                if (SubTot == "" || SubTot == null || SubTot == '')
                    SubTot = 0;
                $("#txtDCDiscountAmt").val(0);
                $("#txtDCNetTotal").val(NetTotal.toFixed(2));
            }
        }
    }
}
function DeleteOrdersRow(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var myrow = selectedrow.parentNode.parentNode;
    var rowIndex = row.rowIndex;
    var dtlSeqID = parseFloat(row.cells[2].innerHTML);
    document.getElementById("tblDCDrugBind").deleteRow(myrow.rowIndex);
    var table = document.getElementById("tblDCDrugBind");
    var rowCount = table.rows.length;
    var i = myrow.rowIndex;
    regroup(i, rowCount, "tblDCDrugBind");
    TotalCalculation();
}
function regroup(i, rc, ti) {
    for (j = (i + 1); j < rc; j++) {
        if (j > 0) {
            document.getElementById(ti).rows[j].cells[0].innerHTML = j;
        }
    }
}
function DcPurchaseSave() {
    if (Validation()) {
        var DrugInfo = new Array();
        var tblDrugSales = document.getElementById("tblDCDrugBind");
        var rowtblDrugSales = tblDrugSales.rows.length;
        var ErrorMsg = "";
        DrugInfo.length = 0;
        for (M = 1; M < rowtblDrugSales; M++) {
            var rowDrug = tblDrugSales.rows[M];
            var ObjectDetails = new Object();
            var QtyPerPack = parseInt(rowDrug.cells[16].innerHTML);
            ObjectDetails.DrugCode = parseFloat(rowDrug.cells[1].innerHTML);
            ObjectDetails.DrugName = rowDrug.cells[2].innerHTML;
            var Batch = rowDrug.cells[3].getElementsByTagName("input")[0].value;
            if (Batch !== "")
                ObjectDetails.Batch = rowDrug.cells[3].getElementsByTagName("input")[0].value;
            else
                ErrorMsg += "\n - Please Enter Batch for:" + ObjectDetails.DrugName + "";
            var ExpiryDt = rowDrug.cells[4].getElementsByTagName("input")[0].value;
            if (ExpiryDt !== "") {
                var DateSplit = ExpiryDt.split('-');
                var year = DateSplit[0];
                var month = DateSplit[1];
                var day = DateSplit[2];
                var today = new Date();
                var consoleddate = month + '/' + day + '/' + year + ' 23:59:59';
                var exdate = new Date(consoleddate);
                if (year.length > 4) {
                    ErrorMsg += "\n - Please Enter A Valid Year For: " + ObjectDetails.DrugName;
                }
                else if (exdate < today) {
                    ErrorMsg += "\n - Expiry Date Must Be Greater Than Today's Date For: " + ObjectDetails.DrugName;
                }
                else {
                    ObjectDetails.ExpiryDate = rowDrug.cells[4].getElementsByTagName("input")[0].value;
                }
            }
            else {

                ErrorMsg += "\n - Please Enter Expiry Date for:" + ObjectDetails.DrugName + "";
            }

            var Qty = rowDrug.cells[5].getElementsByTagName("input")[0].value;
            if (Qty !== "") {
                var pkqty = QtyPerPack;
                var Orderqty = parseInt(rowDrug.cells[5].getElementsByTagName("input")[0].value);
                var DrugQuantity = Orderqty * pkqty;
                ObjectDetails.Qty = parseInt(DrugQuantity);
            }
            else
                ErrorMsg += "\n - Please Enter Qty for:" + ObjectDetails.DrugName + "";
            var Free = rowDrug.cells[6].getElementsByTagName("input")[0].value;
            if (Free !== "") {
                var pkqty = QtyPerPack;
                var FreeOrderQty = parseFloat(rowDrug.cells[6].getElementsByTagName("input")[0].value);
                FreeOrderQty = FreeOrderQty * pkqty;
                ObjectDetails.FreeQty = parseFloat(FreeOrderQty);
            }
            else
                ObjectDetails.FreeQty = 0;
            var Rate = rowDrug.cells[7].getElementsByTagName("input")[0].value;
            if (Rate !== "") {
                var StripRate = parseFloat(rowDrug.cells[7].getElementsByTagName("input")[0].value);
                var pkqty = QtyPerPack;
                var DrugRate = parseFloat(StripRate / pkqty).toFixed(2);
                ObjectDetails.Rate = parseFloat(DrugRate);
            }
            else {
                ErrorMsg += "\n - Please Enter Rate for:" + ObjectDetails.DrugName + "";
            }
            var MRPValue = rowDrug.cells[8].getElementsByTagName("input")[0].value;
            if (MRPValue !== "") {
                var StripMRP = parseFloat(rowDrug.cells[8].getElementsByTagName("input")[0].value);
                var pkqty = QtyPerPack;
                var DrugMRP = parseFloat(StripMRP / pkqty).toFixed(2);
                ObjectDetails.MRP = parseFloat(DrugMRP);
            }
            else {
                ErrorMsg += "\n - Please Enter MRP for:" + ObjectDetails.DrugName + "";
            }
            ObjectDetails.Amount = parseFloat(rowDrug.cells[9].innerHTML);
            ObjectDetails.TaxPrecentage = parseFloat(rowDrug.cells[10].innerHTML);
            ObjectDetails.Tax = parseFloat(rowDrug.cells[11].innerHTML);
            var Discount = rowDrug.cells[12].getElementsByTagName("input")[0].value;
            if (Discount !== "") {
                ObjectDetails.Discount = parseFloat(rowDrug.cells[12].getElementsByTagName("input")[0].value);
            }
            else
                ObjectDetails.Discount = 0;
            ObjectDetails.NetAmount = parseFloat(rowDrug.cells[13].innerHTML);
            ObjectDetails.DtlSeqID = parseFloat(rowDrug.cells[15].innerHTML);
            DrugInfo.push(ObjectDetails);
        }
        if (ErrorMsg === "") {
            var sendJsonData = {
                SupplierDCNumber: $("#txtSupplierDCNo").val(),
                SupplierID: parseInt($("#ddlDCSuplier").val()),
                SupplierDCDate: $("#txtSupplierDCDate").val(),
                DcTaxType: $("#ddlDcPurchaseGstType option:selected").text(),
                SupplierName: $("#ddlDCSuplier option:selected").text(),
                WareHouse: $("#ddlDCWherHouse option:selected").text(),
                Amount: parseFloat($("#txtDCSummaryAmt").val()),
                Tax: parseFloat($("#txtDCSummaryTax").val()),
                NetAmount: parseFloat($("#txtDCNetTotal").val()),
                Discount: parseFloat($("#txtDCDiscountAmt").val()),
                DisType: $("#ddlDCDiscountType").val(),
                DiscountValue: parseFloat($("#txtDCRateValue").val()),
                DraftDeatils: DrugInfo
            };
            $.ajax({
                url: "/Pharma/DcPurchase/DcPurchaseSave",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(sendJsonData),
                dataType: "json",
                beforeSend: function () { $("#loading").css("display", "block"); },
                success: function (response) {
                    var element = document.getElementById('DivDcPurchaseEntry');
                    element.style.removeProperty("display");
                    var elementAddButton = document.getElementById('DivDcPurAdd');
                    elementAddButton.style.removeProperty("display");
                    ClearAll();
                    alert(response);
                },
                complete: function () { $("#loading").css("display", "none"); }
            });
        }
        else {
            alert(ErrorMsg);
        }
    }
}
function Validation() {
    var txt = "Required to fill the following field(s)";
    var opt = 0;
    var Supplire = parseFloat($("#ddlDCSuplier").val());
    if (Supplire === 0) {
        txt += "\n - Please Select Supplier Name";
        var opt = 1;
    }
    var SupplierInvoiceNumber = $("#txtSupplierDCNo").val();
    if (SupplierInvoiceNumber === '') {
        txt += "\n - Please Enter Dc Number";
        var opt = 1;
    }
    var InvoiceDt = $("#txtSupplierDCNo").val();
    if (InvoiceDt === '') {
        txt += "\n - Please Enter DC Date";
        var opt = 1;
    }
    var rowCount = $('#tblDCDrugBind tr').length;
    if (rowCount <= 1) {
        txt += "\n - Please Enter Drug Deatils";
        var opt = 1;
    }
    if (opt == "1") {
        alert(txt);
        $("#txtDCBrandSearch").focus();
        return false;
    }
    return true;
}
function ClearAll() {
    $("#txtDCBrandSearch").val('');
    $("#txtDCBatch").val('');
    $("#txtDCExpiryDt").val('');
    $("#txtDCExpiryDt").prop("disabled", true);
    $("#txtDCStripQty").val('');
    $("#txtDCFree").val('');
    $("#txtDCTotalQty").val('');
    $("#txtDCstripRate").val('');
    $("#txtDCDiscount").val('');
    $("#txtDCTax").val('');
    $("#txtDCStripMRP").val('');

    $("#ddlDCSuplier").val(0);
    $("#ddlDCWherHouse").val(0);
    $("#txtSupplierDCNo").val('');
    $("#txtSupplierDCDate").val('');
    $("#tblDCDrugBind tbody").empty();
    $("#tblDCInvoiceSummary tbody").empty();
    $("#txtDCSummaryAmt").val('');
    $("#txtDCSummaryTax").val('');
    $("#txtDCSummaryNetAmt").val('');
    $("#txtDCNetTotal").val('');
    $("#txtDCDiscountAmt").val('0');
    $("#txtDCRateValue").val('0');
}
function OpenDCList() {
    GetTop100DcPurchase();
    $("#ModelDCList").dialog(
        {
            title: "DC List",
            width: 1103,
            height: 500,
            modal: true,
            buttons: {
                "Convert Invoice": function () {
                    SaveInvoicepickList();
                },
                "Cancel": function () {
                    $("#ModelDCList").dialog("close");
                }
            }
        });
}
function GetTop100DcPurchase() {
    $.ajax({
        url: "/Pharma/DcPurchase/GetTop100DcPurchase",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblDCInvoice tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var PH_DC_SEQID = response[PCHeader].PH_DC_SEQID;
                    var PH_DC_DOCNO = response[PCHeader].PH_DC_DOCNO;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;
                    var PH_DC_ENTRYDATE = response[PCHeader].PH_DC_ENTRYDATE;
                    var PH_DC_ITEMCOUNT = response[PCHeader].PH_DC_ITEMCOUNT;
                    var PH_DC_InvoiceTotalAmt = response[PCHeader].PH_DC_InvoiceTotalAmt;
                    var PH_DC_SUPID = response[PCHeader].PH_DC_SUPID;
                    html += "<tr>";
                    html += "<td><input type='image' style='width:21px;height:21px;' src='" + rootUrl + "/Images/details_open.png' onclick='javascript:return CheckSelect(this)'></td>";
                    html += "<td>" + Sno + "</td>";
                    html += "<td style='display:none;'>" + PH_DC_SEQID + "</td>";
                    html += "<td>" + PH_SupplierName + "</td>";
                    html += "<td>" + PH_DC_DOCNO + "</td>";
                    html += "<td>" + PH_DC_ENTRYDATE + "</td>";
                    html += "<td>" + PH_DC_InvoiceTotalAmt + "</td>";
                    html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";
                    html += "<td style='display:none;'>" + PH_DC_SUPID + "</td>";
                    html += "<td style='text-align: center;display:none;'>";
                    html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteInvoiceRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                    html += "</tr>";
                }
                $("#tblDCInvoice tbody").append(html);
            }
        }
    });
}
function CheckPurListSelect(SelectedRow) {
    var row = SelectedRow.parentNode.parentNode;
    var myrow = SelectedRow.parentNode.parentNode;
    var rowIndex = row.rowIndex - 1;
    var PSeqID = row.cells[2].innerHTML;

    var className = myrow.cells[0].getElementsByTagName("input")[0].className;
    if (className === "shown") {
        var imgUrl = rootUrl + "/Images/details_open.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'abc');
        $('#tblDCPurchaseOrder > tbody > tr').eq(rowIndex).next().remove();
    }
    else {
        var imgUrl = rootUrl + "/Images/details_close.png";
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('src', imgUrl);
        myrow.cells[0].getElementsByTagName("input")[0].setAttribute('class', 'shown');

        var arg = parseFloat(PSeqID);
        $.ajax({
            url: "/api/PurchaseApi/GetPolistDrugDeatils/?SeqID=" + arg + "",
            type: 'Get',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {
                var html = "<table id='myTable1' class='table table-striped table-bordered table-hover table-full-width dataTable no-footer'>";
                html += "<tr>";
                html += "<thead><th>Brand Name</th><th>Order Qty</th><th>Cost</th>";
                html += "<th>Gst</th><th>Total Amount</th><th></th>";
                html += "</thead>";
                html += "</tr>";
                for (SymCount = 0; SymCount < response.length; SymCount++) {
                    var IsMoved = response[SymCount].IsMoved;
                    html += "<tr>";
                    html += "<td>" + response[SymCount].BrandName + "</td>";
                    html += "<td>" + response[SymCount].OrderStripQty + "</td>";
                    html += "<td>" + response[SymCount].Cost + "</td>";
                    html += "<td>" + response[SymCount].GST + "</td>";
                    html += "<td>" + response[SymCount].TotalAmount + "</td>";
                    if (IsMoved == false)
                        html += "<td>PO</td>";
                    else
                        html += "<td>Converterd To DC</td>";
                    html += "</tr>";
                }
                html += "</table></br>";
                var newRow = $('<tr><td></td><td colspan="11">' + html + '</td></tr>');
                $('#tblDCPurchaseOrder > tbody > tr').eq(rowIndex).after(newRow);
            },
            complete: function () { $("#loading").css("display", "none"); }
        });
    }
    return false;
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
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 32 && (charCode < 48 || charCode > 57) || (charCode == 32))

        return false;

    return true;
}
function OpenReprint() {
    GetDCPurchaseOrderTop100();
    $("#ModelDCPurchaseOrderList").dialog(
        {
            title: "Purchase Order List",
            width: 1006,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelDCPurchaseOrderList").dialog("close");
                }
            }
        });
}
function GetDCPurchaseOrderTop100() {
    $.ajax({
        url: "/api/PurchaseApi/GetPurchaseOrderTop100",
        type: "GET",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblDCPurchaseOrder tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var SeqID = response[PCHeader].SeqID;
                    var PurchaseOrderNumber = response[PCHeader].PurchaseOrderNumber;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;
                    var CreatedDatetime = response[PCHeader].CreatedDatetime;
                    var TotalItem = response[PCHeader].TotalItem;
                    var TotalAmount = response[PCHeader].TotalAmount;
                    var IsAprovedBy = response[PCHeader].IsAprovedBy;
                    if (IsAprovedBy === true) {
                        html += "<tr>";
                        html += "<td><input type='image' style='width:21px;height:21px;' src='" + rootUrl + "/Images/details_open.png' onclick='javascript:return CheckPurListSelect(this)'></td>";//0
                        html += "<td>" + Sno + "</td>";//1
                        html += "<td style='display:none;'>" + SeqID + "</td>";//2
                        html += "<td>" + PurchaseOrderNumber + "</td>";//3
                        html += "<td>" + PH_SupplierName + "</td>";//4
                        html += "<td>" + CreatedDatetime + "</td>";//5
                        html += "<td>" + TotalItem + "</td>";//6
                        html += "<td>" + TotalAmount + "</td>";//7
                        html += "<td>Approved</td>";//8
                        html += "<td><button type='button' onclick='return SelectDCPurchaseOrderBind(this)'>Select</button></td>";//9
                        html += "</tr>";
                    }
                }
                $("#tblDCPurchaseOrder tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function SelectDCPurchaseOrderBind(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var dtlSeqID = parseFloat(row.cells[2].innerHTML);
    var Status = row.cells[8].innerHTML;
    GetDCPurchaseOrderDrugBySeqID(dtlSeqID);
}
function GetDCPurchaseOrderDrugBySeqID(SeqID) {
    var SeqID = parseInt(SeqID);
    $.ajax({
        url: "/api/PurchaseApi/GetPurchaseOrderDrugBySeqID",
        type: "GET",
        data: {
            SeqID: SeqID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            var html = "";
            var table = document.getElementById("tblDCDrugBind");
            var tbodyRowCount = table.tBodies[0].rows.length;
            for (PCHeader = 0; PCHeader < response.PrintHeader.length; PCHeader++) {
                var SeqID = response.PrintHeader[PCHeader].SeqID;
                var PurchaseOrderNumber = response.PrintHeader[PCHeader].PurchaseOrderNumber;
                var CreatedDatetime = response.PrintHeader[PCHeader].CreatedDatetime;
                var PH_SupplierName = response.PrintHeader[PCHeader].PH_SupplierName;
                var TotalItem = response.PrintHeader[PCHeader].TotalItem;
                var TotalAmount = response.PrintHeader[PCHeader].TotalAmount;
                var QuotationNo = response.PrintHeader[PCHeader].QuotationNo;
                var DeliveryDate = response.PrintHeader[PCHeader].DeliveryDate;
                var IsAprovedBy = response.PrintHeader[PCHeader].IsAprovedBy;
                var Status = response.PrintHeader[PCHeader].Status;
                var WarehouseId = response.PrintHeader[PCHeader].WarehouseId;
                var WarehouseName = response.PrintHeader[PCHeader].WarehouseName;
                var SupplierID = response.PrintHeader[PCHeader].SupplierID;
                $("#ddlDCSuplier").val(SupplierID);
                $("#ddlDCWherHouse").val(WarehouseId);
            }
            for (PCDetails = 0; PCDetails < response.PrintDeatils.length; PCDetails++) {
                var DrugCode = response.PrintDeatils[PCDetails].DrugCode;
                var BrandName = response.PrintDeatils[PCDetails].BrandName;
                var GST = response.PrintDeatils[PCDetails].GST;
                var Cost = response.PrintDeatils[PCDetails].Cost;
                var PH_ITEM_HSNCODE = response.PrintDeatils[PCDetails].PH_ITEM_HSNCODE;
                var PH_ITEM_DRUG_GENERIC = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_GENERIC;
                var Qty = parseFloat(response.PrintDeatils[PCDetails].Qty).toFixed(2);
                var QtyperPack = parseInt(response.PrintDeatils[PCDetails].PH_ITEM_DRUG_QTY);
                var OrderStripQty = parseInt(response.PrintDeatils[PCDetails].OrderStripQty);
                var Uom = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_UOM;
                var dtlSeqID = response.PrintDeatils[PCDetails].DtlSeqID;
                var IsMoved = response.PrintDeatils[PCDetails].IsMoved;
                var Tax = parseFloat(GST).toFixed(2);
                var amt = parseFloat(Cost * OrderStripQty).toFixed(2);
                var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
                var total = parseFloat(amt) + parseFloat(vatamt);
                total = total.toFixed(2);
                var $tr = $('#tblDCDrugBind tr[data-id="' + DrugCode + '"]');
                if ($tr.length === 0) {
                    var Sno = PCDetails + 1;
                    if (IsMoved == false) {
                        html += "<tr data-id=\"" + DrugCode + "\"><td>" + Sno + "</td>";//0
                        html += "<td style='display:none;'>" + DrugCode + "</td>";//1
                        html += "<td>" + BrandName + "</td>";//2
                        html += "<td><input type=\"Text\" id='txtBatch' value=''style='height:28px;width:120px;' autocomplete='off' ></td>";//3
                        html += "<td><input type=\"date\" style='height:28px;' data-date-format='DD/MM/YYYY' value='' autocomplete='off' ></td>";//4
                        html += "<td><label hidden>" + Qty + "</label><input type=\"Text\" value='" + OrderStripQty + "' id='txtQTY' onkeypress='return isNumberKey(event);' onchange='RateValue(this)' autocomplete='off' value=''style='height:28px;width:50px;'> " + Uom + "</td>";//5
                        html += "<td><input type=\"Text\" id='txtFree' onkeypress='return isNumberKey(event);' autocomplete='off' value='' style='height:28px;width:50px;'></td>";//6
                        html += "<td><input type=\"Text\" value='" + Cost + "' id='txtRate' value=''style='height:28px;width:50px;' autocomplete='off' onchange='RateValue(this)' onkeypress='return isNumberAndDecimal(this,event)'> " + Uom + "</td>";//7
                        html += "<td><input type=\"Text\" id='txtMRP' value='' onkeypress='return isNumberAndDecimal(this,event)' autocomplete='off' style='height:28px;width:50px;'> " + Uom + "</td>";//8
                        html += "<td  class='DCAmountCell'>" + amt + "</td>";//9
                        html += "<td>" + GST + "</td>";//10
                        html += "<td class='DCTaxCell'>" + vatamt + "</td>";//11
                        html += "<td><input type=\"Text\" id='txtDiscount' onchange='DiscountValue(this)' onkeypress='return isNumberAndDecimal(this,event)' autocomplete='off' value=''style='height:28px;width:50px;'></td>";//12
                        html += "<td class='TotalDCAmountCell'>" + total + "</td>";//13
                        html += "<td style='text-align: center;'>";
                        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";//14
                        html += "<td style='display:none;'>" + dtlSeqID + "</td>";//15
                        html += "<td style='display:none;'>" + QtyperPack+"</td>";//16
                        html += "</tr>";
                    }
                }
            }
            if (html == "") {
                alert("PO Already Converted To DC");
            }
            $("#tblDCDrugBind tbody").append(html);
            TotalCalculation();
            //document.getElementById("DivDcPurchaseEntry").style.display = "none";
            //document.getElementById("DivDcPurAdd").style.display = "none";
            $("#ModelDCPurchaseOrderList").dialog("close");
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
function DCValidation() {
    var txt = "Required to fill the following field(s)";
    var opt = 0;

    var DrugName = $("#txtDCBrandSearch").val();
    if (DrugName === '') {
        $("#txtBrandSearch").focus();
        txt += "\n - Please Enter Drug Name";
        var opt = 1;
    }
    var Batch = $("#txtDCBatch").val();
    if (Batch === '') {
        $("#txtBatch").focus();
        txt += "\n - Please Enter BatchNo";
        var opt = 1;
    }
    var ExpiryDt = $("#txtDCExpiryDt").val();
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
    var Rate = $("#txtDCstripRate").val();
    if (Rate === '' || Rate === '0.00') {
        $("#txtDCstripRate").focus();
        $("#txtDCstripRate").val('0.00');
        txt += "\n - Please Enter Purchase Rate";
        var opt = 1;
    }
    var MRP = $("#txtDCStripMRP").val();
    if (MRP === '' || MRP === '0.00') {
        $("#txtDCStripMRP").focus();
        $("#txtDCStripMRP").val('0.00');
        txt += "\n - Please Enter MRP";
        var opt = 1;
    }
    var QTY = $("#txtDCStripQty").val();
    if (QTY === '' || QTY === '0.00') {
        $("#txtDCStripQty").focus();
        $("#txtDCStripQty").val('0.00');
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
function AddToDCTable() {
    if (DCValidation()) {
        StripDCRateChange();
        var DrugName = $("#txtDCBrandSearch").val();
        var DrugCode = parseInt($("#txtDCDrugCode").val());
        var Batch = $("#txtDCBatch").val();
        var ExpiryDate = $("#txtDCExpiryDt").val();
        var DateSplit = ExpiryDate.split('/');
        var day = DateSplit[0];
        var month = DateSplit[1];
        var year = DateSplit[2];
        var ExpiryDtval = year + "-" + month + "-" + day;
        var Rate = parseFloat($("#txtDCUnitRate").val());
        var MRP = parseFloat($("#txtDCUnitMRP").val());
        //var Qty = parseFloat($("#txtDCTotalQty").val());
        var Qty = parseFloat($("#txtDCStripQty").val());
        var QtyPerPack = parseInt($("#txtDCStripNs").val());
        var FreeQty = $("#txtDCFree").val();
        if (FreeQty !== "")
            FreeQty = parseFloat(FreeQty);
        else
            FreeQty = 0;
        var Discount = $("#txtDCDiscount").val();
        var TaxAmount = parseFloat($("#txtDCTaxAmt").val());
        var NetAmount = parseFloat($("#txtDCTotalAmt").val());
        var Tax = parseFloat($("#txtDCTax").val());
        var Uom = $("#lblDCUom").text();
        var Amount = parseFloat(NetAmount) - parseFloat(TaxAmount);
        Amount = Amount.toFixed(2);
        //var disAmt = 0;
        //if (Discount !== "") {
        //    disAmt = parseFloat((Amount * Discount) / 100).toFixed(2);
        //    Amount = parseFloat(Amount) - parseFloat(disAmt);
        //    NetAmount = parseFloat(Amount) + parseFloat(TaxAmount);
        //}
        var html = "";
        var table = document.getElementById("tblDCDrugBind");
        var tbodyRowCount = table.tBodies[0].rows.length;
        var Sno = tbodyRowCount + 1;
        html += "<tr data-id=\"" + DrugCode + "\"><td>" + Sno + "</td>";//0
        html += "<td style='display:none;'>" + DrugCode + "</td>";//1
        html += "<td>" + DrugName + "</td>";//2
        html += "<td><input type=\"Text\" id='txtBatch' value='" + Batch + "'style='height:28px;width:120px;' ></td>";//3
        html += "<td><input type=\"date\" style='height:28px;' data-date-format='DD/MM/YYYY' value='" + ExpiryDtval + "'></td>";//4
        html += "<td><input type=\"Text\" id='txtQTY' onkeypress='return isNumberKey(event);' onchange='RateValue(this)' value='" + Qty + "'style='height:28px;width:50px;'> " + Uom + "</td>";//5
        html += "<td><input type=\"Text\" id='txtFree' onkeypress='return isNumberKey(event);' onchange='RateValue(this)' value='" + FreeQty + "' value=''style='height:28px;width:50px;'></td>";//6
        html += "<td><input type=\"Text\" id='txtRate' value='" + Rate + "'style='height:28px;width:50px;' onchange='RateValue(this)' onkeypress='return isNumberAndDecimal(this,event)'> " + Uom + "</td>";//7
        html += "<td><input type=\"Text\" id='txtMRP' value='" + MRP + "' onkeypress='return isNumberAndDecimal(this,event)' style='height:28px;width:50px;'> " + Uom + "</td>";//8
        html += "<td  class='DCAmountCell'>" + Amount + "</td>";//9
        html += "<td>" + Tax + "</td>";//10
        html += "<td class='DCTaxCell'>" + TaxAmount + "</td>";//11
        html += "<td><input type=\"Text\" id='txtDiscount' value='" + Discount + "'  onchange='DiscountValue(this)' onkeypress='return isNumberAndDecimal(this,event)' value=''style='height:28px;width:50px;'></td>";//12
        html += "<td class='TotalDCAmountCell'>" + NetAmount + "</td>";//13
        html += "<td style='text-align: center;'>";
        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeleteOrdersRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'>";//14
        html += "<td style='display:none;'>" + 0 + "</td>";//15
        html += "<td style='display:none;'>" + QtyPerPack + "</td>";//16
        html += "</tr>";

        $("#tblDCDrugBind tbody").append(html);
        $("#txtDCBrandSearch").val('');
        html = "";
        TotalCalculation();
        ClearDraft();
    }
}
function ClearDraft() {
    $("#txtDCBrandSearch").val('');
    $("#txtDCDrugCode").val('');
    $("#txtDCBatch").val('');
    $("#txtDCExpiryDt").val('');
    $("#txtDCUnitRate").val('');
    $("#txtDCUnitMRP").val('');
    $("#txtDCTotalQty").val('');
    $("#txtDCFree").val('0');
    $("#txtDCDiscount").val('');
    $("#txtDCTaxAmt").val('');
    $("#txtDCTotalAmt").val('');
    $("#txtDCTax").val('');
    $("#lblDCUom").text('UOM');
    $("#txtDCStripQty").val('');
    $("#txtDCstripRate").val('');
    $("#txtDCStripMRP").val('');
    $("#txtDCStripNs").val(0);
}
function GetDCDeatilsForGst() {
    var supplierId = parseInt($("#ddlSuplier").val());


    $.ajax({
        url: "/Pharma/Invoice/GetStateDeatilsForGst?SupplierId=" + supplierId,
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response > 0) {
                document.getElementById("ddlDcPurchaseGstType").value = "GST purchase";
                document.getElementById("ddlDcPurchaseGstType").text = "GST purchase";
            }
            else {
                document.getElementById("ddlDcPurchaseGstType").value = "IGST purchase";
                document.getElementById("ddlDcPurchaseGstType").text = "IGST purchase";
                /*document.getElementById("ddlGstType").value("IGST purchase");*/

            }
        }
    });
}

function GetDCPurchaseExpiryDays() {
    var ExpiryDate = $("#txtExpiryDt").val();
    var wareHouse = $("#ddlDCWherHouse").val();
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
            $("#txtDCExpiryDt").datepicker({
                // yearRange: '1900:' + Presentyear,
                changeMonth: true,
                changeYear: true,
                // dateFormat: "mm/dd/yy"
                dateFormat: "dd/mm/yy",
                //minDate: '-0D',
                minDate: new Date(year, month - 1, day),
                maxDate: '+48M',
            });
            $('#txtDCExpiryDt').prop("disabled", false);

        },
        complete: function () { $("#loading").css("display", "none"); }
    });
}