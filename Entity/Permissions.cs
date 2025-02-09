namespace Entity;

public static class Permissions
{
    public static Dictionary<string, string> AllPermissions = new()
    {
        { "User.Index", "نمایش کابران" },
        { "User.Create", "ایجاد کابران" },
        { "User.Edit", "ویرایش کابران" },
        { "User.Delete", "حذف کابران" },
        
        { "Customer.Index", "نمایش مشتریان" },
        { "Customer.Create", "ایجاد مشتریان" },
        { "Customer.Edit", "ویرایش مشتریان" },
        { "Customer.Delete", "حذف مشتریان" },
        
        { "Role.Index", "نمایش نقش" },
        { "Role.Create", "ایجاد نقش" },
        { "Role.Edit", "ویرایش نقش" },
        { "Role.Delete", "حذف نقش" },
    };

}