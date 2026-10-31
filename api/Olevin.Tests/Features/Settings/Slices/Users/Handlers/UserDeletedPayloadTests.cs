using System.Text;
using Olevin.Api.Features.Settings.Slices.Users.Handlers;

namespace Olevin.Tests.Features.Settings.Slices.Users.Handlers;

public sealed class UserDeletedPayloadTests
{
    private static UserDeletedPayload? Parse(string json) =>
        UserDeletedPayload.Parse(Encoding.UTF8.GetBytes(json));

    [Fact]
    public void UserDeletedEvent_GivesTheUserIdOfTheRoute() =>
        Assert.Equal(
            "u_001",
            Parse(
                """
                {"hookId":"hook","event":"User.Deleted","method":"DELETE","status":204,"params":{"userId":"u_001"},"data":null}
                """
            )?.DeletedUserId
        );

    [Theory]
    [InlineData("""{"event":"User.Created","params":{"userId":"u_001"}}""")]
    [InlineData("""{"event":"User.Deleted","params":{}}""")]
    [InlineData("""{"event":"User.Deleted","params":{"userId":" "}}""")]
    [InlineData("""{"event":"User.Deleted"}""")]
    [InlineData("""{}""")]
    public void OtherEventOrMissingUserId_GivesNoUser(string json) =>
        Assert.Null(Parse(json)?.DeletedUserId);

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("""{"event":1}""")]
    public void InvalidBody_GivesNoPayload(string json) => Assert.Null(Parse(json));
}
