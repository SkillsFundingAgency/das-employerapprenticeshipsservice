using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : AccountMessage, IAsyncRequest<GetLegalEntityResponse>
    {
        [Required]
        public long? LegalEntityId { get; set; }
    }
}