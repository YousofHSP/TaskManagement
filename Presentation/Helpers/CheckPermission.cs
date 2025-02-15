using System.Security.Claims;

namespace Presentation.Helpers;

public static class CheckPermission
{
    

    public static bool Check(ClaimsPrincipal user, string permission)
    {

        if (user.IsInRole("Admin"))
            return true;
        return user.Claims.Any(c => c.Type == "Permission" && c.Value == permission);
    }
}