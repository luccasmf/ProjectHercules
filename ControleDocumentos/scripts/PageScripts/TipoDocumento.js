/// <reference path="Util.js" />
$(document).ready(function () {
    bindDatatable();
    bindCadastro();
    bindExclusao();

    bindCamposModal();
});

function bindDatatable() {
    $('#tipoDocTable').DataTable({
        "language": {
            "sEmptyTable": "Nenhum registro encontrado",
            "sInfo": "Mostrando de _START_ atÃ© _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 atÃ© 0 de 0 registros",
            "sInfoFiltered": "(Filtrados de _MAX_ registros)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "_MENU_ resultados por pÃ¡gina",
            "sLoadingRecords": "Carregando...",
            "sProcessing": "Processando...",
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar",
            "oPaginate": {
                "sNext": "PrÃ³ximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeiro",
                "sLast": "Ãšltimo"
            },
            "oAria": {
                "sSortAscending": ": Ordenar colunas de forma ascendente",
                "sSortDescending": ": Ordenar colunas de forma descendente"
            }
        }
    });
}

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
                alert("erro")
            }
        });
    });
}

function bindExclusao() {
    $(document).on("click", ".btnExclusao", function () {
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            success: function (data) {
                $("#divModalGlobalBody").html(data);
                $("#divModalGlobal").modal("show");
            },
            error: function () {
                alert("erro")
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
                showNotificationRedirect(result);
                //if (result.Status == true) {
                //    var n = noty({
                //        text: result.Message,
                //        layout: "topCenter",
                //        type: "success"
                //    });
                //    $("#divModalGlobal").modal("hide")
                //}
                //else {
                //    var n = noty({
                //        text: result.Message,
                //        layout: "topCenter",
                //        type: "error"
                //    });
                //}
            },
            error: function (result) {
                var n = noty({
                    text: result.Message,
                    layout: "topCenter",
                    type: "error"
                });
            }
        });
        return false;
    });
}