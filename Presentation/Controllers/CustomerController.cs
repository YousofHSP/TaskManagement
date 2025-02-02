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
        base.Configure(method, ct);
        SetTitle("مشتریان");
        SetIncludes("Parent");
        AddColumn("عنوان", "Title");
        AddColumn("والد", "ParentTitle");



        if (method is "create" or "edit")
        {
            var items = repository.TableNoTracking
                .AsQueryable();
            
            if (Model is not null)
                items = items.Where(i => i.Id != Model.Id);
            var res = await items
                .ToListAsync(ct);
            AddField("Title", "عنوان");
            AddField("ParentId", "والد", FieldType.Select, Model?.ParentId?.ToString() ?? "",res.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList());
        }
    }
}