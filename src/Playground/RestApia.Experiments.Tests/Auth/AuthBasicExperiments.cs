using FluentAssertions;
using RestApia.Extensions.Auth.Basic;
using RestApia.Shared.Common.Enums;
namespace RestApia.Experiments.Tests.Auth;

public class AuthBasicExperiments
{
    [TestCase("user", "password", "dXNlcjpwYXNzd29yZA==")]
    public async Task GetValues_Defined_Expected(string userName, string password, string expected)
    {
        // arrange
        var settings = new BasicAuthSettings
        {
            UserName = userName,
            Password = password,
        };
        var service = new BasicAuthService();

        // act
        var results = await service.GetValuesAsync(settings, Guid.Empty);

        // assert
        results.Should().NotBeNull();
        results.Should().ContainSingle();

        var result = results.First();
        result.Name.Should().Be("Authorization");
        result.Type.Should().Be(ValuesContentItemTypeEnum.Header);
        result.Value.Should().Be($"Basic {expected}");
    }
}
