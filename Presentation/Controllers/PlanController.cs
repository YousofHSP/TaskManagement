using AutoMapper;
using Data.Contracts;
using Entity;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class PlanController(IRepository<Plan> repository, IMapper mapper ): BaseController<PlanDto, PlanResDto, Plan>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        base.Configure(method, ct);
        AddColumn("عنوان", nameof(PlanResDto.Title));
        AddColumn("ساعت شروع", "StartTime");
        AddColumn("ساعت پایان", "EndTime");
        AddField("Title", "عنوان");
        AddField("StartTime", "ساعت شروع", FieldType.Time);
        AddField("EndTime", "ساعت پایان", FieldType.Time);
    }
}