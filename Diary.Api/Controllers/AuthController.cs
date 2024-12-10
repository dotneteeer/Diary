using Diary.Domain.Dto.Token;
using Diary.Domain.Dto.User;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
    private readonly JwtSettings _jwtSettings;

    public AuthController(IAuthService authService, IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, response.Data.Principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.LifeTime),
                    IsPersistent = true
                });
            response.Data.Principal = null;
            return Ok(response);
        }

        return BadRequest(response);
    }
}