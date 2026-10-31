using FluentValidation.TestHelper;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateTimezone;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.UpdateTimezone;

public sealed class UpdateTimezoneValidatorTests
{
    private readonly UpdateTimezoneValidator Validator = new();

    [Fact]
    public void IanaName_IsAccepted() =>
        Validator
            .TestValidate(new UpdateTimezoneRequest("Europe/Kyiv"))
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void UnknownTimezone_IsRejected() =>
        Validator
            .TestValidate(new UpdateTimezoneRequest("Europe/Nowhere"))
            .ShouldHaveValidationErrorFor(r => r.Timezone);
}
