using System.Security.Claims;

namespace Diary.Domain.Dto.Token;

public class TokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public ClaimsPrincipal Principal { get; set; }

    public long UserId { get; set; }
}