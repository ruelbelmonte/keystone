using Keystone.Domain.Common;

namespace Keystone.Api;

/// <summary>Maps a failed Result to an HTTP problem response. Status is derived
/// from the error code, keeping HTTP concerns out of the Application layer.</summary>
public static class ResultExtensions
{
    public static IResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("A successful result cannot be a problem.");
        }

        var statusCode = result.Error.Code.EndsWith("NotFound", StringComparison.Ordinal)
            ? StatusCodes.Status404NotFound
            : StatusCodes.Status400BadRequest;

        return Results.Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Message);
    }
}
