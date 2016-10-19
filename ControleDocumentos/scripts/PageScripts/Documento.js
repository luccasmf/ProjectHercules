/// <reference path="Util.js" />
$(document).ready(function () {
    bindExclusao();
    bindDatatable();
    bindFormFilter();
    bindGetAlunos();
    bindFormSubmitExclusao();

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
                    showNotificationRedirect(result, true);
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
});

function bindGetAlunos() {
    $(document).on("change", "#ddlCurso", function () {
        var idCurso = $(this).val();
        if (idCurso > 0) {
            var url = $(this).attr("url");
            $.ajax({
                url: url,
                type: 'post',
                data: { "idCurso": idCurso },
                success: function (data) {
                    var options = $("#ddlAluno");
                    options.empty();
                    $.each(data, function () {
                        options.append($("<option />").val(this.Value).text(this.Text));
                    });
                },
                error: function () {
                    var obj = {
                        Type: "error",
                        Message: "Ocorreu um erro ao realizar esta operação"
                    };
                    showNotification(obj);
                }
            });
        }
        else {
            var options = $("#ddlAluno");
            options.empty();
            options.append($("<option />").val("").text("Selecione um curso"))
        }
    });
}
