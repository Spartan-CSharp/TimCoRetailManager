using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace TRMApi
{
	public class AuthOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{

			IEnumerable<AuthorizeAttribute> authAttributes = context.MethodInfo
			  .GetCustomAttributes(true)
			  .OfType<AuthorizeAttribute>()
			  .Distinct();

			if ( authAttributes.Any() )
			{

				_ = operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
				_ = operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

				OpenApiSecurityScheme jwtbearerScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
				};

				operation.Security =
				[
					new OpenApiSecurityRequirement
					{
						[ jwtbearerScheme ] = []
					}
				];
			}
		}
	}
}
