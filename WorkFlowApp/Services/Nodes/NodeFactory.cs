using System.Reflection;
using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Services.Nodes;

public class NodeFactory
{
	private readonly IServiceProvider _services;
	private readonly Dictionary<NodeType, Type> _map;

	public NodeFactory(IServiceProvider services, IDataRepo dataRepo)
	{
		this._services = services;
		this._map = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t =>
				!t.IsAbstract
				&& t.IsSubclassOfRawGeneric(typeof(BaseNode<,>))
			)
			// Create a “probe” instance just to read .Type
			.Select(t => {
				var ctorArgs = new object[] { dataRepo };
				var inst = (dynamic)Activator.CreateInstance(t, ctorArgs)!;
				return (NodeType: (NodeType)inst.Type, ImplType: t);
			})
			.ToDictionary(x => x.NodeType, x => x.ImplType);
	}

	public TOutput Execute<TInput, TOutput>(
		System.Security.Claims.ClaimsPrincipal user,
		TInput input,
		string nodeId,
		string workflowId,
		NodeType nodeType
	)
		where TInput : class
		where TOutput : class
	{
		if (!this._map.TryGetValue(nodeType, out var impl))
			throw new InvalidOperationException($"No node for {nodeType}");

		var node = (BaseNode<TInput, TOutput>)this._services.GetRequiredService(impl);

		return node.Execute(input, nodeId, workflowId, user);
	}
}
