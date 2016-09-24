function showNotificationRedirect(notification) {
    var type = notification.Type;
    var text = notification.Message;


    $.ajax({
        url: notification.ReturnUrl,
        type: "get",
        success: function (result) {
            $("body").html(result);

            var n = noty({
                text: text,
                layout: "topCenter",
                type: type
            });

        },
        error: function (result) {
            var n = noty({
                text: result.Message,
                layout: "topCenter",
                type: "error"
            });
        },
        async: true
    });
};

function showNotification() {

};
