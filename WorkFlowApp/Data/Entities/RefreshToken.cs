
using Newtonsoft.Json;

namespace WorkFlowApp.Data.Entities;

#nullable disable
public class RefreshToken
{
	[JsonIgnore]
	public long Id { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; }

	[JsonProperty("tokenString")]
	public string TokenString { get; set; }

	[JsonProperty("expireAt")]
	public DateTime ExpireAt { get; set; }
}
