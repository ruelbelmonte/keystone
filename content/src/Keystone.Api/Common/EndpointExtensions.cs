using Keystone.Contracts;

namespace Keystone.Api.Common;

public static class EndpointExtensions
{
    /// <summary>Declares the error responses every route can return.</summary>
    /// <remarks>
    /// Service failures return <see cref="ApiError"/>. Unhandled exceptions and malformed requests
    /// return ProblemDetails from <see cref="GlobalExceptionHandler"/>.
    /// </remarks>
    public static RouteHandlerBuilder ProducesStandardErrorResponses(this RouteHandlerBuilder builder) =>
        builder
            .Produces<ApiError>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiError>(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
