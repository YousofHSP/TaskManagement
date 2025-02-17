using AutoMapper;
using Data.Contracts;
using Entity;
using Entity.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class CustomerController(IRepository<Customer> repository, IRepository<Plan> planRepository, IMapper mapper) : BaseController<CustomerDto,CustomerResDto, Customer>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetTitle("مشتریان");
        SetIncludes("Parent", "Plan");

        if (method is "create" or "edit")
        {
            AddField("Title", "عنوان");
            var customerItems = repository.TableNoTracking
                .Where(i => i.ParentId == null)
                .AsQueryable();
            
            if (Model is not null)
                customerItems = customerItems.Where(i => i.Id != Model.Id);
            var res = await customerItems
                .ToListAsync(ct);
            AddField("ParentId", "والد", FieldType.Select, Model?.ParentId?.ToString() ?? "",res.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList());
            var plansItem = await planRepository.TableNoTracking.ToListAsync(ct);
            AddField("PlanId", "پلن", FieldType.Select, Model?.PlanId.ToString() ?? "",plansItem.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList());
        }
    }
}