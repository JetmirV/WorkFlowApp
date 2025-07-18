using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WorkFlowApp.Models;

namespace WorkFlowApp.Interfaces;
public interface IJwtAuthManager
{
	(ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
	JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now);
	JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
	void RemoveRefreshTokenByUserName(string username);
}