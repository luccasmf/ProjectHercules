/// <reference path="Util.js" />
$(document).ready(function () {
    bindExclusao();

    bindDatatable();
    bindFormFilter();


    $(document).on("submit", '#frmDoc', function () {
        var frm = $('#frmDoc');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                if (result.Status == true)
                    showNotificationRedirect(result, true);
                else
                    showNotification(result);
            },
            error: function (result) {
                var obj = new {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            }
        });
        return false;
    });

});

