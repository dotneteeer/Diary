using System.Security.Claims;
using Diary.Domain.Dto.Token;
using Diary.Domain.Result;

namespace Diary.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    
    Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto);
}