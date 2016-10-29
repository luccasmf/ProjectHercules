/// <reference path="Util.js" />
$(document).ready(function () {
    // Util
    bindDatatable();
    bindFormFilter();
    bindCancelar();
    bindCadastro();

    bindSubmitDocumento();
    bindFiltro();
});

function bindSubmitDocumento() {
    $(document).on("submit", '.frm-submit', function () {
        var frm = $('.frm-submit');
        $.ajax({
            url: frm.attr("action"),
            cache: false,
            contentType: false,
            processData: false,
            type: frm.attr("method"),
            data: new FormData(this),
            success: function (result) {
                if (result.Status == true)
                    showNotificationRefresh(result, false, true);
                else
                    showNotification(result);
            },
            error: function (result) {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            }
        });
        return false;
    });
}

function bindFiltro() {
    $(document).on("change", "#apenasPendentes", function () {
        var apenasPendentes = $(this).prop("checked");
        var url = $(this).attr("url");
        $.ajax({
            url: url,
            type: 'post',
            data: { "apenasPendentes": apenasPendentes },
            success: function (result) {
                $(".partialList").html(result);
                bindDatatable();
            },
            error: function () {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                };
                showNotification(obj);
            }
        });
    });

}