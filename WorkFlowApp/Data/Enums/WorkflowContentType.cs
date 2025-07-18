using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkFlowApp.Data.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum WorkflowContentType
{
	Task,
	Note,
	Attachment
}
