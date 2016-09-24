function bindFormFilter() {
    $('.form-filter').submit(function () {
        var frm = $('.form-filter');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                $(".partialList").html(result);
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
}

function showNotificationReloadFilter(notification, hideModal) {
    if (hideModal != undefined && hideModal != null && hideModal == true) {
        $("#divModalGlobal").modal("hide");
    }

    $(".form-filter").submit(function () {
        bindFormFilter();
    });
    var n = noty({
        text: notification.Message,
        layout: "topCenter",
        type: notification.Type,
    })
    
};

function showNotification(notification) {
    var n = noty({
        text: notification.Message,
        layout: "topCenter",
        type: notification.Type
    });
};
