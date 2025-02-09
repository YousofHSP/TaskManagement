using AutoMapper;
using Common.Utilities;
using Data.Contracts;
using Entity;
using Presentation.DTO;

namespace Presentation.Controllers;

public class EventController(IRepository<Event> repository, IMapper mapper) : BaseController<EventDto, Event>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        AddColumn(ModelExtensions.ToDisplay<EventDto>(i => i.Title), nameof(EventDto.Title));
        AddColumn(ModelExtensions.ToDisplay<EventDto>(i => i.Description), nameof(EventDto.Description));
        
        AddField(nameof(EventDto.Title), ModelExtensions.ToDisplay<EventDto>(i => i.Title));
        AddField(nameof(EventDto.Description), ModelExtensions.ToDisplay<EventDto>(i => i.Description));
    }
}