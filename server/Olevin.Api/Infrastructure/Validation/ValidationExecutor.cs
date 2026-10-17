using FluentValidation;
using FluentValidation.Results;
using Olevin.Api.Shared;
using Wolverine.Http;

namespace Olevin.Api.Infrastructure.Validation;

/// <summary>
/// Runs the FluentValidation validators of a request before its endpoint.
/// </summary>
public static class ValidationExecutor
{
    /// <summary>
    /// Validates the request and stops the endpoint with a bad request (400) when any rule fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TData">The type of the data the endpoint returns.</typeparam>
    /// <param name="validators">The validators of the request.</param>
    /// <param name="request">The request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The bad request with the failed rules, or a result that lets the endpoint continue.</returns>
    public static async Task<IResult> ValidateAsync<TRequest, TData>(
        IEnumerable<IValidator<TRequest>> validators,
        TRequest request,
        CancellationToken ct
    )
    {
        ValidationResult[] results = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(request, ct))
        );
        string[] errors =
        [
            .. results.SelectMany(result => result.Errors).Select(failure => failure.ErrorMessage),
        ];

        return errors.Length == 0
            ? WolverineContinue.Result()
            : TypedResults.Json(
                Response<TData>.Invalid(errors),
                statusCode: StatusCodes.Status400BadRequest
            );
    }
}
