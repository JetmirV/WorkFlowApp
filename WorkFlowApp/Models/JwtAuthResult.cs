using Newtonsoft.Json;
using WorkFlowApp.Data.Entities;

namespace WorkFlowApp.Models;

#nullable disable
public class JwtAuthResult
{
	[JsonProperty("accessToken")]
	public string AccessToken { get; set; }

	[JsonProperty("refreshToken")]
	public RefreshToken RefreshToken { get; set; }
}
