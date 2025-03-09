using AutoMapper;
using Common.Utilities;
using Data.Contracts;
using Entity;
using Presentation.DTO;

namespace Presentation.Controllers;

public class EventController(IRepository<Event> repository, IMapper mapper) : BaseController<EventDto, Event>(repository, mapper)
{
}