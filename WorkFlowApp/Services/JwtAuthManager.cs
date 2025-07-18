using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WorkFlowApp.Data.Entities;
using WorkFlowApp.Interfaces;
using WorkFlowApp.Models;

namespace WorkFlowApp.Services;

public class JwtAuthManager : IJwtAuthManager
{
	private readonly JwtTokenConfig _jwtTokenConfig;
	private readonly byte[] _secret;

	private readonly IDataRepo _dataRepo;

	public JwtAuthManager(JwtTokenConfig jwtTokenConfig, IDataRepo repository)
	{
		this._jwtTokenConfig = jwtTokenConfig;
		this._secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
		this._dataRepo = repository;
	}

	public void RemoveRefreshTokenByUserName(string username)
	{
		var refreshToken = this._dataRepo.GetRefreshTokenForUser(username).FirstOrDefault();

		if (refreshToken is not null)
			this._dataRepo.DeleteRefreshTokenById(refreshToken.Id);
	}

	public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now)
	{
		var jwtToken = new JwtSecurityToken(
			this._jwtTokenConfig.Issuer,
			null,
			claims,
			expires: now.AddMinutes(this._jwtTokenConfig.AccessTokenExpiration),
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(this._secret), SecurityAlgorithms.HmacSha256Signature));
		var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

		var refreshToken = new RefreshToken
		{
			Username = username,
			TokenString = GenerateRefreshTokenString(),
			ExpireAt = now.AddMinutes(this._jwtTokenConfig.RefreshTokenExpiration)
		};

		this._dataRepo.AddRefreshToken(refreshToken);

		return new JwtAuthResult
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	public JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now)
	{
		var (principal, jwtToken) = this.DecodeJwtToken(accessToken);
		if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
		{
			throw new SecurityTokenException("Invalid token");
		}

		var userName = principal.Identity?.Name;
		var existingRefreshToken = this._dataRepo.GetRefreshTokenForUser(userName!).FirstOrDefault();
		if (existingRefreshToken is null)
			throw new SecurityTokenException("Invalid token");

		if (existingRefreshToken.Username != userName || existingRefreshToken.ExpireAt < now)
			throw new SecurityTokenException("Invalid token");

		return this.GenerateTokens(userName, principal.Claims.ToArray(), now); // need to recover the original claims
	}

	public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new SecurityTokenException("Invalid token");
		}
		var principal = new JwtSecurityTokenHandler()
			.ValidateToken(token,
				new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = this._jwtTokenConfig.Issuer,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(this._secret),
					ValidAudience = string.Empty,
					ValidateAudience = true,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.FromMinutes(1)
				},
				out var validatedToken);
		return (principal, validatedToken as JwtSecurityToken);
	}

	private static string GenerateRefreshTokenString()
	{
		var randomNumber = new byte[32];
		using var randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}
