/// <reference path="Util.js" />
$(document).ready(function () {
    bindCadastro();
    bindExclusao();
    bindCamposModal();

    bindDatatable();
    bindFormFilter();
});

function bindCadastro() {
    $(document).on("click", ".btnCadastro", function () {
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            success: function (data) {
                $("#divModalGlobalBody").html(data);
                $("#divModalGlobal").modal("show");
            },
            error: function () {
                var obj = new {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            }
        });
    });
}

function bindCamposModal() {
    $('.modal').on('shown.bs.modal', function () {
        $("#TipoDocumento1").focus();
        bindFormSubmit();
    })
}

function bindFormSubmit() {
    $('#frmTipoDoc').submit(function () {
        var frm = $('#frmTipoDoc');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                if (result.Status == true)
                    showNotificationModal(result,true);
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
}