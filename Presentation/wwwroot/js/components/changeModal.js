
$(".open-modal").click(function () {
    let rowId = $(this).data("id")
    $.ajax({
        url: "/Job/GetInfo/",
        data: {id: rowId},
        success: result => {
            $("#editModal [name=StartedAt]").val(result.startedAt)
            $("#editModal [name=EndedAt]").val(result.endedAt)
            $("#editModal [name=Status]").val(result.status).trigger("change")
            $("#editModal").modal("show");
        }
    })
    $(".save").click(function () {
        let startAt = $("#editModal [name=StartedAt]").val()
        let endtedAt = $("#editModal [name=EndedAt]").val()
        let status = $("#editModal [name=Status]").val()
        $.ajax({
            url: "/Job/QuickUpdate/",
            data: {Id: rowId, StartedAt: startAt, EndedAt: endtedAt, Status: status},
            success: result => {
                $("#editModal").modal("hide");
                location.reload();
            }
        })
    })
})
    