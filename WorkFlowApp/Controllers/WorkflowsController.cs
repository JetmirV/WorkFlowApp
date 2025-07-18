using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WorkFlowApp.DTOs.Requests;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
	private readonly IWorkflowService _workflowService;

	public WorkflowsController(IWorkflowService workflowService)
	{
		this._workflowService = workflowService;
	}

	[HttpPost("{workflowId}/execute")]
	[Authorize(Policy = "WriteScope")]
	public async Task<IActionResult> ExecuteWorkflow(string workflowId, [FromBody] Trigger trigger)
	{
		if (!this.ModelState.IsValid)
			return this.BadRequest(this.ModelState);
		try
		{
			var workflow = await this._workflowService.RunWorkFlow(this.ControllerContext.HttpContext.User, workflowId, trigger);

			var response = new { workflow };

			return new JsonResult(response)
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					Formatting = Formatting.Indented,
					Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
				}
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowsController.ExecuteWorkflow: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while executing the workflow." });
		}
	}

	[HttpGet]
	[Authorize(Policy = "ReadScope")]
	public async Task<IActionResult> GetAll()
	{		
		try
		{
			var workflows = await this._workflowService.GetAll();

			var response = new { workflows };

			return new JsonResult(response)
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					Formatting = Formatting.Indented,
					Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
				}
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowsController.ExecuteWorkflow: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while executing the workflow." });
		}
	}

	[HttpGet("{workflowid}/content")]
	[Authorize(Policy = "ReadScope")]
	public async Task<IActionResult> GetWorkflowWithContent(string workflowid)
	{
		try
		{
			var workflows = await this._workflowService.GetWorkflowContent(workflowid);

			var response = new { workflows };

			return new JsonResult(response)
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					Formatting = Formatting.Indented,
					Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
				}
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Exception in WorkflowsController.ExecuteWorkflow: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while executing the workflow." });
		}
	}
}
