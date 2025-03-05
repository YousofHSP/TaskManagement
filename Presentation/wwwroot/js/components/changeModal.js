
let index = 0
$(".open-modal").click(function () {
    let rowId = $(this).data("id")
    $.ajax({
        url: "/Job/GetInfo/",
        data: {id: rowId},
        success: result => {
            $("#editModal [name=StartedAt]").val(result.startedAt)
            $("#editModal [name=EndedAt]").val(result.endedAt)
            $("#editModal [name=Status]").val(result.status).trigger("change")
            $(".sub-jobs").html("")
            index = 0
            result.subJobs.forEach(item => {
                let el =`<div class="row">
                    <input type="hidden" name="SubJobs[${index}][Id]" value="${item.id}">
                    <div class="col-3 form-group">
                        <label class="form-label">عنوان</label>
                        <input name="SubJobs[${index}][Title]" class="form-control" value="${item.title}">
                    </div>
                    <div class="col-3 form-group">
                        <label class="form-label">تاریخ شروع</label>
                        <input name="SubJobs[${index}][StartedAt]" class="form-control date-picker" autocomplete="off" value="${item.startedAt}">
                    </div>
                    <div class="col-3 form-group">
                        <label class="form-label">تاریخ پایان</label>
                        <input name="SubJobs[${index}][EndedAt]" class="form-control date-picker" autocomplete="off" value="${item.endedAt}">
                    </div>
                    <div class="col-3 form-group">
                        <label class="form-label">وضعیت</label>
                        <select name="SubJobs[${index}][Status]" class="form-control">
                            <option ${item.status == 0 ? "selected" : ""} value="0">درانتظار</option>
                            <option ${item.status == 1 ? "selected" : ""} value="1">درحال پردازش</option>
                            <option ${item.status == 2 ? "selected" : ""} value="2">انجام شده</option>
                        </select>
                    </div>
                </div>`;
                $(".sub-jobs").append(el)
                index++;
            })
            $(".sub-jobs .date-picker").pDatepicker(datePickerConfigs)
            $("#editModal").modal("show");
        }
    })
    $(".save").click(function () {
        let startAt = $("#editModal [name=StartedAt]").val()
        let endedAt = $("#editModal [name=EndedAt]").val()
        let status = $("#editModal [name=Status]").val()
        let subJobs = []
        for(let i = 0; i < index; i++) {
            let subJob = {
                Id: $(`[name='SubJobs[${i}][Id]']`).val(),
                Title: $(`[name='SubJobs[${i}][Title]']`).val(),
                StartedAt: $(`[name='SubJobs[${i}][StartedAt]']`).val(),
                EndedAt: $(`[name='SubJobs[${i}][EndedAt]']`).val(),
                Status: $(`[name='SubJobs[${i}][Status]']`).val(),
            }
            subJobs.push(subJob)
        }
        
        $.ajax({
            url: "/Job/QuickUpdate/",
            method: "post",
            data: {Id: rowId, StartedAt: startAt, EndedAt: endedAt, Status: status, SubJobs: subJobs},
            success: result => {
                $("#editModal").modal("hide");
                location.reload();
            }
        })
    })
})
    