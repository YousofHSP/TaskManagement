using AutoMapper;
using Data.Contracts;
using Entity;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class PlanController(IRepository<Plan> repository, IMapper mapper ): BaseController<PlanDto, PlanResDto, Plan>(repository, mapper)
{
}