using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WorkFlowApp.Data;
using WorkFlowApp.Interfaces;
using WorkFlowApp.Models;
using WorkFlowApp.Services.Nodes;

namespace WorkFlowApp.Services;

public static class ServiceExtensions
{
	public static void ConfigureCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("CorsPolicy",
				builder => builder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());
		});
	}

	public static void ConfigureJwtServices(this IServiceCollection services, JwtTokenConfig jwtTokenConfig)
	{
		services.AddScoped<IJwtAuthManager, JwtAuthManager>();
		services.AddSingleton(jwtTokenConfig);
		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(x =>
		{
			x.RequireHttpsMetadata = true;
			x.SaveToken = true;
			x.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidIssuer = jwtTokenConfig.Issuer,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
				ValidateLifetime = true,
				ValidateAudience = false,
				ClockSkew = TimeSpan.FromMinutes(1)
			};

			x.Events = new JwtBearerEvents
			{
				OnTokenValidated = context =>
				{
					var db = context.HttpContext.RequestServices.GetRequiredService<IDataRepo>();

					var username = context.Principal?.FindFirst(ClaimTypes.Name)?.Value;

					if (username is null)
					{
						context.Fail("Unauthorized");
						return Task.CompletedTask;
					}

					var user = db.GetRefreshTokenForUser(username);

					if (user is null)
					{
						context.Fail("Unauthorized");
						return Task.CompletedTask;
					}

					return Task.CompletedTask;
				}
			};
		});

		services.AddAuthorization(options =>
		{
			options.AddPolicy("ReadScope", policy =>
				policy.RequireClaim("scope", "read"));

			options.AddPolicy("WriteScope", policy =>
				policy.RequireClaim("my-claim-type", "write"));
		});
	}

	public static void ConfigureSecondaryServices(this IServiceCollection services)
	{
		services.AddSingleton<IDataRepo, DataRepo>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IEncryptionService, EncryptionService>();
		services.AddScoped<IWorkflowService, WorkflowService>();
		services.AddScoped<IDataAggregationService, DataAggregationService>();

		// nodes
		services.AddSingleton<NodeFactory>();
		services.AddTransient<InitNode>();
		services.AddTransient<ConditionNode>();
		services.AddTransient<SaveMessageNode>();
		services.AddTransient<ModifyMessageNode>();

	}
}
