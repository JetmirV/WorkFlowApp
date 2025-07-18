using System.Linq;
using WorkFlowApp.Data.Entities;
using WorkFlowApp.Data.Enums;
using WorkFlowApp.DTOs.Requests;
using WorkFlowApp.Interfaces;
using WorkFlowApp.Services.Nodes;

namespace WorkFlowApp.Services;

public class WorkflowService : IWorkflowService
{
	private readonly IDataRepo _dataRepo;
	private readonly NodeFactory _nodeFactory;

	public WorkflowService(IDataRepo dataRepo, NodeFactory nodeFactory)
	{
		this._dataRepo = dataRepo;
		this._nodeFactory = nodeFactory;
	}

	public async Task<Workflow> RunWorkFlow(System.Security.Claims.ClaimsPrincipal user, string workflowId, Trigger trigger)
	{
		try
		{
			this.ValidateRequest(workflowId, trigger, out var workflow);

			Console.WriteLine($"Starting workflow: {workflowId}");
			this._dataRepo.UpdateWorkflowStatus(workflowId, WorkflowStatus.InProgress);

			await this.RunWorkflowNodes(user, workflow!, trigger);

			// Reload it
			workflow = this._dataRepo.GetWorkflowById(workflowId)!;

			if (workflow.Nodes.All(x => x.Status == NodeStatus.Completed))
				this._dataRepo.UpdateWorkflowStatus(workflowId, WorkflowStatus.Completed);

			return this._dataRepo.GetWorkflowById(workflowId)!;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowService.RunWorkFlow: {ex.Message}");
			this._dataRepo.UpdateWorkflowStatus(workflowId, WorkflowStatus.Failed);

			throw new InvalidOperationException($"Failed to run workflow {workflowId}.", ex);
		}
	}

	public Task<List<Workflow>> GetAll()
	{
		try
		{
			var workflows = this._dataRepo.GetAllWorkflows();

			if (workflows == null || !workflows.Any())
			{
				Console.WriteLine("No workflows found.");
				return Task.FromResult(new List<Workflow>());
			}

			return Task.FromResult(workflows);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowService.GetAll: {ex.Message}");
			throw new InvalidOperationException("Failed to retrieve workflows.", ex);
		}

	}

	public Task<Workflow> GetWorkflowContent(string workflowId)
	{
		try
		{
			var workflow = this._dataRepo.GetWorkflowById(workflowId);

			if (workflow == null)
			{
				Console.WriteLine("No workflow found.");
				return Task.FromResult(new Workflow());
			}

			var tasks = this._dataRepo.GetTasksByWorkflowId(workflowId);
			var notes = this._dataRepo.GetNotesByWorkflowId(workflowId);
			var attachments = this._dataRepo.GetAttachmentsByWorkflowId(workflowId);

			workflow.Content = tasks.Cast<object>().Concat(notes.Cast<object>()).Concat(attachments.Cast<object>()).ToList();

			return Task.FromResult(workflow);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowService.GetAll: {ex.Message}");
			throw new InvalidOperationException("Failed to retrieve workflows.", ex);
		}

	}

	private async Task RunWorkflowNodes(System.Security.Claims.ClaimsPrincipal user, Workflow workflow, Trigger trigger)
	{
		// To make sure we always run the init node first we do it separately from the others, unless it has already been completed or its rejected.
		var initNode = workflow.Nodes.FirstOrDefault(n => n.Type == NodeType.Init);
		if (initNode!.Status == NodeStatus.Rejected) return;

		if (initNode.Status == NodeStatus.Pending)
		{
			await this.RunNodeAsync(user, workflow.Id, initNode!, trigger);
		}

		var nodesWithoutInit = workflow.Nodes.Where(n => n.Type != NodeType.Init).ToList();

		// Group nodes by order lower to higher, so that nodes with the same order are executed in paralel
		var groupedNodes = nodesWithoutInit.GroupBy(n => n.Order).OrderBy(g => g.Key);

		foreach (var group in groupedNodes)
		{
			var lastNodes = this._dataRepo.GetNodeByWorkflowIdAndOrder(workflow.Id, group.Key - 1);

			// If the last group has had at least one uncompleted node, we stop
			if (lastNodes != null && lastNodes.Any(x => x.Status != NodeStatus.Completed))
			{
				var lastNode = lastNodes.FirstOrDefault();
				Console.WriteLine($"Stopping execution for workflow {workflow.Id} as last node {lastNode!.Id} is not completed.");
				this._dataRepo.UpdateWorkflowStatus(workflow.Id, WorkflowStatus.Failed);
				break;
			}

			Console.WriteLine($"Running nodes with order: {group.Key}");

			var taskList = new List<Task>();

			foreach (var node in group)
			{
				taskList.Add(this.RunNodeAsync(user, workflow.Id, node, trigger));
			}

			await Task.WhenAll(taskList);
		}
	}

	private async Task RunNodeAsync(System.Security.Claims.ClaimsPrincipal user, string workflowId, Node node, Trigger trigger)
	{
		var result = await Task.Run(() =>
			this._nodeFactory.Execute<string, string>(
				user,
				trigger.Message,
				node.Id,
				workflowId,
				node.Type
			)
		);

		if (result == null)
		{
			Console.WriteLine($"Node {node.Id} execution failed in workflow {workflowId}");
			this._dataRepo.UpdateNodeStatus(workflowId, node.Id, NodeStatus.Rejected);
		}
	}

	private void ValidateRequest(string workflowId, Trigger trigger, out Workflow? workflow)
	{
		if (string.IsNullOrEmpty(workflowId))
		{
			throw new ArgumentException("Workflow ID cannot be null or empty.", nameof(workflowId));
		}

		if (trigger == null)
		{
			throw new ArgumentNullException(nameof(trigger), "Trigger cannot be null.");
		}

		workflow = this._dataRepo.GetWorkflowById(workflowId);

		if (workflow == null)
		{
			throw new KeyNotFoundException($"Workflow with ID {workflowId} not found.");
		}

		if (workflow.Nodes == null || !workflow.Nodes.Any())
		{
			throw new InvalidOperationException($"Workflow with ID {workflowId} has no nodes to execute.");
		}

		if (workflow.Status != WorkflowStatus.Pending)
		{
			throw new InvalidOperationException($"Workflow with ID {workflowId} is not pending, cannot start it.");
		}
	}

	
}
