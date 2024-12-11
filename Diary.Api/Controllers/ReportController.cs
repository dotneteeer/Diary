using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using Diary.Api.Filters.ReportControllersFilter;
using Diary.Domain.Dto.Report;
using Diary.Domain.Entity;
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
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Report> _reportRepository;
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService, IBaseRepository<Report> reportRepository, IMapper mapper)
    {
        _reportService = reportService;
        _reportRepository = reportRepository;
        _mapper = mapper;
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
    [UserIdValidationFilter("userId")]
    public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId,
        [FromQuery] PageReportDto? pageReportDto)
    {
        if (pageReportDto!.PageNumber != 0 && pageReportDto.PageSize != 0)
        {
            if (HttpContext.Session.GetInt32(nameof(PageReportDto.PageNumber)) == pageReportDto.PageNumber &&
                HttpContext.Session.GetInt32(nameof(PageReportDto.PageSize)) == pageReportDto.PageSize)
            {
                var reports = JsonSerializer.Deserialize<ReportDto[]>(HttpContext.Session.GetString("Reports")!);
                return Ok(new CollectionResult<ReportDto>
                {
                    Data = reports
                });
            }
        }

        var response = await _reportService.GetReportsAsync(userId, pageReportDto);

        Response.Headers.Append("x-total-count", response.TotalCount.ToString());

        if (pageReportDto!.PageNumber != 0 && pageReportDto.PageSize != 0)
        {
            HttpContext.Session.SetInt32(nameof(PageReportDto.PageNumber), pageReportDto.PageNumber);
            HttpContext.Session.SetInt32(nameof(PageReportDto.PageSize), pageReportDto.PageSize);

            var json = JsonSerializer.Serialize(response.Data);
            HttpContext.Session.SetString("Reports", json);
        }

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
    [ReportOwnershipValidationFilter("id")]
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
    [ReportOwnershipValidationFilter("id")]
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
    [UserIdValidationFilter("userId")]
    public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody] CreateReportDto dto)
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
    [ReportOwnershipValidationFilter("id")]
    public async Task<ActionResult<BaseResult<ReportDto>>> Update([FromBody] UpdateReportDto dto)
    {
        var response = await _reportService.UpdateReportAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}