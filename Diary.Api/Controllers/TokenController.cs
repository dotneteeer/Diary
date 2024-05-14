using Diary.Domain.Dto;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace Diary.Api.Controllers;

/// <summary>
/// 
/// </summary>
[ApiController]
public class TokenController : Controller
{
    private readonly ITokenService _tokenService;
    
    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult> RereshToken([FromBody] TokenDto dto)
    {
        var response = await _tokenService.RefreshToken(dto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}