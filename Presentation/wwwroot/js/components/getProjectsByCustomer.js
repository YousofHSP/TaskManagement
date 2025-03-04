$("[name=CustomerId]").change(function(){
    let projectEl = $("[name=ProjectId]")
    let customerId = $(this).val()
    let projectId = projectEl.val()
    projectEl.html("")
    $.ajax({
        url: "/Project/GetByCustomer/",
        data: {id: customerId},
        method: "get",
        success: res => {
            projectEl.html("")
            res.forEach(item => {
                projectEl.append(`<option value="${item.value}">${item.text}</option>`)
            })
        }
    })
})