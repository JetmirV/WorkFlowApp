using Newtonsoft.Json;

namespace WorkFlowApp.Models;

#nullable disable
public class ChartOption1
{
	[JsonProperty("xAxis")]
	public Axis XAxis { get; set; }

	[JsonProperty("yAxis")]
	public Axis YAxis { get; set; }

	[JsonProperty("series")]
	public List<Series> Series { get; set; }
}

public class Axis
{
	[JsonProperty("type")]
	public string Type { get; set; }

	// for xAxis only
	[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
	public List<string>? Data { get; set; }
}

public class Series
{
	[JsonProperty("data")]
	public List<double> Data { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }
}
