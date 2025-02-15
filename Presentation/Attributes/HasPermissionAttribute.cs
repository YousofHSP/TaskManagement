using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentation.Helpers;

namespace Presentation.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private string? _permission;

    public HasPermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public HasPermissionAttribute()
    {
        _permission = null;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }


        if (_permission is null)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            _permission = $"{controller}.{action}";
        }

        var hasPermission = CheckPermission.Check(user, _permission);
        if (!hasPermission)
            context.Result = new ForbidResult();
    }
}