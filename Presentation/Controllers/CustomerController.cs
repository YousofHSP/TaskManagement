using AutoMapper;
using Data.Contracts;
using Entity;
using Entity.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class CustomerController(IRepository<Customer> repository, IMapper mapper) : BaseController<CustomerDto,CustomerResDto, Customer>(repository, mapper)
{
}