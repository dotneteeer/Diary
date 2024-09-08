using System.Security.Claims;
using Asp.Versioning;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Report controller
/// </summary>
/// <response code="200">If report was created/deleted/updated/received</response>
/// <response code="400">If report was not created/deleted/updated/received</response>
/// <response code="500">If internal server error occured</response>
/// <response code="401">If user is unauthorized</response>
/// <response code="403">If user is forbidden to make request</response>
[Authorize]
[ApiController]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportController : ControllerBase
{
    private const string Bearer = "Bearer";
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService, IBaseRepository<Report> reportRepository)
    {
        _reportService = reportService;
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Get reports of user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="pageReportDto">Received page number and size</param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     GET
    ///     {
    ///         "pageNumber":1,
    ///         "pageSize":5
    ///     }
    /// </remarks>
    [HttpGet("reports/{userId:int:min(0)}")] //":int:min(0)" added by myself
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId,
        [FromQuery] PageReportDto? pageReportDto)
    {
        if (!CheckIsUserAllowedToGetData(userId.ToString()))
        {
            return Forbid(Bearer);
        }

        var response = await _reportService.GetReportsAsync(userId, pageReportDto);

        Response.Headers.Append("x-total-count", response.TotalCount.ToString());

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Gets report by id
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     GET
    ///     {
    ///         "id":1
    ///         
    ///     }
    /// </remarks>
    [HttpGet("{id:int:min(0)}")] //":int:min(0)" added by myself
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
        if (!CheckIsAnyDataBelongsToUser(id))
        {
            return Forbid(Bearer);
        }

        var response = await _reportService.GetReportByIdAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Deletes report
    /// </summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     DELETE
    ///     {
    ///         "id":1
    ///     }
    /// </remarks>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<BaseResult<ReportDto>>> Delete(long id)
    {
        if (!CheckIsAnyDataBelongsToUser(id))
        {
            return Forbid(Bearer);
        }

        var response = await _reportService.DeleteReportAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Creates Report
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "name":"string",
    ///         "description":"string",
    ///         "userId":1
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody] CreateReportDto dto)
    {
        if (!CheckIsUserAllowedToGetData(dto.UserId.ToString()))
        {
            return Forbid(Bearer);
        }

        var response = await _reportService.CreateReportAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Updates report
    /// </summary>
    /// <param name="dto"></param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     UPDATE
    ///     {
    ///         "id":1,
    ///         "name":"string",
    ///         "description":"string"
    ///         
    ///     }
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromBody] UpdateReportDto dto)
    {
        if (!CheckIsAnyDataBelongsToUser(dto.Id))
        {
            return Forbid(Bearer);
        }

        var response = await _reportService.UpdateReportAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    private bool CheckIsUserAllowedToGetData(string identifier)
    {
        var userRole = User.FindAll(ClaimTypes.Role);
        string[] canGetAnyDataRoles =
        [
            nameof(Roles.Admin),
            nameof(Roles.Moderator)
        ];
        var canGetAnyData = userRole.Any(currentRole => canGetAnyDataRoles.Any(role => role == currentRole.Value));
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isSame = id == identifier;
        return canGetAnyData || isSame;
    }

    private bool CheckIsAnyDataBelongsToUser(long id)
    {
        var userRole = User.FindAll(ClaimTypes.Role);
        string[] canGetAnyDataRoles =
        [
            nameof(Roles.Admin),
            nameof(Roles.Moderator)
        ];
        var canGetAnyData = userRole.Any(currentRole => canGetAnyDataRoles.Any(role => role == currentRole.Value));
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userReports = _reportRepository.GetAll().AsEnumerable().Where(x => x.UserId == userId);
        return canGetAnyData || userReports.Any(x => x.Id == id);
    }
}