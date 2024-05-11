using Diary.Domain.Dto;
using Diary.Domain.Dto.User;
using Diary.Domain.Entity;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

/// <summary>
/// Authorization service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// user registration 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> Register(RegisterUserDto dto);
    /// <summary>
    /// user authorization 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> Login(LoginUserDto dto);
}