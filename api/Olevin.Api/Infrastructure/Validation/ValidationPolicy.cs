using FluentValidation;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using Olevin.Api.Shared;
using Wolverine.Http;
using Wolverine.Http.CodeGen;

namespace Olevin.Api.Infrastructure.Validation;

/// <summary>
/// Validates the request of every endpoint that has FluentValidation validators and answers a failed validation with
/// the <see cref="Response{T}"/> envelope that lists the failed rules.
/// </summary>
public sealed class ValidationPolicy : IHttpPolicy
{
    /// <inheritdoc />
    public void Apply(
        IReadOnlyList<HttpChain> chains,
        GenerationRules rules,
        IServiceContainer container
    )
    {
        foreach (HttpChain chain in chains)
            Apply(chain, container);
    }

    private static void Apply(HttpChain chain, IServiceContainer container)
    {
        Type? requestType = chain.HasRequestType ? chain.RequestType : chain.ComplexQueryStringType;

        if (requestType is null)
            return;

        if (!container.RegistrationsFor(typeof(IValidator<>).MakeGenericType(requestType)).Any())
            return;

        Type dataType =
            chain.ResourceType is { IsGenericType: true } resourceType
            && resourceType.GetGenericTypeDefinition() == typeof(Response<>)
                ? resourceType.GetGenericArguments()[0]
                : typeof(object);

        MethodCall validation = new(
            typeof(ValidationExecutor),
            typeof(ValidationExecutor)
                .GetMethod(nameof(ValidationExecutor.ValidateAsync))!
                .MakeGenericMethod(requestType, dataType)
        );

        chain.Metadata.Produces(
            StatusCodes.Status400BadRequest,
            typeof(Response<>).MakeGenericType(dataType)
        );
        chain.Middleware.InsertRange(
            0,
            [validation, new MaybeEndWithResultFrame(validation.ReturnVariable!)]
        );
    }
}
