
namespace Presentation.Models;

public class ViewSetting
{
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
}

public class ListAction
{
    public string Title { get; set; }
    public string Class { get; set; }
    public string Url { get; set; }
}

public class IndexViewModel<TModel>
{
    public string Title { get; set; } = "";
    public ViewSetting ViewSetting { get; set; } = new();
    public List<Field> Filters { get; set; } = [];
    public List<Column> Columns { get; set; } = [];
    public List<TModel> Rows { get; set; } = [];
    public List<ListAction> ListActions { get; set; } = [];
}