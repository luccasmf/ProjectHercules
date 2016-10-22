/// <reference path="Util.js" />
$(document).ready(function () {
    bindCadastro();
    bindCamposModal();

    //Util
    bindFormFilter();
    bindFormSubmitModal();
    bindDatatable();
    bindExclusao();
    bindFormSubmitExclusao();
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
    })
}