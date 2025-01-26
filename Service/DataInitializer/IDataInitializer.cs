using Common;

namespace Services.DataInitializer;

public interface IDataInitializer: IScopedDependency
{
    public void InitializerData();
}