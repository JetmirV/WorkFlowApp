using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Data.Entities;

#nullable disable
public class Node
{
	public string Id { get; set; }

	[JsonProperty("type")]
	[JsonConverter(typeof(StringEnumConverter))]
	public NodeType Type { get; set; }
	public int Order { get; set; }
	public NodeStatus Status { get; set; }
}
