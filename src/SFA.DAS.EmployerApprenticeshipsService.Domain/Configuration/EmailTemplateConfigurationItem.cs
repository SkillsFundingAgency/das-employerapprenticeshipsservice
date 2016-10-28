using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public enum EmailTemplateType
    {
        None,
        Invitation
    }

    public class EmailTemplateConfigurationItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EmailTemplateType TemplateType { get; set; }
        public string TemplateName { get; set; }
        public string Key { get; set; }
    }
}