using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkFlowApp.DTOs.Requests;
using WorkFlowApp.DTOs.Responses;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IEncryptionService _encryptionService;
	private readonly IJwtAuthManager _jwtAuthManager;

	public AccountController(IUserService userService, IEncryptionService encryptionService, IJwtAuthManager jwtAuthManager)
	{
		this._userService = userService;
		this._encryptionService = encryptionService;
		this._jwtAuthManager = jwtAuthManager;
	}

	[AllowAnonymous]
	[HttpPost("login")]
	public IActionResult Login([FromBody] LoginRequestDto request)
	{
		if (!this.ModelState.IsValid)
			return this.BadRequest(this.ModelState);

		var user = this._userService.DoesUserHaveValidCredentials(request.Username, this._encryptionService.Encrypt(request.Password)!);

		if (user is null)
			return this.Unauthorized("Wrong credentials");

		var claims = new[]
		{
			new Claim(ClaimTypes.Name,request.Username),
			new Claim(ClaimTypes.GivenName, user.Name),
			new Claim(ClaimTypes.Role, user.Role),
			new Claim("Scope", user.Scope),
		};

		var jwtResult = this._jwtAuthManager.GenerateTokens(request.Username, claims, DateTime.Now);

		var response = new
		{
			status = "ok",
			model = new LoginResponseDto
			{
				UserName = request.Username,
				Name = user.Name,
				Role = user.Role,
				AccessToken = jwtResult.AccessToken,
				RefreshToken = jwtResult.RefreshToken.TokenString,
			}
		};

		return this.Ok(response);
	}

	[HttpPost("logout")]
	public IActionResult Logout()
	{
		var userName = this.User.Identity?.Name;
		this._jwtAuthManager.RemoveRefreshTokenByUserName(userName!);
		return this.Ok(new { message = "User logged out" });
	}

	[AllowAnonymous]
	[HttpPost("signin")]
	public IActionResult Signin([FromBody] SigninRequestDto requestDto)
	{
		if (!this.ModelState.IsValid)
			return this.BadRequest(this.ModelState);

		this._userService.CreateUser(requestDto);

		return this.Ok();
	}

	[HttpPost("refresh-token")]
	public async Task<IActionResult> RefreshToken(string refreshToken)
	{
		var userName = this.User.Identity?.Name;

		if (string.IsNullOrWhiteSpace(refreshToken))
		{
			return this.Unauthorized(new { message = "Not Authorized!" });
		}

		var accessToken = await this.HttpContext.GetTokenAsync("Bearer", "access_token");

		var jwtResult = this._jwtAuthManager.Refresh(refreshToken, accessToken!, DateTime.Now);
		if (jwtResult is null)
			return this.BadRequest("Invalid tokens");

		var response = new LoginResponseDto
		{
			UserName = userName!,
			Name = this.User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
			Role = this.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
			AccessToken = jwtResult.AccessToken,
			RefreshToken = jwtResult.RefreshToken.TokenString
		};
		return this.Ok(response);
	}
}

