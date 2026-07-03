using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RoomsBooking.API.OpenApi;

internal sealed class BearerSecurityOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var hasAuthorize = context.Description.ActionDescriptor.EndpointMetadata
            .Any(metadata => metadata is IAuthorizeData);

        var allowAnonymous = context.Description.ActionDescriptor.EndpointMetadata
            .Any(metadata => metadata is IAllowAnonymous);

        if (!hasAuthorize || allowAnonymous) return Task.CompletedTask;
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        };

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(securityRequirement);

        return Task.CompletedTask;
    }
}