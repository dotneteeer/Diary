using Asp.Versioning;
using Diary.Domain.Dto.Report;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Repport controller
/// </summary>
/// <response code="200">If report was created/deleted/updated/get</response>
/// <response code="400">If report was not created/deleted/updated/get</response>
/// <response code="500">If internal server error occured</response>
/// <response code="401">If user is unauthorized</response>
[Authorize]
[ApiController]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]

public class ReportController:ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    /// <summary>
    /// Get reports of user
    /// </summary>
    /// <param name="userId"></param>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     GET
    ///     {
    ///         "userId":1
    ///         
    ///     }
    /// </remarks>

    [HttpGet("reports/{userId:int:min(0)}")]//":int:min(0)" added by myself
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId)
    {
        var response = await _reportService.GetReportsAsync(userId);
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

    [HttpGet("{id:int:min(0)}")]//":int:min(0)" added by myself
    public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
    {
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
    public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody]CreateReportDto dto)
    {
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
    public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromBody]UpdateReportDto dto)
    {
        var response = await _reportService.UpdateReportAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
}