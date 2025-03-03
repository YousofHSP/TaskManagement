using System.Security.Claims;

namespace Presentation.Helpers;

public static class CheckPermission
{
    

    public static bool Check(ClaimsPrincipal user, string[] permissions, bool checkAll = false)
    {

        if (user.IsInRole("Admin"))
            return true;
        var userPermissions = user.Claims
            .Where(c => c.Type == "Permission")
            .Select(i => i.Value)
            .ToHashSet();
        return checkAll
            ? permissions.All(p => userPermissions.Contains(p))
            : permissions.Any(p => userPermissions.Contains(p));
    }

    public static bool Check(ClaimsPrincipal user, string permission)
    {
        return Check(user, [permission], true);
    }
}