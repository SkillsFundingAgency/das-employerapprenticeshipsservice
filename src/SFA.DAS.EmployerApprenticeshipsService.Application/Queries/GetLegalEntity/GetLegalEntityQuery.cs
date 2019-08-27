using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : AccountMessage
    {
        [Required]
        public long? LegalEntityId { get; set; }
    }
}