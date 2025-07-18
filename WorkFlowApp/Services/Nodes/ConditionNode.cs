using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services.Nodes;

public class ConditionNode : BaseNode<string?, string?>
{
	private readonly IDataRepo _dataRepo;

	public ConditionNode(IDataRepo dataRepo)
	{
		this._dataRepo = dataRepo;
	}

	public override NodeType Type => NodeType.Condition;

	public override string? Execute(string? _, string nodeId, string workflowId, ClaimsPrincipal user)
	{
		try
		{
			var username = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
			if (!this.Validate(workflowId) || !this.Validate(username))
			{
				this.HandleFailure();
				return null;
			}

			Console.WriteLine($"Executing ConditionNode for workflow: {workflowId} with username: {username}");
			if (!this._dataRepo.UpdateNodeStatus(workflowId!, nodeId, NodeStatus.InProgress))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to InProgress");
				this.HandleFailure();
				return null;
			}

			if (username.ToLower() != "john")
				throw new ValidationException("Condition not met: username is not John");

			// Set node status to success
			Console.WriteLine("ConditionNode execution successful");
			if (!this._dataRepo.UpdateNodeStatus(workflowId!, nodeId, NodeStatus.Completed))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to Completed");
				this.HandleFailure();
				return null;
			}

			return String.Empty;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in ConditionNode.Execute: {ex.Message}");
			if (!this._dataRepo.UpdateNodeStatus(workflowId!, nodeId, NodeStatus.Rejected))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to Failed");
			}

			this.HandleFailure();
			return null;
		}
	}

	public override bool Validate(string? input)
	{
		if (string.IsNullOrEmpty(input))
		{
			Console.WriteLine("ConditionNode validation failed: input is null or empty.");
			return false;
		}

		return true;
	}

	public override void HandleFailure()
	{
		Console.WriteLine("ConditionNode execution failed.");
	}
}
