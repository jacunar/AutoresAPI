using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoresAPI.Utilities; 
public class AddParameterLlaveApi: IOperationFilter {
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter {
            Name = "X-Api-Key",
            In = ParameterLocation.Header,
            Required = false
        });
    }
}