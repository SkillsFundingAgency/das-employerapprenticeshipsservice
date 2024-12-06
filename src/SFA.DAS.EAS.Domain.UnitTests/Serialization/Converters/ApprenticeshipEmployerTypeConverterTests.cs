using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Serialization.Converters;

namespace SFA.DAS.EAS.Domain.UnitTests.Serialization.Converters;

[TestFixture]
public class ApprenticeshipEmployerTypeConverterTests
{
    private JsonSerializerOptions _options;

    [SetUp]
    public void SetUp()
    {
        _options = new JsonSerializerOptions
        {
            Converters = { new ApprenticeshipEmployerTypeConverter() }
        };
    }

    [TestCase("Levy", ApprenticeshipEmployerType.Levy)]
    [TestCase("Non Levy", ApprenticeshipEmployerType.NonLevy)]
    [TestCase("Unknown", ApprenticeshipEmployerType.Unknown)]
    public void Read_ValidStringValue_ReturnsEnumValue(string jsonValue, ApprenticeshipEmployerType expected)
    {
        // Arrange
        string json = $"\"{jsonValue}\"";

        // Act
        var result = JsonSerializer.Deserialize<ApprenticeshipEmployerType>(json, _options);

        // Assert
        result.Should().Be(expected);
    }

    [TestCase(ApprenticeshipEmployerType.Levy, "\"Levy\"")]
    [TestCase(ApprenticeshipEmployerType.NonLevy, "\"NonLevy\"")]
    [TestCase(ApprenticeshipEmployerType.Unknown, "\"Unknown\"")]
    public void Write_ValidEnumValue_ReturnsStringValue(ApprenticeshipEmployerType enumValue, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(enumValue, _options);

        // Assert
        json.Should().Be(expectedJson);
    }

    [Test]
    public void Read_InvalidStringValue_ThrowsJsonException()
    {
        // Arrange
        string json = "\"InvalidValue\"";

        // Act
        Action act = () => JsonSerializer.Deserialize<ApprenticeshipEmployerType>(json, _options);

        // Assert
        act.Should().Throw<JsonException>().WithMessage("Unknown value for ApprenticeshipEmployerType: InvalidValue");
    }
}
