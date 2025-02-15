using System.Globalization;
using AutoMapper;
using Common.Utilities;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Mvc;
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
        SetTitle("تسک");
        SetIncludes("Customer", "User", "Parent", "Event", "Plan");
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.Title), nameof(JobResDto.Title));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.UserFullName), nameof(JobResDto.UserFullName));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.CustomerTitle), nameof(JobResDto.CustomerTitle));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.ParentTitle), nameof(JobResDto.ParentTitle));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.EventTitle), nameof(JobResDto.EventTitle));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.PlanTitle), nameof(JobResDto.PlanTitle));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.Description), nameof(JobResDto.Description));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.StartDateTime), nameof(JobResDto.StartDateTime));
        // AddColumn(ModelExtensions.ToDisplay<JobResDto>(i => i.EndDateTime), nameof(JobResDto.EndDateTime));

        var users = await userRepository.TableNoTracking.Where(i => i.Id != 1).ToListAsync(ct);
        var customers = await customerRepository.TableNoTracking.ToListAsync(ct);
        var events = await eventRepository.TableNoTracking.ToListAsync(ct);
        var plans = await planRepository.TableNoTracking.ToListAsync(ct);
        var jobStatus = new List<SelectListItem>
        {
            new SelectListItem(JobStatus.Todo.ToDisplay(), JobStatus.Todo.ToString()),
            new SelectListItem(JobStatus.InProgress.ToDisplay(), JobStatus.InProgress.ToString()),
            new SelectListItem(JobStatus.Done.ToDisplay(), JobStatus.Done.ToString()),
        };
        var parentsQuery = repository.TableNoTracking.AsQueryable();
        if (Model is not null)
            parentsQuery = parentsQuery.Where(i => i.Id != Model.Id);

        var parents = await parentsQuery.ToListAsync(ct);
        AddField(nameof(JobDto.Title), ModelExtensions.ToDisplay<JobDto>(i => i.Title));
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
        AddField(nameof(JobDto.StartedAt), ModelExtensions.ToDisplay<JobDto>(i => i.StartedAt), FieldType.DateTime);
        AddField(nameof(JobDto.EndedAt), ModelExtensions.ToDisplay<JobDto>(i => i.EndedAt), FieldType.DateTime);
        AddField(nameof(JobDto.Status),
            ModelExtensions.ToDisplay<JobDto>(i => i.Status),
            FieldType.Select,
            Model?.Status.ToString() ?? "",
            jobStatus
        );
        AddField(nameof(JobDto.Description), ModelExtensions.ToDisplay<JobDto>(i => i.Description));

        AddFilter(nameof(JobDto.UserId),
            ModelExtensions.ToDisplay<JobDto>(i => i.UserId),
            FieldType.Select,
            "",
            users.Select(i => new SelectListItem(i.FullName, i.Id.ToString())).ToList()
        );
        AddFilter(nameof(JobDto.CustomerId),
            ModelExtensions.ToDisplay<JobDto>(i => i.CustomerId),
            FieldType.Select,
            "",
            customers.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
        );
        AddFilter(nameof(JobDto.PlanId),
            ModelExtensions.ToDisplay<JobDto>(i => i.PlanId),
            FieldType.Select,
            "",
            plans.Select(i => new SelectListItem(i.Title, i.Id.ToString())).ToList()
        );
        AddSum("جمع ساعات", i =>
        {
            if (i is { EndDateTime: not null, StartDateTime: not null })
                return (i.EndDateTime.Value - i.StartDateTime.Value).TotalMinutes;
            return 0;
        }, SumTypeEnum.Time);
    }

    [HttpGet]
    public async Task<IActionResult> Report(JobReportViewModel dto, CancellationToken ct)
    {
        var jobsQuery = repository.TableNoTracking
            .Include(i => i.User)
            .Include(i => i.Customer)
            .AsQueryable();
        if (dto.UserId is not null)
            jobsQuery = jobsQuery.Where(i => i.UserId == dto.UserId);
        if (dto.CustomerId is not null)
            jobsQuery = jobsQuery.Where(i => i.CustomerId == dto.CustomerId);
        var jobs = await jobsQuery.ToListAsync(ct);
        var plans = await planRepository.TableNoTracking.ToListAsync(ct);
        var plansSums = new Dictionary<string, int>();
        foreach (var plan in plans)
            plansSums.Add(plan.Title, 0);
        foreach (var job in jobs)
        {
            if (job is not { StartDateTime: not null, EndDateTime: not null }) continue;
            var jobStartDate = job.StartDateTime.Value;
            var jobEndDate = job.EndDateTime.Value;
            for (var day = jobStartDate.Date; day <= jobEndDate.Date; day = day.AddDays(1))
            {
                var jobStart = (day == jobStartDate.Date) ? jobStartDate.TimeOfDay : TimeSpan.Zero;
                var jobEnd = (day == jobEndDate.Date) ? jobEndDate.TimeOfDay : new TimeSpan(23, 59, 59);
                foreach (var plan in plans)
                {

                    var overlapStart = jobStart > plan.StartTime ? jobStart : plan.StartTime;
                    var overlapEnd = jobEnd < plan.EndTime ? jobEnd : plan.EndTime;

                    if (overlapStart < overlapEnd)
                    {
                        plansSums[plan.Title] += (int)(overlapEnd - overlapStart).TotalMinutes;
                    }
                }
            }
        }

        var jobResDtos = mapper.Map<List<JobResDto>>(jobs);
        dto.Jobs = jobResDtos;
        dto.PlansSum = plansSums.ToDictionary(i => i.Key, v => $"{(v.Value / 60):D2}:{(v.Value % 60):D2}");
        return View(dto);
    }
}