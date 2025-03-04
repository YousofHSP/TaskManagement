using AutoMapper;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.DTO;

namespace Presentation.Controllers;

public class ProjectController(IRepository<Project> repository, IRepository<Customer> customerRepository, IMapper mapper) : BaseController<ProjectDto, ProjectResDto, Project>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetIncludes(nameof(Project.Customer));

        var customers = await customerRepository.GetSelectListItems(ct: ct);
        AddOptions(nameof(ProjectDto.CustomerId), customers);
    }

    [HttpGet]
    public async Task<List<SelectListItem>> GetByCustomer(int? id, CancellationToken ct)
    {
        if(id is null)
            return await repository.GetSelectListItems(ct:ct);
        return await repository.GetSelectListItems(whereFunc: i => i.CustomerId == id, ct:ct);
    }
}