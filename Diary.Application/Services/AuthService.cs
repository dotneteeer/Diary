using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Diary.Application.Resources;
using Diary.Domain.Dto.Token;
using Diary.Domain.Dto.User;
using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Diary.Domain.Interfaces.Databases;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Diary.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IMapper _mapper;
    private readonly int _refreshTokenValidityInDays;

    public AuthService(IBaseRepository<User> userRepository, IMapper mapper,
        IBaseRepository<UserToken> userTokenRepository, ITokenService tokenService,
        IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork, IOptions<JwtSettings> options)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userTokenRepository = userTokenRepository;
        _tokenService = tokenService;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _refreshTokenValidityInDays = options.Value.RefreshTokenValidityInDays;
    }

    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
        {
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.PasswordMismatch,
                ErrorCode = (int)ErrorCodes.PasswordMismatch
            };
        }

        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
        if (user != null)
        {
            return new BaseResult<UserDto>()
            {
                ErrorMessage = ErrorMessage.UserAlreadyExists,
                ErrorCode = (int)ErrorCodes.UserAlreadyExists
            };
        }

        var hashUserPassword = HashPassword(dto.Password);

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                user = new User
                {
                    Login = dto.Login,
                    Password = hashUserPassword
                };

                await _unitOfWork.Users.CreateAsync(user);

                await _unitOfWork.SaveChangesAsync();

                var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
                if (role == null)
                {
                    return new BaseResult<UserDto>
                    {
                        ErrorMessage = ErrorMessage.RoleNotFound,
                        ErrorCode = (int)ErrorCodes.RoleNotFound
                    };
                }

                UserRole userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                await _unitOfWork.UserRoles.CreateAsync(userRole);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        return new BaseResult<UserDto>
        {
            Data = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Login == dto.Login);
        if (user == null)
        {
            return new BaseResult<TokenDto>()
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };
        }


        if (!IsVerifiedPassword(user.Password, dto.Password))
        {
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.PasswordIsWrong,
                ErrorCode = (int)ErrorCodes.PasswordIsWrong
            };
        }

        var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

        var userRoles = user.Roles;
        var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.Login));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        if (userToken == null)
        {
            userToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenValidityInDays)
            };

            await _userTokenRepository.CreateAsync(userToken);
            await _userTokenRepository.SaveChangesAsync();
        }
        else
        {
            userToken.RefreshToken = refreshToken;
            userToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenValidityInDays);

            _userTokenRepository
                .Update(userToken); //was not checked to return new value(when it was checking by ithomester(lesson 17(25:58)))
            await _userTokenRepository.SaveChangesAsync();
        }

        return new BaseResult<TokenDto>
        {
            Data = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id
            }
        };
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool IsVerifiedPassword(string userPasswordHash, string userPassword)
    {
        var hash = HashPassword(userPassword);
        return userPasswordHash == hash;
    }
}