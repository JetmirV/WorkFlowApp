using System.Collections;
using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Data.Entities;

#nullable disable
public class Workflow
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public WorkflowStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<Node> Nodes { get; set; }
	public object Content { get; set; }

	public Workflow()
	{
	}

	public Workflow(string id, string name, string description, DateTime createdAt, List<Node> nodes, object content)
	{
		this.CreatedAt = createdAt;

		if (nodes.Count == 0)
			throw new ArgumentException("Workflow must contain at least one node.", nameof(nodes));

		if (!nodes.Any(x => x.Type == NodeType.Init) || nodes.Count(x => x.Type == NodeType.Init) > 1)
			throw new ArgumentException("Workflow can only contain one Init node.", nameof(nodes));

		var initNode = nodes.FirstOrDefault(x => x.Type == NodeType.Init);
		if (initNode!.Order != 1)
			throw new ArgumentException("Initial node must have an order of 1.", nameof(nodes));

		this.ValidateContent(content);

		this.Id = id;
		this.Name = name;
		this.Description = description;
		this.Nodes = nodes;
	}

	private void ValidateContent(object content)
	{
		if (content is null)
			throw new ArgumentNullException(nameof(content));

		if (content is IEnumerable enumerable && content is not string)
		{
			var validatedList = new List<object>();
			foreach (var item in enumerable)
			{
				validatedList.Add(this.ValidateSingleContent(item));
			}

			this.Content = validatedList;
			return;
		}

		// Handle single item
		this.Content = this.ValidateSingleContent(content);
	}

	private object ValidateSingleContent(object content)
	{
		if (content is WorkflowTask task)
		{
			return new WorkflowTask(
				task.Type,
				task.Title,
				task.AssigneeId,
				task.ReporterId,
				task.WorkFlowId,
				task.Order,
				task.Time,
				task.Priority
			);
		}
		else if (content is WorkflowNote note)
		{
			return new WorkflowNote(
				note.Type,
				note.WorkFlowId,
				note.Text
			);
		}
		else if (content is WorkflowAttachment attachment)
		{
			return new WorkflowAttachment(
				attachment.Type,
				attachment.WorkFlowId,
				attachment.Url
			);
		}

		throw new ArgumentException(
			"Content must be WorkflowTask, WorkflowNote, WorkflowAttachment, or a list of them",
			nameof(content)
		);
	}
}
