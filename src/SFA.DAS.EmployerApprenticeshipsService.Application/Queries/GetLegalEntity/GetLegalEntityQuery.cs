using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : IAuthorizationContextModel, IAsyncRequest<GetLegalEntityResponse>
    {
        [IgnoreMap]
        [Required]
        public string AccountHashedId { get; set; }

        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [Required]
        public long? LegalEntityId { get; set; }
    }
}