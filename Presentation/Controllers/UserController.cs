using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoMapper;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using DTO;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;

namespace Presentation.Controllers;

public class UserController(
    IMapper mapper,
    IUserRepository repository,
    IRepository<Role> roleRepository,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : BaseController<UserDto, UserResDto, User>(repository, mapper)
{
    private readonly IMapper _mapper = mapper;
    public override async Task Configure(string method, CancellationToken ct)
    {
        await base.Configure(method, ct);
        SetIncludes("Roles");

        var roles= await roleRepository.GetSelectListItems(nameof(Role.Name), nameof(Role.Name), i => i.Name != "Admin" ,ct);
        AddField(nameof(UserDto.FullName), ModelExtensions.ToDisplay<UserDto>(i => i.FullName));
        AddField(nameof(UserDto.PhoneNumber), ModelExtensions.ToDisplay<UserDto>(i => i.PhoneNumber));
        AddField(nameof(UserDto.Password), ModelExtensions.ToDisplay<UserDto>(i => i.Password));
        var selectedRole = "";
        if (method is "edit" or "update" && Model is not null)
            selectedRole = (await userManager.GetRolesAsync(Model)).FirstOrDefault() ?? "";
        AddField(nameof(UserDto.RoleName), ModelExtensions.ToDisplay<UserDto>(i => i.RoleName), FieldType.Select, selectedRole,roles);
        AddCondition(m => !m.Id.Equals(1));
        
    }

    public override async Task<IActionResult> Create(UserDto dto, CancellationToken ct)
    {
        await Configure("create", ct);
        if (string.IsNullOrEmpty(dto.Password))
        {
            
            CreateViewModel.Error = true;
            ModelState.AddModelError(nameof(UserDto.Password), "رمز کاربر را وارد کنید");
            return View("~/Views/Base/Create.cshtml", CreateViewModel);
        }
        if (!ModelState.IsValid)
        {
            CreateViewModel.Error = true;
            return View("~/Views/Base/Create.cshtml", CreateViewModel);
        }
        var isExists = await repository.TableNoTracking.AnyAsync(i => i.PhoneNumber == dto.PhoneNumber, ct);
        if (isExists)
        {
            ModelState.AddModelError(nameof(UserDto.PhoneNumber), "این موبایل قبلا انتخاب شده");
            CreateViewModel.Error = true;
            return View("~/Views/Base/Create.cshtml", CreateViewModel);
        }
            
        var model = dto.ToEntity(mapper);
        model.UserName = model.PhoneNumber;
        await userManager.CreateAsync(model);
        await userManager.AddToRoleAsync(model, dto.RoleName);
        await userManager.AddPasswordAsync(model, dto.Password);
        return RedirectToAction(nameof(Index));

    }

    public override async  Task<IActionResult> Edit(UserDto dto, CancellationToken ct)
    {
        await Configure("update", ct);
        var model = await repository.GetByIdAsync(ct, dto.Id);
        if (model is null)
            return NotFound();
        model = dto.ToEntity(model, mapper);
        EditViewModel.Fields = CreateViewModel.Fields;
        EditViewModel.Error = false;
        ViewBag.Model = EditViewModel;
        if (!ModelState.IsValid)
        {
            EditViewModel.Error = true;
            return View("~/Views/Base/Edit.cshtml", dto);
        }

        await userManager.UpdateAsync(model);
        var roles = await userManager.GetRolesAsync(model);
        await userManager.RemoveFromRolesAsync(model, roles);
        if(!string.IsNullOrEmpty(dto.RoleName))
            await userManager.AddToRoleAsync(model, dto.RoleName);
        if (!string.IsNullOrEmpty(dto.Password))
        {
            await userManager.RemovePasswordAsync(model);
            await userManager.AddPasswordAsync(model, dto.Password);
        }
        return RedirectToAction(nameof(Index));
    }

    public override async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await userManager.FindByIdAsync(id.ToString());
        if (model is null)
            throw new NotFoundException();
        var roles = await userManager.GetRolesAsync(model);
        await userManager.RemoveFromRolesAsync(model, roles);

        await userManager.DeleteAsync(model);
        return RedirectToAction(nameof(Index));
    }
}