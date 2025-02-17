namespace Entity;

public static class Permissions
{
    public static List<Permission> All =
    [
        
        new() { Controller = "Customer", ControllerLabel = "مشتری",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Customer", ControllerLabel = "مشتری", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Role", ControllerLabel = "نقش",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Event", ControllerLabel = "رویداد",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Event", ControllerLabel = "رویداد", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Event", ControllerLabel = "رویداد", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Event", ControllerLabel = "رویداد", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Plan", ControllerLabel = "پلن",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Plan", ControllerLabel = "پلن", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Plan", ControllerLabel = "پلن", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Plan", ControllerLabel = "پلن", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "User", Action = "Index", ControllerLabel = "کاربر", ActionLabel = "نمایش" },
        new() { Controller = "User", Action = "Create", ControllerLabel = "کاربر", ActionLabel = "ایجاد" },
        new() { Controller = "User", Action = "Edit", ControllerLabel = "کاربر", ActionLabel = "ویرایش" },
        new() { Controller = "User", Action = "Delete", ControllerLabel = "کاربر", ActionLabel = "حذف" },
        
        new() { Controller = "Job", ControllerLabel = "تسک",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Job", ControllerLabel = "تسک", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Job", ControllerLabel = "تسک", Action = "Edit",  ActionLabel = "ویرایش" },
        new() { Controller = "Job", ControllerLabel = "تسک", Action = "Delete", ActionLabel = "حذف" },
        new() { Controller = "Job", ControllerLabel = "تسک", Action = "Report", ActionLabel = "گزارش" },
        new() { Controller = "Job", Action = "ShowAllInfo", ControllerLabel = "تسک", ActionLabel = "نمایش اطلاعات دیگران" },
    ];
}

public class Permission
{
    public required string Controller { get; set; }
    public required string ControllerLabel { get; set; }
    public required string Action { get; set; }
    public required string ActionLabel { get; set; }
}