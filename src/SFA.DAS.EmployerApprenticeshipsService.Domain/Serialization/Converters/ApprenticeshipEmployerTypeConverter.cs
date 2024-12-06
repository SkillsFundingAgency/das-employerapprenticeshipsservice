using System.Text.Json;
using System.Text.Json.Serialization;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Domain.Serialization.Converters;

public class ApprenticeshipEmployerTypeConverter : JsonConverter<ApprenticeshipEmployerType>
{
    public override ApprenticeshipEmployerType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString()!.Replace(" ", "");
        return stringValue switch
        {
            "Levy" => ApprenticeshipEmployerType.Levy,
            "NonLevy" => ApprenticeshipEmployerType.NonLevy,
            "Unknown" => ApprenticeshipEmployerType.Unknown,
            _ => throw new JsonException($"Unknown value for ApprenticeshipEmployerType: {stringValue}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ApprenticeshipEmployerType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ApprenticeshipEmployerType.Levy => "Levy",
            ApprenticeshipEmployerType.NonLevy => "NonLevy",
            ApprenticeshipEmployerType.Unknown => "Unknown",
            _ => throw new JsonException($"Unknown value for ApprenticeshipEmployerType: {value}")
        };
        
        writer.WriteStringValue(stringValue);
    }
}