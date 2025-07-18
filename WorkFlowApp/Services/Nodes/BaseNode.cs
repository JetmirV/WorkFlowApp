using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Services.Nodes;

#nullable disable
public abstract class BaseNode<TInput, TOutput> where TInput : class where TOutput : class
{
	public abstract NodeType Type { get; }

	public abstract TOutput Execute(TInput input, string nodeId, string workflowId, System.Security.Claims.ClaimsPrincipal user);

	public abstract bool Validate(TInput input);

	public abstract void HandleFailure();
}
