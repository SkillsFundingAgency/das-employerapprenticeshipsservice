using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Models.PAYE
{
    public class PayeView
    {
        public string Ref { get; set; }
        public string Name { get; set; }
        public long AccountId { get; set; }
        public string LegalEntityName { get; set; }
        public long LegalEntityId { get; set; }

        public DasEnglishFraction EnglishFraction { get; set; }
    }
}