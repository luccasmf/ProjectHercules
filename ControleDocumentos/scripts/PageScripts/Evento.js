/// <reference path="Util.js" />
$(document).ready(function () {
    // Util
    bindShowConfirmacao();
    bindDatatable();
    bindFormFilter();
    bindCancelar();
    bindFormSubmit();
    bindCadastro();
    bindFormSubmitModal();

    bindChamada();
    bindCancelarChamada();
    bindGerarCertificados();
});

function bindChamada() {
    $(document).on("click", ".btnChamada", function () {
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            beforeSend: function () {
                showLoader();
            },
            success: function (data) {
                if (data == "") {
                    var obj = {
                        Type: "error",
                        Message: "Não autorizado!"
                    }
                    showNotification(obj);
                }
                else {
                    $(".formChamada").html(data);
                    $(".divFormChamada").show();
                    $(".divList").hide();
                }
            },
            error: function () {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            },
            complete: function () {
                hideLoader();
            }
        });
    });

}

function bindCancelarChamada() {
    $(document).on("click", ".btnCancelarChamada", function () {
        $(".divFormChamada").hide();
        $(".divList").show();
    });
}

function bindGerarCertificados() {
    $(document).on("click", ".btnGerarCertificados", function () {
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            beforeSend: function () {
                showLoader();
            },
            success: function (data) {
                showNotification(result);
            },
            error: function () {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            },
            complete: function () {
                hideLoader();
            }
        });
    });

}