using AutoMapper;
using Common.Utilities;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class JobController(
    IRepository<Job> repository,
    IRepository<User> userRepository,
    IRepository<Customer> customerRepository,
    IRepository<Event> eventRepository,
    IRepository<Plan> planRepository,
    IMapper mapper
    ) : BaseController<JobDto, JobResDto, Job>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.Title), nameof(JobResDto.Title));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.UserFullName), nameof(JobResDto.UserFullName));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.CustomerTitle), nameof(JobResDto.CustomerTitle));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.ParentTitle), nameof(JobResDto.ParentTitle));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.EventTitle), nameof(JobResDto.EventTitle));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.PlanTitle), nameof(JobResDto.PlanTitle));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.Description), nameof(JobResDto.Description));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.StartDateTime), nameof(JobResDto.StartDateTime));
        AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.EndDateTime), nameof(JobResDto.EndDateTime));
        
        var users = await userRepository.TableNoTracking.Where(i => i.Id != 1).ToListAsync(ct);
        var customers = await customerRepository.TableNoTracking.ToListAsync(ct);
        var events = await eventRepository.TableNoTracking.ToListAsync(ct);
        var plans = await planRepository.TableNoTracking.ToListAsync(ct);
        var parentsQuery = repository.TableNoTracking.AsQueryable();
        if(Model is not null)
            parentsQuery = parentsQuery.Where(i => i.Id != Model.Id);

        var parents = await parentsQuery.ToListAsync(ct);
        AddField(nameof(JobDto.Title),ModelExtensions.ToDisplay<JobDto>(i => i.Title));
        AddField(nameof(JobDto.UserId),
            ModelExtensions.ToDisplay<JobDto>(i => i.UserId),
            FieldType.Select,
            Model?.UserId.ToString() ?? "",
            users.Select(i => new SelectListItem(i.FullName, i.Id.ToString())).ToList()
            );
        AddField(nameof(JobDto.CustomerId),
            ModelExtensions.ToDisplay<JobDto>(i => i.CustomerId),
            FieldType.Select,
            Model?.CustomerId.ToString() ?? "",
            customers.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
            );
        AddField(nameof(JobDto.ParentId),
            ModelExtensions.ToDisplay<JobDto>(i => i.ParentId),
            FieldType.Select,
            Model?.ParentId?.ToString() ?? "",
            parents.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
            );
        AddField(nameof(JobDto.EventId),
            ModelExtensions.ToDisplay<JobDto>(i => i.EventId),
            FieldType.Select,
            Model?.EventId.ToString() ?? "",
            events.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
            );
        AddField(nameof(JobDto.PlanId),
            ModelExtensions.ToDisplay<JobDto>(i => i.PlanId),
            FieldType.Select,
            Model?.PlanId.ToString() ?? "",
            plans.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
            );
        AddField(nameof(JobDto.StartedAt),ModelExtensions.ToDisplay<JobDto>(i => i.StartedAt), FieldType.DateTime);
        AddField(nameof(JobDto.EndedAt),ModelExtensions.ToDisplay<JobDto>(i => i.EndedAt), FieldType.DateTime);
        AddField(nameof(JobDto.Description),ModelExtensions.ToDisplay<JobDto>(i => i.Description));
    }
}