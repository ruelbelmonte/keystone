using System.Security.Claims;
using System.Text.Json;
using Keystone.Api.Common;
using Keystone.Api.Customers.Services.Interface;
using Keystone.Contracts;
using Keystone.Contracts.Customers;
using Microsoft.AspNetCore.Authorization;

namespace Keystone.Api.Customers.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/customers").WithTags("Customers");

        group.MapGet("/{id:guid}", CustomerHandlers.GetCustomer)
            .ProducesStandardErrorResponses()
            .Produces<CustomerResponse>()
            .Produces<ApiError>(StatusCodes.Status404NotFound)
            .WithName(nameof(CustomerHandlers.GetCustomer))
            .RequireAuthorization(new AuthorizeAttribute { Policy = Roles.Readers });

        group.MapPost("/", CustomerHandlers.RegisterCustomer)
            .ProducesStandardErrorResponses()
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<ApiError>(StatusCodes.Status409Conflict)
            .RequireAuthorization(new AuthorizeAttribute { Policy = Roles.Writers });

        return routes;
    }

    internal static class CustomerHandlers
    {
        /// <summary>Gets a customer by id.</summary>
        /// <remarks>Requires the Readers policy.</remarks>
        /// <param name="id">The customer id.</param>
        /// <param name="service">The customer service.</param>
        /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
        /// <returns>200 with the customer, or 404 when no customer has that id.</returns>
        public static async Task<IResult> GetCustomer(
            Guid id,
            ICustomerService service,
            CancellationToken cancellationToken)
        {
            var result = await service.GetCustomer(id, cancellationToken);

            return result.IsSuccessful
                ? Results.Ok(result.Data)
                : Results.Json(result.Error, JsonSerializerOptions.Web, statusCode: (int)result.Error.HttpStatusCode);
        }

        /// <summary>Registers a new customer.</summary>
        /// <remarks>
        /// Requires the Writers policy. The email must not already be registered, compared
        /// case-insensitively: a duplicate found by validation returns 400, one inserted concurrently
        /// returns 409.
        /// </remarks>
        /// <param name="request">The customer to register.</param>
        /// <param name="user">The caller, recorded against the registration.</param>
        /// <param name="service">The customer service.</param>
        /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
        /// <returns>201 with the customer and its location, 400 when invalid, or 409 on a concurrent duplicate.</returns>
        public static async Task<IResult> RegisterCustomer(
            RegisterCustomerRequest request,
            ClaimsPrincipal user,
            ICustomerService service,
            CancellationToken cancellationToken)
        {
            var result = await service.RegisterCustomer(request, user, cancellationToken);

            return result.IsSuccessful
                ? Results.CreatedAtRoute(nameof(GetCustomer), new { id = result.Data.Id }, result.Data)
                : Results.Json(result.Error, JsonSerializerOptions.Web, statusCode: (int)result.Error.HttpStatusCode);
        }
    }
}
