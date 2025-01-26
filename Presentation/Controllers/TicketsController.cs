using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;

namespace Presentation.Controllers
{
    public class TicketsController : Controller
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public TicketsController(IRepository<Ticket> ticketRepository, IMapper mapper, IRepository<User> userRepository)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // GET: TicketsController
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
            {
                var tickets = await _ticketRepository.TableNoTracking
                    .Where(t => t.ParentId == null)
                    .OrderByDescending(t => t.IsSeen)
                    .ThenByDescending(t => t.Id)
                    .Include(t => t.User)
                    .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                return View(tickets);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var tickets = await _ticketRepository.TableNoTracking
                        .Where(t => t.ParentId == null && t.UserId == userId.ToInt())
                        .OrderByDescending(t => t.IsSeen)
                        .ThenByDescending(t => t.Id)
                        .Include(t => t.User)
                        .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                return View(tickets);
            }
        }

        // GET: TicketsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TicketDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);

                var userId = User.Identity!.GetUserId();
                var model = dto.ToEntity(_mapper);
                model.UserId = userId.ToInt();
                await _ticketRepository.AddAsync(model, cancellationToken);
                var resultDto = await _ticketRepository.TableNoTracking
                    .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(t => t.Id.Equals(model.Id));
                if (resultDto is null) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(dto);
            }
        }

        public async Task<IActionResult> Answer(int id, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var unseenedTickets = await _ticketRepository.Entities
                .Where(t => !t.IsSeen)
                .Where(t => t.Id.Equals(id) || t.ParentId.Equals(id))
                .Where(t => t.UserId != userId.ToInt())
                .ExecuteUpdateAsync(t => t.SetProperty(i => i.IsSeen, true), cancellationToken);

            var tickets = await _ticketRepository.TableNoTracking

                .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                .Where(t => t.Id.Equals(id) || t.ParentId.Equals(id))
                .ToListAsync(cancellationToken);
            ViewBag.Tickets = tickets;
            ViewBag.ParentId = id;
            ViewBag.Priority = tickets.First().Priority;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Answer(TicketDto dto, CancellationToken cancellationToken)
        {

            try
            {
                if (!ModelState.IsValid) return View(dto);

                var userId = User.Identity!.GetUserId();
                var model = dto.ToEntity(_mapper);
                model.Id = 0;
                model.UserId = userId.ToInt();
                await _ticketRepository.AddAsync(model, cancellationToken);
                var resultDto = await _ticketRepository.TableNoTracking
                    .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(t => t.Id.Equals(model.Id));
                if (resultDto is null) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(dto);
            }
        }


    }
}
