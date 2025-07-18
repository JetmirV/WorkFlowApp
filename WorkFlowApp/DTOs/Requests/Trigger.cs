using Newtonsoft.Json;
using WorkFlowApp.DTOs.Enums;

namespace WorkFlowApp.DTOs.Requests;

#nullable disable
public class Trigger
{
	[JsonProperty("userid")]
	public string UserId { get; set; }

	[JsonProperty("type")]
	public TriggerType Type { get; set; }

	[JsonProperty("message")]
	public string Message { get; set; }
}
