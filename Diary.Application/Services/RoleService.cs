using Diary.Application.Resources;
using Diary.Domain.Dto.Role;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace Diary.Application.Services;

public class RoleService : IRoleService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;

    public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<BaseResult<Role>> CreateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
        if (role != null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleAlreadyExists,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExists
            };
        }

        role = new Role
        {
            Name = dto.Name
        };
        await _roleRepository.CreateAsync(role);
        return new BaseResult<Role>
        {
            Data = role
        };
    }

    public async Task<BaseResult<Role>> DeleteRoleAsync(long id)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        if (role == null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };
        }
        
        await _roleRepository.RemoveAsync(role);
        return new BaseResult<Role>
        {
            Data = role
        };
    }

    public async Task<BaseResult<Role>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (role == null)
        {
            return new BaseResult<Role>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExists
            };
        }

        role.Name = dto.Name;
        await _roleRepository.UpdateAsync(role);
        return new BaseResult<Role>
        {
            Data = role
        };
    }
}