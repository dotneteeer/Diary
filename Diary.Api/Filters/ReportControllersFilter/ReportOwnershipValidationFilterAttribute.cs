using System.Security.Claims;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Extensions;
using Diary.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Diary.Api.Filters.ReportControllersFilter;

/// <summary>
/// Validates if user request his report 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ReportOwnershipValidationFilterAttribute : ValidatorFilterBaseAttribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Gets name of identifier to compare with same identifier of User
    /// </summary>
    /// <param name="identifierName">Name of identifier to compare with same identifier of User</param>
    public ReportOwnershipValidationFilterAttribute(string? identifierName = null)
    {
        IdentifierName = identifierName ?? "id";
    }


    /// <inheritdoc />
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var reportRepository = context.HttpContext.RequestServices.GetService<IBaseRepository<Report>>();
        var user = context.HttpContext.User;
        var id = context.RouteData.Values[IdentifierName]?.ToString().ToNullableLong();
        if (id == null)
        {
            context.HttpContext.Request.EnableBuffering();
            var bodyStream = new StreamReader(context.HttpContext.Request.Body);
            var bodyText = await bodyStream.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;

            dynamic requestBody = JsonConvert.DeserializeObject(bodyText);
            id = long.Parse((requestBody?[IdentifierName] ?? requestBody?.id)?.ToString());
        }

        id.ThrowIfNullWithReferenceObj(IdentifierName);

        var userRole = user.FindAll(ClaimTypes.Role);
        string[] canGetAnyDataRoles =
        [
            nameof(Roles.Admin),
            nameof(Roles.Moderator)
        ];
        var canGetAnyData = userRole.Any(currentRole => canGetAnyDataRoles.Any(role => role == currentRole.Value));
        var userId = long.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ifUserHasReports = reportRepository.GetAll().Any(x => x.Id == id && x.UserId == userId);
        if (!(canGetAnyData || ifUserHasReports))
        {
            context.Result = new ForbidResult(AuthenticationScheme);
        }
    }
}