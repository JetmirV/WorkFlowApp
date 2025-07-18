
using Newtonsoft.Json;

namespace WorkFlowApp.DTOs.Responses;

#nullable disable
public class LoginResponseDto
{
	[JsonProperty("username")]
	public string UserName { get; set; }

	[JsonProperty("role")]
	public string Role { get; set; }

	[JsonProperty("Name")]
	public string Name { get; set; }

	[JsonProperty("accessToken")]
	public string AccessToken { get; set; }

	[JsonProperty("refreshToken")]
	public string RefreshToken { get; set; }
}
