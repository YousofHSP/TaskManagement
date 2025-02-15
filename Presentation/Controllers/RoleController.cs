using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using DTO;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers;

public class RoleController(IRepository<Role> repository, RoleManager<Role> roleManager, IMapper mapper) : BaseController<RoleDto, RoleResDto, Role>(repository, mapper)
{
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetTitle("نقش");
        AddCondition(i => i.Name != "Admin");
        
    }

    public override async Task<ViewResult> Create(CancellationToken ct)
    {
        await Task.CompletedTask;
        return View();
    }

    public override async Task<IActionResult> Create(RoleDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Create", dto);
        var role = dto.ToEntity(mapper);
        await roleManager.CreateAsync(role);
        foreach (var permission in dto.Permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }
        return RedirectToAction(nameof(Index));
    }

    public override async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        
        var model = roleManager.Roles
            .FirstOrDefault(i => i.Id == id);

        var permissions = await roleManager.GetClaimsAsync(model);
        var dto = mapper.Map<RoleDto>(model);
        dto.Permissions = permissions.Select(p => p.Value).ToList();
        return View(dto);
    }

    public override async Task<IActionResult> Edit(RoleDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Edit", dto);
        var role = repository.TableNoTracking.FirstOrDefault(i => i.Id == dto.Id);
        if(role is null) return NotFound();
        
        role = dto.ToEntity(role, mapper);
        await roleManager.UpdateAsync(role);
        var currentClaims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in currentClaims)
            await roleManager.RemoveClaimAsync(role, claim);
        
        foreach (var permission in dto.Permissions)
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        
        return RedirectToAction(nameof(Index));
    }
}