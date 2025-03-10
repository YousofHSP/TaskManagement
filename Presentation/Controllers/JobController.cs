using System.Globalization;
using AutoMapper;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using DTO;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;
using Presentation.Helpers;
using Presentation.Models;

namespace Presentation.Controllers;

public class JobController(
    IRepository<Job> repository,
    IRepository<User> userRepository,
    IRepository<Customer> customerRepository,
    IRepository<Project> projectRepository,
    IRepository<Event> eventRepository,
    IRepository<Plan> planRepository,
    IMapper mapper
) : BaseController<JobDto, JobResDto, Job>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetIncludes(nameof(Job.Customer), nameof(Job.User), nameof(Job.Event), nameof(Job.Project), nameof(Job.Children));
        AddJsFile("/js/components/changeModal.js", "index");
        AddJsFile("/js/components/getProjectsByCustomer.js", "create", "index");
        AddComponentFile("Components/SubTasks", "create");
        AddComponentFile("Components/ChangeModal", "index");
        AddListAction("ثبت گزارش", "fa fa-arrow-circle-left", "", "open-modal");

        List<SelectListItem> users;
        var selectedUserId = Model?.UserId.ToString() ?? "";
        var selectedCustomerId = Model?.CustomerId.ToString() ?? "";
        var selectedProjectId = Model?.ProjectId.ToString() ?? "";
        var selectedEventId = Model?.EventId.ToString() ?? "";
        var selectedStatus = Model?.Status.ToString() ?? "";

        AddCondition(i => i.ParentId == null);
        if (!CheckPermission.Check(User, "Job.ShowAllInfo"))
        {
            AddCondition(i => i.UserId == User.Identity!.GetUserId<int>());
            users = await userRepository.GetSelectListItems(
                nameof(UserDto.FullName),
                hasDefault: false,
                selected: [selectedUserId],
                whereFunc: i => i.Id != 1 && i.Id == User.Identity!.GetUserId<int>(),
                ct: ct
            );
        }
        else
        {
            users = await userRepository.GetSelectListItems(
                nameof(UserDto.FullName),
                whereFunc: i => i.Id != 1,
                hasDefault: false,
                selected: [selectedUserId],
                ct: ct
            );
        }

        var projects = await projectRepository.GetSelectListItems(selected: [selectedProjectId],ct: ct);
        var customers = await customerRepository.GetSelectListItems(selected: [selectedCustomerId], ct: ct);
        var events = await eventRepository.GetSelectListItems(hasDefault: false, selected: [selectedEventId],ct: ct);
        var jobStatus = new List<SelectListItem>
        {
            new(JobStatus.Todo.ToDisplay(), JobStatus.Todo.ToString(), JobStatus.Todo.ToString() == selectedStatus),
            new(JobStatus.InProgress.ToDisplay(), JobStatus.InProgress.ToString(), JobStatus.InProgress.ToString() == selectedStatus),
            new(JobStatus.Done.ToDisplay(), JobStatus.Done.ToString(), JobStatus.Done.ToString() == selectedStatus)
        };

        AddOptions(nameof(JobDto.UserId), users);
        AddOptions(nameof(JobDto.CustomerId), customers);
        AddOptions(nameof(JobDto.ProjectId), projects);
        AddOptions(nameof(JobDto.EventId), events);
        AddOptions(nameof(JobDto.Status), jobStatus);

        if (method == "index")
        {
            AddFilter(nameof(JobDto.UserId),
                ModelExtensions.ToDisplay<JobDto>(i => i.UserId),
                FieldType.Select,
                "",
                users
            );
            AddFilter(nameof(JobDto.CustomerId),
                ModelExtensions.ToDisplay<JobDto>(i => i.CustomerId),
                FieldType.Select,
                "",
                customers.Where(i => i.Value != "").ToList()
            );
            AddFilter(nameof(JobDto.ProjectId),
                ModelExtensions.ToDisplay<JobDto>(i => i.ProjectId),
                FieldType.Select,
                "",
                projects
                    .Where(i => i.Value != "")
                    .Select(i => { 
                    i.Selected = false;
                    return i;
                }).ToList()
            );
            AddSum("جمع ساعات", i =>
            {
                if (i is { EndDateTime: not null, StartDateTime: not null })
                    return (i.EndDateTime.Value - i.StartDateTime.Value).TotalMinutes;
                return 0;
            }, SumTypeEnum.Time);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Report(JobReportViewModel dto, CancellationToken ct)
    {
        var jobsQuery = repository.TableNoTracking
            .Include(i => i.User)
            .Include(i => i.Customer)
            .Include(i => i.Project)
            .AsQueryable();
        if (dto.UserId is not null)
            jobsQuery = jobsQuery.Where(i => i.UserId == dto.UserId);
        if (dto.CustomerId is not null)
            jobsQuery = jobsQuery.Where(i => i.CustomerId == dto.CustomerId);
        if(dto.ProjectId is not null)
            jobsQuery = jobsQuery.Where(i => i.ProjectId == dto.ProjectId);
            
        if (!string.IsNullOrEmpty(dto.StartDateTime))
        {
            var startDateTime = dto.StartDateTime.ToGregorian();
            jobsQuery = jobsQuery.Where(i => i.StartDateTime <= startDateTime && i.EndDateTime >= startDateTime);
        }

        if (!string.IsNullOrEmpty(dto.EndDateTime))
        {
            var endDateTime = dto.EndDateTime.ToGregorian();
            jobsQuery = jobsQuery.Where(i => i.EndDateTime >= endDateTime && i.StartDateTime <= endDateTime);
        }

        if (!CheckPermission.Check(User, "Job.ShowAllInfo"))
            jobsQuery = jobsQuery.Where(i => i.UserId == User.Identity!.GetUserId<int>());

        var jobs = await jobsQuery.OrderDescending().ToListAsync(ct);
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

    [HttpGet]
    public async Task<JobDto> GetInfo(int id, CancellationToken ct)
    {
        var model = await repository.TableNoTracking
            .Where(i => i.Id == id)
            .Include(i => i.Children)
            .FirstOrDefaultAsync(ct);
        if (model is null)
            throw new NotFoundException("تسک پیدا نشد");
        var dto = mapper.Map<JobDto>(model);
        dto.StartedAt = model.StartDateTime?.ToShamsi() ?? "";
        dto.EndedAt = model.EndDateTime?.ToShamsi() ?? "";
        foreach (var item in model.Children)
        {
            dto.SubJobs.Add(new SubJobDto
            {
                Id = item.Id,
                Title = item.Title,
                Status= item.Status,
                StartedAt = item.StartDateTime.ToString() ?? "",
                EndedAt = item.EndDateTime.ToString() ?? "",
                UserId = item.UserId
            });

        }
        return dto;
    }

    [HttpPost]
    public async Task<IActionResult> QuickUpdate(JobQuickUpdateDto dto, CancellationToken ct)
    {
        var model = await repository.Entities.Include(i => i.Children).Where(i => i.Id == dto.Id).FirstOrDefaultAsync(ct);
        if (model is null)
            throw new NotFoundException("تسک پیدا نشد");
        model.Status = dto.Status;
        model.StartDateTime = dto.StartedAt.ToGregorian();
        model.EndDateTime = dto.EndedAt.ToGregorian();
        await repository.UpdateAsync(model, ct);

        foreach (var item in dto.SubJobs)
        {
            var subModel = model.Children.First(i => i.Id == item.Id);
            subModel.Title = item.Title;
            subModel.Status = item.Status.Value;
            subModel.StartDateTime = item.StartedAt.ToGregorian();
            subModel.EndDateTime = item.EndedAt.ToGregorian();
            
        }
        await repository.UpdateRangeAsync(model.Children, ct);
        
        return Ok();
    }

    public override async Task AfterCreate(JobDto dto, Job model, CancellationToken ct)
    {
        var subJobs = dto.SubJobs.Where(i => !string.IsNullOrEmpty(i.Title)).Select(i => new Job
        {
            UserId = i.UserId.Value,
            Title = i.Title,
            CreatedAt = DateTimeOffset.Now,
            CustomerId = model.CustomerId,
            ProjectId = model.ProjectId,
            ParentId = model.Id,
            EventId = i.EventId.Value
        });
        await repository.AddRangeAsync(subJobs, ct);
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await repository.Table.Where(i => i.ParentId == id).ExecuteDeleteAsync(ct);
        return await base.Delete(id, ct);
    }
}