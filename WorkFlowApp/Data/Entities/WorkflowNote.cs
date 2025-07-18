using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Data.Entities;

public class WorkflowNote
{
	public WorkflowContentType Type { get; set; }
	public string WorkFlowId { get; set; }
	public string Text { get; set; }

	public WorkflowNote(WorkflowContentType type, string workFlowId, string text)
	{
		if (string.IsNullOrWhiteSpace(workFlowId))
			throw new ArgumentException("Workflow ID cannot be null or empty.", nameof(workFlowId));

		if (string.IsNullOrWhiteSpace(text))
			throw new ArgumentException("Text cannot be null or empty.", nameof(text));

		if (text.Length > 500)
			throw new ArgumentException("Text cannot exceed 500 characters.", nameof(text));

		this.Type = type;
		this.WorkFlowId = workFlowId;
		this.Text = text;
	}
}
