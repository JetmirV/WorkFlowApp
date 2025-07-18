using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkFlowApp.Data.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum NodeType
{
	Init,
	Condition,
	ModifyMessage,
	StoreMessage,
}
