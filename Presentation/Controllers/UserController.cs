using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using DTO;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Presentation.Attributes;

namespace Presentation.Controllers;

public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    [HasPermission("User.Index")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await _userRepository
            .TableNoTracking
            .Where(u => u.Id != 1)
            .OrderByDescending(u => u.Id)
            .ProjectTo<UserListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _userRepository
            .TableNoTracking
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));
        var user = await _userManager.FindByIdAsync(id.ToString());
        var role = await _userManager.GetRolesAsync(user!);
        dto.RoleName = role.FirstOrDefault() ?? "";
        var roles = new List<SelectListItem>(
            _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Description,
                Value = r.Name,
                Selected = dto.RoleName == r.Name,
            }).ToList());
        dto.Roles = roles;
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserDto dto, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid) return View(dto);
        var user = await _userRepository.GetByIdAsync(cancellationToken, dto.Id);
        if(user is null) return View(dto);

        user = dto.ToEntity(user, _mapper);
        await _userRepository.UpdateAsync(user, cancellationToken);
        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user!, dto.RoleName);

        if (dto.Password is not null)
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, dto.Password);
        }

        return RedirectToAction("Index", "User");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userRepository
            .TableNoTracking
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));
        return View(user);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var dto = new UserDto();
        var roles = new List<SelectListItem>(
            _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Description,
                Value = r.Name,
                Selected = dto.RoleName == r.Name,
            }).ToList());
        dto.Roles = roles;
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserDto dto, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid) return View(dto);
        var checkPhoneNumber = await _userRepository.CheckUserName(dto.PhoneNumber, cancellationToken);
        if (checkPhoneNumber)
        {
            ModelState.AddModelError(nameof(dto.PhoneNumber), "شماره موبایل تکراری است");
            return View(dto);
        }
        var user = new User();
        user = dto.ToEntity(user, _mapper);
        user.UserName = dto.PhoneNumber;
        var result = await _userManager.CreateAsync(user, dto.Password);
        if(!result.Succeeded)
        {
            var errorMessage = "";
            foreach (var item in result.Errors)
            {
                errorMessage += item + Environment.NewLine;
            }
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(dto);
        }
        await _userManager.AddToRoleAsync(user!, dto.RoleName);
        return RedirectToAction("Index", "User");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        if (user is null) return NotFound();


        var dto = UserDeleteDto.FromEntity(user, _mapper);
        return View(dto);

    }

    public async Task<IActionResult> Delete(UserDeleteDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, dto.Id);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.DeleteAsync(user);
        return RedirectToAction("Index", "User");
    }

    public async Task<IActionResult> AddUserRole(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken,id);
        if(user is null) return NotFound();
        var roles = new List<SelectListItem>(
            _roleManager.Roles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Name,
        }).ToList());
        return View(new AddUserRoleDto { Roles = roles, Id = id, FullName = user.FullName, PhoneNumber = user.PhoneNumber });
    }

    [HttpPost]
    public async Task<IActionResult> AddUserRole(AddUserRoleDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, dto.Id);
        await _userManager.AddToRoleAsync(user!, dto.Role);
        return RedirectToAction("UserRoles", "User", new { id = user.Id });
    }

    public async Task<IActionResult> UserRoles(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        var roles = await _userManager.GetRolesAsync(user!);
        ViewBag.UserInfo = $"Name : {user.FullName} PhoneNumber: {user.PhoneNumber}";
        return View(roles);
    }

}