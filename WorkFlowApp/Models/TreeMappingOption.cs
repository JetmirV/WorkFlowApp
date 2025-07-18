using Newtonsoft.Json;

namespace WorkFlowApp.Models;

#nullable disable
public class TreemapOption
{
	[JsonProperty("series")]
	public TreemapSeries Series { get; set; }
}

public class TreemapSeries
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("data")]
	public List<TreemapData> Data { get; set; }
}

public class TreemapData
{
	[JsonProperty("value")]
	public double Value { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }
}
