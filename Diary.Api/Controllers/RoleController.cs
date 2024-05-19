using System.Net.Mime;
using Diary.Domain.Dto.Role;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// Role controller
/// </summary>
/// <response code="200">If role was created/deleted/updated/get/added</response>
/// <response code="400">If role was not created/deleted/updated/get/added</response>
/// <response code="500">If internal server error occured</response>
[Consumes(MediaTypeNames.Application.Json)]
[Authorize(Roles="User")]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    /// <summary>
    /// Create role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     POST
    ///     {
    ///         "name":"Admin"
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<Role>>> Create([FromBody] CreateRoleDto dto)
    {
        var response = await _roleService.CreateRoleAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
    /// <summary>
    /// Deletes role by its id
    /// </summary>
    /// <param name="id">role's id</param>
    /// <returns></returns>
    /// /// <remarks>
    /// Request for create report:
    /// 
    ///     DELETE
    ///     {
    ///         "id":1
    ///     }
    /// </remarks>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<BaseResult<Role>>> Delete(long id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for create report:
    /// 
    ///     UPDATE
    ///     {
    ///         "id":1,
    ///         "name":"Admin"
    ///         
    ///     }
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<BaseResult<Role>>> Update([FromBody] RoleDto dto)
    {
        var response = await _roleService.UpdateRoleAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    /// <summary>
    /// Adding role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for add role for user:
    /// 
    ///     POST
    ///     {
    ///         "login":"user1",
    ///         "roleName":"Admin"
    ///     }
    /// </remarks>
    [HttpPost("addRole")]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> AddRoleForUser([FromBody] UserRoleDto dto)
    {
        var response = await _roleService.AddRoleForUserAsync(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}