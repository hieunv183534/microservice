using System.Text.Json;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class ClaimRequirementFilter : IAuthorizationFilter
{
    private readonly CommandCode _commandCode;
    private readonly FunctionCode _functionCode;

    public ClaimRequirementFilter(FunctionCode functionCode, CommandCode commandCode)
    {
        _functionCode = functionCode;
        _commandCode = commandCode;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var permissionsClaim = context.HttpContext.User.Claims
            .SingleOrDefault(c => c.Type.Equals(SystemConstants.Claims.Permissions));
        if (permissionsClaim != null)
        {
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);
            if (!permissions.Contains(PermissionHelper.GetPermission(_functionCode, _commandCode))) 
                context.Result = new ForbidResult();
        }
        else
        {
            context.Result = new ForbidResult();
        }
    }
}