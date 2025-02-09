using AutoMapper;
using DTO;
using Entity;

namespace Presentation.DTO;

public class PlanDto: BaseDto<PlanDto, Plan>
{
    public string Title { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}

public class PlanResDto: BaseDto<PlanResDto, Plan>
{
    public string Title { get; set; }
    public string StartTime { get; set; }
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