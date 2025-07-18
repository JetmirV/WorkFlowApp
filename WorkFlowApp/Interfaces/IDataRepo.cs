using WorkFlowApp.Data.Entities;
using WorkFlowApp.Data.Enums;

namespace WorkFlowApp.Interfaces;

public interface IDataRepo
{
	void AddRefreshToken(RefreshToken token);
	void CreateUser(User user);
	void DeleteRefreshTokenById(long id);
	List<Workflow> GetAllWorkflows();
	List<WorkflowAttachment> GetAttachmentsByWorkflowId(string workflowId);
	List<Node> GetNodeByWorkflowIdAndOrder(string id, int v);
	List<WorkflowNote> GetNotesByWorkflowId(string workflowId);
	List<RefreshToken> GetRefreshTokenForUser(string userName);
	Role? GetRoleById(int id);
	List<WorkflowTask> GetTasksByWorkflowId(string workflowId);
	User? GetUserByEmail(string email);
	Workflow? GetWorkflowById(string id);
	bool UpdateNodeStatus(string workflowId, string nodeId, NodeStatus status);
	bool UpdateWorkflowStatus(string workflowId, WorkflowStatus status);
}
