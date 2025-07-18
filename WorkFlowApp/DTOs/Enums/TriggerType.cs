using System.Runtime.Serialization;

namespace WorkFlowApp.DTOs.Enums;

public enum TriggerType
{
	[EnumMember(Value = "user-message")]
	UserMessage
}
