using Diary.Domain.Dto.Role;
using Diary.Domain.Entity;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

/// <summary>
/// Service for roles' control
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Role creating
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> CreateRoleAsync(RoleDto dto);
    
    /// <summary>
    /// Role deleting
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> DeleteRoleAsync(long id);
    
    /// <summary>
    /// Role updating
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> UpdateRoleAsync(RoleDto dto);
    
}