using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReadingIsGood.BuildingBlocks.Common.Wrapper;

namespace ReadingIsGood.Application.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class CustomAuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        if (!httpContext.Items.TryGetValue("UserId", out var userId) || string.IsNullOrEmpty(userId?.ToString()))
        {
            context.Result = new JsonResult(BaseResponse<string>.Fail(
                "Authorization token is either incorrect or missing.",
                StatusCodes.Status401Unauthorized));
            return;
        }

        if (roles.Length > 0)
        {
            if (!httpContext.Items.TryGetValue("Roles", out var rolesObj) || rolesObj is not string userRole)
            {
                context.Result = new JsonResult(BaseResponse<string>.Fail(
                    "Roles not found in request context.",
                    StatusCodes.Status403Forbidden));
                return;
            }

            if (!roles.Contains(userRole))
            {
                context.Result = new JsonResult(BaseResponse<string>.Fail(
                    "You do not have permission to access this resource.",
                    StatusCodes.Status403Forbidden));
            }
        }
    }
}