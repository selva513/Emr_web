var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
$(document).ready(function () {
    GetPOWarehouseList();
    SearchSupplierDetails();
    var objDate = new Date();
    var Presentyear = objDate.getFullYear();
    $("#txtDeliveryDate").datepicker({
        // yearRange: '1900:' + Presentyear,
        changeMonth: true,
        changeYear: true,
        // dateFormat: "mm/dd/yy"
        dateFormat: "dd/mm/yy",
        minDate: 0
    });
});

function GetUserRoleAccess() {
    try {
        $.ajax({
            url: rootUrl + "/api/PurchaseApi/",
            type: 'GET',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () { $("#loading").css("display", "block"); },
            success: function (response) {

            },
            complete: function () { $("#loading").css("display", "none"); }
        })
    } catch (e) { console.log(e); }
}
function GetPOWarehouseList() {
    $.ajax({
        url: "/Pharma/Invoice/GetStoreName",
        type: "GET",
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                $("#ddlPOWarehouse").empty();
                $('#ddlPOWarehouse').append($("<option hidden></option>").val(0).html("-Select-"));
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var HIS_PH_STOREMASTER = response[PCHeader].HIS_PH_STOREMASTER;
                    var HIS_PH_STORENAME = response[PCHeader].HIS_PH_STORENAME;
                    $('#ddlPOWarehouse').append($("<option></option>").val(HIS_PH_STOREMASTER).html(HIS_PH_STORENAME));
                }
            }
        }
    });
}
function SearchSupplierDetailsClick() {
    var SupplierName = $("#txtSearchPoList").val();
    if (SupplierName != null && SupplierName != "")
        $("#txtSearchPoList").autocomplete("Search", SupplierName);
    else {
        $("#txtSearchPoList").autocomplete("Search", "");
    }
}
function SearchSupplierDetails() {
    $("#txtSearchPoList").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: rootUrl + "/api/PurchaseApi/GetSupplierNameBySearch",
                type: "GET",
                data: {
                    Search: request.term
                },
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.PH_SUPPLIERNAME,
                            value: el.PH_SUPPLIERID
                        };
                    }));
                }
            });
        },
        focus: function (event, ui) {
            var SupplierName = ui.item.label;
            $('#txtSearchPoList').val(SupplierName);
            //$("#txtCity").val(ui.item.label);
            event.preventDefault();
        },
        select: function (event, ui) {
            $("#lblSupplierName").html(ui.item.label);
            var SupplierName = ui.item.label;
            $("#txtSearchPoList").val(SupplierName);
            $("#hidSupplierID").val(ui.item.value);
            GetSelectedPurchaseOrder();
            event.preventDefault();
            false;
        },
        minLength: 0
    });
}

//Selvendiran
function SearchDrugDetails() {
    var DrugSearch = $("#txtdrugSearch").val();
    $.ajax({
        url: rootUrl + "/api/PurchaseApi/GetDrugDetailsBySearch?Search=" + DrugSearch,
        type: "GET",
        dataType: "json",
        success: BindDrugDetails,
    });

}
function SelectAll() {
    var checkAll = $("#SelectAll").prop('checked');
    if (checkAll) {
        $(".case").prop("checked", true);
    } else {
        $(".case").prop("checked", false);
    }
}
$(".case").click(function () {
    if ($(".case").length == $(".case:checked").length) {
        $("#SelectAll").prop("checked", true);
    } else {
        $("#SelectAll").prop("checked", false);
    }
});
function GetPurchaseDrugBySearch() {
    $("#txtPurchaseBrandSearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: rootUrl + "/api/PurchaseApi/GetDrugBySearch",
                type: "GET",
                data: {
                    Search: request.term
                },
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (el) {
                        return {
                            label: el.PH_ITEM_DRUGNAME_BRAND,
                            value: el.PH_ITEM_DrugCode
                        };
                    }));
                }
            });
        },
        focus: function (event, ui) {
            event.preventDefault();
        },
        select: function (event, ui) {
            $("#txtPurchaseBrandSearch").val(ui.item.label);
            var DrugCode = parseInt(ui.item.value);

            GetCurretntStockByDrugCode(DrugCode);
            return false;
        },
        minLength: 0
    });
}
function OnClickAddDrug() {
    var SupplierName = $("#lblSupplierName").html();
    var HEADERID = sessionStorage.getItem("PH_ITEM_HEADERID");
    if (HEADERID == null || HEADERID == "") {
        HEADERID = 0;
    }
    var PH_ITEM_HEADERID = parseFloat(HEADERID);
    if (SupplierName == "") {
        alert("Please Select the Supplier")
    }
    else
        $('#modal-default').modal('show');
    //var PH_SupplierName = $("#lblSupplierName").html();
    $.ajax({
        url: rootUrl + "/api/PurchaseApi/GetDrugMasterDetailsByHospitalID?",
        type: 'Get',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: BindDrugDetails,
        complete: function () { $("#loading").css("display", "none"); },
    });
}
function BindDrugDetails(response) {
    var html = "";
    $("#tblDrug tbody").empty();
    for (Drug = 0; Drug < response.lstResult.length; Drug++) {
        var PH_ITEM_DRUGNAME_BRAND = response.lstResult[Drug].PH_ITEM_DRUGNAME_BRAND
        html += "<tr  style='font-size:13px;font-family:sans-serif;'>";
        html += "<td style='display:none'>" + response.lstResult[Drug].PH_ITEM_DrugCode + "</td>";
        html += "<td style='display:none'></td>";
        html += "<td>" + PH_ITEM_DRUGNAME_BRAND + "</td>";
        html += "<td>" + response.lstResult[Drug].PH_ITEM_DRUG_GENERIC + "</td>";
        html += "<td>" + response.lstResult[Drug].PH_ITEM_DRUG_UOM + "</td>";
        html += "<td>" + response.lstResult[Drug].PH_ITEM_SCHEDULETYPE + "</td>";
        html += "<td>" + response.lstResult[Drug].PH_ITEM_STRENGTH + "</td>";
        html += "<td><input type='checkbox' class='case'  id='Check' style='text-align: center;' onclick ='GetSelectedDrugRow(this);' /></td >";
        html += "<tr>";
    }
    $("#tblDrug tbody").append(html);
}
function GetCurretntStockByDrugCode(DrugCode) {
    $.ajax({
        url: "/api/PurchaseApi/GetPurDrugByDrugCode",
        type: "GET",
        data: {
            DrugCode: DrugCode,
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var table = document.getElementById("tblPurchaseDrugBind");
                var tbodyRowCount = table.tBodies[0].rows.length;
                var PH_CUR_DRUGBRANDNAME = "";
                var PH_ITEM_STRENGTH = "";
                var PH_ItemCatShortCode = "";
                var PH_CUR_STOCK = 0;
                var PH_ITEM_DRUG_VAT = "";
                var PH_ITEM_HSNCODE = "";
                var PH_CUR_STOCK_PURCHCOST = "";
                var PH_ITEM_DRUG_GENERIC = "";
                var PH_CUR_DRUGCODE = 0;
                var PH_ITEM_DRUG_UOM = "";
                var PH_ITEM_DRUG_QTY = 1;
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    PH_CUR_DRUGCODE = response[PCHeader].PH_CUR_DRUGCODE;
                    PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_CUR_DRUGBRANDNAME;
                    PH_ITEM_STRENGTH = response[PCHeader].PH_ITEM_STRENGTH;
                    PH_ItemCatShortCode = response[PCHeader].PH_ItemCatShortCode;
                    var PurStock = parseFloat(response[PCHeader].PH_CUR_STOCK);
                    PH_CUR_STOCK = parseFloat(PH_CUR_STOCK) + parseFloat(PurStock);
                    PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_DRUG_VAT;
                    PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                    PH_CUR_STOCK_PURCHCOST = response[PCHeader].PH_CUR_STOCK_PURCHCOST;
                    ExpiryDt = response[PCHeader].ExpiryDt;
                    PH_ITEM_DRUG_GENERIC = response[PCHeader].PH_ITEM_DRUG_GENERIC;
                    PH_ITEM_DRUG_UOM = response[PCHeader].PH_ITEM_DRUG_UOM;
                    PH_ITEM_DRUG_QTY = response[PCHeader].PH_ITEM_DRUG_QTY;
                    PH_CUR_STOCK_PURCHCOST = parseFloat(PH_CUR_STOCK_PURCHCOST) * parseFloat(PH_ITEM_DRUG_QTY);
                }
                var $tr = $('#tblPurchaseDrugBind tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                if ($tr.length === 0) {
                    html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\">";
                    html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//0
                    html += "<td>" + PH_ItemCatShortCode + "." + PH_CUR_DRUGBRANDNAME + " " + PH_ITEM_STRENGTH + "</td>";//1
                    html += "<td style='display:none;'>" + PH_ITEM_DRUG_GENERIC + "</td>";//2
                    html += "<td>" + PH_CUR_STOCK + "</td>";//3
                    html += "<td>" + PH_ITEM_HSNCODE + "</td>";//4
                    html += "<td>" + PH_ITEM_DRUG_VAT + "</td>";//5
                    html += "<td>0</td>";//6
                    html += "<td> <input value='" + PH_ITEM_DRUG_QTY + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//7
                    html += "<td><input value='0'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//8
                    html += "<td></td>";//9
                    html += "<td><input value='" + PH_CUR_STOCK_PURCHCOST.toFixed(2) + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'></td>";//10
                    html += "<td class='TaxCell'></td>";//11
                    html += "<td class='AmountCell'></td>";//12
                    //html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";//11
                    html += "<td style='text-align: center;'>";
                    html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeletePurchaseRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                    html += "</tr>";
                    $("#tblPurchaseDrugBind tbody").append(html);
                    $("#txtPurchaseBrandSearch").val('');
                    html = "";
                }
                else {
                    alert('Drug Already Exists');
                    $("#txtPurchaseBrandSearch").val('');
                    $("#txtPurchaseBrandSearch").focus();
                }
            }
            else {
                GetPoListDrugByDrugCode(DrugCode);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function GetPoListDrugByDrugCode(DrugCode) {
    $.ajax({
        url: "/Pharma/Invoice/GetDrugByDrugCode",
        type: "GET",
        data: {
            DrugCode: DrugCode
        },
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    var PH_ITEM_DRUG_VAT = response[PCHeader].PH_ITEM_GST;
                    var Uom = response[PCHeader].PH_ITEM_DRUG_UOM;
                    var PH_ITEM_DRUG_QTY = response[PCHeader].PH_ITEM_DRUG_QTY;
                    var PH_CUR_DRUGBRANDNAME = response[PCHeader].PH_ITEM_DRUGNAME_BRAND;
                    var PH_ITEM_STRENGTH = response[PCHeader].PH_ITEM_STRENGTH;
                    var PH_ItemCatShortCode = response[PCHeader].PH_ItemCatShortCode;
                    var PH_CUR_DRUGCODE = response[PCHeader].PH_ITEM_DrugCode;
                    var PH_ITEM_HSNCODE = response[PCHeader].PH_ITEM_HSNCODE;
                    var PH_ITEM_DRUG_GENERIC = response[PCHeader].PH_ITEM_DRUG_GENERIC;
                    var $tr = $('#tblPurchaseDrugBind tr[data-id="' + PH_CUR_DRUGCODE + '"]');
                    if ($tr.length === 0) {
                        html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\">";
                        html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//0
                        html += "<td>" + PH_ItemCatShortCode + "." + PH_CUR_DRUGBRANDNAME + " " + PH_ITEM_STRENGTH + "</td>";//1
                        html += "<td style='display:none;'>" + PH_ITEM_DRUG_GENERIC + "</td>";//2
                        html += "<td>" + 0 + "</td>";//3
                        html += "<td>" + PH_ITEM_HSNCODE + "</td>";//4
                        html += "<td>" + PH_ITEM_DRUG_VAT + "</td>";//5
                        html += "<td>" + 0 + "</td>";//6
                        html += "<td> <input value='" + PH_ITEM_DRUG_QTY + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + Uom + " </td>";//7
                        html += "<td><input value='0'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + Uom + " </td>";//8
                        html += "<td></td>";//9
                        html += "<td><input value=''style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'></td>";//10
                        html += "<td class='TaxCell'></td>";//11
                        html += "<td class='AmountCell'></td>";//12
                        //html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";//11
                        html += "<td style='text-align: center;'>";
                        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeletePurchaseRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                        html += "</tr>";
                        $("#tblPurchaseDrugBind tbody").append(html);
                        $("#txtPurchaseBrandSearch").val('');
                        html = "";
                    }
                    else {
                        alert('Drug Already Exists');
                        $("#txtPurchaseBrandSearch").val('');
                        $("#txtPurchaseBrandSearch").focus();
                    }
                }
            }
        }
    });
}
function TotalPurchaseCalculation() {
    var AmountCells = document.getElementsByClassName("AmountCell"); //returns a list with all the elements that have class 'priceCell'
    var Amount = 0;
    //loop over the cells array and add to total price
    for (var i = 0; i < AmountCells.length; i++) {
        if (AmountCells[i].innerHTML !== "") {
            var thisPrice = parseFloat(AmountCells[i].innerHTML); //get inner text of this cell in number format
            Amount = Amount + thisPrice;
        }
    };
    Amount = Amount.toFixed(2);
    $("#txtTotalamt").val(Amount);
}
function PurchaseQtyValu(SelectedRow) {
    // var DrugName = $("#txtSearchDrugType").val();
    var row = SelectedRow.parentNode.parentNode;
    var rowIndex = SelectedRow.rowIndex;
    var gst = row.cells[5].innerHTML;
    if (gst == "0.00" || gst == "0") {
        $('#modal-PurchaseAddGst').modal('show');

        var PH_ITEM_DrugCode = row.cells[0].innerHTML;
        var drugName = row.cells[1].innerHTML;
        $('#hidPurSeqID').val(PH_ITEM_DrugCode);
        $('#PurlblDrugName').html(drugName);
        $('#txtPopupDrugCode').val(PH_ITEM_DrugCode);
        return false;
    }
    var Qty = row.cells[8].getElementsByTagName("input")[0].value;
    if (Qty !== "") {
        var Rate = parseFloat(row.cells[10].getElementsByTagName("input")[0].value).toFixed(2);
        var Cost = 0;
        if (Rate > 0) {
            Cost = Rate;
        }
        else {
            Cost = parseFloat(row.cells[6].innerHTML).toFixed(2);
        }
        var PackQty = parseFloat(row.cells[7].getElementsByTagName("input")[0].value).toFixed(2);
        var TotalQty = parseFloat(Qty) * parseFloat(PackQty);
        var Tax = parseFloat(row.cells[5].innerHTML).toFixed(2);
        var amt = parseFloat(Cost * Qty).toFixed(2);
        var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
        var total = parseFloat(amt) + parseFloat(vatamt);
        total = total.toFixed(2);
        row.cells[9].innerHTML = TotalQty;
        row.cells[11].innerHTML = vatamt;
        row.cells[12].innerHTML = total;
        TotalPurchaseCalculation();
        //$("#txtPurchaseBrandSearch").focus();
        var DrugCode = parseInt(row.cells[0].innerHTML);
        var pkqty = parseInt(row.cells[7].getElementsByTagName("input")[0].value);
        POlistPackQtyChange(DrugCode, pkqty);
    }
    else {

    }
}
function POlistPackQtyChange(DrugCode, pkqty) {
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
function SavePurchaseOrder(IsApproved) {
    if (PurchaseValidation()) {
        var tbl12 = document.getElementById("tblPurchaseDrugBind");
        var PickArray = new Array();
        var row12 = tbl12.rows.length;
        for (k = 1; k < row12; k++) {
            var sval12 = tbl12.rows[k];
            var QtyValue = sval12.cells[8].getElementsByTagName("input")[0].value;
            var RateValue = sval12.cells[10].getElementsByTagName("input")[0].value;
            if (RateValue !== "") {
                RateValue = parseFloat(RateValue);
            }
            else {
                RateValue = 0;
            }
            if (QtyValue !== "") {
                QtyValue = parseFloat(QtyValue);
            }
            else {
                QtyValue = 0;
            }
            if (QtyValue !== 0 && RateValue !== 0) {
                var objDetails = new Object();
                var PackQty = parseFloat(sval12.cells[7].getElementsByTagName("input")[0].value);
                var Rate = parseFloat(sval12.cells[10].getElementsByTagName("input")[0].value);
                if (Rate > 0) {
                    //PackQty = Rate / PackQty;
                    //objDetails.Cost = parseFloat(PackQty);
                    objDetails.Cost = Rate;
                }
                else {

                    objDetails.Cost = parseFloat(sval12.cells[6].innerHTML);
                    //PackQty = objDetails.Cost / PackQty;
                    //objDetails.Cost = parseFloat(PackQty);
                }
                objDetails.DrugCode = parseInt(sval12.cells[0].innerHTML);
                objDetails.BrandName = sval12.cells[1].innerHTML;
                objDetails.GST = parseFloat(sval12.cells[5].innerHTML);
                objDetails.Qty = parseInt(sval12.cells[9].innerHTML);
                objDetails.OrderStripQty = QtyValue;
                objDetails.TaxAmount = parseFloat(sval12.cells[9].innerHTML);
                objDetails.TotalAmount = parseFloat(sval12.cells[12].innerHTML);
                if (objDetails.Qty > 0) {
                    PickArray.push(objDetails);
                }
            }
        }
        var Supplier = $("#hidSupplierID").val();
        var WarehouseId = $("#ddlPOWarehouse").val();
        var WarehouseName = $("#ddlPOWarehouse option:selected").text();

        if (Supplier.length > 0) {
            if (PickArray.length > 0) {
                var SeqID = $("#hidHeadSeqID").val();
                if (SeqID !== "")
                    SeqID = parseFloat($("#hidHeadSeqID").val());
                else
                    SeqID = 0;
                var sendJsonData = {
                    SeqID: SeqID,
                    SupplierID: parseInt($("#hidSupplierID").val()),
                    TotalAmount: parseFloat($("#txtTotalamt").val()),
                    QuotationNo: $("#txtQuotationNo").val(),
                    DeliveryDate: $("#txtDeliveryDate").val(),
                    IsAprovedBy: IsApproved,
                    WarehouseId: parseInt(WarehouseId),
                    WarehouseName: WarehouseName,
                    purchaseOrderDtlInfos: PickArray
                };
                $.ajax({
                    url: "/api/PurchaseApi/SavePurchaseOrder",
                    type: 'post',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(sendJsonData),
                    dataType: "json",
                    beforeSend: function () { $("#loading").css("display", "block"); },
                    success: function (response) {
                        $("#hidSupplierID").val('');
                        $("#txtQuotationNo").val('');
                        $("#txtDeliveryDate").val('');
                        $("#hidHeadSeqID").val(0);
                        $("#tblPurchaseDrugBind tbody").empty();
                        $("#txtTotalamt").val('');
                        $("#txtSearchPoList").val('');
                        $("#ddlPOWarehouse").val(0);
                    },
                    complete: function () { $("#loading").css("display", "none"); }
                });
            }
            else {
                alert('Please Enter Qty And Rate For All Drug');
            }
        }
        else {
            alert('Please Select Supplier');
        }
        return false;
    }
}
function PurchaseValidation() {
    var txt = "Required to fill the following field(s)";
    var opt = 0;
    var Supplire = $("#hidSupplierID").val();

    if (Supplire === '') {
        txt += "\n - Please Select Supplier Name";
        opt = 1;
    }
    var SupplierInvoiceNumber = $("#txtQuotationNo").val();
    if (SupplierInvoiceNumber === '') {
        txt += "\n - Please Enter Quotation Number";
        opt = 1;
    }
    var InvoiceDt = $("#txtDeliveryDate").val();
    if (InvoiceDt === '') {
        txt += "\n - Please Enter Delivery Date";
        opt = 1;
    }
    var rowCount = $('#tblPurchaseDrugBind tr').length;
    if (rowCount <= 1) {
        txt += "\n - Please Enter Drug Deatils";
        opt = 1;
    }
    var WarehouseId = $("#ddlPOWarehouse").val();
    if (WarehouseId == 0) {
        txt += "\n - Please Select Warehouse";
        opt = 1;
    }
    if (opt == "1") {
        alert(txt);
        return false;
    }
    return true;
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
function BillPrint(response) {
    if (response.PrintHeader.length > 0) {
        var html = "";
        for (PCHeader = 0; PCHeader < response.PrintHeader.length; PCHeader++) {
            var SeqID = response.PrintHeader[PCHeader].SeqID;
            var PurchaseOrderNumber = response.PrintHeader[PCHeader].PurchaseOrderNumber;
            var CreatedDatetime = response.PrintHeader[PCHeader].CreatedDatetime;
            var PH_SupplierName = response.PrintHeader[PCHeader].PH_SupplierName;
            var TotalItem = response.PrintHeader[PCHeader].TotalItem;
            var TotalAmount = response.PrintHeader[PCHeader].TotalAmount;
            var PH_Supplier_Address1 = response.PrintHeader[PCHeader].PH_Supplier_Address1;
            var PH_Supplier_Address2 = response.PrintHeader[PCHeader].PH_Supplier_Address2;
            var PH_Supplier_City = response.PrintHeader[PCHeader].PH_Supplier_City;
            var PH_Supplier_State = response.PrintHeader[PCHeader].PH_Supplier_State;
            var PH_Supplier_Country = response.PrintHeader[PCHeader].PH_Supplier_Country;
            var PH_Supplier_Contact1 = response.PrintHeader[PCHeader].PH_Supplier_Office_Contact1;
            var PH_Supplier_Contact2 = response.PrintHeader[PCHeader].PH_Supplier_Office_Contact2;
            var PH_Supplier_GSTRegNo = response.PrintHeader[PCHeader].PH_Supplier_GSTRegNo;
            var PH_Supplier_Url = response.PrintHeader[PCHeader].PH_Supplier_Url;
            var QuotationNo = response.PrintHeader[PCHeader].QuotationNo;
            var DeliveryDate = response.PrintHeader[PCHeader].DeliveryDate;
            var WarehouseId = response.PrintHeader[PCHeader].WarehouseId;
            var WarehouseName = response.PrintHeader[PCHeader].WarehouseName;
            var UserName = response.PrintHeader[PCHeader].UserName;
            if (PH_Supplier_City != "") {
                PH_Supplier_State = ", " + PH_Supplier_State;
            }
            if (PH_Supplier_State != "") {
                PH_Supplier_Country = ", " + PH_Supplier_Country;
            }

            //Purchase Text
            var Sub = response.PurchaseText[PCHeader].Sub;
            var Body = response.PurchaseText[PCHeader].Body;
            var TINO = response.PurchaseText[PCHeader].TINO;
            var Delivery = response.PurchaseText[PCHeader].Delivery;
            var Disposal = response.PurchaseText[PCHeader].Disposal;

            if (response.Client.length > 0) {
                //Client Deatils
                var ClientName = response.Client[PCHeader].ClientName;
                var Address1 = response.Client[PCHeader].Address1;
                var Address2 = response.Client[PCHeader].Address2;
                var Email = response.Client[PCHeader].Email;
                var PhomeNumber = response.Client[PCHeader].PhomeNumber;
                var GstNo = response.Client[PCHeader].Gst;
                var ARNNo = response.Client[PCHeader].ARNNo;
                var CityName = response.Client[PCHeader].CityName;
                var StateName = response.Client[PCHeader].StateName;
                var CountryName = response.Client[PCHeader].CountryName;

                html += "<hr>";
                html += "<div style='text-align: Center;'>" + ClientName + "</div><br/>";
                html += "<div style='text-align: Center;'>" + Address1 + "</div><br/>";
                html += "<div style='text-align: Center;'>" + Address2 + "</div><br/>";
                html += "<div style='margin-left:10px;float:left'>T.I.No:" + TINO + "</div>";
                html += "<div style='margin-right:10px;float:right;'>GST No:" + GstNo + "</div><br/>";
                html += "<hr>";
            }

            // Header
            html += "<table style='font-family:sans-serif;font-size: 12px;font-weight: 500;margin:10px;'>";
            html += "<tr><td style='width:100px !important;'>Purchase Order</td><td>:</td><td style='width:341px !important;'>" + SeqID + "</td>";
            html += "<td>Date</td><td>:</td><td style='width:341px !important;'>" + CreatedDatetime + "</td></tr></table>";
            html += "<br/>";
            //html += "<span style='float:right;'>GST :" + PH_Supplier_GSTRegNo + "</span><br/>";
            //html += "<span style='margin-left:10px;'>Address</span><br/>";
            html += "<span style='margin-left:10px;'>" + PH_SupplierName + "</span><br/>";
            if (PH_Supplier_Address1 != "") {
                html += "<span style='margin-left:10px;'>" + PH_Supplier_Address1 + "</span><br/>";
            }
            if (PH_Supplier_Address2 != "") {
                html += "<span style='margin-left:10px;'>" + PH_Supplier_Address2 + "</span><br/>";
            }
            if (PH_Supplier_City != "" || PH_Supplier_State != "" || PH_Supplier_Country != "") {
                html += "<span style='margin-left:10px;'>" + PH_Supplier_City + PH_Supplier_State + PH_Supplier_Country + "</span><br/>";
            }
            if (PH_Supplier_Contact1 != "" || PH_Supplier_Contact2 != "") {
                html += "<span style='margin-left:10px;'>" + PH_Supplier_Contact1 + " " + PH_Supplier_Contact2 + "</span><br/>";
            }
            if (PH_Supplier_Url != "" && PH_Supplier_Url != null) {
                html += "<span style='margin-left:10px;'>" + PH_Supplier_Url + "</span><br/>";
            }
            html += "<br/>";
            html += "<br/>";
            html += "<span style='margin-left:10px;'>Sub: " + Sub + "</span><br/><br/>";
            html += "<span style='margin-left:10px;'>Ref: Your Quotation " + QuotationNo + " Date " + DeliveryDate + "</span><br/><br/>";
            html += "<span style='margin-left:10px;'>" + Body + "</span><br/><br/>";


            //Deatils
            html += "<table border='1' style='font-family:sans-serif;font-size: 12px;font-weight: 500;border-collapse:collapse;width: 98%;margin:10px;'><tr>";
            html += "<td style='width: 53px !important;'>S.no</td>";
            html += "<td style='width: 275px !important;'>Particular</td>";
            /* html += " <td align='right' style='width: 53px !important;'>Qty</td>";*/
            html += " <td align='right' style='width: 73px !important;'>Order Qty</td>";
            html += " <td align='right' style='width: 43px !important;'>Uom</td>";
            html += "<td align='right' style='width: 53px !important;'>Rate</td>";
            html += "<td align='right' style='width: 63px !important;'>GST</td>";
            html += "<td align='right' style='width: 63px !important;'>Total</td></tr>";
            var Sno = 0;
            for (PCDetails = 0; PCDetails < response.PrintDeatils.length; PCDetails++) {
                Sno = Sno + 1;
                var BrandName = response.PrintDeatils[PCDetails].BrandName;
                var uom = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_UOM;
                var GST = response.PrintDeatils[PCDetails].GST;
                var orderQty = response.PrintDeatils[PCDetails].OrderStripQty;
                var Cost = response.PrintDeatils[PCDetails].Cost;
                var Qty = response.PrintDeatils[PCDetails].Qty;
                var RowTotalAmount = response.PrintDeatils[PCDetails].TotalAmount;

                html += "<tr><td style='width: 53px !important;'>" + Sno + "</td>";
                html += "<td style='width: 109px !important;'>" + BrandName + "</td>";
                /* html += "<td style='width: 53px !important;text-align: right;'>" + Qty + "</td>";*/
                html += "<td style='width: 53px !important;text-align: right;'>" + orderQty + "</td>";
                html += "<td style='width: 53px !important;text-align: right;'>" + uom + "</td>";
                html += "<td style='width: 140px !important;text-align: right;'>" + Cost + "</td>";
                html += "<td style='width: 275px !important;text-align: right;'>" + GST + "</td>";
                html += "<td align='right' style='width: 53px !important;text-align: right;'>" + RowTotalAmount + "</td>";
                html += "</tr>";
            }
            html += "</table><br/>";
            //Amount Details
            html += "<span style='margin-right: 21px;float: right;'>Net Total:" + TotalAmount + "</span><br/>";

            //html += "<span style='margin-left:10px;'>Our T.I.No:" + TINO + "</span><br/><br/>";
            html += "<div style='margin-left:10px;'>Delivery Date:" + DeliveryDate + "</div><br/>";
            html += "<div style='margin-left:10px;'>Delivery Address:" + WarehouseName + "</div><br/><br/>";
            html += "<div style='margin-left:10px;'>Disposal of L.R " + Disposal + "</div><br/>";
            html += "<div style='margin-left:10px;'>For " + WarehouseName + "</div><br/><br/>";
            html += "<div style='margin-left:10px;'>Authorised Signatory</div><br/>";
            html += "<span style='margin-left:10px;'>" + UserName + "</span><br/>";

        }
        sessionStorage.setItem("PrintDetails", html);
        PurchaseopenRequestedPopup();
    }
}
function PurchaseopenRequestedPopup() {
    var rootUrl = "http://" + document.location.hostname + ":" + window.location.port;
    var url = rootUrl + '/Pharmacy/Dispense/print';
    window.open(url, '_blank');
    sessionStorage.setItem("PrintDetails", "");
}
function SelectedReprint(BillNo) {
    var BillNo = parseFloat(BillNo);
    $.ajax({
        url: "/api/PurchaseApi/GetSelectedPurchaseOrder",
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
function GetSelectedPurchaseOrder() {
    var SupplierID = parseInt($("#hidSupplierID").val());
    $.ajax({
        url: "/api/PurchaseApi/GetPurchaseOrderDrugBySupplierID",
        type: "GET",
        data: {
            SupplierID: SupplierID
        },
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            $("#tblPurchaseDrugBind tbody").empty();
            var html = "";
            for (PCDetails = 0; PCDetails < response.PrintDeatils.length; PCDetails++) {
                var DrugCode = response.PrintDeatils[PCDetails].DrugCode;
                var BrandName = response.PrintDeatils[PCDetails].BrandName;
                var GST = response.PrintDeatils[PCDetails].GST;
                var Cost = response.PrintDeatils[PCDetails].Cost;
                var PH_ITEM_HSNCODE = response.PrintDeatils[PCDetails].PH_ITEM_HSNCODE;
                var PH_ITEM_DRUG_GENERIC = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_GENERIC;
                var PH_ITEM_DRUG_UOM = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_UOM;
                var PH_ITEM_DRUG_QTY = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_QTY;
                var PH_CUR_STOCK_PURCHCOST = parseFloat(Cost) * parseFloat(PH_ITEM_DRUG_QTY);
                GetCurretntStockByDrugCode(DrugCode);
            }
            $("#tblPurchaseDrugBind tbody").append(html);
        },
        complete: function () { $("#loading").css("display", "none"); }
    });

    return false;
    //  $('#modal-default').modal().hide();
}
function DeletePurchaseRow(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var myrow = selectedrow.parentNode.parentNode;
    document.getElementById("tblPurchaseDrugBind").deleteRow(myrow.rowIndex);
    //var table = document.getElementById("tblPurchaseDrugBind");
    //var rowCount = table.rows.length;
    //var i = myrow.rowIndex;
    //regroup(i, rowCount, "tblPurchaseDrugBind");
    TotalPurchaseCalculation();
}
function regroup(i, rc, ti) {
    for (j = (i + 1); j < rc; j++) {
        if (j > 0) {
            document.getElementById(ti).rows[j].cells[0].innerHTML = j;
        }
    }
}
function OpenPurchase() {
    GetPurchaseOrderTop100();
    $("#ModelPurchaseOrder").dialog(
        {
            title: "Re-Print",
            width: 906,
            height: 500,
            modal: true,
            buttons: {
                "Cancel": function () {
                    $("#ModelPurchaseOrder").dialog("close");
                }
            }
        });
}
function GetPurchaseOrderTop100() {
    $.ajax({
        url: "/api/PurchaseApi/GetPurchaseOrderTop100",
        type: "GET",
        dataType: "json",
        beforeSend: function () { $("#loading").css("display", "block"); },
        success: function (response) {
            if (response.length > 0) {
                var html = "";
                var Sno = 0;
                $("#tblPurchaseOrderBind tbody").empty();
                for (PCHeader = 0; PCHeader < response.length; PCHeader++) {
                    Sno = PCHeader + 1;
                    var SeqID = response[PCHeader].SeqID;
                    var PurchaseOrderNumber = response[PCHeader].PurchaseOrderNumber;
                    var PH_SupplierName = response[PCHeader].PH_SupplierName;
                    var CreatedDatetime = response[PCHeader].CreatedDatetime;
                    var TotalItem = response[PCHeader].TotalItem;
                    var TotalAmount = response[PCHeader].TotalAmount;
                    var IsAprovedBy = response[PCHeader].IsAprovedBy;
                    if (IsAprovedBy === false) {
                        html += "<tr><td>" + Sno + "</td>";//0
                        html += "<td style='display:none;'>" + SeqID + "</td>";//1
                        html += "<td>" + PurchaseOrderNumber + "</td>";//2
                        html += "<td>" + PH_SupplierName + "</td>";//3
                        html += "<td>" + CreatedDatetime + "</td>";//4
                        html += "<td>" + TotalItem + "</td>";//5
                        html += "<td>" + TotalAmount + "</td>";//6
                        html += "<td>Not Approved</td>";//7
                        html += "<td><button type='button' onclick='return SelectOrderBind(this)'>Select</button></td>";//8
                        html += "</tr>";
                    }
                }
                $("#tblPurchaseOrderBind tbody").append(html);
            }
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
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
function GetPurchaseOrderDrugBySupplierID(selectedrow) {
    var BillNo = parseFloat(selectedrow.cells[0].innerHTML);
    $.ajax({
        url: "/api/PurchaseApi/GetPurchaseOrderDrugBySupplierID",
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
function SelectOrderBind(selectedrow) {
    var row = selectedrow.parentNode.parentNode;
    var dtlSeqID = parseFloat(row.cells[1].innerHTML);
    var Status = row.cells[7].innerHTML;
    if (Status === 'Not Approved') {
        GetPurchaseOrderDrugBySeqID(dtlSeqID);
    }
    else {
        SelectedReprint(dtlSeqID);
    }
}
function GetPurchaseOrderDrugBySeqID(SeqID) {
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
            $("#tblPurchaseDrugBind tbody").empty();
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
                var SupplierID = response.PrintHeader[PCHeader].SupplierID;
                var WarehouseId = response.PrintHeader[PCHeader].WarehouseId;
                var WarehouseName = response.PrintHeader[PCHeader].WarehouseName;
                $("#hidSupplierID").val(SupplierID);
                $("#txtQuotationNo").val(QuotationNo);
                $("#txtDeliveryDate").val(DeliveryDate);
                $("#lblSupplierName").html(PH_SupplierName);
                $("#hidPoNumber").val(PurchaseOrderNumber);
                $("#hidHeadSeqID").val(SeqID);
                $("#ddlPOWarehouse").val(WarehouseId);
            }
            for (PCDetails = 0; PCDetails < response.PrintDeatils.length; PCDetails++) {
                var DrugCode = response.PrintDeatils[PCDetails].DrugCode;
                var BrandName = response.PrintDeatils[PCDetails].BrandName;
                var GST = response.PrintDeatils[PCDetails].GST;
                var Cost = response.PrintDeatils[PCDetails].Cost;
                var PH_ITEM_HSNCODE = response.PrintDeatils[PCDetails].PH_ITEM_HSNCODE;
                var PH_ITEM_DRUG_GENERIC = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_GENERIC;
                var Qty = parseInt(response.PrintDeatils[PCDetails].Qty);
                var OrderStripQty = parseInt(response.PrintDeatils[PCDetails].OrderStripQty);
                var AvgPurCost = 0;
                var PH_ITEM_DRUG_UOM = response.PrintDeatils[PCDetails].PH_ITEM_DRUG_UOM;
                var PH_ITEM_DRUG_QTY = parseFloat(response.PrintDeatils[PCDetails].PH_ITEM_DRUG_QTY).toFixed(2);
                var TotalStripCost = parseFloat(Cost) * parseFloat(PH_ITEM_DRUG_QTY);
                var TotalStripQty = OrderStripQty / parseFloat(PH_ITEM_DRUG_QTY);

                var Tax = parseFloat(GST).toFixed(2);
                var amt = parseFloat(Cost * OrderStripQty).toFixed(2);
                var vatamt = parseFloat((amt * Tax) / 100).toFixed(2);
                var total = parseFloat(amt) + parseFloat(vatamt);
                total = total.toFixed(2);
                var $tr = $('#tblPurchaseDrugBind tr[data-id="' + DrugCode + '"]');
                if ($tr.length === 0) {
                    html += "<tr data-id=\"" + DrugCode + "\">";
                    html += "<td style='display:none;'>" + DrugCode + "</td>";//0
                    html += "<td>" + BrandName + "</td>";//1
                    html += "<td style='display:none;'>" + PH_ITEM_DRUG_GENERIC + "</td>";//2
                    html += "<td>" + 0 + "</td>";//3
                    html += "<td>" + PH_ITEM_HSNCODE + "</td>";//4
                    html += "<td>" + GST + "</td>";//5
                    html += "<td>" + AvgPurCost + "</td>";//6
                    html += "<td> " + PH_ITEM_DRUG_UOM + " <input value='" + PH_ITEM_DRUG_QTY + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'></td>";//7
                    html += "<td><input value='" + OrderStripQty + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'></td>";//8
                    html += "<td>" + Qty + "</td>";//9
                    html += "<td><input value='" + Cost.toFixed(2) + "'style='height:28px;width:50px;text-align: right;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'></td>";//10
                    html += "<td class='TaxCell'>" + vatamt + "</td>";//11
                    html += "<td class='AmountCell'>" + total + "</td>";//12
                    //html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";//11
                    html += "<td style='text-align: center;'>";
                    html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeletePurchaseRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                    html += "</tr>";
                }
            }
            $("#tblPurchaseDrugBind tbody").append(html);
            TotalPurchaseCalculation();
            $("#ModelPurchaseOrder").dialog("close");
        },
        complete: function () { $("#loading").css("display", "none"); }
    });
    return false;
}
function PurchaseViewCancel() {
    $("#hidSupplierID").val('');
    $("#txtSearchPoList").val('');
    $("#txtQuotationNo").val('');
    $("#txtDeliveryDate").val('');
    $("#hidHeadSeqID").val(0);
    $("#tblPurchaseDrugBind tbody").empty();
    $("#txtTotalamt").val('');
}
function GotoLogin() {
    try {
        window.location.href = "/Login/Login";
    }
    catch (e) {
    }
}
function PurUpdateGSTChange() {
    var DrugCode = parseInt($("#hidPurSeqID").val());
    var GST = parseFloat($("#txtPurPopUpGst").val());
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
                /*       var table = document.getElementById("tblPurchaseDrugBind");*/
                var html = "";
                var Row = document.getElementById("tblPurchaseDrugBind");
                for (var i = 1; i < Row.rows.length; i++) {
                    var PH_CUR_DRUGCODE = Row.rows[i].cells[0].innerHTML;
                    var DrugName = Row.rows[i].cells[1].innerHTML;
                    var PH_ITEM_DRUG_GENERIC = Row.rows[i].cells[2].innerHTML;
                    var PH_CUR_STOCK = Row.rows[i].cells[3].innerHTML;
                    var PH_ITEM_HSNCODE = Row.rows[i].cells[4].innerHTML;
                    var PH_ITEM_DRUG_VAT = Row.rows[i].cells[5].innerHTML;
                    var a = Row.rows[i].cells[6].innerHTML;
                    var PH_ITEM_DRUG_QTY = Row.rows[i].cells[7].getElementsByTagName("input")[0].value;
                    var PH_ITEM_DRUG_UOM = Row.rows[i].cells[7].innerText;
                    var PH_ITEM_DRUG_UOM = Row.rows[i].cells[8].innerText;
                    var PH_ITEM_DRUG_ORDER_QTY = Row.rows[i].cells[8].getElementsByTagName("input")[0].value;
                    var PH_ITEM_TOTAL_UNIT = Row.rows[i].cells[9].innerHTML;
                    var PH_ITEM_TAX  = Row.rows[i].cells[11].innerHTML;
                    var PH_ITEM_TOTAL_PRICE = Row.rows[i].cells[12].innerHTML;
                    var PH_CUR_STOCK_PURCHCOST = Row.rows[i].cells[10].getElementsByTagName("input")[0].value;
                    var code = parseFloat(PH_CUR_DRUGCODE);
                    if (DrugCode == code) {
                        var gst = $("#txtPurPopUpGst").val();
                       
                        html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\">";
                        html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//0
                        html += "<td>" + DrugName + "</td>";//1
                        html += "<td style='display:none;'>" + PH_ITEM_DRUG_GENERIC + "</td>";//2
                        html += "<td>" + PH_CUR_STOCK + "</td>";//3
                        html += "<td>" + PH_ITEM_HSNCODE + "</td>";//4
                        html += "<td>" + gst + "</td>";//5
                        html += "<td>0</td>";//6
                        html += "<td> <input value='" + PH_ITEM_DRUG_QTY + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//7
                        html += "<td><input style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//8
                        html += "<td></td>";//9
                        html += "<td><input value='" + PH_CUR_STOCK_PURCHCOST + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'></td>";//10
                        html += "<td class='TaxCell'></td>";//11
                        html += "<td class='AmountCell'></td>";//12
                        //html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";//11
                        html += "<td style='text-align: center;'>";
                        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeletePurchaseRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                        html += "</tr>";
                        


                    }
                    else {
                        html += "<tr data-id=\"" + PH_CUR_DRUGCODE + "\">";
                        html += "<td style='display:none;'>" + PH_CUR_DRUGCODE + "</td>";//0
                        html += "<td>" + DrugName + "</td>";//1
                        html += "<td style='display:none;'>" + PH_ITEM_DRUG_GENERIC + "</td>";//2
                        html += "<td>" + PH_CUR_STOCK + "</td>";//3
                        html += "<td>" + PH_ITEM_HSNCODE + "</td>";//4
                        html += "<td>" + PH_ITEM_DRUG_VAT + "</td>";//5
                        html += "<td>0</td>";//6
                        html += "<td> <input value='" + PH_ITEM_DRUG_QTY + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//7
                        html += "<td><input value='" + PH_ITEM_DRUG_ORDER_QTY +"'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberKey(event)'> " + PH_ITEM_DRUG_UOM + "</td>";//8
                        html += "<td>" + PH_ITEM_TOTAL_UNIT + "</td>";//9
                        html += "<td><input value='" + PH_CUR_STOCK_PURCHCOST + "'style='height:28px;width:50px;text-align: right;margin-bottom:0px;' type=\"Text\" onchange='PurchaseQtyValu(this)' onkeypress='return isNumberAndDecimal(this,event)'></td>";//10
                        html += "<td class='TaxCell'>" + PH_ITEM_TAX + "</td>";//11
                        html += "<td class='AmountCell'>" + PH_ITEM_TOTAL_PRICE + "</td>";//12
                        //html += "<td style='text-align: center;'><input type='checkbox' class='case'  id='Check' style='text-align: center;' /></td>";//11
                        html += "<td style='text-align: center;'>";
                        html += "<img src='" + rootUrl + "/Images/DELETE-32.png'  onclick='return DeletePurchaseRow(this)' style='cursor:pointer;width: 30px !important;height:30px !important;'></td>";
                        html += "</tr>";
                        
                    }
                   
                }
                $("#tblPurchaseDrugBind tbody").empty();
                $("#tblPurchaseDrugBind tbody").append(html);
                $("#txtPurPopUpGst").val('');
            }
        },

        complete: function () { $("#loading").css("display", "none"); }
    });
}

