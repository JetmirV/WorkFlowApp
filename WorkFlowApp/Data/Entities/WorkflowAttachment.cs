using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Data.Entities;

public class WorkflowAttachment
{
	public WorkflowContentType Type { get; set; }
	public string WorkFlowId { get; set; }
	public Uri Url { get; set; }

	public WorkflowAttachment(WorkflowContentType type, string workFlowId, Uri url)
	{
		if (string.IsNullOrWhiteSpace(workFlowId))
			throw new ArgumentException("Workflow ID cannot be null or empty.", nameof(workFlowId));

		if (url == null)
			throw new ArgumentNullException(nameof(url), "URL cannot be null.");
		if (!Uri.IsWellFormedUriString(url.ToString(), UriKind.Absolute))
			throw new ArgumentException("URL is not well-formed.", nameof(url));

		this.Type = type;
		this.WorkFlowId = workFlowId;
		this.Url = url;
	}
}
