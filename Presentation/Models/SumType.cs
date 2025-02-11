namespace Presentation.Models;

public class SumType<T>
{
    public string Title { get; set; }
    public SumTypeEnum Type{ get; set; }
    public Func<T, double> Func { get; set; }
}

public enum SumTypeEnum
{
    Price,
    Time,
    Count
}