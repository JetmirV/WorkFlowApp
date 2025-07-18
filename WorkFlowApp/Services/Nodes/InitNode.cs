using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services.Nodes;

public class InitNode : BaseNode<string?, string?>
{
	private readonly IDataRepo _dataRepo;

	public InitNode(IDataRepo dataRepo)
	{
		this._dataRepo = dataRepo;
	}

	public override NodeType Type => NodeType.Init;

	public override string? Execute(string? _, string nodeId, string workflowId, System.Security.Claims.ClaimsPrincipal user)
	{
		try
		{
			if (!this.Validate(workflowId))
			{
				this.HandleFailure();
				return null;
			}

			Console.WriteLine($"Executing InitNode for workflow: {workflowId}");
			if (!this._dataRepo.UpdateNodeStatus(workflowId!, nodeId, NodeStatus.InProgress))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to InProgress");
				this.HandleFailure();
				return null;
			}

			// Set node status to success
			Console.WriteLine("InitNode execution successful");

			if (!this._dataRepo.UpdateNodeStatus(workflowId!, nodeId, NodeStatus.Completed))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId}");
				this.HandleFailure();
				return null;
			}

			return workflowId;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in InitNode.Execute: {ex.Message}");
			this.HandleFailure();
			return null;
		}
	}

	public override void HandleFailure()
	{
		Console.WriteLine("Failure in InitNode execution");
	}

	public override bool Validate(string? input)
	{
		Console.WriteLine("Validating InitNode with input: " + input);
		if (string.IsNullOrEmpty(input))
		{
			Console.WriteLine("Validation failed: Input is null or empty.");
			return false;
		}

		Console.WriteLine("Validation passed.");

		return true;
	}
}
