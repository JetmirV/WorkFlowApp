using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services.Nodes;

public class ModifyMessageNode : BaseNode<string?, string?>
{
	private readonly IDataRepo _dataRepo;

	public ModifyMessageNode(IDataRepo dataRepo)
	{
		this._dataRepo = dataRepo;
	}

	public override NodeType Type => NodeType.ModifyMessage;

	public override string? Execute(string? incommingMessage, string nodeId, string workflowId, System.Security.Claims.ClaimsPrincipal user)
	{
		try
		{
			if (!this.Validate(incommingMessage))
			{
				this.HandleFailure();
				return null;
			}

			Console.WriteLine($"Executing ModifyMessageNode for workflow: {workflowId} with input: {incommingMessage}");
			if (!this._dataRepo.UpdateNodeStatus(workflowId, nodeId, NodeStatus.InProgress))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to InProgress");
				this.HandleFailure();
				return null;
			}

			// Simulate modifying the message
			var modifiedMessage = incommingMessage + "Hello";
			
			// Set node status to success
			Console.WriteLine("ModifyMessageNode execution successful");
			if (!this._dataRepo.UpdateNodeStatus(workflowId, nodeId, NodeStatus.Completed))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to Completed");
				this.HandleFailure();
				return null;
			}

			return modifiedMessage;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in ModifyMessageNode.Execute: {ex.Message}");
			this.HandleFailure();
			return null;
		}
	}

	public override bool Validate(string? input)
	{
		return !string.IsNullOrEmpty(input);
	}

	public override void HandleFailure()
	{
		Console.WriteLine("Failure in ModifyMessageNode execution");
	}
}
