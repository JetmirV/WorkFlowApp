using Newtonsoft.Json;

namespace WorkFlowApp.Models;

#nullable disable
public class PieOption
{
	[JsonProperty("series")]
	public PieSeries Series { get; set; }
}

public class PieSeries
{
	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("data")]
	public List<PieData> Data { get; set; }
}

public class PieData
{
	[JsonProperty("value")]
	public double Value { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("itemStyle")]
	public ItemStyle ItemStyle { get; set; }
}

public class ItemStyle
{
	[JsonProperty("normal")]
	public ColorState Normal { get; set; }

	[JsonProperty("emphasis")]
	public ColorState Emphasis { get; set; }
}

public class ColorState
{
	[JsonProperty("color")]
	public string Color { get; set; }
}
