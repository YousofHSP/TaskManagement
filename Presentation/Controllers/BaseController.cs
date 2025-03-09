using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using DTO;
using Entity;
using Entity.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Attributes;
using Presentation.DTO;
using Presentation.Helpers;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
public class BaseController<TDto, TResDto, TEntity, TKey>(IRepository<TEntity> repository, IMapper mapper) : Controller
    where TEntity : class, IEntity<TKey>
    where TDto : BaseDto<TDto, TEntity, TKey>
    where TResDto : BaseDto<TResDto, TEntity, TKey>
{
    private List<Column> Columns = [];
    private readonly IndexViewModel<TResDto> _indexViewModel = new();
    protected readonly CreateViewModel CreateViewModel = new();
    protected Dictionary<string, List<SelectListItem>> Options = new();
    protected readonly EditViewModel EditViewModel = new();
    private List<string> _includes = [];
    private List<Expression<Func<TEntity, bool>>> _conditions = [];
    private List<SumType<TEntity>> Sums = new();
    private Dictionary<string, List<string>?> JsFiles = [];
    private Dictionary<string, List<string>?> Components = [];
    protected TEntity? Model { get; private set; }

    protected void AddJsFile(string src, params string[] places)
    {
        foreach (var place in places)
        {
            if (!JsFiles.ContainsKey(place))
                JsFiles.Add(place, []);
            JsFiles[place].Add(src);
        }
    }

    protected void AddComponentFile(string src, string place = "")
    {
        if (!Components.ContainsKey(place))
            Components.Add(place, []);
        Components[place].Add(src);
    }

    protected void SetIncludes(params List<string> includes)
    {
        _includes = includes;
    }

    protected void AddOptions(string name, List<SelectListItem> options)
    {
        Options.Add(name, options);
    }

    protected void AddListAction(string title, string cls, string url, string aClass = "", string rowUrl = "#")
    {
        _indexViewModel.ListActions.Add(new()
            { Class = cls, Title = title, Url = url, AClass = aClass, RowUrl = rowUrl });
    }

    protected void AddCondition(Expression<Func<TEntity, bool>> condition)
    {
        _conditions.Add(condition);
    }

    private void SetTitle(string title)
    {
        _indexViewModel.Title = title;
        CreateViewModel.Title = title;
        EditViewModel.Title = title;
    }

    private void AddColumn(string name, string value)
    {
        Columns.Add(new Column { Name = name, Value = value });
    }

    protected void AddSum(string title, Func<TEntity, double> sumFunc, SumTypeEnum type)
    {
        Sums.Add(new SumType<TEntity>
        {
            Title = title,
            Type = type,
            Func = sumFunc
        });
    }

    private void AddField(string name, string label, FieldType type = FieldType.Text, string value = "",
        List<SelectListItem>? items = null)
    {
        items ??= [];
        if (type is FieldType.Select or FieldType.MultiSelect)
            items.Insert(0, new SelectListItem("انتخاب کنید", "0"));
        CreateViewModel.Fields.Add(new Field
            { Label = label, Name = name, Type = type, Value = value, Items = items });
    }

    protected void AddFilter(string name, string label, FieldType type = FieldType.Text, string value = "",
        List<SelectListItem>? items = null)
    {
        items ??= [];
        if (type is FieldType.Select or FieldType.MultiSelect)
            items.Insert(0, new SelectListItem("انتخاب کنید", ""));
        _indexViewModel.Filters.Add(new Field
            { Label = label, Name = name, Type = type, Value = value, Items = items });
    }

    public virtual async Task Configure(string method, CancellationToken ct)
    {
        _indexViewModel.ViewSetting.Create = false;


        var title = typeof(TEntity).GetCustomAttribute<DisplayAttribute>()?.Name ?? "";
        SetTitle(title);

        var resDtoProperties = typeof(TResDto).GetProperties();
        foreach (var property in resDtoProperties)
        {
            if (property.Name == "Id") continue;
            var attr = property.GetCustomAttributes(typeof(DisplayAttribute), false).First() as DisplayAttribute;
            if (attr?.Name is null)
                continue;
            AddColumn(attr.Name, property.Name);
        }
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<ViewResult> Index(IndexDto model, CancellationToken ct)
    {
        await Configure("index", ct);
        Components.TryGetValue("index", out var components);
        ViewBag.Components = components ?? [];
        var controllerName = ControllerContext.ActionDescriptor.ControllerName;
        if (CheckPermission.Check(User, $"{controllerName}.Create"))
            _indexViewModel.ViewSetting.Create = true;
        if (CheckPermission.Check(User, $"{controllerName}.Edit"))
            AddListAction("ویرایش", "fa fa-pencil", "Edit");
        if (CheckPermission.Check(User, $"{controllerName}.Delete"))
            AddListAction("حذف", "fa fa-trash", "Delete", "ask");
        var list = repository.TableNoTracking;

        if (_includes.Count != 0)
            foreach (var item in _includes)
                list = list.Include(item);

        if (model.Filters.Count != 0)
            foreach (var filter in model.Filters)
                if (!string.IsNullOrEmpty(filter.Value))
                    list = list.Where($"{filter.Key} == @0", filter.Value);

        if (_conditions.Count > 0)
            foreach (var condition in _conditions)
                list = list.Where(condition).AsQueryable();
        var queryRes = await list.OrderDescending().ToListAsync(ct);
        var res = mapper.Map<List<TResDto>>(queryRes);
        _indexViewModel.Rows = res;
        _indexViewModel.Columns = Columns;
        JsFiles.TryGetValue("index", out var jsFiles);
        ViewBag.JsFiles = jsFiles ?? [];
        ViewBag.SelectedFilters = model.Filters;
        var sumItems = new Dictionary<string, string>();
        foreach (var sum in Sums)
        {
            var sumRow = queryRes.Sum(sum.Func);
            if (sum.Type == SumTypeEnum.Time)
            {
                var timeSpan = TimeSpan.FromMinutes(sumRow);
                sumItems.Add(sum.Title, $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}");
            }
            else
            {
                sumItems.Add(sum.Title, sumRow.ToString(CultureInfo.InvariantCulture));
            }
        }

        ViewBag.Sums = sumItems;

        ViewBag.Model = _indexViewModel;
        return View("~/Views/Base/Index.cshtml", _indexViewModel);
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<ViewResult> Create(CancellationToken ct)
    {
        await Configure("create", ct);
        CreateViewModel.Properties = typeof(TDto).GetProperties();
        CreateViewModel.Options = Options;
        JsFiles.TryGetValue("create", out var jsFiles);
        ViewBag.JsFiles = jsFiles ?? [];
        Components.TryGetValue("create", out var components);
        ViewBag.Components = components ?? [];
        return View("~/Views/Base/Create.cshtml", CreateViewModel);
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<IActionResult> Edit(TKey id, CancellationToken ct)
    {
        var query = repository
            .TableNoTracking
            .Where(i => i.Id.Equals(id))
            .AsQueryable();

        if (_includes.Count != 0)
        {
            query = _includes.Aggregate(query, (current, item) => current.Include(item).AsQueryable());
        }

        Model = await query.FirstOrDefaultAsync(ct);
        await Configure("edit", ct);
        EditViewModel.Fields = CreateViewModel.Fields;
        EditViewModel.Properties = typeof(TDto).GetProperties();
        EditViewModel.Options = Options;
        var dto = mapper.Map<TDto>(Model);
        if (dto is null)
            return NotFound();
        ViewBag.Model = EditViewModel;
        return View("~/Views/Base/Edit.cshtml", dto);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create(TDto dto, CancellationToken ct)
    {
        await Configure("create", ct);
        var model = dto.ToEntity(mapper);
        if (!ModelState.IsValid)
        {
            CreateViewModel.Properties = typeof(TDto).GetProperties();
            CreateViewModel.Options = Options;
            CreateViewModel.Error = true;

            JsFiles.TryGetValue("create", out var jsFiles);
            ViewBag.JsFiles = jsFiles ?? [];
            Components.TryGetValue("create", out var components);
            ViewBag.Components = components ?? [];
            return View("~/Views/Base/Create.cshtml", CreateViewModel);
        }

        await repository.AddAsync(model, ct);
        await AfterCreate(dto, model, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Edit(TDto dto, CancellationToken ct)
    {
        await Configure("update", ct);
        var model = await repository.GetByIdAsync(ct, dto.Id);
        if (model is null)
            return NotFound();
        model = dto.ToEntity(model, mapper);
        EditViewModel.Properties = typeof(TDto).GetProperties();
        EditViewModel.Options = Options;
        EditViewModel.Error = false;
        ViewBag.Model = EditViewModel;
        if (!ModelState.IsValid)
        {
            EditViewModel.Error = true;
            return View("~/Views/Base/Edit.cshtml", dto);
        }

        await repository.UpdateAsync(model, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [HasPermission]
    public virtual async Task<IActionResult> Delete(TKey id, CancellationToken ct)
    {
        var model = await repository.GetByIdAsync(ct, id);
        if (model is null)
            return NotFound();
        await repository.DeleteAsync(model, ct);
        return RedirectToAction(nameof(Index));
    }

    public virtual async Task AfterCreate(TDto dto, TEntity model, CancellationToken ct)
    {
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