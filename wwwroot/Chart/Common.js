jQuery.extend({
    getValues: function (url) {
        var result = null;
        $.ajax({
            url: url,
            type: 'get',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                result = data;
            },
           
        });
        return result;
    },
    postValues: function (url,jdata) {
        var result = null;
        $.ajax({
            url: url,
            type: 'post',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(jdata),
            dataType: "json",
            async: false,
            success: function (data) {
                result = data;
            }
        });
        return result;
    }
});