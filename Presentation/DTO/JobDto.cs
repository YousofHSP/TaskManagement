using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AutoMapper;
using DTO;
using Entity;
using Common.Utilities;
using Presentation.Attributes;
using Presentation.Models;

namespace Presentation.DTO;

public class JobDto:BaseDto<JobDto, Job>
{

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Text)]
    public string Title { get; set; }
    
    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Select)]
    public int UserId { get; set; }
    
    [Display(Name = "مشتری")]
    [Field(FieldType.Select)]
    public int? CustomerId { get; set; }
    
    [Display(Name = "پروژه")]
    [Field(FieldType.Select)]
    public int? ProjectId { get; set; }
    
    
    [Display(Name = "تاریخ شروع")]
    [Field(FieldType.DateTime)]
    public string? StartedAt { get; set; }
    
    [Display(Name = "تاریخ پایان")]
    [Field(FieldType.DateTime)]
    public string? EndedAt { get; set; }

    [Display(Name = "فعالیت")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Select)]
    public int EventId { get; set; }
    
    [Display(Name = "وضعیت")]
    [Field(FieldType.Select)]
    public JobStatus Status { get; set; }

    [Display(Name = "توضیحات")]
    [Field(FieldType.Text)]
    public string? Description { get; set; } = "";

    public List<SubJobDto> SubJobs { get; set; } = [];
    protected override void CustomMappings(IMappingExpression<Job, JobDto> mapping)
    {
        mapping.ForMember(
            d => d.StartedAt,
            s => s.MapFrom(m => m.StartDateTime != null ? m.StartDateTime.ToString() : ""
            )
        );
        mapping.ForMember(
            d => d.EndedAt,
            s => s.MapFrom(m => m.EndDateTime != null ? m.EndDateTime.Value.ToString() : ""
            )
        );
    }

    protected override void CustomMappings(IMappingExpression<JobDto, Job> mapping)
    {
        mapping.ForMember(
            m => m.StartDateTime,
            s => s.MapFrom(d => d.StartedAt.ToGregorian())
            );
        mapping.ForMember(
            m => m.EndDateTime,
            s => s.MapFrom(d => d.EndedAt.ToGregorian())
            );
    }
}

public class SubJobDto : BaseDto<SubJobDto, Job>
{
    public string? Title { get; set; }
    public int? EventId { get; set; }
    public int? UserId { get; set; }
    public string? StartedAt { get; set; }
    public string? EndedAt { get; set; }
    public JobStatus? Status { get; set; }
}

public class JobResDto: BaseDto<JobResDto, Job>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "کاربر")]
    public string UserFullName { get; set; }
    [Display(Name = "مشتری")]
    public string CustomerTitle { get; set; }
    [Display(Name = "پروژه")]
    public string ProjectTitle { get; set; }
    [Display(Name = "فعالیت")]
    public string EventTitle { get; set; }
    [Display(Name = "توضیحات")]
    public string Description { get; set; }
    [Display(Name = "تاریخ شروع")]
    public string StartDateTime { get; set; }
    [Display(Name = "تاریخ پایان")]
    public string EndDateTime { get; set; }
    
    [Display(Name = "وضعیت")] 
    public JobStatus Status { get; set; }

    [Display(Name = "تعداد زیرفعالیت")] 
    public int SubJobsCount { get; set; }
    protected override void CustomMappings(IMappingExpression<Job, JobResDto> mapping)
    {
        mapping.ForMember(
            d => d.SubJobsCount,
            s => s.MapFrom(m => m.Children.Count 
            )
        );
        mapping.ForMember(
            d => d.StartDateTime,
            s => s.MapFrom(m => m.StartDateTime != null ? m.StartDateTime.Value.ToShamsi() : ""
            )
        );
        mapping.ForMember(
            d => d.EndDateTime,
            s => s.MapFrom(m => m.EndDateTime != null ? m.EndDateTime.Value.ToShamsi() : ""
            )
        );
    }
}

public class JobReportViewModel
{
    [Display(Name = "مشتری")]
    public int? CustomerId { get; set; }
    [Display(Name = "پروژه")]
    public int? ProjectId { get; set; }
    [Display(Name = "کاربر")]
    public int? UserId { get; set; }

    [Display(Name = "زمان شروع")]
    public string? StartDateTime { get; set; }
    [Display(Name = "زمان پایان")]
    public string? EndDateTime { get; set; }
    public List<JobResDto> Jobs { get; set; }
    public Dictionary<string, string> PlansSum { get; set; }
}

public class JobQuickUpdateDto: BaseDto<JobQuickUpdateDto, Job>
{
    public JobStatus Status { get; set; }
    public string StartedAt { get; set; }
    public string EndedAt { get; set; }
    public List<SubJobDto> SubJobs { get; set; } = [];

}