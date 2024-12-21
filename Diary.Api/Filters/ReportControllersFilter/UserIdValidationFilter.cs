using System.Security.Claims;
using Diary.Domain.Enum;
using Diary.Domain.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Diary.Api.Filters.ReportControllersFilter;

/// <summary>
/// Validates if id of user is equals to transmitted userId
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class UserIdValidationFilter : ValidatorFilterBaseAttribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Gets name of identifier to compare with same identifier of User
    /// </summary>
    /// <param name="identifierName">Name of identifier to compare with same identifier of User</param>
    public UserIdValidationFilter(string? identifierName = null)
    {
        IdentifierName = identifierName ?? "userId";
    }

    /// <inheritdoc />
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var userId = context.RouteData.Values[IdentifierName]?.ToString();
        if (userId == null)
        {
            context.HttpContext.Request.EnableBuffering();
            var bodyStream = new StreamReader(context.HttpContext.Request.Body);
            var bodyText = await bodyStream.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;

            dynamic requestBody = JsonConvert.DeserializeObject(bodyText);
            userId = (requestBody?[IdentifierName] ?? requestBody?.UserId)?.ToString();
        }

        userId.ThrowIfNullWithReferenceObj(IdentifierName);

        var userRole = user.FindAll(ClaimTypes.Role);
        string[] canGetAnyDataRoles =
        [
            nameof(Roles.Admin),
            nameof(Roles.Moderator)
        ];
        var canGetAnyData = userRole.Any(currentRole => canGetAnyDataRoles.Any(role => role == currentRole.Value));
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var isSame = id == userId;
        if (!(canGetAnyData || isSame))
        {
            context.Result = new ForbidResult(AuthenticationScheme);
        }
    }
}