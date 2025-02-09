using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using DTO;
using Entity;
using Entity.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Attributes;
using Presentation.DTO;
using Presentation.Models;

namespace Presentation.Controllers;

public class BaseController<TDto, TResDto, TEntity, TKey>(IRepository<TEntity> repository, IMapper mapper) : Controller
    where TEntity : class, IEntity<TKey>
    where TDto : BaseDto<TDto, TEntity, TKey>
    where TResDto : BaseDto<TResDto, TEntity, TKey>
{
    private List<Column> Columns = [];
    private readonly IndexViewModel<TResDto> _indexViewModel = new();
    private readonly CreateViewModel _createViewModel = new();
    private readonly EditViewModel _editViewModel = new();
    private List<string> Includes = [];
    protected TEntity? Model { get; private set; }

    protected void SetIncludes(params List<string> includes)
    {
        Includes = includes;
    }

    protected void SetTitle(string title)
    {
        _indexViewModel.Title = title;
    }

    protected void AddColumn(string name, string value)
    {
        Columns.Add(new Column { Name = name, Value = value });
    }

    protected void AddField(string name, string label, FieldType type = FieldType.Text, string value = "",
        List<SelectListItem>? items = null)
    {
        items ??= [];
        if (type is FieldType.Select or FieldType.MultiSelect)
            items.Insert(0, new SelectListItem("انتخاب کنید", ""));
        _createViewModel.Fields.Add(new Field
            { Label = label, Name = name, Type = type, Value = value, Items = items });
    }

    public virtual async Task Configure(string method, CancellationToken ct)
    {
        _indexViewModel.ViewSetting.Create = true;
        _indexViewModel.ViewSetting.Edit = true;
        _indexViewModel.ViewSetting.Delete = true;
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<ViewResult> Index(CancellationToken ct)
    {
        await Configure("index", ct);
        var list = repository.TableNoTracking
            .AsQueryable();

        if (Includes.Count != 0)
        {
            foreach (var item in Includes)
                list = list.Include(item).AsQueryable();
        }

        var res = await list.ProjectTo<TResDto>(mapper.ConfigurationProvider).ToListAsync(ct);
        _indexViewModel.Rows = res;
        _indexViewModel.Columns = Columns;
        ViewBag.Model = _indexViewModel;
        return View("~/Views/Base/Index.cshtml", _indexViewModel);
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<ViewResult> Create(CancellationToken ct)
    {
        await Configure("create", ct);
        return View("~/Views/Base/Create.cshtml", _createViewModel);
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<IActionResult> Edit(TKey id, CancellationToken ct)
    {
        var model = await repository
            .TableNoTracking
            .Where(i => i.Id.Equals(id))
            .FirstOrDefaultAsync(ct);
        Model = model;
        await Configure("edit", ct);
        _editViewModel.Fields = _createViewModel.Fields;
        var dto = mapper.Map<TDto>(model);
        if (dto is null)
            return NotFound();
        ViewBag.Model = _editViewModel;
        return View("~/Views/Base/Edit.cshtml", dto);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Store(TDto dto, CancellationToken ct)
    {
        await Configure("store", ct);
        var model = dto.ToEntity(mapper);
        if (!ModelState.IsValid)
        {
            _createViewModel.Error = true;
            return View("~/Views/Base/Create.cshtml", _createViewModel);
        }

        await repository.AddAsync(model, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Update(TDto dto, CancellationToken ct)
    {
        try
        {
            await Configure("update", ct);
            var model = await repository.GetByIdAsync(ct, dto.Id);
            if (model is null)
                return NotFound();
            model = dto.ToEntity(model, mapper);
            _editViewModel.Fields = _createViewModel.Fields;
            _editViewModel.Error = false;
            ViewBag.Model = _editViewModel;
            if (!ModelState.IsValid)
            {
                _editViewModel.Error = true;
                return View("~/Views/Base/Edit.cshtml", dto);
            }

            await repository.UpdateAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<IActionResult> Delete(TKey id, CancellationToken ct)
    {
        var model = await repository.GetByIdAsync(ct,id);
        if (model is null)
            return NotFound();
        await repository.DeleteAsync(model, ct);
        return RedirectToAction(nameof(Index));
    }
}

public class BaseController<TDto, TResDto, TEntity>(IRepository<TEntity> repository, IMapper mapper)
    : BaseController<TDto, TResDto, TEntity, int>(repository, mapper)
    where TEntity : class, IEntity<int>
    where TDto : BaseDto<TDto, TEntity, int>
    where TResDto : BaseDto<TResDto, TEntity, int>;

public class BaseController<TDto, TEntity>(IRepository<TEntity> repository, IMapper mapper)
    : BaseController<TDto, TDto, TEntity, int>(repository, mapper)
    where TEntity : class, IEntity<int>
    where TDto : BaseDto<TDto, TEntity, int>;