using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace SFA.DAS.EAS.Support.Web.Tests.Configuration;

[ExcludeFromCodeCoverage]
public class FormatSchemaProvider : JSchemaGenerationProvider
{
    public override JSchema GetSchema(JSchemaTypeGenerationContext context)
    {
        var generator = new JSchemaGenerator();
        var schema = generator.Generate(context.ObjectType, context.Required != Required.Always);

        schema.SchemaVersion = new Uri("http://json-schema.org/draft-04/schema#");
        foreach (var schemaProperty in schema.Properties)
        {
            var key = schemaProperty.Key;
            var item = schemaProperty.Value;
            if (item?.Type == null) continue;

            if (item.Type.Value.HasFlag(JSchemaType.Object))
            {
                foreach (var valueProperty in item.Properties)
                {
                    AddFormat(valueProperty, key);
                }
            }
            else if (item.Type.Value.HasFlag(JSchemaType.Boolean) ||
                     item.Type.Value.HasFlag(JSchemaType.Integer) ||
                     item.Type.Value.HasFlag(JSchemaType.Number) ||
                     item.Type.Value.HasFlag(JSchemaType.Array) ||
                     item.Type.Value.HasFlag(JSchemaType.String)
                    )
            {
                AddFormat(schemaProperty, context.ObjectType.Name);
            }
        }

        return schema;
    }

    private static void AddFormat(KeyValuePair<string, JSchema> schemaProperty, string parentName)
    {
        var item = schemaProperty.Value;
        item.Format = $"{parentName}{schemaProperty.Key}";
    }
}