using WorkFlowApp.Data.Entities;
using WorkFlowApp.Data.Enums;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Data;

public class DataRepo : IDataRepo
{
	private static readonly List<User> Users = InMemoryData.Users;
	private static readonly List<RefreshToken> RefreshTokens = InMemoryData.RefreshTokens;
	private static readonly List<Workflow> Workflows = InMemoryData.Workflows;
	private static readonly List<WorkflowTask> WorkflowTasks = InMemoryData.WorkflowTasks;
	private static readonly List<WorkflowNote> WorkflowNotes = InMemoryData.WorkflowNotes;
	private static readonly List<WorkflowAttachment> WorkflowAttachments = InMemoryData.WorkflowAttachments;
	private static readonly List<Role> Roles = InMemoryData.Roles;

	public DataRepo()
	{
		InMemoryData.Initialize();
	}

	#region RefreshToken
	//Delete a refresh token by its ID
	public void DeleteRefreshTokenById(long id)
	{
		var token = RefreshTokens.FirstOrDefault(t => t.Id == id);
		if (token != null)
		{
			RefreshTokens.Remove(token);
		}
	}

	//Get refresh tokens for a specific user
	public List<RefreshToken> GetRefreshTokenForUser(string userName)
	{
		return RefreshTokens.Where(t => t.Username == userName).ToList();
	}

	//Add refresh token for a user
	public void AddRefreshToken(RefreshToken token)
	{
		if (token != null && !RefreshTokens.Any(t => t.Id == token.Id))
		{
			RefreshTokens.Add(token);
		}
	}
	#endregion

	#region User
	//get all users
	public List<User> GetAllUsers()
	{
		return Users;
	}

	//get user by email
	public User? GetUserByEmail(string email)
	{
		return Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
	}

	//Create a new user
	public void CreateUser(User user)
	{
		if (user != null && !Users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
		{
			Users.Add(user);
		}
	}

	//get role by id
	public Role? GetRoleById(int id)
	{
		return Roles.FirstOrDefault(r => r.Id == id);
	}

	#endregion

	#region Workflow

	public List<Workflow> GetAllWorkflows()
	{
		return Workflows;
	}

	public Workflow? GetWorkflowById(string id)
	{
		return Workflows.FirstOrDefault(w => w.Id == id);
	}	

	//update node statsus by workflow id, node id and status
	public bool UpdateNodeStatus(string workflowId, string nodeId, NodeStatus status)
	{
		var workflow = this.GetWorkflowById(workflowId);

		if (workflow == null) return false;

		var node = workflow.Nodes.FirstOrDefault(n => n.Id == nodeId);

		if (node == null) return false;

		node.Status = status;
		return this.UpdateNodeStatus(workflow, node);
	}

	// Update workflow status by workflow id and status
	public bool UpdateWorkflowStatus(string workflowId, WorkflowStatus status)
	{
		var workflow = this.GetWorkflowById(workflowId);
		if (workflow == null) return false;
		workflow.Status = status;
		return true;
	}

	private bool UpdateNodeStatus(Workflow? workflow, Node node)
	{
		if (workflow == null || node == null)
			return false;
		var existingWorkflow = Workflows.FirstOrDefault(w => w.Id == workflow.Id);
		if (existingWorkflow != null)
		{
			var existingNode = existingWorkflow.Nodes.FirstOrDefault(n => n.Id == node.Id);
			if (existingNode != null)
			{
				existingNode.Status = node.Status;
			}

			return true;
		}
		else
		{
			Console.WriteLine($"Workflow with ID {workflow.Id} not found.");
			return false;
		}
	}

	public List<Node> GetNodeByWorkflowIdAndOrder(string id, int v)
	{
		var workflow = this.GetWorkflowById(id);

		if (workflow == null) return new List<Node>();

		return workflow.Nodes.Where(n => n.Order == v).ToList();
	}

	//Get tasks by workflow id
	public List<WorkflowTask> GetTasksByWorkflowId(string workflowId)
	{
		return WorkflowTasks.Where(t => t.WorkFlowId == workflowId).ToList();
	}

	//Get notes by workflow id
	public List<WorkflowNote> GetNotesByWorkflowId(string workflowId)
	{
		return WorkflowNotes.Where(n => n.WorkFlowId == workflowId).ToList();
	}

	//Get attachments by workflow id
	public List<WorkflowAttachment> GetAttachmentsByWorkflowId(string workflowId)
	{
		return WorkflowAttachments.Where(a => a.WorkFlowId == workflowId).ToList();
	}

	#endregion
}
