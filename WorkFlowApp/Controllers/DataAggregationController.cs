using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WorkFlowApp.Interfaces;

namespace WorkFlowApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataAggregationController : ControllerBase
{
	private readonly IDataAggregationService _dataAggregationService;

	public DataAggregationController(IDataAggregationService dataAggregationService)
	{
		this._dataAggregationService = dataAggregationService;
	}

	[HttpGet("option1")]
	public async Task<IActionResult> AggregateDataOption1()
	{
		try
		{
			var result = this._dataAggregationService.GetSalesDataForOption1();

			return new JsonResult(result)
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
			Console.WriteLine($"Exception in DataAggregationController.AggregateData: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while aggregating data." });
		}
	}

	[HttpGet("option2")]
	public async Task<IActionResult> AggregateDataOption2()
	{
		try
		{
			var result = this._dataAggregationService.GetSalesDataForOption2();

			return new JsonResult(result)
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
			Console.WriteLine($"Exception in DataAggregationController.AggregateData: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while aggregating data." });
		}
	}

	[HttpGet("option3")]
	public async Task<IActionResult> AggregateDataOption3()
	{
		try
		{
			var result = this._dataAggregationService.GetSalesDataForOption3();

			return new JsonResult(result)
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
			Console.WriteLine($"Exception in DataAggregationController.AggregateData: {ex.Message}");
			return this.StatusCode(500, new { status = "error", message = "An error occurred while aggregating data." });
		}
	}
}
