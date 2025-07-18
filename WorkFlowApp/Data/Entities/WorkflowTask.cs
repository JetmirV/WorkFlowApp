using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Data.Entities;

public class WorkflowTask
{
	public WorkflowContentType Type { get; set; }
	public string Title { get; set; }
	public string AssigneeId { get; set; }
	public string ReporterId { get; set; }
	public string WorkFlowId { get; set; }
	public int Order { get; set; }
	public string Time { get; set; }
	public TaskPriority Priority { get; set; }

	public WorkflowTask(WorkflowContentType type, string title, string assigneeId, string reporterId, string workFlowId, int order, string time, TaskPriority priority)
	{
		if (!Enum.IsDefined(typeof(WorkflowContentType), type))
			throw new ArgumentException("Type must be a valid WorkflowContentType value.", nameof(type));

		if(string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Title cannot be null or empty.", nameof(title));

		if (string.IsNullOrWhiteSpace(assigneeId))
			throw new ArgumentException("AssigneeId cannot be null or empty.", nameof(assigneeId));

		if (string.IsNullOrWhiteSpace(reporterId))
			throw new ArgumentException("ReporterId cannot be null or empty.", nameof(reporterId));

		if (string.IsNullOrWhiteSpace(workFlowId))
			throw new ArgumentException("WorkFlowId cannot be null or empty.", nameof(workFlowId));

		if (order <= 0)
			throw new ArgumentException("Order must be greater than zero.", nameof(order));

		if (!IsValidUnixTimestamp(time))
			throw new ArgumentException("Time must be a valid Unix timestamp.", nameof(time));

		if (!Enum.IsDefined(typeof(TaskPriority), priority))
			throw new ArgumentException("Priority must be a valid TaskPriority value.", nameof(priority));

		this.Type = type;
		this.Title = title;
		this.AssigneeId = assigneeId;
		this.ReporterId = reporterId;
		this.WorkFlowId = workFlowId;
		this.Order = order;
		this.Time = time;
		this.Priority = priority;
	}

	public static bool IsValidUnixTimestamp(string text)
	{
		if (!long.TryParse(text, out var seconds))
			return false;

		try
		{
			var dto = DateTimeOffset.FromUnixTimeSeconds(seconds);

			if (dto.Year < 1970 || dto.Year > 3000)
				return false;

			return true;
		}
		catch (ArgumentOutOfRangeException)
		{
			return false;
		}
	}
}
