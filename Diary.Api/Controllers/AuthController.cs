using Diary.Domain.Dto.Token;
using Diary.Domain.Dto.User;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// authentication controller
/// </summary>\
/// <response code="200">If user was registrated/logined</response>
/// <response code="400">If user was not registrated/logined</response>
/// <response code="500">If internal server error occured</response>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// user registartion
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult>> Register([FromBody] RegisterUserDto dto)
    {
        var response = await _authService.Register(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    /// <summary>
    /// user login
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto)
    {
        var response = await _authService.Login(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}