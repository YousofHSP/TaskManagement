using AutoMapper;
using Data.Contracts;
using Entity;
using Entity.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class CustomerController(IRepository<Customer> repository, IMapper mapper) : BaseController<CustomerDto,CustomerResDto, Customer>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetIncludes("Parent", "Plan");

        if (method is "create" or "edit")
        {
            List<SelectListItem> res;
            if (Model is not null)
                res = await repository.GetSelectListItems(whereFunc: i => i.Id != Model.Id, ct: ct);
            else
                res = await repository.GetSelectListItems(ct: ct);

            var selectedParent = Model?.ParentId;
            if (selectedParent != null)
                foreach (var p in res.Where(p => p.Value == selectedParent.ToString()))
                    p.Selected = true;
            AddOptions(nameof(CustomerDto.ParentId),res);
        }
    }
}