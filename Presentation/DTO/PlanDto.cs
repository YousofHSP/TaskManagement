using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DTO;
using Entity;
using Presentation.Attributes;
using Presentation.Models;

namespace Presentation.DTO;

public class PlanDto: BaseDto<PlanDto, Plan>
{
    [Display(Name = "عنوان")]
    [Field(FieldType.Text)]
    public string Title { get; set; }
    [Display(Name = "زمان شروع")]
    [Field(FieldType.Time)]
    public string StartTime { get; set; }
    [Display(Name = "زمان پایان")]
    [Field(FieldType.Time)]
    public string EndTime { get; set; }
}

public class PlanResDto: BaseDto<PlanResDto, Plan>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "زمان شروع")]
    public string StartTime { get; set; }
    [Display(Name = "زمان پایان")]
    public string EndTime { get; set; }

    protected override void CustomMappings(IMappingExpression<Plan, PlanResDto> mapping)
    {
        mapping.ForMember(
            i => i.StartTime,
            conf => conf.MapFrom(s => s.StartTime.ToString(@"hh\:mm"))
        );
        mapping.ForMember(
            i => i.EndTime,
            conf => conf.MapFrom(s => s.EndTime.ToString(@"hh\:mm"))
        );
    }
}