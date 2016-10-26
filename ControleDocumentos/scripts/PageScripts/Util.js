$(document).ready(function () {
    $.datepicker.setDefaults(
        $.extend(
        { 'dateFormat': 'dd-mm-yyyy' },
        $.datepicker.regional['pt-BR']
        )
    );
})

// Início Notification

function showNotificationRefresh(notification, hideModal, hideForm) {
    if (hideModal == true)
        $("#divModalGlobal").modal("hide");

    if (hideForm) {
        $(".divFormSave").hide();
        $(".divList").show();
    }

    $('.form-filter').trigger('submit', function () {
        bindFormFilter();
    });

    var obj = {
        Type: notification.Type,
        Message: notification.Message
    }
    showNotification(obj);
}

function showNotificationRedirect(notification) {
    $.ajax({
        url: notification.ReturnUrl,
        type: 'GET',
        success: function (data) {
            $("body").html(data);
            var obj = {
                Type: notification.Type,
                Message: notification.Message
            }
            showNotification(obj);
        },
        error: function () {
            var obj = {
                Type: "error",
                Message: "Ocorreu um erro ao realizar esta operação"
            }
            showNotification(obj);
        }
    });
}

function showNotification(notification) {
    var n = noty({
        text: notification.Message,
        layout: "topCenter",
        type: notification.Type
    });
};

// Fim Notification

/*--------------------------------------------------------*/

// Início Submit

function bindFormSubmitModal() {
    $(document).on("submit", ".frm-submit", function (e) {
        e.preventDefault();
        var frm = $('.frm-submit');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                if (result.Status == true)
                    showNotificationRefresh(result, true, false);
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

function bindFormSubmit() {
    $(document).on("submit", ".frm-submit", function (e) {
        e.preventDefault();
        var frm = $('.frm-submit');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
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

function bindFormSubmitExclusao() {
    $(document).on("submit", '.frmSubmitExclusao', function (e) {
        e.preventDefault();
        var frm = $('.frmSubmitExclusao');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                if (result.Status == true)
                    showNotificationRefresh(result, true, false);
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

// Fim Submit

/*--------------------------------------------------------*/

// Início List

function bindFormFilter() {
    $(document).on('submit', '.form-filter', function (e) {
        e.preventDefault();
        var frm = $('.form-filter');
        $.ajax({
            url: frm.attr("action"),
            type: frm.attr("method"),
            data: frm.serialize(),
            success: function (result) {
                $(".partialList").html(result);
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

// Fim List

/*--------------------------------------------------------*/

// Início Form Binds

function bindExclusao() {
    $(document).on("click", ".btnExclusao", function (e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            success: function (data) {
                $("#divModalGlobalBody").html(data);
                $("#divModalGlobal").modal("show");
            },
            error: function () {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            }
        });
    });
}

function bindCadastro() {
    $(document).on("click", ".btnCadastro", function () {
        $.ajax({
            url: $(this).attr("url"),
            type: 'GET',
            success: function (data) {
                $(".formSave").html(data);
                $(".divFormSave").show();
                $(".divList").hide();

                $('.datepicker').datepicker();
            },
            error: function () {
                var obj = {
                    Type: "error",
                    Message: "Ocorreu um erro ao realizar esta operação"
                }
                showNotification(obj);
            }
        });
    });
}

function bindCancelar() {
    $(document).on("click", ".btnCancelar", function () {
        $(".divFormSave").hide();
        $(".divList").show();
    });
}

// Fim Form Binds