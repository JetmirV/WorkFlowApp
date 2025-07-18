using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services.Nodes;

public class SaveMessageNode : BaseNode<string?, string?>
{
	private readonly IDataRepo _dataRepo;

	public SaveMessageNode(IDataRepo dataRepo)
	{
		this._dataRepo = dataRepo;
	}

	public override NodeType Type => NodeType.StoreMessage;

	public override string? Execute(string? incommingMessage, string nodeId, string workflowId, System.Security.Claims.ClaimsPrincipal user)
	{
		try
		{
			if (!this.Validate(incommingMessage))
			{
				this.HandleFailure();
				return null;
			}

			Console.WriteLine($"Executing SaveMessageNode for workflow: {workflowId} with input: {incommingMessage}");
			if (!this._dataRepo.UpdateNodeStatus(workflowId, nodeId, NodeStatus.InProgress))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to InProgress");
				this.HandleFailure();
				return null;
			}

			// Simulate saving the message
			var savedMessage = incommingMessage + " - Saved";

			// Set node status to success
			Console.WriteLine("SaveMessageNode execution successful");
			if (!this._dataRepo.UpdateNodeStatus(workflowId, nodeId, NodeStatus.Completed))
			{
				Console.WriteLine($"Failed to update node status for node {nodeId} in workflow {workflowId} to Completed");
				this.HandleFailure();
				return null;
			}

			return savedMessage;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in SaveMessageNode.Execute: {ex.Message}");
			this.HandleFailure();
			return null;
		}
	}

	public override void HandleFailure()
	{
		Console.WriteLine("Failure in SaveMessageNode execution");
	}

	public override bool Validate(string? input)
	{
		return !string.IsNullOrEmpty(input);
	}
}
