using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions;

public class ConfigurationExtensionsTests
{
    [Test]
    [MoqInlineAutoData("true", true)]
    [MoqInlineAutoData("false", false)]
    [MoqInlineAutoData(null, false)]
    [MoqInlineAutoData("", false)]
    public void UseGovUkSignIn_WhenConfigValue_ReturnCorrectValue(string configValue, bool expected)
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(x => x["SFA.DAS.EmployerAccounts:UseGovSignIn"]).Returns(configValue);

        // Act
        var result = configuration.Object.UseGovUkSignIn();

        // Assert
        result.Should().Be(expected);
    }
}