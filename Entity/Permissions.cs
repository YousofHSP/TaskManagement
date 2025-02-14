namespace Entity;

public static class Permissions
{
    public static List<Permission> All =
    [
        new() { Controller = "User", Action = "Index", ControllerLabel = "کاربر", ActionLabel = "نمایش" },
        new() { Controller = "User", Action = "Create", ControllerLabel = "کاربر", ActionLabel = "ایجاد" },
        new() { Controller = "User", Action = "Edit", ControllerLabel = "کاربر", ActionLabel = "ویرایش" },
        new() { Controller = "User", Action = "Delete", ControllerLabel = "کاربر", ActionLabel = "حذف" },
        
        new() { Controller = "Customer", ControllerLabel = "مشتری",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Role", ControllerLabel = "نقش",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Delete", ActionLabel = "حذف" },
    ];
}

public class Permission
{
    public required string Controller { get; set; }
    public required string ControllerLabel { get; set; }
    public required string Action { get; set; }
    public required string ActionLabel { get; set; }
}