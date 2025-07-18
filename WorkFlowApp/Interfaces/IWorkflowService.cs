using WorkFlowApp.Data.Entities;
using WorkFlowApp.DTOs.Requests;

namespace WorkFlowApp.Interfaces;
public interface IWorkflowService
{
	Task<List<Workflow>> GetAll();
	Task<Workflow> GetWorkflowContent(string workflowId);
	Task<Workflow> RunWorkFlow(System.Security.Claims.ClaimsPrincipal user, string workflowId, Trigger trigger);
}
