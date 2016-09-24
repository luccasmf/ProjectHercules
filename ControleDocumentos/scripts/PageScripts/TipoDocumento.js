/// <reference path="Util.js" />
$(document).ready(function () {
    bindDatatable();
    bindCadastro();
    bindExclusao();

    bindCamposModal();
    bindFormFilter();
});

function bindDatatable() {
    $('#tipoDocTable').DataTable({
        "language": {
            "sEmptyTable": "Nenhum registro encontrado",
            "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
            "sInfoFiltered": "(Filtrados de _MAX_ registros)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "_MENU_ resultados por página",
            "sLoadingRecords": "Carregando...",
            "sProcessing": "Processando...",
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar",
            "oPaginate": {
                "sNext": "Próximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeiro",
                "sLast": "Último"
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
                if (result.Status == true)
                    showNotificationReloadFilter(result,true);
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