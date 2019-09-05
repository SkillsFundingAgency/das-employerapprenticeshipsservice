using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferConnectionsResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}