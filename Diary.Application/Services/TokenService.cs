using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Diary.Domain.Dto.Token;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using Diary.Domain.Interfaces.Services;
using Diary.Domain.Resources;
using Diary.Domain.Result;
using Diary.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Diary.Application.Services;

public class TokenService : ITokenService
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _jwtKey;
    private readonly int _lifetime;
    private readonly IBaseRepository<User> _userRepository;

    public TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
        _lifetime = options.Value.LifeTime;
        _jwtKey = options.Value.JwtKey;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var securityToken =
            new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.AddMinutes(_lifetime), credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }

    public string GenerateRefreshToken()
    {
        var randomNumbers = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumbers);
        return Convert.ToBase64String(randomNumbers);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
            ValidateLifetime = false,
            ValidAudience = _audience,
            ValidIssuer = _issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var claimsPrincipal =
                tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase)) //Check what is jwtSecurityToken
                throw new SecurityTokenException();

            return claimsPrincipal;
        }
        catch (SecurityTokenException)
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken);
        }
        catch (Exception exception)
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken + exception.Message);
        }
    }

    public async Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto)
    {
        var accessToken = dto.AccessToken;
        var refreshToken = dto.RefreshToken;

        try
        {
            var claimsPrincipal = GetPrincipalFromExpiredToken(accessToken);
            var username = claimsPrincipal.Identity?.Name;

            var user = await _userRepository.GetAll()
                .Include(x => x.UserToken)
                .FirstOrDefaultAsync(x => x.Login == username);

            if (user == null || user.UserToken.RefreshToken != refreshToken ||
                user.UserToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new BaseResult<TokenDto>
                {
                    ErrorMessage = ErrorMessage.InvalidClientRequest
                };
            }

            var newAccessToken = GenerateAccessToken(claimsPrincipal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.UserToken.RefreshToken = newRefreshToken;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new BaseResult<TokenDto>
            {
                Data = new TokenDto
                {
                    RefreshToken = newRefreshToken,
                    AccessToken = newAccessToken,
                    UserId = user.Id
                }
            };
        }
        catch (Exception exception)
        {
            return new BaseResult<TokenDto>
            {
                ErrorMessage = exception.Message
            };
        }
    }
}