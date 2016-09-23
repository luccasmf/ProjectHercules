$(document).ready(function () {
    bindDatatable();
    bindCadastro();
    bindExclusao();

    bindCamposModal();
});

function bindDatatable() {
    $('.datatable').dataTable({
        "sDom": "<'row'<'col-lg-6'l><'col-lg-6'f>r>t<'row'<'col-lg-12'i><'col-lg-12 center'p>>",
        "sPaginationType": "bootstrap",
        "iDisplayLength": "5",
        "oLanguage": {
            "semptyTable": "",
            "sinfo": "Mostrando _START_ até _END_ de _TOTAL_ registros",
            "sinfoEmpty": "",
            "sinfoFiltered": "(Filtrado de _MAX_ registros)",
            "ssearch": "Buscar:",
            "szeroRecords": "Nenhum registro encontrado",
            "spaginate": {
                "first": "Primeiro",
                "last": "Último",
                "next": "Próximo",
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
                if (result.Status == true) {
                    var n = noty({
                        text: result.Message,
                        layout: "topCenter",
                        type: "success"
                    });
                    $("#divModalGlobal").modal("hide")
                }
                else {
                    var n = noty({
                        text: result.Message,
                        layout: "topCenter",
                        type: "error"
                    });
                }
            },
            error: function (result) {
                alert("Failed");
            }
        });
        return false;
    });
}