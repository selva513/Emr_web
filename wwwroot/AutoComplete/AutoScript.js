
//newly added by Siva
$(document).ready(function () {    
    SearchText();
  
});
function SearchText() {
    $("#txtSearch").autocomplete({
        source: function (request, response) {
            
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "ColonFinding.aspx/BindFindingName",
                data: "{'EMR_FINDINGS_desc':'" + document.getElementById('txtSearch').value + "'}",
                //data: "{}",
                dataType: "json",
                dataFilter: function (data) { return data; },
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            value: item.EMR_FINDINGS_desc

                        }
                    }))
                },

                error: function (result) {
                    alert("Error");
                }
                
            });
        }
    });
}



//End







