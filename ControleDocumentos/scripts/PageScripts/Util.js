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

function bindDatatable() {
    $(".commmon-datatable").DataTable({
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
