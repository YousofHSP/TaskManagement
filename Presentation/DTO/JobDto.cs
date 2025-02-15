using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AutoMapper;
using DTO;
using Entity;
using Common.Utilities;

namespace Presentation.DTO;

public class JobDto:BaseDto<JobDto, Job>
{

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Title { get; set; }
    
    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int UserId { get; set; }
    
    [Display(Name = "مشتری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int CustomerId { get; set; }
    
    [Display(Name = "والد")]
    public int? ParentId { get; set; }
    
    [Display(Name = "پلن")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int PlanId { get; set; }
    
    [Display(Name = "توضیحات")]
    public string Description { get; set; }
    
    [Display(Name = "رویداد")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int EventId { get; set; }
    
    [Display(Name = "تاریخ شروع")]
    public string StartedAt { get; set; }
    
    [Display(Name = "تاریخ پایان")]
    public string EndedAt { get; set; }

    [Display(Name = "وضعیت")]
    public JobStatus Status { get; set; }
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

public class JobResDto: BaseDto<JobResDto, Job>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "کاربر")]
    public string UserFullName { get; set; }
    [Display(Name = "مشتری")]
    public string CustomerTitle { get; set; }
    [Display(Name = "والد")]
    public string ParentTitle { get; set; }
    [Display(Name = "پلن")]
    public string PlanTitle { get; set; }
    [Display(Name = "رویداد")]
    public string EventTitle { get; set; }
    [Display(Name = "توضیحات")]
    public string Description { get; set; }
    [Display(Name = "تاریخ شروع")]
    public string StartDateTime { get; set; }
    [Display(Name = "تاریخ پایان")]
    public string EndDateTime { get; set; }
    
    [Display(Name = "وضعیت")] 
    public JobStatus Status { get; set; }

    protected override void CustomMappings(IMappingExpression<Job, JobResDto> mapping)
    {
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
    [Display(Name = "کاربر")]
    public int? UserId { get; set; }
    public List<JobResDto> Jobs { get; set; }
    public Dictionary<string, string> PlansSum { get; set; }
}