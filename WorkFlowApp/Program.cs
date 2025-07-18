using Newtonsoft.Json.Serialization;
using WorkFlowApp.Models;
using WorkFlowApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var jwtTokenConfig = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
builder.Services.ConfigureJwtServices(jwtTokenConfig!);
builder.Services.ConfigureSecondaryServices();
builder.Services.ConfigureCors();

builder.Services.AddControllers()
	.AddNewtonsoftJson(opts =>
{
	// make enums serialize as strings
	opts.SerializerSettings.Converters.Add(
	  new Newtonsoft.Json.Converters.StringEnumConverter()
	);

	opts.SerializerSettings.ContractResolver = new DefaultContractResolver
	{
		NamingStrategy = new CamelCaseNamingStrategy()
		{
			ProcessDictionaryKeys = true,
			OverrideSpecifiedNames = false
		}
	};
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
